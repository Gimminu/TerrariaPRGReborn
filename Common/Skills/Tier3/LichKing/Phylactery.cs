using Terraria;
using Terraria.ModLoader;
using Rpg.Common.Base;

namespace Rpg.Common.Skills.Tier3.LichKing
{
    /// <summary>
    /// Phylactery - Lich King's passive for survivability.
    /// </summary>
    public class Phylactery : BaseSkill
    {
        public override string InternalName => "Phylactery";
        public override string DisplayName => "Phylactery";
        public override string Description => "Your dark pact grants you increased damage and summon slots.";

        public override SkillType SkillType => SkillType.Passive;
        public override JobType RequiredJob => JobType.LichKing;
        public override int RequiredLevel => RpgConstants.THIRD_JOB_LEVEL + 15;
        public override int SkillPointCost => 2;
        public override int MaxRank => 5;
        public override string IconTexture => $"{RpgConstants.ICON_PATH}Skills/Phylactery";
        public override float CooldownSeconds => 0f;
        public override int ResourceCost => 0;
        public override ResourceType ResourceType => ResourceType.None;

        private static readonly float[] SUMMON_DAMAGE = { 0.08f, 0.12f, 0.16f, 0.20f, 0.28f };

        public override void ApplyPassive(Player player)
        {
            if (CurrentRank <= 0) return;
            int rank = CurrentRank;
            player.GetDamage(DamageClass.Summon) += SUMMON_DAMAGE[rank - 1];
            player.maxMinions += 1;
            if (rank >= 3) player.maxMinions += 1;
            if (rank >= 5) player.maxMinions += 1;
        }
    }
}
