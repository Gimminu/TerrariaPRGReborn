using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using RpgMod.Common.Base;

namespace RpgMod.Common.Skills.Tier2.Archmage
{
    /// <summary>
    /// Meteor - Archmage's powerful AoE spell.
    /// </summary>
    public class Meteor : BaseSkill
    {
        public override string InternalName => "Meteor";
        public override string DisplayName => "Meteor";
        public override string Description => "Call down a devastating meteor from the sky.";

        public override SkillType SkillType => SkillType.Active;
        public override JobType RequiredJob => JobType.Archmage;
        public override int RequiredLevel => RpgConstants.SECOND_JOB_LEVEL;
        public override int SkillPointCost => 1;
        public override int MaxRank => 5;
        public override string IconTexture => $"{RpgConstants.ICON_PATH}Skills/Meteor";
        public override float CooldownSeconds => 20f;
        public override int ResourceCost => 35;
        public override ResourceType ResourceType => ResourceType.Mana;

        private static readonly int[] DAMAGE = { 60, 80, 105, 135, 180 };
        private static readonly float[] RADIUS = { 120f, 140f, 160f, 180f, 220f };

        protected override void OnActivate(Player player)
        {
            int rank = Math.Max(1, CurrentRank);
            int damage = DAMAGE[rank - 1];
            float radius = RADIUS[rank - 1];

            Vector2 impactPoint = Main.MouseWorld;

            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC npc = Main.npc[i];
                if (!npc.active || npc.friendly || npc.lifeMax <= 5) continue;
                if (Vector2.Distance(npc.Center, impactPoint) > radius) continue;

                int dir = npc.Center.X >= impactPoint.X ? 1 : -1;
                float scaledDamage = GetScaledDamage(player, DamageClass.Magic, damage);
                bool crit = RollCrit(player, DamageClass.Magic);
                int finalDamage = ApplyDamageVariance(player, scaledDamage);
                player.ApplyDamageToNPC(npc, finalDamage, 8f, dir, crit, DamageClass.Magic, false);
                npc.AddBuff(BuffID.OnFire, 300);
            }

            PlayMeteorEffects(impactPoint, radius);
        }

        private void PlayMeteorEffects(Vector2 center, float radius)
        {
            SoundEngine.PlaySound(SoundID.Item14, center);
            for (int i = 0; i < 60; i++)
            {
                float angle = MathHelper.TwoPi * i / 60f;
                Vector2 dir = new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle));
                float dist = Main.rand.NextFloat(radius);
                Dust dust = Dust.NewDustDirect(center + dir * dist, 8, 8,
                    DustID.Torch, dir.X * 6f, dir.Y * 6f, 100, Color.OrangeRed, 2.5f);
                dust.noGravity = true;
            }
            for (int i = 0; i < 20; i++)
            {
                Dust smoke = Dust.NewDustDirect(center + Main.rand.NextVector2Circular(radius, radius), 12, 12,
                    DustID.Smoke, 0, -3f, 150, Color.DarkGray, 2f);
                smoke.noGravity = true;
            }
        }
    }
}
