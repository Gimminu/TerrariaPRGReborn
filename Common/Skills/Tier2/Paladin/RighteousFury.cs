using Terraria;
using RpgMod.Common.Base;

namespace RpgMod.Common.Skills.Tier2.Paladin
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
        private static readonly float[] DAMAGE_PER_STACK = { 0.01f, 0.012f, 0.014f, 0.016f, 0.018f, 0.02f, 0.022f, 0.024f, 0.026f, 0.03f };
        private static readonly int[] MAX_STACKS = { 3, 3, 4, 4, 5, 5, 6, 6, 7, 8 };
        private static readonly int[] DURATION_SECONDS = { 6, 6, 7, 7, 8, 8, 9, 9, 10, 12 };

        public static float GetDamagePerStack(int rank)
        {
            return DAMAGE_PER_STACK[System.Math.Clamp(rank - 1, 0, 9)];
        }

        public static int GetMaxStacks(int rank)
        {
            return MAX_STACKS[System.Math.Clamp(rank - 1, 0, 9)];
        }

        public static int GetDurationSeconds(int rank)
        {
            return DURATION_SECONDS[System.Math.Clamp(rank - 1, 0, 9)];
        }

        public override void ApplyPassive(Player player)
        {
            // 피격 시 스택 증가 - RpgPlayer에서 처리
        }
    }
}
