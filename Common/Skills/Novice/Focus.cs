using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using RpgMod.Common.Base;

namespace RpgMod.Common.Skills.Novice
{
    /// <summary>
    /// Focus - 집중하여 다음 공격의 크리티컬 확률을 높이는 버프.
    /// 초보자도 결정적 한방을 노릴 수 있게 해주는 스킬.
    /// 만렙 시 쿨타임 = 지속시간 (12초)
    /// </summary>
    public class Focus : BaseSkill
    {
        public override string InternalName => "Focus";
        public override string DisplayName => "Focus";
        public override string Description => "Concentrate to increase critical strike chance for a duration.";

        public override SkillType SkillType => SkillType.Buff;
        public override JobType RequiredJob => JobType.Novice;
        public override int RequiredLevel => 2;
        public override int SkillPointCost => 1;
        public override int MaxRank => 5;
        public override string IconTexture => $"{RpgConstants.ICON_PATH}Skills/Focus";
        
        // 만렙 시 쿨 = 지속 (12초)
        public override float CooldownSeconds => 20f - (CurrentRank * 1.6f); // 20 -> 12초
        public override int ResourceCost => 10;
        public override ResourceType ResourceType => ResourceType.Stamina;

        // 랭크별 지속시간 (만렙 12초)
        private static readonly int[] DURATION_SECONDS = { 6, 8, 9, 10, 12 };

        protected override void OnActivate(Player player)
        {
            int rank = System.Math.Max(1, CurrentRank);
            int duration = DURATION_SECONDS[rank - 1] * 60;

            // Rage 버프 = 크리티컬 확률 증가
            player.AddBuff(BuffID.Rage, duration);

            PlayEffects(player);
            ShowMessage(player, "Focused!", Color.Yellow);
        }

        private void PlayEffects(Player player)
        {
            SoundEngine.PlaySound(SoundID.MaxMana, player.position);
            for (int i = 0; i < 20; i++)
            {
                float angle = MathHelper.TwoPi * i / 20f;
                Vector2 offset = new Vector2((float)System.Math.Cos(angle), (float)System.Math.Sin(angle)) * 25f;
                Dust dust = Dust.NewDustDirect(player.Center + offset, 4, 4, DustID.YellowTorch, 
                    -offset.X * 0.05f, -offset.Y * 0.05f, 100, Color.Yellow, 1.0f);
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
