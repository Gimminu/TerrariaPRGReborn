using Terraria;
using Terraria.ModLoader;
using Rpg.Common.Base;

namespace Rpg.Common.Skills.Tier2.Spellblade
{
    /// <summary>
    /// Spell Strike - Spellblade's passive for hybrid damage.
    /// </summary>
    public class SpellStrike : BaseSkill
    {
        public override string InternalName => "SpellStrike";
        public override string DisplayName => "Spell Strike";
        public override string Description => "Increase both melee and magic damage.";

        public override SkillType SkillType => SkillType.Passive;
        public override JobType RequiredJob => JobType.Spellblade;
        public override int RequiredLevel => RpgConstants.SECOND_JOB_LEVEL + 15;
        public override int SkillPointCost => 1;
        public override int MaxRank => 5;
        public override string IconTexture => $"{RpgConstants.ICON_PATH}Skills/SpellStrike";
        public override float CooldownSeconds => 0f;
        public override int ResourceCost => 0;
        public override ResourceType ResourceType => ResourceType.None;

        private static readonly float[] MELEE_BONUS = { 0.03f, 0.05f, 0.07f, 0.09f, 0.12f };
        private static readonly float[] MAGIC_BONUS = { 0.03f, 0.05f, 0.07f, 0.09f, 0.12f };

        public override void ApplyPassive(Player player)
        {
            if (CurrentRank <= 0) return;
            int rank = CurrentRank;
            player.GetDamage(DamageClass.Melee) += MELEE_BONUS[rank - 1];
            player.GetDamage(DamageClass.Magic) += MAGIC_BONUS[rank - 1];
        }
    }
}
