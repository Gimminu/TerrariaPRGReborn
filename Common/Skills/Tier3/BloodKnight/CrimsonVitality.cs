using Terraria;
using Rpg.Common.Base;

namespace Rpg.Common.Skills.Tier3.BloodKnight
{
    /// <summary>
    /// Crimson Vitality - 진홍의 활력.
    /// 체력이 낮을수록 피해 증가.
    /// 블러드나이트의 저체력 패시브.
    /// </summary>
    public class CrimsonVitality : BaseSkill
    {
        public override string InternalName => "CrimsonVitality";
        public override string DisplayName => "Crimson Vitality";
        public override string Description => "Deal more damage when at low health.";

        public override SkillType SkillType => SkillType.Passive;
        public override JobType RequiredJob => JobType.BloodKnight;
        public override int RequiredLevel => 135;
        public override int SkillPointCost => 1;
        public override int MaxRank => 10;
        public override string IconTexture => $"{RpgConstants.ICON_PATH}Skills/CrimsonVitality";
        public override float CooldownSeconds => 0f;
        public override int ResourceCost => 0;
        public override ResourceType ResourceType => ResourceType.None;

        // 저체력 보너스 (RpgPlayer.ModifyHitNPC에서 처리)
        // 체력 25% 이하에서 최대 효과
        private static readonly float[] LOW_HP_BONUS = { 0.10f, 0.15f, 0.20f, 0.25f, 0.30f, 0.36f, 0.42f, 0.50f, 0.60f, 0.80f };

        public static float GetLowHpBonus(int rank)
        {
            return LOW_HP_BONUS[System.Math.Clamp(rank - 1, 0, 9)];
        }

        public override void ApplyPassive(Player player)
        {
            if (CurrentRank <= 0) return;
            
            // 기본 체력 재생 증가
            player.lifeRegen += CurrentRank * 2;
        }
    }
}
