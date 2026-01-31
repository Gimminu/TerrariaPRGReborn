using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using RpgMod.Common.Base;

namespace RpgMod.Common.Skills.Tier2.Knight
{
    /// <summary>
    /// Taunt - Knight's aggro skill.
    /// Forces enemies to focus on you and hardens defenses.
    /// </summary>
    public class Taunt : BaseSkill
    {
        public override string InternalName => "Taunt";
        public override string DisplayName => "Taunt";
        public override string Description => "Draw enemy attention and harden yourself with Thorns.";

        public override SkillType SkillType => SkillType.Active;
        public override JobType RequiredJob => JobType.Knight;
        public override int RequiredLevel => RpgConstants.SECOND_JOB_LEVEL + 10;
        public override int SkillPointCost => 1;
        public override int MaxRank => 5;
        public override string IconTexture => $"{RpgConstants.ICON_PATH}Skills/Taunt";
        public override float CooldownSeconds => 18f;
        public override int ResourceCost => 15;
        public override ResourceType ResourceType => ResourceType.Stamina;

        // Scaling per rank
        private static readonly int[] DURATION_SECONDS = { 8, 10, 12, 14, 18 };
        private static readonly float[] AGGRO_RANGE = { 200f, 250f, 300f, 350f, 400f };

        protected override void OnActivate(Player player)
        {
            int rank = System.Math.Max(1, CurrentRank);
            int duration = DURATION_SECONDS[rank - 1] * 60;
            float aggroRange = AGGRO_RANGE[rank - 1];

            // Apply buffs
            player.AddBuff(BuffID.Thorns, duration);
            player.AddBuff(BuffID.Endurance, duration);

            // Increase aggro for nearby enemies
            player.aggro += 400;

            // Pull enemy aggro towards player
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC npc = Main.npc[i];
                if (!npc.active || npc.friendly || npc.lifeMax <= 5)
                    continue;

                float distance = Vector2.Distance(npc.Center, player.Center);
                if (distance > aggroRange)
                    continue;

                // Make enemies face player
                if (npc.Center.X > player.Center.X)
                    npc.direction = -1;
                else
                    npc.direction = 1;

                // Slight pull towards player
                Vector2 direction = (player.Center - npc.Center).SafeNormalize(Vector2.Zero);
                npc.velocity += direction * 0.5f;
            }

            PlaySkillEffects(player);
            ShowMessage(player, "TAUNT!", Color.Red);
        }

        private void PlaySkillEffects(Player player)
        {
            SoundEngine.PlaySound(SoundID.Roar, player.position);

            // Shockwave effect
            for (int i = 0; i < 30; i++)
            {
                float angle = MathHelper.TwoPi * i / 30f;
                Vector2 direction = new Vector2(
                    (float)System.Math.Cos(angle),
                    (float)System.Math.Sin(angle)
                );

                Dust dust = Dust.NewDustDirect(
                    player.Center,
                    4,
                    4,
                    DustID.Torch,
                    direction.X * 5f,
                    direction.Y * 5f,
                    100,
                    Color.OrangeRed,
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
