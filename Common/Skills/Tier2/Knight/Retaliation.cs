using Terraria;
using Terraria.ModLoader;
using Rpg.Common.Base;

namespace Rpg.Common.Skills.Tier2.Knight
{
    /// <summary>
    /// Retaliation - 반격.
    /// 근접 공격력 증가 (방어력 기반).
    /// 기사도 공격할 수 있게 해주는 패시브.
    /// </summary>
    public class Retaliation : BaseSkill
    {
        public override string InternalName => "Retaliation";
        public override string DisplayName => "Retaliation";
        public override string Description => "Turn defense into offense, gaining melee damage based on your defense.";

        public override SkillType SkillType => SkillType.Passive;
        public override JobType RequiredJob => JobType.Knight;
        public override int RequiredLevel => 78;
        public override int SkillPointCost => 1;
        public override int MaxRank => 10;
        public override string IconTexture => $"{RpgConstants.ICON_PATH}Skills/Retaliation";
        public override float CooldownSeconds => 0f;
        public override int ResourceCost => 0;
        public override ResourceType ResourceType => ResourceType.None;

        private static readonly float[] CONVERSION = { 0.005f, 0.01f, 0.015f, 0.02f, 0.025f, 0.03f, 0.035f, 0.04f, 0.045f, 0.05f };

        public override void ApplyPassive(Player player)
        {
            if (CurrentRank <= 0) return;
            int rank = CurrentRank;
            
            // 방어력의 일정 비율만큼 근접 피해 증가
            float bonusDamage = player.statDefense * CONVERSION[rank - 1];
            player.GetDamage(DamageClass.Melee) += bonusDamage;
        }
    }
}
