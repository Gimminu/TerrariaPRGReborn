using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using RpgMod.Common.Base;

namespace RpgMod.Common.Skills.Tier3.Deadeye
{
    /// <summary>
    /// Perfect Shot - 완벽한 사격.
    /// 초고피해 단일 타겟 사격.
    /// 데드아이의 궁극 공격기.
    /// </summary>
    public class PerfectShot : BaseSkill
    {
        public override string InternalName => "PerfectShot";
        public override string DisplayName => "Perfect Shot";
        public override string Description => "Fire a perfectly aimed shot that deals massive damage.";

        public override SkillType SkillType => SkillType.Active;
        public override JobType RequiredJob => JobType.Deadeye;
        public override int RequiredLevel => 120;
        public override int SkillPointCost => 1;
        public override int MaxRank => 10;
        public override string IconTexture => $"{RpgConstants.ICON_PATH}Skills/PerfectShot";
        
        public override float CooldownSeconds => 15f - (CurrentRank * 0.5f); // 15 -> 10초
        public override int ResourceCost => 45;
        public override ResourceType ResourceType => ResourceType.Stamina;

        private static readonly int[] DAMAGE = { 250, 315, 390, 480, 580, 695, 825, 975, 1150, 1400 };

        protected override void OnActivate(Player player)
        {
            int rank = System.Math.Max(1, CurrentRank);
            int damage = DAMAGE[rank - 1];
            
            Vector2 direction = (Main.MouseWorld - player.Center).SafeNormalize(Vector2.UnitX);
            
            // 초고속 관통탄
            int projId = Projectile.NewProjectile(player.GetSource_FromThis(), player.Center, direction * 30f,
                ProjectileID.BulletHighVelocity, damage, 10f, player.whoAmI);
            if (projId >= 0 && projId < Main.maxProjectiles)
            {
                Main.projectile[projId].DamageType = DamageClass.Ranged;
            }
            
            PlayEffects(player, direction);
        }

        private void PlayEffects(Player player, Vector2 direction)
        {
            SoundEngine.PlaySound(SoundID.Item40 with { Pitch = 0.5f }, player.position);
            
            for (int i = 0; i < 20; i++)
            {
                Dust dust = Dust.NewDustDirect(player.Center, 8, 8, DustID.Torch,
                    direction.X * 8f + Main.rand.NextFloat(-2f, 2f), 
                    direction.Y * 8f + Main.rand.NextFloat(-2f, 2f), 
                    150, Color.Orange, 1.5f);
                dust.noGravity = true;
            }
        }
    }
}
