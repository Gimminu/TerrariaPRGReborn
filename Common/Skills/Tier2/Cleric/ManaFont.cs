using Terraria;
using RpgMod.Common.Base;

namespace RpgMod.Common.Skills.Tier2.Cleric
{
    /// <summary>
    /// Mana Font - 마나 샘.
    /// 마나 재생 증가.
    /// </summary>
    public class ManaFont : BaseSkill
    {
        public override string InternalName => "ManaFont";
        public override string DisplayName => "Mana Font";
        public override string Description => "A font of divine energy, increasing max mana and regeneration.";

        public override SkillType SkillType => SkillType.Passive;
        public override JobType RequiredJob => JobType.Cleric;
        public override int RequiredLevel => 72;
        public override int SkillPointCost => 1;
        public override int MaxRank => 10;
        public override string IconTexture => $"{RpgConstants.ICON_PATH}Skills/ManaFont";
        public override float CooldownSeconds => 0f;
        public override int ResourceCost => 0;
        public override ResourceType ResourceType => ResourceType.None;

        private static readonly int[] MANA_BONUS = { 10, 20, 30, 45, 60, 80, 100, 125, 155, 200 };

        public override void ApplyPassive(Player player)
        {
            if (CurrentRank <= 0) return;
            int rank = CurrentRank;
            
            player.statManaMax2 += MANA_BONUS[rank - 1];
            player.manaRegenBonus += rank * 2;
        }
    }
}
