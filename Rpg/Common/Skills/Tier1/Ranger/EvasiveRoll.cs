using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Rpg.Common.Base;

namespace Rpg.Common.Skills.Tier1.Ranger
{
    /// <summary>
    /// Evasive Roll - Ranger's mobility skill.
    /// Quickly dash in a direction with temporary invincibility frames.
    /// </summary>
    public class EvasiveRoll : BaseSkill
    {
        public override string InternalName => "EvasiveRoll";
        public override string DisplayName => "Evasive Roll";
        public override string Description => "Quickly evade incoming danger with a fast roll.";

        public override SkillType SkillType => SkillType.Active;
        public override JobType RequiredJob => JobType.Ranger;
        public override int RequiredLevel => RpgConstants.FIRST_JOB_LEVEL + 5;
        public override int SkillPointCost => 1;
        public override int MaxRank => 5;
        public override string IconTexture => $"{RpgConstants.ICON_PATH}Skills/EvasiveRoll";
        public override float CooldownSeconds => 15f;
        public override int ResourceCost => 15;
        public override ResourceType ResourceType => ResourceType.Stamina;

        // Scaling per rank
        private static readonly float[] DASH_SPEED = { 10f, 12f, 14f, 16f, 20f };
        private static readonly int[] IFRAMES = { 15, 20, 25, 30, 40 };

        protected override void OnActivate(Player player)
        {
            int rank = System.Math.Max(1, CurrentRank);
            float dashSpeed = DASH_SPEED[rank - 1];
            int iframes = IFRAMES[rank - 1];

            // Determine dash direction
            int direction = player.direction;
            if (player.controlLeft) direction = -1;
            else if (player.controlRight) direction = 1;

            // Apply dash velocity
            player.velocity.X = direction * dashSpeed;
            if (player.controlDown)
            {
                player.velocity.Y = 5f;
            }
            else if (player.controlUp)
            {
                player.velocity.Y = -dashSpeed * 0.5f;
            }

            // Grant invincibility frames
            player.immune = true;
            player.immuneTime = iframes;
            player.immuneNoBlink = true;

            // Apply swiftness buff
            player.AddBuff(BuffID.Swiftness, 180); // 3 seconds

            PlaySkillEffects(player, direction);
            ShowMessage(player, "Evasive Roll!", Color.LightCyan);
        }

        private void PlaySkillEffects(Player player, int direction)
        {
            SoundEngine.PlaySound(SoundID.Item24, player.position);

            // Trail effect
            for (int i = 0; i < 15; i++)
            {
                Vector2 dustPos = player.position + new Vector2(-direction * i * 4, Main.rand.NextFloat(-10f, 10f));
                
                Dust dust = Dust.NewDustDirect(
                    dustPos,
                    4,
                    4,
                    DustID.Smoke,
                    -direction * 2f,
                    0f,
                    100,
                    Color.Gray,
                    1.5f
                );
                dust.noGravity = true;
                dust.fadeIn = 0.5f;
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
