using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Rpg.Common.Base;

namespace Rpg.Common.Skills.Tier1.Summoner
{
    /// <summary>
    /// Command: Focus - 명령: 집중.
    /// 모든 소환수가 지정한 적을 공격.
    /// 소환사의 타겟팅 스킬.
    /// </summary>
    public class CommandFocus : BaseSkill
    {
        public override string InternalName => "CommandFocus";
        public override string DisplayName => "Command: Focus";
        public override string Description => "Command all your minions to focus on the target near your cursor.";

        public override SkillType SkillType => SkillType.Active;
        public override JobType RequiredJob => JobType.Summoner;
        public override int RequiredLevel => 25;
        public override int SkillPointCost => 1;
        public override int MaxRank => 10;
        public override string IconTexture => $"{RpgConstants.ICON_PATH}Skills/CommandFocus";
        
        public override float CooldownSeconds => 3f; // 짧은 쿨 - 자주 쓰는 스킬
        public override int ResourceCost => 10;
        public override ResourceType ResourceType => ResourceType.Mana;

        protected override void OnActivate(Player player)
        {
            // 마우스 근처 적 찾기
            NPC targetNPC = null;
            float closestDist = 300f;
            
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC npc = Main.npc[i];
                if (npc.active && !npc.friendly && npc.CanBeChasedBy())
                {
                    float dist = Vector2.Distance(Main.MouseWorld, npc.Center);
                    if (dist < closestDist)
                    {
                        closestDist = dist;
                        targetNPC = npc;
                    }
                }
            }
            
            if (targetNPC != null)
            {
                // 모든 미니언이 해당 타겟을 공격하도록 설정
                player.MinionAttackTargetNPC = targetNPC.whoAmI;
                
                PlayEffects(player, targetNPC);
                ShowMessage(targetNPC, "TARGET!", Color.Red);
            }
        }

        private void PlayEffects(Player player, NPC target)
        {
            SoundEngine.PlaySound(SoundID.Item4, player.position);
            
            // 타겟 마킹 이펙트
            for (int i = 0; i < 20; i++)
            {
                float angle = MathHelper.TwoPi * i / 20f;
                Vector2 offset = new Vector2((float)System.Math.Cos(angle), (float)System.Math.Sin(angle)) * 30f;
                Dust dust = Dust.NewDustDirect(target.Center + offset, 4, 4, DustID.RedTorch,
                    0, 0, 100, Color.Red, 1.2f);
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
