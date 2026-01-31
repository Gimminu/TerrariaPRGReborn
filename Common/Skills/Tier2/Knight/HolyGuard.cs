using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using RpgMod.Common.Base;

namespace RpgMod.Common.Skills.Tier2.Knight
{
    /// <summary>
    /// Holy Guard - 신성한 수호.
    /// 짧은 시간 완전 무적.
    /// 기사의 비상 방어기 (강한 버프).
    /// </summary>
    public class HolyGuard : BaseSkill
    {
        public override string InternalName => "HolyGuard";
        public override string DisplayName => "Holy Guard";
        public override string Description => "Invoke divine protection, becoming completely immune to damage briefly.";

        public override SkillType SkillType => SkillType.Buff;
        public override JobType RequiredJob => JobType.Knight;
        public override int RequiredLevel => 75;
        public override int SkillPointCost => 1;
        public override int MaxRank => 10;
        public override string IconTexture => $"{RpgConstants.ICON_PATH}Skills/HolyGuard";
        
        // 매우 강한 버프 - 긴 쿨타임
        public override float CooldownSeconds => 60f - (CurrentRank * 2f); // 60 -> 40초
        public override int ResourceCost => 60;
        public override ResourceType ResourceType => ResourceType.Stamina;

        private static readonly int[] DURATION_FRAMES = { 60, 75, 90, 105, 120, 135, 150, 165, 180, 210 };

        protected override void OnActivate(Player player)
        {
            int rank = System.Math.Max(1, CurrentRank);
            int duration = DURATION_FRAMES[rank - 1];

            player.immune = true;
            player.immuneTime = duration;

            PlayEffects(player);
            ShowMessage(player, "INVINCIBLE!", Color.Gold);
        }

        private void PlayEffects(Player player)
        {
            SoundEngine.PlaySound(SoundID.Item29, player.position);
            
            for (int i = 0; i < 50; i++)
            {
                float angle = MathHelper.TwoPi * i / 50f;
                Vector2 offset = new Vector2((float)System.Math.Cos(angle), (float)System.Math.Sin(angle)) * 40f;
                Dust dust = Dust.NewDustDirect(player.Center + offset, 4, 4, DustID.GoldFlame,
                    0, -1f, 100, Color.Gold, 1.5f);
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
