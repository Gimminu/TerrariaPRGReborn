using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Rpg.Common.Base;

namespace Rpg.Common.Skills.Tier2.Cleric
{
    /// <summary>
    /// Divine Shield - 신성 보호막.
    /// 일시적인 무적 + 방어.
    /// 클레릭의 생존 버프. 강한 버프: 쿨 > 지속.
    /// </summary>
    public class DivineShield : BaseSkill
    {
        public override string InternalName => "DivineShield";
        public override string DisplayName => "Divine Shield";
        public override string Description => "Shield yourself with divine protection.";

        public override SkillType SkillType => SkillType.Buff;
        public override JobType RequiredJob => JobType.Cleric;
        public override int RequiredLevel => 70;
        public override int SkillPointCost => 1;
        public override int MaxRank => 10;
        public override string IconTexture => $"{RpgConstants.ICON_PATH}Skills/DivineShield";
        
        // 강한 버프: 쿨 > 지속
        public override float CooldownSeconds => 40f - (CurrentRank * 1f); // 40 -> 30초
        public override int ResourceCost => 40;
        public override ResourceType ResourceType => ResourceType.Mana;

        private static readonly int[] DURATION_SECONDS = { 3, 4, 4, 5, 5, 6, 6, 7, 7, 8 };

        protected override void OnActivate(Player player)
        {
            int rank = System.Math.Max(1, CurrentRank);
            int duration = DURATION_SECONDS[rank - 1] * 60;
            
            // 무적
            player.immune = true;
            player.immuneTime = duration;
            
            player.AddBuff(BuffID.Shine, duration);
            
            PlayEffects(player);
            ShowMessage(player, "Divine Shield!", Color.Gold);
        }

        private void PlayEffects(Player player)
        {
            SoundEngine.PlaySound(SoundID.Item29 with { Pitch = 0.3f }, player.position);
            
            for (int i = 0; i < 30; i++)
            {
                float angle = (float)i / 30f * MathHelper.TwoPi;
                Vector2 offset = new Vector2((float)System.Math.Cos(angle), (float)System.Math.Sin(angle)) * 50f;
                
                Dust dust = Dust.NewDustDirect(player.Center + offset, 4, 4, DustID.GoldFlame,
                    0f, 0f, 150, Color.Gold, 1.5f);
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
