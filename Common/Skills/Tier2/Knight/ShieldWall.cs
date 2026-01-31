using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using RpgMod.Common.Base;

namespace RpgMod.Common.Skills.Tier2.Knight
{
    /// <summary>
    /// Shield Wall - Knight's ultimate defensive skill.
    /// Creates an impenetrable barrier that greatly reduces damage.
    /// </summary>
    public class ShieldWall : BaseSkill
    {
        public override string InternalName => "ShieldWall";
        public override string DisplayName => "Shield Wall";
        public override string Description => "Brace behind a solid defense, greatly reducing all damage taken.";

        public override SkillType SkillType => SkillType.Active;
        public override JobType RequiredJob => JobType.Knight;
        public override int RequiredLevel => RpgConstants.SECOND_JOB_LEVEL;
        public override int SkillPointCost => 1;
        public override int MaxRank => 5;
        public override string IconTexture => $"{RpgConstants.ICON_PATH}Skills/ShieldWall";
        public override float CooldownSeconds => 25f;
        public override int ResourceCost => 25;
        public override ResourceType ResourceType => ResourceType.Stamina;

        // Scaling per rank
        private static readonly int[] DURATION_SECONDS = { 8, 10, 12, 14, 18 };
        private static readonly int[] DEFENSE_BONUS = { 15, 20, 25, 30, 40 };

        protected override void OnActivate(Player player)
        {
            int rank = Math.Max(1, CurrentRank);
            int duration = DURATION_SECONDS[rank - 1] * 60;
            int defenseBonus = DEFENSE_BONUS[rank - 1];

            // Apply buffs
            player.AddBuff(BuffID.Endurance, duration);
            player.AddBuff(BuffID.Ironskin, duration);

            // Apply extra defense via RpgPlayer
            var rpgPlayer = player.GetModPlayer<Players.RpgPlayer>();
            rpgPlayer.AddTemporaryDefense(defenseBonus, duration);

            PlaySkillEffects(player);
            ShowMessage(player, $"Shield Wall! (+{defenseBonus} DEF)", Color.SteelBlue);
        }

        private void PlaySkillEffects(Player player)
        {
            SoundEngine.PlaySound(SoundID.NPCHit4, player.position);

            // Shield barrier effect
            for (int i = 0; i < 40; i++)
            {
                float angle = MathHelper.TwoPi * i / 40f;
                Vector2 offset = new Vector2(
                    (float)Math.Cos(angle) * 50f,
                    (float)Math.Sin(angle) * 50f
                );

                Dust dust = Dust.NewDustDirect(
                    player.Center + offset,
                    4,
                    4,
                    DustID.Silver,
                    0,
                    0,
                    100,
                    Color.LightSteelBlue,
                    1.5f
                );
                dust.noGravity = true;
            }
        }

        private void ShowMessage(Player player, string text, Color color)
        {
            if (Main.netMode != NetmodeID.Server)
            {
                CombatText.NewText(player.Hitbox, color, text, false, false);
            }
        }
    }
}
