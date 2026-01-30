using Terraria;
using Terraria.ModLoader;
using Rpg.Common.Base;

namespace Rpg.Common.Skills.Tier2.Warlock
{
    /// <summary>
    /// Dark Knowledge - 어둠의 지식.
    /// 마법 피해와 마나 증가.
    /// </summary>
    public class DarkKnowledge : BaseSkill
    {
        public override string InternalName => "DarkKnowledge";
        public override string DisplayName => "Dark Knowledge";
        public override string Description => "Deep knowledge of dark arts increases your magic power.";

        public override SkillType SkillType => SkillType.Passive;
        public override JobType RequiredJob => JobType.Warlock;
        public override int RequiredLevel => 75;
        public override int SkillPointCost => 1;
        public override int MaxRank => 10;
        public override string IconTexture => $"{RpgConstants.ICON_PATH}Skills/DarkKnowledge";
        public override float CooldownSeconds => 0f;
        public override int ResourceCost => 0;
        public override ResourceType ResourceType => ResourceType.None;

        private static readonly float[] DAMAGE_BONUS = { 0.02f, 0.04f, 0.06f, 0.08f, 0.10f, 0.13f, 0.16f, 0.19f, 0.22f, 0.28f };
        private static readonly int[] MANA_BONUS = { 10, 20, 30, 40, 55, 70, 85, 105, 130, 170 };

        public override void ApplyPassive(Player player)
        {
            if (CurrentRank <= 0) return;
            int rank = CurrentRank;
            
            player.GetDamage(DamageClass.Magic) += DAMAGE_BONUS[rank - 1];
            player.statManaMax2 += MANA_BONUS[rank - 1];
        }
    }
}
