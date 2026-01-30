using Terraria;
using Terraria.ModLoader;
using Rpg.Common.Base;

namespace Rpg.Common.Skills.Tier2.Necromancer
{
    /// <summary>
    /// Undead Army - 언데드 군단.
    /// 추가 미니언 슬롯.
    /// </summary>
    public class UndeadArmy : BaseSkill
    {
        public override string InternalName => "UndeadArmy";
        public override string DisplayName => "Undead Army";
        public override string Description => "Command a larger army of undead, gaining additional minion slots.";

        public override SkillType SkillType => SkillType.Passive;
        public override JobType RequiredJob => JobType.Necromancer;
        public override int RequiredLevel => 60;
        public override int SkillPointCost => 1;
        public override int MaxRank => 10;
        public override string IconTexture => $"{RpgConstants.ICON_PATH}Skills/UndeadArmy";
        public override float CooldownSeconds => 0f;
        public override int ResourceCost => 0;
        public override ResourceType ResourceType => ResourceType.None;

        private static readonly int[] MINION_SLOTS = { 0, 0, 1, 1, 1, 2, 2, 2, 2, 3 };
        private static readonly float[] MINION_DAMAGE = { 0.02f, 0.04f, 0.06f, 0.08f, 0.10f, 0.12f, 0.14f, 0.16f, 0.18f, 0.22f };

        public override void ApplyPassive(Player player)
        {
            if (CurrentRank <= 0) return;
            int rank = CurrentRank;
            
            player.maxMinions += MINION_SLOTS[rank - 1];
            player.GetDamage(DamageClass.Summon) += MINION_DAMAGE[rank - 1];
        }
    }
}
