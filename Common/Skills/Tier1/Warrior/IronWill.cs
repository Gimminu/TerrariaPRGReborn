using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using RpgMod.Common.Base;
using RpgMod.Common.Players;

namespace RpgMod.Common.Skills.Tier1.Warrior
{
    /// <summary>
    /// Iron Will - 철의 의지로 방어력을 높이고 넉백 저항을 얻는다.
    /// 전사의 핵심 방어 버프.
    /// 만렙 시 쿨타임 = 지속시간 (30초)
    /// </summary>
    public class IronWill : BaseSkill
    {
        public override string InternalName => "IronWill";
        public override string DisplayName => "Iron Will";
        public override string Description => "Steel your resolve, greatly increasing defense and knockback resistance.";

        public override SkillType SkillType => SkillType.Buff;
        public override JobType RequiredJob => JobType.Warrior;
        public override int RequiredLevel => 15;
        public override int SkillPointCost => 1;
        public override int MaxRank => 10;
        public override string IconTexture => $"{RpgConstants.ICON_PATH}Skills/IronWill";
        
        // 만렙 시 쿨 = 지속 (30초)
        public override float CooldownSeconds => 45f - (CurrentRank * 1.5f); // 45 -> 30초
        public override int ResourceCost => 25 - CurrentRank;
        public override ResourceType ResourceType => ResourceType.Stamina;

        private static readonly int[] DURATION_SECONDS = { 15, 18, 20, 22, 24, 26, 27, 28, 29, 30 };
        private static readonly int[] DEFENSE_BONUS = { 5, 7, 9, 12, 15, 18, 21, 24, 27, 30 };
        private static readonly float[] KNOCKBACK_RESIST = { 0.05f, 0.07f, 0.09f, 0.12f, 0.15f, 0.18f, 0.21f, 0.24f, 0.27f, 0.30f };

        protected override void OnActivate(Player player)
        {
            int rank = System.Math.Max(1, CurrentRank);
            int duration = DURATION_SECONDS[rank - 1] * 60;

            player.AddBuff(BuffID.Ironskin, duration);
            
            // RpgPlayer를 통해 추가 방어력 부여
            var rpgPlayer = player.GetModPlayer<RpgPlayer>();
            rpgPlayer.AddTemporaryDefense(DEFENSE_BONUS[rank - 1], duration);
            rpgPlayer.AddTemporaryKnockbackResist(KNOCKBACK_RESIST[rank - 1], duration);

            PlayEffects(player);
            ShowMessage(player, "Iron Will!", Color.Silver);
        }

        private void PlayEffects(Player player)
        {
            SoundEngine.PlaySound(SoundID.Item37, player.position);
            
            // 방패 형태 이펙트
            for (int i = 0; i < 25; i++)
            {
                float angle = MathHelper.TwoPi * i / 25f;
                Vector2 offset = new Vector2((float)System.Math.Cos(angle), (float)System.Math.Sin(angle)) * 30f;
                Dust dust = Dust.NewDustDirect(player.Center + offset, 4, 4, DustID.Silver,
                    0, 0, 150, Color.Silver, 1.2f);
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
