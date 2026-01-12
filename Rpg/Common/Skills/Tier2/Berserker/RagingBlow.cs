using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using Rpg.Common.Base;

namespace Rpg.Common.Skills.Tier2.Berserker
{
    /// <summary>
    /// Raging Blow - 격노의 일격.
    /// 체력이 낮을수록 피해가 증가하는 강타.
    /// </summary>
    public class RagingBlow : BaseSkill
    {
        public override string InternalName => "RagingBlow";
        public override string DisplayName => "Raging Blow";
        public override string Description => "Strike with fury. Deals more damage the lower your health.";

        public override SkillType SkillType => SkillType.Active;
        public override JobType RequiredJob => JobType.Berserker;
        public override int RequiredLevel => 63;
        public override int SkillPointCost => 1;
        public override int MaxRank => 10;
        public override string IconTexture => $"{RpgConstants.ICON_PATH}Skills/RagingBlow";
        
        public override float CooldownSeconds => 5f - (CurrentRank * 0.2f);
        public override int ResourceCost => 25;
        public override ResourceType ResourceType => ResourceType.Stamina;

        private static readonly int[] BASE_DAMAGE = { 80, 100, 120, 145, 170, 200, 230, 265, 300, 380 };

        protected override void OnActivate(Player player)
        {
            int rank = System.Math.Max(1, CurrentRank);
            
            // 체력이 낮을수록 피해 증가 (최대 2배)
            float healthRatio = (float)player.statLife / player.statLifeMax2;
            float damageMultiplier = 1f + (1f - healthRatio);
            
            Vector2 direction = player.direction == 1 ? Vector2.UnitX : -Vector2.UnitX;
            Rectangle hitbox = new Rectangle(
                (int)(player.Center.X + direction.X * 30 - 60),
                (int)(player.Center.Y - 40),
                120, 80);
            
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC npc = Main.npc[i];
                if (npc.active && !npc.friendly && npc.Hitbox.Intersects(hitbox))
                {
                    int damage = (int)(Main.DamageVar(BASE_DAMAGE[rank - 1], player.luck) * damageMultiplier);
                    npc.SimpleStrikeNPC(damage, player.direction, true, 7f, DamageClass.Melee, true);
                }
            }
            
            PlayEffects(player, direction);
        }

        private void PlayEffects(Player player, Vector2 direction)
        {
            SoundEngine.PlaySound(SoundID.Item71, player.position);
            
            for (int i = 0; i < 30; i++)
            {
                Vector2 dustPos = player.Center + direction * (40 + Main.rand.NextFloat(40));
                Dust dust = Dust.NewDustDirect(dustPos, 4, 4, DustID.Blood,
                    direction.X * 5f, Main.rand.NextFloat(-3f, 3f), 100, Color.DarkRed, 1.4f);
                dust.noGravity = true;
            }
        }
    }
}
