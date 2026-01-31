using Terraria;
using RpgMod.Common.Base;

namespace RpgMod.Common.Skills.Tier2.DeathKnight
{
    /// <summary>
    /// Unholy Strength - 부정한 힘.
    /// 근접 피해 증가.
    /// </summary>
    public class UnholyStrength : BaseSkill
    {
        public override string InternalName => "UnholyStrength";
        public override string DisplayName => "Unholy Strength";
        public override string Description => "Dark power flows through you, increasing melee damage.";

        public override SkillType SkillType => SkillType.Passive;
        public override JobType RequiredJob => JobType.DeathKnight;
        public override int RequiredLevel => 60;
        public override int SkillPointCost => 1;
        public override int MaxRank => 10;
        public override string IconTexture => $"{RpgConstants.ICON_PATH}Skills/UnholyStrength";
        public override float CooldownSeconds => 0f;
        public override int ResourceCost => 0;
        public override ResourceType ResourceType => ResourceType.None;

        private static readonly float[] DAMAGE_BONUS = { 0.03f, 0.06f, 0.09f, 0.12f, 0.15f, 0.18f, 0.21f, 0.24f, 0.28f, 0.35f };

        public override void ApplyPassive(Player player)
        {
            if (CurrentRank <= 0) return;
            int rank = CurrentRank;
            
            player.GetDamage(Terraria.ModLoader.DamageClass.Melee) += DAMAGE_BONUS[rank - 1];
        }
    }
}
