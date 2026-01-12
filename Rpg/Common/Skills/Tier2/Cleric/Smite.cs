using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using Rpg.Common.Base;

namespace Rpg.Common.Skills.Tier2.Cleric
{
    /// <summary>
    /// Smite - 천벌.
    /// 신성 피해.
    /// </summary>
    public class Smite : BaseSkill
    {
        public override string InternalName => "Smite";
        public override string DisplayName => "Smite";
        public override string Description => "Call down divine judgment upon your enemies.";

        public override SkillType SkillType => SkillType.Active;
        public override JobType RequiredJob => JobType.Cleric;
        public override int RequiredLevel => 68;
        public override int SkillPointCost => 1;
        public override int MaxRank => 10;
        public override string IconTexture => $"{RpgConstants.ICON_PATH}Skills/Smite";
        
        public override float CooldownSeconds => 8f - (CurrentRank * 0.3f);
        public override int ResourceCost => 35;
        public override ResourceType ResourceType => ResourceType.Mana;

        private static readonly int[] DAMAGE = { 80, 105, 130, 160, 195, 235, 280, 330, 385, 480 };

        protected override void OnActivate(Player player)
        {
            int rank = System.Math.Max(1, CurrentRank);
            int damage = DAMAGE[rank - 1];
            Vector2 targetPos = Main.MouseWorld;
            
            // 타겟 위치에서 가장 가까운 적
            NPC target = null;
            float closestDist = 200f;
            
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC npc = Main.npc[i];
                if (npc.active && !npc.friendly && npc.CanBeChasedBy())
                {
                    float dist = Vector2.Distance(targetPos, npc.Center);
                    if (dist < closestDist)
                    {
                        closestDist = dist;
                        target = npc;
                    }
                }
            }
            
            if (target != null)
            {
                int finalDamage = Main.DamageVar(damage, player.luck);
                target.SimpleStrikeNPC(finalDamage, player.direction, true, 4f, DamageClass.Magic, true);
                
                PlayEffects(target.Center);
            }
            else
            {
                PlayEffects(targetPos);
            }
        }

        private void PlayEffects(Vector2 position)
        {
            SoundEngine.PlaySound(SoundID.Item122, position);
            
            // 천상의 빛
            for (int i = 0; i < 20; i++)
            {
                Vector2 start = position + new Vector2(Main.rand.NextFloat(-50, 50), -600);
                Vector2 direction = (position - start).SafeNormalize(Vector2.UnitY);
                
                Dust dust = Dust.NewDustDirect(start + direction * i * 30, 4, 4, DustID.GoldCoin,
                    direction.X * 5f, direction.Y * 5f, 100, Color.Gold, 1.5f);
                dust.noGravity = true;
            }
            
            // 착탄 이펙트
            for (int i = 0; i < 25; i++)
            {
                Dust dust = Dust.NewDustDirect(position, 4, 4, DustID.GoldCoin,
                    Main.rand.NextFloat(-5f, 5f), Main.rand.NextFloat(-5f, 5f), 100, Color.Gold, 1.3f);
                dust.noGravity = true;
            }
        }
    }
}
