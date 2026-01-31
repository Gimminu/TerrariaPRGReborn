using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using RpgMod.Common.Base;

namespace RpgMod.Common.Skills.Tier2.DeathKnight
{
    /// <summary>
    /// Death Strike - 죽음의 일격.
    /// 피해를 입히고 생명력 흡수.
    /// </summary>
    public class DeathStrike : BaseSkill
    {
        public override string InternalName => "DeathStrike";
        public override string DisplayName => "Death Strike";
        public override string Description => "A powerful strike that deals dark damage and heals you for a portion.";

        public override SkillType SkillType => SkillType.Active;
        public override JobType RequiredJob => JobType.DeathKnight;
        public override int RequiredLevel => 60;
        public override int SkillPointCost => 1;
        public override int MaxRank => 10;
        public override string IconTexture => $"{RpgConstants.ICON_PATH}Skills/DeathStrike";
        
        public override float CooldownSeconds => 6f - (CurrentRank * 0.2f);
        public override int ResourceCost => 25;
        public override ResourceType ResourceType => ResourceType.Mana;

        private static readonly int[] DAMAGE = { 100, 130, 165, 205, 250, 300, 355, 415, 480, 600 };
        private static readonly float[] LIFESTEAL = { 0.10f, 0.12f, 0.14f, 0.16f, 0.18f, 0.20f, 0.22f, 0.24f, 0.26f, 0.30f };

        protected override void OnActivate(Player player)
        {
            int rank = System.Math.Max(1, CurrentRank);
            int damage = DAMAGE[rank - 1];
            float lifestealPercent = LIFESTEAL[rank - 1];
            
            // 가장 가까운 적 공격
            NPC target = null;
            float closestDist = 150f;
            
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC npc = Main.npc[i];
                if (npc.active && !npc.friendly && npc.CanBeChasedBy())
                {
                    float dist = Vector2.Distance(player.Center, npc.Center);
                    if (dist < closestDist)
                    {
                        closestDist = dist;
                        target = npc;
                    }
                }
            }
            
            if (target != null)
            {
                float scaledDamage = GetScaledDamage(player, DamageClass.Melee, damage);
                int finalDamage = ApplyDamageVariance(player, scaledDamage);
                bool crit = RollCrit(player, DamageClass.Melee);
                target.SimpleStrikeNPC(finalDamage, player.direction, crit, 3f, DamageClass.Melee);
                
                int healAmount = (int)(finalDamage * lifestealPercent);
                player.Heal(healAmount);
                
                PlayEffects(player, target);
            }
        }

        private void PlayEffects(Player player, NPC target)
        {
            SoundEngine.PlaySound(SoundID.NPCDeath52, player.position);
            
            for (int i = 0; i < 25; i++)
            {
                Dust dust = Dust.NewDustDirect(target.Center, 4, 4, DustID.Shadowflame,
                    Main.rand.NextFloat(-5f, 5f), Main.rand.NextFloat(-5f, 5f), 100, Color.Purple, 1.3f);
                dust.noGravity = true;
            }
        }
    }
}
