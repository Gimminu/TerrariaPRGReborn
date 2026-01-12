using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Rpg.Common.Base;

namespace Rpg.Common.Skills.Tier2.Knight
{
    /// <summary>
    /// Provoke - 주변 적의 어그로를 끈다.
    /// 기사의 탱킹 스킬.
    /// </summary>
    public class Provoke : BaseSkill
    {
        public override string InternalName => "Provoke";
        public override string DisplayName => "Provoke";
        public override string Description => "Taunt nearby enemies, drawing their attention to you.";

        public override SkillType SkillType => SkillType.Active;
        public override JobType RequiredJob => JobType.Knight;
        public override int RequiredLevel => 63;
        public override int SkillPointCost => 1;
        public override int MaxRank => 10;
        public override string IconTexture => $"{RpgConstants.ICON_PATH}Skills/Provoke";
        
        public override float CooldownSeconds => 15f - (CurrentRank * 0.5f); // 15 -> 10초
        public override int ResourceCost => 25;
        public override ResourceType ResourceType => ResourceType.Stamina;

        private static readonly float[] RADIUS = { 150f, 170f, 190f, 210f, 230f, 250f, 270f, 290f, 310f, 350f };

        protected override void OnActivate(Player player)
        {
            int rank = System.Math.Max(1, CurrentRank);
            float radius = RADIUS[rank - 1];
            
            int count = 0;
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC npc = Main.npc[i];
                if (npc.active && !npc.friendly && npc.CanBeChasedBy())
                {
                    float dist = Vector2.Distance(player.Center, npc.Center);
                    if (dist <= radius)
                    {
                        // 타겟을 플레이어로 설정
                        npc.target = player.whoAmI;
                        count++;
                        
                        // 도발 이펙트
                        for (int d = 0; d < 5; d++)
                        {
                            Dust dust = Dust.NewDustDirect(npc.position, npc.width, npc.height,
                                DustID.RedTorch, 0, -2f, 100, Color.Red, 1.0f);
                            dust.noGravity = true;
                        }
                    }
                }
            }
            
            PlayEffects(player, radius);
            if (count > 0)
                ShowMessage(player, $"Provoked {count} enemies!", Color.Red);
        }

        private void PlayEffects(Player player, float radius)
        {
            SoundEngine.PlaySound(SoundID.Roar with { Pitch = 0.3f }, player.position);
            
            for (int i = 0; i < 40; i++)
            {
                float angle = MathHelper.TwoPi * i / 40f;
                Vector2 offset = new Vector2((float)System.Math.Cos(angle), (float)System.Math.Sin(angle)) * radius;
                Dust dust = Dust.NewDustDirect(player.Center + offset, 4, 4, DustID.RedTorch,
                    0, 0, 100, Color.Red, 1.2f);
                dust.noGravity = true;
            }
        }

        private void ShowMessage(Player player, string text, Color color)
        {
            if (Main.netMode != NetmodeID.Server)
                CombatText.NewText(player.Hitbox, color, text);
        }
    }
}
