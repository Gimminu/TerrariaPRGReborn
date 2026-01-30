using Terraria;
using Rpg.Common.Base;

namespace Rpg.Common.Skills.Tier2.Assassin
{
    /// <summary>
    /// Evasion - 회피.
    /// 공격을 회피할 확률.
    /// 암살자의 생존 패시브.
    /// </summary>
    public class Evasion : BaseSkill
    {
        public override string InternalName => "Evasion";
        public override string DisplayName => "Evasion";
        public override string Description => "Chance to completely dodge attacks.";

        public override SkillType SkillType => SkillType.Passive;
        public override JobType RequiredJob => JobType.Assassin;
        public override int RequiredLevel => 72;
        public override int SkillPointCost => 1;
        public override int MaxRank => 10;
        public override string IconTexture => $"{RpgConstants.ICON_PATH}Skills/Evasion";
        public override float CooldownSeconds => 0f;
        public override int ResourceCost => 0;
        public override ResourceType ResourceType => ResourceType.None;

        // 회피 확률 (RpgPlayer.ModifyHurt에서 처리)
        private static readonly float[] DODGE_CHANCE = { 0.02f, 0.04f, 0.06f, 0.08f, 0.10f, 0.12f, 0.14f, 0.16f, 0.18f, 0.22f };

        public static float GetDodgeChance(int rank)
        {
            return DODGE_CHANCE[System.Math.Clamp(rank - 1, 0, 9)];
        }

        public override void ApplyPassive(Player player)
        {
            if (CurrentRank <= 0) return;
            
            // 회피 효과는 RpgPlayer에서 처리
            // 여기서는 이동속도 보너스
            player.moveSpeed += 0.05f * CurrentRank;
        }
    }
}
