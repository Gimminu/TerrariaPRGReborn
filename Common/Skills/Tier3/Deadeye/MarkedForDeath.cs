using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using RpgMod.Common.Base;

namespace RpgMod.Common.Skills.Tier3.Deadeye
{
    /// <summary>
    /// Marked for Death - 죽음의 표식.
    /// 타겟 마킹하여 피해 증가.
    /// 데드아이의 디버프기.
    /// </summary>
    public class MarkedForDeath : BaseSkill
    {
        public override string InternalName => "MarkedForDeath";
        public override string DisplayName => "Marked for Death";
        public override string Description => "Mark an enemy, causing them to take increased damage.";

        public override SkillType SkillType => SkillType.Active;
        public override JobType RequiredJob => JobType.Deadeye;
        public override int RequiredLevel => 130;
        public override int SkillPointCost => 1;
        public override int MaxRank => 10;
        public override string IconTexture => $"{RpgConstants.ICON_PATH}Skills/MarkedForDeath";
        
        public override float CooldownSeconds => 20f - (CurrentRank * 0.6f); // 20 -> 14초
        public override int ResourceCost => 30;
        public override ResourceType ResourceType => ResourceType.Stamina;

        private static readonly int[] MARK_DURATION = { 180, 210, 240, 270, 300, 330, 360, 400, 450, 540 };

        protected override void OnActivate(Player player)
        {
            int rank = System.Math.Max(1, CurrentRank);
            
            // 가장 가까운 적 마킹
            NPC target = null;
            float closestDist = 600f;
            
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
                // 방어 감소 디버프
                target.AddBuff(BuffID.BrokenArmor, MARK_DURATION[rank - 1]);
                target.AddBuff(BuffID.Ichor, MARK_DURATION[rank - 1]);
                
                PlayEffects(target);
                ShowMessage(target, "MARKED!", Color.Red);
            }
        }

        private void PlayEffects(NPC target)
        {
            SoundEngine.PlaySound(SoundID.Item29, target.position);
            
            for (int i = 0; i < 25; i++)
            {
                Dust dust = Dust.NewDustDirect(target.position, target.width, target.height,
                    DustID.Torch, Main.rand.NextFloat(-2f, 2f), Main.rand.NextFloat(-2f, 2f), 150, Color.Red, 1.3f);
                dust.noGravity = true;
            }
        }

        private void ShowMessage(NPC target, string text, Color color)
        {
            if (Main.netMode != NetmodeID.Server)
                CombatText.NewText(target.Hitbox, color, text);
        }
    }
}
