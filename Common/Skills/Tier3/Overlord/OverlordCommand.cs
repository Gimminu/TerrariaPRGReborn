using Terraria;
using Terraria.ModLoader;
using RpgMod.Common.Base;

namespace RpgMod.Common.Skills.Tier3.Overlord
{
    /// <summary>
    /// Overlord Command - Overlord's passive for ultimate summoner power.
    /// </summary>
    public class OverlordCommand : BaseSkill
    {
        public override string InternalName => "OverlordCommand";
        public override string DisplayName => "Overlord Command";
        public override string Description => "Your command over beasts is absolute, granting extra minion slots, summon damage, and defense.";

        public override SkillType SkillType => SkillType.Passive;
        public override JobType RequiredJob => JobType.Overlord;
        public override int RequiredLevel => RpgConstants.THIRD_JOB_LEVEL + 15;
        public override int SkillPointCost => 2;
        public override int MaxRank => 5;
        public override string IconTexture => $"{RpgConstants.ICON_PATH}Skills/OverlordCommand";
        public override float CooldownSeconds => 0f;
        public override int ResourceCost => 0;
        public override ResourceType ResourceType => ResourceType.None;

        private static readonly float[] SUMMON_DAMAGE = { 0.10f, 0.15f, 0.20f, 0.25f, 0.35f };
        private static readonly int[] DEFENSE = { 3, 5, 7, 9, 12 };

        public override void ApplyPassive(Player player)
        {
            if (CurrentRank <= 0) return;
            int rank = CurrentRank;
            player.GetDamage(DamageClass.Summon) += SUMMON_DAMAGE[rank - 1];
            player.statDefense += DEFENSE[rank - 1];
            player.maxMinions += 1;
            if (rank >= 3) player.maxMinions += 1;
            if (rank >= 5) player.maxMinions += 1;
        }
    }
}
