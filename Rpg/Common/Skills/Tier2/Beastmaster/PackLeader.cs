using Terraria;
using Rpg.Common.Base;

namespace Rpg.Common.Skills.Tier2.Beastmaster
{
    /// <summary>
    /// Pack Leader - 무리의 리더.
    /// 소환수 슬롯 증가.
    /// 야수조련사의 슬롯 패시브.
    /// </summary>
    public class PackLeader : BaseSkill
    {
        public override string InternalName => "PackLeader";
        public override string DisplayName => "Pack Leader";
        public override string Description => "Increase maximum minion slots.";

        public override SkillType SkillType => SkillType.Passive;
        public override JobType RequiredJob => JobType.Beastmaster;
        public override int RequiredLevel => 70;
        public override int SkillPointCost => 1;
        public override int MaxRank => 10;
        public override string IconTexture => $"{RpgConstants.ICON_PATH}Skills/PackLeader";
        public override float CooldownSeconds => 0f;
        public override int ResourceCost => 0;
        public override ResourceType ResourceType => ResourceType.None;

        private static readonly int[] MINION_SLOTS = { 0, 0, 1, 1, 1, 1, 2, 2, 2, 3 };

        public override void ApplyPassive(Player player)
        {
            if (CurrentRank <= 0) return;
            int rank = CurrentRank;
            
            player.maxMinions += MINION_SLOTS[rank - 1];
        }
    }
}
