using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using RpgMod.Common.Base;

namespace RpgMod.Common.Skills.Tier2.Warlock
{
    /// <summary>
    /// Shadow Bolt - 어둠의 화살.
    /// 강력한 어둠 마법 피해.
    /// </summary>
    public class ShadowBolt : BaseSkill
    {
        public override string InternalName => "ShadowBolt";
        public override string DisplayName => "Shadow Bolt";
        public override string Description => "Hurl a bolt of shadow energy at your target.";

        public override SkillType SkillType => SkillType.Active;
        public override JobType RequiredJob => JobType.Warlock;
        public override int RequiredLevel => 65;
        public override int SkillPointCost => 1;
        public override int MaxRank => 10;
        public override string IconTexture => $"{RpgConstants.ICON_PATH}Skills/ShadowBolt";
        
        public override float CooldownSeconds => 5f - (CurrentRank * 0.15f);
        public override int ResourceCost => 25;
        public override ResourceType ResourceType => ResourceType.Mana;

        private static readonly int[] DAMAGE = { 70, 90, 115, 145, 175, 210, 250, 295, 345, 430 };

        protected override void OnActivate(Player player)
        {
            int rank = System.Math.Max(1, CurrentRank);
            int damage = DAMAGE[rank - 1];
            
            // 마우스 방향으로 타겟 찾기
            NPC target = null;
            float closestDist = 600f;
            Vector2 direction = (Main.MouseWorld - player.Center).SafeNormalize(Vector2.UnitX);
            
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC npc = Main.npc[i];
                if (npc.active && !npc.friendly && npc.CanBeChasedBy())
                {
                    Vector2 toNPC = npc.Center - player.Center;
                    float dist = toNPC.Length();
                    float dot = Vector2.Dot(toNPC.SafeNormalize(Vector2.Zero), direction);
                    
                    if (dot > 0.85f && dist < closestDist)
                    {
                        closestDist = dist;
                        target = npc;
                    }
                }
            }
            
            if (target != null)
            {
                float scaledDamage = GetScaledDamage(player, DamageClass.Magic, damage);
                int finalDamage = ApplyDamageVariance(player, scaledDamage);
                bool crit = RollCrit(player, DamageClass.Magic);
                target.SimpleStrikeNPC(finalDamage, player.direction, crit, 3f, DamageClass.Magic);
                
                PlayEffects(player, target);
            }
        }

        private void PlayEffects(Player player, NPC target)
        {
            SoundEngine.PlaySound(SoundID.Item8, player.position);
            
            // 어둠 화살 궤적
            Vector2 direction = (target.Center - player.Center).SafeNormalize(Vector2.Zero);
            float distance = Vector2.Distance(player.Center, target.Center);
            
            for (int i = 0; i < (int)(distance / 15); i++)
            {
                Vector2 pos = Vector2.Lerp(player.Center, target.Center, (float)i / (distance / 15));
                Dust dust = Dust.NewDustDirect(pos, 4, 4, DustID.Shadowflame,
                    direction.X * 2f, direction.Y * 2f, 100, Color.Purple, 1.0f);
                dust.noGravity = true;
            }
        }
    }
}
