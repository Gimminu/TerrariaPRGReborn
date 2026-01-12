using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using Rpg.Common.Base;

namespace Rpg.Common.Skills.Novice
{
    /// <summary>
    /// Second Wind - Novice survival skill.
    /// Instant heal + temporary regeneration.
    /// </summary>
    public class SecondWind : BaseSkill
    {
        public override string InternalName => "SecondWind";
        public override string DisplayName => "Second Wind";
        public override string Description => "Instantly heal and gain regeneration for a short time.";

        public override SkillType SkillType => SkillType.Active;
        public override JobType RequiredJob => JobType.Novice;
        public override int RequiredLevel => 3;
        public override int SkillPointCost => 1;
        public override int MaxRank => 5;
        public override string IconTexture => $"{RpgConstants.ICON_PATH}Skills/SecondWind";
        public override float CooldownSeconds => 30f;
        public override int ResourceCost => 40;
        public override ResourceType ResourceType => ResourceType.Stamina;

        private const int UseAnimation = 30;
        private const int UseTime = 30;

        private static readonly int[] INSTANT_HEAL = { 50, 75, 100, 150, 200 };
        private static readonly float[] HEAL_PERCENT = { 0.1f, 0.12f, 0.15f, 0.18f, 0.20f };
        private static readonly int[] REGEN_DURATION = { 5, 6, 7, 8, 10 };

        public override bool CanUse(Player player)
        {
            if (!base.CanUse(player))
                return false;

            if (player.statLife >= player.statLifeMax2)
            {
                ShowMessage(player, "Already at full health!", Color.Orange);
            }

            return true;
        }

        protected override void OnActivate(Player player)
        {
            int rank = Math.Max(1, CurrentRank);
            int instantHeal = INSTANT_HEAL[rank - 1];
            int percentHeal = (int)(player.statLifeMax2 * HEAL_PERCENT[rank - 1]);
            int totalHeal = instantHeal + percentHeal;

            player.statLife = Math.Min(player.statLife + totalHeal, player.statLifeMax2);
            ShowMessage(player, $"+{totalHeal} HP", Color.LimeGreen);

            int buffDuration = REGEN_DURATION[rank - 1] * 60;
            player.AddBuff(BuffID.Regeneration, buffDuration);

            PlaySkillEffects(player);

            player.itemAnimation = UseAnimation;
            player.itemTime = UseTime;
        }

        private void PlaySkillEffects(Player player)
        {
            SoundEngine.PlaySound(SoundID.Item3, player.position);

            for (int i = 0; i < 30; i++)
            {
                Vector2 velocity = new Vector2(
                    Main.rand.NextFloat(-3f, 3f),
                    Main.rand.NextFloat(-3f, 3f)
                );

                Dust dust = Dust.NewDustDirect(
                    player.position,
                    player.width,
                    player.height,
                    DustID.HealingPlus,
                    velocity.X,
                    velocity.Y,
                    100,
                    Color.LightGreen,
                    1.5f
                );
                dust.noGravity = true;
                dust.fadeIn = 1.2f;
            }
        }

        private void ShowMessage(Player player, string text, Color color)
        {
            if (Main.netMode != NetmodeID.Server)
            {
                CombatText.NewText(player.Hitbox, color, text, false, false);
            }
        }
    }
}
