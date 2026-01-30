using Terraria;
using Terraria.ModLoader;
using Rpg.Common.Base;

namespace Rpg.Common.Skills.Tier2.Spellthief
{
    /// <summary>
    /// Arcane Larceny - 신비 절도.
    /// 원거리와 마법 피해 증가.
    /// 스펠시프의 핵심 패시브.
    /// </summary>
    public class ArcaneLarceny : BaseSkill
    {
        public override string InternalName => "ArcaneLarceny";
        public override string DisplayName => "Arcane Larceny";
        public override string Description => "Increase ranged and magic damage.";

        public override SkillType SkillType => SkillType.Passive;
        public override JobType RequiredJob => JobType.Spellthief;
        public override int RequiredLevel => 60;
        public override int SkillPointCost => 1;
        public override int MaxRank => 10;
        public override string IconTexture => $"{RpgConstants.ICON_PATH}Skills/ArcaneLarceny";
        public override float CooldownSeconds => 0f;
        public override int ResourceCost => 0;
        public override ResourceType ResourceType => ResourceType.None;

        private static readonly float[] DAMAGE_BONUS = { 0.03f, 0.05f, 0.07f, 0.09f, 0.12f, 0.15f, 0.18f, 0.21f, 0.25f, 0.32f };

        public override void ApplyPassive(Player player)
        {
            if (CurrentRank <= 0) return;
            int rank = CurrentRank;
            
            player.GetDamage(DamageClass.Ranged) += DAMAGE_BONUS[rank - 1];
            player.GetDamage(DamageClass.Magic) += DAMAGE_BONUS[rank - 1];
        }
    }
}
