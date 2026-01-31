using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using RpgMod.Common.Base;

namespace RpgMod.Common.Skills.Tier1.Ranger
{
    /// <summary>
    /// Steady Aim - Ranger's critical hit passive.
    /// Increases ranged damage and critical strike chance.
    /// </summary>
    public class SteadyAim : BaseSkill
    {
        public override string InternalName => "SteadyAim";
        public override string DisplayName => "Steady Aim";
        public override string Description => "Increase critical chance and ranged damage passively.";

        public override SkillType SkillType => SkillType.Passive;
        public override JobType RequiredJob => JobType.Ranger;
        public override int RequiredLevel => RpgConstants.FIRST_JOB_LEVEL + 10;
        public override int SkillPointCost => 1;
        public override int MaxRank => 5;
        public override string IconTexture => $"{RpgConstants.ICON_PATH}Skills/SteadyAim";
        public override float CooldownSeconds => 0f;
        public override int ResourceCost => 0;
        public override ResourceType ResourceType => ResourceType.None;

        // Scaling per rank
        private static readonly float[] DAMAGE_BONUS = { 0.04f, 0.06f, 0.08f, 0.10f, 0.14f };
        private static readonly float[] CRIT_BONUS = { 4f, 6f, 8f, 10f, 14f };

        public override void ApplyPassive(Player player)
        {
            if (CurrentRank <= 0)
                return;

            int rank = CurrentRank;

            // Apply ranged damage bonus
            player.GetDamage(DamageClass.Ranged) += DAMAGE_BONUS[rank - 1];

            // Apply critical chance bonus
            player.GetCritChance(DamageClass.Ranged) += CRIT_BONUS[rank - 1];
        }
    }
}
