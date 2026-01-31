using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using RpgMod.Common.Base;

namespace RpgMod.Common.Skills.Tier2.Berserker
{
    /// <summary>
    /// Bloodlust - Berserker's sustain skill.
    /// Recover health through battle instinct.
    /// </summary>
    public class Bloodlust : BaseSkill
    {
        public override string InternalName => "Bloodlust";
        public override string DisplayName => "Bloodlust";
        public override string Description => "Recover health through battle instinct.";

        public override SkillType SkillType => SkillType.Active;
        public override JobType RequiredJob => JobType.Berserker;
        public override int RequiredLevel => RpgConstants.SECOND_JOB_LEVEL + 10;
        public override int SkillPointCost => 1;
        public override int MaxRank => 5;
        public override string IconTexture => $"{RpgConstants.ICON_PATH}Skills/Bloodlust";
        public override float CooldownSeconds => 25f;
        public override int ResourceCost => 20;
        public override ResourceType ResourceType => ResourceType.Stamina;

        // Scaling per rank
        private static readonly int[] HEAL_AMOUNT = { 30, 45, 60, 80, 100 };
        private static readonly float[] HEAL_PERCENT = { 0.08f, 0.10f, 0.12f, 0.15f, 0.20f };
        private static readonly int[] REGEN_DURATION = { 6, 7, 8, 9, 12 };

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
            ShowMessage(player, $"+{totalHeal} HP", Color.LimeGreen);
        }

        private void PlaySkillEffects(Player player)
        {
            SoundEngine.PlaySound(SoundID.Item3, player.position);

            for (int i = 0; i < 25; i++)
            {
                Vector2 velocity = new Vector2(
                    Main.rand.NextFloat(-3f, 3f),
                    Main.rand.NextFloat(-4f, 0f)
                );

                Dust dust = Dust.NewDustDirect(
                    player.position,
                    player.width,
                    player.height,
                    DustID.LifeDrain,
                    velocity.X,
                    velocity.Y,
                    100,
                    default,
                    1.5f
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
