using Terraria;
using Terraria.ModLoader;
using Rpg.Common.Base;

namespace Rpg.Common.Skills.Tier3.BloodKnight
{
    /// <summary>
    /// Blood Pact - 피의 계약.
    /// 모든 피해 증가 + 흡혈.
    /// 블러드나이트의 핵심 패시브.
    /// </summary>
    public class BloodPact : BaseSkill
    {
        public override string InternalName => "BloodPact";
        public override string DisplayName => "Blood Pact";
        public override string Description => "Increase all damage and gain lifesteal.";

        public override SkillType SkillType => SkillType.Passive;
        public override JobType RequiredJob => JobType.BloodKnight;
        public override int RequiredLevel => 120;
        public override int SkillPointCost => 1;
        public override int MaxRank => 10;
        public override string IconTexture => $"{RpgConstants.ICON_PATH}Skills/BloodPact";
        public override float CooldownSeconds => 0f;
        public override int ResourceCost => 0;
        public override ResourceType ResourceType => ResourceType.None;

        private static readonly float[] DAMAGE_BONUS = { 0.05f, 0.09f, 0.13f, 0.17f, 0.21f, 0.26f, 0.31f, 0.37f, 0.44f, 0.55f };
        // 흡혈은 RpgPlayer에서 처리
        private static readonly float[] LIFESTEAL = { 0.02f, 0.03f, 0.04f, 0.05f, 0.06f, 0.07f, 0.08f, 0.09f, 0.11f, 0.15f };

        public static float GetLifesteal(int rank)
        {
            return LIFESTEAL[System.Math.Clamp(rank - 1, 0, 9)];
        }

        public override void ApplyPassive(Player player)
        {
            if (CurrentRank <= 0) return;
            int rank = CurrentRank;
            
            player.GetDamage(DamageClass.Generic) += DAMAGE_BONUS[rank - 1];
        }
    }
}
