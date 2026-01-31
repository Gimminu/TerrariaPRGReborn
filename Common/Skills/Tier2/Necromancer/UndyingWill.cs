using Terraria;
using RpgMod.Common.Base;

namespace RpgMod.Common.Skills.Tier2.Necromancer
{
    /// <summary>
    /// Undying Will - 불사의 의지.
    /// 최대 체력과 소환수 슬롯 증가.
    /// 강령술사의 생존 패시브.
    /// </summary>
    public class UndyingWill : BaseSkill
    {
        public override string InternalName => "UndyingWill";
        public override string DisplayName => "Undying Will";
        public override string Description => "Increase max health and minion slots.";

        public override SkillType SkillType => SkillType.Passive;
        public override JobType RequiredJob => JobType.Necromancer;
        public override int RequiredLevel => 72;
        public override int SkillPointCost => 1;
        public override int MaxRank => 10;
        public override string IconTexture => $"{RpgConstants.ICON_PATH}Skills/UndyingWill";
        public override float CooldownSeconds => 0f;
        public override int ResourceCost => 0;
        public override ResourceType ResourceType => ResourceType.None;

        private static readonly int[] HP_BONUS = { 10, 20, 35, 50, 70, 95, 125, 160, 200, 260 };
        private static readonly int[] MINION_SLOTS = { 0, 0, 0, 1, 1, 1, 1, 2, 2, 3 };

        public override void ApplyPassive(Player player)
        {
            if (CurrentRank <= 0) return;
            int rank = CurrentRank;
            
            player.statLifeMax2 += HP_BONUS[rank - 1];
            player.maxMinions += MINION_SLOTS[rank - 1];
        }
    }
}
