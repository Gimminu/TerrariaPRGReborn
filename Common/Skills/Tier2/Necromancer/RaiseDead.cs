using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using RpgMod.Common.Base;

namespace RpgMod.Common.Skills.Tier2.Necromancer
{
    /// <summary>
    /// Raise Dead - 죽은 자를 일으켜라.
    /// 언데드 소환수 소환.
    /// 강령술사의 소환 공격기.
    /// </summary>
    public class RaiseDead : BaseSkill
    {
        public override string InternalName => "RaiseDead";
        public override string DisplayName => "Raise Dead";
        public override string Description => "Raise skeletal minions to fight for you.";

        public override SkillType SkillType => SkillType.Active;
        public override JobType RequiredJob => JobType.Necromancer;
        public override int RequiredLevel => 60;
        public override int SkillPointCost => 1;
        public override int MaxRank => 10;
        public override string IconTexture => $"{RpgConstants.ICON_PATH}Skills/RaiseDead";
        
        public override float CooldownSeconds => 18f - (CurrentRank * 0.5f); // 18 -> 13초
        public override int ResourceCost => 40;
        public override ResourceType ResourceType => ResourceType.Mana;

        private static readonly int[] SKELETON_COUNT = { 2, 2, 3, 3, 4, 4, 5, 5, 6, 8 };
        private static readonly int[] DAMAGE = { 30, 40, 50, 60, 75, 90, 108, 130, 155, 195 };

        protected override void OnActivate(Player player)
        {
            int rank = System.Math.Max(1, CurrentRank);
            int count = SKELETON_COUNT[rank - 1];
            int damage = DAMAGE[rank - 1];
            
            for (int i = 0; i < count; i++)
            {
                Vector2 offset = new Vector2(Main.rand.NextFloat(-60, 60), -10);
                int projId = Projectile.NewProjectile(player.GetSource_FromThis(), player.Center + offset, Vector2.Zero,
                    ProjectileID.BabySlime, damage, 2f, player.whoAmI);
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
            SoundEngine.PlaySound(SoundID.NPCDeath2, player.position);
            
            for (int i = 0; i < 25; i++)
            {
                Dust dust = Dust.NewDustDirect(player.position, player.width, player.height,
                    DustID.Bone, Main.rand.NextFloat(-3f, 3f), Main.rand.NextFloat(-3f, 0f), 150, Color.White, 1.2f);
                dust.noGravity = true;
            }
        }
    }
}
