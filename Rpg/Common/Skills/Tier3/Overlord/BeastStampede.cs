using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using Rpg.Common.Base;

namespace Rpg.Common.Skills.Tier3.Overlord
{
    /// <summary>
    /// Beast Stampede - Overlord's AoE damage skill.
    /// </summary>
    public class BeastStampede : BaseSkill
    {
        public override string InternalName => "BeastStampede";
        public override string DisplayName => "Beast Stampede";
        public override string Description => "Command your beasts to stampede, damaging all enemies in their path.";

        public override SkillType SkillType => SkillType.Active;
        public override JobType RequiredJob => JobType.Overlord;
        public override int RequiredLevel => RpgConstants.THIRD_JOB_LEVEL + 10;
        public override int SkillPointCost => 2;
        public override int MaxRank => 5;
        public override string IconTexture => $"{RpgConstants.ICON_PATH}Skills/BeastStampede";
        public override float CooldownSeconds => 20f;
        public override int ResourceCost => 30;
        public override ResourceType ResourceType => ResourceType.Mana;

        private static readonly int[] DAMAGE = { 50, 70, 95, 125, 170 };
        private static readonly float[] RADIUS = { 160f, 190f, 220f, 250f, 300f };

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
                bool crit = Main.rand.NextFloat(100f) < player.GetCritChance(DamageClass.Summon);
                player.ApplyDamageToNPC(npc, damage, 8f, dir, crit, DamageClass.Summon, false);
                CreateStampedeEffect(npc);
            }

            PlayEffects(player, radius);
        }

        private void CreateStampedeEffect(NPC target)
        {
            for (int i = 0; i < 10; i++)
            {
                Dust dust = Dust.NewDustDirect(target.position, target.width, target.height,
                    DustID.Dirt, Main.rand.NextFloat(-5f, 5f), Main.rand.NextFloat(-3f, 0f), 100, default, 1.5f);
                dust.noGravity = true;
            }
        }

        private void PlayEffects(Player player, float radius)
        {
            SoundEngine.PlaySound(SoundID.Roar, player.position);
            SoundEngine.PlaySound(SoundID.DD2_ExplosiveTrapExplode, player.position);
            for (int i = 0; i < 50; i++)
            {
                float angle = MathHelper.TwoPi * i / 50f;
                Vector2 dir = new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle));
                Dust dust = Dust.NewDustDirect(player.Center + dir * radius * 0.5f, 8, 8,
                    DustID.Dirt, dir.X * 6f, dir.Y * 6f, 100, default, 2f);
                dust.noGravity = true;
            }
        }
    }
}
