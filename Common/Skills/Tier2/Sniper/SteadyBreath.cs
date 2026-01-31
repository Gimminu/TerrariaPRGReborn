using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using RpgMod.Common.Base;
using RpgMod.Common.Players;

namespace RpgMod.Common.Skills.Tier2.Sniper
{
    /// <summary>
    /// Steady Breath - Sniper's aim enhancement skill.
    /// </summary>
    public class SteadyBreath : BaseSkill
    {
        public override string InternalName => "SteadyBreath";
        public override string DisplayName => "Steady Breath";
        public override string Description => "Steady your aim for increased ranged damage and critical chance.";

        public override SkillType SkillType => SkillType.Active;
        public override JobType RequiredJob => JobType.Sniper;
        public override int RequiredLevel => RpgConstants.SECOND_JOB_LEVEL + 10;
        public override int SkillPointCost => 1;
        public override int MaxRank => 5;
        public override string IconTexture => $"{RpgConstants.ICON_PATH}Skills/SteadyBreath";
        public override float CooldownSeconds => 18f;
        public override int ResourceCost => 20;
        public override ResourceType ResourceType => ResourceType.Stamina;

        private static readonly int[] DURATION_SECONDS = { 8, 10, 12, 14, 18 };
        private static readonly float[] DAMAGE_BONUS = { 0.12f, 0.16f, 0.20f, 0.25f, 0.35f };
        private static readonly float[] CRIT_BONUS = { 4f, 6f, 8f, 10f, 12f };

        protected override void OnActivate(Player player)
        {
            int rank = System.Math.Max(1, CurrentRank);
            int duration = DURATION_SECONDS[rank - 1] * 60;
            float bonus = DAMAGE_BONUS[rank - 1];
            float critBonus = CRIT_BONUS[rank - 1];

            var rpgPlayer = player.GetModPlayer<RpgPlayer>();
            rpgPlayer.AddTemporaryRangedDamage(bonus, duration);
            rpgPlayer.AddTemporaryRangedCrit(critBonus, duration);

            player.AddBuff(BuffID.Archery, duration);

            PlayEffects(player);
            ShowMessage(player, "Steady Aim...", Color.LightGreen);
        }

        private void PlayEffects(Player player)
        {
            SoundEngine.PlaySound(SoundID.MaxMana, player.position);
            for (int i = 0; i < 15; i++)
            {
                Dust dust = Dust.NewDustDirect(player.position, player.width, player.height,
                    DustID.GreenTorch, 0, -1f, 100, Color.Green, 1.0f);
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
