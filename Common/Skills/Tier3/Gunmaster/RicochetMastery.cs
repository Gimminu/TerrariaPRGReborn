using Terraria;
using Terraria.ModLoader;
using Terraria.ID;
using Rpg.Common.Base;

namespace Rpg.Common.Skills.Tier3.Gunmaster
{
    /// <summary>
    /// Ricochet Mastery - Gunmaster's passive for bouncing shots.
    /// </summary>
    public class RicochetMastery : BaseSkill
    {
        public override string InternalName => "RicochetMastery";
        public override string DisplayName => "Ricochet Mastery";
        public override string Description => "Increase ranged damage and attack speed significantly.";

        public override SkillType SkillType => SkillType.Passive;
        public override JobType RequiredJob => JobType.Gunmaster;
        public override int RequiredLevel => RpgConstants.THIRD_JOB_LEVEL + 15;
        public override int SkillPointCost => 2;
        public override int MaxRank => 5;
        public override string IconTexture => $"{RpgConstants.ICON_PATH}Skills/RicochetMastery";
        public override float CooldownSeconds => 0f;
        public override int ResourceCost => 0;
        public override ResourceType ResourceType => ResourceType.None;

        private static readonly float[] DAMAGE_BONUS = { 0.08f, 0.12f, 0.16f, 0.20f, 0.28f };
        private static readonly float[] ATTACK_SPEED = { 0.05f, 0.08f, 0.11f, 0.14f, 0.18f };
        private static readonly int[] CRIT_BONUS = { 4, 6, 8, 10, 14 };

        public override void ApplyPassive(Player player)
        {
            if (CurrentRank <= 0) return;
            int rank = CurrentRank;
            player.GetDamage(DamageClass.Ranged) += DAMAGE_BONUS[rank - 1];
            player.GetAttackSpeed(DamageClass.Ranged) += ATTACK_SPEED[rank - 1];
            player.GetCritChance(DamageClass.Ranged) += CRIT_BONUS[rank - 1];
        }
    }
}
