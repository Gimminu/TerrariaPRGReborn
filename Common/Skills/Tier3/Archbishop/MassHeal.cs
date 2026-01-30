using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Rpg.Common.Base;

namespace Rpg.Common.Skills.Tier3.Archbishop
{
    /// <summary>
    /// Mass Heal - Archbishop's ultimate healing skill.
    /// Restore health to yourself and nearby allies.
    /// </summary>
    public class MassHeal : BaseSkill
    {
        public override string InternalName => "MassHeal";
        public override string DisplayName => "Mass Heal";
        public override string Description => "Restore health to yourself and all nearby allies.";

        public override SkillType SkillType => SkillType.Active;
        public override JobType RequiredJob => JobType.Archbishop;
        public override int RequiredLevel => RpgConstants.THIRD_JOB_LEVEL;
        public override int SkillPointCost => 1;
        public override int MaxRank => 5;
        public override string IconTexture => $"{RpgConstants.ICON_PATH}Skills/MassHeal";
        public override float CooldownSeconds => 30f;
        public override int ResourceCost => 35;
        public override ResourceType ResourceType => ResourceType.Mana;

        // Scaling per rank
        private static readonly int[] HEAL_AMOUNT = { 60, 80, 100, 130, 170 };
        private static readonly float[] HEAL_PERCENT = { 0.15f, 0.18f, 0.22f, 0.26f, 0.32f };
        private static readonly float[] HEAL_RADIUS = { 200f, 250f, 300f, 350f, 400f };
        private static readonly int[] REGEN_DURATION = { 10, 12, 14, 16, 20 };

        protected override void OnActivate(Player player)
        {
            int rank = Math.Max(1, CurrentRank);
            int healAmount = HEAL_AMOUNT[rank - 1];
            float healPercent = HEAL_PERCENT[rank - 1];
            float healRadius = HEAL_RADIUS[rank - 1];
            int regenDuration = REGEN_DURATION[rank - 1] * 60;

            // Heal self
            int selfHeal = healAmount + (int)(player.statLifeMax2 * healPercent);
            player.statLife = Math.Min(player.statLife + selfHeal, player.statLifeMax2);
            player.AddBuff(BuffID.Regeneration, regenDuration);

            // Heal nearby players (multiplayer)
            int healedPlayers = 1; // Self
            for (int i = 0; i < Main.maxPlayers; i++)
            {
                Player otherPlayer = Main.player[i];
                if (!otherPlayer.active || otherPlayer.dead || otherPlayer.whoAmI == player.whoAmI)
                    continue;

                float distance = Vector2.Distance(otherPlayer.Center, player.Center);
                if (distance > healRadius)
                    continue;

                int otherHeal = healAmount + (int)(otherPlayer.statLifeMax2 * healPercent);
                otherPlayer.statLife = Math.Min(otherPlayer.statLife + otherHeal, otherPlayer.statLifeMax2);
                otherPlayer.AddBuff(BuffID.Regeneration, regenDuration);
                
                CreateHealEffect(otherPlayer);
                healedPlayers++;
            }

            PlaySkillEffects(player, healRadius);
            ShowMessage(player, $"Mass Heal! +{selfHeal} HP ({healedPlayers} healed)", Color.LightGreen);
        }

        private void CreateHealEffect(Player target)
        {
            for (int i = 0; i < 20; i++)
            {
                Vector2 velocity = new Vector2(
                    Main.rand.NextFloat(-2f, 2f),
                    Main.rand.NextFloat(-3f, 0f)
                );

                Dust dust = Dust.NewDustDirect(
                    target.position,
                    target.width,
                    target.height,
                    DustID.HealingPlus,
                    velocity.X,
                    velocity.Y,
                    100,
                    Color.LightGreen,
                    1.5f
                );
                dust.noGravity = true;
            }
        }

        private void PlaySkillEffects(Player player, float radius)
        {
            SoundEngine.PlaySound(SoundID.Item4 with { Pitch = 0.2f }, player.position);

            // Holy expanding ring
            int particleCount = (int)(radius / 5f);
            for (int i = 0; i < particleCount; i++)
            {
                float angle = MathHelper.TwoPi * i / particleCount;
                Vector2 direction = new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle));

                for (int j = 0; j < 3; j++)
                {
                    Vector2 pos = player.Center + direction * (50f + j * 50f);
                    
                    Dust dust = Dust.NewDustDirect(
                        pos,
                        4,
                        4,
                        DustID.GoldFlame,
                        direction.X * 2f,
                        direction.Y * 2f,
                        100,
                        Color.Gold,
                        1.5f
                    );
                    dust.noGravity = true;
                }
            }

            // Center holy pillar
            for (int i = 0; i < 40; i++)
            {
                Dust dust = Dust.NewDustDirect(
                    player.Center + new Vector2(Main.rand.NextFloat(-30f, 30f), Main.rand.NextFloat(-80f, 20f)),
                    4,
                    4,
                    DustID.HealingPlus,
                    0,
                    -2f,
                    100,
                    Color.White,
                    2f
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
