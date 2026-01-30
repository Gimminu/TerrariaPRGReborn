using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Rpg.Common.Base;
using Rpg.Common.Players;

namespace Rpg.Common.Skills.Tier3.Gunmaster
{
    /// <summary>
    /// Quick Reload - Gunmaster's attack speed buff.
    /// </summary>
    public class QuickReload : BaseSkill
    {
        public override string InternalName => "QuickReload";
        public override string DisplayName => "Quick Reload";
        public override string Description => "Drastically increase attack speed for a short duration.";

        public override SkillType SkillType => SkillType.Active;
        public override JobType RequiredJob => JobType.Gunmaster;
        public override int RequiredLevel => RpgConstants.THIRD_JOB_LEVEL + 10;
        public override int SkillPointCost => 2;
        public override int MaxRank => 5;
        public override string IconTexture => $"{RpgConstants.ICON_PATH}Skills/QuickReload";
        public override float CooldownSeconds => 25f;
        public override int ResourceCost => 28;
        public override ResourceType ResourceType => ResourceType.Stamina;

        private static readonly int[] DURATION_SECONDS = { 8, 10, 12, 14, 18 };
        private static readonly float[] ATTACK_SPEED = { 0.25f, 0.32f, 0.40f, 0.50f, 0.65f };

        protected override void OnActivate(Player player)
        {
            int rank = System.Math.Max(1, CurrentRank);
            int duration = DURATION_SECONDS[rank - 1] * 60;
            float speed = ATTACK_SPEED[rank - 1];

            var rpgPlayer = player.GetModPlayer<RpgPlayer>();
            rpgPlayer.AddTemporaryAttackSpeed(speed, duration);

            player.AddBuff(BuffID.Swiftness, duration);

            PlayEffects(player);
            ShowMessage(player, "QUICK RELOAD!", Color.Yellow);
        }

        private void PlayEffects(Player player)
        {
            SoundEngine.PlaySound(SoundID.Coins, player.position);
            for (int i = 0; i < 20; i++)
            {
                Dust dust = Dust.NewDustDirect(player.position, player.width, player.height,
                    DustID.Torch, Main.rand.NextFloat(-2f, 2f), Main.rand.NextFloat(-2f, 0f), 100, Color.Yellow, 1.3f);
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
