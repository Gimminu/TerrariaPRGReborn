using Terraria;
using RpgMod.Common.Base;

namespace RpgMod.Common.Skills.Tier2.Spellblade
{
    /// <summary>
    /// Mana Blade - 마나 칼날.
    /// 근접 공격 시 마나 회복.
    /// 마검사의 마나 회수 패시브.
    /// </summary>
    public class ManaBlade : BaseSkill
    {
        public override string InternalName => "ManaBlade";
        public override string DisplayName => "Mana Blade";
        public override string Description => "Restore mana on melee hits and improve mana regeneration.";

        public override SkillType SkillType => SkillType.Passive;
        public override JobType RequiredJob => JobType.Spellblade;
        public override int RequiredLevel => 70;
        public override int SkillPointCost => 1;
        public override int MaxRank => 10;
        public override string IconTexture => $"{RpgConstants.ICON_PATH}Skills/ManaBlade";
        public override float CooldownSeconds => 0f;
        public override int ResourceCost => 0;
        public override ResourceType ResourceType => ResourceType.None;

        // 적중 시 마나 회복 (RpgPlayer.OnHitNPC에서 처리)
        private static readonly int[] MANA_ON_HIT = { 2, 3, 4, 5, 6, 7, 8, 9, 11, 15 };

        public static int GetManaOnHit(int rank)
        {
            return MANA_ON_HIT[System.Math.Clamp(rank - 1, 0, 9)];
        }

        public override void ApplyPassive(Player player)
        {
            if (CurrentRank <= 0) return;
            
            // 마나 재생 보너스
            player.manaRegen += CurrentRank * 2;
        }
    }
}
