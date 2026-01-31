using Terraria;
using RpgMod.Common.Base;

namespace RpgMod.Common.Skills.Tier3.Guardian
{
    /// <summary>
    /// Unbreakable - Guardian's ultimate passive.
    /// Greatly increases defense permanently.
    /// </summary>
    public class Unbreakable : BaseSkill
    {
        public override string InternalName => "Unbreakable";
        public override string DisplayName => "Unbreakable";
        public override string Description => "Your body becomes like steel, greatly increasing defense and knockback resistance (immunity at high rank).";

        public override SkillType SkillType => SkillType.Passive;
        public override JobType RequiredJob => JobType.Guardian;
        public override int RequiredLevel => RpgConstants.THIRD_JOB_LEVEL + 20;
        public override int SkillPointCost => 1;
        public override int MaxRank => 5;
        public override string IconTexture => $"{RpgConstants.ICON_PATH}Skills/Unbreakable";
        public override float CooldownSeconds => 0f;
        public override int ResourceCost => 0;
        public override ResourceType ResourceType => ResourceType.None;

        // Scaling per rank
        private static readonly int[] DEFENSE_BONUS = { 12, 18, 24, 32, 45 };
        private static readonly float[] KNOCKBACK_RESIST = { 0.10f, 0.15f, 0.20f, 0.25f, 0.35f };

        public static float GetKnockbackResist(int rank)
        {
            if (rank <= 0)
                return 0f;
            int index = System.Math.Min(rank, KNOCKBACK_RESIST.Length) - 1;
            return KNOCKBACK_RESIST[index];
        }

        public override void ApplyPassive(Player player)
        {
            if (CurrentRank <= 0)
                return;

            int rank = CurrentRank;

            // Apply defense bonus
            player.statDefense += DEFENSE_BONUS[rank - 1];

            // Apply knockback resistance
            player.noKnockback = rank >= 5 || player.noKnockback;
        }
    }
}
