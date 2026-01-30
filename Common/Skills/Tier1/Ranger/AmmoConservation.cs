using Terraria;
using Rpg.Common.Base;

namespace Rpg.Common.Skills.Tier1.Ranger
{
    /// <summary>
    /// Ammo Conservation - 탄약 절약.
    /// 탄약 소비 확률 감소.
    /// 레인저의 자원 효율 패시브.
    /// </summary>
    public class AmmoConservation : BaseSkill
    {
        public override string InternalName => "AmmoConservation";
        public override string DisplayName => "Ammo Conservation";
        public override string Description => "Learn to conserve ammunition, reducing ammo consumption chance.";

        public override SkillType SkillType => SkillType.Passive;
        public override JobType RequiredJob => JobType.Ranger;
        public override int RequiredLevel => 22;
        public override int SkillPointCost => 1;
        public override int MaxRank => 10;
        public override string IconTexture => $"{RpgConstants.ICON_PATH}Skills/AmmoConservation";
        public override float CooldownSeconds => 0f;
        public override int ResourceCost => 0;
        public override ResourceType ResourceType => ResourceType.None;

        private static readonly float[] AMMO_SAVE = { 0.02f, 0.04f, 0.06f, 0.08f, 0.10f, 0.12f, 0.15f, 0.18f, 0.21f, 0.25f };

        public override void ApplyPassive(Player player)
        {
            if (CurrentRank <= 0) return;
            int rank = CurrentRank;
            
            // 탄약 절약 확률 (테라리아 ammoCost 사용)
            player.ammoCost80 = player.ammoCost80 || Main.rand.NextFloat() < AMMO_SAVE[rank - 1];
        }
    }
}
