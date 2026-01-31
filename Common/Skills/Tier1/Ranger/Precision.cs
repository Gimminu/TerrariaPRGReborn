using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using RpgMod.Common.Base;

namespace RpgMod.Common.Skills.Tier1.Ranger
{
    /// <summary>
    /// Precision - 정밀 조준.
    /// 크리티컬 확률과 크리티컬 피해를 높인다.
    /// 만렙 시 쿨타임 = 지속시간 (25초)
    /// </summary>
    public class Precision : BaseSkill
    {
        public override string InternalName => "Precision";
        public override string DisplayName => "Precision";
        public override string Description => "Focus your aim, increasing critical chance.";

        public override SkillType SkillType => SkillType.Buff;
        public override JobType RequiredJob => JobType.Ranger;
        public override int RequiredLevel => 16;
        public override int SkillPointCost => 1;
        public override int MaxRank => 10;
        public override string IconTexture => $"{RpgConstants.ICON_PATH}Skills/Precision";
        
        // 만렙 시 쿨 = 지속 (25초)
        public override float CooldownSeconds => 40f - (CurrentRank * 1.5f); // 40 -> 25초
        public override int ResourceCost => 25;
        public override ResourceType ResourceType => ResourceType.Stamina;

        private static readonly int[] DURATION_SECONDS = { 12, 14, 16, 18, 19, 20, 21, 22, 24, 25 };

        protected override void OnActivate(Player player)
        {
            int rank = System.Math.Max(1, CurrentRank);
            int duration = DURATION_SECONDS[rank - 1] * 60;

            player.AddBuff(BuffID.Rage, duration);

            PlayEffects(player);
            ShowMessage(player, "Precision!", Color.Yellow);
        }

        private void PlayEffects(Player player)
        {
            SoundEngine.PlaySound(SoundID.Item35, player.position);
            
            // 조준 이펙트
            Vector2 forward = new Vector2(player.direction * 50, 0);
            for (int i = 0; i < 10; i++)
            {
                Dust dust = Dust.NewDustDirect(player.Center + forward + new Vector2(Main.rand.NextFloat(-5, 5), Main.rand.NextFloat(-5, 5)), 4, 4, DustID.YellowTorch,
                    0, 0, 100, Color.Yellow, 1.0f);
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
