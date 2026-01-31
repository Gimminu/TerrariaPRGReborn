using Terraria;
using Terraria.ModLoader;
using RpgMod.Common.Base;

namespace RpgMod.Common.Skills.Tier2.BattleMage
{
    /// <summary>
    /// Battle Arcana - 전투 신비술.
    /// 근접과 마법 피해 + 방어 증가.
    /// 배틀메이지의 핵심 패시브.
    /// </summary>
    public class BattleArcana : BaseSkill
    {
        public override string InternalName => "BattleArcana";
        public override string DisplayName => "Battle Arcana";
        public override string Description => "Increase melee, magic damage and defense.";

        public override SkillType SkillType => SkillType.Passive;
        public override JobType RequiredJob => JobType.BattleMage;
        public override int RequiredLevel => 60;
        public override int SkillPointCost => 1;
        public override int MaxRank => 10;
        public override string IconTexture => $"{RpgConstants.ICON_PATH}Skills/BattleArcana";
        public override float CooldownSeconds => 0f;
        public override int ResourceCost => 0;
        public override ResourceType ResourceType => ResourceType.None;

        private static readonly float[] DAMAGE_BONUS = { 0.02f, 0.04f, 0.06f, 0.08f, 0.10f, 0.12f, 0.14f, 0.17f, 0.20f, 0.25f };
        private static readonly int[] DEFENSE_BONUS = { 2, 3, 5, 7, 9, 11, 13, 16, 19, 24 };

        public override void ApplyPassive(Player player)
        {
            if (CurrentRank <= 0) return;
            int rank = CurrentRank;
            
            player.GetDamage(DamageClass.Melee) += DAMAGE_BONUS[rank - 1];
            player.GetDamage(DamageClass.Magic) += DAMAGE_BONUS[rank - 1];
            player.statDefense += DEFENSE_BONUS[rank - 1];
        }
    }
}
