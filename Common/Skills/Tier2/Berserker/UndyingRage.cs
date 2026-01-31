using Terraria;
using RpgMod.Common.Base;

namespace RpgMod.Common.Skills.Tier2.Berserker
{
    /// <summary>
    /// Undying Rage - 불사의 분노.
    /// 치명적인 피해를 받아도 1회 생존.
    /// </summary>
    public class UndyingRage : BaseSkill
    {
        public override string InternalName => "UndyingRage";
        public override string DisplayName => "Undying Rage";
        public override string Description => "Your rage keeps you alive. Survive fatal damage once.";

        public override SkillType SkillType => SkillType.Passive;
        public override JobType RequiredJob => JobType.Berserker;
        public override int RequiredLevel => 82;
        public override int SkillPointCost => 1;
        public override int MaxRank => 10;
        public override string IconTexture => $"{RpgConstants.ICON_PATH}Skills/UndyingRage";
        public override float CooldownSeconds => 0f;
        public override int ResourceCost => 0;
        public override ResourceType ResourceType => ResourceType.None;

        // 실제 효과는 RpgPlayer에서 처리
        // 랭크에 따라 생존 시 남는 체력 증가
        private static readonly int[] SURVIVE_HP = { 5, 10, 15, 20, 30, 40, 50, 65, 80, 100 };

        public static int GetSurviveHp(int rank)
        {
            return SURVIVE_HP[System.Math.Clamp(rank - 1, 0, 9)];
        }

        public override void ApplyPassive(Player player)
        {
            // 효과는 RpgPlayer.PreKill에서 처리
            // 이 패시브가 있으면 죽음을 면함
        }
    }
}
