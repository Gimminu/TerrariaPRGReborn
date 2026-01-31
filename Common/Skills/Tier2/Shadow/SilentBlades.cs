using Terraria;
using Terraria.ModLoader;
using RpgMod.Common.Base;

namespace RpgMod.Common.Skills.Tier2.Shadow
{
    /// <summary>
    /// Silent Blades - Shadow's passive for ranged-melee hybrid.
    /// </summary>
    public class SilentBlades : BaseSkill
    {
        public override string InternalName => "SilentBlades";
        public override string DisplayName => "Silent Blades";
        public override string Description => "Increase melee and ranged damage, and crit chance.";

        public override SkillType SkillType => SkillType.Passive;
        public override JobType RequiredJob => JobType.Shadow;
        public override int RequiredLevel => RpgConstants.SECOND_JOB_LEVEL + 15;
        public override int SkillPointCost => 1;
        public override int MaxRank => 5;
        public override string IconTexture => $"{RpgConstants.ICON_PATH}Skills/SilentBlades";
        public override float CooldownSeconds => 0f;
        public override int ResourceCost => 0;
        public override ResourceType ResourceType => ResourceType.None;

        private static readonly float[] DAMAGE_BONUS = { 0.03f, 0.05f, 0.07f, 0.09f, 0.12f };
        private static readonly int[] CRIT_BONUS = { 2, 4, 6, 8, 11 };

        public override void ApplyPassive(Player player)
        {
            if (CurrentRank <= 0) return;
            int rank = CurrentRank;
            player.GetDamage(DamageClass.Melee) += DAMAGE_BONUS[rank - 1];
            player.GetDamage(DamageClass.Ranged) += DAMAGE_BONUS[rank - 1];
            player.GetCritChance(DamageClass.Melee) += CRIT_BONUS[rank - 1];
            player.GetCritChance(DamageClass.Ranged) += CRIT_BONUS[rank - 1];
        }
    }
}
