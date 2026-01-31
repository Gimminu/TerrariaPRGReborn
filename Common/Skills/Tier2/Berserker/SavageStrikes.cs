using Terraria;
using Terraria.ModLoader;
using RpgMod.Common.Base;

namespace RpgMod.Common.Skills.Tier2.Berserker
{
    /// <summary>
    /// Savage Strikes - 야만의 일격.
    /// 크리티컬 피해 증가.
    /// </summary>
    public class SavageStrikes : BaseSkill
    {
        public override string InternalName => "SavageStrikes";
        public override string DisplayName => "Savage Strikes";
        public override string Description => "Your strikes are more vicious, increasing critical chance.";

        public override SkillType SkillType => SkillType.Passive;
        public override JobType RequiredJob => JobType.Berserker;
        public override int RequiredLevel => 75;
        public override int SkillPointCost => 1;
        public override int MaxRank => 10;
        public override string IconTexture => $"{RpgConstants.ICON_PATH}Skills/SavageStrikes";
        public override float CooldownSeconds => 0f;
        public override int ResourceCost => 0;
        public override ResourceType ResourceType => ResourceType.None;

        private static readonly int[] CRIT_BONUS = { 2, 4, 6, 8, 10, 12, 14, 16, 18, 22 };

        public override void ApplyPassive(Player player)
        {
            if (CurrentRank <= 0) return;
            int rank = CurrentRank;
            
            player.GetCritChance(DamageClass.Melee) += CRIT_BONUS[rank - 1];
        }
    }
}
