using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Rpg.Common.Base;

namespace Rpg.Common.Skills.Tier2.Paladin
{
    /// <summary>
    /// Divine Bless - Paladin's party buff skill.
    /// </summary>
    public class DivineBless : BaseSkill
    {
        public override string InternalName => "DivineBless";
        public override string DisplayName => "Divine Bless";
        public override string Description => "Bless yourself with holy power, gaining Wrath and Regeneration.";

        public override SkillType SkillType => SkillType.Active;
        public override JobType RequiredJob => JobType.Paladin;
        public override int RequiredLevel => RpgConstants.SECOND_JOB_LEVEL + 10;
        public override int SkillPointCost => 1;
        public override int MaxRank => 5;
        public override string IconTexture => $"{RpgConstants.ICON_PATH}Skills/DivineBless";
        public override float CooldownSeconds => 25f;
        public override int ResourceCost => 20;
        public override ResourceType ResourceType => ResourceType.Mana;

        private static readonly int[] DURATION_SECONDS = { 10, 12, 14, 16, 20 };

        protected override void OnActivate(Player player)
        {
            int rank = System.Math.Max(1, CurrentRank);
            int duration = DURATION_SECONDS[rank - 1] * 60;

            player.AddBuff(BuffID.Wrath, duration);
            player.AddBuff(BuffID.Regeneration, duration);

            PlayEffects(player);
            ShowMessage(player, "Divine Bless!", Color.Gold);
        }

        private void PlayEffects(Player player)
        {
            SoundEngine.PlaySound(SoundID.Item29, player.position);
            for (int i = 0; i < 25; i++)
            {
                float angle = MathHelper.TwoPi * i / 25f;
                Vector2 offset = new Vector2((float)System.Math.Cos(angle), (float)System.Math.Sin(angle)) * 40f;
                Dust dust = Dust.NewDustDirect(player.Center + offset, 4, 4, DustID.GoldFlame, 0, -1f, 100, Color.Gold, 1.3f);
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
