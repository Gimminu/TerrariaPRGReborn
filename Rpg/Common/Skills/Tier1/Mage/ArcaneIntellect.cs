using Terraria;
using Terraria.ModLoader;
using Rpg.Common.Base;

namespace Rpg.Common.Skills.Tier1.Mage
{
    /// <summary>
    /// Arcane Intellect - Mage's passive skill.
    /// Increases magic damage and reduces mana costs.
    /// </summary>
    public class ArcaneIntellect : BaseSkill
    {
        public override string InternalName => "ArcaneIntellect";
        public override string DisplayName => "Arcane Intellect";
        public override string Description => "Increase magic output and improve mana efficiency.";

        public override SkillType SkillType => SkillType.Passive;
        public override JobType RequiredJob => JobType.Mage;
        public override int RequiredLevel => RpgConstants.FIRST_JOB_LEVEL + 10;
        public override int SkillPointCost => 1;
        public override int MaxRank => 5;
        public override string IconTexture => $"{RpgConstants.ICON_PATH}Skills/ArcaneIntellect";
        public override float CooldownSeconds => 0f;
        public override int ResourceCost => 0;
        public override ResourceType ResourceType => ResourceType.None;

        // Scaling per rank
        private static readonly float[] DAMAGE_BONUS = { 0.04f, 0.06f, 0.08f, 0.10f, 0.14f };
        private static readonly float[] MANA_COST_REDUCTION = { 0.03f, 0.05f, 0.07f, 0.09f, 0.12f };

        public override void ApplyPassive(Player player)
        {
            if (CurrentRank <= 0)
                return;

            int rank = CurrentRank;

            // Apply magic damage bonus
            player.GetDamage(DamageClass.Magic) += DAMAGE_BONUS[rank - 1];

            // Apply mana cost reduction
            player.manaCost -= MANA_COST_REDUCTION[rank - 1];
            if (player.manaCost < 0.2f)
                player.manaCost = 0.2f; // Minimum 20% mana cost
        }
    }
}
