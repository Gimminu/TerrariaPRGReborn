using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using RpgMod.Common.Base;

namespace RpgMod.Common.Skills.Tier2.Sniper
{
    /// <summary>
    /// Headshot - Sniper's precision attack.
    /// Strike with a precise lethal shot dealing massive damage.
    /// </summary>
    public class Headshot : BaseSkill
    {
        public override string InternalName => "Headshot";
        public override string DisplayName => "Headshot";
        public override string Description => "Strike with a precise lethal shot that always critically hits and boosts critical damage.";

        public override SkillType SkillType => SkillType.Active;
        public override JobType RequiredJob => JobType.Sniper;
        public override int RequiredLevel => RpgConstants.SECOND_JOB_LEVEL;
        public override int SkillPointCost => 1;
        public override int MaxRank => 5;
        public override string IconTexture => $"{RpgConstants.ICON_PATH}Skills/Headshot";
        public override float CooldownSeconds => 12f;
        public override int ResourceCost => 20;
        public override ResourceType ResourceType => ResourceType.Stamina;

        // Scaling per rank
        private static readonly int[] BASE_DAMAGE = { 50, 65, 80, 100, 130 };
        private static readonly float[] CRIT_DAMAGE_MULT = { 2.0f, 2.2f, 2.4f, 2.6f, 3.0f };
        private static readonly float[] RANGE = { 300f, 350f, 400f, 450f, 500f };

        public override bool CanUse(Player player)
        {
            if (!base.CanUse(player))
                return false;

            if (player.HeldItem == null || player.HeldItem.IsAir)
            {
                ShowMessage(player, "Need a weapon!", Color.Red);
                return false;
            }

            if (!player.HeldItem.CountsAsClass(Terraria.ModLoader.DamageClass.Ranged))
            {
                ShowMessage(player, "Need a ranged weapon!", Color.Orange);
                return false;
            }

            return true;
        }

        protected override void OnActivate(Player player)
        {
            int rank = Math.Max(1, CurrentRank);
            int baseDamage = BASE_DAMAGE[rank - 1];
            float critMult = CRIT_DAMAGE_MULT[rank - 1];
            float range = RANGE[rank - 1];

            // Add weapon damage
            Item weapon = player.HeldItem;
            int totalDamage = baseDamage + weapon.damage;

            // Find the nearest enemy in range
            NPC target = null;
            float closestDist = range;

            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC npc = Main.npc[i];
                if (!npc.active || npc.friendly || npc.lifeMax <= 5)
                    continue;

                float distance = Vector2.Distance(npc.Center, player.Center);
                if (distance < closestDist)
                {
                    closestDist = distance;
                    target = npc;
                }
            }

            if (target != null)
            {
                // Always crit with bonus crit damage
                int hitDirection = target.Center.X >= player.Center.X ? 1 : -1;
                float scaledDamage = GetScaledDamage(player, DamageClass.Ranged, totalDamage);
                int critDamage = (int)(scaledDamage * critMult);

                player.ApplyDamageToNPC(target, critDamage, 2f, hitDirection, true, Terraria.ModLoader.DamageClass.Ranged, false);
                
                CreateHitEffect(player, target);
                ShowMessage(player, $"HEADSHOT! ({critDamage})", Color.Gold);
            }
            else
            {
                ShowMessage(player, "No target in range!", Color.Gray);
            }

            PlaySkillEffects(player);
        }

        private void CreateHitEffect(Player player, NPC target)
        {
            // Bullet trail effect
            Vector2 direction = (target.Center - player.Center).SafeNormalize(Vector2.Zero);
            float distance = Vector2.Distance(player.Center, target.Center);

            for (int i = 0; i < distance; i += 10)
            {
                Vector2 pos = player.Center + direction * i;
                
                Dust dust = Dust.NewDustDirect(
                    pos,
                    4,
                    4,
                    DustID.YellowTorch,
                    direction.X * 2f,
                    direction.Y * 2f,
                    100,
                    Color.Gold,
                    0.8f
                );
                dust.noGravity = true;
            }

            // Impact effect
            for (int i = 0; i < 15; i++)
            {
                Dust dust = Dust.NewDustDirect(
                    target.position,
                    target.width,
                    target.height,
                    DustID.GoldFlame,
                    Main.rand.NextFloat(-4f, 4f),
                    Main.rand.NextFloat(-4f, 4f),
                    100,
                    default,
                    1.5f
                );
                dust.noGravity = true;
            }
        }

        private void PlaySkillEffects(Player player)
        {
            SoundEngine.PlaySound(SoundID.Item40 with { Pitch = -0.2f }, player.position);
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
