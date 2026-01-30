using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using Rpg.Common.Base;

namespace Rpg.Common.Skills.Tier2.Archmage
{
    /// <summary>
    /// Chain Lightning - 연쇄 번개.
    /// 적들 사이에서 연쇄되는 번개.
    /// </summary>
    public class ChainLightning : BaseSkill
    {
        public override string InternalName => "ChainLightning";
        public override string DisplayName => "Chain Lightning";
        public override string Description => "Release a bolt of lightning that chains between enemies.";

        public override SkillType SkillType => SkillType.Active;
        public override JobType RequiredJob => JobType.Archmage;
        public override int RequiredLevel => 68;
        public override int SkillPointCost => 1;
        public override int MaxRank => 10;
        public override string IconTexture => $"{RpgConstants.ICON_PATH}Skills/ChainLightning";
        
        public override float CooldownSeconds => 8f - (CurrentRank * 0.3f);
        public override int ResourceCost => 50;
        public override ResourceType ResourceType => ResourceType.Mana;

        private static readonly int[] DAMAGE = { 60, 80, 100, 125, 150, 180, 210, 245, 285, 360 };
        private static readonly int[] MAX_CHAINS = { 3, 3, 4, 4, 5, 5, 6, 6, 7, 8 };

        protected override void OnActivate(Player player)
        {
            int rank = System.Math.Max(1, CurrentRank);
            int damage = DAMAGE[rank - 1];
            int maxChains = MAX_CHAINS[rank - 1];
            
            System.Collections.Generic.List<NPC> hitNPCs = new System.Collections.Generic.List<NPC>();
            NPC current = FindNearestEnemy(Main.MouseWorld, hitNPCs, 400f);
            
            for (int chain = 0; chain < maxChains && current != null; chain++)
            {
                int finalDamage = Main.DamageVar(damage, player.luck);
                current.SimpleStrikeNPC(finalDamage, player.direction, true, 4f, DamageClass.Magic, true);
                
                hitNPCs.Add(current);
                CreateLightningEffect(chain == 0 ? player.Center : hitNPCs[chain - 1].Center, current.Center);
                
                current = FindNearestEnemy(current.Center, hitNPCs, 300f);
            }
            
            SoundEngine.PlaySound(SoundID.Item122, player.position);
        }

        private NPC FindNearestEnemy(Vector2 position, System.Collections.Generic.List<NPC> exclude, float range)
        {
            NPC nearest = null;
            float nearestDist = range;
            
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC npc = Main.npc[i];
                if (npc.active && !npc.friendly && npc.CanBeChasedBy() && !exclude.Contains(npc))
                {
                    float dist = Vector2.Distance(position, npc.Center);
                    if (dist < nearestDist)
                    {
                        nearestDist = dist;
                        nearest = npc;
                    }
                }
            }
            
            return nearest;
        }

        private void CreateLightningEffect(Vector2 start, Vector2 end)
        {
            int segments = (int)(Vector2.Distance(start, end) / 20f);
            Vector2 direction = (end - start).SafeNormalize(Vector2.Zero);
            
            for (int i = 0; i <= segments; i++)
            {
                Vector2 pos = Vector2.Lerp(start, end, (float)i / segments);
                pos += Main.rand.NextVector2Circular(8f, 8f);
                
                Dust dust = Dust.NewDustDirect(pos, 4, 4, DustID.Electric,
                    0, 0, 100, Color.Yellow, 1.2f);
                dust.noGravity = true;
            }
        }
    }
}
