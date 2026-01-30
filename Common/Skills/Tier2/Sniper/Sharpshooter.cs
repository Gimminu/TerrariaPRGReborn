using Terraria;
using Terraria.ModLoader;
using Rpg.Common.Base;

namespace Rpg.Common.Skills.Tier2.Sniper
{
    /// <summary>
    /// Sharpshooter - Sniper's passive for ranged mastery.
    /// </summary>
    public class Sharpshooter : BaseSkill
    {
        public override string InternalName => "Sharpshooter";
        public override string DisplayName => "Sharpshooter";
        public override string Description => "Increase ranged damage and critical strike chance.";

        public override SkillType SkillType => SkillType.Passive;
        public override JobType RequiredJob => JobType.Sniper;
        public override int RequiredLevel => RpgConstants.SECOND_JOB_LEVEL + 15;
        public override int SkillPointCost => 1;
        public override int MaxRank => 5;
        public override string IconTexture => $"{RpgConstants.ICON_PATH}Skills/Sharpshooter";
        public override float CooldownSeconds => 0f;
        public override int ResourceCost => 0;
        public override ResourceType ResourceType => ResourceType.None;

        private static readonly float[] DAMAGE_BONUS = { 0.04f, 0.06f, 0.08f, 0.10f, 0.14f };
        private static readonly int[] CRIT_BONUS = { 3, 5, 7, 9, 12 };

        public override void ApplyPassive(Player player)
        {
            if (CurrentRank <= 0) return;
            int rank = CurrentRank;
            player.GetDamage(DamageClass.Ranged) += DAMAGE_BONUS[rank - 1];
            player.GetCritChance(DamageClass.Ranged) += CRIT_BONUS[rank - 1];
        }
    }
}
