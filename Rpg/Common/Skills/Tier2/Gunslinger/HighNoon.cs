using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using Rpg.Common.Base;

namespace Rpg.Common.Skills.Tier2.Gunslinger
{
    /// <summary>
    /// High Noon - 정오의 결투.
    /// 강력한 단일 타겟 사격.
    /// 건슬링거의 필살기.
    /// </summary>
    public class HighNoon : BaseSkill
    {
        public override string InternalName => "HighNoon";
        public override string DisplayName => "High Noon";
        public override string Description => "A deadly precise shot that deals massive damage to a single target.";

        public override SkillType SkillType => SkillType.Active;
        public override JobType RequiredJob => JobType.Gunslinger;
        public override int RequiredLevel => 68;
        public override int SkillPointCost => 1;
        public override int MaxRank => 10;
        public override string IconTexture => $"{RpgConstants.ICON_PATH}Skills/HighNoon";
        
        public override float CooldownSeconds => 12f - (CurrentRank * 0.4f); // 12 -> 8초
        public override int ResourceCost => 35;
        public override ResourceType ResourceType => ResourceType.Stamina;

        private static readonly int[] DAMAGE = { 150, 180, 215, 255, 300, 350, 400, 460, 530, 620 };

        protected override void OnActivate(Player player)
        {
            int rank = System.Math.Max(1, CurrentRank);
            int damage = DAMAGE[rank - 1];
            
            Vector2 direction = (Main.MouseWorld - player.Center).SafeNormalize(Vector2.UnitX);
            
            // 고속 관통 총알
            Projectile.NewProjectile(player.GetSource_FromThis(), player.Center, direction * 25f,
                ProjectileID.BulletHighVelocity, damage, 6f, player.whoAmI);
            
            PlayEffects(player, direction);
        }

        private void PlayEffects(Player player, Vector2 direction)
        {
            SoundEngine.PlaySound(SoundID.Item40, player.position);
            
            for (int i = 0; i < 15; i++)
            {
                Vector2 dustVel = direction.RotatedByRandom(0.2f) * Main.rand.NextFloat(3f, 6f);
                Dust dust = Dust.NewDustDirect(player.Center, 4, 4, DustID.Torch,
                    dustVel.X, dustVel.Y, 150, Color.Orange, 1.5f);
                dust.noGravity = true;
            }
        }
    }
}
