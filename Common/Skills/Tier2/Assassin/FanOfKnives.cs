using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using Rpg.Common.Base;

namespace Rpg.Common.Skills.Tier2.Assassin
{
    /// <summary>
    /// Fan of Knives - 표창 난무.
    /// 주변 모든 적에게 피해 + 독.
    /// </summary>
    public class FanOfKnives : BaseSkill
    {
        public override string InternalName => "FanOfKnives";
        public override string DisplayName => "Fan of Knives";
        public override string Description => "Throw a flurry of poisoned knives at all nearby enemies.";

        public override SkillType SkillType => SkillType.Active;
        public override JobType RequiredJob => JobType.Assassin;
        public override int RequiredLevel => 70;
        public override int SkillPointCost => 1;
        public override int MaxRank => 10;
        public override string IconTexture => $"{RpgConstants.ICON_PATH}Skills/FanOfKnives";
        
        public override float CooldownSeconds => 10f - (CurrentRank * 0.4f);
        public override int ResourceCost => 35;
        public override ResourceType ResourceType => ResourceType.Stamina;

        private static readonly int[] DAMAGE = { 40, 55, 70, 85, 105, 125, 145, 170, 195, 250 };
        private static readonly float RADIUS = 180f;

        protected override void OnActivate(Player player)
        {
            int rank = System.Math.Max(1, CurrentRank);
            int damage = DAMAGE[rank - 1];
            
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC npc = Main.npc[i];
                if (npc.active && !npc.friendly)
                {
                    float dist = Vector2.Distance(player.Center, npc.Center);
                    if (dist <= RADIUS)
                    {
                        int finalDamage = Main.DamageVar(damage, player.luck);
                        npc.SimpleStrikeNPC(finalDamage, player.direction, true, 3f, DamageClass.Ranged, true);
                        npc.AddBuff(BuffID.Poisoned, 300 + rank * 30);
                    }
                }
            }
            
            PlayEffects(player);
        }

        private void PlayEffects(Player player)
        {
            SoundEngine.PlaySound(SoundID.Item39, player.position);
            
            for (int i = 0; i < 40; i++)
            {
                float angle = MathHelper.TwoPi * i / 40f;
                Vector2 offset = new Vector2((float)System.Math.Cos(angle), (float)System.Math.Sin(angle)) * RADIUS;
                Dust dust = Dust.NewDustDirect(player.Center + offset * Main.rand.NextFloat(0.3f, 1f), 4, 4, DustID.GreenTorch,
                    offset.X * 0.05f, offset.Y * 0.05f, 100, Color.Green, 1.0f);
                dust.noGravity = true;
            }
        }
    }
}
