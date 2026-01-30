using Terraria;
using Rpg.Common.Base;

namespace Rpg.Common.Skills.Tier3.LichKing
{
    /// <summary>
    /// Eternal Darkness - 영원한 어둠.
    /// 소환수 슬롯과 마나 대폭 증가.
    /// 리치킹의 스탯 패시브.
    /// </summary>
    public class EternalDarkness : BaseSkill
    {
        public override string InternalName => "EternalDarkness";
        public override string DisplayName => "Eternal Darkness";
        public override string Description => "Embrace eternal darkness, greatly increasing minion slots and mana.";

        public override SkillType SkillType => SkillType.Passive;
        public override JobType RequiredJob => JobType.LichKing;
        public override int RequiredLevel => 135;
        public override int SkillPointCost => 1;
        public override int MaxRank => 10;
        public override string IconTexture => $"{RpgConstants.ICON_PATH}Skills/EternalDarkness";
        public override float CooldownSeconds => 0f;
        public override int ResourceCost => 0;
        public override ResourceType ResourceType => ResourceType.None;

        private static readonly int[] MINION_SLOTS = { 0, 1, 1, 2, 2, 2, 3, 3, 4, 5 };
        private static readonly int[] MANA_BONUS = { 30, 55, 85, 120, 160, 205, 260, 325, 405, 530 };

        public override void ApplyPassive(Player player)
        {
            if (CurrentRank <= 0) return;
            int rank = CurrentRank;
            
            player.maxMinions += MINION_SLOTS[rank - 1];
            player.statManaMax2 += MANA_BONUS[rank - 1];
        }
    }
}
