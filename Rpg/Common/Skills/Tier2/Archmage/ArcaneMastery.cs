using Terraria;
using Terraria.ModLoader;
using Rpg.Common.Base;

namespace Rpg.Common.Skills.Tier2.Archmage
{
    /// <summary>
    /// Arcane Mastery - Archmage's passive for magic power.
    /// </summary>
    public class ArcaneMastery : BaseSkill
    {
        public override string InternalName => "ArcaneMastery";
        public override string DisplayName => "Arcane Mastery";
        public override string Description => "Increase magic damage and mana regeneration.";

        public override SkillType SkillType => SkillType.Passive;
        public override JobType RequiredJob => JobType.Archmage;
        public override int RequiredLevel => RpgConstants.SECOND_JOB_LEVEL + 15;
        public override int SkillPointCost => 1;
        public override int MaxRank => 5;
        public override string IconTexture => $"{RpgConstants.ICON_PATH}Skills/ArcaneMastery";
        public override float CooldownSeconds => 0f;
        public override int ResourceCost => 0;
        public override ResourceType ResourceType => ResourceType.None;

        private static readonly float[] DAMAGE_BONUS = { 0.04f, 0.07f, 0.10f, 0.13f, 0.18f };
        private static readonly int[] MANA_REGEN = { 2, 3, 4, 5, 7 };

        public override void ApplyPassive(Player player)
        {
            if (CurrentRank <= 0) return;
            int rank = CurrentRank;
            player.GetDamage(DamageClass.Magic) += DAMAGE_BONUS[rank - 1];
            player.manaRegen += MANA_REGEN[rank - 1];
        }
    }
}
