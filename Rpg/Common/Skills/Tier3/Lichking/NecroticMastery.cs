using Terraria;
using Terraria.ModLoader;
using Rpg.Common.Base;

namespace Rpg.Common.Skills.Tier3.LichKing
{
    /// <summary>
    /// Necrotic Mastery - 사령 숙련.
    /// 소환수와 마법 피해 대폭 증가.
    /// 리치킹의 핵심 패시브.
    /// </summary>
    public class NecroticMastery : BaseSkill
    {
        public override string InternalName => "NecroticMastery";
        public override string DisplayName => "Necrotic Mastery";
        public override string Description => "Master the arts of necromancy, greatly increasing summon and magic damage.";

        public override SkillType SkillType => SkillType.Passive;
        public override JobType RequiredJob => JobType.LichKing;
        public override int RequiredLevel => 120;
        public override int SkillPointCost => 1;
        public override int MaxRank => 10;
        public override string IconTexture => $"{RpgConstants.ICON_PATH}Skills/NecroticMastery";
        public override float CooldownSeconds => 0f;
        public override int ResourceCost => 0;
        public override ResourceType ResourceType => ResourceType.None;

        private static readonly float[] DAMAGE_BONUS = { 0.05f, 0.09f, 0.14f, 0.19f, 0.24f, 0.30f, 0.37f, 0.44f, 0.53f, 0.68f };

        public override void ApplyPassive(Player player)
        {
            if (CurrentRank <= 0) return;
            int rank = CurrentRank;
            
            player.GetDamage(DamageClass.Summon) += DAMAGE_BONUS[rank - 1];
            player.GetDamage(DamageClass.Magic) += DAMAGE_BONUS[rank - 1];
        }
    }
}
