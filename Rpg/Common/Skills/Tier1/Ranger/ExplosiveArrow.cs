using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using Rpg.Common.Base;

namespace Rpg.Common.Skills.Tier1.Ranger
{
    /// <summary>
    /// Explosive Arrow - 폭발 화살.
    /// 착탄 지점에 폭발을 일으킨다.
    /// 레인저의 범위 공격.
    /// </summary>
    public class ExplosiveArrow : BaseSkill
    {
        public override string InternalName => "ExplosiveArrow";
        public override string DisplayName => "Explosive Arrow";
        public override string Description => "Fire an arrow that explodes on impact, dealing area damage.";

        public override SkillType SkillType => SkillType.Active;
        public override JobType RequiredJob => JobType.Ranger;
        public override int RequiredLevel => 28;
        public override int SkillPointCost => 1;
        public override int MaxRank => 10;
        public override string IconTexture => $"{RpgConstants.ICON_PATH}Skills/ExplosiveArrow";
        
        public override float CooldownSeconds => 8f - (CurrentRank * 0.3f); // 8 -> 5초
        public override int ResourceCost => 30 - CurrentRank;
        public override ResourceType ResourceType => ResourceType.Stamina;

        private static readonly int[] EXPLOSION_DAMAGE = { 40, 50, 60, 75, 90, 105, 120, 140, 160, 200 };
        private static readonly float[] EXPLOSION_RADIUS = { 60f, 70f, 80f, 90f, 100f, 110f, 120f, 130f, 140f, 160f };

        protected override void OnActivate(Player player)
        {
            int rank = System.Math.Max(1, CurrentRank);
            
            // 마우스 방향으로 폭발 화살 발사
            Vector2 direction = (Main.MouseWorld - player.Center).SafeNormalize(Vector2.UnitX);
            Vector2 explosionPoint = player.Center + direction * 300f;
            
            // 가장 가까운 적 근처에서 폭발
            NPC closestNPC = null;
            float closestDist = 600f;
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC npc = Main.npc[i];
                if (npc.active && !npc.friendly)
                {
                    float dist = Vector2.Distance(player.Center, npc.Center);
                    if (dist < closestDist)
                    {
                        closestDist = dist;
                        closestNPC = npc;
                    }
                }
            }
            
            if (closestNPC != null)
                explosionPoint = closestNPC.Center;
            
            // 폭발 피해
            int damage = EXPLOSION_DAMAGE[rank - 1];
            float radius = EXPLOSION_RADIUS[rank - 1];
            
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC npc = Main.npc[i];
                if (npc.active && !npc.friendly)
                {
                    float dist = Vector2.Distance(explosionPoint, npc.Center);
                    if (dist <= radius)
                    {
                        float distFactor = 1f - (dist / radius) * 0.5f;
                        int finalDamage = (int)(Main.DamageVar(damage, player.luck) * distFactor);
                        npc.SimpleStrikeNPC(finalDamage, player.direction, true, 6f, DamageClass.Ranged, true);
                    }
                }
            }
            
            PlayEffects(explosionPoint, radius);
        }

        private void PlayEffects(Vector2 position, float radius)
        {
            SoundEngine.PlaySound(SoundID.Item14, position);
            
            // 폭발 이펙트
            for (int i = 0; i < 40; i++)
            {
                float angle = MathHelper.TwoPi * i / 40f;
                float dist = Main.rand.NextFloat(radius);
                Vector2 offset = new Vector2((float)System.Math.Cos(angle), (float)System.Math.Sin(angle)) * dist;
                Dust dust = Dust.NewDustDirect(position + offset, 4, 4, DustID.Torch,
                    offset.X * 0.1f, offset.Y * 0.1f, 100, Color.Orange, 1.5f);
                dust.noGravity = true;
            }
            
            // 연기
            for (int i = 0; i < 20; i++)
            {
                Dust dust = Dust.NewDustDirect(position, 4, 4, DustID.Smoke,
                    Main.rand.NextFloat(-3f, 3f), Main.rand.NextFloat(-4f, 0f), 150, Color.Gray, 1.2f);
                dust.noGravity = false;
            }
        }
    }
}
