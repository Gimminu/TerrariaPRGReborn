using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Rpg.Common.Base;

namespace Rpg.Common.Skills.Tier2.Paladin
{
    /// <summary>
    /// Lay on Hands - Paladin's powerful instant heal.
    /// </summary>
    public class LayOnHands : BaseSkill
    {
        public override string InternalName => "LayOnHands";
        public override string DisplayName => "Lay on Hands";
        public override string Description => "Restore a large amount of health instantly with holy power.";

        public override SkillType SkillType => SkillType.Active;
        public override JobType RequiredJob => JobType.Paladin;
        public override int RequiredLevel => RpgConstants.SECOND_JOB_LEVEL;
        public override int SkillPointCost => 1;
        public override int MaxRank => 5;
        public override string IconTexture => $"{RpgConstants.ICON_PATH}Skills/LayOnHands";
        public override float CooldownSeconds => 25f;
        public override int ResourceCost => 25;
        public override ResourceType ResourceType => ResourceType.Mana;

        private static readonly int[] HEAL_AMOUNT = { 50, 70, 90, 120, 160 };
        private static readonly float[] HEAL_PERCENT = { 0.12f, 0.15f, 0.18f, 0.22f, 0.28f };

        protected override void OnActivate(Player player)
        {
            int rank = Math.Max(1, CurrentRank);
            int heal = HEAL_AMOUNT[rank - 1] + (int)(player.statLifeMax2 * HEAL_PERCENT[rank - 1]);

            player.statLife = Math.Min(player.statLife + heal, player.statLifeMax2);
            player.AddBuff(BuffID.Regeneration, 600);

            PlayEffects(player);
            ShowMessage(player, $"+{heal} HP", Color.Gold);
        }

        private void PlayEffects(Player player)
        {
            SoundEngine.PlaySound(SoundID.Item4, player.position);
            for (int i = 0; i < 30; i++)
            {
                Dust dust = Dust.NewDustDirect(player.position, player.width, player.height,
                    DustID.GoldFlame, Main.rand.NextFloat(-2f, 2f), Main.rand.NextFloat(-3f, 0f), 100, Color.Gold, 1.5f);
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
