using Terraria;
using RpgMod.Common.Base;

namespace RpgMod.Common.Skills.Tier1.Warrior
{
    /// <summary>
    /// Combat Regeneration - 전투 재생.
    /// 체력 회복량 증가.
    /// 전투 중에도 꾸준히 회복되는 전사의 패시브.
    /// </summary>
    public class CombatRegeneration : BaseSkill
    {
        public override string InternalName => "CombatRegeneration";
        public override string DisplayName => "Combat Regeneration";
        public override string Description => "Your body heals rapidly even in the heat of battle.";

        public override SkillType SkillType => SkillType.Passive;
        public override JobType RequiredJob => JobType.Warrior;
        public override int RequiredLevel => 28;
        public override int SkillPointCost => 1;
        public override int MaxRank => 10;
        public override string IconTexture => $"{RpgConstants.ICON_PATH}Skills/CombatRegeneration";
        public override float CooldownSeconds => 0f;
        public override int ResourceCost => 0;
        public override ResourceType ResourceType => ResourceType.None;

        private static readonly int[] REGEN_BONUS = { 1, 2, 3, 4, 5, 6, 7, 9, 11, 14 };

        public override void ApplyPassive(Player player)
        {
            if (CurrentRank <= 0) return;
            int rank = CurrentRank;
            
            player.lifeRegen += REGEN_BONUS[rank - 1];
        }
    }
}
