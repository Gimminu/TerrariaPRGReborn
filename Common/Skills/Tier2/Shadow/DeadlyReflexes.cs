using Terraria;
using Terraria.ModLoader;
using RpgMod.Common.Base;

namespace RpgMod.Common.Skills.Tier2.Shadow
{
    /// <summary>
    /// Deadly Reflexes - 치명적 반사신경.
    /// 크리티컬과 회피 증가.
    /// 그림자의 크리티컬 패시브.
    /// </summary>
    public class DeadlyReflexes : BaseSkill
    {
        public override string InternalName => "DeadlyReflexes";
        public override string DisplayName => "Deadly Reflexes";
        public override string Description => "Increase critical chance and dodge.";

        public override SkillType SkillType => SkillType.Passive;
        public override JobType RequiredJob => JobType.Shadow;
        public override int RequiredLevel => 70;
        public override int SkillPointCost => 1;
        public override int MaxRank => 10;
        public override string IconTexture => $"{RpgConstants.ICON_PATH}Skills/DeadlyReflexes";
        public override float CooldownSeconds => 0f;
        public override int ResourceCost => 0;
        public override ResourceType ResourceType => ResourceType.None;

        private static readonly int[] CRIT_BONUS = { 3, 5, 7, 9, 11, 13, 16, 19, 23, 30 };

        public override void ApplyPassive(Player player)
        {
            if (CurrentRank <= 0) return;
            int rank = CurrentRank;
            
            player.GetCritChance(DamageClass.Melee) += CRIT_BONUS[rank - 1];
            player.GetCritChance(DamageClass.Ranged) += CRIT_BONUS[rank - 1];
        }
    }
}
