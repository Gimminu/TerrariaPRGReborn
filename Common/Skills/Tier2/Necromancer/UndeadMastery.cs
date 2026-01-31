using Terraria;
using Terraria.ModLoader;
using RpgMod.Common.Base;

namespace RpgMod.Common.Skills.Tier2.Necromancer
{
    /// <summary>
    /// Undead Mastery - Necromancer's passive for summon power.
    /// </summary>
    public class UndeadMastery : BaseSkill
    {
        public override string InternalName => "UndeadMastery";
        public override string DisplayName => "Undead Mastery";
        public override string Description => "Command the dead with greater power and numbers.";

        public override SkillType SkillType => SkillType.Passive;
        public override JobType RequiredJob => JobType.Necromancer;
        public override int RequiredLevel => RpgConstants.SECOND_JOB_LEVEL + 15;
        public override int SkillPointCost => 1;
        public override int MaxRank => 5;
        public override string IconTexture => $"{RpgConstants.ICON_PATH}Skills/UndeadMastery";
        public override float CooldownSeconds => 0f;
        public override int ResourceCost => 0;
        public override ResourceType ResourceType => ResourceType.None;

        private static readonly float[] SUMMON_DAMAGE = { 0.05f, 0.08f, 0.11f, 0.14f, 0.20f };

        public override void ApplyPassive(Player player)
        {
            if (CurrentRank <= 0) return;
            int rank = CurrentRank;
            player.GetDamage(DamageClass.Summon) += SUMMON_DAMAGE[rank - 1];
            if (rank >= 3) player.maxMinions += 1;
            if (rank >= 5) player.maxMinions += 1;
        }
    }
}
