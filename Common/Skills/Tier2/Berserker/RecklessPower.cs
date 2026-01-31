using Terraria;
using Terraria.ModLoader;
using RpgMod.Common.Base;

namespace RpgMod.Common.Skills.Tier2.Berserker
{
    /// <summary>
    /// Reckless Power - 무모한 힘.
    /// 크리티컬 확률 대폭 증가, 방어력 감소.
    /// 광전사의 공격 패시브.
    /// </summary>
    public class RecklessPower : BaseSkill
    {
        public override string InternalName => "RecklessPower";
        public override string DisplayName => "Reckless Power";
        public override string Description => "Sacrifice defense for greatly increased critical chance.";

        public override SkillType SkillType => SkillType.Passive;
        public override JobType RequiredJob => JobType.Berserker;
        public override int RequiredLevel => 72;
        public override int SkillPointCost => 1;
        public override int MaxRank => 10;
        public override string IconTexture => $"{RpgConstants.ICON_PATH}Skills/RecklessPower";
        public override float CooldownSeconds => 0f;
        public override int ResourceCost => 0;
        public override ResourceType ResourceType => ResourceType.None;

        private static readonly int[] CRIT_BONUS = { 5, 7, 9, 11, 13, 15, 17, 19, 22, 25 };
        private static readonly int[] DEFENSE_PENALTY = { 5, 5, 5, 5, 5, 5, 5, 5, 5, 5 };

        public override void ApplyPassive(Player player)
        {
            if (CurrentRank <= 0) return;
            int rank = CurrentRank;
            
            player.GetCritChance(DamageClass.Melee) += CRIT_BONUS[rank - 1];
            player.statDefense -= DEFENSE_PENALTY[rank - 1];
        }
    }
}
