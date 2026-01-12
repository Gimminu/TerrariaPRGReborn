using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Rpg.Common.Base;
using Rpg.Common.Players;

namespace Rpg.Common.Skills.Tier2.Spellthief
{
    /// <summary>
    /// Steal Buff - Spellthief's buff stealing skill.
    /// </summary>
    public class StealBuff : BaseSkill
    {
        public override string InternalName => "StealBuff";
        public override string DisplayName => "Steal Buff";
        public override string Description => "Steal magic from enemies, gaining random buffs.";

        public override SkillType SkillType => SkillType.Active;
        public override JobType RequiredJob => JobType.Spellthief;
        public override int RequiredLevel => RpgConstants.SECOND_JOB_LEVEL;
        public override int SkillPointCost => 1;
        public override int MaxRank => 5;
        public override string IconTexture => $"{RpgConstants.ICON_PATH}Skills/StealBuff";
        public override float CooldownSeconds => 20f;
        public override int ResourceCost => 20;
        public override ResourceType ResourceType => ResourceType.Mana;

        private static readonly int[] DURATION_SECONDS = { 10, 13, 16, 19, 25 };
        private static readonly int[] BUFF_COUNT = { 1, 1, 2, 2, 3 };

        private readonly int[] possibleBuffs = {
            BuffID.Swiftness, BuffID.Regeneration, BuffID.Wrath, BuffID.Rage,
            BuffID.Endurance, BuffID.Ironskin, BuffID.MagicPower, BuffID.Archery
        };

        protected override void OnActivate(Player player)
        {
            int rank = System.Math.Max(1, CurrentRank);
            int duration = DURATION_SECONDS[rank - 1] * 60;
            int buffCount = BUFF_COUNT[rank - 1];

            for (int i = 0; i < buffCount; i++)
            {
                int randomBuff = possibleBuffs[Main.rand.Next(possibleBuffs.Length)];
                player.AddBuff(randomBuff, duration);
            }

            PlayEffects(player);
            ShowMessage(player, "Buffs Stolen!", Color.MediumPurple);
        }

        private void PlayEffects(Player player)
        {
            SoundEngine.PlaySound(SoundID.Item28, player.position);
            for (int i = 0; i < 20; i++)
            {
                Dust dust = Dust.NewDustDirect(player.position, player.width, player.height,
                    DustID.MagicMirror, Main.rand.NextFloat(-4f, 4f), Main.rand.NextFloat(-4f, 0f), 100, Color.Purple, 1.4f);
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
