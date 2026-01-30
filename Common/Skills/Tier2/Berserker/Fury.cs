using Terraria;
using Terraria.ModLoader;
using Rpg.Common.Base;

namespace Rpg.Common.Skills.Tier2.Berserker
{
    /// <summary>
    /// Fury - 분노.
    /// 잃은 HP 비례 공격력 증가.
    /// 광전사의 핵심 패시브.
    /// </summary>
    public class Fury : BaseSkill
    {
        public override string InternalName => "Fury";
        public override string DisplayName => "Fury";
        public override string Description => "The lower your health, the more damage you deal.";

        public override SkillType SkillType => SkillType.Passive;
        public override JobType RequiredJob => JobType.Berserker;
        public override int RequiredLevel => 60;
        public override int SkillPointCost => 1;
        public override int MaxRank => 10;
        public override string IconTexture => $"{RpgConstants.ICON_PATH}Skills/Fury";
        public override float CooldownSeconds => 0f;
        public override int ResourceCost => 0;
        public override ResourceType ResourceType => ResourceType.None;

        // 잃은 HP 1%당 공격력 증가
        private static readonly float[] DAMAGE_PER_PERCENT = { 0.002f, 0.004f, 0.006f, 0.008f, 0.01f, 0.012f, 0.014f, 0.016f, 0.018f, 0.02f };

        public override void ApplyPassive(Player player)
        {
            if (CurrentRank <= 0) return;
            int rank = CurrentRank;
            
            float missingHpPercent = (1f - ((float)player.statLife / player.statLifeMax2)) * 100f;
            float bonusDamage = missingHpPercent * DAMAGE_PER_PERCENT[rank - 1];
            
            player.GetDamage(DamageClass.Generic) += bonusDamage;
        }
    }
}
