using Terraria;
using Terraria.ModLoader;
using Rpg.Common.Base;

namespace Rpg.Common.Skills.Tier2.Berserker
{
    /// <summary>
    /// Carnage - 대학살.
    /// 적을 죽일 때 공격력 버프 스택.
    /// 광전사의 킬 보상 패시브.
    /// </summary>
    public class Carnage : BaseSkill
    {
        public override string InternalName => "Carnage";
        public override string DisplayName => "Carnage";
        public override string Description => "Killing enemies grants stacking damage buff.";

        public override SkillType SkillType => SkillType.Passive;
        public override JobType RequiredJob => JobType.Berserker;
        public override int RequiredLevel => 75;
        public override int SkillPointCost => 1;
        public override int MaxRank => 10;
        public override string IconTexture => $"{RpgConstants.ICON_PATH}Skills/Carnage";
        public override float CooldownSeconds => 0f;
        public override int ResourceCost => 0;
        public override ResourceType ResourceType => ResourceType.None;

        // 스택당 피해 증가
        private static readonly float[] DAMAGE_PER_STACK = { 0.01f, 0.012f, 0.014f, 0.016f, 0.018f, 0.02f, 0.022f, 0.024f, 0.026f, 0.03f };
        // 최대 스택
        private static readonly int[] MAX_STACKS = { 5, 6, 7, 8, 9, 10, 11, 12, 14, 16 };

        public override void ApplyPassive(Player player)
        {
            // 실제 스택 시스템은 RpgPlayer.OnKill에서 처리
            // 여기서는 기본적인 피해 증가만
            if (CurrentRank <= 0) return;
            
            // 기본 피해 증가
            player.GetDamage(DamageClass.Melee) += 0.05f;
        }
    }
}
