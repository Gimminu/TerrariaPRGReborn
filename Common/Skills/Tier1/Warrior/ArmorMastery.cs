using Terraria;
using RpgMod.Common.Base;

namespace RpgMod.Common.Skills.Tier1.Warrior
{
    /// <summary>
    /// Armor Mastery - 갑옷 숙련.
    /// 방어력 효율 증가 + 추가 방어력.
    /// 전사의 방어 패시브.
    /// </summary>
    public class ArmorMastery : BaseSkill
    {
        public override string InternalName => "ArmorMastery";
        public override string DisplayName => "Armor Mastery";
        public override string Description => "Learn to use armor effectively, increasing defense and damage reduction.";

        public override SkillType SkillType => SkillType.Passive;
        public override JobType RequiredJob => JobType.Warrior;
        public override int RequiredLevel => 20;
        public override int SkillPointCost => 1;
        public override int MaxRank => 10;
        public override string IconTexture => $"{RpgConstants.ICON_PATH}Skills/ArmorMastery";
        public override float CooldownSeconds => 0f;
        public override int ResourceCost => 0;
        public override ResourceType ResourceType => ResourceType.None;
        
        // 전제조건: WeaponMastery 3랭크 이상 필요
        public override string[] PrerequisiteSkills => new[] { "WeaponMastery:3" };

        private static readonly int[] DEFENSE_BONUS = { 2, 4, 6, 8, 10, 12, 15, 18, 21, 25 };
        private static readonly float[] DR_BONUS = { 0.01f, 0.02f, 0.02f, 0.03f, 0.03f, 0.04f, 0.04f, 0.05f, 0.05f, 0.06f };

        public override void ApplyPassive(Player player)
        {
            if (CurrentRank <= 0) return;
            int rank = CurrentRank;
            
            player.statDefense += DEFENSE_BONUS[rank - 1];
            player.endurance += DR_BONUS[rank - 1]; // Damage reduction
        }
    }
}
