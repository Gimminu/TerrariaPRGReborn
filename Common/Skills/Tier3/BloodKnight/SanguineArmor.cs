using Terraria;
using Terraria.ModLoader;
using RpgMod.Common.Base;

namespace RpgMod.Common.Skills.Tier3.BloodKnight
{
    /// <summary>
    /// Sanguine Armor - Blood Knight's passive for life-based defense.
    /// </summary>
    public class SanguineArmor : BaseSkill
    {
        public override string InternalName => "SanguineArmor";
        public override string DisplayName => "Sanguine Armor";
        public override string Description => "Gain defense based on missing health.";

        public override SkillType SkillType => SkillType.Passive;
        public override JobType RequiredJob => JobType.BloodKnight;
        public override int RequiredLevel => RpgConstants.THIRD_JOB_LEVEL + 15;
        public override int SkillPointCost => 2;
        public override int MaxRank => 5;
        public override string IconTexture => $"{RpgConstants.ICON_PATH}Skills/SanguineArmor";
        public override float CooldownSeconds => 0f;
        public override int ResourceCost => 0;
        public override ResourceType ResourceType => ResourceType.None;

        private static readonly int[] BASE_DEFENSE = { 5, 8, 12, 16, 22 };
        private static readonly float[] MISSING_HEALTH_SCALE = { 0.15f, 0.20f, 0.25f, 0.30f, 0.40f };

        public override void ApplyPassive(Player player)
        {
            if (CurrentRank <= 0) return;
            int rank = CurrentRank;

            int missingHealth = player.statLifeMax2 - player.statLife;
            int bonusDefense = BASE_DEFENSE[rank - 1] + (int)(missingHealth * MISSING_HEALTH_SCALE[rank - 1] / 10f);
            player.statDefense += bonusDefense;
        }
    }
}
