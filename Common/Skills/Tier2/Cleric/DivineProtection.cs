using Terraria;
using Terraria.ModLoader;
using Rpg.Common.Base;

namespace Rpg.Common.Skills.Tier2.Cleric
{
    /// <summary>
    /// Divine Protection - 신성 보호.
    /// 방어력 증가.
    /// </summary>
    public class DivineProtection : BaseSkill
    {
        public override string InternalName => "DivineProtection";
        public override string DisplayName => "Divine Protection";
        public override string Description => "Divine power shields you, increasing defense.";

        public override SkillType SkillType => SkillType.Passive;
        public override JobType RequiredJob => JobType.Cleric;
        public override int RequiredLevel => 60;
        public override int SkillPointCost => 1;
        public override int MaxRank => 10;
        public override string IconTexture => $"{RpgConstants.ICON_PATH}Skills/DivineProtection";
        public override float CooldownSeconds => 0f;
        public override int ResourceCost => 0;
        public override ResourceType ResourceType => ResourceType.None;

        private static readonly int[] DEFENSE_BONUS = { 2, 4, 6, 8, 11, 14, 18, 22, 27, 35 };

        public override void ApplyPassive(Player player)
        {
            if (CurrentRank <= 0) return;
            int rank = CurrentRank;
            
            player.statDefense += DEFENSE_BONUS[rank - 1];
        }
    }
}
