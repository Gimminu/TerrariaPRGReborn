using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using Rpg.Common.Base;

namespace Rpg.Common.Skills.Tier3.Lichking
{
    /// <summary>
    /// Death's Embrace - 죽음의 포옹.
    /// 주변 모든 적에게 피해 + 언데드 소환.
    /// 리치킹의 궁극 공격기.
    /// </summary>
    public class DeathsEmbrace : BaseSkill
    {
        public override string InternalName => "DeathsEmbrace";
        public override string DisplayName => "Death's Embrace";
        public override string Description => "Embrace enemies with death, dealing massive damage and raising undead.";

        public override SkillType SkillType => SkillType.Active;
        public override JobType RequiredJob => JobType.Lichking;
        public override int RequiredLevel => 120;
        public override int SkillPointCost => 1;
        public override int MaxRank => 10;
        public override string IconTexture => $"{RpgConstants.ICON_PATH}Skills/DeathsEmbrace";
        
        public override float CooldownSeconds => 15f - (CurrentRank * 0.45f); // 15 -> 10.5초
        public override int ResourceCost => 55;
        public override ResourceType ResourceType => ResourceType.Mana;

        private static readonly int[] DAMAGE = { 130, 165, 205, 250, 305, 365, 435, 520, 620, 780 };
        private static readonly int[] SUMMON_COUNT = { 2, 2, 3, 3, 4, 4, 5, 6, 7, 10 };

        protected override void OnActivate(Player player)
        {
            int rank = System.Math.Max(1, CurrentRank);
            int damage = DAMAGE[rank - 1];
            int summonCount = SUMMON_COUNT[rank - 1];
            float radius = 250f;
            
            // 광역 피해
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC npc = Main.npc[i];
                if (npc.active && !npc.friendly && npc.CanBeChasedBy())
                {
                    float dist = Vector2.Distance(player.Center, npc.Center);
                    if (dist <= radius)
                    {
                        int finalDamage = Main.DamageVar(damage, player.luck);
                        npc.SimpleStrikeNPC(finalDamage, player.direction, true, 5f, DamageClass.Magic, true);
                    }
                }
            }
            
            // 언데드 소환
            for (int i = 0; i < summonCount; i++)
            {
                Vector2 offset = new Vector2(Main.rand.NextFloat(-60, 60), -10);
                Projectile.NewProjectile(player.GetSource_FromThis(), player.Center + offset, Vector2.Zero,
                    ProjectileID.BabySlime, damage / 3, 2f, player.whoAmI);
            }
            
            PlayEffects(player, radius);
        }

        private void PlayEffects(Player player, float radius)
        {
            SoundEngine.PlaySound(SoundID.NPCDeath10, player.position);
            
            for (int i = 0; i < 50; i++)
            {
                float angle = (float)i / 50f * MathHelper.TwoPi;
                Vector2 offset = new Vector2((float)System.Math.Cos(angle), (float)System.Math.Sin(angle)) * radius * 0.5f;
                
                Dust dust = Dust.NewDustDirect(player.Center + offset, 8, 8, DustID.Bone,
                    offset.X * 0.05f, offset.Y * 0.05f, 150, Color.Purple, 1.5f);
                dust.noGravity = true;
            }
        }
    }
}
