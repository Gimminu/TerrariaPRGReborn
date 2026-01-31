using Terraria;
using RpgMod.Common.Base;

namespace RpgMod.Common.Skills.Tier1.Summoner
{
    /// <summary>
    /// Soul Bond - 영혼 결속.
    /// 소환수가 피해를 줄 때 체력 회복.
    /// 소환사의 생존 패시브.
    /// </summary>
    public class SoulBond : BaseSkill
    {
        public override string InternalName => "SoulBond";
        public override string DisplayName => "Soul Bond";
        public override string Description => "Form a bond with your minions, healing on their hits and boosting life regen.";

        public override SkillType SkillType => SkillType.Passive;
        public override JobType RequiredJob => JobType.Summoner;
        public override int RequiredLevel => 20;
        public override int SkillPointCost => 1;
        public override int MaxRank => 10;
        public override string IconTexture => $"{RpgConstants.ICON_PATH}Skills/SoulBond";
        public override float CooldownSeconds => 0f;
        public override int ResourceCost => 0;
        public override ResourceType ResourceType => ResourceType.None;

        // 흡혈량은 RpgPlayer에서 OnHit 이벤트로 처리
        private static readonly float[] LIFESTEAL = { 0.01f, 0.015f, 0.02f, 0.025f, 0.03f, 0.035f, 0.04f, 0.045f, 0.05f, 0.06f };

        public static float GetLifeSteal(int rank)
        {
            return LIFESTEAL[System.Math.Clamp(rank - 1, 0, 9)];
        }

        public override void ApplyPassive(Player player)
        {
            // 실제 흡혈 효과는 RpgPlayer의 OnHitNPCWithProj에서 처리
            // 여기서는 체력 재생 보너스를 추가
            if (CurrentRank <= 0) return;
            int rank = CurrentRank;
            
            player.lifeRegen += rank; // 기본 재생 보너스
        }
    }
}
