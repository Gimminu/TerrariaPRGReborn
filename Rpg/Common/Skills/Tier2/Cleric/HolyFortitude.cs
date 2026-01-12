using Terraria;
using Rpg.Common.Base;

namespace Rpg.Common.Skills.Tier2.Cleric
{
    /// <summary>
    /// Holy Fortitude - 신성한 강인함.
    /// 최대 체력 증가.
    /// 클레릭의 체력 패시브.
    /// </summary>
    public class HolyFortitude : BaseSkill
    {
        public override string InternalName => "HolyFortitude";
        public override string DisplayName => "Holy Fortitude";
        public override string Description => "Increase maximum health.";

        public override SkillType SkillType => SkillType.Passive;
        public override JobType RequiredJob => JobType.Cleric;
        public override int RequiredLevel => 68;
        public override int SkillPointCost => 1;
        public override int MaxRank => 10;
        public override string IconTexture => $"{RpgConstants.ICON_PATH}Skills/HolyFortitude";
        public override float CooldownSeconds => 0f;
        public override int ResourceCost => 0;
        public override ResourceType ResourceType => ResourceType.None;

        private static readonly int[] HP_BONUS = { 15, 30, 50, 70, 95, 125, 160, 200, 250, 320 };

        public override void ApplyPassive(Player player)
        {
            if (CurrentRank <= 0) return;
            int rank = CurrentRank;
            
            player.statLifeMax2 += HP_BONUS[rank - 1];
        }
    }
}
