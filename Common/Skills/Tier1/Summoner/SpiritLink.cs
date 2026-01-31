using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using RpgMod.Common.Base;

namespace RpgMod.Common.Skills.Tier1.Summoner
{
    /// <summary>
    /// Spirit Link - 정령 연결.
    /// 소환수와 연결하여 이동속도와 회복력 증가.
    /// 만렙 시 쿨타임 = 지속시간 (25초)
    /// </summary>
    public class SpiritLink : BaseSkill
    {
        public override string InternalName => "SpiritLink";
        public override string DisplayName => "Spirit Link";
        public override string Description => "Link with your minions, sharing their speed and recovering health over time.";

        public override SkillType SkillType => SkillType.Buff;
        public override JobType RequiredJob => JobType.Summoner;
        public override int RequiredLevel => 22;
        public override int SkillPointCost => 1;
        public override int MaxRank => 10;
        public override string IconTexture => $"{RpgConstants.ICON_PATH}Skills/SpiritLink";
        
        // 만렙 시 쿨 = 지속 (25초)
        public override float CooldownSeconds => 40f - (CurrentRank * 1.5f); // 40 -> 25초
        public override int ResourceCost => 35;
        public override ResourceType ResourceType => ResourceType.Mana;

        private static readonly int[] DURATION_SECONDS = { 12, 14, 16, 18, 19, 20, 21, 22, 24, 25 };

        protected override void OnActivate(Player player)
        {
            int rank = System.Math.Max(1, CurrentRank);
            int duration = DURATION_SECONDS[rank - 1] * 60;

            player.AddBuff(BuffID.Swiftness, duration);
            player.AddBuff(BuffID.Regeneration, duration);

            PlayEffects(player);
            ShowMessage(player, "Spirit Link!", Color.Violet);
        }

        private void PlayEffects(Player player)
        {
            SoundEngine.PlaySound(SoundID.Item44, player.position);
            
            // 연결선 이펙트 (플레이어에서 소환수로)
            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                var proj = Main.projectile[i];
                if (proj.active && proj.owner == player.whoAmI && (proj.minion || proj.sentry || proj.minionSlots > 0f))
                {
                    Vector2 direction = (proj.Center - player.Center).SafeNormalize(Vector2.Zero);
                    float dist = Vector2.Distance(player.Center, proj.Center);
                    
                    for (int d = 0; d < dist / 20; d++)
                    {
                        Vector2 pos = player.Center + direction * d * 20;
                        Dust dust = Dust.NewDustDirect(pos, 4, 4, DustID.Enchanted_Pink,
                            0, 0, 100, Color.Violet, 0.8f);
                        dust.noGravity = true;
                    }
                }
            }
        }

        private void ShowMessage(Player player, string text, Color color)
        {
            if (Main.netMode != NetmodeID.Server)
                CombatText.NewText(player.Hitbox, color, text);
        }
    }
}
