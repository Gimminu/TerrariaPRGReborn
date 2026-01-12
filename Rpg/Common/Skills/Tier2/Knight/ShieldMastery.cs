using Terraria;
using Rpg.Common.Base;

namespace Rpg.Common.Skills.Tier2.Knight
{
    /// <summary>
    /// Shield Mastery - 방패 숙련.
    /// 피해 감소와 넉백 저항.
    /// 기사의 방어 패시브.
    /// </summary>
    public class ShieldMastery : BaseSkill
    {
        public override string InternalName => "ShieldMastery";
        public override string DisplayName => "Shield Mastery";
        public override string Description => "Master the shield, reducing damage taken and resisting knockback.";

        public override SkillType SkillType => SkillType.Passive;
        public override JobType RequiredJob => JobType.Knight;
        public override int RequiredLevel => 65;
        public override int SkillPointCost => 1;
        public override int MaxRank => 10;
        public override string IconTexture => $"{RpgConstants.ICON_PATH}Skills/ShieldMastery";
        public override float CooldownSeconds => 0f;
        public override int ResourceCost => 0;
        public override ResourceType ResourceType => ResourceType.None;

        private static readonly float[] DR_BONUS = { 0.02f, 0.03f, 0.04f, 0.05f, 0.06f, 0.07f, 0.08f, 0.09f, 0.10f, 0.12f };

        public override void ApplyPassive(Player player)
        {
            if (CurrentRank <= 0) return;
            int rank = CurrentRank;
            
            player.endurance += DR_BONUS[rank - 1];
            player.noKnockback = rank >= 5; // 5랭크 이상이면 넉백 면역
        }
    }
}
