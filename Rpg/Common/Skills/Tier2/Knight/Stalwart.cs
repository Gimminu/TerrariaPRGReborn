using Terraria;
using Rpg.Common.Base;

namespace Rpg.Common.Skills.Tier2.Knight
{
    /// <summary>
    /// Stalwart - 확고부동.
    /// 체력이 낮을수록 피해 감소 증가.
    /// </summary>
    public class Stalwart : BaseSkill
    {
        public override string InternalName => "Stalwart";
        public override string DisplayName => "Stalwart";
        public override string Description => "The lower your health, the more damage you resist.";

        public override SkillType SkillType => SkillType.Passive;
        public override JobType RequiredJob => JobType.Knight;
        public override int RequiredLevel => 85;
        public override int SkillPointCost => 1;
        public override int MaxRank => 10;
        public override string IconTexture => $"{RpgConstants.ICON_PATH}Skills/Stalwart";
        public override float CooldownSeconds => 0f;
        public override int ResourceCost => 0;
        public override ResourceType ResourceType => ResourceType.None;

        private static readonly float[] MAX_DR = { 0.05f, 0.08f, 0.11f, 0.14f, 0.17f, 0.20f, 0.23f, 0.26f, 0.29f, 0.35f };

        public override void ApplyPassive(Player player)
        {
            if (CurrentRank <= 0) return;
            int rank = CurrentRank;
            
            // 체력 비율이 낮을수록 피해 감소 증가
            float healthRatio = (float)player.statLife / player.statLifeMax2;
            float missingHealthRatio = 1f - healthRatio;
            float dr = MAX_DR[rank - 1] * missingHealthRatio;
            
            player.endurance += dr;
        }
    }
}
