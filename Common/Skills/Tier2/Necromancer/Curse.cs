using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using RpgMod.Common.Base;

namespace RpgMod.Common.Skills.Tier2.Necromancer
{
    /// <summary>
    /// Curse - 저주.
    /// 적의 방어력 감소.
    /// </summary>
    public class Curse : BaseSkill
    {
        public override string InternalName => "Curse";
        public override string DisplayName => "Curse";
        public override string Description => "Place a dark curse on enemies, reducing their defense.";

        public override SkillType SkillType => SkillType.Active;
        public override JobType RequiredJob => JobType.Necromancer;
        public override int RequiredLevel => 70;
        public override int SkillPointCost => 1;
        public override int MaxRank => 10;
        public override string IconTexture => $"{RpgConstants.ICON_PATH}Skills/Curse";
        
        public override float CooldownSeconds => 18f - (CurrentRank * 0.6f);
        public override int ResourceCost => 45;
        public override ResourceType ResourceType => ResourceType.Mana;

        private static readonly float RADIUS = 300f;

        protected override void OnActivate(Player player)
        {
            int rank = System.Math.Max(1, CurrentRank);
            int debuffDuration = (150 + rank * 30); // 2.5 ~ 5.5초
            
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC npc = Main.npc[i];
                if (npc.active && !npc.friendly)
                {
                    float dist = Vector2.Distance(player.Center, npc.Center);
                    if (dist <= RADIUS)
                    {
                        npc.AddBuff(BuffID.BetsysCurse, debuffDuration); // 방어력 감소
                        npc.AddBuff(BuffID.Slow, debuffDuration);
                        
                        CreateCurseEffect(npc.Center);
                    }
                }
            }
            
            PlayEffects(player);
        }

        private void CreateCurseEffect(Vector2 position)
        {
            for (int i = 0; i < 10; i++)
            {
                Dust dust = Dust.NewDustDirect(position, 4, 4, DustID.Shadowflame,
                    Main.rand.NextFloat(-2f, 2f), Main.rand.NextFloat(-2f, 2f), 100, Color.Purple, 1.0f);
                dust.noGravity = true;
            }
        }

        private void PlayEffects(Player player)
        {
            SoundEngine.PlaySound(SoundID.NPCDeath52, player.position);
            
            for (int i = 0; i < 30; i++)
            {
                float angle = MathHelper.TwoPi * i / 30f;
                Vector2 offset = new Vector2((float)System.Math.Cos(angle), (float)System.Math.Sin(angle)) * RADIUS;
                Dust dust = Dust.NewDustDirect(player.Center + offset, 4, 4, DustID.Shadowflame,
                    0, 0, 100, Color.Purple, 0.8f);
                dust.noGravity = true;
            }
        }
    }
}
