using Terraria;
using Terraria.ModLoader;
using Rpg.Common.Base;

namespace Rpg.Common.Skills.Tier2.Assassin
{
    /// <summary>
    /// Lethal Precision - 치명적 정밀함.
    /// 크리티컬 확률과 크리티컬 피해 증가.
    /// 암살자의 크리티컬 패시브.
    /// </summary>
    public class LethalPrecision : BaseSkill
    {
        public override string InternalName => "LethalPrecision";
        public override string DisplayName => "Lethal Precision";
        public override string Description => "Strike with deadly precision, increasing critical chance.";

        public override SkillType SkillType => SkillType.Passive;
        public override JobType RequiredJob => JobType.Assassin;
        public override int RequiredLevel => 65;
        public override int SkillPointCost => 1;
        public override int MaxRank => 10;
        public override string IconTexture => $"{RpgConstants.ICON_PATH}Skills/LethalPrecision";
        public override float CooldownSeconds => 0f;
        public override int ResourceCost => 0;
        public override ResourceType ResourceType => ResourceType.None;

        private static readonly int[] CRIT_BONUS = { 3, 5, 7, 9, 11, 13, 15, 17, 19, 22 };

        public override void ApplyPassive(Player player)
        {
            if (CurrentRank <= 0) return;
            int rank = CurrentRank;
            
            player.GetCritChance(DamageClass.Melee) += CRIT_BONUS[rank - 1];
            player.GetCritChance(DamageClass.Ranged) += CRIT_BONUS[rank - 1];
        }
    }
}
