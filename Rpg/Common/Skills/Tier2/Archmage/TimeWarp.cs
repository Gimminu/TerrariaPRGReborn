using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Rpg.Common.Base;
using Rpg.Common.Players;

namespace Rpg.Common.Skills.Tier2.Archmage
{
    /// <summary>
    /// Time Warp - Archmage's buff that speeds up actions.
    /// </summary>
    public class TimeWarp : BaseSkill
    {
        public override string InternalName => "TimeWarp";
        public override string DisplayName => "Time Warp";
        public override string Description => "Bend time to increase your casting speed.";

        public override SkillType SkillType => SkillType.Active;
        public override JobType RequiredJob => JobType.Archmage;
        public override int RequiredLevel => RpgConstants.SECOND_JOB_LEVEL + 10;
        public override int SkillPointCost => 1;
        public override int MaxRank => 5;
        public override string IconTexture => $"{RpgConstants.ICON_PATH}Skills/TimeWarp";
        public override float CooldownSeconds => 25f;
        public override int ResourceCost => 30;
        public override ResourceType ResourceType => ResourceType.Mana;

        private static readonly int[] DURATION_SECONDS = { 8, 10, 12, 14, 18 };
        private static readonly float[] ATTACK_SPEED_BONUS = { 0.10f, 0.15f, 0.20f, 0.25f, 0.35f };

        protected override void OnActivate(Player player)
        {
            int rank = System.Math.Max(1, CurrentRank);
            int duration = DURATION_SECONDS[rank - 1] * 60;
            float speedBonus = ATTACK_SPEED_BONUS[rank - 1];

            var rpgPlayer = player.GetModPlayer<RpgPlayer>();
            rpgPlayer.AddTemporaryAttackSpeed(speedBonus, duration);

            player.AddBuff(BuffID.Swiftness, duration);

            PlayEffects(player);
            ShowMessage(player, "Time Warp!", Color.Cyan);
        }

        private void PlayEffects(Player player)
        {
            SoundEngine.PlaySound(SoundID.Item28, player.position);
            for (int i = 0; i < 30; i++)
            {
                float angle = MathHelper.TwoPi * i / 30f;
                Vector2 offset = new Vector2((float)System.Math.Cos(angle), (float)System.Math.Sin(angle)) * 50f;
                Dust dust = Dust.NewDustDirect(player.Center + offset, 4, 4, DustID.MagicMirror, 
                    -offset.X * 0.05f, -offset.Y * 0.05f, 100, Color.Cyan, 1.3f);
                dust.noGravity = true;
            }
        }

        private void ShowMessage(Player player, string text, Color color)
        {
            if (Main.netMode != NetmodeID.Server)
                CombatText.NewText(player.Hitbox, color, text);
        }
    }
}
