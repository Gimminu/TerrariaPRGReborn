using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Rpg.Common.Base;

namespace Rpg.Common.Skills.Tier3.Guardian
{
    /// <summary>
    /// Aegis Wall - Guardian's ultimate defensive skill.
    /// Creates an impenetrable barrier with massive damage reduction.
    /// </summary>
    public class AegisWall : BaseSkill
    {
        public override string InternalName => "AegisWall";
        public override string DisplayName => "Aegis Wall";
        public override string Description => "Raise an impenetrable barrier with massive damage reduction.";

        public override SkillType SkillType => SkillType.Active;
        public override JobType RequiredJob => JobType.Guardian;
        public override int RequiredLevel => RpgConstants.THIRD_JOB_LEVEL;
        public override int SkillPointCost => 1;
        public override int MaxRank => 5;
        public override string IconTexture => $"{RpgConstants.ICON_PATH}Skills/AegisWall";
        public override float CooldownSeconds => 30f;
        public override int ResourceCost => 35;
        public override ResourceType ResourceType => ResourceType.Stamina;

        // Scaling per rank
        private static readonly int[] DURATION_SECONDS = { 10, 12, 14, 16, 20 };
        private static readonly int[] DEFENSE_BONUS = { 30, 40, 50, 60, 80 };
        private static readonly float[] DAMAGE_REDUCTION = { 0.25f, 0.30f, 0.35f, 0.40f, 0.50f };

        protected override void OnActivate(Player player)
        {
            int rank = Math.Max(1, CurrentRank);
            int duration = DURATION_SECONDS[rank - 1] * 60;
            int defenseBonus = DEFENSE_BONUS[rank - 1];
            float damageReduction = DAMAGE_REDUCTION[rank - 1];

            // Apply buffs
            player.AddBuff(BuffID.Endurance, duration);
            player.AddBuff(BuffID.Ironskin, duration);

            // Apply massive defense and damage reduction via RpgPlayer
            var rpgPlayer = player.GetModPlayer<Players.RpgPlayer>();
            rpgPlayer.AddTemporaryDefense(defenseBonus, duration);
            rpgPlayer.AddTemporaryDamageReduction(damageReduction, duration);

            PlaySkillEffects(player);
            ShowMessage(player, $"AEGIS WALL! (+{defenseBonus} DEF, -{(int)(damageReduction * 100)}% DMG)", Color.Gold);
        }

        private void PlaySkillEffects(Player player)
        {
            SoundEngine.PlaySound(SoundID.Item29, player.position);

            // Golden shield barrier
            for (int layer = 0; layer < 3; layer++)
            {
                float radius = 40f + layer * 20f;
                int particleCount = 30 + layer * 10;

                for (int i = 0; i < particleCount; i++)
                {
                    float angle = MathHelper.TwoPi * i / particleCount;
                    Vector2 offset = new Vector2(
                        (float)Math.Cos(angle) * radius,
                        (float)Math.Sin(angle) * radius
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
                        1.8f - layer * 0.3f
                    );
                    dust.noGravity = true;
                    dust.velocity = offset.SafeNormalize(Vector2.Zero) * 0.3f;
                }
            }

            // Central glow
            for (int i = 0; i < 20; i++)
            {
                Dust dust = Dust.NewDustDirect(
                    player.Center,
                    4,
                    4,
                    DustID.SolarFlare,
                    Main.rand.NextFloat(-2f, 2f),
                    Main.rand.NextFloat(-2f, 2f),
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
                CombatText.NewText(player.Hitbox, color, text, true, false);
            }
        }
    }
}
