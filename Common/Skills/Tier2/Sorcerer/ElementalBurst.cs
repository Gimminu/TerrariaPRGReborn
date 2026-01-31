using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using RpgMod.Common.Base;

namespace RpgMod.Common.Skills.Tier2.Sorcerer
{
    /// <summary>
    /// Elemental Burst - Sorcerer's powerful AoE.
    /// Releases volatile elemental energy around the caster.
    /// </summary>
    public class ElementalBurst : BaseSkill
    {
        public override string InternalName => "ElementalBurst";
        public override string DisplayName => "Elemental Burst";
        public override string Description => "Release volatile elemental energy, damaging all nearby foes.";

        public override SkillType SkillType => SkillType.Active;
        public override JobType RequiredJob => JobType.Sorcerer;
        public override int RequiredLevel => RpgConstants.SECOND_JOB_LEVEL;
        public override int SkillPointCost => 1;
        public override int MaxRank => 5;
        public override string IconTexture => $"{RpgConstants.ICON_PATH}Skills/ElementalBurst";
        public override float CooldownSeconds => 12f;
        public override int ResourceCost => 25;
        public override ResourceType ResourceType => ResourceType.Mana;

        // Scaling per rank
        private static readonly int[] BASE_DAMAGE = { 45, 60, 75, 95, 120 };
        private static readonly float[] RADIUS = { 130f, 150f, 170f, 190f, 220f };
        private static readonly float[] MAGIC_SCALING = { 1.0f, 1.1f, 1.2f, 1.3f, 1.5f };

        protected override void OnActivate(Player player)
        {
            int rank = Math.Max(1, CurrentRank);
            int baseDamage = BASE_DAMAGE[rank - 1];
            float radius = RADIUS[rank - 1];
            float magicScaling = MAGIC_SCALING[rank - 1];

            // Calculate damage with magic scaling
            float scaledDamage = GetScaledDamage(player, DamageClass.Magic, baseDamage);
            float totalDamage = baseDamage + (scaledDamage - baseDamage) * magicScaling;

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
                bool crit = RollCrit(player, DamageClass.Magic);

                int damage = ApplyDamageVariance(player, totalDamage);
                player.ApplyDamageToNPC(npc, damage, 5f, hitDirection, crit, DamageClass.Magic, false);
                hitCount++;

                CreateHitEffect(npc);
            }

            PlaySkillEffects(player, radius);

            if (hitCount > 0)
            {
                ShowMessage(player, $"Elemental Burst! ({hitCount} hit)", Color.Cyan);
            }
        }

        private void CreateHitEffect(NPC target)
        {
            int[] dustTypes = { DustID.IceTorch, DustID.Torch, DustID.Electric };
            int dustType = dustTypes[Main.rand.Next(dustTypes.Length)];

            for (int d = 0; d < 10; d++)
            {
                Dust dust = Dust.NewDustDirect(
                    target.position,
                    target.width,
                    target.height,
                    dustType,
                    Main.rand.NextFloat(-5f, 5f),
                    Main.rand.NextFloat(-5f, 5f),
                    100,
                    default,
                    1.8f
                );
                dust.noGravity = true;
            }
        }

        private void PlaySkillEffects(Player player, float radius)
        {
            SoundEngine.PlaySound(SoundID.Item122, player.position);

            // Multi-element ring
            int particleCount = (int)(radius / 4f);
            for (int i = 0; i < particleCount; i++)
            {
                float angle = MathHelper.TwoPi * i / particleCount;
                Vector2 direction = new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle));

                int[] dustTypes = { DustID.IceTorch, DustID.Torch, DustID.Electric };
                int dustType = dustTypes[i % dustTypes.Length];

                Vector2 pos = player.Center + direction * radius * 0.7f;
                
                Dust dust = Dust.NewDustDirect(
                    pos,
                    4,
                    4,
                    dustType,
                    direction.X * 4f,
                    direction.Y * 4f,
                    100,
                    default,
                    1.5f
                );
                dust.noGravity = true;
            }

            // Center burst
            for (int i = 0; i < 40; i++)
            {
                int[] dustTypes = { DustID.IceTorch, DustID.Torch, DustID.Electric };
                int dustType = dustTypes[i % dustTypes.Length];

                Vector2 velocity = Main.rand.NextVector2Circular(8f, 8f);
                
                Dust dust = Dust.NewDustDirect(
                    player.Center,
                    4,
                    4,
                    dustType,
                    velocity.X,
                    velocity.Y,
                    100,
                    default,
                    2f
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
