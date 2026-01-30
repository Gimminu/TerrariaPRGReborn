using Terraria;
using Terraria.ModLoader;
using Rpg.Common.Base;

namespace Rpg.Common.Skills.Tier2.Beastmaster
{
    /// <summary>
    /// Wild Bond - Beastmaster's passive for summoner synergy.
    /// </summary>
    public class WildBond : BaseSkill
    {
        public override string InternalName => "WildBond";
        public override string DisplayName => "Wild Bond";
        public override string Description => "Increase minion count and summon damage.";

        public override SkillType SkillType => SkillType.Passive;
        public override JobType RequiredJob => JobType.Beastmaster;
        public override int RequiredLevel => RpgConstants.SECOND_JOB_LEVEL + 15;
        public override int SkillPointCost => 1;
        public override int MaxRank => 5;
        public override string IconTexture => $"{RpgConstants.ICON_PATH}Skills/WildBond";
        public override float CooldownSeconds => 0f;
        public override int ResourceCost => 0;
        public override ResourceType ResourceType => ResourceType.None;

        private static readonly float[] SUMMON_DAMAGE = { 0.04f, 0.06f, 0.08f, 0.10f, 0.14f };

        public override void ApplyPassive(Player player)
        {
            if (CurrentRank <= 0) return;
            int rank = CurrentRank;
            player.GetDamage(DamageClass.Summon) += SUMMON_DAMAGE[rank - 1];
            if (rank >= 3) player.maxMinions += 1;
            if (rank >= 5) player.maxMinions += 1;
        }
    }
}
