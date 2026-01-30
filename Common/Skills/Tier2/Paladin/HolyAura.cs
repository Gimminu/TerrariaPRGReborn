using Terraria;
using Rpg.Common.Base;

namespace Rpg.Common.Skills.Tier2.Paladin
{
    /// <summary>
    /// Holy Aura - Paladin's passive that boosts defense and regeneration.
    /// </summary>
    public class HolyAura : BaseSkill
    {
        public override string InternalName => "HolyAura";
        public override string DisplayName => "Holy Aura";
        public override string Description => "Passively increase defense and life regeneration.";

        public override SkillType SkillType => SkillType.Passive;
        public override JobType RequiredJob => JobType.Paladin;
        public override int RequiredLevel => RpgConstants.SECOND_JOB_LEVEL + 15;
        public override int SkillPointCost => 1;
        public override int MaxRank => 5;
        public override string IconTexture => $"{RpgConstants.ICON_PATH}Skills/HolyAura";
        public override float CooldownSeconds => 0f;
        public override int ResourceCost => 0;
        public override ResourceType ResourceType => ResourceType.None;

        private static readonly int[] DEFENSE_BONUS = { 4, 6, 8, 10, 14 };
        private static readonly int[] LIFE_REGEN = { 1, 2, 2, 3, 4 };

        public override void ApplyPassive(Player player)
        {
            if (CurrentRank <= 0) return;
            int rank = CurrentRank;
            player.statDefense += DEFENSE_BONUS[rank - 1];
            player.lifeRegen += LIFE_REGEN[rank - 1];
        }
    }
}
