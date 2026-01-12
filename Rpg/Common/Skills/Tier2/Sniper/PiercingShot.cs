using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using Rpg.Common.Base;

namespace Rpg.Common.Skills.Tier2.Sniper
{
    /// <summary>
    /// Piercing Shot - 관통탄.
    /// 적을 관통하는 강력한 사격.
    /// </summary>
    public class PiercingShot : BaseSkill
    {
        public override string InternalName => "PiercingShot";
        public override string DisplayName => "Piercing Shot";
        public override string Description => "Fire a powerful shot that pierces through multiple enemies.";

        public override SkillType SkillType => SkillType.Active;
        public override JobType RequiredJob => JobType.Sniper;
        public override int RequiredLevel => 70;
        public override int SkillPointCost => 1;
        public override int MaxRank => 10;
        public override string IconTexture => $"{RpgConstants.ICON_PATH}Skills/PiercingShot";
        
        public override float CooldownSeconds => 10f - (CurrentRank * 0.4f);
        public override int ResourceCost => 40;
        public override ResourceType ResourceType => ResourceType.Stamina;

        private static readonly int[] DAMAGE = { 80, 100, 125, 150, 180, 210, 245, 280, 320, 400 };

        protected override void OnActivate(Player player)
        {
            int rank = System.Math.Max(1, CurrentRank);
            int damage = DAMAGE[rank - 1];
            
            Vector2 direction = (Main.MouseWorld - player.Center).SafeNormalize(Vector2.UnitX);
            
            // 경로상의 모든 적에게 피해
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC npc = Main.npc[i];
                if (npc.active && !npc.friendly && npc.CanBeChasedBy())
                {
                    Vector2 toNPC = npc.Center - player.Center;
                    float dist = toNPC.Length();
                    float dot = Vector2.Dot(toNPC.SafeNormalize(Vector2.Zero), direction);
                    
                    if (dot > 0.9f && dist < 1000f)
                    {
                        int finalDamage = Main.DamageVar(damage, player.luck);
                        npc.SimpleStrikeNPC(finalDamage, player.direction, true, 4f, DamageClass.Ranged, true);
                    }
                }
            }
            
            PlayEffects(player, direction);
        }

        private void PlayEffects(Player player, Vector2 direction)
        {
            SoundEngine.PlaySound(SoundID.Item40, player.position);
            
            // 관통 궤적
            for (int i = 0; i < 100; i++)
            {
                Vector2 pos = player.Center + direction * i * 10;
                Dust dust = Dust.NewDustDirect(pos, 4, 4, DustID.YellowTorch,
                    direction.X * 2f, direction.Y * 2f, 100, Color.Yellow, 0.8f);
                dust.noGravity = true;
            }
        }
    }
}
