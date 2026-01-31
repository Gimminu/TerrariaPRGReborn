using Terraria;
using Terraria.ModLoader;
using RpgMod.Common.Base;

namespace RpgMod.Common.Skills.Tier2.Sniper
{
    /// <summary>
    /// Eagle Eye - 매의 눈.
    /// 사거리와 정확도 증가.
    /// </summary>
    public class EagleEye : BaseSkill
    {
        public override string InternalName => "EagleEye";
        public override string DisplayName => "Eagle Eye";
        public override string Description => "Your vision sharpens like an eagle, increasing ranged crit and granting scope vision.";

        public override SkillType SkillType => SkillType.Passive;
        public override JobType RequiredJob => JobType.Sniper;
        public override int RequiredLevel => 75;
        public override int SkillPointCost => 1;
        public override int MaxRank => 10;
        public override string IconTexture => $"{RpgConstants.ICON_PATH}Skills/EagleEye";
        public override float CooldownSeconds => 0f;
        public override int ResourceCost => 0;
        public override ResourceType ResourceType => ResourceType.None;

        private static readonly int[] CRIT_BONUS = { 1, 2, 3, 4, 5, 6, 7, 8, 10, 12 };

        public override void ApplyPassive(Player player)
        {
            if (CurrentRank <= 0) return;
            int rank = CurrentRank;
            
            player.GetCritChance(DamageClass.Ranged) += CRIT_BONUS[rank - 1];
            player.scope = true; // 스코프 효과
        }
    }
}
