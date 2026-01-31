using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using RpgMod.Common.Base;

namespace RpgMod.Common.Skills.Novice
{
    /// <summary>
    /// Perseverance - Novice passive skill.
    /// Bonus stats when HP is low.
    /// </summary>
    public class Perseverance : BaseSkill
    {
        public override string InternalName => "Perseverance";
        public override string DisplayName => "Perseverance";
        public override string Description => "Gain bonus damage and defense when below a HP threshold.";

        public override SkillType SkillType => SkillType.Passive;
        public override JobType RequiredJob => JobType.Novice;
        public override int RequiredLevel => 5;
        public override int SkillPointCost => 1;
        public override int MaxRank => 5;
        public override string IconTexture => $"{RpgConstants.ICON_PATH}Skills/Perseverance";
        public override float CooldownSeconds => 0f;
        public override int ResourceCost => 0;
        public override ResourceType ResourceType => ResourceType.None;

        private static readonly float[] HP_THRESHOLD = { 0.40f, 0.40f, 0.45f, 0.45f, 0.50f };
        private static readonly float[] DAMAGE_BONUS = { 0.10f, 0.15f, 0.20f, 0.25f, 0.30f };
        private static readonly int[] DEFENSE_BONUS = { 3, 5, 7, 10, 15 };
        private static readonly float[] REGEN_BONUS = { 0f, 0f, 1f, 2f, 3f };

        private bool lastFrameActive;

        public override void ApplyPassive(Player player)
        {
            if (CurrentRank <= 0)
                return;

            int rank = CurrentRank;
            float hpPercent = (float)player.statLife / player.statLifeMax2;
            float threshold = HP_THRESHOLD[rank - 1];

            bool isActive = hpPercent <= threshold;

            if (isActive)
            {
                float damageBonus = DAMAGE_BONUS[rank - 1];
                int defenseBonus = DEFENSE_BONUS[rank - 1];

                player.GetDamage(DamageClass.Generic) += damageBonus;
                player.statDefense += defenseBonus;

                float regenBonus = REGEN_BONUS[rank - 1];
                if (regenBonus > 0)
                {
                    int regenFrames = (int)(60f / regenBonus);
                    if (Main.GameUpdateCount % regenFrames == 0)
                    {
                        player.statLife = Utils.Clamp(player.statLife + 1, 0, player.statLifeMax2);
                    }
                }

                if (!lastFrameActive)
                {
                    ShowActivationEffect(player);
                    ShowMessage(player, "Perseverance Active!", Color.OrangeRed);
                }

                if (Main.GameUpdateCount % 20 == 0)
                {
                    CreateAuraEffect(player);
                }
            }
            else if (lastFrameActive)
            {
                ShowMessage(player, "Perseverance faded...", Color.Gray);
            }

            lastFrameActive = isActive;
        }

        private void ShowActivationEffect(Player player)
        {
            for (int i = 0; i < 30; i++)
            {
                Vector2 velocity = new Vector2(
                    Main.rand.NextFloat(-5f, 5f),
                    Main.rand.NextFloat(-5f, 5f)
                );

                Dust dust = Dust.NewDustDirect(
                    player.position,
                    player.width,
                    player.height,
                    DustID.Torch,
                    velocity.X,
                    velocity.Y,
                    100,
                    Color.OrangeRed,
                    1.8f
                );
                dust.noGravity = true;
            }

            SoundEngine.PlaySound(SoundID.Item8, player.position);
        }

        private void CreateAuraEffect(Player player)
        {
            for (int i = 0; i < 2; i++)
            {
                float angle = Main.rand.NextFloat(0f, 6.28f);
                float distance = 30f;
                Vector2 offset = new Vector2(
                    (float)System.Math.Cos(angle) * distance,
                    (float)System.Math.Sin(angle) * distance
                );

                Dust dust = Dust.NewDustDirect(
                    player.Center + offset,
                    1,
                    1,
                    DustID.Torch,
                    0,
                    -1f,
                    100,
                    Color.Orange,
                    1.0f
                );
                dust.noGravity = true;
                dust.fadeIn = 0.8f;
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
