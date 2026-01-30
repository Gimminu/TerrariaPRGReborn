using Terraria;
using Terraria.ModLoader;
using Rpg.Common.Base;

namespace Rpg.Common.Skills.Tier3.Overlord
{
    /// <summary>
    /// Supreme Commander - 최고 지휘관.
    /// 소환수 피해와 슬롯 대폭 증가.
    /// 오버로드의 핵심 패시브.
    /// </summary>
    public class SupremeCommander : BaseSkill
    {
        public override string InternalName => "SupremeCommander";
        public override string DisplayName => "Supreme Commander";
        public override string Description => "Greatly increase minion damage and slots.";

        public override SkillType SkillType => SkillType.Passive;
        public override JobType RequiredJob => JobType.Overlord;
        public override int RequiredLevel => 120;
        public override int SkillPointCost => 1;
        public override int MaxRank => 10;
        public override string IconTexture => $"{RpgConstants.ICON_PATH}Skills/SupremeCommander";
        public override float CooldownSeconds => 0f;
        public override int ResourceCost => 0;
        public override ResourceType ResourceType => ResourceType.None;

        private static readonly float[] DAMAGE_BONUS = { 0.06f, 0.11f, 0.16f, 0.22f, 0.28f, 0.35f, 0.43f, 0.52f, 0.62f, 0.80f };
        private static readonly int[] MINION_SLOTS = { 0, 1, 1, 1, 2, 2, 2, 3, 3, 4 };

        public override void ApplyPassive(Player player)
        {
            if (CurrentRank <= 0) return;
            int rank = CurrentRank;
            
            player.GetDamage(DamageClass.Summon) += DAMAGE_BONUS[rank - 1];
            player.maxMinions += MINION_SLOTS[rank - 1];
        }
    }
}
