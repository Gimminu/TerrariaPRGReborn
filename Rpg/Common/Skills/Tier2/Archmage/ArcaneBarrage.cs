using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using Rpg.Common.Base;

namespace Rpg.Common.Skills.Tier2.Archmage
{
    /// <summary>
    /// Arcane Barrage - 신비탄막.
    /// 다수의 마법탄 발사.
    /// 대마법사의 광역 공격기.
    /// </summary>
    public class ArcaneBarrage : BaseSkill
    {
        public override string InternalName => "ArcaneBarrage";
        public override string DisplayName => "Arcane Barrage";
        public override string Description => "Fire a barrage of arcane missiles.";

        public override SkillType SkillType => SkillType.Active;
        public override JobType RequiredJob => JobType.Archmage;
        public override int RequiredLevel => 60;
        public override int SkillPointCost => 1;
        public override int MaxRank => 10;
        public override string IconTexture => $"{RpgConstants.ICON_PATH}Skills/ArcaneBarrage";
        
        public override float CooldownSeconds => 8f - (CurrentRank * 0.25f); // 8 -> 5.5초
        public override int ResourceCost => 40;
        public override ResourceType ResourceType => ResourceType.Mana;

        private static readonly int[] MISSILE_COUNT = { 4, 5, 5, 6, 6, 7, 7, 8, 9, 12 };
        private static readonly int[] DAMAGE = { 45, 55, 65, 78, 92, 108, 125, 145, 170, 210 };

        protected override void OnActivate(Player player)
        {
            int rank = System.Math.Max(1, CurrentRank);
            int count = MISSILE_COUNT[rank - 1];
            int damage = DAMAGE[rank - 1];
            
            Vector2 direction = (Main.MouseWorld - player.Center).SafeNormalize(Vector2.UnitX);
            
            for (int i = 0; i < count; i++)
            {
                float spread = Main.rand.NextFloat(-0.3f, 0.3f);
                Vector2 shotDir = direction.RotatedBy(spread);
                float speed = 12f + Main.rand.NextFloat(-2f, 2f);
                
                Projectile.NewProjectile(player.GetSource_FromThis(), player.Center, shotDir * speed,
                    ProjectileID.NebulaBlaze1, damage, 2f, player.whoAmI);
            }
            
            PlayEffects(player);
        }

        private void PlayEffects(Player player)
        {
            SoundEngine.PlaySound(SoundID.Item125, player.position);
            
            for (int i = 0; i < 15; i++)
            {
                Dust dust = Dust.NewDustDirect(player.Center, 8, 8, DustID.PinkFairy,
                    Main.rand.NextFloat(-3f, 3f), Main.rand.NextFloat(-3f, 3f), 100, Color.MediumPurple, 1.2f);
                dust.noGravity = true;
            }
        }
    }
}
