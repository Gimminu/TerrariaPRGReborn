using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using RpgMod.Common.Base;

namespace RpgMod.Common.Skills.Tier1.Warrior
{
    /// <summary>
    /// Fortify - Warrior's defensive buff skill.
    /// Grants Ironskin and Endurance for a short time.
    /// </summary>
    public class Fortify : BaseSkill
    {
        public override string InternalName => "Fortify";
        public override string DisplayName => "Fortify";
        public override string Description => "Boost defense for a short time with Ironskin and Endurance effects.";

        public override SkillType SkillType => SkillType.Active;
        public override JobType RequiredJob => JobType.Warrior;
        public override int RequiredLevel => RpgConstants.FIRST_JOB_LEVEL + 5;
        public override int SkillPointCost => 1;
        public override int MaxRank => 5;
        public override string IconTexture => $"{RpgConstants.ICON_PATH}Skills/Fortify";
        public override float CooldownSeconds => 20f;
        public override int ResourceCost => 20;
        public override ResourceType ResourceType => ResourceType.Stamina;

        // Scaling per rank
        private static readonly int[] DURATION_SECONDS = { 10, 12, 14, 16, 20 };
        private static readonly int[] DEFENSE_BONUS = { 4, 6, 8, 10, 14 };

        protected override void OnActivate(Player player)
        {
            int rank = System.Math.Max(1, CurrentRank);
            int duration = DURATION_SECONDS[rank - 1] * 60;
            int defenseBonus = DEFENSE_BONUS[rank - 1];

            // Apply buffs
            player.AddBuff(BuffID.Ironskin, duration);
            player.AddBuff(BuffID.Endurance, duration);

            // Additional flat defense boost via RpgPlayer temporary buff
            var rpgPlayer = player.GetModPlayer<Players.RpgPlayer>();
            rpgPlayer.AddTemporaryDefense(defenseBonus, duration);

            PlaySkillEffects(player);
            ShowMessage(player, $"Fortify! (+{defenseBonus} DEF)", Color.SteelBlue);
        }

        private void PlaySkillEffects(Player player)
        {
            SoundEngine.PlaySound(SoundID.Item4, player.position);

            // Shield barrier effect
            for (int i = 0; i < 30; i++)
            {
                float angle = MathHelper.TwoPi * i / 30f;
                Vector2 offset = new Vector2(
                    (float)System.Math.Cos(angle) * 40f,
                    (float)System.Math.Sin(angle) * 40f
                );

                Dust dust = Dust.NewDustDirect(
                    player.Center + offset,
                    4,
                    4,
                    DustID.Electric,
                    0,
                    0,
                    100,
                    Color.LightBlue,
                    1.2f
                );
                dust.noGravity = true;
                dust.velocity = offset.SafeNormalize(Vector2.Zero) * 0.5f;
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
