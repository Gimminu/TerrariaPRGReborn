using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Rpg.Common.Base;
using Rpg.Common.Players;

namespace Rpg.Common.Skills.Tier3.Deadeye
{
    /// <summary>
    /// Deadeye Focus - Deadeye's ultimate precision skill.
    /// </summary>
    public class DeadeyeFocus : BaseSkill
    {
        public override string InternalName => "DeadeyeFocus";
        public override string DisplayName => "Deadeye Focus";
        public override string Description => "Enter a state of perfect focus, massively increasing crit chance.";

        public override SkillType SkillType => SkillType.Active;
        public override JobType RequiredJob => JobType.Deadeye;
        public override int RequiredLevel => RpgConstants.THIRD_JOB_LEVEL + 10;
        public override int SkillPointCost => 2;
        public override int MaxRank => 5;
        public override string IconTexture => $"{RpgConstants.ICON_PATH}Skills/DeadeyeFocus";
        public override float CooldownSeconds => 28f;
        public override int ResourceCost => 30;
        public override ResourceType ResourceType => ResourceType.Stamina;

        private static readonly int[] DURATION_SECONDS = { 8, 10, 12, 14, 18 };
        private static readonly float[] DAMAGE_BONUS = { 0.20f, 0.28f, 0.36f, 0.45f, 0.60f };

        protected override void OnActivate(Player player)
        {
            int rank = System.Math.Max(1, CurrentRank);
            int duration = DURATION_SECONDS[rank - 1] * 60;
            float bonus = DAMAGE_BONUS[rank - 1];

            var rpgPlayer = player.GetModPlayer<RpgPlayer>();
            rpgPlayer.AddTemporaryDamage(bonus, duration);

            player.AddBuff(BuffID.Archery, duration);

            PlayEffects(player);
            ShowMessage(player, "DEADEYE FOCUS!", Color.OrangeRed);
        }

        private void PlayEffects(Player player)
        {
            SoundEngine.PlaySound(SoundID.MaxMana, player.position);
            for (int i = 0; i < 25; i++)
            {
                Dust dust = Dust.NewDustDirect(player.position, player.width, player.height,
                    DustID.Torch, 0, -2f, 100, Color.Orange, 1.5f);
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
