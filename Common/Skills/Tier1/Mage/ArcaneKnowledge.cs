using Terraria;
using RpgMod.Common.Base;

namespace RpgMod.Common.Skills.Tier1.Mage
{
    /// <summary>
    /// Arcane Knowledge - 비전 지식.
    /// 마법 공격이 적의 방어를 일부 무시.
    /// 메이지의 관통 패시브.
    /// </summary>
    public class ArcaneKnowledge : BaseSkill
    {
        public override string InternalName => "ArcaneKnowledge";
        public override string DisplayName => "Arcane Knowledge";
        public override string Description => "Your deep understanding of magic allows spells to partially ignore enemy defense.";

        public override SkillType SkillType => SkillType.Passive;
        public override JobType RequiredJob => JobType.Mage;
        public override int RequiredLevel => 28;
        public override int SkillPointCost => 1;
        public override int MaxRank => 10;
        public override string IconTexture => $"{RpgConstants.ICON_PATH}Skills/ArcaneKnowledge";
        public override float CooldownSeconds => 0f;
        public override int ResourceCost => 0;
        public override ResourceType ResourceType => ResourceType.None;

        private static readonly int[] ARMOR_PEN = { 2, 4, 6, 8, 10, 12, 14, 17, 20, 25 };

        public override void ApplyPassive(Player player)
        {
            if (CurrentRank <= 0) return;
            int rank = CurrentRank;
            
            player.GetArmorPenetration(Terraria.ModLoader.DamageClass.Magic) += ARMOR_PEN[rank - 1];
        }
    }
}
