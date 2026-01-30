using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Rpg.Common.Base;

namespace Rpg.Common.Skills.Tier2.Spellblade
{
    /// <summary>
    /// Enchanted Blade - 마법 부여 검.
    /// 근접과 마법 피해 증가 버프.
    /// 마검사의 핵심 버프. 만렙 시 쿨 = 지속 (25초).
    /// </summary>
    public class EnchantedBlade : BaseSkill
    {
        public override string InternalName => "EnchantedBlade";
        public override string DisplayName => "Enchanted Blade";
        public override string Description => "Enchant your blade, increasing melee and magic damage.";

        public override SkillType SkillType => SkillType.Buff;
        public override JobType RequiredJob => JobType.Spellblade;
        public override int RequiredLevel => 65;
        public override int SkillPointCost => 1;
        public override int MaxRank => 10;
        public override string IconTexture => $"{RpgConstants.ICON_PATH}Skills/EnchantedBlade";
        
        // 만렙 시 쿨 = 지속 (25초)
        public override float CooldownSeconds => 35f - (CurrentRank * 1f); // 35 -> 25초
        public override int ResourceCost => 30;
        public override ResourceType ResourceType => ResourceType.Mana;

        private static readonly int[] DURATION_SECONDS = { 15, 16, 17, 18, 19, 20, 21, 22, 23, 25 };

        protected override void OnActivate(Player player)
        {
            int rank = System.Math.Max(1, CurrentRank);
            int duration = DURATION_SECONDS[rank - 1] * 60;
            
            player.AddBuff(BuffID.WeaponImbueVenom, duration);
            player.AddBuff(BuffID.MagicPower, duration);
            
            PlayEffects(player);
            ShowMessage(player, "Blade Enchanted!", Color.MediumPurple);
        }

        private void PlayEffects(Player player)
        {
            SoundEngine.PlaySound(SoundID.Item29, player.position);
            
            for (int i = 0; i < 25; i++)
            {
                Dust dust = Dust.NewDustDirect(player.position, player.width, player.height,
                    DustID.Enchanted_Gold, Main.rand.NextFloat(-2f, 2f), Main.rand.NextFloat(-2f, 0f), 150, Color.Purple, 1.2f);
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
