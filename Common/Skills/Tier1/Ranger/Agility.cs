using Terraria;
using Rpg.Common.Base;

namespace Rpg.Common.Skills.Tier1.Ranger
{
    /// <summary>
    /// Agility - 민첩.
    /// 회피율 증가.
    /// 레인저의 생존 패시브.
    /// </summary>
    public class Agility : BaseSkill
    {
        public override string InternalName => "Agility";
        public override string DisplayName => "Agility";
        public override string Description => "Your nimble movements make you harder to hit, increasing dodge chance.";

        public override SkillType SkillType => SkillType.Passive;
        public override JobType RequiredJob => JobType.Ranger;
        public override int RequiredLevel => 30;
        public override int SkillPointCost => 1;
        public override int MaxRank => 10;
        public override string IconTexture => $"{RpgConstants.ICON_PATH}Skills/Agility";
        public override float CooldownSeconds => 0f;
        public override int ResourceCost => 0;
        public override ResourceType ResourceType => ResourceType.None;

        public override void ApplyPassive(Player player)
        {
            if (CurrentRank <= 0) return;
            
            // 회피율은 RpgPlayer에서 별도 처리 필요
            // 여기서는 블랙벨트 효과처럼 대시 가능 여부
            if (CurrentRank >= 5)
            {
                player.dash = player.dash == 0 ? 1 : player.dash;
            }
        }
    }
}
