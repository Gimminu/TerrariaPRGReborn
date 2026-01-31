using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using RpgMod.Common.Base;

namespace RpgMod.Common.Skills.Tier2.Druid
{
    /// <summary>
    /// Vine Whip - 덩굴 채찍.
    /// 넓은 범위 공격.
    /// 드루이드의 광역 공격기.
    /// </summary>
    public class VineWhip : BaseSkill
    {
        public override string InternalName => "VineWhip";
        public override string DisplayName => "Vine Whip";
        public override string Description => "Lash out with thorny vines, damaging nearby enemies.";

        public override SkillType SkillType => SkillType.Active;
        public override JobType RequiredJob => JobType.Druid;
        public override int RequiredLevel => 68;
        public override int SkillPointCost => 1;
        public override int MaxRank => 10;
        public override string IconTexture => $"{RpgConstants.ICON_PATH}Skills/VineWhip";
        
        public override float CooldownSeconds => 6f - (CurrentRank * 0.2f); // 6 -> 4초
        public override int ResourceCost => 25;
        public override ResourceType ResourceType => ResourceType.Mana;

        private static readonly int[] DAMAGE = { 55, 70, 85, 105, 125, 150, 180, 215, 255, 315 };
        private static readonly float[] RANGE = { 100f, 110f, 120f, 130f, 140f, 150f, 160f, 175f, 195f, 230f };

        protected override void OnActivate(Player player)
        {
            int rank = System.Math.Max(1, CurrentRank);
            int damage = DAMAGE[rank - 1];
            float range = RANGE[rank - 1];
            
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC npc = Main.npc[i];
                if (npc.active && !npc.friendly && npc.CanBeChasedBy())
                {
                    float dist = Vector2.Distance(player.Center, npc.Center);
                    if (dist <= range)
                    {
                        float scaledDamage = GetScaledDamage(player, DamageClass.Summon, damage);
                        int finalDamage = ApplyDamageVariance(player, scaledDamage);
                        bool crit = RollCrit(player, DamageClass.Summon);
                        npc.SimpleStrikeNPC(finalDamage, player.direction, crit, 4f, DamageClass.Summon);
                        npc.AddBuff(BuffID.Poisoned, 180 + rank * 20);
                    }
                }
            }
            
            PlayEffects(player, range);
        }

        private void PlayEffects(Player player, float range)
        {
            SoundEngine.PlaySound(SoundID.Grass, player.position);
            
            for (int i = 0; i < 30; i++)
            {
                float angle = (float)i / 30f * MathHelper.TwoPi;
                Vector2 offset = new Vector2((float)System.Math.Cos(angle), (float)System.Math.Sin(angle)) * range * 0.6f;
                
                Dust dust = Dust.NewDustDirect(player.Center + offset, 4, 4, DustID.Grass,
                    0f, 0f, 150, Color.Green, 1.3f);
                dust.noGravity = true;
            }
        }
    }
}
