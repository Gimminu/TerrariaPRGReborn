using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using RpgMod.Common.Base;
using RpgMod.Common.Players;

namespace RpgMod.Common.Skills.Tier2.Sorcerer
{
    /// <summary>
    /// Arcane Surge - 마력 급증.
    /// 일시적으로 마법 피해 대폭 증가.
    /// 소서러의 강력한 버프. 쿨 > 지속 (강한 버프).
    /// </summary>
    public class ArcaneSurge : BaseSkill
    {
        public override string InternalName => "ArcaneSurge";
        public override string DisplayName => "Arcane Surge";
        public override string Description => "Surge with arcane power, greatly increasing magic damage and granting Magic Power and Clairvoyance.";

        public override SkillType SkillType => SkillType.Buff;
        public override JobType RequiredJob => JobType.Sorcerer;
        public override int RequiredLevel => 65;
        public override int SkillPointCost => 1;
        public override int MaxRank => 10;
        public override string IconTexture => $"{RpgConstants.ICON_PATH}Skills/ArcaneSurge";
        
        // 강한 버프: 쿨 > 지속
        public override float CooldownSeconds => 35f - (CurrentRank * 1f); // 35 -> 25초
        public override int ResourceCost => 40;
        public override ResourceType ResourceType => ResourceType.Mana;

        private static readonly int[] DURATION_SECONDS = { 8, 9, 10, 10, 11, 11, 12, 12, 13, 15 };
        private static readonly float[] DAMAGE_BONUS = { 0.15f, 0.18f, 0.21f, 0.24f, 0.28f, 0.32f, 0.36f, 0.40f, 0.45f, 0.55f };

        protected override void OnActivate(Player player)
        {
            int rank = System.Math.Max(1, CurrentRank);
            int durationTicks = DURATION_SECONDS[rank - 1] * 60;
            
            player.AddBuff(BuffID.MagicPower, durationTicks);
            player.AddBuff(BuffID.Clairvoyance, durationTicks);
            
            // 추가 피해는 RpgPlayer에서 처리
            var rpg = player.GetModPlayer<RpgPlayer>();
            rpg.AddTemporaryMagicDamage(DAMAGE_BONUS[rank - 1], durationTicks);
            
            PlayEffects(player);
            ShowMessage(player, "Arcane Surge!", Color.MediumPurple);
        }

        private void PlayEffects(Player player)
        {
            SoundEngine.PlaySound(SoundID.Item29, player.position);
            
            for (int i = 0; i < 30; i++)
            {
                Dust dust = Dust.NewDustDirect(player.position, player.width, player.height,
                    DustID.PurpleCrystalShard, Main.rand.NextFloat(-3f, 3f), Main.rand.NextFloat(-3f, 0f), 150, Color.MediumPurple, 1.3f);
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
