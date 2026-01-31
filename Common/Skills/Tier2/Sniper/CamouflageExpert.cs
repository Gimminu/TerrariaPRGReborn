using Terraria;
using RpgMod.Common.Base;

namespace RpgMod.Common.Skills.Tier2.Sniper
{
    /// <summary>
    /// Camouflage Expert - 위장 전문가.
    /// 공격하지 않을 때 회피율 증가.
    /// 저격수의 생존 패시브.
    /// </summary>
    public class CamouflageExpert : BaseSkill
    {
        public override string InternalName => "CamouflageExpert";
        public override string DisplayName => "Camouflage Expert";
        public override string Description => "Blend into the shadows when idle, reducing aggro and gaining bonus defense while stationary.";

        public override SkillType SkillType => SkillType.Passive;
        public override JobType RequiredJob => JobType.Sniper;
        public override int RequiredLevel => 75;
        public override int SkillPointCost => 1;
        public override int MaxRank => 10;
        public override string IconTexture => $"{RpgConstants.ICON_PATH}Skills/CamouflageExpert";
        public override float CooldownSeconds => 0f;
        public override int ResourceCost => 0;
        public override ResourceType ResourceType => ResourceType.None;

        public override void ApplyPassive(Player player)
        {
            if (CurrentRank <= 0) return;
            int rank = CurrentRank;
            
            // 어그로 감소 (RpgPlayer에서 추가 처리)
            // 여기서는 방어력 보너스
            if (player.velocity.Length() < 1f) // 움직이지 않을 때
            {
                player.statDefense += rank * 2;
            }
        }
    }
}
