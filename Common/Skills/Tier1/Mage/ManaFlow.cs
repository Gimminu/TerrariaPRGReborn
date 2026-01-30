using Terraria;
using Rpg.Common.Base;

namespace Rpg.Common.Skills.Tier1.Mage
{
    /// <summary>
    /// Mana Flow - 마나 흐름.
    /// 최대 마나와 마나 재생 증가.
    /// 메이지의 자원 관리 패시브.
    /// </summary>
    public class ManaFlow : BaseSkill
    {
        public override string InternalName => "ManaFlow";
        public override string DisplayName => "Mana Flow";
        public override string Description => "Improve the flow of mana through your body, increasing max mana and regeneration.";

        public override SkillType SkillType => SkillType.Passive;
        public override JobType RequiredJob => JobType.Mage;
        public override int RequiredLevel => 14;
        public override int SkillPointCost => 1;
        public override int MaxRank => 10;
        public override string IconTexture => $"{RpgConstants.ICON_PATH}Skills/ManaFlow";
        public override float CooldownSeconds => 0f;
        public override int ResourceCost => 0;
        public override ResourceType ResourceType => ResourceType.None;

        private static readonly int[] MANA_BONUS = { 10, 20, 30, 45, 60, 75, 90, 110, 130, 160 };
        private static readonly int[] REGEN_BONUS = { 1, 2, 3, 4, 5, 6, 7, 8, 10, 12 };

        public override void ApplyPassive(Player player)
        {
            if (CurrentRank <= 0) return;
            int rank = CurrentRank;
            
            player.statManaMax2 += MANA_BONUS[rank - 1];
            player.manaRegen += REGEN_BONUS[rank - 1];
        }
    }
}
