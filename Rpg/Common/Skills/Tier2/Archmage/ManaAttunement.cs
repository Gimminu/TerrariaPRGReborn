using Terraria;
using Rpg.Common.Base;

namespace Rpg.Common.Skills.Tier2.Archmage
{
    /// <summary>
    /// Mana Attunement - 마나 조율.
    /// 최대 마나 증가.
    /// </summary>
    public class ManaAttunement : BaseSkill
    {
        public override string InternalName => "ManaAttunement";
        public override string DisplayName => "Mana Attunement";
        public override string Description => "Attune your spirit to the flow of mana, increasing maximum mana.";

        public override SkillType SkillType => SkillType.Passive;
        public override JobType RequiredJob => JobType.Archmage;
        public override int RequiredLevel => 65;
        public override int SkillPointCost => 1;
        public override int MaxRank => 10;
        public override string IconTexture => $"{RpgConstants.ICON_PATH}Skills/ManaAttunement";
        public override float CooldownSeconds => 0f;
        public override int ResourceCost => 0;
        public override ResourceType ResourceType => ResourceType.None;

        private static readonly int[] MANA_BONUS = { 15, 30, 50, 70, 95, 120, 150, 180, 220, 300 };

        public override void ApplyPassive(Player player)
        {
            if (CurrentRank <= 0) return;
            int rank = CurrentRank;
            
            player.statManaMax2 += MANA_BONUS[rank - 1];
        }
    }
}
