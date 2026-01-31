using Terraria;
using Terraria.ModLoader;
using RpgMod.Common.Base;

namespace RpgMod.Common.Skills.Tier2.Gunslinger
{
    /// <summary>
    /// Quick Draw - Gunslinger's passive for faster attack speed.
    /// </summary>
    public class QuickDraw : BaseSkill
    {
        public override string InternalName => "QuickDraw";
        public override string DisplayName => "Quick Draw";
        public override string Description => "Increase ranged attack speed and move speed.";

        public override SkillType SkillType => SkillType.Passive;
        public override JobType RequiredJob => JobType.Gunslinger;
        public override int RequiredLevel => RpgConstants.SECOND_JOB_LEVEL + 15;
        public override int SkillPointCost => 1;
        public override int MaxRank => 5;
        public override string IconTexture => $"{RpgConstants.ICON_PATH}Skills/QuickDraw";
        public override float CooldownSeconds => 0f;
        public override int ResourceCost => 0;
        public override ResourceType ResourceType => ResourceType.None;

        private static readonly float[] ATTACK_SPEED = { 0.05f, 0.08f, 0.11f, 0.14f, 0.18f };
        private static readonly float[] MOVE_SPEED = { 0.03f, 0.05f, 0.07f, 0.09f, 0.12f };

        public override void ApplyPassive(Player player)
        {
            if (CurrentRank <= 0) return;
            int rank = CurrentRank;
            player.GetAttackSpeed(DamageClass.Ranged) += ATTACK_SPEED[rank - 1];
            player.moveSpeed += MOVE_SPEED[rank - 1];
        }
    }
}
