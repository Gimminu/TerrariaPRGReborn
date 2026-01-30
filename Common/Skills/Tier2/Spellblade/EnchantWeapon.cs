using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using Rpg.Common.Base;
using Rpg.Common.Players;

namespace Rpg.Common.Skills.Tier2.Spellblade
{
    /// <summary>
    /// Enchant Weapon - Spellblade's weapon enhancement skill.
    /// </summary>
    public class EnchantWeapon : BaseSkill
    {
        public override string InternalName => "EnchantWeapon";
        public override string DisplayName => "Enchant Weapon";
        public override string Description => "Enchant your weapon with arcane energy.";

        public override SkillType SkillType => SkillType.Active;
        public override JobType RequiredJob => JobType.Spellblade;
        public override int RequiredLevel => RpgConstants.SECOND_JOB_LEVEL;
        public override int SkillPointCost => 1;
        public override int MaxRank => 5;
        public override string IconTexture => $"{RpgConstants.ICON_PATH}Skills/EnchantWeapon";
        public override float CooldownSeconds => 20f;
        public override int ResourceCost => 25;
        public override ResourceType ResourceType => ResourceType.Mana;

        private static readonly int[] DURATION_SECONDS = { 12, 15, 18, 22, 28 };
        private static readonly float[] DAMAGE_BONUS = { 0.08f, 0.12f, 0.16f, 0.20f, 0.28f };

        protected override void OnActivate(Player player)
        {
            int rank = System.Math.Max(1, CurrentRank);
            int duration = DURATION_SECONDS[rank - 1] * 60;
            float bonus = DAMAGE_BONUS[rank - 1];

            var rpgPlayer = player.GetModPlayer<RpgPlayer>();
            rpgPlayer.AddTemporaryDamage(bonus, duration);

            player.AddBuff(BuffID.WeaponImbueFire, duration);

            PlayEffects(player);
            ShowMessage(player, "Weapon Enchanted!", Color.Cyan);
        }

        private void PlayEffects(Player player)
        {
            SoundEngine.PlaySound(SoundID.Item29, player.position);
            for (int i = 0; i < 20; i++)
            {
                Dust dust = Dust.NewDustDirect(player.position, player.width, player.height,
                    DustID.MagicMirror, Main.rand.NextFloat(-3f, 3f), Main.rand.NextFloat(-3f, 0f), 100, Color.Cyan, 1.4f);
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
