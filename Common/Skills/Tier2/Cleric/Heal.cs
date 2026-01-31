using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using RpgMod.Common.Base;
using RpgMod.Common.Effects;

namespace RpgMod.Common.Skills.Tier2.Cleric
{
    /// <summary>
    /// Heal - 치유.
    /// 자신과 주변 아군을 치유.
    /// </summary>
    public class Heal : BaseSkill
    {
        public override string InternalName => "Heal";
        public override string DisplayName => "Heal";
        public override string Description => "Channel divine energy to restore health.";

        public override SkillType SkillType => SkillType.Active;
        public override JobType RequiredJob => JobType.Cleric;
        public override int RequiredLevel => 60;
        public override int SkillPointCost => 1;
        public override int MaxRank => 10;
        public override string IconTexture => $"{RpgConstants.ICON_PATH}Skills/Heal";
        
        public override float CooldownSeconds => 10f - (CurrentRank * 0.4f);
        public override int ResourceCost => 40;
        public override ResourceType ResourceType => ResourceType.Mana;
        
        // Use the HealEffect from registry
        public override BaseSkillEffect SkillEffect => SkillEffectRegistry.Heal;

        private static readonly int[] HEAL_AMOUNT = { 50, 70, 95, 120, 150, 185, 225, 270, 320, 400 };

        protected override void OnActivate(Player player)
        {
            int rank = System.Math.Max(1, CurrentRank);
            int healAmount = HEAL_AMOUNT[rank - 1];
            
            player.Heal(healAmount);
            
            // Use new effect system
            SpawnEffect(player, rank / 3 + 1);
            ShowMessage(player, $"+{healAmount}", Color.LightGreen);
        }

        private void ShowMessage(Player player, string text, Color color)
        {
            if (Main.netMode != NetmodeID.Server)
                CombatText.NewText(player.Hitbox, color, text);
        }
    }
}
