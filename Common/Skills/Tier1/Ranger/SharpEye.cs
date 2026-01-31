using Terraria;
using Terraria.ModLoader;
using RpgMod.Common.Base;

namespace RpgMod.Common.Skills.Tier1.Ranger
{
    /// <summary>
    /// Sharp Eye - 날카로운 눈.
    /// 원거리 피해와 크리티컬 증가.
    /// 레인저의 핵심 공격 패시브.
    /// </summary>
    public class SharpEye : BaseSkill
    {
        public override string InternalName => "SharpEye";
        public override string DisplayName => "Sharp Eye";
        public override string Description => "Your keen eyes find weak spots, increasing ranged damage and crit chance.";

        public override SkillType SkillType => SkillType.Passive;
        public override JobType RequiredJob => JobType.Ranger;
        public override int RequiredLevel => 10;
        public override int SkillPointCost => 1;
        public override int MaxRank => 10;
        public override string IconTexture => $"{RpgConstants.ICON_PATH}Skills/SharpEye";
        public override float CooldownSeconds => 0f;
        public override int ResourceCost => 0;
        public override ResourceType ResourceType => ResourceType.None;

        private static readonly float[] DAMAGE_BONUS = { 0.02f, 0.04f, 0.06f, 0.08f, 0.10f, 0.12f, 0.14f, 0.16f, 0.18f, 0.20f };
        private static readonly int[] CRIT_BONUS = { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };

        public override void ApplyPassive(Player player)
        {
            if (CurrentRank <= 0) return;
            int rank = CurrentRank;
            
            player.GetDamage(DamageClass.Ranged) += DAMAGE_BONUS[rank - 1];
            player.GetCritChance(DamageClass.Ranged) += CRIT_BONUS[rank - 1];
        }
    }
}
