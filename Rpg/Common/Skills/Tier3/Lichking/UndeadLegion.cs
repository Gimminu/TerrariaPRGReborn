using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Rpg.Common.Base;
using Rpg.Common.Players;

namespace Rpg.Common.Skills.Tier3.Lichking
{
    /// <summary>
    /// Undead Legion - Lichking's ultimate summon skill.
    /// </summary>
    public class UndeadLegion : BaseSkill
    {
        public override string InternalName => "UndeadLegion";
        public override string DisplayName => "Undead Legion";
        public override string Description => "Command a legion of undead, massively boosting summon power.";

        public override SkillType SkillType => SkillType.Active;
        public override JobType RequiredJob => JobType.Lichking;
        public override int RequiredLevel => RpgConstants.THIRD_JOB_LEVEL + 10;
        public override int SkillPointCost => 2;
        public override int MaxRank => 5;
        public override string IconTexture => $"{RpgConstants.ICON_PATH}Skills/UndeadLegion";
        public override float CooldownSeconds => 35f;
        public override int ResourceCost => 45;
        public override ResourceType ResourceType => ResourceType.Mana;

        private static readonly int[] DURATION_SECONDS = { 15, 18, 22, 26, 35 };
        private static readonly float[] SUMMON_BONUS = { 0.30f, 0.40f, 0.50f, 0.65f, 0.85f };

        protected override void OnActivate(Player player)
        {
            int rank = System.Math.Max(1, CurrentRank);
            int duration = DURATION_SECONDS[rank - 1] * 60;
            float bonus = SUMMON_BONUS[rank - 1];

            var rpgPlayer = player.GetModPlayer<RpgPlayer>();
            rpgPlayer.AddTemporarySummonDamage(bonus, duration);

            player.AddBuff(BuffID.Wrath, duration);

            PlayEffects(player);
            ShowMessage(player, "UNDEAD LEGION!", Color.DarkMagenta);
        }

        private void PlayEffects(Player player)
        {
            SoundEngine.PlaySound(SoundID.NPCDeath10, player.position);
            for (int i = 0; i < 40; i++)
            {
                Dust dust = Dust.NewDustDirect(player.position, player.width, player.height,
                    DustID.Shadowflame, Main.rand.NextFloat(-5f, 5f), Main.rand.NextFloat(-5f, 0f), 100, Color.Purple, 2f);
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
