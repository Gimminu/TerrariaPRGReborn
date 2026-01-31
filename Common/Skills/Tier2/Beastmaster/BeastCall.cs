using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using RpgMod.Common.Base;

namespace RpgMod.Common.Skills.Tier2.Beastmaster
{
    /// <summary>
    /// Beast Call - Beastmaster's minion enhancement skill.
    /// </summary>
    public class BeastCall : BaseSkill
    {
        public override string InternalName => "BeastCall";
        public override string DisplayName => "Beast Call";
        public override string Description => "Empower your summons with ferocity.";

        public override SkillType SkillType => SkillType.Active;
        public override JobType RequiredJob => JobType.Beastmaster;
        public override int RequiredLevel => RpgConstants.SECOND_JOB_LEVEL;
        public override int SkillPointCost => 1;
        public override int MaxRank => 5;
        public override string IconTexture => $"{RpgConstants.ICON_PATH}Skills/BeastCall";
        public override float CooldownSeconds => 20f;
        public override int ResourceCost => 25;
        public override ResourceType ResourceType => ResourceType.Mana;

        private static readonly int[] DURATION_SECONDS = { 12, 15, 18, 22, 28 };

        protected override void OnActivate(Player player)
        {
            int rank = System.Math.Max(1, CurrentRank);
            int duration = DURATION_SECONDS[rank - 1] * 60;

            player.AddBuff(BuffID.Wrath, duration);
            player.AddBuff(BuffID.Endurance, duration);
            player.AddBuff(BuffID.Summoning, duration);

            PlayEffects(player);
            ShowMessage(player, "Beasts Empowered!", Color.Orange);
        }

        private void PlayEffects(Player player)
        {
            SoundEngine.PlaySound(SoundID.NPCDeath5, player.position);
            for (int i = 0; i < 25; i++)
            {
                Dust dust = Dust.NewDustDirect(player.position, player.width, player.height,
                    DustID.Terra, Main.rand.NextFloat(-4f, 4f), Main.rand.NextFloat(-4f, 4f), 100, Color.Green, 1.5f);
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
