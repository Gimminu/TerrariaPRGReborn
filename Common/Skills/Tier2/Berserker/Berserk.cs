using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Rpg.Common.Base;

namespace Rpg.Common.Skills.Tier2.Berserker
{
    /// <summary>
    /// Berserk - Berserker's signature skill.
    /// Unleash fury for massive damage but take more damage.
    /// </summary>
    public class Berserk : BaseSkill
    {
        public override string InternalName => "Berserk";
        public override string DisplayName => "Berserk";
        public override string Description => "Unleash fury for massive power, but take 20% more damage.";

        public override SkillType SkillType => SkillType.Active;
        public override JobType RequiredJob => JobType.Berserker;
        public override int RequiredLevel => RpgConstants.SECOND_JOB_LEVEL;
        public override int SkillPointCost => 1;
        public override int MaxRank => 5;
        public override string IconTexture => $"{RpgConstants.ICON_PATH}Skills/Berserk";
        public override float CooldownSeconds => 25f;
        public override int ResourceCost => 25;
        public override ResourceType ResourceType => ResourceType.Stamina;

        // Scaling per rank
        private static readonly int[] DURATION_SECONDS = { 8, 10, 12, 14, 18 };
        private static readonly float[] DAMAGE_BONUS = { 0.25f, 0.30f, 0.35f, 0.40f, 0.50f };

        protected override void OnActivate(Player player)
        {
            int rank = Math.Max(1, CurrentRank);
            int duration = DURATION_SECONDS[rank - 1] * 60;
            float damageBonus = DAMAGE_BONUS[rank - 1];

            // Apply vanilla buffs
            player.AddBuff(BuffID.Rage, duration);
            player.AddBuff(BuffID.Wrath, duration);

            // Apply damage boost via RpgPlayer
            var rpgPlayer = player.GetModPlayer<Players.RpgPlayer>();
            rpgPlayer.AddTemporaryDamage(damageBonus, duration);
            rpgPlayer.AddTemporaryDamageTaken(0.2f, duration); // Take 20% more damage

            PlaySkillEffects(player);
            ShowMessage(player, $"BERSERK! (+{(int)(damageBonus * 100)}% DMG)", Color.DarkRed);
        }

        private void PlaySkillEffects(Player player)
        {
            SoundEngine.PlaySound(SoundID.Roar with { Pitch = -0.5f }, player.position);

            // Blood rage effect
            for (int i = 0; i < 40; i++)
            {
                Vector2 velocity = new Vector2(
                    Main.rand.NextFloat(-6f, 6f),
                    Main.rand.NextFloat(-6f, 2f)
                );

                Dust dust = Dust.NewDustDirect(
                    player.position,
                    player.width,
                    player.height,
                    DustID.Blood,
                    velocity.X,
                    velocity.Y,
                    100,
                    default,
                    2f
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
