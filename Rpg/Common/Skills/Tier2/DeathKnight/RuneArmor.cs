using Terraria;
using Rpg.Common.Base;

namespace Rpg.Common.Skills.Tier2.DeathKnight
{
    /// <summary>
    /// Rune Armor - 룬 갑옷.
    /// 방어력 증가.
    /// </summary>
    public class RuneArmor : BaseSkill
    {
        public override string InternalName => "RuneArmor";
        public override string DisplayName => "Rune Armor";
        public override string Description => "Ancient runes protect you, increasing defense.";

        public override SkillType SkillType => SkillType.Passive;
        public override JobType RequiredJob => JobType.DeathKnight;
        public override int RequiredLevel => 68;
        public override int SkillPointCost => 1;
        public override int MaxRank => 10;
        public override string IconTexture => $"{RpgConstants.ICON_PATH}Skills/RuneArmor";
        public override float CooldownSeconds => 0f;
        public override int ResourceCost => 0;
        public override ResourceType ResourceType => ResourceType.None;

        private static readonly int[] DEFENSE_BONUS = { 3, 6, 9, 12, 16, 20, 25, 30, 36, 45 };

        public override void ApplyPassive(Player player)
        {
            if (CurrentRank <= 0) return;
            int rank = CurrentRank;
            
            player.statDefense += DEFENSE_BONUS[rank - 1];
        }
    }
}
