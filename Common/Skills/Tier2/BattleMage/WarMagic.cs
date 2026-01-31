using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using RpgMod.Common.Base;

namespace RpgMod.Common.Skills.Tier2.BattleMage
{
    /// <summary>
    /// War Magic - 전쟁 마법.
    /// 근접 거리에서 마법 폭발.
    /// 배틀메이지의 근접 공격기.
    /// </summary>
    public class WarMagic : BaseSkill
    {
        public override string InternalName => "WarMagic";
        public override string DisplayName => "War Magic";
        public override string Description => "Unleash explosive magic in close combat.";

        public override SkillType SkillType => SkillType.Active;
        public override JobType RequiredJob => JobType.BattleMage;
        public override int RequiredLevel => 60;
        public override int SkillPointCost => 1;
        public override int MaxRank => 10;
        public override string IconTexture => $"{RpgConstants.ICON_PATH}Skills/WarMagic";
        
        public override float CooldownSeconds => 6f - (CurrentRank * 0.2f); // 6 -> 4초
        public override int ResourceCost => 25;
        public override ResourceType ResourceType => ResourceType.Mana;

        private static readonly int[] DAMAGE = { 90, 115, 140, 170, 205, 245, 290, 340, 400, 490 };
        private static readonly float[] RADIUS = { 80f, 90f, 100f, 110f, 120f, 130f, 140f, 150f, 165f, 200f };

        protected override void OnActivate(Player player)
        {
            int rank = System.Math.Max(1, CurrentRank);
            int damage = DAMAGE[rank - 1];
            float radius = RADIUS[rank - 1];
            
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC npc = Main.npc[i];
                if (npc.active && !npc.friendly && npc.CanBeChasedBy())
                {
                    float dist = Vector2.Distance(player.Center, npc.Center);
                    if (dist <= radius)
                    {
                        float scaledDamage = GetScaledDamage(player, DamageClass.Magic, damage);
                        int finalDamage = ApplyDamageVariance(player, scaledDamage);
                        bool crit = RollCrit(player, DamageClass.Magic);
                        npc.SimpleStrikeNPC(finalDamage, player.direction, crit, 5f, DamageClass.Magic);
                        npc.AddBuff(BuffID.OnFire, 180 + rank * 20);
                    }
                }
            }
            
            PlayEffects(player, radius);
        }

        private void PlayEffects(Player player, float radius)
        {
            SoundEngine.PlaySound(SoundID.Item74, player.position);
            
            for (int i = 0; i < 40; i++)
            {
                float angle = (float)i / 40f * MathHelper.TwoPi;
                Vector2 offset = new Vector2((float)System.Math.Cos(angle), (float)System.Math.Sin(angle)) * radius * 0.5f;
                
                Dust dust = Dust.NewDustDirect(player.Center + offset, 8, 8, DustID.Torch,
                    offset.X * 0.1f, offset.Y * 0.1f, 150, Color.OrangeRed, 1.5f);
                dust.noGravity = true;
            }
        }
    }
}
