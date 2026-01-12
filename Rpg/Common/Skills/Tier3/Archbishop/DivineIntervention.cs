using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Rpg.Common.Base;

namespace Rpg.Common.Skills.Tier3.Archbishop
{
    /// <summary>
    /// Divine Intervention - 신의 개입.
    /// 대량 회복 + 무적.
    /// 대주교의 궁극 힐.
    /// </summary>
    public class DivineIntervention : BaseSkill
    {
        public override string InternalName => "DivineIntervention";
        public override string DisplayName => "Divine Intervention";
        public override string Description => "Call upon divine power to fully heal and become invincible briefly.";

        public override SkillType SkillType => SkillType.Buff;
        public override JobType RequiredJob => JobType.Archbishop;
        public override int RequiredLevel => 120;
        public override int SkillPointCost => 1;
        public override int MaxRank => 10;
        public override string IconTexture => $"{RpgConstants.ICON_PATH}Skills/DivineIntervention";
        
        // 강한 버프: 쿨 > 지속
        public override float CooldownSeconds => 60f - (CurrentRank * 2f); // 60 -> 40초
        public override int ResourceCost => 80;
        public override ResourceType ResourceType => ResourceType.Mana;

        private static readonly float[] HEAL_PERCENT = { 0.30f, 0.35f, 0.40f, 0.45f, 0.50f, 0.55f, 0.60f, 0.68f, 0.78f, 1.00f };
        private static readonly int[] IMMUNITY_TICKS = { 60, 75, 90, 105, 120, 135, 150, 170, 195, 240 };

        protected override void OnActivate(Player player)
        {
            int rank = System.Math.Max(1, CurrentRank);
            
            // 대량 회복
            int heal = (int)(player.statLifeMax2 * HEAL_PERCENT[rank - 1]);
            player.Heal(heal);
            
            // 무적
            player.immune = true;
            player.immuneTime = IMMUNITY_TICKS[rank - 1];
            
            player.AddBuff(BuffID.Regeneration, 600);
            player.AddBuff(BuffID.Shine, 600);
            
            PlayEffects(player);
            ShowMessage(player, "Divine Intervention!", Color.Gold);
        }

        private void PlayEffects(Player player)
        {
            SoundEngine.PlaySound(SoundID.Item29 with { Pitch = 0.5f }, player.position);
            
            for (int i = 0; i < 50; i++)
            {
                float angle = (float)i / 50f * MathHelper.TwoPi;
                Vector2 offset = new Vector2((float)System.Math.Cos(angle), (float)System.Math.Sin(angle)) * 70f;
                
                Dust dust = Dust.NewDustDirect(player.Center + offset, 8, 8, DustID.GoldFlame,
                    0f, -2f, 150, Color.Gold, 1.6f);
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
