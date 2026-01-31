using Terraria;
using Terraria.ModLoader;
using RpgMod.Common.Base;

namespace RpgMod.Common.Skills.Novice
{
    /// <summary>
    /// Adventurer's Spirit - 모험가의 정신.
    /// 초보자의 패시브로, 기본적인 스탯 증가와 자원 회복 보너스.
    /// 모든 직업으로 전직 후에도 기반이 되는 스킬.
    /// </summary>
    public class AdventurerSpirit : BaseSkill
    {
        public override string InternalName => "AdventurerSpirit";
        public override string DisplayName => "Adventurer's Spirit";
        public override string Description => "The spirit of adventure burns within. Increases all damage, defense, and regeneration.";

        public override SkillType SkillType => SkillType.Passive;
        public override JobType RequiredJob => JobType.Novice;
        public override int RequiredLevel => 5;
        public override int SkillPointCost => 1;
        public override int MaxRank => 5;
        public override string IconTexture => $"{RpgConstants.ICON_PATH}Skills/AdventurerSpirit";
        public override float CooldownSeconds => 0f;
        public override int ResourceCost => 0;
        public override ResourceType ResourceType => ResourceType.None;

        // 만능형 패시브 - 모든 것을 조금씩
        private static readonly float[] DAMAGE_BONUS = { 0.02f, 0.03f, 0.04f, 0.05f, 0.06f };
        private static readonly int[] DEFENSE_BONUS = { 1, 2, 3, 4, 5 };
        private static readonly int[] LIFE_REGEN = { 1, 1, 2, 2, 3 };

        public override void ApplyPassive(Player player)
        {
            if (CurrentRank <= 0) return;
            int rank = CurrentRank;
            
            player.GetDamage(DamageClass.Generic) += DAMAGE_BONUS[rank - 1];
            player.statDefense += DEFENSE_BONUS[rank - 1];
            player.lifeRegen += LIFE_REGEN[rank - 1];
        }
    }
}
