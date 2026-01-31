using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using RpgMod.Common.Base;

namespace RpgMod.Common.Skills.Tier2.Assassin
{
    /// <summary>
    /// Poison Mastery - Assassin's passive for poison damage.
    /// Grants the Flask of Poison buff effect.
    /// </summary>
    public class PoisonMastery : BaseSkill
    {
        public override string InternalName => "PoisonMastery";
        public override string DisplayName => "Poison Mastery";
        public override string Description => "Passively imbue your weapons with poison, and increase damage.";

        public override SkillType SkillType => SkillType.Passive;
        public override JobType RequiredJob => JobType.Assassin;
        public override int RequiredLevel => RpgConstants.SECOND_JOB_LEVEL + 15;
        public override int SkillPointCost => 1;
        public override int MaxRank => 5;
        public override string IconTexture => $"{RpgConstants.ICON_PATH}Skills/PoisonMastery";
        public override float CooldownSeconds => 0f;
        public override int ResourceCost => 0;
        public override ResourceType ResourceType => ResourceType.None;

        private static readonly float[] DAMAGE_BONUS = { 0.03f, 0.05f, 0.07f, 0.09f, 0.12f };
        private static readonly int[] CRIT_BONUS = { 2, 3, 4, 5, 7 };

        public override void ApplyPassive(Player player)
        {
            if (CurrentRank <= 0) return;
            int rank = CurrentRank;
            
            // Grant poison weapon imbue
            player.AddBuff(BuffID.WeaponImbuePoison, 2);
            
            // Bonus damage and crit
            player.GetDamage(DamageClass.Melee) += DAMAGE_BONUS[rank - 1];
            player.GetCritChance(DamageClass.Melee) += CRIT_BONUS[rank - 1];
        }
    }
}
