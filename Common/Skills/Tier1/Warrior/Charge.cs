using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using RpgMod.Common.Base;

namespace RpgMod.Common.Skills.Tier1.Warrior
{
    /// <summary>
    /// Charge - 전방으로 돌진하여 적에게 피해를 입히고 밀쳐낸다.
    /// 전사의 진입/추격 스킬.
    /// </summary>
    public class Charge : BaseSkill
    {
        public override string InternalName => "Charge";
        public override string DisplayName => "Charge";
        public override string Description => "Rush forward, damaging and knocking back enemies in your path.";

        public override SkillType SkillType => SkillType.Active;
        public override JobType RequiredJob => JobType.Warrior;
        public override int RequiredLevel => 18;
        public override int SkillPointCost => 1;
        public override int MaxRank => 10;
        public override string IconTexture => $"{RpgConstants.ICON_PATH}Skills/Charge";
        
        public override float CooldownSeconds => 8f - (CurrentRank * 0.3f); // 8 -> 5초
        public override int ResourceCost => 30 - CurrentRank; // 30 -> 20
        public override ResourceType ResourceType => ResourceType.Stamina;
        
        // 전제조건: PowerStrike 5랭크 이상 필요
        public override string[] PrerequisiteSkills => new[] { "PowerStrike:5" };

        // 돌진 거리 (픽셀)
        private static readonly float[] CHARGE_DISTANCE = { 200f, 230f, 260f, 290f, 320f, 350f, 380f, 410f, 440f, 500f };
        private static readonly float[] DAMAGE_MULTIPLIER = { 1.0f, 1.1f, 1.2f, 1.3f, 1.4f, 1.5f, 1.6f, 1.7f, 1.8f, 2.0f };

        protected override void OnActivate(Player player)
        {
            int rank = System.Math.Max(1, CurrentRank);
            float distance = CHARGE_DISTANCE[rank - 1];
            float multiplier = DAMAGE_MULTIPLIER[rank - 1];
            
            // 돌진
            Vector2 direction = player.direction == 1 ? Vector2.UnitX : -Vector2.UnitX;
            player.velocity = direction * 25f;
            
            // 잠시 무적 (iframes)
            player.immune = true;
            player.immuneTime = 15;
            
            // 경로에 있는 적 피해
            int baseDamage = (int)(50 * multiplier);
            Rectangle chargeHitbox = new Rectangle(
                (int)(player.Center.X - 50),
                (int)(player.Center.Y - 30),
                (int)(distance + 100),
                60);
            
            if (player.direction == -1)
                chargeHitbox.X -= (int)distance;
            
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC npc = Main.npc[i];
                if (npc.active && !npc.friendly && npc.Hitbox.Intersects(chargeHitbox))
                {
                    float scaledDamage = GetScaledDamage(player, DamageClass.Melee, baseDamage);
                    int finalDamage = ApplyDamageVariance(player, scaledDamage);
                    bool crit = RollCrit(player, DamageClass.Melee);
                    npc.SimpleStrikeNPC(finalDamage, player.direction, crit, 10f, DamageClass.Melee);
                }
            }
            
            PlayEffects(player, direction);
        }

        private void PlayEffects(Player player, Vector2 direction)
        {
            SoundEngine.PlaySound(SoundID.Item74, player.position);
            
            // 돌진 트레일
            for (int i = 0; i < 30; i++)
            {
                Vector2 dustPos = player.Center - direction * Main.rand.NextFloat(50);
                Dust dust = Dust.NewDustDirect(dustPos, 4, 4, DustID.Smoke,
                    -direction.X * 3f, Main.rand.NextFloat(-1f, 1f), 150, Color.Gray, 1.5f);
                dust.noGravity = true;
            }
        }
    }
}
