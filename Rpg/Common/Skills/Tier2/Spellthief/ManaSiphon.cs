using Terraria;
using Rpg.Common.Base;

namespace Rpg.Common.Skills.Tier2.Spellthief
{
    /// <summary>
    /// Mana Siphon - 마나 흡수.
    /// 공격 시 마나 회복 확률.
    /// 스펠시프의 마나 회수 패시브.
    /// </summary>
    public class ManaSiphon : BaseSkill
    {
        public override string InternalName => "ManaSiphon";
        public override string DisplayName => "Mana Siphon";
        public override string Description => "Chance to restore mana when attacking.";

        public override SkillType SkillType => SkillType.Passive;
        public override JobType RequiredJob => JobType.Spellthief;
        public override int RequiredLevel => 72;
        public override int SkillPointCost => 1;
        public override int MaxRank => 10;
        public override string IconTexture => $"{RpgConstants.ICON_PATH}Skills/ManaSiphon";
        public override float CooldownSeconds => 0f;
        public override int ResourceCost => 0;
        public override ResourceType ResourceType => ResourceType.None;

        // 마나 회복 확률과 양 (RpgPlayer.OnHitNPC에서 처리)
        private static readonly float[] SIPHON_CHANCE = { 0.08f, 0.12f, 0.16f, 0.20f, 0.24f, 0.28f, 0.32f, 0.36f, 0.42f, 0.55f };
        private static readonly int[] MANA_AMOUNT = { 5, 7, 9, 11, 14, 17, 20, 24, 29, 38 };

        public static float GetSiphonChance(int rank)
        {
            return SIPHON_CHANCE[System.Math.Clamp(rank - 1, 0, 9)];
        }

        public static int GetManaAmount(int rank)
        {
            return MANA_AMOUNT[System.Math.Clamp(rank - 1, 0, 9)];
        }

        public override void ApplyPassive(Player player)
        {
            if (CurrentRank <= 0) return;
            
            // 마나 재생 보너스
            player.manaRegen += CurrentRank * 2;
        }
    }
}
