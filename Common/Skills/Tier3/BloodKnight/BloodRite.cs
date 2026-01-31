using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using RpgMod.Common.Base;

namespace RpgMod.Common.Skills.Tier3.BloodKnight
{
    /// <summary>
    /// Blood Rite - Blood Knight's signature skill.
    /// Sacrifice HP for massive power boost.
    /// </summary>
    public class BloodRite : BaseSkill
    {
        public override string InternalName => "BloodRite";
        public override string DisplayName => "Blood Rite";
        public override string Description => "Sacrifice health to gain massive damage and lifesteal.";

        public override SkillType SkillType => SkillType.Active;
        public override JobType RequiredJob => JobType.BloodKnight;
        public override int RequiredLevel => RpgConstants.THIRD_JOB_LEVEL;
        public override int SkillPointCost => 1;
        public override int MaxRank => 5;
        public override string IconTexture => $"{RpgConstants.ICON_PATH}Skills/BloodRite";
        public override float CooldownSeconds => 30f;
        public override int ResourceCost => 0; // Uses HP instead
        public override ResourceType ResourceType => ResourceType.None;

        // Scaling per rank
        private static readonly float[] HP_COST_PERCENT = { 0.15f, 0.15f, 0.12f, 0.12f, 0.10f };
        private static readonly int[] DURATION_SECONDS = { 10, 12, 14, 16, 20 };
        private static readonly float[] DAMAGE_BONUS = { 0.35f, 0.45f, 0.55f, 0.65f, 0.80f };
        private static readonly float[] LIFESTEAL = { 0.05f, 0.07f, 0.09f, 0.11f, 0.15f };

        public override bool CanUse(Player player)
        {
            if (!base.CanUse(player))
                return false;

            int rank = Math.Max(1, CurrentRank);
            int hpCost = (int)(player.statLifeMax2 * HP_COST_PERCENT[rank - 1]);

            if (player.statLife <= hpCost + 50)
            {
                ShowMessage(player, "Not enough HP!", Color.Red);
                return false;
            }

            return true;
        }

        protected override void OnActivate(Player player)
        {
            int rank = Math.Max(1, CurrentRank);
            float hpCostPercent = HP_COST_PERCENT[rank - 1];
            int duration = DURATION_SECONDS[rank - 1] * 60;
            float damageBonus = DAMAGE_BONUS[rank - 1];
            float lifesteal = LIFESTEAL[rank - 1];

            // Consume HP
            int hpCost = (int)(player.statLifeMax2 * hpCostPercent);
            player.statLife -= hpCost;

            // Apply buffs
            player.AddBuff(BuffID.Wrath, duration);
            player.AddBuff(BuffID.Rage, duration);

            // Apply damage and lifesteal via RpgPlayer
            var rpgPlayer = player.GetModPlayer<Players.RpgPlayer>();
            rpgPlayer.AddTemporaryDamage(damageBonus, duration);
            rpgPlayer.AddTemporaryLifesteal(lifesteal, duration);

            PlaySkillEffects(player);
            ShowMessage(player, $"BLOOD RITE! (+{(int)(damageBonus * 100)}% DMG)", Color.DarkRed);
        }

        private void PlaySkillEffects(Player player)
        {
            SoundEngine.PlaySound(SoundID.Item119, player.position);

            // Blood explosion
            for (int i = 0; i < 50; i++)
            {
                Vector2 velocity = Main.rand.NextVector2Circular(8f, 8f);

                Dust dust = Dust.NewDustDirect(
                    player.position,
                    player.width,
                    player.height,
                    DustID.Blood,
                    velocity.X,
                    velocity.Y,
                    100,
                    default,
                    2.5f
                );
                dust.noGravity = true;
            }

            // Dark aura
            for (int i = 0; i < 30; i++)
            {
                float angle = MathHelper.TwoPi * i / 30f;
                Vector2 offset = new Vector2(
                    (float)Math.Cos(angle) * 60f,
                    (float)Math.Sin(angle) * 60f
                );

                Dust dust = Dust.NewDustDirect(
                    player.Center + offset,
                    4,
                    4,
                    DustID.LifeDrain,
                    0,
                    0,
                    100,
                    Color.DarkRed,
                    1.5f
                );
                dust.noGravity = true;
            }
        }

        private void ShowMessage(Player player, string text, Color color)
        {
            if (Main.netMode != NetmodeID.Server)
            {
                CombatText.NewText(player.Hitbox, color, text, true, false);
            }
        }
    }
}
