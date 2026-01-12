using Terraria;
using Rpg.Common.Base;

namespace Rpg.Common.Skills.Tier1.Summoner
{
    /// <summary>
    /// Sentry Mastery - 센트리 숙련.
    /// 센트리 피해 증가 및 센트리 슬롯 증가.
    /// 소환사의 센트리 패시브.
    /// </summary>
    public class SentryMastery : BaseSkill
    {
        public override string InternalName => "SentryMastery";
        public override string DisplayName => "Sentry Mastery";
        public override string Description => "Master sentry placement, increasing sentry damage and max sentry slots.";

        public override SkillType SkillType => SkillType.Passive;
        public override JobType RequiredJob => JobType.Summoner;
        public override int RequiredLevel => 28;
        public override int SkillPointCost => 1;
        public override int MaxRank => 10;
        public override string IconTexture => $"{RpgConstants.ICON_PATH}Skills/SentryMastery";
        public override float CooldownSeconds => 0f;
        public override int ResourceCost => 0;
        public override ResourceType ResourceType => ResourceType.None;

        private static readonly float[] DAMAGE_BONUS = { 0.02f, 0.04f, 0.06f, 0.08f, 0.10f, 0.12f, 0.14f, 0.16f, 0.18f, 0.22f };
        // 센트리 슬롯은 5, 10렙에 +1
        private static readonly int[] SENTRY_SLOTS = { 0, 0, 0, 0, 1, 1, 1, 1, 1, 2 };

        public override void ApplyPassive(Player player)
        {
            if (CurrentRank <= 0) return;
            int rank = CurrentRank;
            
            player.GetDamage(Terraria.ModLoader.DamageClass.Summon) += DAMAGE_BONUS[rank - 1];
            player.maxTurrets += SENTRY_SLOTS[rank - 1];
        }
    }
}
