using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using Rpg.Common.Base;

namespace Rpg.Common.Skills.Tier2.Warlock
{
    /// <summary>
    /// Dark Bolt - 암흑 화살.
    /// 암흑 피해 공격.
    /// 흑마법사의 기본 공격기.
    /// </summary>
    public class DarkBolt : BaseSkill
    {
        public override string InternalName => "DarkBolt";
        public override string DisplayName => "Dark Bolt";
        public override string Description => "Fire a bolt of dark energy.";

        public override SkillType SkillType => SkillType.Active;
        public override JobType RequiredJob => JobType.Warlock;
        public override int RequiredLevel => 60;
        public override int SkillPointCost => 1;
        public override int MaxRank => 10;
        public override string IconTexture => $"{RpgConstants.ICON_PATH}Skills/DarkBolt";
        
        public override float CooldownSeconds => 4f - (CurrentRank * 0.15f); // 4 -> 2.5초
        public override int ResourceCost => 20;
        public override ResourceType ResourceType => ResourceType.Mana;

        private static readonly int[] DAMAGE = { 65, 80, 100, 120, 145, 175, 205, 240, 285, 350 };

        protected override void OnActivate(Player player)
        {
            int rank = System.Math.Max(1, CurrentRank);
            int damage = DAMAGE[rank - 1];
            
            Vector2 direction = (Main.MouseWorld - player.Center).SafeNormalize(Vector2.UnitX);
            
            Projectile.NewProjectile(player.GetSource_FromThis(), player.Center, direction * 14f,
                ProjectileID.ShadowBeamFriendly, damage, 4f, player.whoAmI);
            
            PlayEffects(player, direction);
        }

        private void PlayEffects(Player player, Vector2 direction)
        {
            SoundEngine.PlaySound(SoundID.Item8, player.position);
            
            for (int i = 0; i < 10; i++)
            {
                Dust dust = Dust.NewDustDirect(player.Center, 4, 4, DustID.Shadowflame,
                    direction.X * 3f, direction.Y * 3f, 150, Color.Purple, 1.2f);
                dust.noGravity = true;
            }
        }
    }
}
