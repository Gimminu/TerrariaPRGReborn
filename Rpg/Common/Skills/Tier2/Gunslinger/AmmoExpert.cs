using Terraria;
using Terraria.ModLoader;
using Rpg.Common.Base;

namespace Rpg.Common.Skills.Tier2.Gunslinger
{
    /// <summary>
    /// Ammo Expert - 탄약 전문가.
    /// 탄약 소모 감소 확률.
    /// 건슬링거의 자원 절약 패시브.
    /// </summary>
    public class AmmoExpert : BaseSkill
    {
        public override string InternalName => "AmmoExpert";
        public override string DisplayName => "Ammo Expert";
        public override string Description => "Chance to not consume ammo when firing.";

        public override SkillType SkillType => SkillType.Passive;
        public override JobType RequiredJob => JobType.Gunslinger;
        public override int RequiredLevel => 60;
        public override int SkillPointCost => 1;
        public override int MaxRank => 10;
        public override string IconTexture => $"{RpgConstants.ICON_PATH}Skills/AmmoExpert";
        public override float CooldownSeconds => 0f;
        public override int ResourceCost => 0;
        public override ResourceType ResourceType => ResourceType.None;

        private static readonly float[] AMMO_SAVE_CHANCE = { 0.03f, 0.06f, 0.09f, 0.12f, 0.15f, 0.18f, 0.21f, 0.24f, 0.28f, 0.35f };

        public override void ApplyPassive(Player player)
        {
            if (CurrentRank <= 0) return;
            int rank = CurrentRank;
            
            // 탄약 절약 확률은 AmmoItem.CanBeConsumedAsAmmo에서 처리
            // 여기서는 원거리 피해 증가
            player.GetDamage(DamageClass.Ranged) += 0.015f * rank;
        }

        public static float GetAmmoSaveChance(int rank)
        {
            return AMMO_SAVE_CHANCE[System.Math.Clamp(rank - 1, 0, 9)];
        }
    }
}
