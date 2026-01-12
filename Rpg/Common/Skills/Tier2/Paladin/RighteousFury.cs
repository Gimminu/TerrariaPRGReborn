using Terraria;
using Rpg.Common.Base;

namespace Rpg.Common.Skills.Tier2.Paladin
{
    /// <summary>
    /// Righteous Fury - 정의의 분노.
    /// 피해를 받을 때 공격력 증가.
    /// </summary>
    public class RighteousFury : BaseSkill
    {
        public override string InternalName => "RighteousFury";
        public override string DisplayName => "Righteous Fury";
        public override string Description => "When you take damage, your righteous fury increases your damage.";

        public override SkillType SkillType => SkillType.Passive;
        public override JobType RequiredJob => JobType.Paladin;
        public override int RequiredLevel => 85;
        public override int SkillPointCost => 1;
        public override int MaxRank => 10;
        public override string IconTexture => $"{RpgConstants.ICON_PATH}Skills/RighteousFury";
        public override float CooldownSeconds => 0f;
        public override int ResourceCost => 0;
        public override ResourceType ResourceType => ResourceType.None;

        // 효과는 RpgPlayer에서 OnHit 이벤트로 처리

        public override void ApplyPassive(Player player)
        {
            // 피격 시 스택 증가 - RpgPlayer에서 처리
        }
    }
}
