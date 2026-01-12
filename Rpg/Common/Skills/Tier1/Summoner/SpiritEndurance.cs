using Terraria;
using Rpg.Common.Base;

namespace Rpg.Common.Skills.Tier1.Summoner
{
    /// <summary>
    /// Spirit Endurance - 정령 내구.
    /// 소환수가 있을 때 받는 피해 감소.
    /// 소환사의 방어 패시브.
    /// </summary>
    public class SpiritEndurance : BaseSkill
    {
        public override string InternalName => "SpiritEndurance";
        public override string DisplayName => "Spirit Endurance";
        public override string Description => "Your spirits shield you, reducing damage taken while minions are active.";

        public override SkillType SkillType => SkillType.Passive;
        public override JobType RequiredJob => JobType.Summoner;
        public override int RequiredLevel => 24;
        public override int SkillPointCost => 1;
        public override int MaxRank => 10;
        public override string IconTexture => $"{RpgConstants.ICON_PATH}Skills/SpiritEndurance";
        public override float CooldownSeconds => 0f;
        public override int ResourceCost => 0;
        public override ResourceType ResourceType => ResourceType.None;

        private static readonly float[] DR_BONUS = { 0.01f, 0.02f, 0.03f, 0.04f, 0.05f, 0.06f, 0.07f, 0.08f, 0.09f, 0.10f };

        public override void ApplyPassive(Player player)
        {
            if (CurrentRank <= 0) return;
            int rank = CurrentRank;
            
            // 미니언이 있을 때만 피해 감소
            int minionCount = 0;
            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                var proj = Main.projectile[i];
                if (proj.active && proj.owner == player.whoAmI && proj.minion)
                    minionCount++;
            }
            
            if (minionCount > 0)
            {
                player.endurance += DR_BONUS[rank - 1];
            }
        }
    }
}
