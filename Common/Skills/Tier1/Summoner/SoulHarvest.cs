using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using Rpg.Common.Base;

namespace Rpg.Common.Skills.Tier1.Summoner
{
    /// <summary>
    /// Soul Harvest - 영혼 수확.
    /// 근처 적을 공격하여 체력을 회복.
    /// 소환사의 생존기.
    /// </summary>
    public class SoulHarvest : BaseSkill
    {
        public override string InternalName => "SoulHarvest";
        public override string DisplayName => "Soul Harvest";
        public override string Description => "Harvest souls from nearby enemies, dealing damage and healing yourself.";

        public override SkillType SkillType => SkillType.Active;
        public override JobType RequiredJob => JobType.Summoner;
        public override int RequiredLevel => 18;
        public override int SkillPointCost => 1;
        public override int MaxRank => 10;
        public override string IconTexture => $"{RpgConstants.ICON_PATH}Skills/SoulHarvest";
        
        public override float CooldownSeconds => 15f - (CurrentRank * 0.5f); // 15 -> 10초
        public override int ResourceCost => 45;
        public override ResourceType ResourceType => ResourceType.Mana;

        private static readonly int[] DAMAGE = { 30, 40, 50, 65, 80, 95, 110, 130, 150, 180 };
        private static readonly int[] HEAL_PER_HIT = { 5, 7, 9, 12, 15, 18, 21, 25, 30, 40 };
        private static readonly float RADIUS = 150f;

        protected override void OnActivate(Player player)
        {
            int rank = System.Math.Max(1, CurrentRank);
            int damage = DAMAGE[rank - 1];
            int healPerHit = HEAL_PER_HIT[rank - 1];
            int totalHeal = 0;
            
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC npc = Main.npc[i];
                if (npc.active && !npc.friendly)
                {
                    float dist = Vector2.Distance(player.Center, npc.Center);
                    if (dist <= RADIUS)
                    {
                        int finalDamage = Main.DamageVar(damage, player.luck);
                        npc.SimpleStrikeNPC(finalDamage, player.direction, true, 3f, DamageClass.Summon, true);
                        totalHeal += healPerHit;
                        
                        // 영혼 흡수 이펙트
                        for (int d = 0; d < 5; d++)
                        {
                            Vector2 dir = (player.Center - npc.Center).SafeNormalize(Vector2.Zero);
                            Dust dust = Dust.NewDustDirect(npc.Center, 4, 4, DustID.Wraith,
                                dir.X * 5f, dir.Y * 5f, 100, Color.LightGreen, 1.0f);
                            dust.noGravity = true;
                        }
                    }
                }
            }
            
            // 회복
            if (totalHeal > 0)
            {
                int maxHeal = player.statLifeMax2 / 4; // 최대 25% 회복 제한
                totalHeal = System.Math.Min(totalHeal, maxHeal);
                player.statLife = System.Math.Min(player.statLife + totalHeal, player.statLifeMax2);
                player.HealEffect(totalHeal, true);
            }
            
            PlayEffects(player);
        }

        private void PlayEffects(Player player)
        {
            SoundEngine.PlaySound(SoundID.NPCDeath39, player.position);
            
            // 영혼 수확 이펙트
            for (int i = 0; i < 30; i++)
            {
                float angle = MathHelper.TwoPi * i / 30f;
                Vector2 offset = new Vector2((float)System.Math.Cos(angle), (float)System.Math.Sin(angle)) * RADIUS;
                Dust dust = Dust.NewDustDirect(player.Center + offset, 4, 4, DustID.Wraith,
                    -offset.X * 0.05f, -offset.Y * 0.05f, 150, Color.LightGreen, 1.2f);
                dust.noGravity = true;
            }
        }
    }
}
