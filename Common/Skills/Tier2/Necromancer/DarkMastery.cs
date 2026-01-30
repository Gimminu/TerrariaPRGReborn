using Terraria;
using Terraria.ModLoader;
using Rpg.Common.Base;

namespace Rpg.Common.Skills.Tier2.Necromancer
{
    /// <summary>
    /// Dark Mastery - 암흑 숙련.
    /// 소환수와 마법 피해 증가.
    /// 강령술사의 핵심 패시브.
    /// </summary>
    public class DarkMastery : BaseSkill
    {
        public override string InternalName => "DarkMastery";
        public override string DisplayName => "Dark Mastery";
        public override string Description => "Increase summon and magic damage.";

        public override SkillType SkillType => SkillType.Passive;
        public override JobType RequiredJob => JobType.Necromancer;
        public override int RequiredLevel => 60;
        public override int SkillPointCost => 1;
        public override int MaxRank => 10;
        public override string IconTexture => $"{RpgConstants.ICON_PATH}Skills/DarkMastery";
        public override float CooldownSeconds => 0f;
        public override int ResourceCost => 0;
        public override ResourceType ResourceType => ResourceType.None;

        private static readonly float[] DAMAGE_BONUS = { 0.03f, 0.05f, 0.08f, 0.10f, 0.13f, 0.16f, 0.19f, 0.22f, 0.26f, 0.33f };

        public override void ApplyPassive(Player player)
        {
            if (CurrentRank <= 0) return;
            int rank = CurrentRank;
            
            player.GetDamage(DamageClass.Summon) += DAMAGE_BONUS[rank - 1];
            player.GetDamage(DamageClass.Magic) += DAMAGE_BONUS[rank - 1];
        }
    }
}
