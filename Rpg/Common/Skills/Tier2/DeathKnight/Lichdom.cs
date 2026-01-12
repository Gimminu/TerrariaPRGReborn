using Terraria;
using Terraria.ModLoader;
using Rpg.Common.Base;

namespace Rpg.Common.Skills.Tier2.DeathKnight
{
    /// <summary>
    /// Lichdom - Death Knight's passive for mana efficiency and damage.
    /// </summary>
    public class Lichdom : BaseSkill
    {
        public override string InternalName => "Lichdom";
        public override string DisplayName => "Lichdom";
        public override string Description => "Improve mana efficiency and damage through dark arts.";

        public override SkillType SkillType => SkillType.Passive;
        public override JobType RequiredJob => JobType.DeathKnight;
        public override int RequiredLevel => RpgConstants.SECOND_JOB_LEVEL + 15;
        public override int SkillPointCost => 1;
        public override int MaxRank => 5;
        public override string IconTexture => $"{RpgConstants.ICON_PATH}Skills/Lichdom";
        public override float CooldownSeconds => 0f;
        public override int ResourceCost => 0;
        public override ResourceType ResourceType => ResourceType.None;

        private static readonly float[] DAMAGE_BONUS = { 0.03f, 0.05f, 0.07f, 0.09f, 0.12f };
        private static readonly float[] MANA_REDUCTION = { 0.04f, 0.06f, 0.08f, 0.10f, 0.14f };

        public override void ApplyPassive(Player player)
        {
            if (CurrentRank <= 0) return;
            int rank = CurrentRank;
            player.GetDamage(DamageClass.Generic) += DAMAGE_BONUS[rank - 1];
            player.manaCost -= MANA_REDUCTION[rank - 1];
            if (player.manaCost < 0.2f) player.manaCost = 0.2f;
        }
    }
}
