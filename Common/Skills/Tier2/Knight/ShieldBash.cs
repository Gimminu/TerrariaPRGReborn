using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using RpgMod.Common.Base;

namespace RpgMod.Common.Skills.Tier2.Knight
{
    /// <summary>
    /// Shield Bash - 방패 강타.
    /// 방패로 적을 내리쳐 피해를 주고 기절시킨다.
    /// </summary>
    public class ShieldBash : BaseSkill
    {
        public override string InternalName => "ShieldBash";
        public override string DisplayName => "Shield Bash";
        public override string Description => "Bash enemies with your shield, dealing damage and stunning them.";

        public override SkillType SkillType => SkillType.Active;
        public override JobType RequiredJob => JobType.Knight;
        public override int RequiredLevel => 63;
        public override int SkillPointCost => 1;
        public override int MaxRank => 10;
        public override string IconTexture => $"{RpgConstants.ICON_PATH}Skills/ShieldBash";
        
        public override float CooldownSeconds => 8f - (CurrentRank * 0.3f);
        public override int ResourceCost => 30;
        public override ResourceType ResourceType => ResourceType.Stamina;

        private static readonly int[] DAMAGE = { 60, 80, 100, 125, 150, 180, 210, 245, 280, 350 };
        private static readonly int[] STUN_FRAMES = { 30, 35, 40, 45, 50, 55, 60, 70, 80, 100 };

        protected override void OnActivate(Player player)
        {
            int rank = System.Math.Max(1, CurrentRank);
            
            Vector2 direction = player.direction == 1 ? Vector2.UnitX : -Vector2.UnitX;
            Rectangle hitbox = new Rectangle(
                (int)(player.Center.X + direction.X * 20 - 40),
                (int)(player.Center.Y - 30),
                80, 60);
            
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC npc = Main.npc[i];
                if (npc.active && !npc.friendly && npc.Hitbox.Intersects(hitbox))
                {
                    float scaledDamage = GetScaledDamage(player, DamageClass.Melee, DAMAGE[rank - 1]);
                    int damage = ApplyDamageVariance(player, scaledDamage);
                    bool crit = RollCrit(player, DamageClass.Melee);
                    npc.SimpleStrikeNPC(damage, player.direction, crit, 8f, DamageClass.Melee);
                    
                    // 기절 (Confused 버프로 대체)
                    npc.AddBuff(BuffID.Confused, STUN_FRAMES[rank - 1]);
                }
            }
            
            PlayEffects(player, direction);
        }

        private void PlayEffects(Player player, Vector2 direction)
        {
            SoundEngine.PlaySound(SoundID.Item37, player.position);
            
            for (int i = 0; i < 20; i++)
            {
                Vector2 dustPos = player.Center + direction * 40;
                Dust dust = Dust.NewDustDirect(dustPos, 4, 4, DustID.Iron,
                    direction.X * 3f + Main.rand.NextFloat(-2f, 2f), Main.rand.NextFloat(-2f, 2f), 150, Color.Gray, 1.2f);
                dust.noGravity = true;
            }
        }
    }
}
