using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using Rpg.Common.Base;

namespace Rpg.Common.Skills.Tier1.Mage
{
    /// <summary>
    /// Fireball - Mage's signature AoE damage skill.
    /// Explodes arcane fire around the caster.
    /// </summary>
    public class Fireball : BaseSkill
    {
        public override string InternalName => "Fireball";
        public override string DisplayName => "Fireball";
        public override string Description => "Explode arcane fire around you, damaging all nearby enemies.";

        public override SkillType SkillType => SkillType.Active;
        public override JobType RequiredJob => JobType.Mage;
        public override int RequiredLevel => RpgConstants.FIRST_JOB_LEVEL;
        public override int SkillPointCost => 1;
        public override int MaxRank => 5;
        public override string IconTexture => $"{RpgConstants.ICON_PATH}Skills/Fireball";
        public override float CooldownSeconds => 6f;
        public override int ResourceCost => 20;
        public override ResourceType ResourceType => ResourceType.Mana;

        // Scaling per rank
        private static readonly int[] BASE_DAMAGE = { 35, 45, 55, 70, 90 };
        private static readonly float[] RADIUS = { 100f, 120f, 140f, 160f, 180f };
        private static readonly float[] MAGIC_SCALING = { 0.8f, 0.9f, 1.0f, 1.1f, 1.3f };

        protected override void OnActivate(Player player)
        {
            int rank = Math.Max(1, CurrentRank);
            int baseDamage = BASE_DAMAGE[rank - 1];
            float radius = RADIUS[rank - 1];
            float magicScaling = MAGIC_SCALING[rank - 1];

            // Calculate total damage with magic damage bonus
            float magicDamageMultiplier = player.GetDamage(DamageClass.Magic).Additive;
            int totalDamage = (int)(baseDamage * (1f + (magicDamageMultiplier - 1f) * magicScaling));

            int hitCount = 0;

            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC npc = Main.npc[i];
                if (!npc.active || npc.friendly || npc.lifeMax <= 5)
                    continue;

                float distance = Vector2.Distance(npc.Center, player.Center);
                if (distance > radius)
                    continue;

                int hitDirection = npc.Center.X >= player.Center.X ? 1 : -1;
                float critChance = player.GetCritChance(DamageClass.Magic);
                bool crit = Main.rand.NextFloat(100f) < critChance;

                int damage = Main.DamageVar(totalDamage, player.luck);
                if (crit) damage = (int)(damage * 1.5f);

                player.ApplyDamageToNPC(npc, damage, 4f, hitDirection, crit, DamageClass.Magic, false);
                hitCount++;

                // Fire hit effect
                CreateHitEffect(npc);
            }

            PlaySkillEffects(player, radius);

            if (hitCount > 0)
            {
                ShowMessage(player, $"Fireball! ({hitCount} hit)", Color.Orange);
            }
        }

        private void CreateHitEffect(NPC target)
        {
            for (int d = 0; d < 8; d++)
            {
                Dust dust = Dust.NewDustDirect(
                    target.position,
                    target.width,
                    target.height,
                    DustID.Torch,
                    Main.rand.NextFloat(-4f, 4f),
                    Main.rand.NextFloat(-4f, 4f),
                    100,
                    default,
                    2f
                );
                dust.noGravity = true;
            }
        }

        private void PlaySkillEffects(Player player, float radius)
        {
            SoundEngine.PlaySound(SoundID.Item45, player.position);

            // Expanding fire ring
            int particleCount = (int)(radius / 5f);
            for (int i = 0; i < particleCount; i++)
            {
                float angle = MathHelper.TwoPi * i / particleCount;
                Vector2 direction = new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle));

                for (int j = 0; j < 3; j++)
                {
                    Vector2 pos = player.Center + direction * (20f + j * 30f);
                    
                    Dust dust = Dust.NewDustDirect(
                        pos,
                        4,
                        4,
                        DustID.Torch,
                        direction.X * 3f,
                        direction.Y * 3f,
                        100,
                        default,
                        1.5f
                    );
                    dust.noGravity = true;
                }
            }

            // Center explosion
            for (int i = 0; i < 30; i++)
            {
                Vector2 velocity = Main.rand.NextVector2Circular(6f, 6f);
                
                Dust dust = Dust.NewDustDirect(
                    player.Center,
                    4,
                    4,
                    DustID.SolarFlare,
                    velocity.X,
                    velocity.Y,
                    100,
                    default,
                    1.8f
                );
                dust.noGravity = true;
            }
        }

        private void ShowMessage(Player player, string text, Color color)
        {
            if (Main.netMode != NetmodeID.Server)
            {
                CombatText.NewText(player.Hitbox, color, text, false, false);
            }
        }
    }
}
