using Terraria;
using Rpg.Common.Base;

namespace Rpg.Common.Skills.Tier2.Cleric
{
    /// <summary>
    /// Divine Grace - Cleric's passive for healing power.
    /// </summary>
    public class DivineGrace : BaseSkill
    {
        public override string InternalName => "DivineGrace";
        public override string DisplayName => "Divine Grace";
        public override string Description => "Increase life regeneration and max life.";

        public override SkillType SkillType => SkillType.Passive;
        public override JobType RequiredJob => JobType.Cleric;
        public override int RequiredLevel => RpgConstants.SECOND_JOB_LEVEL + 15;
        public override int SkillPointCost => 1;
        public override int MaxRank => 5;
        public override string IconTexture => $"{RpgConstants.ICON_PATH}Skills/DivineGrace";
        public override float CooldownSeconds => 0f;
        public override int ResourceCost => 0;
        public override ResourceType ResourceType => ResourceType.None;

        private static readonly int[] LIFE_REGEN = { 2, 3, 4, 5, 7 };
        private static readonly int[] MAX_LIFE = { 15, 25, 35, 50, 75 };

        public override void ApplyPassive(Player player)
        {
            if (CurrentRank <= 0) return;
            int rank = CurrentRank;
            player.lifeRegen += LIFE_REGEN[rank - 1];
            player.statLifeMax2 += MAX_LIFE[rank - 1];
        }
    }
}
