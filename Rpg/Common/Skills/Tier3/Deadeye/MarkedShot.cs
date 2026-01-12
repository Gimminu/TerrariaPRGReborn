using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using Rpg.Common.Base;

namespace Rpg.Common.Skills.Tier3.Deadeye
{
    /// <summary>
    /// Marked Shot - Deadeye's ultimate precision attack.
    /// Exploit enemy weak points for devastating damage.
    /// </summary>
    public class MarkedShot : BaseSkill
    {
        public override string InternalName => "MarkedShot";
        public override string DisplayName => "Marked Shot";
        public override string Description => "Exploit enemy weak points with a devastating marked shot.";

        public override SkillType SkillType => SkillType.Active;
        public override JobType RequiredJob => JobType.Deadeye;
        public override int RequiredLevel => RpgConstants.THIRD_JOB_LEVEL;
        public override int SkillPointCost => 1;
        public override int MaxRank => 5;
        public override string IconTexture => $"{RpgConstants.ICON_PATH}Skills/MarkedShot";
        public override float CooldownSeconds => 20f;
        public override int ResourceCost => 30;
        public override ResourceType ResourceType => ResourceType.Stamina;

        // Scaling per rank
        private static readonly int[] BASE_DAMAGE = { 70, 90, 110, 140, 180 };
        private static readonly float[] CRIT_DAMAGE_MULT = { 2.5f, 2.8f, 3.1f, 3.5f, 4.0f };
        private static readonly float[] RANGE = { 400f, 450f, 500f, 550f, 600f };
        private static readonly int[] BONUS_HITS = { 1, 1, 2, 2, 3 };

        protected override void OnActivate(Player player)
        {
            int rank = Math.Max(1, CurrentRank);
            int baseDamage = BASE_DAMAGE[rank - 1];
            float critMult = CRIT_DAMAGE_MULT[rank - 1];
            float range = RANGE[rank - 1];
            int bonusHits = BONUS_HITS[rank - 1];

            // Add weapon damage
            Item weapon = player.HeldItem;
            int weaponDamage = weapon != null && !weapon.IsAir ? weapon.damage : 0;
            int totalDamage = baseDamage + weaponDamage;

            // Find targets in range
            int hitCount = 0;
            for (int i = 0; i < Main.maxNPCs && hitCount <= bonusHits; i++)
            {
                NPC npc = Main.npc[i];
                if (!npc.active || npc.friendly || npc.lifeMax <= 5)
                    continue;

                float distance = Vector2.Distance(npc.Center, player.Center);
                if (distance > range)
                    continue;

                // Always crit with massive damage
                int hitDirection = npc.Center.X >= player.Center.X ? 1 : -1;
                int critDamage = (int)(totalDamage * critMult);

                player.ApplyDamageToNPC(npc, critDamage, 3f, hitDirection, true, DamageClass.Ranged, false);
                
                CreateHitEffect(player, npc);
                hitCount++;
            }

            PlaySkillEffects(player);

            if (hitCount > 0)
            {
                ShowMessage(player, $"MARKED! ({hitCount} targets)", Color.Gold);
            }
            else
            {
                ShowMessage(player, "No targets in range!", Color.Gray);
            }
        }

        private void CreateHitEffect(Player player, NPC target)
        {
            // Laser beam effect
            Vector2 direction = (target.Center - player.Center).SafeNormalize(Vector2.Zero);
            float distance = Vector2.Distance(player.Center, target.Center);

            for (int i = 0; i < distance; i += 8)
            {
                Vector2 pos = player.Center + direction * i;
                
                Dust dust = Dust.NewDustDirect(
                    pos,
                    4,
                    4,
                    DustID.GoldFlame,
                    direction.X,
                    direction.Y,
                    100,
                    Color.Gold,
                    1f
                );
                dust.noGravity = true;
            }

            // Target explosion
            for (int i = 0; i < 20; i++)
            {
                Dust dust = Dust.NewDustDirect(
                    target.position,
                    target.width,
                    target.height,
                    DustID.SolarFlare,
                    Main.rand.NextFloat(-5f, 5f),
                    Main.rand.NextFloat(-5f, 5f),
                    100,
                    default,
                    2f
                );
                dust.noGravity = true;
            }
        }

        private void PlaySkillEffects(Player player)
        {
            SoundEngine.PlaySound(SoundID.Item75, player.position);
        }

        private void ShowMessage(Player player, string text, Color color)
        {
            if (Main.netMode != NetmodeID.Server)
            {
                CombatText.NewText(player.Hitbox, color, text, true, false);
            }
        }
    }
}
