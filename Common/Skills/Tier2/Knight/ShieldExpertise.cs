using Terraria;
using RpgMod.Common.Base;

namespace RpgMod.Common.Skills.Tier2.Knight
{
    /// <summary>
    /// Shield Expertise - 방패 전문가.
    /// 넉백 저항 + 방패 장착 시 보너스.
    /// </summary>
    public class ShieldExpertise : BaseSkill
    {
        public override string InternalName => "ShieldExpertise";
        public override string DisplayName => "Shield Expertise";
        public override string Description => "Become an expert with shields, increasing knockback resistance and defense.";

        public override SkillType SkillType => SkillType.Passive;
        public override JobType RequiredJob => JobType.Knight;
        public override int RequiredLevel => 80;
        public override int SkillPointCost => 1;
        public override int MaxRank => 10;
        public override string IconTexture => $"{RpgConstants.ICON_PATH}Skills/ShieldExpertise";
        public override float CooldownSeconds => 0f;
        public override int ResourceCost => 0;
        public override ResourceType ResourceType => ResourceType.None;

        private static readonly float[] KB_RESIST = { 0.05f, 0.10f, 0.15f, 0.20f, 0.25f, 0.30f, 0.35f, 0.40f, 0.45f, 0.50f };

        public static float GetKnockbackResist(int rank)
        {
            if (rank <= 0)
                return 0f;
            int index = System.Math.Min(rank, KB_RESIST.Length) - 1;
            return KB_RESIST[index];
        }

        public override void ApplyPassive(Player player)
        {
            if (CurrentRank <= 0) return;
            int rank = CurrentRank;

            player.noKnockback = rank >= 10;
            player.statDefense += rank * 2;
        }
    }
}
