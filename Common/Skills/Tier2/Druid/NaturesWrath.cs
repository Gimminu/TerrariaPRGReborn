using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using RpgMod.Common.Base;

namespace RpgMod.Common.Skills.Tier2.Druid
{
    /// <summary>
    /// Nature's Wrath - Druid's AoE nature damage skill.
    /// </summary>
    public class NaturesWrath : BaseSkill
    {
        public override string InternalName => "NaturesWrath";
        public override string DisplayName => "Nature's Wrath";
        public override string Description => "Call upon nature to damage all nearby enemies.";

        public override SkillType SkillType => SkillType.Active;
        public override JobType RequiredJob => JobType.Druid;
        public override int RequiredLevel => RpgConstants.SECOND_JOB_LEVEL + 10;
        public override int SkillPointCost => 1;
        public override int MaxRank => 5;
        public override string IconTexture => $"{RpgConstants.ICON_PATH}Skills/NaturesWrath";
        public override float CooldownSeconds => 15f;
        public override int ResourceCost => 25;
        public override ResourceType ResourceType => ResourceType.Mana;

        private static readonly int[] DAMAGE = { 30, 42, 55, 70, 95 };
        private static readonly float[] RADIUS = { 120f, 140f, 160f, 180f, 220f };

        protected override void OnActivate(Player player)
        {
            int rank = Math.Max(1, CurrentRank);
            int damage = DAMAGE[rank - 1];
            float radius = RADIUS[rank - 1];

            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC npc = Main.npc[i];
                if (!npc.active || npc.friendly || npc.lifeMax <= 5) continue;
                if (Vector2.Distance(npc.Center, player.Center) > radius) continue;

                int dir = npc.Center.X >= player.Center.X ? 1 : -1;
                float scaledDamage = GetScaledDamage(player, DamageClass.Magic, damage);
                bool crit = RollCrit(player, DamageClass.Magic);
                int finalDamage = ApplyDamageVariance(player, scaledDamage);
                player.ApplyDamageToNPC(npc, finalDamage, 4f, dir, crit, DamageClass.Magic, false);
                npc.AddBuff(BuffID.Poisoned, 180);
            }

            PlayEffects(player, radius);
        }

        private void PlayEffects(Player player, float radius)
        {
            SoundEngine.PlaySound(SoundID.Grass, player.position);
            for (int i = 0; i < 40; i++)
            {
                float angle = MathHelper.TwoPi * i / 40f;
                Vector2 dir = new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle));
                Dust dust = Dust.NewDustDirect(player.Center + dir * radius * 0.6f, 6, 6,
                    DustID.GrassBlades, dir.X * 2f, dir.Y * 2f, 100, Color.Green, 1.6f);
                dust.noGravity = true;
            }
        }
    }
}
