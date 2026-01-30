using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using Rpg.Common.Base;

namespace Rpg.Common.Skills.Tier2.Spellblade
{
    /// <summary>
    /// Arcane Strike - 마력 일격.
    /// 마법과 근접 피해를 함께.
    /// 마검사의 기본 공격기.
    /// </summary>
    public class ArcaneStrike : BaseSkill
    {
        public override string InternalName => "ArcaneStrike";
        public override string DisplayName => "Arcane Strike";
        public override string Description => "Strike with a blade infused with arcane energy.";

        public override SkillType SkillType => SkillType.Active;
        public override JobType RequiredJob => JobType.Spellblade;
        public override int RequiredLevel => 60;
        public override int SkillPointCost => 1;
        public override int MaxRank => 10;
        public override string IconTexture => $"{RpgConstants.ICON_PATH}Skills/ArcaneStrike";
        
        public override float CooldownSeconds => 5f - (CurrentRank * 0.15f); // 5 -> 3.5초
        public override int ResourceCost => 20;
        public override ResourceType ResourceType => ResourceType.Mana;

        private static readonly int[] DAMAGE = { 80, 100, 125, 150, 180, 215, 255, 300, 355, 430 };

        protected override void OnActivate(Player player)
        {
            int rank = System.Math.Max(1, CurrentRank);
            int damage = DAMAGE[rank - 1];
            
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
                    int finalDamage = Main.DamageVar(damage, player.luck);
                    npc.SimpleStrikeNPC(finalDamage, player.direction, true, 4f, DamageClass.Melee, true);
                    
                    // 추가 마법 탄환
                    Vector2 toNpc = (npc.Center - player.Center).SafeNormalize(Vector2.UnitX);
                    Projectile.NewProjectile(player.GetSource_FromThis(), player.Center, toNpc * 10f,
                        ProjectileID.AmethystBolt, damage / 3, 1f, player.whoAmI);
                }
            }
            
            PlayEffects(player, direction);
        }

        private void PlayEffects(Player player, Vector2 direction)
        {
            SoundEngine.PlaySound(SoundID.Item71, player.position);
            SoundEngine.PlaySound(SoundID.Item8, player.position);
            
            for (int i = 0; i < 20; i++)
            {
                Vector2 dustPos = player.Center + direction * (20 + Main.rand.NextFloat(40));
                Dust dust = Dust.NewDustDirect(dustPos, 4, 4, DustID.PurpleCrystalShard,
                    direction.X * 3f, Main.rand.NextFloat(-2f, 2f), 150, Color.MediumPurple, 1.2f);
                dust.noGravity = true;
            }
        }
    }
}
