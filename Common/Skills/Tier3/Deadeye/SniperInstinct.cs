using Terraria;
using Terraria.ModLoader;
using RpgMod.Common.Base;

namespace RpgMod.Common.Skills.Tier3.Deadeye
{
    /// <summary>
    /// Sniper Instinct - Deadeye's passive for ranged mastery.
    /// </summary>
    public class SniperInstinct : BaseSkill
    {
        public override string InternalName => "SniperInstinct";
        public override string DisplayName => "Sniper Instinct";
        public override string Description => "Greatly increase ranged damage and critical chance.";

        public override SkillType SkillType => SkillType.Passive;
        public override JobType RequiredJob => JobType.Deadeye;
        public override int RequiredLevel => RpgConstants.THIRD_JOB_LEVEL + 15;
        public override int SkillPointCost => 2;
        public override int MaxRank => 5;
        public override string IconTexture => $"{RpgConstants.ICON_PATH}Skills/SniperInstinct";
        public override float CooldownSeconds => 0f;
        public override int ResourceCost => 0;
        public override ResourceType ResourceType => ResourceType.None;

        private static readonly float[] DAMAGE_BONUS = { 0.06f, 0.10f, 0.14f, 0.18f, 0.25f };
        private static readonly int[] CRIT_BONUS = { 5, 8, 11, 14, 20 };

        public override void ApplyPassive(Player player)
        {
            if (CurrentRank <= 0) return;
            int rank = CurrentRank;
            player.GetDamage(DamageClass.Ranged) += DAMAGE_BONUS[rank - 1];
            player.GetCritChance(DamageClass.Ranged) += CRIT_BONUS[rank - 1];
        }
    }
}
