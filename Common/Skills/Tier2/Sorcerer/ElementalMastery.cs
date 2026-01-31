using Terraria;
using Terraria.ModLoader;
using RpgMod.Common.Base;

namespace RpgMod.Common.Skills.Tier2.Sorcerer
{
    /// <summary>
    /// Elemental Mastery - Sorcerer's passive for elemental power.
    /// </summary>
    public class ElementalMastery : BaseSkill
    {
        public override string InternalName => "ElementalMastery";
        public override string DisplayName => "Elemental Mastery";
        public override string Description => "Master the elements to boost magic damage and mana.";

        public override SkillType SkillType => SkillType.Passive;
        public override JobType RequiredJob => JobType.Sorcerer;
        public override int RequiredLevel => RpgConstants.SECOND_JOB_LEVEL + 15;
        public override int SkillPointCost => 1;
        public override int MaxRank => 5;
        public override string IconTexture => $"{RpgConstants.ICON_PATH}Skills/ElementalMastery";
        public override float CooldownSeconds => 0f;
        public override int ResourceCost => 0;
        public override ResourceType ResourceType => ResourceType.None;

        private static readonly float[] DAMAGE_BONUS = { 0.04f, 0.06f, 0.08f, 0.10f, 0.14f };
        private static readonly int[] MANA_BONUS = { 10, 20, 30, 45, 65 };

        public override void ApplyPassive(Player player)
        {
            if (CurrentRank <= 0) return;
            int rank = CurrentRank;
            player.GetDamage(DamageClass.Magic) += DAMAGE_BONUS[rank - 1];
            player.statManaMax2 += MANA_BONUS[rank - 1];
        }
    }
}
