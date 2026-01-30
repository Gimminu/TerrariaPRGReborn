using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Rpg.Common.Base;

namespace Rpg.Common.Skills.Novice
{
    /// <summary>
    /// Nimble - 이동속도를 높이는 버프.
    /// 탐험과 도주에 유용한 초보자 필수 스킬.
    /// 만렙 시 쿨타임 = 지속시간 (20초)
    /// </summary>
    public class Nimble : BaseSkill
    {
        public override string InternalName => "Nimble";
        public override string DisplayName => "Nimble";
        public override string Description => "Move swiftly, increasing movement speed.";

        public override SkillType SkillType => SkillType.Buff;
        public override JobType RequiredJob => JobType.Novice;
        public override int RequiredLevel => 4;
        public override int SkillPointCost => 1;
        public override int MaxRank => 5;
        public override string IconTexture => $"{RpgConstants.ICON_PATH}Skills/Nimble";
        
        // 만렙 시 쿨 = 지속 (20초)
        public override float CooldownSeconds => 30f - (CurrentRank * 2f); // 30 -> 20초
        public override int ResourceCost => 8;
        public override ResourceType ResourceType => ResourceType.Stamina;

        private static readonly int[] DURATION_SECONDS = { 12, 14, 16, 18, 20 };

        protected override void OnActivate(Player player)
        {
            int rank = System.Math.Max(1, CurrentRank);
            int duration = DURATION_SECONDS[rank - 1] * 60;

            player.AddBuff(BuffID.Swiftness, duration);

            PlayEffects(player);
            ShowMessage(player, "Swift!", Color.Cyan);
        }

        private void PlayEffects(Player player)
        {
            SoundEngine.PlaySound(SoundID.Item24, player.position);
            for (int i = 0; i < 12; i++)
            {
                Dust dust = Dust.NewDustDirect(player.position, player.width, player.height,
                    DustID.Cloud, Main.rand.NextFloat(-3f, 3f), 0, 100, Color.White, 1.0f);
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
