using Terraria;
using Rpg.Common.Base;

namespace Rpg.Common.Skills.Tier1.Summoner
{
    /// <summary>
    /// Summon Mastery - Summoner's passive skill.
    /// Increases maximum minion slots.
    /// </summary>
    public class SummonMastery : BaseSkill
    {
        public override string InternalName => "SummonMastery";
        public override string DisplayName => "Summon Mastery";
        public override string Description => "Increase your maximum minion slots.";

        public override SkillType SkillType => SkillType.Passive;
        public override JobType RequiredJob => JobType.Summoner;
        public override int RequiredLevel => RpgConstants.FIRST_JOB_LEVEL + 10;
        public override int SkillPointCost => 1;
        public override int MaxRank => 5;
        public override string IconTexture => $"{RpgConstants.ICON_PATH}Skills/SummonMastery";
        public override float CooldownSeconds => 0f;
        public override int ResourceCost => 0;
        public override ResourceType ResourceType => ResourceType.None;

        // Scaling per rank
        private static readonly int[] MINION_SLOTS = { 1, 1, 2, 2, 3 };

        public override void ApplyPassive(Player player)
        {
            if (CurrentRank <= 0)
                return;

            int rank = CurrentRank;

            // Add minion slots
            player.maxMinions += MINION_SLOTS[rank - 1];
        }
    }
}
