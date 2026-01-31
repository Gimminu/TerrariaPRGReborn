using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using RpgMod.Common.Base;

namespace RpgMod.Common.Skills.Tier2.Sorcerer
{
    /// <summary>
    /// Meteor Strike - 운석 충돌.
    /// 하늘에서 운석을 떨어뜨림.
    /// 소서러의 필살기.
    /// </summary>
    public class MeteorStrike : BaseSkill
    {
        public override string InternalName => "MeteorStrike";
        public override string DisplayName => "Meteor Strike";
        public override string Description => "Call down a devastating meteor from the sky.";

        public override SkillType SkillType => SkillType.Active;
        public override JobType RequiredJob => JobType.Sorcerer;
        public override int RequiredLevel => 72;
        public override int SkillPointCost => 1;
        public override int MaxRank => 10;
        public override string IconTexture => $"{RpgConstants.ICON_PATH}Skills/MeteorStrike";
        
        public override float CooldownSeconds => 15f - (CurrentRank * 0.5f); // 15 -> 10초
        public override int ResourceCost => 50;
        public override ResourceType ResourceType => ResourceType.Mana;

        private static readonly int[] DAMAGE = { 150, 185, 225, 270, 320, 375, 435, 500, 575, 680 };

        protected override void OnActivate(Player player)
        {
            int rank = System.Math.Max(1, CurrentRank);
            int damage = DAMAGE[rank - 1];
            
            // 마우스 위치 상공에서 운석
            Vector2 spawnPos = Main.MouseWorld - new Vector2(0, 600);
            Vector2 direction = new Vector2(Main.rand.NextFloat(-1f, 1f), 5f).SafeNormalize(Vector2.UnitY);
            
            int mainProjId = Projectile.NewProjectile(player.GetSource_FromThis(), spawnPos, direction * 15f,
                ProjectileID.Meteor1, damage, 8f, player.whoAmI);
            if (mainProjId >= 0 && mainProjId < Main.maxProjectiles)
            {
                Main.projectile[mainProjId].DamageType = DamageClass.Magic;
            }
            
            // 추가 작은 운석들
            int smallDamage = damage / 3;
            for (int i = 0; i < 2 + rank / 3; i++)
            {
                Vector2 offset = new Vector2(Main.rand.NextFloat(-100, 100), Main.rand.NextFloat(-50, 0));
                int projId = Projectile.NewProjectile(player.GetSource_FromThis(), spawnPos + offset, direction * 12f,
                    ProjectileID.Meteor2, smallDamage, 3f, player.whoAmI);
                if (projId >= 0 && projId < Main.maxProjectiles)
                {
                    Main.projectile[projId].DamageType = DamageClass.Magic;
                }
            }
            
            PlayEffects(player);
        }

        private void PlayEffects(Player player)
        {
            SoundEngine.PlaySound(SoundID.Item88, player.position);
        }
    }
}
