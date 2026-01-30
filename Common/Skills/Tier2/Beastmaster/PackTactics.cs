using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Rpg.Common.Base;
using Rpg.Common.Players;

namespace Rpg.Common.Skills.Tier2.Beastmaster
{
    /// <summary>
    /// Pack Tactics - Beastmaster's buff when minions are active.
    /// </summary>
    public class PackTactics : BaseSkill
    {
        public override string InternalName => "PackTactics";
        public override string DisplayName => "Pack Tactics";
        public override string Description => "Boost summon damage and your own damage when minions are active.";

        public override SkillType SkillType => SkillType.Active;
        public override JobType RequiredJob => JobType.Beastmaster;
        public override int RequiredLevel => RpgConstants.SECOND_JOB_LEVEL + 10;
        public override int SkillPointCost => 1;
        public override int MaxRank => 5;
        public override string IconTexture => $"{RpgConstants.ICON_PATH}Skills/PackTactics";
        public override float CooldownSeconds => 22f;
        public override int ResourceCost => 20;
        public override ResourceType ResourceType => ResourceType.Mana;

        private static readonly int[] DURATION_SECONDS = { 10, 12, 14, 16, 20 };
        private static readonly float[] SUMMON_BONUS = { 0.10f, 0.14f, 0.18f, 0.22f, 0.30f };

        protected override void OnActivate(Player player)
        {
            int rank = System.Math.Max(1, CurrentRank);
            int duration = DURATION_SECONDS[rank - 1] * 60;
            float bonus = SUMMON_BONUS[rank - 1];

            var rpgPlayer = player.GetModPlayer<RpgPlayer>();
            rpgPlayer.AddTemporarySummonDamage(bonus, duration);

            PlayEffects(player);
            ShowMessage(player, "Pack Tactics!", Color.Green);
        }

        private void PlayEffects(Player player)
        {
            SoundEngine.PlaySound(SoundID.Roar, player.position);
            for (int i = 0; i < 20; i++)
            {
                Dust dust = Dust.NewDustDirect(player.position, player.width, player.height,
                    DustID.GreenTorch, Main.rand.NextFloat(-3f, 3f), Main.rand.NextFloat(-3f, 0f), 100, Color.Green, 1.3f);
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
