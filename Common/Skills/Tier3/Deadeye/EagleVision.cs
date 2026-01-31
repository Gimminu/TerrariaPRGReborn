using Terraria;
using Terraria.ModLoader;
using RpgMod.Common.Base;

namespace RpgMod.Common.Skills.Tier3.Deadeye
{
    /// <summary>
    /// Eagle Vision - 독수리의 시야.
    /// 크리티컬과 원거리 피해 대폭 증가.
    /// 데드아이의 핵심 패시브.
    /// </summary>
    public class EagleVision : BaseSkill
    {
        public override string InternalName => "EagleVision";
        public override string DisplayName => "Eagle Vision";
        public override string Description => "Greatly increase ranged damage and critical chance.";

        public override SkillType SkillType => SkillType.Passive;
        public override JobType RequiredJob => JobType.Deadeye;
        public override int RequiredLevel => 120;
        public override int SkillPointCost => 1;
        public override int MaxRank => 10;
        public override string IconTexture => $"{RpgConstants.ICON_PATH}Skills/EagleVision";
        public override float CooldownSeconds => 0f;
        public override int ResourceCost => 0;
        public override ResourceType ResourceType => ResourceType.None;

        private static readonly float[] DAMAGE_BONUS = { 0.05f, 0.09f, 0.13f, 0.18f, 0.23f, 0.29f, 0.35f, 0.42f, 0.50f, 0.65f };
        private static readonly int[] CRIT_BONUS = { 5, 9, 13, 17, 21, 26, 31, 37, 44, 58 };

        public override void ApplyPassive(Player player)
        {
            if (CurrentRank <= 0) return;
            int rank = CurrentRank;
            
            player.GetDamage(DamageClass.Ranged) += DAMAGE_BONUS[rank - 1];
            player.GetCritChance(DamageClass.Ranged) += CRIT_BONUS[rank - 1];
        }
    }
}
