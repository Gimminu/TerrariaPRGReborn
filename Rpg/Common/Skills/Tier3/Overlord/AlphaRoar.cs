using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Rpg.Common.Base;
using Rpg.Common.Players;

namespace Rpg.Common.Skills.Tier3.Overlord
{
    /// <summary>
    /// Alpha Roar - Overlord's battle cry that boosts all summons.
    /// </summary>
    public class AlphaRoar : BaseSkill
    {
        public override string InternalName => "AlphaRoar";
        public override string DisplayName => "Alpha Roar";
        public override string Description => "Let out a mighty roar that empowers all your minions.";

        public override SkillType SkillType => SkillType.Active;
        public override JobType RequiredJob => JobType.Overlord;
        public override int RequiredLevel => RpgConstants.THIRD_JOB_LEVEL;
        public override int SkillPointCost => 2;
        public override int MaxRank => 5;
        public override string IconTexture => $"{RpgConstants.ICON_PATH}Skills/AlphaRoar";
        public override float CooldownSeconds => 25f;
        public override int ResourceCost => 35;
        public override ResourceType ResourceType => ResourceType.Mana;

        private static readonly int[] DURATION_SECONDS = { 15, 18, 22, 26, 35 };
        private static readonly float[] SUMMON_BONUS = { 0.25f, 0.35f, 0.45f, 0.58f, 0.75f };

        protected override void OnActivate(Player player)
        {
            int rank = System.Math.Max(1, CurrentRank);
            int duration = DURATION_SECONDS[rank - 1] * 60;
            float bonus = SUMMON_BONUS[rank - 1];

            var rpgPlayer = player.GetModPlayer<RpgPlayer>();
            rpgPlayer.AddTemporarySummonDamage(bonus, duration);

            player.AddBuff(BuffID.Wrath, duration);
            player.AddBuff(BuffID.Rage, duration);

            PlayEffects(player);
            ShowMessage(player, "ALPHA ROAR!", Color.OrangeRed);
        }

        private void PlayEffects(Player player)
        {
            SoundEngine.PlaySound(SoundID.Roar, player.position);
            for (int i = 0; i < 40; i++)
            {
                float angle = MathHelper.TwoPi * i / 40f;
                Vector2 dir = new Vector2((float)System.Math.Cos(angle), (float)System.Math.Sin(angle));
                Dust dust = Dust.NewDustDirect(player.Center + dir * 50f, 6, 6,
                    DustID.Torch, dir.X * 5f, dir.Y * 5f, 100, Color.Orange, 2f);
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
