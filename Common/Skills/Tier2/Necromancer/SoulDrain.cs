using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using Rpg.Common.Base;

namespace Rpg.Common.Skills.Tier2.Necromancer
{
    /// <summary>
    /// Soul Drain - 영혼 흡수.
    /// 적의 생명력을 흡수.
    /// </summary>
    public class SoulDrain : BaseSkill
    {
        public override string InternalName => "SoulDrain";
        public override string DisplayName => "Soul Drain";
        public override string Description => "Drain the life force from nearby enemies to restore your health.";

        public override SkillType SkillType => SkillType.Active;
        public override JobType RequiredJob => JobType.Necromancer;
        public override int RequiredLevel => 65;
        public override int SkillPointCost => 1;
        public override int MaxRank => 10;
        public override string IconTexture => $"{RpgConstants.ICON_PATH}Skills/SoulDrain";
        
        public override float CooldownSeconds => 12f - (CurrentRank * 0.4f);
        public override int ResourceCost => 40;
        public override ResourceType ResourceType => ResourceType.Mana;

        private static readonly int[] DAMAGE = { 30, 40, 50, 65, 80, 95, 115, 135, 160, 200 };
        private static readonly float RADIUS = 250f;

        protected override void OnActivate(Player player)
        {
            int rank = System.Math.Max(1, CurrentRank);
            int damage = DAMAGE[rank - 1];
            int totalHealing = 0;
            
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC npc = Main.npc[i];
                if (npc.active && !npc.friendly)
                {
                    float dist = Vector2.Distance(player.Center, npc.Center);
                    if (dist <= RADIUS)
                    {
                        int finalDamage = Main.DamageVar(damage, player.luck);
                        npc.SimpleStrikeNPC(finalDamage, player.direction, true, 0.5f, DamageClass.Summon, true);
                        
                        totalHealing += finalDamage / 4;
                        
                        CreateDrainEffect(npc.Center, player.Center);
                    }
                }
            }
            
            if (totalHealing > 0)
            {
                player.Heal(System.Math.Min(totalHealing, 100));
            }
            
            SoundEngine.PlaySound(SoundID.NPCDeath39, player.position);
        }

        private void CreateDrainEffect(Vector2 start, Vector2 end)
        {
            Vector2 direction = (end - start).SafeNormalize(Vector2.Zero);
            float distance = Vector2.Distance(start, end);
            
            for (int i = 0; i < (int)(distance / 30); i++)
            {
                Vector2 pos = Vector2.Lerp(start, end, (float)i / (distance / 30));
                Dust dust = Dust.NewDustDirect(pos, 4, 4, DustID.VenomStaff,
                    direction.X * 2f, direction.Y * 2f, 100, Color.Purple, 1.0f);
                dust.noGravity = true;
            }
        }
    }
}
