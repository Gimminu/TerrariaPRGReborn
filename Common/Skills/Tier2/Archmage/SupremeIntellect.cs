using Terraria;
using Terraria.ModLoader;
using Rpg.Common.Base;

namespace Rpg.Common.Skills.Tier2.Archmage
{
    /// <summary>
    /// Supreme Intellect - 최상의 지성.
    /// 최대 마나와 마법 피해 대폭 증가.
    /// 대마법사의 핵심 패시브.
    /// </summary>
    public class SupremeIntellect : BaseSkill
    {
        public override string InternalName => "SupremeIntellect";
        public override string DisplayName => "Supreme Intellect";
        public override string Description => "Greatly increase maximum mana and magic damage.";

        public override SkillType SkillType => SkillType.Passive;
        public override JobType RequiredJob => JobType.Archmage;
        public override int RequiredLevel => 60;
        public override int SkillPointCost => 1;
        public override int MaxRank => 10;
        public override string IconTexture => $"{RpgConstants.ICON_PATH}Skills/SupremeIntellect";
        public override float CooldownSeconds => 0f;
        public override int ResourceCost => 0;
        public override ResourceType ResourceType => ResourceType.None;

        private static readonly int[] MANA_BONUS = { 20, 40, 65, 90, 120, 155, 195, 245, 305, 400 };
        private static readonly float[] DAMAGE_BONUS = { 0.04f, 0.07f, 0.10f, 0.13f, 0.16f, 0.20f, 0.24f, 0.28f, 0.33f, 0.42f };

        public override void ApplyPassive(Player player)
        {
            if (CurrentRank <= 0) return;
            int rank = CurrentRank;
            
            player.statManaMax2 += MANA_BONUS[rank - 1];
            player.GetDamage(DamageClass.Magic) += DAMAGE_BONUS[rank - 1];
        }
    }
}
