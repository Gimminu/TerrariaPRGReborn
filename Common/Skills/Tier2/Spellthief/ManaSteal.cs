using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using RpgMod.Common.Base;

namespace RpgMod.Common.Skills.Tier2.Spellthief
{
    /// <summary>
    /// Mana Steal - 마나 절도.
    /// 적을 공격하고 마나 회복.
    /// 스펠시프의 기본 공격기.
    /// </summary>
    public class ManaSteal : BaseSkill
    {
        public override string InternalName => "ManaSteal";
        public override string DisplayName => "Mana Steal";
        public override string Description => "Strike enemies and steal their magical energy.";

        public override SkillType SkillType => SkillType.Active;
        public override JobType RequiredJob => JobType.Spellthief;
        public override int RequiredLevel => 60;
        public override int SkillPointCost => 1;
        public override int MaxRank => 10;
        public override string IconTexture => $"{RpgConstants.ICON_PATH}Skills/ManaSteal";
        
        public override float CooldownSeconds => 5f - (CurrentRank * 0.15f); // 5 -> 3.5초
        public override int ResourceCost => 15;
        public override ResourceType ResourceType => ResourceType.Stamina;

        private static readonly int[] DAMAGE = { 60, 75, 95, 115, 140, 165, 195, 230, 275, 340 };
        private static readonly int[] MANA_GAIN = { 10, 14, 18, 22, 27, 32, 38, 45, 55, 70 };

        protected override void OnActivate(Player player)
        {
            int rank = System.Math.Max(1, CurrentRank);
            int damage = DAMAGE[rank - 1];
            int manaGain = MANA_GAIN[rank - 1];
            
            Vector2 direction = (Main.MouseWorld - player.Center).SafeNormalize(Vector2.UnitX);
            
            // 가장 가까운 적
            NPC target = null;
            float closestDist = 200f;
            
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
                float scaledDamage = GetScaledDamage(player, DamageClass.Magic, damage);
                int finalDamage = ApplyDamageVariance(player, scaledDamage);
                bool crit = RollCrit(player, DamageClass.Magic);
                target.SimpleStrikeNPC(finalDamage, player.direction, crit, 3f, DamageClass.Magic);
                
                // 마나 회복
                player.statMana = System.Math.Min(player.statMana + manaGain, player.statManaMax2);
                
                PlayEffects(player, target);
                ShowMessage(player, $"+{manaGain} Mana", Color.Cyan);
            }
        }

        private void PlayEffects(Player player, NPC target)
        {
            SoundEngine.PlaySound(SoundID.Item8, player.position);
            
            Vector2 direction = (player.Center - target.Center).SafeNormalize(Vector2.UnitX);
            for (int i = 0; i < 12; i++)
            {
                Vector2 pos = Vector2.Lerp(target.Center, player.Center, i / 12f);
                Dust dust = Dust.NewDustDirect(pos, 4, 4, DustID.BlueCrystalShard,
                    direction.X * 2f, direction.Y * 2f, 150, Color.Cyan, 1.2f);
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
