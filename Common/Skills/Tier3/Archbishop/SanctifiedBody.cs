using Terraria;
using Rpg.Common.Base;

namespace Rpg.Common.Skills.Tier3.Archbishop
{
    /// <summary>
    /// Sanctified Body - 성화된 육체.
    /// 최대 체력과 마나 대폭 증가.
    /// 대주교의 스탯 패시브.
    /// </summary>
    public class SanctifiedBody : BaseSkill
    {
        public override string InternalName => "SanctifiedBody";
        public override string DisplayName => "Sanctified Body";
        public override string Description => "Greatly increase maximum health and mana.";

        public override SkillType SkillType => SkillType.Passive;
        public override JobType RequiredJob => JobType.Archbishop;
        public override int RequiredLevel => 135;
        public override int SkillPointCost => 1;
        public override int MaxRank => 10;
        public override string IconTexture => $"{RpgConstants.ICON_PATH}Skills/SanctifiedBody";
        public override float CooldownSeconds => 0f;
        public override int ResourceCost => 0;
        public override ResourceType ResourceType => ResourceType.None;

        private static readonly int[] HP_BONUS = { 40, 80, 130, 190, 260, 340, 430, 540, 670, 880 };
        private static readonly int[] MANA_BONUS = { 30, 55, 85, 120, 160, 210, 265, 330, 410, 530 };

        public override void ApplyPassive(Player player)
        {
            if (CurrentRank <= 0) return;
            int rank = CurrentRank;
            
            player.statLifeMax2 += HP_BONUS[rank - 1];
            player.statManaMax2 += MANA_BONUS[rank - 1];
        }
    }
}
