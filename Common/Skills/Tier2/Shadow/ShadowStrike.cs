using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using RpgMod.Common.Base;

namespace RpgMod.Common.Skills.Tier2.Shadow
{
    /// <summary>
    /// Shadow Strike - 그림자 일격.
    /// 은밀한 강력 공격.
    /// 그림자의 기본 공격기.
    /// </summary>
    public class ShadowStrike : BaseSkill
    {
        public override string InternalName => "ShadowStrike";
        public override string DisplayName => "Shadow Strike";
        public override string Description => "Strike from the shadows with deadly precision.";

        public override SkillType SkillType => SkillType.Active;
        public override JobType RequiredJob => JobType.Shadow;
        public override int RequiredLevel => 60;
        public override int SkillPointCost => 1;
        public override int MaxRank => 10;
        public override string IconTexture => $"{RpgConstants.ICON_PATH}Skills/ShadowStrike";
        
        public override float CooldownSeconds => 5f - (CurrentRank * 0.15f); // 5 -> 3.5초
        public override int ResourceCost => 20;
        public override ResourceType ResourceType => ResourceType.Stamina;

        private static readonly int[] DAMAGE = { 90, 115, 140, 170, 205, 245, 290, 345, 410, 500 };

        protected override void OnActivate(Player player)
        {
            int rank = System.Math.Max(1, CurrentRank);
            int damage = DAMAGE[rank - 1];
            
            // 은신 상태면 보너스
            bool invisible = player.HasBuff(BuffID.Invisibility);
            if (invisible) damage = (int)(damage * 1.5f);
            
            Vector2 direction = player.direction == 1 ? Vector2.UnitX : -Vector2.UnitX;
            Rectangle hitbox = new Rectangle(
                (int)(player.Center.X + direction.X * 20 - 50),
                (int)(player.Center.Y - 40),
                100, 80);
            
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC npc = Main.npc[i];
                if (npc.active && !npc.friendly && npc.Hitbox.Intersects(hitbox))
                {
                    float scaledDamage = GetScaledDamage(player, DamageClass.Melee, damage);
                    int finalDamage = ApplyDamageVariance(player, scaledDamage);
                    bool crit = RollCrit(player, DamageClass.Melee);
                    npc.SimpleStrikeNPC(finalDamage, player.direction, crit, 4f, DamageClass.Melee);
                    npc.AddBuff(BuffID.Bleeding, 180 + rank * 20);
                }
            }
            
            if (invisible) player.ClearBuff(BuffID.Invisibility);
            PlayEffects(player, direction);
        }

        private void PlayEffects(Player player, Vector2 direction)
        {
            SoundEngine.PlaySound(SoundID.Item71 with { Pitch = 0.3f }, player.position);
            
            for (int i = 0; i < 20; i++)
            {
                Dust dust = Dust.NewDustDirect(player.Center + direction * Main.rand.NextFloat(50), 4, 4,
                    DustID.Shadowflame, direction.X * 4f, Main.rand.NextFloat(-2f, 2f), 150, Color.DarkViolet, 1.2f);
                dust.noGravity = true;
            }
        }
    }
}
