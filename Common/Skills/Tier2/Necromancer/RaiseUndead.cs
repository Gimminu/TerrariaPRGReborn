using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Rpg.Common.Base;

namespace Rpg.Common.Skills.Tier2.Necromancer
{
    /// <summary>
    /// Raise Undead - Necromancer's summoning enhancement.
    /// Empowers undead minions with increased damage.
    /// </summary>
    public class RaiseUndead : BaseSkill
    {
        public override string InternalName => "RaiseUndead";
        public override string DisplayName => "Raise Undead";
        public override string Description => "Call undead minions to battle with enhanced power.";

        public override SkillType SkillType => SkillType.Active;
        public override JobType RequiredJob => JobType.Necromancer;
        public override int RequiredLevel => RpgConstants.SECOND_JOB_LEVEL;
        public override int SkillPointCost => 1;
        public override int MaxRank => 5;
        public override string IconTexture => $"{RpgConstants.ICON_PATH}Skills/RaiseUndead";
        public override float CooldownSeconds => 20f;
        public override int ResourceCost => 25;
        public override ResourceType ResourceType => ResourceType.Mana;

        // Scaling per rank
        private static readonly int[] DURATION_SECONDS = { 10, 12, 14, 16, 20 };
        private static readonly float[] SUMMON_DAMAGE_BONUS = { 0.20f, 0.25f, 0.30f, 0.35f, 0.45f };

        protected override void OnActivate(Player player)
        {
            int rank = System.Math.Max(1, CurrentRank);
            int duration = DURATION_SECONDS[rank - 1] * 60;
            float damageBonus = SUMMON_DAMAGE_BONUS[rank - 1];

            // Apply buffs
            player.AddBuff(BuffID.Summoning, duration);
            player.AddBuff(BuffID.Bewitched, duration);

            // Apply summon damage via RpgPlayer
            var rpgPlayer = player.GetModPlayer<Players.RpgPlayer>();
            rpgPlayer.AddTemporarySummonDamage(damageBonus, duration);

            PlaySkillEffects(player);
            ShowMessage(player, $"Rise! (+{(int)(damageBonus * 100)}% Summon DMG)", Color.DarkGray);
        }

        private void PlaySkillEffects(Player player)
        {
            SoundEngine.PlaySound(SoundID.NPCDeath2 with { Pitch = -0.5f }, player.position);

            // Dark summoning effect
            for (int i = 0; i < 40; i++)
            {
                Vector2 velocity = new Vector2(
                    Main.rand.NextFloat(-4f, 4f),
                    Main.rand.NextFloat(-5f, 0f)
                );

                Dust dust = Dust.NewDustDirect(
                    player.position - new Vector2(30, 0),
                    player.width + 60,
                    player.height,
                    DustID.Shadowflame,
                    velocity.X,
                    velocity.Y,
                    100,
                    Color.Purple,
                    1.8f
                );
                dust.noGravity = true;
            }

            // Ground circle
            for (int i = 0; i < 30; i++)
            {
                float angle = MathHelper.TwoPi * i / 30f;
                Vector2 offset = new Vector2(
                    (float)System.Math.Cos(angle) * 50f,
                    20f
                );

                Dust dust = Dust.NewDustDirect(
                    player.Center + offset,
                    4,
                    4,
                    DustID.Wraith,
                    0,
                    -2f,
                    100,
                    default,
                    1.3f
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
