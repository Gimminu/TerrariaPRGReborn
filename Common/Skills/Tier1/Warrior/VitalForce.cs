using Terraria;
using RpgMod.Common.Base;

namespace RpgMod.Common.Skills.Tier1.Warrior
{
    /// <summary>
    /// Vital Force - 생명력.
    /// 최대 체력 증가.
    /// 전사의 생존 패시브.
    /// </summary>
    public class VitalForce : BaseSkill
    {
        public override string InternalName => "VitalForce";
        public override string DisplayName => "Vital Force";
        public override string Description => "Your body grows stronger, increasing maximum health.";

        public override SkillType SkillType => SkillType.Passive;
        public override JobType RequiredJob => JobType.Warrior;
        public override int RequiredLevel => 14;
        public override int SkillPointCost => 1;
        public override int MaxRank => 10;
        public override string IconTexture => $"{RpgConstants.ICON_PATH}Skills/VitalForce";
        public override float CooldownSeconds => 0f;
        public override int ResourceCost => 0;
        public override ResourceType ResourceType => ResourceType.None;

        private static readonly int[] HEALTH_BONUS = { 10, 20, 35, 50, 70, 90, 115, 140, 170, 200 };

        public override void ApplyPassive(Player player)
        {
            if (CurrentRank <= 0) return;
            int rank = CurrentRank;
            
            player.statLifeMax2 += HEALTH_BONUS[rank - 1];
        }
    }
}
