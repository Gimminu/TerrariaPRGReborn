using Terraria;
using RpgMod.Common.Base;

namespace RpgMod.Common.Skills.Tier2.Cleric
{
    /// <summary>
    /// Holy Armor - 성스러운 갑옷.
    /// 방어력 증가.
    /// 클레릭의 방어 패시브.
    /// </summary>
    public class HolyArmor : BaseSkill
    {
        public override string InternalName => "HolyArmor";
        public override string DisplayName => "Holy Armor";
        public override string Description => "Increase defense.";

        public override SkillType SkillType => SkillType.Passive;
        public override JobType RequiredJob => JobType.Cleric;
        public override int RequiredLevel => 75;
        public override int SkillPointCost => 1;
        public override int MaxRank => 10;
        public override string IconTexture => $"{RpgConstants.ICON_PATH}Skills/HolyArmor";
        public override float CooldownSeconds => 0f;
        public override int ResourceCost => 0;
        public override ResourceType ResourceType => ResourceType.None;

        private static readonly int[] DEFENSE_BONUS = { 3, 5, 8, 11, 14, 18, 22, 27, 33, 42 };

        public override void ApplyPassive(Player player)
        {
            if (CurrentRank <= 0) return;
            int rank = CurrentRank;
            
            player.statDefense += DEFENSE_BONUS[rank - 1];
        }
    }
}
