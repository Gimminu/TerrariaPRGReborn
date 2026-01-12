using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Rpg.Common.Base;
using Rpg.Common.Players;

namespace Rpg.Common.Skills.Tier3.BloodKnight
{
    /// <summary>
    /// Crimson Frenzy - Blood Knight's damage boost at low health.
    /// </summary>
    public class CrimsonFrenzy : BaseSkill
    {
        public override string InternalName => "CrimsonFrenzy";
        public override string DisplayName => "Crimson Frenzy";
        public override string Description => "Enter a blood frenzy, gaining massive damage at the cost of life drain.";

        public override SkillType SkillType => SkillType.Active;
        public override JobType RequiredJob => JobType.BloodKnight;
        public override int RequiredLevel => RpgConstants.THIRD_JOB_LEVEL + 10;
        public override int SkillPointCost => 2;
        public override int MaxRank => 5;
        public override string IconTexture => $"{RpgConstants.ICON_PATH}Skills/CrimsonFrenzy";
        public override float CooldownSeconds => 30f;
        public override int ResourceCost => 50;
        public override ResourceType ResourceType => ResourceType.Life;

        private static readonly int[] DURATION_SECONDS = { 10, 12, 14, 16, 20 };
        private static readonly float[] DAMAGE_BONUS = { 0.25f, 0.32f, 0.40f, 0.50f, 0.65f };
        private static readonly float[] LIFESTEAL = { 0.05f, 0.07f, 0.09f, 0.11f, 0.15f };

        protected override void OnActivate(Player player)
        {
            int rank = System.Math.Max(1, CurrentRank);
            int duration = DURATION_SECONDS[rank - 1] * 60;
            float damageBonus = DAMAGE_BONUS[rank - 1];
            float lifesteal = LIFESTEAL[rank - 1];

            var rpgPlayer = player.GetModPlayer<RpgPlayer>();
            rpgPlayer.AddTemporaryDamage(damageBonus, duration);
            rpgPlayer.AddTemporaryLifesteal(lifesteal, duration);

            player.AddBuff(BuffID.Rage, duration);

            PlayEffects(player);
            ShowMessage(player, "CRIMSON FRENZY!", Color.DarkRed);
        }

        private void PlayEffects(Player player)
        {
            SoundEngine.PlaySound(SoundID.Roar, player.position);
            for (int i = 0; i < 35; i++)
            {
                Dust dust = Dust.NewDustDirect(player.position, player.width, player.height,
                    DustID.Blood, Main.rand.NextFloat(-5f, 5f), Main.rand.NextFloat(-5f, 5f), 100, Color.DarkRed, 2f);
                dust.noGravity = true;
            }
        }

        private void ShowMessage(Player player, string text, Color color)
        {
            if (Main.netMode != NetmodeID.Server)
                CombatText.NewText(player.Hitbox, color, text);
        }
    }
}
