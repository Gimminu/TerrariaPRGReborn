using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using RpgMod.Common.Base;

namespace RpgMod.Common.Skills.Tier2.DeathKnight
{
    /// <summary>
    /// Unholy Aura - Death Knight's dark buff.
    /// </summary>
    public class UnholyAura : BaseSkill
    {
        public override string InternalName => "UnholyAura";
        public override string DisplayName => "Unholy Aura";
        public override string Description => "Empower yourself with dark energy, gaining Thorns and Rage.";

        public override SkillType SkillType => SkillType.Active;
        public override JobType RequiredJob => JobType.DeathKnight;
        public override int RequiredLevel => RpgConstants.SECOND_JOB_LEVEL + 10;
        public override int SkillPointCost => 1;
        public override int MaxRank => 5;
        public override string IconTexture => $"{RpgConstants.ICON_PATH}Skills/UnholyAura";
        public override float CooldownSeconds => 20f;
        public override int ResourceCost => 20;
        public override ResourceType ResourceType => ResourceType.Mana;

        private static readonly int[] DURATION_SECONDS = { 10, 12, 14, 16, 20 };

        protected override void OnActivate(Player player)
        {
            int rank = System.Math.Max(1, CurrentRank);
            int duration = DURATION_SECONDS[rank - 1] * 60;

            player.AddBuff(BuffID.Thorns, duration);
            player.AddBuff(BuffID.Rage, duration);

            PlayEffects(player);
            ShowMessage(player, "Unholy Aura!", Color.DarkMagenta);
        }

        private void PlayEffects(Player player)
        {
            SoundEngine.PlaySound(SoundID.Item119, player.position);
            for (int i = 0; i < 25; i++)
            {
                Dust dust = Dust.NewDustDirect(player.position, player.width, player.height,
                    DustID.Shadowflame, Main.rand.NextFloat(-4f, 4f), Main.rand.NextFloat(-4f, 4f), 100, Color.Purple, 1.5f);
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
