using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using RpgMod.Common.Base;

namespace RpgMod.Common.Skills.Tier1.Warrior
{
    /// <summary>
    /// War Cry - 전투함성으로 자신과 주변 아군의 공격력을 높인다.
    /// 파티 플레이에서도 유용한 공격 버프.
    /// 만렙 시 쿨타임 = 지속시간 (30초)
    /// </summary>
    public class WarCry : BaseSkill
    {
        public override string InternalName => "WarCry";
        public override string DisplayName => "War Cry";
        public override string Description => "Let out a mighty war cry, increasing your damage.";

        public override SkillType SkillType => SkillType.Buff;
        public override JobType RequiredJob => JobType.Warrior;
        public override int RequiredLevel => 12;
        public override int SkillPointCost => 1;
        public override int MaxRank => 10;
        public override string IconTexture => $"{RpgConstants.ICON_PATH}Skills/WarCry";
        
        // 만렙 시 쿨 = 지속 (30초)
        public override float CooldownSeconds => 45f - (CurrentRank * 1.5f); // 45 -> 30초
        public override int ResourceCost => 25 - CurrentRank; // 25 -> 15
        public override ResourceType ResourceType => ResourceType.Stamina;

        private static readonly int[] DURATION_SECONDS = { 15, 18, 20, 22, 24, 26, 27, 28, 29, 30 };

        protected override void OnActivate(Player player)
        {
            int rank = System.Math.Max(1, CurrentRank);
            int duration = DURATION_SECONDS[rank - 1] * 60;

            // Wrath 버프 = 공격력 증가
            player.AddBuff(BuffID.Wrath, duration);

            PlayEffects(player);
            ShowMessage(player, "WAR CRY!", Color.Red);
        }

        private void PlayEffects(Player player)
        {
            SoundEngine.PlaySound(SoundID.Roar with { Pitch = 0.5f }, player.position);
            
            // 파동 이펙트
            for (int i = 0; i < 40; i++)
            {
                float angle = MathHelper.TwoPi * i / 40f;
                Vector2 offset = new Vector2((float)System.Math.Cos(angle), (float)System.Math.Sin(angle)) * 50f;
                Dust dust = Dust.NewDustDirect(player.Center + offset, 4, 4, DustID.InfernoFork,
                    offset.X * 0.1f, offset.Y * 0.1f, 150, Color.OrangeRed, 1.3f);
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
