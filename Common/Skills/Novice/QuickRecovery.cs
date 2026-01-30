using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Rpg.Common.Base;

namespace Rpg.Common.Skills.Novice
{
    /// <summary>
    /// Quick Recovery - 빠르게 체력을 회복하는 초보자 힐 스킬.
    /// 위험할 때 쓰는 비상용 회복기.
    /// </summary>
    public class QuickRecovery : BaseSkill
    {
        public override string InternalName => "QuickRecovery";
        public override string DisplayName => "Quick Recovery";
        public override string Description => "Quickly restore a portion of your health. Essential for beginners.";

        public override SkillType SkillType => SkillType.Active;
        public override JobType RequiredJob => JobType.Novice;
        public override int RequiredLevel => 1;
        public override int SkillPointCost => 1;
        public override int MaxRank => 5;
        public override string IconTexture => $"{RpgConstants.ICON_PATH}Skills/QuickRecovery";
        
        // 만렙 기준 20초 쿨 - 비상용이라 좀 긴 편
        public override float CooldownSeconds => 30f - (CurrentRank * 2f); // 30 -> 20초
        public override int ResourceCost => 15;
        public override ResourceType ResourceType => ResourceType.Stamina;

        // 랭크별 회복량 (기본 + 최대체력 %)
        private static readonly int[] BASE_HEAL = { 15, 25, 35, 50, 70 };
        private static readonly float[] PERCENT_HEAL = { 0.05f, 0.08f, 0.11f, 0.14f, 0.18f };

        protected override void OnActivate(Player player)
        {
            int rank = System.Math.Max(1, CurrentRank);
            int heal = BASE_HEAL[rank - 1] + (int)(player.statLifeMax2 * PERCENT_HEAL[rank - 1]);

            player.statLife = System.Math.Min(player.statLife + heal, player.statLifeMax2);
            player.HealEffect(heal, true);

            PlayEffects(player);
        }

        private void PlayEffects(Player player)
        {
            SoundEngine.PlaySound(SoundID.Item3, player.position);
            for (int i = 0; i < 15; i++)
            {
                Dust dust = Dust.NewDustDirect(player.position, player.width, player.height,
                    DustID.HealingPlus, Main.rand.NextFloat(-2f, 2f), Main.rand.NextFloat(-3f, 0f), 100, Color.LightGreen, 1.2f);
                dust.noGravity = true;
            }
        }
    }
}
