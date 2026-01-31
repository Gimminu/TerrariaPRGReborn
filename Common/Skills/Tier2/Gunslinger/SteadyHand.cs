using Terraria;
using Terraria.ModLoader;
using RpgMod.Common.Base;

namespace RpgMod.Common.Skills.Tier2.Gunslinger
{
    /// <summary>
    /// Steady Hand - 안정된 손.
    /// 원거리 정확도와 크리티컬 증가.
    /// 건슬링거의 정확도 패시브.
    /// </summary>
    public class SteadyHand : BaseSkill
    {
        public override string InternalName => "SteadyHand";
        public override string DisplayName => "Steady Hand";
        public override string Description => "Your aim is true, increasing ranged critical chance.";

        public override SkillType SkillType => SkillType.Passive;
        public override JobType RequiredJob => JobType.Gunslinger;
        public override int RequiredLevel => 65;
        public override int SkillPointCost => 1;
        public override int MaxRank => 10;
        public override string IconTexture => $"{RpgConstants.ICON_PATH}Skills/SteadyHand";
        public override float CooldownSeconds => 0f;
        public override int ResourceCost => 0;
        public override ResourceType ResourceType => ResourceType.None;

        private static readonly int[] CRIT_BONUS = { 2, 4, 6, 8, 10, 12, 14, 16, 18, 22 };

        public override void ApplyPassive(Player player)
        {
            if (CurrentRank <= 0) return;
            int rank = CurrentRank;
            
            player.GetCritChance(DamageClass.Ranged) += CRIT_BONUS[rank - 1];
        }
    }
}
