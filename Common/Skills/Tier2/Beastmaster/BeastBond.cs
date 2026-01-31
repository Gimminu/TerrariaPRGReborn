using Terraria;
using Terraria.ModLoader;
using RpgMod.Common.Base;

namespace RpgMod.Common.Skills.Tier2.Beastmaster
{
    /// <summary>
    /// Beast Bond - 야수 유대.
    /// 소환수 피해와 지속시간 증가.
    /// 야수조련사의 핵심 패시브.
    /// </summary>
    public class BeastBond : BaseSkill
    {
        public override string InternalName => "BeastBond";
        public override string DisplayName => "Beast Bond";
        public override string Description => "Increase minion damage and whip speed.";

        public override SkillType SkillType => SkillType.Passive;
        public override JobType RequiredJob => JobType.Beastmaster;
        public override int RequiredLevel => 60;
        public override int SkillPointCost => 1;
        public override int MaxRank => 10;
        public override string IconTexture => $"{RpgConstants.ICON_PATH}Skills/BeastBond";
        public override float CooldownSeconds => 0f;
        public override int ResourceCost => 0;
        public override ResourceType ResourceType => ResourceType.None;

        private static readonly float[] DAMAGE_BONUS = { 0.04f, 0.07f, 0.10f, 0.13f, 0.16f, 0.20f, 0.24f, 0.28f, 0.33f, 0.42f };
        private static readonly float[] SPEED_BONUS = { 0.02f, 0.04f, 0.06f, 0.08f, 0.10f, 0.12f, 0.14f, 0.16f, 0.18f, 0.22f };

        public override void ApplyPassive(Player player)
        {
            if (CurrentRank <= 0) return;
            int rank = CurrentRank;
            
            player.GetDamage(DamageClass.Summon) += DAMAGE_BONUS[rank - 1];
            player.GetAttackSpeed(DamageClass.SummonMeleeSpeed) += SPEED_BONUS[rank - 1];
        }
    }
}
