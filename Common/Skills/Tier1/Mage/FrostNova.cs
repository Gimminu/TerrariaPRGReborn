using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using RpgMod.Common.Base;

namespace RpgMod.Common.Skills.Tier1.Mage
{
    /// <summary>
    /// Frost Nova - 서리 신성.
    /// 주변 적을 얼려서 느리게 만든다.
    /// 메이지의 CC 스킬.
    /// </summary>
    public class FrostNova : BaseSkill
    {
        public override string InternalName => "FrostNova";
        public override string DisplayName => "Frost Nova";
        public override string Description => "Release a wave of frost, damaging and slowing nearby enemies.";

        public override SkillType SkillType => SkillType.Active;
        public override JobType RequiredJob => JobType.Mage;
        public override int RequiredLevel => 22;
        public override int SkillPointCost => 1;
        public override int MaxRank => 10;
        public override string IconTexture => $"{RpgConstants.ICON_PATH}Skills/FrostNova";
        
        public override float CooldownSeconds => 12f - (CurrentRank * 0.5f); // 12 -> 7초
        public override int ResourceCost => 40;
        public override ResourceType ResourceType => ResourceType.Mana;

        private static readonly int[] DAMAGE = { 35, 45, 55, 70, 85, 100, 115, 130, 150, 180 };
        private static readonly float[] RADIUS = { 80f, 90f, 100f, 110f, 120f, 130f, 140f, 150f, 165f, 180f };
        private static readonly int[] SLOW_DURATION = { 120, 150, 180, 210, 240, 270, 300, 330, 360, 420 };

        protected override void OnActivate(Player player)
        {
            int rank = System.Math.Max(1, CurrentRank);
            int damage = DAMAGE[rank - 1];
            float radius = RADIUS[rank - 1];
            
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC npc = Main.npc[i];
                if (npc.active && !npc.friendly)
                {
                    float dist = Vector2.Distance(player.Center, npc.Center);
                    if (dist <= radius)
                    {
                        float distFactor = 1f - (dist / radius) * 0.4f;
                        float scaledDamage = GetScaledDamage(player, DamageClass.Magic, damage);
                        int finalDamage = ApplyDamageVariance(player, scaledDamage);
                        finalDamage = (int)(finalDamage * distFactor);
                        bool crit = RollCrit(player, DamageClass.Magic);
                        npc.SimpleStrikeNPC(finalDamage, player.direction, crit, 2f, DamageClass.Magic);
                        
                        // 동상/느림 부여
                        npc.AddBuff(BuffID.Frostburn, SLOW_DURATION[rank - 1]);
                        npc.AddBuff(BuffID.Slow, SLOW_DURATION[rank - 1]);
                    }
                }
            }
            
            PlayEffects(player, radius);
        }

        private void PlayEffects(Player player, float radius)
        {
            SoundEngine.PlaySound(SoundID.Item30, player.position);
            
            // 얼음 파동
            for (int i = 0; i < 60; i++)
            {
                float angle = MathHelper.TwoPi * i / 60f;
                Vector2 offset = new Vector2((float)System.Math.Cos(angle), (float)System.Math.Sin(angle)) * radius;
                Dust dust = Dust.NewDustDirect(player.Center + offset * Main.rand.NextFloat(0.2f, 1f), 4, 4, DustID.Ice,
                    offset.X * 0.05f, offset.Y * 0.05f, 150, Color.LightBlue, 1.2f);
                dust.noGravity = true;
            }
        }
    }
}
