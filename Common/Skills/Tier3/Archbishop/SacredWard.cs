using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using RpgMod.Common.Base;

namespace RpgMod.Common.Skills.Tier3.Archbishop
{
    /// <summary>
    /// Sacred Ward - Archbishop's protection skill.
    /// </summary>
    public class SacredWard : BaseSkill
    {
        public override string InternalName => "SacredWard";
        public override string DisplayName => "Sacred Ward";
        public override string Description => "Protect yourself with regeneration, defense, and mana regeneration.";

        public override SkillType SkillType => SkillType.Active;
        public override JobType RequiredJob => JobType.Archbishop;
        public override int RequiredLevel => RpgConstants.THIRD_JOB_LEVEL + 10;
        public override int SkillPointCost => 2;
        public override int MaxRank => 5;
        public override string IconTexture => $"{RpgConstants.ICON_PATH}Skills/SacredWard";
        public override float CooldownSeconds => 30f;
        public override int ResourceCost => 40;
        public override ResourceType ResourceType => ResourceType.Mana;

        private static readonly int[] DURATION_SECONDS = { 15, 18, 22, 26, 35 };

        protected override void OnActivate(Player player)
        {
            int rank = System.Math.Max(1, CurrentRank);
            int duration = DURATION_SECONDS[rank - 1] * 60;

            player.AddBuff(BuffID.Regeneration, duration);
            player.AddBuff(BuffID.Endurance, duration);
            player.AddBuff(BuffID.Ironskin, duration);
            player.AddBuff(BuffID.ManaRegeneration, duration);

            PlayEffects(player);
            ShowMessage(player, "Sacred Ward!", Color.Gold);
        }

        private void PlayEffects(Player player)
        {
            SoundEngine.PlaySound(SoundID.Item4, player.position);
            for (int i = 0; i < 40; i++)
            {
                float angle = MathHelper.TwoPi * i / 40f;
                Vector2 offset = new Vector2((float)System.Math.Cos(angle), (float)System.Math.Sin(angle)) * 70f;
                Dust dust = Dust.NewDustDirect(player.Center + offset, 4, 4, DustID.GoldFlame, 0, -1.5f, 100, Color.Gold, 2f);
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
