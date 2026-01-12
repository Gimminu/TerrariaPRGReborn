using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using Rpg.Common.Base;
using Rpg.Common.Players;

namespace Rpg.Common.Skills.Tier2.Assassin
{
    /// <summary>
    /// Backstab - Assassin's high crit damage skill.
    /// </summary>
    public class Backstab : BaseSkill
    {
        public override string InternalName => "Backstab";
        public override string DisplayName => "Backstab";
        public override string Description => "Strike from the shadows with a guaranteed critical hit.";

        public override SkillType SkillType => SkillType.Active;
        public override JobType RequiredJob => JobType.Assassin;
        public override int RequiredLevel => RpgConstants.SECOND_JOB_LEVEL;
        public override int SkillPointCost => 1;
        public override int MaxRank => 5;
        public override string IconTexture => $"{RpgConstants.ICON_PATH}Skills/Backstab";
        public override float CooldownSeconds => 12f;
        public override int ResourceCost => 20;
        public override ResourceType ResourceType => ResourceType.Stamina;

        private static readonly int[] DAMAGE = { 45, 60, 80, 100, 130 };
        private static readonly float[] CRIT_MULTIPLIER = { 1.5f, 1.6f, 1.7f, 1.8f, 2.0f };

        protected override void OnActivate(Player player)
        {
            int rank = System.Math.Max(1, CurrentRank);
            int baseDamage = DAMAGE[rank - 1];
            float critMult = CRIT_MULTIPLIER[rank - 1];

            NPC nearestEnemy = FindNearestEnemy(player, 200f);
            if (nearestEnemy != null)
            {
                int finalDamage = (int)(baseDamage * critMult);
                int dir = nearestEnemy.Center.X >= player.Center.X ? 1 : -1;
                player.ApplyDamageToNPC(nearestEnemy, finalDamage, 5f, dir, true, DamageClass.Melee, false);
                PlayEffects(player, nearestEnemy);
                ShowMessage(nearestEnemy, $"-{finalDamage} CRIT!", Color.Red);
            }
            else
            {
                var rpgPlayer = player.GetModPlayer<RpgPlayer>();
                rpgPlayer.AddTemporaryDamage(0.20f + rank * 0.05f, 180);
                ShowMessage(player, "Backstab Ready!", Color.Orange);
            }
        }

        private NPC FindNearestEnemy(Player player, float range)
        {
            NPC nearest = null;
            float nearestDist = range;

            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC npc = Main.npc[i];
                if (!npc.active || npc.friendly || npc.lifeMax <= 5) continue;
                float dist = Vector2.Distance(npc.Center, player.Center);
                if (dist < nearestDist)
                {
                    nearestDist = dist;
                    nearest = npc;
                }
            }
            return nearest;
        }

        private void PlayEffects(Player player, NPC target)
        {
            SoundEngine.PlaySound(SoundID.DD2_DarkMageHurt, target.position);
            for (int i = 0; i < 15; i++)
            {
                Dust dust = Dust.NewDustDirect(target.position, target.width, target.height,
                    DustID.Smoke, Main.rand.NextFloat(-3f, 3f), Main.rand.NextFloat(-3f, 3f), 100, Color.DarkGray, 1.5f);
                dust.noGravity = true;
            }
        }

        private void ShowMessage(Entity target, string text, Color color)
        {
            if (Main.netMode != NetmodeID.Server)
                CombatText.NewText(target.Hitbox, color, text);
        }
    }
}
