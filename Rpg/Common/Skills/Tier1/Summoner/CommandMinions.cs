using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Rpg.Common.Base;

namespace Rpg.Common.Skills.Tier1.Summoner
{
    /// <summary>
    /// Command Minions - Summoner's minion damage buff.
    /// Temporarily boosts minion damage significantly.
    /// </summary>
    public class CommandMinions : BaseSkill
    {
        public override string InternalName => "CommandMinions";
        public override string DisplayName => "Command Minions";
        public override string Description => "Command your minions to fight harder, boosting their damage.";

        public override SkillType SkillType => SkillType.Active;
        public override JobType RequiredJob => JobType.Summoner;
        public override int RequiredLevel => RpgConstants.FIRST_JOB_LEVEL + 5;
        public override int SkillPointCost => 1;
        public override int MaxRank => 5;
        public override string IconTexture => $"{RpgConstants.ICON_PATH}Skills/CommandMinions";
        public override float CooldownSeconds => 18f;
        public override int ResourceCost => 15;
        public override ResourceType ResourceType => ResourceType.Mana;

        // Scaling per rank
        private static readonly int[] DURATION_SECONDS = { 10, 12, 14, 16, 20 };
        private static readonly float[] DAMAGE_BONUS = { 0.15f, 0.20f, 0.25f, 0.30f, 0.40f };

        protected override void OnActivate(Player player)
        {
            int rank = System.Math.Max(1, CurrentRank);
            int duration = DURATION_SECONDS[rank - 1] * 60;
            float damageBonus = DAMAGE_BONUS[rank - 1];

            // Apply buff
            player.AddBuff(BuffID.Summoning, duration);

            // Apply summon damage via RpgPlayer
            var rpgPlayer = player.GetModPlayer<Players.RpgPlayer>();
            rpgPlayer.AddTemporarySummonDamage(damageBonus, duration);

            PlaySkillEffects(player);
            ShowMessage(player, $"Command! (+{(int)(damageBonus * 100)}% Minion DMG)", Color.MediumPurple);
        }

        private void PlaySkillEffects(Player player)
        {
            SoundEngine.PlaySound(SoundID.Roar, player.position);

            // Command wave effect
            for (int i = 0; i < 20; i++)
            {
                Vector2 velocity = new Vector2(
                    Main.rand.NextFloat(-5f, 5f),
                    Main.rand.NextFloat(-3f, 0f)
                );

                Dust dust = Dust.NewDustDirect(
                    player.position,
                    player.width,
                    player.height,
                    DustID.Shadowflame,
                    velocity.X,
                    velocity.Y,
                    100,
                    Color.Magenta,
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
