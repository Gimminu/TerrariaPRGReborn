using Terraria;
using Terraria.ModLoader;
using RpgMod.Common.Base;

namespace RpgMod.Common.Skills.Tier1.Summoner
{
    /// <summary>
    /// Whip Mastery - 채찍 숙련.
    /// 채찍 피해와 속도 증가.
    /// 소환사의 채찍 패시브.
    /// </summary>
    public class WhipMastery : BaseSkill
    {
        public override string InternalName => "WhipMastery";
        public override string DisplayName => "Whip Mastery";
        public override string Description => "Master the whip, increasing whip damage and attack speed.";

        public override SkillType SkillType => SkillType.Passive;
        public override JobType RequiredJob => JobType.Summoner;
        public override int RequiredLevel => 16;
        public override int SkillPointCost => 1;
        public override int MaxRank => 10;
        public override string IconTexture => $"{RpgConstants.ICON_PATH}Skills/WhipMastery";
        public override float CooldownSeconds => 0f;
        public override int ResourceCost => 0;
        public override ResourceType ResourceType => ResourceType.None;

        private static readonly float[] DAMAGE_BONUS = { 0.02f, 0.04f, 0.06f, 0.08f, 0.10f, 0.12f, 0.14f, 0.16f, 0.18f, 0.20f };
        private static readonly float[] SPEED_BONUS = { 0.01f, 0.02f, 0.03f, 0.04f, 0.05f, 0.06f, 0.07f, 0.08f, 0.09f, 0.10f };

        public override void ApplyPassive(Player player)
        {
            if (CurrentRank <= 0) return;
            int rank = CurrentRank;
            
            player.GetDamage(DamageClass.SummonMeleeSpeed) += DAMAGE_BONUS[rank - 1];
            player.GetAttackSpeed(DamageClass.SummonMeleeSpeed) += SPEED_BONUS[rank - 1];
        }
    }
}
