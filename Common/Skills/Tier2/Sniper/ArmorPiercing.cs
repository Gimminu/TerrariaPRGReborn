using Terraria;
using Terraria.ModLoader;
using Rpg.Common.Base;

namespace Rpg.Common.Skills.Tier2.Sniper
{
    /// <summary>
    /// Armor Piercing - 철갑탄.
    /// 적 방어력 무시.
    /// 저격수의 관통 패시브.
    /// </summary>
    public class ArmorPiercing : BaseSkill
    {
        public override string InternalName => "ArmorPiercing";
        public override string DisplayName => "Armor Piercing";
        public override string Description => "Your shots pierce through enemy armor.";

        public override SkillType SkillType => SkillType.Passive;
        public override JobType RequiredJob => JobType.Sniper;
        public override int RequiredLevel => 70;
        public override int SkillPointCost => 1;
        public override int MaxRank => 10;
        public override string IconTexture => $"{RpgConstants.ICON_PATH}Skills/ArmorPiercing";
        public override float CooldownSeconds => 0f;
        public override int ResourceCost => 0;
        public override ResourceType ResourceType => ResourceType.None;

        private static readonly int[] ARMOR_PEN = { 5, 8, 11, 14, 17, 20, 24, 28, 32, 40 };

        public override void ApplyPassive(Player player)
        {
            if (CurrentRank <= 0) return;
            int rank = CurrentRank;
            
            player.GetArmorPenetration(DamageClass.Ranged) += ARMOR_PEN[rank - 1];
        }
    }
}
