using Terraria;
using Terraria.ModLoader;
using Rpg.Common.Base;

namespace Rpg.Common.Skills.Tier2.Spellblade
{
    /// <summary>
    /// Hybrid Mastery - 하이브리드 숙련.
    /// 근접과 마법 피해 모두 증가.
    /// 마검사의 핵심 패시브.
    /// </summary>
    public class HybridMastery : BaseSkill
    {
        public override string InternalName => "HybridMastery";
        public override string DisplayName => "Hybrid Mastery";
        public override string Description => "Increase both melee and magic damage.";

        public override SkillType SkillType => SkillType.Passive;
        public override JobType RequiredJob => JobType.Spellblade;
        public override int RequiredLevel => 60;
        public override int SkillPointCost => 1;
        public override int MaxRank => 10;
        public override string IconTexture => $"{RpgConstants.ICON_PATH}Skills/HybridMastery";
        public override float CooldownSeconds => 0f;
        public override int ResourceCost => 0;
        public override ResourceType ResourceType => ResourceType.None;

        private static readonly float[] DAMAGE_BONUS = { 0.03f, 0.05f, 0.08f, 0.10f, 0.13f, 0.16f, 0.19f, 0.22f, 0.26f, 0.32f };

        public override void ApplyPassive(Player player)
        {
            if (CurrentRank <= 0) return;
            int rank = CurrentRank;
            
            player.GetDamage(DamageClass.Melee) += DAMAGE_BONUS[rank - 1];
            player.GetDamage(DamageClass.Magic) += DAMAGE_BONUS[rank - 1];
        }
    }
}
