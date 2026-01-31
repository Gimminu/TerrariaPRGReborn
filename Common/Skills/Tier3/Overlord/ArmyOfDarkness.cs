using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using RpgMod.Common.Base;

namespace RpgMod.Common.Skills.Tier3.Overlord
{
    /// <summary>
    /// Army of Darkness - 어둠의 군대.
    /// 대량의 소환수 일시 소환.
    /// 오버로드의 궁극기.
    /// </summary>
    public class ArmyOfDarkness : BaseSkill
    {
        public override string InternalName => "ArmyOfDarkness";
        public override string DisplayName => "Army of Darkness";
        public override string Description => "Summon an army of dark minions to overwhelm enemies.";

        public override SkillType SkillType => SkillType.Active;
        public override JobType RequiredJob => JobType.Overlord;
        public override int RequiredLevel => 120;
        public override int SkillPointCost => 1;
        public override int MaxRank => 10;
        public override string IconTexture => $"{RpgConstants.ICON_PATH}Skills/ArmyOfDarkness";
        
        public override float CooldownSeconds => 25f - (CurrentRank * 0.8f); // 25 -> 17초
        public override int ResourceCost => 60;
        public override ResourceType ResourceType => ResourceType.Mana;

        private static readonly int[] MINION_COUNT = { 4, 5, 6, 7, 8, 10, 12, 14, 17, 22 };
        private static readonly int[] DAMAGE = { 55, 70, 88, 108, 132, 160, 195, 235, 285, 360 };

        protected override void OnActivate(Player player)
        {
            int rank = System.Math.Max(1, CurrentRank);
            int count = MINION_COUNT[rank - 1];
            int damage = DAMAGE[rank - 1];
            
            for (int i = 0; i < count; i++)
            {
                Vector2 offset = new Vector2(Main.rand.NextFloat(-80, 80), -20 - Main.rand.NextFloat(20));
                int projType = Main.rand.NextBool() ? ProjectileID.Raven : ProjectileID.BabySlime;
                
                int projId = Projectile.NewProjectile(player.GetSource_FromThis(), player.Center + offset, Vector2.Zero,
                    projType, damage, 2f, player.whoAmI);
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
            SoundEngine.PlaySound(SoundID.NPCDeath10, player.position);
            
            for (int i = 0; i < 40; i++)
            {
                Dust dust = Dust.NewDustDirect(player.position, player.width, player.height,
                    DustID.Shadowflame, Main.rand.NextFloat(-4f, 4f), Main.rand.NextFloat(-4f, 0f), 200, Color.DarkViolet, 1.4f);
                dust.noGravity = true;
            }
        }
    }
}
