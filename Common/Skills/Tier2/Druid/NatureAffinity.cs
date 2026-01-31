using Terraria;
using Terraria.ModLoader;
using RpgMod.Common.Base;

namespace RpgMod.Common.Skills.Tier2.Druid
{
    /// <summary>
    /// Nature Affinity - 자연 친화.
    /// 소환수와 마법 피해 + 재생 증가.
    /// 드루이드의 핵심 패시브.
    /// </summary>
    public class NatureAffinity : BaseSkill
    {
        public override string InternalName => "NatureAffinity";
        public override string DisplayName => "Nature Affinity";
        public override string Description => "Bond with nature, increasing damage and regeneration.";

        public override SkillType SkillType => SkillType.Passive;
        public override JobType RequiredJob => JobType.Druid;
        public override int RequiredLevel => 60;
        public override int SkillPointCost => 1;
        public override int MaxRank => 10;
        public override string IconTexture => $"{RpgConstants.ICON_PATH}Skills/NatureAffinity";
        public override float CooldownSeconds => 0f;
        public override int ResourceCost => 0;
        public override ResourceType ResourceType => ResourceType.None;

        private static readonly float[] DAMAGE_BONUS = { 0.02f, 0.04f, 0.06f, 0.08f, 0.10f, 0.12f, 0.14f, 0.16f, 0.19f, 0.24f };
        private static readonly int[] REGEN_BONUS = { 1, 2, 3, 4, 5, 6, 7, 9, 11, 15 };

        public override void ApplyPassive(Player player)
        {
            if (CurrentRank <= 0) return;
            int rank = CurrentRank;
            
            player.GetDamage(DamageClass.Summon) += DAMAGE_BONUS[rank - 1];
            player.GetDamage(DamageClass.Magic) += DAMAGE_BONUS[rank - 1];
            player.lifeRegen += REGEN_BONUS[rank - 1];
        }
    }
}
