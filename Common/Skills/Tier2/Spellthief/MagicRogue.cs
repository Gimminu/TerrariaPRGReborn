using Terraria;
using Terraria.ModLoader;
using RpgMod.Common.Base;

namespace RpgMod.Common.Skills.Tier2.Spellthief
{
    /// <summary>
    /// Magic Rogue - Spellthief's passive for hybrid damage.
    /// </summary>
    public class MagicRogue : BaseSkill
    {
        public override string InternalName => "MagicRogue";
        public override string DisplayName => "Magic Rogue";
        public override string Description => "Increase ranged and magic damage while reducing mana costs.";

        public override SkillType SkillType => SkillType.Passive;
        public override JobType RequiredJob => JobType.Spellthief;
        public override int RequiredLevel => RpgConstants.SECOND_JOB_LEVEL + 15;
        public override int SkillPointCost => 1;
        public override int MaxRank => 5;
        public override string IconTexture => $"{RpgConstants.ICON_PATH}Skills/MagicRogue";
        public override float CooldownSeconds => 0f;
        public override int ResourceCost => 0;
        public override ResourceType ResourceType => ResourceType.None;

        private static readonly float[] RANGED_BONUS = { 0.03f, 0.05f, 0.07f, 0.09f, 0.12f };
        private static readonly float[] MAGIC_BONUS = { 0.03f, 0.05f, 0.07f, 0.09f, 0.12f };
        private static readonly float[] MANA_REDUCTION = { 0.03f, 0.04f, 0.05f, 0.06f, 0.08f };

        public override void ApplyPassive(Player player)
        {
            if (CurrentRank <= 0) return;
            int rank = CurrentRank;
            player.GetDamage(DamageClass.Ranged) += RANGED_BONUS[rank - 1];
            player.GetDamage(DamageClass.Magic) += MAGIC_BONUS[rank - 1];
            player.manaCost -= MANA_REDUCTION[rank - 1];
            if (player.manaCost < 0.2f) player.manaCost = 0.2f;
        }
    }
}
