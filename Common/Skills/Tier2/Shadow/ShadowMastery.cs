using Terraria;
using Terraria.ModLoader;
using RpgMod.Common.Base;

namespace RpgMod.Common.Skills.Tier2.Shadow
{
    /// <summary>
    /// Shadow Mastery - 그림자 숙련.
    /// 근접과 원거리 피해 + 이동속도 증가.
    /// 그림자의 핵심 패시브.
    /// </summary>
    public class ShadowMastery : BaseSkill
    {
        public override string InternalName => "ShadowMastery";
        public override string DisplayName => "Shadow Mastery";
        public override string Description => "Increase melee, ranged damage and move speed.";

        public override SkillType SkillType => SkillType.Passive;
        public override JobType RequiredJob => JobType.Shadow;
        public override int RequiredLevel => 60;
        public override int SkillPointCost => 1;
        public override int MaxRank => 10;
        public override string IconTexture => $"{RpgConstants.ICON_PATH}Skills/ShadowMastery";
        public override float CooldownSeconds => 0f;
        public override int ResourceCost => 0;
        public override ResourceType ResourceType => ResourceType.None;

        private static readonly float[] DAMAGE_BONUS = { 0.03f, 0.05f, 0.07f, 0.09f, 0.11f, 0.14f, 0.17f, 0.20f, 0.24f, 0.30f };
        private static readonly float[] SPEED_BONUS = { 0.02f, 0.04f, 0.06f, 0.08f, 0.10f, 0.12f, 0.14f, 0.16f, 0.19f, 0.24f };

        public override void ApplyPassive(Player player)
        {
            if (CurrentRank <= 0) return;
            int rank = CurrentRank;
            
            player.GetDamage(DamageClass.Melee) += DAMAGE_BONUS[rank - 1];
            player.GetDamage(DamageClass.Ranged) += DAMAGE_BONUS[rank - 1];
            player.moveSpeed += SPEED_BONUS[rank - 1];
        }
    }
}
