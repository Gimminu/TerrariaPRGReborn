using Terraria;
using Rpg.Common.Base;

namespace Rpg.Common.Skills.Tier2.Sorcerer
{
    /// <summary>
    /// Mana Regeneration - 마나 재생.
    /// 마나 재생 속도 증가.
    /// 소서러의 마나 패시브.
    /// </summary>
    public class ManaRegeneration : BaseSkill
    {
        public override string InternalName => "ManaRegeneration";
        public override string DisplayName => "Mana Regeneration";
        public override string Description => "Regenerate mana faster.";

        public override SkillType SkillType => SkillType.Passive;
        public override JobType RequiredJob => JobType.Sorcerer;
        public override int RequiredLevel => 64;
        public override int SkillPointCost => 1;
        public override int MaxRank => 10;
        public override string IconTexture => $"{RpgConstants.ICON_PATH}Skills/ManaRegeneration";
        public override float CooldownSeconds => 0f;
        public override int ResourceCost => 0;
        public override ResourceType ResourceType => ResourceType.None;

        private static readonly int[] MANA_REGEN = { 2, 4, 6, 8, 10, 12, 15, 18, 22, 28 };

        public override void ApplyPassive(Player player)
        {
            if (CurrentRank <= 0) return;
            int rank = CurrentRank;
            
            player.manaRegen += MANA_REGEN[rank - 1];
        }
    }
}
