using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using RpgMod.Common.Base;

namespace RpgMod.Common.Skills.Tier3.LichKing
{
    /// <summary>
    /// Soul Harvest - Lich King's signature skill.
    /// Drain souls from enemies to heal yourself.
    /// </summary>
    public class SoulHarvest : BaseSkill
    {
        public override string InternalName => "LichKingSoulHarvest";
        public override string DisplayName => "Soul Harvest";
        public override string Description => "Drain souls from nearby enemies to heal yourself.";

        public override SkillType SkillType => SkillType.Active;
        public override JobType RequiredJob => JobType.LichKing;
        public override int RequiredLevel => RpgConstants.THIRD_JOB_LEVEL;
        public override int SkillPointCost => 1;
        public override int MaxRank => 5;
        public override string IconTexture => $"{RpgConstants.ICON_PATH}Skills/SoulHarvest";
        public override float CooldownSeconds => 20f;
        public override int ResourceCost => 30;
        public override ResourceType ResourceType => ResourceType.Mana;

        // Scaling per rank
        private static readonly int[] BASE_DAMAGE = { 40, 55, 70, 90, 120 };
        private static readonly float[] RADIUS = { 130f, 150f, 170f, 190f, 220f };
        private static readonly float[] HEAL_PER_HIT = { 0.03f, 0.04f, 0.05f, 0.06f, 0.08f };
        private static readonly int[] FLAT_HEAL = { 15, 20, 25, 30, 40 };

        protected override void OnActivate(Player player)
        {
            int rank = Math.Max(1, CurrentRank);
            int baseDamage = BASE_DAMAGE[rank - 1];
            float radius = RADIUS[rank - 1];
            float healPercent = HEAL_PER_HIT[rank - 1];
            int flatHeal = FLAT_HEAL[rank - 1];

            int hitCount = 0;
            int totalHeal = 0;

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

                float scaledDamage = GetScaledDamage(player, DamageClass.Magic, baseDamage);
                int damage = ApplyDamageVariance(player, scaledDamage);
                player.ApplyDamageToNPC(npc, damage, 2f, hitDirection, crit, DamageClass.Magic, false);
                
                // Heal per hit
                int heal = flatHeal + (int)(player.statLifeMax2 * healPercent);
                totalHeal += heal;
                hitCount++;

                CreateDrainEffect(player, npc);
            }

            // Apply healing
            if (totalHeal > 0)
            {
                player.statLife = Math.Min(player.statLife + totalHeal, player.statLifeMax2);
            }

            PlaySkillEffects(player, radius);

            if (hitCount > 0)
            {
                ShowMessage(player, $"Soul Harvest! +{totalHeal} HP", Color.MediumPurple);
            }
        }

        private void CreateDrainEffect(Player player, NPC target)
        {
            // Soul drain trail
            Vector2 direction = (player.Center - target.Center).SafeNormalize(Vector2.Zero);
            float distance = Vector2.Distance(player.Center, target.Center);

            for (int i = 0; i < distance; i += 15)
            {
                Vector2 pos = target.Center + direction * i;
                
                Dust dust = Dust.NewDustDirect(
                    pos,
                    4,
                    4,
                    DustID.Wraith,
                    direction.X * 3f,
                    direction.Y * 3f,
                    100,
                    Color.Purple,
                    1.2f
                );
                dust.noGravity = true;
            }
        }

        private void PlaySkillEffects(Player player, float radius)
        {
            SoundEngine.PlaySound(SoundID.NPCDeath6 with { Pitch = -0.5f }, player.position);

            // Dark soul ring
            int particleCount = (int)(radius / 5f);
            for (int i = 0; i < particleCount; i++)
            {
                float angle = MathHelper.TwoPi * i / particleCount;
                Vector2 direction = new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle));
                Vector2 pos = player.Center + direction * radius;

                Dust dust = Dust.NewDustDirect(
                    pos,
                    4,
                    4,
                    DustID.Shadowflame,
                    -direction.X * 3f,
                    -direction.Y * 3f,
                    100,
                    Color.Purple,
                    1.5f
                );
                dust.noGravity = true;
            }

            // Center vortex
            for (int i = 0; i < 30; i++)
            {
                float angle = MathHelper.TwoPi * i / 30f;
                Vector2 offset = new Vector2(
                    (float)Math.Cos(angle) * 30f,
                    (float)Math.Sin(angle) * 30f
                );

                Dust dust = Dust.NewDustDirect(
                    player.Center + offset,
                    4,
                    4,
                    DustID.Wraith,
                    0,
                    0,
                    100,
                    Color.DarkMagenta,
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
