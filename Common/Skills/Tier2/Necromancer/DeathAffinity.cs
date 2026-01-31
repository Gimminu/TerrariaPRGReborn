using Terraria;
using RpgMod.Common.Base;

namespace RpgMod.Common.Skills.Tier2.Necromancer
{
    /// <summary>
    /// Death Affinity - 죽음의 친화.
    /// 적 사망 시 생명력 회복.
    /// </summary>
    public class DeathAffinity : BaseSkill
    {
        public override string InternalName => "DeathAffinity";
        public override string DisplayName => "Death Affinity";
        public override string Description => "Draw power from death, recovering health when enemies die nearby.";

        public override SkillType SkillType => SkillType.Passive;
        public override JobType RequiredJob => JobType.Necromancer;
        public override int RequiredLevel => 75;
        public override int SkillPointCost => 1;
        public override int MaxRank => 10;
        public override string IconTexture => $"{RpgConstants.ICON_PATH}Skills/DeathAffinity";
        public override float CooldownSeconds => 0f;
        public override int ResourceCost => 0;
        public override ResourceType ResourceType => ResourceType.None;

        // 이 패시브는 GlobalNPC에서 처리해야 함
        public static readonly int[] HEAL_AMOUNT = { 2, 4, 6, 8, 10, 12, 15, 18, 22, 30 };

        public override void ApplyPassive(Player player)
        {
            // 패시브 효과는 GlobalNPC.OnKill에서 처리
        }
    }
}
