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
    /// Fan the Hammer - Gunslinger's rapid multi-shot skill.
    /// </summary>
    public class FanTheHammer : BaseSkill
    {
        public override string InternalName => "FanTheHammer";
        public override string DisplayName => "Fan the Hammer";
        public override string Description => "Rapidly fire multiple shots at nearby enemies.";

        public override SkillType SkillType => SkillType.Active;
        public override JobType RequiredJob => JobType.Gunslinger;
        public override int RequiredLevel => RpgConstants.SECOND_JOB_LEVEL;
        public override int SkillPointCost => 1;
        public override int MaxRank => 5;
        public override string IconTexture => $"{RpgConstants.ICON_PATH}Skills/FanTheHammer";
        public override float CooldownSeconds => 14f;
        public override int ResourceCost => 20;
        public override ResourceType ResourceType => ResourceType.Stamina;

        private static readonly int[] SHOT_COUNT = { 3, 4, 5, 6, 8 };
        private static readonly int[] DAMAGE_PER_SHOT = { 15, 18, 22, 26, 32 };
        private static readonly float RANGE = 300f;

        protected override void OnActivate(Player player)
        {
            int rank = Math.Max(1, CurrentRank);
            int shots = SHOT_COUNT[rank - 1];
            int damage = DAMAGE_PER_SHOT[rank - 1];

            NPC[] enemies = FindNearestEnemies(player, RANGE, shots);
            int hitCount = 0;

            foreach (NPC enemy in enemies)
            {
                if (enemy == null) continue;
                int dir = enemy.Center.X >= player.Center.X ? 1 : -1;
                float scaledDamage = GetScaledDamage(player, DamageClass.Ranged, damage);
                int finalDamage = ApplyDamageVariance(player, scaledDamage);
                bool crit = RollCrit(player, DamageClass.Ranged);
                player.ApplyDamageToNPC(enemy, finalDamage, 2f, dir, crit, DamageClass.Ranged, false);
                CreateBulletTrail(player, enemy);
                hitCount++;
            }

            PlayEffects(player);
            ShowMessage(player, $"{hitCount} Shots!", Color.Orange);
        }

        private NPC[] FindNearestEnemies(Player player, float range, int count)
        {
            var enemies = new System.Collections.Generic.List<(NPC npc, float dist)>();
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC npc = Main.npc[i];
                if (!npc.active || npc.friendly || npc.lifeMax <= 5) continue;
                float dist = Vector2.Distance(npc.Center, player.Center);
                if (dist <= range)
                    enemies.Add((npc, dist));
            }

            enemies.Sort((a, b) => a.dist.CompareTo(b.dist));
            NPC[] result = new NPC[count];
            for (int i = 0; i < count && i < enemies.Count; i++)
                result[i] = enemies[i].npc;
            return result;
        }

        private void CreateBulletTrail(Player player, NPC target)
        {
            Vector2 dir = (target.Center - player.Center).SafeNormalize(Vector2.Zero);
            for (int i = 0; i < 10; i++)
            {
                Vector2 pos = player.Center + dir * (i * 30);
                Dust dust = Dust.NewDustDirect(pos, 4, 4, DustID.Torch, dir.X * 5f, dir.Y * 5f, 100, default, 0.8f);
                dust.noGravity = true;
            }
        }

        private void PlayEffects(Player player)
        {
            for (int i = 0; i < 3; i++)
                SoundEngine.PlaySound(SoundID.Item11, player.position);
        }

        private void ShowMessage(Player player, string text, Color color)
        {
            if (Main.netMode != NetmodeID.Server)
                CombatText.NewText(player.Hitbox, color, text);
        }
    }
}
