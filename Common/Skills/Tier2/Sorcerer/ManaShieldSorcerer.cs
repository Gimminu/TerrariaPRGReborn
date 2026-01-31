using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using RpgMod.Common.Base;

namespace RpgMod.Common.Skills.Tier2.Sorcerer
{
    /// <summary>
    /// Mana Shield - 마나 보호막.
    /// 마나로 피해 흡수.
    /// 소서러의 방어 버프. 만렙 시 쿨 = 지속.
    /// </summary>
    public class ManaShieldSorcerer : BaseSkill
    {
        public override string InternalName => "ManaShieldSorcerer";
        public override string DisplayName => "Mana Shield";
        public override string Description => "Boost mana regeneration and empower mana for a short time.";

        public override SkillType SkillType => SkillType.Buff;
        public override JobType RequiredJob => JobType.Sorcerer;
        public override int RequiredLevel => 68;
        public override int SkillPointCost => 1;
        public override int MaxRank => 10;
        public override string IconTexture => $"{RpgConstants.ICON_PATH}Skills/ManaShieldSorcerer";
        
        // 만렙 시 쿨 = 지속 (25초)
        public override float CooldownSeconds => 35f - (CurrentRank * 1f); // 35 -> 25초
        public override int ResourceCost => 30;
        public override ResourceType ResourceType => ResourceType.Mana;

        private static readonly int[] DURATION_SECONDS = { 15, 16, 17, 18, 19, 20, 21, 22, 23, 25 };

        protected override void OnActivate(Player player)
        {
            int rank = System.Math.Max(1, CurrentRank);
            int duration = DURATION_SECONDS[rank - 1] * 60;
            
            player.AddBuff(BuffID.ManaRegeneration, duration);
            // 네뷸라 아머 효과 (마나 보호막 비슷)
            player.AddBuff(BuffID.NebulaUpMana3, duration);
            
            PlayEffects(player);
            ShowMessage(player, "Mana Shield!", Color.Cyan);
        }

        private void PlayEffects(Player player)
        {
            SoundEngine.PlaySound(SoundID.Item28, player.position);
            
            for (int i = 0; i < 25; i++)
            {
                float angle = (float)i / 25f * MathHelper.TwoPi;
                Vector2 offset = new Vector2((float)System.Math.Cos(angle), (float)System.Math.Sin(angle)) * 40f;
                
                Dust dust = Dust.NewDustDirect(player.Center + offset, 4, 4, DustID.BlueCrystalShard,
                    0f, 0f, 150, Color.Cyan, 1.2f);
                dust.noGravity = true;
                dust.velocity = offset.SafeNormalize(Vector2.Zero) * 2f;
            }
        }

        private void ShowMessage(Player player, string text, Color color)
        {
            if (Main.netMode != NetmodeID.Server)
                CombatText.NewText(player.Hitbox, color, text);
        }
    }
}
