using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Rpg.Common.Base;

namespace Rpg.Common.Skills.Tier2.Cleric
{
    /// <summary>
    /// Healing Prayer - Cleric's primary healing skill.
    /// Heal yourself and apply regeneration.
    /// </summary>
    public class HealingPrayer : BaseSkill
    {
        public override string InternalName => "HealingPrayer";
        public override string DisplayName => "Healing Prayer";
        public override string Description => "Heal yourself with holy light and gain regeneration.";

        public override SkillType SkillType => SkillType.Active;
        public override JobType RequiredJob => JobType.Cleric;
        public override int RequiredLevel => RpgConstants.SECOND_JOB_LEVEL;
        public override int SkillPointCost => 1;
        public override int MaxRank => 5;
        public override string IconTexture => $"{RpgConstants.ICON_PATH}Skills/HealingPrayer";
        public override float CooldownSeconds => 18f;
        public override int ResourceCost => 20;
        public override ResourceType ResourceType => ResourceType.Mana;

        // Scaling per rank
        private static readonly int[] HEAL_AMOUNT = { 40, 55, 70, 90, 120 };
        private static readonly float[] HEAL_PERCENT = { 0.10f, 0.12f, 0.15f, 0.18f, 0.22f };
        private static readonly int[] REGEN_DURATION = { 8, 10, 12, 14, 18 };

        protected override void OnActivate(Player player)
        {
            int rank = Math.Max(1, CurrentRank);
            int healAmount = HEAL_AMOUNT[rank - 1];
            float healPercent = HEAL_PERCENT[rank - 1];
            int regenDuration = REGEN_DURATION[rank - 1] * 60;

            // Calculate total heal
            int percentHeal = (int)(player.statLifeMax2 * healPercent);
            int totalHeal = healAmount + percentHeal;

            player.statLife = Math.Min(player.statLife + totalHeal, player.statLifeMax2);
            player.AddBuff(BuffID.Regeneration, regenDuration);

            PlaySkillEffects(player);
            ShowMessage(player, $"+{totalHeal} HP", Color.LightGreen);
        }

        private void PlaySkillEffects(Player player)
        {
            SoundEngine.PlaySound(SoundID.Item4, player.position);

            // Holy light effect
            for (int i = 0; i < 30; i++)
            {
                Vector2 velocity = new Vector2(
                    Main.rand.NextFloat(-2f, 2f),
                    Main.rand.NextFloat(-4f, 0f)
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
                dust.fadeIn = 1.3f;
            }

            // Holy circle
            for (int i = 0; i < 20; i++)
            {
                float angle = MathHelper.TwoPi * i / 20f;
                Vector2 offset = new Vector2(
                    (float)Math.Cos(angle) * 40f,
                    (float)Math.Sin(angle) * 40f
                );

                Dust dust = Dust.NewDustDirect(
                    player.Center + offset,
                    4,
                    4,
                    DustID.GoldFlame,
                    0,
                    0,
                    100,
                    Color.Gold,
                    1.2f
                );
                dust.noGravity = true;
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
