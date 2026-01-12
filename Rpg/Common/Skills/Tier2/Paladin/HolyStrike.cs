using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using Rpg.Common.Base;

namespace Rpg.Common.Skills.Tier2.Paladin
{
    /// <summary>
    /// Holy Strike - 성스러운 일격.
    /// 신성한 피해를 입히며 언데드에게 추가 피해.
    /// </summary>
    public class HolyStrike : BaseSkill
    {
        public override string InternalName => "HolyStrike";
        public override string DisplayName => "Holy Strike";
        public override string Description => "Strike with holy power. Deals bonus damage to undead enemies.";

        public override SkillType SkillType => SkillType.Active;
        public override JobType RequiredJob => JobType.Paladin;
        public override int RequiredLevel => 64;
        public override int SkillPointCost => 1;
        public override int MaxRank => 10;
        public override string IconTexture => $"{RpgConstants.ICON_PATH}Skills/HolyStrike";
        
        public override float CooldownSeconds => 6f - (CurrentRank * 0.2f);
        public override int ResourceCost => 30;
        public override ResourceType ResourceType => ResourceType.Mana;

        private static readonly int[] DAMAGE = { 70, 90, 115, 140, 170, 200, 235, 270, 310, 380 };

        protected override void OnActivate(Player player)
        {
            int rank = System.Math.Max(1, CurrentRank);
            
            Vector2 direction = player.direction == 1 ? Vector2.UnitX : -Vector2.UnitX;
            Rectangle hitbox = new Rectangle(
                (int)(player.Center.X + direction.X * 20 - 50),
                (int)(player.Center.Y - 40),
                100, 80);
            
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC npc = Main.npc[i];
                if (npc.active && !npc.friendly && npc.Hitbox.Intersects(hitbox))
                {
                    int damage = Main.DamageVar(DAMAGE[rank - 1], player.luck);
                    
                    // 언데드에게 추가 피해
                    if (NPCID.Sets.Zombies[npc.type] || npc.type == NPCID.Skeleton || 
                        npc.type == NPCID.SkeletronHead || npc.type == NPCID.SkeletronPrime)
                    {
                        damage = (int)(damage * 1.5f);
                    }
                    
                    npc.SimpleStrikeNPC(damage, player.direction, true, 6f, DamageClass.Melee, true);
                }
            }
            
            PlayEffects(player, direction);
        }

        private void PlayEffects(Player player, Vector2 direction)
        {
            SoundEngine.PlaySound(SoundID.Item29, player.position);
            
            for (int i = 0; i < 25; i++)
            {
                Vector2 dustPos = player.Center + direction * (30 + Main.rand.NextFloat(40));
                Dust dust = Dust.NewDustDirect(dustPos, 4, 4, DustID.HallowedTorch,
                    direction.X * 4f, Main.rand.NextFloat(-2f, 2f), 100, Color.Gold, 1.3f);
                dust.noGravity = true;
            }
        }
    }
}
