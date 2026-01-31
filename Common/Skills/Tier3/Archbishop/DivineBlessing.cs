using Terraria;
using RpgMod.Common.Base;

namespace RpgMod.Common.Skills.Tier3.Archbishop
{
    /// <summary>
    /// Divine Blessing - Archbishop's passive for healing mastery.
    /// </summary>
    public class DivineBlessing : BaseSkill
    {
        public override string InternalName => "DivineBlessing";
        public override string DisplayName => "Divine Blessing";
        public override string Description => "Greatly increase life, life regen, and mana regeneration.";

        public override SkillType SkillType => SkillType.Passive;
        public override JobType RequiredJob => JobType.Archbishop;
        public override int RequiredLevel => RpgConstants.THIRD_JOB_LEVEL + 15;
        public override int SkillPointCost => 2;
        public override int MaxRank => 5;
        public override string IconTexture => $"{RpgConstants.ICON_PATH}Skills/DivineBlessing";
        public override float CooldownSeconds => 0f;
        public override int ResourceCost => 0;
        public override ResourceType ResourceType => ResourceType.None;

        private static readonly int[] LIFE_REGEN = { 4, 6, 8, 10, 14 };
        private static readonly int[] MANA_REGEN = { 3, 5, 7, 9, 12 };
        private static readonly int[] MAX_LIFE = { 25, 40, 60, 85, 120 };

        public override void ApplyPassive(Player player)
        {
            if (CurrentRank <= 0) return;
            int rank = CurrentRank;
            player.lifeRegen += LIFE_REGEN[rank - 1];
            player.manaRegen += MANA_REGEN[rank - 1];
            player.statLifeMax2 += MAX_LIFE[rank - 1];
        }
    }
}
