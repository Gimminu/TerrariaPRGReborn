using Terraria;
using Terraria.ModLoader;
using RpgMod.Common.Base;

namespace RpgMod.Common.Skills.Tier2.BattleMage
{
    /// <summary>
    /// Battle Caster - Battle Mage's passive for combat magic.
    /// </summary>
    public class BattleCaster : BaseSkill
    {
        public override string InternalName => "BattleCaster";
        public override string DisplayName => "Battle Caster";
        public override string Description => "Increase defense while also boosting magic damage.";

        public override SkillType SkillType => SkillType.Passive;
        public override JobType RequiredJob => JobType.BattleMage;
        public override int RequiredLevel => RpgConstants.SECOND_JOB_LEVEL + 15;
        public override int SkillPointCost => 1;
        public override int MaxRank => 5;
        public override string IconTexture => $"{RpgConstants.ICON_PATH}Skills/BattleCaster";
        public override float CooldownSeconds => 0f;
        public override int ResourceCost => 0;
        public override ResourceType ResourceType => ResourceType.None;

        private static readonly int[] DEFENSE_BONUS = { 3, 5, 7, 9, 12 };
        private static readonly float[] MAGIC_BONUS = { 0.03f, 0.05f, 0.07f, 0.09f, 0.12f };

        public override void ApplyPassive(Player player)
        {
            if (CurrentRank <= 0) return;
            int rank = CurrentRank;
            player.statDefense += DEFENSE_BONUS[rank - 1];
            player.GetDamage(DamageClass.Magic) += MAGIC_BONUS[rank - 1];
        }
    }
}
