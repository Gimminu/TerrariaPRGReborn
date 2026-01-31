using Terraria;
using Terraria.ModLoader;
using RpgMod.Common.Base;

namespace RpgMod.Common.Skills.Tier2.Sniper
{
    /// <summary>
    /// Long Range - 장거리 전문가.
    /// 거리에 따른 피해 증가.
    /// 저격수의 거리 패시브.
    /// </summary>
    public class LongRange : BaseSkill
    {
        public override string InternalName => "LongRange";
        public override string DisplayName => "Long Range";
        public override string Description => "Gain ranged damage and deal more to distant enemies.";

        public override SkillType SkillType => SkillType.Passive;
        public override JobType RequiredJob => JobType.Sniper;
        public override int RequiredLevel => 66;
        public override int SkillPointCost => 1;
        public override int MaxRank => 10;
        public override string IconTexture => $"{RpgConstants.ICON_PATH}Skills/LongRange";
        public override float CooldownSeconds => 0f;
        public override int ResourceCost => 0;
        public override ResourceType ResourceType => ResourceType.None;

        // 거리 보너스는 RpgPlayer.ModifyHitNPC에서 처리
        // 여기서는 기본 피해 증가
        private static readonly float[] DAMAGE_BONUS = { 0.02f, 0.03f, 0.04f, 0.05f, 0.06f, 0.07f, 0.08f, 0.09f, 0.10f, 0.12f };

        public override void ApplyPassive(Player player)
        {
            if (CurrentRank <= 0) return;
            int rank = CurrentRank;
            
            player.GetDamage(DamageClass.Ranged) += DAMAGE_BONUS[rank - 1];
        }

        public static float GetDistanceBonus(int rank)
        {
            return DAMAGE_BONUS[System.Math.Clamp(rank - 1, 0, 9)];
        }
    }
}
