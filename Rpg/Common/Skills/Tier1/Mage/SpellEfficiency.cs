using Terraria;
using Terraria.ModLoader;
using Rpg.Common.Base;

namespace Rpg.Common.Skills.Tier1.Mage
{
    /// <summary>
    /// Spell Efficiency - 주문 효율.
    /// 마나 소비 감소.
    /// 메이지의 자원 효율 패시브.
    /// </summary>
    public class SpellEfficiency : BaseSkill
    {
        public override string InternalName => "SpellEfficiency";
        public override string DisplayName => "Spell Efficiency";
        public override string Description => "Cast spells more efficiently, reducing mana cost.";

        public override SkillType SkillType => SkillType.Passive;
        public override JobType RequiredJob => JobType.Mage;
        public override int RequiredLevel => 20;
        public override int SkillPointCost => 1;
        public override int MaxRank => 10;
        public override string IconTexture => $"{RpgConstants.ICON_PATH}Skills/SpellEfficiency";
        public override float CooldownSeconds => 0f;
        public override int ResourceCost => 0;
        public override ResourceType ResourceType => ResourceType.None;

        private static readonly float[] MANA_REDUCTION = { 0.02f, 0.04f, 0.06f, 0.08f, 0.10f, 0.12f, 0.14f, 0.16f, 0.18f, 0.20f };

        public override void ApplyPassive(Player player)
        {
            if (CurrentRank <= 0) return;
            int rank = CurrentRank;
            
            player.manaCost -= MANA_REDUCTION[rank - 1];
        }
    }
}
