using Terraria;
using RpgMod.Common.Base;

namespace RpgMod.Common.Skills.Tier2.Knight
{
    /// <summary>
    /// Vigilance - 경계.
    /// 체력이 낮을 때 방어력 증가.
    /// 기사의 위기 대응 패시브.
    /// </summary>
    public class Vigilance : BaseSkill
    {
        public override string InternalName => "Vigilance";
        public override string DisplayName => "Vigilance";
        public override string Description => "When health is low, your defense increases dramatically.";

        public override SkillType SkillType => SkillType.Passive;
        public override JobType RequiredJob => JobType.Knight;
        public override int RequiredLevel => 72;
        public override int SkillPointCost => 1;
        public override int MaxRank => 10;
        public override string IconTexture => $"{RpgConstants.ICON_PATH}Skills/Vigilance";
        public override float CooldownSeconds => 0f;
        public override int ResourceCost => 0;
        public override ResourceType ResourceType => ResourceType.None;

        private static readonly int[] DEFENSE_BONUS = { 10, 15, 20, 25, 30, 35, 40, 50, 60, 75 };

        public override void ApplyPassive(Player player)
        {
            if (CurrentRank <= 0) return;
            int rank = CurrentRank;
            
            // 체력이 30% 이하일 때
            float hpPercent = (float)player.statLife / player.statLifeMax2;
            if (hpPercent <= 0.3f)
            {
                player.statDefense += DEFENSE_BONUS[rank - 1];
            }
        }
    }
}
