using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Rpg.Common.Base;

namespace Rpg.Common.Skills.Tier2.Cleric
{
    /// <summary>
    /// Sanctuary - Cleric's defensive zone skill.
    /// </summary>
    public class Sanctuary : BaseSkill
    {
        public override string InternalName => "Sanctuary";
        public override string DisplayName => "Sanctuary";
        public override string Description => "Create a holy zone that provides regeneration and protection.";

        public override SkillType SkillType => SkillType.Active;
        public override JobType RequiredJob => JobType.Cleric;
        public override int RequiredLevel => RpgConstants.SECOND_JOB_LEVEL + 10;
        public override int SkillPointCost => 1;
        public override int MaxRank => 5;
        public override string IconTexture => $"{RpgConstants.ICON_PATH}Skills/Sanctuary";
        public override float CooldownSeconds => 25f;
        public override int ResourceCost => 30;
        public override ResourceType ResourceType => ResourceType.Mana;

        private static readonly int[] DURATION_SECONDS = { 10, 12, 14, 16, 20 };

        protected override void OnActivate(Player player)
        {
            int rank = System.Math.Max(1, CurrentRank);
            int duration = DURATION_SECONDS[rank - 1] * 60;

            player.AddBuff(BuffID.Regeneration, duration);
            player.AddBuff(BuffID.Endurance, duration);
            player.AddBuff(BuffID.Ironskin, duration);

            PlayEffects(player);
            ShowMessage(player, "Sanctuary!", Color.Gold);
        }

        private void PlayEffects(Player player)
        {
            SoundEngine.PlaySound(SoundID.Item4, player.position);
            for (int i = 0; i < 40; i++)
            {
                float angle = MathHelper.TwoPi * i / 40f;
                Vector2 offset = new Vector2((float)System.Math.Cos(angle), (float)System.Math.Sin(angle)) * 80f;
                Dust dust = Dust.NewDustDirect(player.Center + offset, 4, 4, DustID.GoldFlame, 0, -0.5f, 100, Color.Gold, 1.5f);
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
