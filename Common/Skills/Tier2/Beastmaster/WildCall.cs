using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using RpgMod.Common.Base;

namespace RpgMod.Common.Skills.Tier2.Beastmaster
{
    /// <summary>
    /// Wild Call - 야생의 부름.
    /// 일시적으로 미니언 소환.
    /// 야수조련사의 소환 공격기.
    /// </summary>
    public class WildCall : BaseSkill
    {
        public override string InternalName => "WildCall";
        public override string DisplayName => "Wild Call";
        public override string Description => "Call wild beasts to fight for you temporarily.";

        public override SkillType SkillType => SkillType.Active;
        public override JobType RequiredJob => JobType.Beastmaster;
        public override int RequiredLevel => 60;
        public override int SkillPointCost => 1;
        public override int MaxRank => 10;
        public override string IconTexture => $"{RpgConstants.ICON_PATH}Skills/WildCall";
        
        public override float CooldownSeconds => 20f - (CurrentRank * 0.5f); // 20 -> 15초
        public override int ResourceCost => 35;
        public override ResourceType ResourceType => ResourceType.Mana;

        private static readonly int[] BEAST_COUNT = { 1, 1, 2, 2, 2, 3, 3, 3, 4, 5 };
        private static readonly int[] DAMAGE = { 35, 45, 55, 65, 80, 95, 115, 135, 160, 200 };

        protected override void OnActivate(Player player)
        {
            int rank = System.Math.Max(1, CurrentRank);
            int count = BEAST_COUNT[rank - 1];
            int damage = DAMAGE[rank - 1];
            
            for (int i = 0; i < count; i++)
            {
                Vector2 offset = new Vector2(Main.rand.NextFloat(-50, 50), -20);
                int projId = Projectile.NewProjectile(player.GetSource_FromThis(), player.Center + offset, Vector2.Zero,
                    ProjectileID.Raven, damage, 2f, player.whoAmI);
                if (projId >= 0 && projId < Main.maxProjectiles)
                {
                    Projectile proj = Main.projectile[projId];
                    proj.DamageType = DamageClass.Summon;
                    proj.ContinuouslyUpdateDamageStats = true;
                }
            }
            
            PlayEffects(player);
        }

        private void PlayEffects(Player player)
        {
            SoundEngine.PlaySound(SoundID.NPCDeath5, player.position);
            
            for (int i = 0; i < 20; i++)
            {
                Dust dust = Dust.NewDustDirect(player.position, player.width, player.height,
                    DustID.GreenFairy, Main.rand.NextFloat(-3f, 3f), Main.rand.NextFloat(-3f, 0f), 150, Color.Green, 1.2f);
                dust.noGravity = true;
            }
        }
    }
}
