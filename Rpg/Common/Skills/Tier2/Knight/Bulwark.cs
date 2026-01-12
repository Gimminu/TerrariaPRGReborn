using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Rpg.Common.Base;
using Rpg.Common.Players;

namespace Rpg.Common.Skills.Tier2.Knight
{
    /// <summary>
    /// Bulwark - 철벽.
    /// 받는 피해를 크게 줄이지만 이동속도 감소.
    /// 만렙 시 쿨타임 = 지속시간 (15초)
    /// </summary>
    public class Bulwark : BaseSkill
    {
        public override string InternalName => "Bulwark";
        public override string DisplayName => "Bulwark";
        public override string Description => "Become an immovable fortress, greatly reducing damage but slowing movement.";

        public override SkillType SkillType => SkillType.Buff;
        public override JobType RequiredJob => JobType.Knight;
        public override int RequiredLevel => 70;
        public override int SkillPointCost => 1;
        public override int MaxRank => 10;
        public override string IconTexture => $"{RpgConstants.ICON_PATH}Skills/Bulwark";
        
        // 강한 버프 - 쿨 > 지속
        public override float CooldownSeconds => 30f - (CurrentRank * 1.5f); // 30 -> 15초
        public override int ResourceCost => 50;
        public override ResourceType ResourceType => ResourceType.Stamina;

        private static readonly int[] DURATION_SECONDS = { 6, 7, 8, 9, 10, 11, 12, 13, 14, 15 };
        private static readonly int[] DEFENSE_BONUS = { 30, 40, 50, 60, 70, 80, 90, 100, 110, 130 };

        protected override void OnActivate(Player player)
        {
            int rank = System.Math.Max(1, CurrentRank);
            int duration = DURATION_SECONDS[rank - 1] * 60;

            player.AddBuff(BuffID.Stoned, duration / 3); // 잠시 느려짐
            
            var rpgPlayer = player.GetModPlayer<RpgPlayer>();
            rpgPlayer.AddTemporaryDefense(DEFENSE_BONUS[rank - 1], duration);

            PlayEffects(player);
            ShowMessage(player, "BULWARK!", Color.Gray);
        }

        private void PlayEffects(Player player)
        {
            SoundEngine.PlaySound(SoundID.Item52, player.position);
            
            for (int i = 0; i < 40; i++)
            {
                Dust dust = Dust.NewDustDirect(player.position, player.width, player.height,
                    DustID.Stone, Main.rand.NextFloat(-2f, 2f), Main.rand.NextFloat(-3f, 0f), 150, Color.Gray, 1.5f);
                dust.noGravity = false;
            }
        }

        private void ShowMessage(Player player, string text, Color color)
        {
            if (Main.netMode != NetmodeID.Server)
                CombatText.NewText(player.Hitbox, color, text);
        }
    }
}
