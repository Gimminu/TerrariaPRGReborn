using Terraria;
using Terraria.ModLoader;
using Rpg.Common.Base;

namespace Rpg.Common.Skills.Tier2.Warlock
{
    /// <summary>
    /// Curse Expert - Warlock's passive for debuff effectiveness.
    /// </summary>
    public class CurseExpert : BaseSkill
    {
        public override string InternalName => "CurseExpert";
        public override string DisplayName => "Curse Expert";
        public override string Description => "Increase magic damage and mana efficiency.";

        public override SkillType SkillType => SkillType.Passive;
        public override JobType RequiredJob => JobType.Warlock;
        public override int RequiredLevel => RpgConstants.SECOND_JOB_LEVEL + 15;
        public override int SkillPointCost => 1;
        public override int MaxRank => 5;
        public override string IconTexture => $"{RpgConstants.ICON_PATH}Skills/CurseExpert";
        public override float CooldownSeconds => 0f;
        public override int ResourceCost => 0;
        public override ResourceType ResourceType => ResourceType.None;

        private static readonly float[] DAMAGE_BONUS = { 0.05f, 0.08f, 0.11f, 0.14f, 0.18f };
        private static readonly float[] MANA_REDUCTION = { 0.04f, 0.06f, 0.08f, 0.10f, 0.14f };

        public override void ApplyPassive(Player player)
        {
            if (CurrentRank <= 0) return;
            int rank = CurrentRank;
            
            player.GetDamage(DamageClass.Magic) += DAMAGE_BONUS[rank - 1];
            player.manaCost -= MANA_REDUCTION[rank - 1];
            if (player.manaCost < 0.2f) player.manaCost = 0.2f;
        }
    }
}
