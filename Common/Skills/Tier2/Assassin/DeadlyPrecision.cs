using Terraria;
using Terraria.ModLoader;
using RpgMod.Common.Base;

namespace RpgMod.Common.Skills.Tier2.Assassin
{
    /// <summary>
    /// Deadly Precision - 치명적 정확도.
    /// 크리티컬 피해 증가.
    /// </summary>
    public class DeadlyPrecision : BaseSkill
    {
        public override string InternalName => "DeadlyPrecision";
        public override string DisplayName => "Deadly Precision";
        public override string Description => "Strike with deadly precision, increasing critical chance.";

        public override SkillType SkillType => SkillType.Passive;
        public override JobType RequiredJob => JobType.Assassin;
        public override int RequiredLevel => 78;
        public override int SkillPointCost => 1;
        public override int MaxRank => 10;
        public override string IconTexture => $"{RpgConstants.ICON_PATH}Skills/DeadlyPrecision";
        public override float CooldownSeconds => 0f;
        public override int ResourceCost => 0;
        public override ResourceType ResourceType => ResourceType.None;

        private static readonly int[] CRIT_BONUS = { 2, 4, 6, 8, 10, 12, 14, 16, 18, 22 };

        public override void ApplyPassive(Player player)
        {
            if (CurrentRank <= 0) return;
            int rank = CurrentRank;
            
            player.GetCritChance(DamageClass.Generic) += CRIT_BONUS[rank - 1];
        }
    }
}
