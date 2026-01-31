using Terraria;
using RpgMod.Common.Base;

namespace RpgMod.Common.Skills.Tier2.Sorcerer
{
    /// <summary>
    /// Spell Amplification - 주문 증폭.
    /// 마나 소비 감소.
    /// 소서러의 효율 패시브.
    /// </summary>
    public class SpellAmplification : BaseSkill
    {
        public override string InternalName => "SpellAmplification";
        public override string DisplayName => "Spell Amplification";
        public override string Description => "Reduce mana cost of spells.";

        public override SkillType SkillType => SkillType.Passive;
        public override JobType RequiredJob => JobType.Sorcerer;
        public override int RequiredLevel => 70;
        public override int SkillPointCost => 1;
        public override int MaxRank => 10;
        public override string IconTexture => $"{RpgConstants.ICON_PATH}Skills/SpellAmplification";
        public override float CooldownSeconds => 0f;
        public override int ResourceCost => 0;
        public override ResourceType ResourceType => ResourceType.None;

        private static readonly float[] MANA_REDUCTION = { 0.02f, 0.04f, 0.06f, 0.08f, 0.10f, 0.12f, 0.14f, 0.17f, 0.20f, 0.25f };

        public override void ApplyPassive(Player player)
        {
            if (CurrentRank <= 0) return;
            int rank = CurrentRank;
            
            player.manaCost -= MANA_REDUCTION[rank - 1];
        }
    }
}
