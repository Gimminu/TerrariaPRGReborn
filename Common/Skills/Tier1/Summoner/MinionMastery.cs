using Terraria;
using Terraria.ModLoader;
using RpgMod.Common.Base;

namespace RpgMod.Common.Skills.Tier1.Summoner
{
    /// <summary>
    /// Minion Mastery - 소환수 숙련.
    /// 소환 피해와 소환수 수 증가.
    /// 소환사의 핵심 패시브.
    /// </summary>
    public class MinionMastery : BaseSkill
    {
        public override string InternalName => "MinionMastery";
        public override string DisplayName => "Minion Mastery";
        public override string Description => "Master the art of summoning, increasing summon damage and max minion slots.";

        public override SkillType SkillType => SkillType.Passive;
        public override JobType RequiredJob => JobType.Summoner;
        public override int RequiredLevel => 10;
        public override int SkillPointCost => 1;
        public override int MaxRank => 10;
        public override string IconTexture => $"{RpgConstants.ICON_PATH}Skills/MinionMastery";
        public override float CooldownSeconds => 0f;
        public override int ResourceCost => 0;
        public override ResourceType ResourceType => ResourceType.None;

        private static readonly float[] DAMAGE_BONUS = { 0.02f, 0.04f, 0.06f, 0.08f, 0.10f, 0.12f, 0.14f, 0.16f, 0.18f, 0.20f };
        // 미니언 슬롯은 5, 10렙에 +1
        private static readonly int[] MINION_SLOTS = { 0, 0, 0, 0, 1, 1, 1, 1, 1, 2 };

        public override void ApplyPassive(Player player)
        {
            if (CurrentRank <= 0) return;
            int rank = CurrentRank;
            
            player.GetDamage(DamageClass.Summon) += DAMAGE_BONUS[rank - 1];
            player.maxMinions += MINION_SLOTS[rank - 1];
        }
    }
}
