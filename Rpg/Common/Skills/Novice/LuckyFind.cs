using Terraria;
using Terraria.ID;
using Rpg.Common.Base;

namespace Rpg.Common.Skills.Novice
{
    /// <summary>
    /// Lucky Find - 행운의 발견.
    /// 운을 높여 더 좋은 아이템을 얻을 확률 증가.
    /// 초보자 시절의 성장을 돕는 패시브.
    /// </summary>
    public class LuckyFind : BaseSkill
    {
        public override string InternalName => "LuckyFind";
        public override string DisplayName => "Lucky Find";
        public override string Description => "Fortune favors the bold. Increases luck for better drops.";

        public override SkillType SkillType => SkillType.Passive;
        public override JobType RequiredJob => JobType.Novice;
        public override int RequiredLevel => 6;
        public override int SkillPointCost => 1;
        public override int MaxRank => 5;
        public override string IconTexture => $"{RpgConstants.ICON_PATH}Skills/LuckyFind";
        public override float CooldownSeconds => 0f;
        public override int ResourceCost => 0;
        public override ResourceType ResourceType => ResourceType.None;

        // 행운 증가
        private static readonly float[] LUCK_BONUS = { 0.02f, 0.04f, 0.06f, 0.08f, 0.10f };

        public override void ApplyPassive(Player player)
        {
            if (CurrentRank <= 0) return;
            int rank = CurrentRank;
            
            // 테라리아의 luck 시스템 활용
            player.luck += LUCK_BONUS[rank - 1];
        }
    }
}
