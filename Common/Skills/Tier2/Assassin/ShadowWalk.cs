using Terraria;
using Rpg.Common.Base;

namespace Rpg.Common.Skills.Tier2.Assassin
{
    /// <summary>
    /// Shadow Walk - 그림자 걸음.
    /// 이동속도 증가 + 회피.
    /// </summary>
    public class ShadowWalk : BaseSkill
    {
        public override string InternalName => "ShadowWalk";
        public override string DisplayName => "Shadow Walk";
        public override string Description => "Move like a shadow, increasing movement speed and evasion.";

        public override SkillType SkillType => SkillType.Passive;
        public override JobType RequiredJob => JobType.Assassin;
        public override int RequiredLevel => 68;
        public override int SkillPointCost => 1;
        public override int MaxRank => 10;
        public override string IconTexture => $"{RpgConstants.ICON_PATH}Skills/ShadowWalk";
        public override float CooldownSeconds => 0f;
        public override int ResourceCost => 0;
        public override ResourceType ResourceType => ResourceType.None;

        private static readonly float[] SPEED_BONUS = { 0.03f, 0.06f, 0.09f, 0.12f, 0.15f, 0.18f, 0.21f, 0.24f, 0.27f, 0.32f };

        public override void ApplyPassive(Player player)
        {
            if (CurrentRank <= 0) return;
            int rank = CurrentRank;
            
            player.moveSpeed += SPEED_BONUS[rank - 1];
            player.maxRunSpeed += SPEED_BONUS[rank - 1] * 3f;
            
            if (rank >= 10)
            {
                player.dash = player.dash == 0 ? 1 : player.dash;
            }
        }
    }
}
