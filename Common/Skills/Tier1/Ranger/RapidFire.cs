using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using RpgMod.Common.Base;

namespace RpgMod.Common.Skills.Tier1.Ranger
{
    /// <summary>
    /// Rapid Fire - Ranger's attack speed buff.
    /// Temporarily increases ranged attack speed and grants Swiftness.
    /// </summary>
    public class RapidFire : BaseSkill
    {
        public override string InternalName => "RapidFire";
        public override string DisplayName => "Rapid Fire";
        public override string Description => "Increase ranged attack speed and movement speed for a short time.";

        public override SkillType SkillType => SkillType.Active;
        public override JobType RequiredJob => JobType.Ranger;
        public override int RequiredLevel => RpgConstants.FIRST_JOB_LEVEL;
        public override int SkillPointCost => 1;
        public override int MaxRank => 5;
        public override string IconTexture => $"{RpgConstants.ICON_PATH}Skills/RapidFire";
        public override float CooldownSeconds => 20f;
        public override int ResourceCost => 20;
        public override ResourceType ResourceType => ResourceType.Stamina;

        // Scaling per rank
        private static readonly int[] DURATION_SECONDS = { 8, 10, 12, 14, 18 };
        private static readonly float[] ATTACK_SPEED_BONUS = { 0.10f, 0.15f, 0.20f, 0.25f, 0.35f };

        protected override void OnActivate(Player player)
        {
            int rank = System.Math.Max(1, CurrentRank);
            int duration = DURATION_SECONDS[rank - 1] * 60;
            float attackSpeedBonus = ATTACK_SPEED_BONUS[rank - 1];

            // Apply buffs
            player.AddBuff(BuffID.Swiftness, duration);
            player.AddBuff(BuffID.Archery, duration);

            // Apply attack speed via RpgPlayer
            var rpgPlayer = player.GetModPlayer<Players.RpgPlayer>();
            rpgPlayer.AddTemporaryRangedAttackSpeed(attackSpeedBonus, duration);

            PlaySkillEffects(player);
            ShowMessage(player, $"Rapid Fire! (+{(int)(attackSpeedBonus * 100)}% Speed)", Color.LightGreen);
        }

        private void PlaySkillEffects(Player player)
        {
            SoundEngine.PlaySound(SoundID.Item5, player.position);

            for (int i = 0; i < 20; i++)
            {
                Vector2 velocity = new Vector2(
                    Main.rand.NextFloat(-2f, 2f),
                    Main.rand.NextFloat(-4f, -1f)
                );

                Dust dust = Dust.NewDustDirect(
                    player.position,
                    player.width,
                    player.height,
                    DustID.IceTorch,
                    velocity.X,
                    velocity.Y,
                    100,
                    Color.LightBlue,
                    1.2f
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
