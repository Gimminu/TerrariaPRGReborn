using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using Rpg.Common.Base;
using Rpg.Common.Players;

namespace Rpg.Common.Skills.Tier3.BloodKnight
{
    /// <summary>
    /// Blood Storm - 피의 폭풍.
    /// 주변 적에게 피해를 주고 생명력 흡수.
    /// 블러드나이트의 궁극 공격기.
    /// </summary>
    public class BloodStorm : BaseSkill
    {
        public override string InternalName => "BloodStorm";
        public override string DisplayName => "Blood Storm";
        public override string Description => "Unleash a storm of blood, damaging enemies and draining their life.";

        public override SkillType SkillType => SkillType.Active;
        public override JobType RequiredJob => JobType.BloodKnight;
        public override int RequiredLevel => 120;
        public override int SkillPointCost => 1;
        public override int MaxRank => 10;
        public override string IconTexture => $"{RpgConstants.ICON_PATH}Skills/BloodStorm";
        
        public override float CooldownSeconds => 12f - (CurrentRank * 0.35f); // 12 -> 8.5초
        public override int ResourceCost => 40;
        public override ResourceType ResourceType => ResourceType.Stamina;

        private static readonly int[] DAMAGE = { 120, 150, 185, 225, 270, 320, 380, 450, 535, 660 };
        private static readonly float[] LIFESTEAL = { 0.10f, 0.12f, 0.14f, 0.16f, 0.18f, 0.20f, 0.22f, 0.24f, 0.27f, 0.35f };

        protected override void OnActivate(Player player)
        {
            int rank = System.Math.Max(1, CurrentRank);
            int damage = DAMAGE[rank - 1];
            float lifesteal = LIFESTEAL[rank - 1];
            float radius = 200f;
            int totalHealed = 0;
            
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC npc = Main.npc[i];
                if (npc.active && !npc.friendly && npc.CanBeChasedBy())
                {
                    float dist = Vector2.Distance(player.Center, npc.Center);
                    if (dist <= radius)
                    {
                        int finalDamage = Main.DamageVar(damage, player.luck);
                        npc.SimpleStrikeNPC(finalDamage, player.direction, true, 4f, DamageClass.Melee, true);
                        npc.AddBuff(BuffID.Bleeding, 300);
                        
                        totalHealed += (int)(finalDamage * lifesteal);
                    }
                }
            }
            
            if (totalHealed > 0) player.Heal(totalHealed);
            
            PlayEffects(player, radius);
            if (totalHealed > 0) ShowMessage(player, $"+{totalHealed}", Color.LimeGreen);
        }

        private void PlayEffects(Player player, float radius)
        {
            SoundEngine.PlaySound(SoundID.Item71 with { Pitch = -0.3f }, player.position);
            
            for (int i = 0; i < 50; i++)
            {
                float angle = (float)i / 50f * MathHelper.TwoPi;
                Vector2 offset = new Vector2((float)System.Math.Cos(angle), (float)System.Math.Sin(angle)) * radius * 0.6f;
                
                Dust dust = Dust.NewDustDirect(player.Center + offset, 8, 8, DustID.Blood,
                    offset.X * 0.05f, offset.Y * 0.05f, 150, Color.DarkRed, 1.5f);
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
