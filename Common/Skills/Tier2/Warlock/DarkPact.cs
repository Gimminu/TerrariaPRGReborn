using Terraria;
using Terraria.ModLoader;
using RpgMod.Common.Base;

namespace RpgMod.Common.Skills.Tier2.Warlock
{
    /// <summary>
    /// Dark Pact - 암흑 계약.
    /// 마법 피해 증가, 체력 감소.
    /// 흑마법사의 트레이드오프 패시브.
    /// </summary>
    public class DarkPact : BaseSkill
    {
        public override string InternalName => "DarkPact";
        public override string DisplayName => "Dark Pact";
        public override string Description => "Sacrifice health for increased magic damage.";

        public override SkillType SkillType => SkillType.Passive;
        public override JobType RequiredJob => JobType.Warlock;
        public override int RequiredLevel => 65;
        public override int SkillPointCost => 1;
        public override int MaxRank => 10;
        public override string IconTexture => $"{RpgConstants.ICON_PATH}Skills/DarkPact";
        public override float CooldownSeconds => 0f;
        public override int ResourceCost => 0;
        public override ResourceType ResourceType => ResourceType.None;

        private static readonly float[] DAMAGE_BONUS = { 0.06f, 0.10f, 0.14f, 0.18f, 0.22f, 0.26f, 0.30f, 0.35f, 0.42f, 0.55f };
        private static readonly int[] HP_PENALTY = { 10, 18, 25, 32, 38, 44, 50, 56, 62, 70 };

        public override void ApplyPassive(Player player)
        {
            if (CurrentRank <= 0) return;
            int rank = CurrentRank;
            
            player.GetDamage(DamageClass.Magic) += DAMAGE_BONUS[rank - 1];
            player.statLifeMax2 -= HP_PENALTY[rank - 1];
        }
    }
}
