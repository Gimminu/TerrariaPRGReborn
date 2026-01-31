using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using RpgMod.Common.Base;

namespace RpgMod.Common.Skills.Tier1.Ranger
{
    /// <summary>
    /// Multishot - 다중 사격.
    /// 추가 발사체를 쏜다 (버프로 구현).
    /// 만렙 시 쿨타임 = 지속시간 (15초)
    /// 강한 버프이므로 짧은 지속시간.
    /// </summary>
    public class Multishot : BaseSkill
    {
        public override string InternalName => "Multishot";
        public override string DisplayName => "Multishot";
        public override string Description => "Your arrows split into multiple projectiles.";

        public override SkillType SkillType => SkillType.Buff;
        public override JobType RequiredJob => JobType.Ranger;
        public override int RequiredLevel => 20;
        public override int SkillPointCost => 1;
        public override int MaxRank => 10;
        public override string IconTexture => $"{RpgConstants.ICON_PATH}Skills/Multishot";
        
        // 강한 버프 - 쿨 > 지속
        public override float CooldownSeconds => 25f - (CurrentRank * 1f); // 25 -> 15초
        public override int ResourceCost => 35 - CurrentRank;
        public override ResourceType ResourceType => ResourceType.Stamina;

        private static readonly int[] DURATION_SECONDS = { 6, 7, 8, 9, 9, 10, 10, 11, 11, 12 };

        protected override void OnActivate(Player player)
        {
            int rank = System.Math.Max(1, CurrentRank);
            int duration = DURATION_SECONDS[rank - 1] * 60;

            // 테라리아의 AmmoBox 버프 = 탄약 소비 감소
            player.AddBuff(BuffID.AmmoBox, duration);

            PlayEffects(player);
            ShowMessage(player, "Multishot!", Color.LightGreen);
        }

        private void PlayEffects(Player player)
        {
            SoundEngine.PlaySound(SoundID.Item5, player.position);
            
            for (int i = 0; i < 3; i++)
            {
                float angle = MathHelper.ToRadians(-15 + i * 15);
                Vector2 dir = new Vector2((float)System.Math.Cos(angle), (float)System.Math.Sin(angle)) * player.direction;
                for (int j = 0; j < 5; j++)
                {
                    Dust dust = Dust.NewDustDirect(player.Center + dir * (20 + j * 10), 4, 4, DustID.GreenTorch,
                        dir.X * 2f, dir.Y * 2f, 100, Color.LightGreen, 0.8f);
                    dust.noGravity = true;
                }
            }
        }

        private void ShowMessage(Player player, string text, Color color)
        {
            if (Main.netMode != NetmodeID.Server)
                CombatText.NewText(player.Hitbox, color, text);
        }
    }
}
