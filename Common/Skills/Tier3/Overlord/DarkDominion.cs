using Terraria;
using Terraria.ModLoader;
using RpgMod.Common.Base;

namespace RpgMod.Common.Skills.Tier3.Overlord
{
    /// <summary>
    /// Dark Dominion - 암흑 지배.
    /// 소환수와 마법 피해 + 센트리 슬롯 증가.
    /// 오버로드의 하이브리드 패시브.
    /// </summary>
    public class DarkDominion : BaseSkill
    {
        public override string InternalName => "DarkDominion";
        public override string DisplayName => "Dark Dominion";
        public override string Description => "Increase summon and magic damage, and sentry slots.";

        public override SkillType SkillType => SkillType.Passive;
        public override JobType RequiredJob => JobType.Overlord;
        public override int RequiredLevel => 135;
        public override int SkillPointCost => 1;
        public override int MaxRank => 10;
        public override string IconTexture => $"{RpgConstants.ICON_PATH}Skills/DarkDominion";
        public override float CooldownSeconds => 0f;
        public override int ResourceCost => 0;
        public override ResourceType ResourceType => ResourceType.None;

        private static readonly float[] DAMAGE_BONUS = { 0.04f, 0.07f, 0.11f, 0.15f, 0.19f, 0.24f, 0.29f, 0.35f, 0.42f, 0.55f };
        private static readonly int[] SENTRY_SLOTS = { 0, 0, 1, 1, 1, 1, 2, 2, 2, 3 };

        public override void ApplyPassive(Player player)
        {
            if (CurrentRank <= 0) return;
            int rank = CurrentRank;
            
            player.GetDamage(DamageClass.Summon) += DAMAGE_BONUS[rank - 1];
            player.GetDamage(DamageClass.Magic) += DAMAGE_BONUS[rank - 1];
            player.maxTurrets += SENTRY_SLOTS[rank - 1];
        }
    }
}
