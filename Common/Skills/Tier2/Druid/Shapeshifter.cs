using Terraria;
using Terraria.ModLoader;
using RpgMod.Common.Base;

namespace RpgMod.Common.Skills.Tier2.Druid
{
    /// <summary>
    /// Shapeshifter - Druid's passive for hybrid bonuses.
    /// </summary>
    public class Shapeshifter : BaseSkill
    {
        public override string InternalName => "Shapeshifter";
        public override string DisplayName => "Shapeshifter";
        public override string Description => "Gain bonuses to summoning, magic, and life regeneration.";

        public override SkillType SkillType => SkillType.Passive;
        public override JobType RequiredJob => JobType.Druid;
        public override int RequiredLevel => RpgConstants.SECOND_JOB_LEVEL + 15;
        public override int SkillPointCost => 1;
        public override int MaxRank => 5;
        public override string IconTexture => $"{RpgConstants.ICON_PATH}Skills/Shapeshifter";
        public override float CooldownSeconds => 0f;
        public override int ResourceCost => 0;
        public override ResourceType ResourceType => ResourceType.None;

        private static readonly float[] SUMMON_BONUS = { 0.03f, 0.05f, 0.07f, 0.09f, 0.12f };
        private static readonly float[] MAGIC_BONUS = { 0.03f, 0.05f, 0.07f, 0.09f, 0.12f };
        private static readonly int[] LIFE_REGEN = { 1, 1, 2, 2, 3 };

        public override void ApplyPassive(Player player)
        {
            if (CurrentRank <= 0) return;
            int rank = CurrentRank;
            player.GetDamage(DamageClass.Summon) += SUMMON_BONUS[rank - 1];
            player.GetDamage(DamageClass.Magic) += MAGIC_BONUS[rank - 1];
            player.lifeRegen += LIFE_REGEN[rank - 1];
        }
    }
}
