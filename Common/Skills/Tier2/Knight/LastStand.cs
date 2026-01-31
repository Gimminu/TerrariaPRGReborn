using Terraria;
using RpgMod.Common.Base;

namespace RpgMod.Common.Skills.Tier2.Knight
{
    /// <summary>
    /// Last Stand - 최후의 저항.
    /// 죽을 피해를 받으면 1HP로 생존 (쿨다운 있음).
    /// 기사의 생존 패시브.
    /// </summary>
    public class LastStand : BaseSkill
    {
        public override string InternalName => "LastStand";
        public override string DisplayName => "Last Stand";
        public override string Description => "When you would take fatal damage, survive with 1 HP (cooldown applies).";

        public override SkillType SkillType => SkillType.Passive;
        public override JobType RequiredJob => JobType.Knight;
        public override int RequiredLevel => 80;
        public override int SkillPointCost => 1;
        public override int MaxRank => 10;
        public override string IconTexture => $"{RpgConstants.ICON_PATH}Skills/LastStand";
        public override float CooldownSeconds => 0f;
        public override int ResourceCost => 0;
        public override ResourceType ResourceType => ResourceType.None;

        // 쿨다운은 랭크에 따라 감소 (이 패시브는 RpgPlayer.PreKill에서 처리)
        public static int GetCooldownSeconds(int rank)
        {
            int[] cooldowns = { 180, 165, 150, 135, 120, 105, 90, 75, 60, 45 };
            return cooldowns[System.Math.Clamp(rank - 1, 0, 9)];
        }

        public override void ApplyPassive(Player player)
        {
            // 실제 효과는 RpgPlayer.PreKill에서 처리
            // 여기서는 아무것도 하지 않음
        }
    }
}
