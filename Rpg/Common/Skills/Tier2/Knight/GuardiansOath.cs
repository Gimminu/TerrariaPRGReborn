using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Rpg.Common.Base;
using Rpg.Common.Players;

namespace Rpg.Common.Skills.Tier2.Knight
{
    /// <summary>
    /// Guardian's Oath - 수호자의 맹세.
    /// 자신과 주변 아군의 방어력을 크게 증가.
    /// 만렙 시 쿨 = 지속 (30초)
    /// </summary>
    public class GuardiansOath : BaseSkill
    {
        public override string InternalName => "GuardiansOath";
        public override string DisplayName => "Guardian's Oath";
        public override string Description => "Swear an oath to protect, greatly increasing defense for you and nearby allies.";

        public override SkillType SkillType => SkillType.Buff;
        public override JobType RequiredJob => JobType.Knight;
        public override int RequiredLevel => 70;
        public override int SkillPointCost => 1;
        public override int MaxRank => 10;
        public override string IconTexture => $"{RpgConstants.ICON_PATH}Skills/GuardiansOath";
        
        public override float CooldownSeconds => 45f - (CurrentRank * 1.5f); // 45 -> 30초
        public override int ResourceCost => 50;
        public override ResourceType ResourceType => ResourceType.Stamina;

        private static readonly int[] DURATION_SECONDS = { 15, 18, 20, 22, 24, 26, 27, 28, 29, 30 };
        private static readonly int[] DEFENSE_BONUS = { 10, 14, 18, 22, 26, 30, 35, 40, 45, 50 };

        protected override void OnActivate(Player player)
        {
            int rank = System.Math.Max(1, CurrentRank);
            int duration = DURATION_SECONDS[rank - 1] * 60;

            player.AddBuff(BuffID.Ironskin, duration);
            
            var rpgPlayer = player.GetModPlayer<RpgPlayer>();
            rpgPlayer.AddTemporaryDefense(DEFENSE_BONUS[rank - 1], duration);

            PlayEffects(player);
            ShowMessage(player, "Guardian's Oath!", Color.Gold);
        }

        private void PlayEffects(Player player)
        {
            SoundEngine.PlaySound(SoundID.Item29, player.position);
            
            for (int i = 0; i < 35; i++)
            {
                float angle = MathHelper.TwoPi * i / 35f;
                Vector2 offset = new Vector2((float)System.Math.Cos(angle), (float)System.Math.Sin(angle)) * 45f;
                Dust dust = Dust.NewDustDirect(player.Center + offset, 4, 4, DustID.GoldCoin,
                    0, 0, 100, Color.Gold, 1.3f);
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
