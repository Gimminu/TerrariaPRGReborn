using Terraria;
using Rpg.Common.Base;

namespace Rpg.Common.Skills.Tier1.Ranger
{
    /// <summary>
    /// Fleet Foot - 빠른 발.
    /// 이동속도 증가.
    /// 레인저의 기동성 패시브.
    /// </summary>
    public class FleetFoot : BaseSkill
    {
        public override string InternalName => "FleetFoot";
        public override string DisplayName => "Fleet Foot";
        public override string Description => "Move swiftly across any terrain, increasing movement speed.";

        public override SkillType SkillType => SkillType.Passive;
        public override JobType RequiredJob => JobType.Ranger;
        public override int RequiredLevel => 15;
        public override int SkillPointCost => 1;
        public override int MaxRank => 10;
        public override string IconTexture => $"{RpgConstants.ICON_PATH}Skills/FleetFoot";
        public override float CooldownSeconds => 0f;
        public override int ResourceCost => 0;
        public override ResourceType ResourceType => ResourceType.None;

        private static readonly float[] SPEED_BONUS = { 0.03f, 0.05f, 0.07f, 0.09f, 0.11f, 0.13f, 0.15f, 0.17f, 0.19f, 0.22f };

        public override void ApplyPassive(Player player)
        {
            if (CurrentRank <= 0) return;
            int rank = CurrentRank;
            
            player.moveSpeed += SPEED_BONUS[rank - 1];
            player.maxRunSpeed += SPEED_BONUS[rank - 1] * 3f;
        }
    }
}
