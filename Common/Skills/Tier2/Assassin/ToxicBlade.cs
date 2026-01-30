using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using Rpg.Common.Base;

namespace Rpg.Common.Skills.Tier2.Assassin
{
    /// <summary>
    /// Toxic Blade - 맹독 칼날.
    /// 독 피해를 입히는 공격.
    /// 암살자의 DoT 공격.
    /// </summary>
    public class ToxicBlade : BaseSkill
    {
        public override string InternalName => "ToxicBlade";
        public override string DisplayName => "Toxic Blade";
        public override string Description => "Strike with a poison-coated blade, dealing damage over time.";

        public override SkillType SkillType => SkillType.Active;
        public override JobType RequiredJob => JobType.Assassin;
        public override int RequiredLevel => 67;
        public override int SkillPointCost => 1;
        public override int MaxRank => 10;
        public override string IconTexture => $"{RpgConstants.ICON_PATH}Skills/ToxicBlade";
        
        public override float CooldownSeconds => 5f - (CurrentRank * 0.15f); // 5 -> 3.5초
        public override int ResourceCost => 20;
        public override ResourceType ResourceType => ResourceType.Stamina;

        private static readonly int[] DAMAGE = { 50, 65, 80, 95, 115, 135, 155, 180, 205, 250 };
        private static readonly int[] POISON_DURATION = { 180, 210, 240, 270, 300, 330, 360, 400, 450, 540 };

        protected override void OnActivate(Player player)
        {
            int rank = System.Math.Max(1, CurrentRank);
            int damage = DAMAGE[rank - 1];
            
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
                    int finalDamage = Main.DamageVar(damage, player.luck);
                    npc.SimpleStrikeNPC(finalDamage, player.direction, true, 2f, DamageClass.Melee, true);
                    
                    // 독 부여
                    npc.AddBuff(BuffID.Poisoned, POISON_DURATION[rank - 1]);
                    npc.AddBuff(BuffID.Venom, POISON_DURATION[rank - 1] / 2);
                }
            }
            
            PlayEffects(player, direction);
        }

        private void PlayEffects(Player player, Vector2 direction)
        {
            SoundEngine.PlaySound(SoundID.Item71, player.position);
            
            for (int i = 0; i < 20; i++)
            {
                Vector2 dustPos = player.Center + direction * (20 + Main.rand.NextFloat(30));
                Dust dust = Dust.NewDustDirect(dustPos, 4, 4, DustID.Poisoned,
                    direction.X * 3f, Main.rand.NextFloat(-2f, 2f), 100, Color.Green, 1.2f);
                dust.noGravity = true;
            }
        }
    }
}
