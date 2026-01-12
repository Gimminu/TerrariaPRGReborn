using Terraria;
using Terraria.ModLoader;
using Rpg.Common.Base;

namespace Rpg.Common.Skills.Tier2.Archmage
{
    /// <summary>
    /// Spell Penetration - 주문 관통.
    /// 마법 크리티컬 증가.
    /// 대마법사의 크리티컬 패시브.
    /// </summary>
    public class SpellPenetration : BaseSkill
    {
        public override string InternalName => "SpellPenetration";
        public override string DisplayName => "Spell Penetration";
        public override string Description => "Increase magic critical strike chance.";

        public override SkillType SkillType => SkillType.Passive;
        public override JobType RequiredJob => JobType.Archmage;
        public override int RequiredLevel => 72;
        public override int SkillPointCost => 1;
        public override int MaxRank => 10;
        public override string IconTexture => $"{RpgConstants.ICON_PATH}Skills/SpellPenetration";
        public override float CooldownSeconds => 0f;
        public override int ResourceCost => 0;
        public override ResourceType ResourceType => ResourceType.None;

        private static readonly int[] CRIT_BONUS = { 3, 5, 8, 10, 13, 15, 18, 21, 25, 32 };

        public override void ApplyPassive(Player player)
        {
            if (CurrentRank <= 0) return;
            int rank = CurrentRank;
            
            player.GetCritChance(DamageClass.Magic) += CRIT_BONUS[rank - 1];
        }
    }
}
