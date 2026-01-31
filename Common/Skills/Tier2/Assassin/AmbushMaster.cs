using Terraria;
using Terraria.ModLoader;
using RpgMod.Common.Base;

namespace RpgMod.Common.Skills.Tier2.Assassin
{
    /// <summary>
    /// Ambush Master - 기습의 달인.
    /// 은신 상태에서 공격하면 추가 피해.
    /// 암살자의 기습 패시브.
    /// </summary>
    public class AmbushMaster : BaseSkill
    {
        public override string InternalName => "AmbushMaster";
        public override string DisplayName => "Ambush Master";
        public override string Description => "Deal bonus damage when attacking from stealth.";

        public override SkillType SkillType => SkillType.Passive;
        public override JobType RequiredJob => JobType.Assassin;
        public override int RequiredLevel => 78;
        public override int SkillPointCost => 1;
        public override int MaxRank => 10;
        public override string IconTexture => $"{RpgConstants.ICON_PATH}Skills/AmbushMaster";
        public override float CooldownSeconds => 0f;
        public override int ResourceCost => 0;
        public override ResourceType ResourceType => ResourceType.None;

        // 기습 보너스 피해 (RpgPlayer.ModifyHitNPC에서 처리)
        private static readonly float[] AMBUSH_BONUS = { 0.10f, 0.15f, 0.20f, 0.25f, 0.30f, 0.35f, 0.40f, 0.45f, 0.50f, 0.60f };

        public static float GetAmbushBonus(int rank)
        {
            return AMBUSH_BONUS[System.Math.Clamp(rank - 1, 0, 9)];
        }

        public override void ApplyPassive(Player player)
        {
            if (CurrentRank <= 0) return;
            
            // 기본 피해 증가
            player.GetDamage(DamageClass.Generic) += 0.03f * CurrentRank;
        }
    }
}
