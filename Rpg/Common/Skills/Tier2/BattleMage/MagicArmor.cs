using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Rpg.Common.Base;
using Rpg.Common.Players;

namespace Rpg.Common.Skills.Tier2.BattleMage
{
    /// <summary>
    /// Magic Armor - Battle Mage's defensive buff.
    /// </summary>
    public class MagicArmor : BaseSkill
    {
        public override string InternalName => "MagicArmor";
        public override string DisplayName => "Magic Armor";
        public override string Description => "Cloak yourself in magical armor for defense.";

        public override SkillType SkillType => SkillType.Active;
        public override JobType RequiredJob => JobType.BattleMage;
        public override int RequiredLevel => RpgConstants.SECOND_JOB_LEVEL;
        public override int SkillPointCost => 1;
        public override int MaxRank => 5;
        public override string IconTexture => $"{RpgConstants.ICON_PATH}Skills/MagicArmor";
        public override float CooldownSeconds => 25f;
        public override int ResourceCost => 25;
        public override ResourceType ResourceType => ResourceType.Mana;

        private static readonly int[] DURATION_SECONDS = { 12, 15, 18, 22, 28 };
        private static readonly int[] DEFENSE_BONUS = { 8, 12, 16, 20, 28 };

        protected override void OnActivate(Player player)
        {
            int rank = System.Math.Max(1, CurrentRank);
            int duration = DURATION_SECONDS[rank - 1] * 60;
            int defense = DEFENSE_BONUS[rank - 1];

            var rpgPlayer = player.GetModPlayer<RpgPlayer>();
            rpgPlayer.AddTemporaryDefense(defense, duration);

            player.AddBuff(BuffID.MagicPower, duration);

            PlayEffects(player);
            ShowMessage(player, "Magic Armor!", Color.Purple);
        }

        private void PlayEffects(Player player)
        {
            SoundEngine.PlaySound(SoundID.Item25, player.position);
            for (int i = 0; i < 25; i++)
            {
                float angle = MathHelper.TwoPi * i / 25f;
                Vector2 offset = new Vector2((float)System.Math.Cos(angle), (float)System.Math.Sin(angle)) * 35f;
                Dust dust = Dust.NewDustDirect(player.Center + offset, 4, 4, DustID.MagicMirror, 
                    0, 0, 100, Color.Purple, 1.3f);
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
