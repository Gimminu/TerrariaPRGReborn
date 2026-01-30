using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using Rpg.Common.Base;

namespace Rpg.Common.Skills.Tier3.Gunmaster
{
    /// <summary>
    /// Bullet Storm - Gunmaster's ultimate multi-shot skill.
    /// </summary>
    public class BulletStorm : BaseSkill
    {
        public override string InternalName => "BulletStorm";
        public override string DisplayName => "Bullet Storm";
        public override string Description => "Unleash a devastating barrage of bullets at all nearby enemies.";

        public override SkillType SkillType => SkillType.Active;
        public override JobType RequiredJob => JobType.Gunmaster;
        public override int RequiredLevel => RpgConstants.THIRD_JOB_LEVEL;
        public override int SkillPointCost => 2;
        public override int MaxRank => 5;
        public override string IconTexture => $"{RpgConstants.ICON_PATH}Skills/BulletStorm";
        public override float CooldownSeconds => 22f;
        public override int ResourceCost => 35;
        public override ResourceType ResourceType => ResourceType.Stamina;

        private static readonly int[] DAMAGE = { 35, 48, 62, 80, 110 };
        private static readonly int[] HITS = { 5, 6, 7, 8, 10 };
        private static readonly float RANGE = 350f;

        protected override void OnActivate(Player player)
        {
            int rank = Math.Max(1, CurrentRank);
            int damage = DAMAGE[rank - 1];
            int hits = HITS[rank - 1];

            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC npc = Main.npc[i];
                if (!npc.active || npc.friendly || npc.lifeMax <= 5) continue;
                if (Vector2.Distance(npc.Center, player.Center) > RANGE) continue;

                for (int h = 0; h < hits / 2; h++)
                {
                    int dir = npc.Center.X >= player.Center.X ? 1 : -1;
                    bool crit = Main.rand.NextFloat(100f) < player.GetCritChance(DamageClass.Ranged);
                    player.ApplyDamageToNPC(npc, damage, 3f, dir, crit, DamageClass.Ranged, false);
                }
                CreateBulletEffect(player, npc);
            }

            PlayEffects(player);
        }

        private void CreateBulletEffect(Player player, NPC target)
        {
            Vector2 dir = (target.Center - player.Center).SafeNormalize(Vector2.Zero);
            for (int i = 0; i < 8; i++)
            {
                Vector2 pos = player.Center + dir * (i * 40);
                Dust dust = Dust.NewDustDirect(pos, 4, 4, DustID.Torch, dir.X * 8f, dir.Y * 8f, 100, Color.Orange, 1.2f);
                dust.noGravity = true;
            }
        }

        private void PlayEffects(Player player)
        {
            for (int i = 0; i < 6; i++)
                SoundEngine.PlaySound(SoundID.Item11, player.position);
        }
    }
}
