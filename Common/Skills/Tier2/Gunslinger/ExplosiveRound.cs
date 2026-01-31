using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using RpgMod.Common.Base;

namespace RpgMod.Common.Skills.Tier2.Gunslinger
{
    /// <summary>
    /// Explosive Round - Gunslinger's AoE damage skill.
    /// </summary>
    public class ExplosiveRound : BaseSkill
    {
        public override string InternalName => "ExplosiveRound";
        public override string DisplayName => "Explosive Round";
        public override string Description => "Fire an explosive bullet that damages all nearby enemies.";

        public override SkillType SkillType => SkillType.Active;
        public override JobType RequiredJob => JobType.Gunslinger;
        public override int RequiredLevel => RpgConstants.SECOND_JOB_LEVEL + 10;
        public override int SkillPointCost => 1;
        public override int MaxRank => 5;
        public override string IconTexture => $"{RpgConstants.ICON_PATH}Skills/ExplosiveRound";
        public override float CooldownSeconds => 18f;
        public override int ResourceCost => 25;
        public override ResourceType ResourceType => ResourceType.Stamina;

        private static readonly int[] DAMAGE = { 40, 55, 70, 90, 120 };
        private static readonly float[] EXPLOSION_RADIUS = { 80f, 100f, 120f, 140f, 180f };

        protected override void OnActivate(Player player)
        {
            int rank = Math.Max(1, CurrentRank);
            int damage = DAMAGE[rank - 1];
            float radius = EXPLOSION_RADIUS[rank - 1];

            Vector2 explosionCenter = Main.MouseWorld;

            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC npc = Main.npc[i];
                if (!npc.active || npc.friendly || npc.lifeMax <= 5) continue;
                if (Vector2.Distance(npc.Center, explosionCenter) > radius) continue;

                int dir = npc.Center.X >= player.Center.X ? 1 : -1;
                float scaledDamage = GetScaledDamage(player, DamageClass.Ranged, damage);
                int finalDamage = ApplyDamageVariance(player, scaledDamage);
                bool crit = RollCrit(player, DamageClass.Ranged);
                player.ApplyDamageToNPC(npc, finalDamage, 6f, dir, crit, DamageClass.Ranged, false);
            }

            PlayExplosionEffects(explosionCenter, radius);
        }

        private void PlayExplosionEffects(Vector2 center, float radius)
        {
            SoundEngine.PlaySound(SoundID.DD2_ExplosiveTrapExplode, center);
            for (int i = 0; i < 40; i++)
            {
                float angle = MathHelper.TwoPi * i / 40f;
                Vector2 dir = new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle));
                Dust dust = Dust.NewDustDirect(center + dir * Main.rand.NextFloat(radius * 0.5f), 8, 8,
                    DustID.Torch, dir.X * 4f, dir.Y * 4f, 100, Color.Orange, 2f);
                dust.noGravity = true;
            }
        }
    }
}
