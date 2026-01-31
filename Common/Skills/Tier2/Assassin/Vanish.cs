using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using RpgMod.Common.Base;
using RpgMod.Common.Players;

namespace RpgMod.Common.Skills.Tier2.Assassin
{
    /// <summary>
    /// Vanish - Assassin's stealth skill.
    /// </summary>
    public class Vanish : BaseSkill
    {
        public override string InternalName => "Vanish";
        public override string DisplayName => "Vanish";
        public override string Description => "Become invisible, move faster, and take reduced damage.";

        public override SkillType SkillType => SkillType.Active;
        public override JobType RequiredJob => JobType.Assassin;
        public override int RequiredLevel => RpgConstants.SECOND_JOB_LEVEL + 10;
        public override int SkillPointCost => 1;
        public override int MaxRank => 5;
        public override string IconTexture => $"{RpgConstants.ICON_PATH}Skills/Vanish";
        public override float CooldownSeconds => 25f;
        public override int ResourceCost => 30;
        public override ResourceType ResourceType => ResourceType.Stamina;

        private static readonly int[] DURATION_SECONDS = { 4, 5, 6, 7, 10 };

        protected override void OnActivate(Player player)
        {
            int rank = System.Math.Max(1, CurrentRank);
            int duration = DURATION_SECONDS[rank - 1] * 60;

            player.AddBuff(BuffID.Invisibility, duration);
            player.AddBuff(BuffID.Swiftness, duration);

            var rpgPlayer = player.GetModPlayer<RpgPlayer>();
            rpgPlayer.AddTemporaryDamageReduction(0.50f, duration);

            PlayEffects(player);
            ShowMessage(player, "Vanished!", Color.DarkGray);
        }

        private void PlayEffects(Player player)
        {
            SoundEngine.PlaySound(SoundID.Item8, player.position);
            for (int i = 0; i < 30; i++)
            {
                Dust dust = Dust.NewDustDirect(player.position, player.width, player.height,
                    DustID.Smoke, Main.rand.NextFloat(-3f, 3f), Main.rand.NextFloat(-2f, 0f), 150, Color.DarkGray, 1.8f);
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
