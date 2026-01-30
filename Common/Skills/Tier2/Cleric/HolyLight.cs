using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Rpg.Common.Base;

namespace Rpg.Common.Skills.Tier2.Cleric
{
    /// <summary>
    /// Holy Light - 성스러운 빛.
    /// 대상 회복.
    /// 클레릭의 기본 힐.
    /// </summary>
    public class HolyLight : BaseSkill
    {
        public override string InternalName => "HolyLight";
        public override string DisplayName => "Holy Light";
        public override string Description => "Heal yourself with holy energy.";

        public override SkillType SkillType => SkillType.Active;
        public override JobType RequiredJob => JobType.Cleric;
        public override int RequiredLevel => 60;
        public override int SkillPointCost => 1;
        public override int MaxRank => 10;
        public override string IconTexture => $"{RpgConstants.ICON_PATH}Skills/HolyLight";
        
        public override float CooldownSeconds => 12f - (CurrentRank * 0.4f); // 12 -> 8초
        public override int ResourceCost => 35;
        public override ResourceType ResourceType => ResourceType.Mana;

        private static readonly int[] HEAL_AMOUNT = { 50, 65, 80, 100, 120, 145, 175, 210, 255, 320 };

        protected override void OnActivate(Player player)
        {
            int rank = System.Math.Max(1, CurrentRank);
            int heal = HEAL_AMOUNT[rank - 1];
            
            player.Heal(heal);
            
            PlayEffects(player);
            ShowMessage(player, $"+{heal}", Color.LimeGreen);
        }

        private void PlayEffects(Player player)
        {
            SoundEngine.PlaySound(SoundID.Item4, player.position);
            
            for (int i = 0; i < 25; i++)
            {
                Dust dust = Dust.NewDustDirect(player.position, player.width, player.height,
                    DustID.GoldFlame, Main.rand.NextFloat(-2f, 2f), Main.rand.NextFloat(-3f, 0f), 150, Color.Gold, 1.2f);
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
