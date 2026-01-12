using Terraria;
using Rpg.Common.Base;

namespace Rpg.Common.Skills.Tier3.Gunmaster
{
    /// <summary>
    /// Infinite Ammo - 무한 탄약.
    /// 탄약 소모 대폭 감소.
    /// 건마스터의 자원 패시브.
    /// </summary>
    public class InfiniteAmmo : BaseSkill
    {
        public override string InternalName => "InfiniteAmmo";
        public override string DisplayName => "Infinite Ammo";
        public override string Description => "Greatly reduce ammo consumption.";

        public override SkillType SkillType => SkillType.Passive;
        public override JobType RequiredJob => JobType.Gunmaster;
        public override int RequiredLevel => 135;
        public override int SkillPointCost => 1;
        public override int MaxRank => 10;
        public override string IconTexture => $"{RpgConstants.ICON_PATH}Skills/InfiniteAmmo";
        public override float CooldownSeconds => 0f;
        public override int ResourceCost => 0;
        public override ResourceType ResourceType => ResourceType.None;

        // 탄약 절약 확률 (RpgPlayer에서 처리)
        private static readonly float[] AMMO_SAVE_CHANCE = { 0.10f, 0.18f, 0.25f, 0.32f, 0.38f, 0.44f, 0.50f, 0.58f, 0.68f, 0.85f };

        public static float GetAmmoSaveChance(int rank)
        {
            return AMMO_SAVE_CHANCE[System.Math.Clamp(rank - 1, 0, 9)];
        }

        public override void ApplyPassive(Player player)
        {
            if (CurrentRank <= 0) return;
            // 구현은 GlobalItem에서 처리
        }
    }
}
