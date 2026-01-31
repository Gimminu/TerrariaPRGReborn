using Terraria;
using Terraria.ModLoader;
using RpgMod.Common.Base;

namespace RpgMod.Common.Skills.Tier2.Paladin
{
    /// <summary>
    /// Holy Strength - 신성한 힘.
    /// 근접 피해와 방어력 증가.
    /// </summary>
    public class HolyStrength : BaseSkill
    {
        public override string InternalName => "HolyStrength";
        public override string DisplayName => "Holy Strength";
        public override string Description => "Divine power fills your body, increasing melee damage and defense.";

        public override SkillType SkillType => SkillType.Passive;
        public override JobType RequiredJob => JobType.Paladin;
        public override int RequiredLevel => 60;
        public override int SkillPointCost => 1;
        public override int MaxRank => 10;
        public override string IconTexture => $"{RpgConstants.ICON_PATH}Skills/HolyStrength";
        public override float CooldownSeconds => 0f;
        public override int ResourceCost => 0;
        public override ResourceType ResourceType => ResourceType.None;

        private static readonly float[] DAMAGE_BONUS = { 0.02f, 0.04f, 0.06f, 0.08f, 0.10f, 0.12f, 0.14f, 0.16f, 0.18f, 0.22f };
        private static readonly int[] DEFENSE_BONUS = { 2, 4, 6, 8, 10, 13, 16, 19, 22, 28 };

        public override void ApplyPassive(Player player)
        {
            if (CurrentRank <= 0) return;
            int rank = CurrentRank;
            
            player.GetDamage(DamageClass.Melee) += DAMAGE_BONUS[rank - 1];
            player.statDefense += DEFENSE_BONUS[rank - 1];
        }
    }
}
