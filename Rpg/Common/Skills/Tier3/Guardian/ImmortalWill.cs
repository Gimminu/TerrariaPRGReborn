using Terraria;
using Rpg.Common.Base;

namespace Rpg.Common.Skills.Tier3.Guardian
{
    /// <summary>
    /// Immortal Will - 불사의 의지.
    /// 최대 체력과 방어력 대폭 증가.
    /// 가디언의 핵심 패시브.
    /// </summary>
    public class ImmortalWill : BaseSkill
    {
        public override string InternalName => "ImmortalWill";
        public override string DisplayName => "Immortal Will";
        public override string Description => "Greatly increase maximum health and defense.";

        public override SkillType SkillType => SkillType.Passive;
        public override JobType RequiredJob => JobType.Guardian;
        public override int RequiredLevel => 120;
        public override int SkillPointCost => 1;
        public override int MaxRank => 10;
        public override string IconTexture => $"{RpgConstants.ICON_PATH}Skills/ImmortalWill";
        public override float CooldownSeconds => 0f;
        public override int ResourceCost => 0;
        public override ResourceType ResourceType => ResourceType.None;

        private static readonly int[] HP_BONUS = { 50, 100, 160, 230, 310, 400, 500, 620, 760, 1000 };
        private static readonly int[] DEFENSE_BONUS = { 8, 15, 22, 30, 38, 47, 58, 70, 85, 110 };

        public override void ApplyPassive(Player player)
        {
            if (CurrentRank <= 0) return;
            int rank = CurrentRank;
            
            player.statLifeMax2 += HP_BONUS[rank - 1];
            player.statDefense += DEFENSE_BONUS[rank - 1];
        }
    }
}
