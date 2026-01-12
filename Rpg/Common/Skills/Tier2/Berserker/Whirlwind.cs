using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using Rpg.Common.Base;

namespace Rpg.Common.Skills.Tier2.Berserker
{
    /// <summary>
    /// Whirlwind - 회전 공격.
    /// 주변 모든 적에게 피해.
    /// </summary>
    public class Whirlwind : BaseSkill
    {
        public override string InternalName => "Whirlwind";
        public override string DisplayName => "Whirlwind";
        public override string Description => "Spin wildly, damaging all enemies around you.";

        public override SkillType SkillType => SkillType.Active;
        public override JobType RequiredJob => JobType.Berserker;
        public override int RequiredLevel => 67;
        public override int SkillPointCost => 1;
        public override int MaxRank => 10;
        public override string IconTexture => $"{RpgConstants.ICON_PATH}Skills/Whirlwind";
        
        public override float CooldownSeconds => 8f - (CurrentRank * 0.3f);
        public override int ResourceCost => 35;
        public override ResourceType ResourceType => ResourceType.Stamina;

        private static readonly int[] DAMAGE = { 50, 65, 80, 95, 115, 135, 155, 180, 205, 250 };
        private static readonly float[] RADIUS = { 100f, 110f, 120f, 130f, 140f, 150f, 160f, 175f, 190f, 220f };

        protected override void OnActivate(Player player)
        {
            int rank = System.Math.Max(1, CurrentRank);
            float radius = RADIUS[rank - 1];
            
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC npc = Main.npc[i];
                if (npc.active && !npc.friendly)
                {
                    float dist = Vector2.Distance(player.Center, npc.Center);
                    if (dist <= radius)
                    {
                        int damage = Main.DamageVar(DAMAGE[rank - 1], player.luck);
                        npc.SimpleStrikeNPC(damage, player.direction, true, 5f, DamageClass.Melee, true);
                    }
                }
            }
            
            PlayEffects(player, radius);
        }

        private void PlayEffects(Player player, float radius)
        {
            SoundEngine.PlaySound(SoundID.Item71, player.position);
            
            for (int i = 0; i < 50; i++)
            {
                float angle = MathHelper.TwoPi * i / 50f;
                Vector2 offset = new Vector2((float)System.Math.Cos(angle), (float)System.Math.Sin(angle)) * radius;
                Dust dust = Dust.NewDustDirect(player.Center + offset * Main.rand.NextFloat(0.3f, 1f), 4, 4, DustID.Torch,
                    offset.X * 0.1f, offset.Y * 0.1f, 100, Color.Orange, 1.3f);
                dust.noGravity = true;
            }
        }
    }
}
