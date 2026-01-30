using Terraria;
using Terraria.ModLoader;
using Rpg.Common.Base;

namespace Rpg.Common.Skills.Tier3.Gunmaster
{
    /// <summary>
    /// Gun God - 총의 신.
    /// 총기 피해와 크리티컬 대폭 증가.
    /// 건마스터의 핵심 패시브.
    /// </summary>
    public class GunGod : BaseSkill
    {
        public override string InternalName => "GunGod";
        public override string DisplayName => "Gun God";
        public override string Description => "Become a god of firearms, greatly increasing ranged damage and critical.";

        public override SkillType SkillType => SkillType.Passive;
        public override JobType RequiredJob => JobType.Gunmaster;
        public override int RequiredLevel => 120;
        public override int SkillPointCost => 1;
        public override int MaxRank => 10;
        public override string IconTexture => $"{RpgConstants.ICON_PATH}Skills/GunGod";
        public override float CooldownSeconds => 0f;
        public override int ResourceCost => 0;
        public override ResourceType ResourceType => ResourceType.None;

        private static readonly float[] DAMAGE_BONUS = { 0.06f, 0.11f, 0.16f, 0.21f, 0.27f, 0.34f, 0.41f, 0.49f, 0.58f, 0.75f };
        private static readonly int[] CRIT_BONUS = { 5, 9, 13, 17, 22, 27, 33, 40, 48, 62 };

        public override void ApplyPassive(Player player)
        {
            if (CurrentRank <= 0) return;
            int rank = CurrentRank;
            
            player.GetDamage(DamageClass.Ranged) += DAMAGE_BONUS[rank - 1];
            player.GetCritChance(DamageClass.Ranged) += CRIT_BONUS[rank - 1];
        }
    }
}
