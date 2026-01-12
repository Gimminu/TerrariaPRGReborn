using Terraria;
using Rpg.Common.Base;

namespace Rpg.Common.Skills.Tier2.Knight
{
    /// <summary>
    /// Fortress - 요새.
    /// 체력이 높을수록 방어력 증가.
    /// </summary>
    public class Fortress : BaseSkill
    {
        public override string InternalName => "Fortress";
        public override string DisplayName => "Fortress";
        public override string Description => "The more health you have, the stronger your defense becomes.";

        public override SkillType SkillType => SkillType.Passive;
        public override JobType RequiredJob => JobType.Knight;
        public override int RequiredLevel => 68;
        public override int SkillPointCost => 1;
        public override int MaxRank => 10;
        public override string IconTexture => $"{RpgConstants.ICON_PATH}Skills/Fortress";
        public override float CooldownSeconds => 0f;
        public override int ResourceCost => 0;
        public override ResourceType ResourceType => ResourceType.None;

        private static readonly float[] DEFENSE_PER_100HP = { 1f, 2f, 3f, 4f, 5f, 6f, 7f, 8f, 9f, 10f };

        public override void ApplyPassive(Player player)
        {
            if (CurrentRank <= 0) return;
            int rank = CurrentRank;
            
            // 최대 체력 100당 방어력 증가
            int bonusDefense = (int)(player.statLifeMax2 / 100f * DEFENSE_PER_100HP[rank - 1]);
            player.statDefense += bonusDefense;
        }
    }
}
