using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Rpg.Common.Base;

namespace Rpg.Common.Skills.Tier1.Summoner
{
    /// <summary>
    /// Summon Ally - Summoner's minion buff skill.
    /// Empowers summons for increased damage.
    /// </summary>
    public class SummonAlly : BaseSkill
    {
        public override string InternalName => "SummonAlly";
        public override string DisplayName => "Summon Ally";
        public override string Description => "Empower your summoning for a short time.";

        public override SkillType SkillType => SkillType.Active;
        public override JobType RequiredJob => JobType.Summoner;
        public override int RequiredLevel => RpgConstants.FIRST_JOB_LEVEL;
        public override int SkillPointCost => 1;
        public override int MaxRank => 5;
        public override string IconTexture => $"{RpgConstants.ICON_PATH}Skills/SummonAlly";
        public override float CooldownSeconds => 20f;
        public override int ResourceCost => 20;
        public override ResourceType ResourceType => ResourceType.Mana;

        // Scaling per rank
        private static readonly int[] DURATION_SECONDS = { 10, 12, 14, 16, 20 };

        protected override void OnActivate(Player player)
        {
            int rank = System.Math.Max(1, CurrentRank);
            int duration = DURATION_SECONDS[rank - 1] * 60;

            // Apply summoning buffs
            player.AddBuff(BuffID.Summoning, duration);
            player.AddBuff(BuffID.Bewitched, duration);

            PlaySkillEffects(player);
            ShowMessage(player, "Summons Empowered!", Color.MediumPurple);
        }

        private void PlaySkillEffects(Player player)
        {
            SoundEngine.PlaySound(SoundID.Item44, player.position);

            // Summoning circle effect
            for (int i = 0; i < 30; i++)
            {
                float angle = MathHelper.TwoPi * i / 30f;
                Vector2 offset = new Vector2(
                    (float)System.Math.Cos(angle) * 60f,
                    (float)System.Math.Sin(angle) * 60f
                );

                Dust dust = Dust.NewDustDirect(
                    player.Center + offset,
                    4,
                    4,
                    DustID.Shadowflame,
                    0,
                    0,
                    100,
                    Color.Purple,
                    1.5f
                );
                dust.noGravity = true;
                dust.velocity = offset.SafeNormalize(Vector2.Zero) * 2f;
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
