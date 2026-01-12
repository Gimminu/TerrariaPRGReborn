using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using Rpg.Common.Base;

namespace Rpg.Common.Skills.Tier2.Necromancer
{
    /// <summary>
    /// Death Coil - 죽음의 코일.
    /// 피해를 주고 체력 회복.
    /// 강령술사의 공격+회복기.
    /// </summary>
    public class DeathCoil : BaseSkill
    {
        public override string InternalName => "NecromancerDeathCoil";
        public override string DisplayName => "Death Coil";
        public override string Description => "Fire a coil of death energy that damages enemies and heals you.";

        public override SkillType SkillType => SkillType.Active;
        public override JobType RequiredJob => JobType.Necromancer;
        public override int RequiredLevel => 68;
        public override int SkillPointCost => 1;
        public override int MaxRank => 10;
        public override string IconTexture => $"{RpgConstants.ICON_PATH}Skills/DeathCoil";
        
        public override float CooldownSeconds => 8f - (CurrentRank * 0.25f); // 8 -> 5.5초
        public override int ResourceCost => 30;
        public override ResourceType ResourceType => ResourceType.Mana;

        private static readonly int[] DAMAGE = { 60, 75, 95, 115, 140, 165, 195, 230, 270, 330 };
        private static readonly int[] HEAL = { 15, 20, 25, 30, 38, 46, 55, 66, 80, 100 };

        protected override void OnActivate(Player player)
        {
            int rank = System.Math.Max(1, CurrentRank);
            int damage = DAMAGE[rank - 1];
            int heal = HEAL[rank - 1];
            
            Vector2 direction = (Main.MouseWorld - player.Center).SafeNormalize(Vector2.UnitX);
            
            // 죽음의 탄환
            Projectile.NewProjectile(player.GetSource_FromThis(), player.Center, direction * 12f,
                ProjectileID.VampireKnife, damage, 3f, player.whoAmI);
            
            // 즉시 회복
            player.Heal(heal);
            
            PlayEffects(player, direction);
        }

        private void PlayEffects(Player player, Vector2 direction)
        {
            SoundEngine.PlaySound(SoundID.Item8, player.position);
            
            for (int i = 0; i < 12; i++)
            {
                Dust dust = Dust.NewDustDirect(player.Center, 4, 4, DustID.Shadowflame,
                    direction.X * 3f, direction.Y * 3f, 150, Color.DarkGreen, 1.2f);
                dust.noGravity = true;
            }
        }
    }
}
