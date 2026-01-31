using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using RpgMod.Common.Base;

namespace RpgMod.Common.Skills.Tier2.Gunslinger
{
    /// <summary>
    /// Gun Tempo - 총격 템포.
    /// 공격 속도 증가 버프.
    /// 건슬링거의 DPS 버프. 만렙 시 쿨 = 지속 (20초).
    /// </summary>
    public class GunTempo : BaseSkill
    {
        public override string InternalName => "GunTempo";
        public override string DisplayName => "Gun Tempo";
        public override string Description => "Enter a rhythm of rapid fire, increasing ranged attack speed and regeneration.";

        public override SkillType SkillType => SkillType.Buff;
        public override JobType RequiredJob => JobType.Gunslinger;
        public override int RequiredLevel => 62;
        public override int SkillPointCost => 1;
        public override int MaxRank => 10;
        public override string IconTexture => $"{RpgConstants.ICON_PATH}Skills/GunTempo";
        
        // 만렙 시 쿨 = 지속
        public override float CooldownSeconds => 30f - (CurrentRank * 1f); // 30 -> 20초
        public override int ResourceCost => 25;
        public override ResourceType ResourceType => ResourceType.Stamina;

        private static readonly int[] DURATION_SECONDS = { 12, 13, 14, 15, 16, 16, 17, 18, 19, 20 };
        private static readonly float[] ATTACK_SPEED_BONUS = { 0.10f, 0.12f, 0.14f, 0.16f, 0.18f, 0.20f, 0.23f, 0.26f, 0.30f, 0.35f };

        protected override void OnActivate(Player player)
        {
            int rank = System.Math.Max(1, CurrentRank);
            int duration = DURATION_SECONDS[rank - 1] * 60;
            float attackSpeed = ATTACK_SPEED_BONUS[rank - 1];
            
            player.AddBuff(BuffID.RapidHealing, duration);

            var rpgPlayer = player.GetModPlayer<Players.RpgPlayer>();
            rpgPlayer.AddTemporaryRangedAttackSpeed(attackSpeed, duration);
            
            PlayEffects(player);
            ShowMessage(player, "Gun Tempo!", Color.Yellow);
        }

        private void PlayEffects(Player player)
        {
            SoundEngine.PlaySound(SoundID.Item6, player.position);
            
            for (int i = 0; i < 20; i++)
            {
                Dust dust = Dust.NewDustDirect(player.position, player.width, player.height,
                    DustID.Torch, Main.rand.NextFloat(-2f, 2f), Main.rand.NextFloat(-2f, 0f), 150, Color.Orange, 1f);
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
