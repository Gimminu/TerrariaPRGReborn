using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using Rpg.Common.Base;

namespace Rpg.Common.Skills.Tier2.Knight
{
    /// <summary>
    /// Counter Strike - 반격.
    /// 피해를 받을 때 자동으로 반격.
    /// </summary>
    public class CounterStrike : BaseSkill
    {
        public override string InternalName => "CounterStrike";
        public override string DisplayName => "Counter Strike";
        public override string Description => "When hit, automatically counter-attack nearby enemies.";

        public override SkillType SkillType => SkillType.Buff;
        public override JobType RequiredJob => JobType.Knight;
        public override int RequiredLevel => 75;
        public override int SkillPointCost => 1;
        public override int MaxRank => 10;
        public override string IconTexture => $"{RpgConstants.ICON_PATH}Skills/CounterStrike";
        
        public override float CooldownSeconds => 40f - (CurrentRank * 1.5f); // 40 -> 25초
        public override int ResourceCost => 35;
        public override ResourceType ResourceType => ResourceType.Stamina;

        private static readonly int[] DURATION_SECONDS = { 12, 14, 16, 17, 18, 19, 20, 22, 24, 25 };

        protected override void OnActivate(Player player)
        {
            int rank = System.Math.Max(1, CurrentRank);
            int duration = DURATION_SECONDS[rank - 1] * 60;

            // Thorns 버프 사용 (반사 피해)
            player.AddBuff(BuffID.Thorns, duration);

            PlayEffects(player);
            ShowMessage(player, "Counter Ready!", Color.Orange);
        }

        private void PlayEffects(Player player)
        {
            SoundEngine.PlaySound(SoundID.Item35, player.position);
            
            for (int i = 0; i < 20; i++)
            {
                Dust dust = Dust.NewDustDirect(player.position, player.width, player.height,
                    DustID.Torch, Main.rand.NextFloat(-2f, 2f), Main.rand.NextFloat(-2f, 0f), 100, Color.Orange, 1.0f);
                dust.noGravity = true;
            }
        }

        private void ShowMessage(Player player, string text, Color color)
        {
            if (Main.netMode != NetmodeID.Server)
                CombatText.NewText(player.Hitbox, color, text);
        }
    }
}
