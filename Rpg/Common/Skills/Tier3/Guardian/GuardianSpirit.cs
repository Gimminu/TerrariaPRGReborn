using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Rpg.Common.Base;

namespace Rpg.Common.Skills.Tier3.Guardian
{
    /// <summary>
    /// Guardian Spirit - Guardian's ultimate protection skill.
    /// </summary>
    public class GuardianSpirit : BaseSkill
    {
        public override string InternalName => "GuardianSpiritTier3";
        public override string DisplayName => "Guardian Spirit";
        public override string Description => "Summon a guardian spirit that provides immunity to knockback and damage reduction.";

        public override SkillType SkillType => SkillType.Active;
        public override JobType RequiredJob => JobType.Guardian;
        public override int RequiredLevel => RpgConstants.THIRD_JOB_LEVEL + 10;
        public override int SkillPointCost => 2;
        public override int MaxRank => 5;
        public override string IconTexture => $"{RpgConstants.ICON_PATH}Skills/GuardianSpirit";
        public override float CooldownSeconds => 35f;
        public override int ResourceCost => 40;
        public override ResourceType ResourceType => ResourceType.Mana;

        private static readonly int[] DURATION_SECONDS = { 12, 15, 18, 22, 30 };

        protected override void OnActivate(Player player)
        {
            int rank = System.Math.Max(1, CurrentRank);
            int duration = DURATION_SECONDS[rank - 1] * 60;

            player.AddBuff(BuffID.Endurance, duration);
            player.AddBuff(BuffID.Ironskin, duration);
            player.AddBuff(BuffID.Regeneration, duration);

            PlayEffects(player);
            ShowMessage(player, "Guardian Spirit!", Color.Gold);
        }

        private void PlayEffects(Player player)
        {
            SoundEngine.PlaySound(SoundID.Item4, player.position);
            for (int i = 0; i < 50; i++)
            {
                float angle = MathHelper.TwoPi * i / 50f;
                Vector2 offset = new Vector2((float)System.Math.Cos(angle), (float)System.Math.Sin(angle)) * 60f;
                Dust dust = Dust.NewDustDirect(player.Center + offset, 4, 4, DustID.GoldFlame, 0, -1f, 100, Color.Gold, 2f);
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
