using Terraria;
using Terraria.ModLoader;
using RpgMod.Common.Base;

namespace RpgMod.Common.Skills.Tier2.Gunslinger
{
    /// <summary>
    /// Gun Mastery - 총기 숙련.
    /// 총기 피해 증가.
    /// 건슬링거의 피해 패시브.
    /// </summary>
    public class GunMastery : BaseSkill
    {
        public override string InternalName => "GunMastery";
        public override string DisplayName => "Gun Mastery";
        public override string Description => "Master of firearms, increasing ranged damage.";

        public override SkillType SkillType => SkillType.Passive;
        public override JobType RequiredJob => JobType.Gunslinger;
        public override int RequiredLevel => 70;
        public override int SkillPointCost => 1;
        public override int MaxRank => 10;
        public override string IconTexture => $"{RpgConstants.ICON_PATH}Skills/GunMastery";
        public override float CooldownSeconds => 0f;
        public override int ResourceCost => 0;
        public override ResourceType ResourceType => ResourceType.None;

        private static readonly float[] DAMAGE_BONUS = { 0.03f, 0.06f, 0.09f, 0.12f, 0.15f, 0.18f, 0.21f, 0.24f, 0.28f, 0.35f };

        public override void ApplyPassive(Player player)
        {
            if (CurrentRank <= 0) return;
            int rank = CurrentRank;
            
            player.GetDamage(DamageClass.Ranged) += DAMAGE_BONUS[rank - 1];
        }
    }
}
