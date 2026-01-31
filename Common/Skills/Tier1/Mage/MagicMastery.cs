using Terraria;
using Terraria.ModLoader;
using RpgMod.Common.Base;

namespace RpgMod.Common.Skills.Tier1.Mage
{
    /// <summary>
    /// Magic Mastery - 마법 숙련.
    /// 마법 피해와 크리티컬 증가.
    /// 메이지의 핵심 공격 패시브.
    /// </summary>
    public class MagicMastery : BaseSkill
    {
        public override string InternalName => "MagicMastery";
        public override string DisplayName => "Magic Mastery";
        public override string Description => "Master the arcane arts, increasing magic damage and critical chance.";

        public override SkillType SkillType => SkillType.Passive;
        public override JobType RequiredJob => JobType.Mage;
        public override int RequiredLevel => 10;
        public override int SkillPointCost => 1;
        public override int MaxRank => 10;
        public override string IconTexture => $"{RpgConstants.ICON_PATH}Skills/MagicMastery";
        public override float CooldownSeconds => 0f;
        public override int ResourceCost => 0;
        public override ResourceType ResourceType => ResourceType.None;

        private static readonly float[] DAMAGE_BONUS = { 0.02f, 0.04f, 0.06f, 0.08f, 0.10f, 0.12f, 0.14f, 0.16f, 0.18f, 0.20f };
        private static readonly int[] CRIT_BONUS = { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };

        public override void ApplyPassive(Player player)
        {
            if (CurrentRank <= 0) return;
            int rank = CurrentRank;
            
            player.GetDamage(DamageClass.Magic) += DAMAGE_BONUS[rank - 1];
            player.GetCritChance(DamageClass.Magic) += CRIT_BONUS[rank - 1];
        }
    }
}
