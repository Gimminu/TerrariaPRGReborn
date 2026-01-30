using Terraria;
using Rpg.Common.Base;

namespace Rpg.Common.Skills.Tier2.Knight
{
    /// <summary>
    /// Fortress Mind - 요새의 정신.
    /// 방어력에 비례하여 추가 방어력 증가.
    /// 기사의 핵심 패시브.
    /// </summary>
    public class FortressMind : BaseSkill
    {
        public override string InternalName => "FortressMind";
        public override string DisplayName => "Fortress Mind";
        public override string Description => "Your armor becomes more effective, gaining bonus defense based on your current defense.";

        public override SkillType SkillType => SkillType.Passive;
        public override JobType RequiredJob => JobType.Knight;
        public override int RequiredLevel => 60;
        public override int SkillPointCost => 1;
        public override int MaxRank => 10;
        public override string IconTexture => $"{RpgConstants.ICON_PATH}Skills/FortressMind";
        public override float CooldownSeconds => 0f;
        public override int ResourceCost => 0;
        public override ResourceType ResourceType => ResourceType.None;

        private static readonly float[] DEFENSE_MULT = { 0.02f, 0.04f, 0.06f, 0.08f, 0.10f, 0.12f, 0.14f, 0.16f, 0.18f, 0.22f };

        public override void ApplyPassive(Player player)
        {
            if (CurrentRank <= 0) return;
            int rank = CurrentRank;
            
            int bonusDefense = (int)(player.statDefense * DEFENSE_MULT[rank - 1]);
            player.statDefense += bonusDefense;
        }
    }
}
