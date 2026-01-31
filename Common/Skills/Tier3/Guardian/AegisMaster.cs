using Terraria;
using RpgMod.Common.Base;

namespace RpgMod.Common.Skills.Tier3.Guardian
{
    /// <summary>
    /// Aegis Master - 방패의 달인.
    /// 피해 감소와 넉백 면역.
    /// 가디언의 방어 패시브.
    /// </summary>
    public class AegisMaster : BaseSkill
    {
        public override string InternalName => "AegisMaster";
        public override string DisplayName => "Aegis Master";
        public override string Description => "Reduce damage taken and gain knockback immunity at higher ranks.";

        public override SkillType SkillType => SkillType.Passive;
        public override JobType RequiredJob => JobType.Guardian;
        public override int RequiredLevel => 130;
        public override int SkillPointCost => 1;
        public override int MaxRank => 10;
        public override string IconTexture => $"{RpgConstants.ICON_PATH}Skills/AegisMaster";
        public override float CooldownSeconds => 0f;
        public override int ResourceCost => 0;
        public override ResourceType ResourceType => ResourceType.None;

        private static readonly float[] DAMAGE_REDUCTION = { 0.02f, 0.04f, 0.06f, 0.08f, 0.10f, 0.12f, 0.14f, 0.16f, 0.19f, 0.25f };

        public override void ApplyPassive(Player player)
        {
            if (CurrentRank <= 0) return;
            int rank = CurrentRank;
            
            player.endurance += DAMAGE_REDUCTION[rank - 1];
            if (rank >= 5) player.noKnockback = true;
        }
    }
}
