using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using RpgMod.Common.Base;

namespace RpgMod.Common.Skills.Tier2.Berserker
{
    /// <summary>
    /// Raging Strike - 분노의 일격.
    /// 강력한 공격 + 잃은 HP 비례 추가 피해.
    /// 광전사의 주력기.
    /// </summary>
    public class RagingStrike : BaseSkill
    {
        public override string InternalName => "RagingStrike";
        public override string DisplayName => "Raging Strike";
        public override string Description => "A powerful strike that deals bonus damage based on missing health.";

        public override SkillType SkillType => SkillType.Active;
        public override JobType RequiredJob => JobType.Berserker;
        public override int RequiredLevel => 63;
        public override int SkillPointCost => 1;
        public override int MaxRank => 10;
        public override string IconTexture => $"{RpgConstants.ICON_PATH}Skills/RagingStrike";
        
        public override float CooldownSeconds => 6f - (CurrentRank * 0.2f); // 6 -> 4초
        public override int ResourceCost => 25;
        public override ResourceType ResourceType => ResourceType.Stamina;

        private static readonly int[] BASE_DAMAGE = { 80, 100, 120, 145, 170, 200, 230, 265, 300, 360 };

        protected override void OnActivate(Player player)
        {
            int rank = System.Math.Max(1, CurrentRank);
            int baseDamage = BASE_DAMAGE[rank - 1];
            
            // 잃은 HP 비례 추가 피해 (최대 +100%)
            float missingHpPercent = 1f - ((float)player.statLife / player.statLifeMax2);
            float bonusMult = 1f + missingHpPercent;
            
            int finalBaseDamage = (int)(baseDamage * bonusMult);
            
            Vector2 direction = player.direction == 1 ? Vector2.UnitX : -Vector2.UnitX;
            Rectangle hitbox = new Rectangle(
                (int)(player.Center.X + direction.X * 30 - 60),
                (int)(player.Center.Y - 50),
                120, 100);
            
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC npc = Main.npc[i];
                if (npc.active && !npc.friendly && npc.Hitbox.Intersects(hitbox))
                {
                    float scaledDamage = GetScaledDamage(player, DamageClass.Melee, finalBaseDamage);
                    int finalDamage = ApplyDamageVariance(player, scaledDamage);
                    bool crit = RollCrit(player, DamageClass.Melee);
                    npc.SimpleStrikeNPC(finalDamage, player.direction, crit, 6f, DamageClass.Melee);
                }
            }
            
            PlayEffects(player, direction, missingHpPercent);
        }

        private void PlayEffects(Player player, Vector2 direction, float rage)
        {
            SoundEngine.PlaySound(SoundID.Item71, player.position);
            
            int dustCount = 20 + (int)(rage * 30);
            for (int i = 0; i < dustCount; i++)
            {
                Vector2 dustPos = player.Center + direction * (30 + Main.rand.NextFloat(50));
                Dust dust = Dust.NewDustDirect(dustPos, 4, 4, DustID.Blood,
                    direction.X * 5f, Main.rand.NextFloat(-3f, 3f), 100, Color.DarkRed, 1.3f + rage * 0.5f);
                dust.noGravity = true;
            }
        }
    }
}
