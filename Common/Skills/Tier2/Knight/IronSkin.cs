using Terraria;
using RpgMod.Common.Base;

namespace RpgMod.Common.Skills.Tier2.Knight
{
    /// <summary>
    /// Iron Skin - Knight's passive skill.
    /// Permanently increases defense.
    /// </summary>
    public class IronSkin : BaseSkill
    {
        public override string InternalName => "IronSkin";
        public override string DisplayName => "Iron Skin";
        public override string Description => "Permanently increase your defense.";

        public override SkillType SkillType => SkillType.Passive;
        public override JobType RequiredJob => JobType.Knight;
        public override int RequiredLevel => RpgConstants.SECOND_JOB_LEVEL + 15;
        public override int SkillPointCost => 1;
        public override int MaxRank => 5;
        public override string IconTexture => $"{RpgConstants.ICON_PATH}Skills/IronSkin";
        public override float CooldownSeconds => 0f;
        public override int ResourceCost => 0;
        public override ResourceType ResourceType => ResourceType.None;

        // Scaling per rank
        private static readonly int[] DEFENSE_BONUS = { 6, 10, 14, 18, 25 };

        public override void ApplyPassive(Player player)
        {
            if (CurrentRank <= 0)
                return;

            int rank = CurrentRank;
            player.statDefense += DEFENSE_BONUS[rank - 1];
        }
    }
}
