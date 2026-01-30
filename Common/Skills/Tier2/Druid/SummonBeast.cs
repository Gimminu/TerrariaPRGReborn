using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Rpg.Common.Base;

namespace Rpg.Common.Skills.Tier2.Druid
{
    /// <summary>
    /// Summon Beast - Druid's summon enhancement skill.
    /// </summary>
    public class SummonBeast : BaseSkill
    {
        public override string InternalName => "SummonBeast";
        public override string DisplayName => "Summon Beast";
        public override string Description => "Channel nature's power to enhance your summons.";

        public override SkillType SkillType => SkillType.Active;
        public override JobType RequiredJob => JobType.Druid;
        public override int RequiredLevel => RpgConstants.SECOND_JOB_LEVEL;
        public override int SkillPointCost => 1;
        public override int MaxRank => 5;
        public override string IconTexture => $"{RpgConstants.ICON_PATH}Skills/SummonBeast";
        public override float CooldownSeconds => 18f;
        public override int ResourceCost => 22;
        public override ResourceType ResourceType => ResourceType.Mana;

        private static readonly int[] DURATION_SECONDS = { 15, 18, 22, 26, 32 };

        protected override void OnActivate(Player player)
        {
            int rank = System.Math.Max(1, CurrentRank);
            int duration = DURATION_SECONDS[rank - 1] * 60;

            player.AddBuff(BuffID.Wrath, duration);
            player.AddBuff(BuffID.Regeneration, duration);

            PlayEffects(player);
            ShowMessage(player, "Nature's Fury!", Color.Green);
        }

        private void PlayEffects(Player player)
        {
            SoundEngine.PlaySound(SoundID.Grass, player.position);
            for (int i = 0; i < 25; i++)
            {
                Dust dust = Dust.NewDustDirect(player.position, player.width, player.height,
                    DustID.GrassBlades, Main.rand.NextFloat(-4f, 4f), Main.rand.NextFloat(-4f, 0f), 100, Color.Green, 1.5f);
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
