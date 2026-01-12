using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Rpg.Common.Base;

namespace Rpg.Common.Skills.Tier1.Mage
{
    /// <summary>
    /// Mana Shield - Mage's defensive skill.
    /// Reduces damage taken and boosts mana regeneration.
    /// </summary>
    public class ManaShield : BaseSkill
    {
        public override string InternalName => "ManaShield";
        public override string DisplayName => "Mana Shield";
        public override string Description => "Create a magical barrier that reduces damage and boosts mana regen.";

        public override SkillType SkillType => SkillType.Active;
        public override JobType RequiredJob => JobType.Mage;
        public override int RequiredLevel => RpgConstants.FIRST_JOB_LEVEL + 5;
        public override int SkillPointCost => 1;
        public override int MaxRank => 5;
        public override string IconTexture => $"{RpgConstants.ICON_PATH}Skills/ManaShield";
        public override float CooldownSeconds => 20f;
        public override int ResourceCost => 25;
        public override ResourceType ResourceType => ResourceType.Mana;

        // Scaling per rank
        private static readonly int[] DURATION_SECONDS = { 10, 12, 14, 16, 20 };

        protected override void OnActivate(Player player)
        {
            int rank = System.Math.Max(1, CurrentRank);
            int duration = DURATION_SECONDS[rank - 1] * 60;

            // Apply buffs
            player.AddBuff(BuffID.Endurance, duration);
            player.AddBuff(BuffID.ManaRegeneration, duration);

            PlaySkillEffects(player);
            ShowMessage(player, "Mana Shield!", Color.Cyan);
        }

        private void PlaySkillEffects(Player player)
        {
            SoundEngine.PlaySound(SoundID.Item8, player.position);

            // Magic shield effect
            for (int i = 0; i < 40; i++)
            {
                float angle = MathHelper.TwoPi * i / 40f;
                Vector2 offset = new Vector2(
                    (float)System.Math.Cos(angle) * 50f,
                    (float)System.Math.Sin(angle) * 50f
                );

                Dust dust = Dust.NewDustDirect(
                    player.Center + offset,
                    4,
                    4,
                    DustID.MagicMirror,
                    0,
                    0,
                    100,
                    Color.Cyan,
                    1.3f
                );
                dust.noGravity = true;
                dust.velocity = Vector2.Zero;
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
