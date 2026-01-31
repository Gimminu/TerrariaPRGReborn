using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using RpgMod.Common.Base;

namespace RpgMod.Common.Skills.Tier1.Warrior
{
    /// <summary>
    /// Power Strike - Warrior's signature melee skill.
    /// A heavy swing that deals massive damage and knocks back enemies.
    /// </summary>
    public class PowerStrike : BaseSkill
    {
        public override string InternalName => "PowerStrike";
        public override string DisplayName => "Power Strike";
        public override string Description => "A heavy swing that hits nearby enemies with devastating force.";

        public override SkillType SkillType => SkillType.Active;
        public override JobType RequiredJob => JobType.Warrior;
        public override int RequiredLevel => RpgConstants.FIRST_JOB_LEVEL;
        public override int SkillPointCost => 1;
        public override int MaxRank => 5;
        public override string IconTexture => $"{RpgConstants.ICON_PATH}Skills/PowerStrike";
        public override float CooldownSeconds => 6f;
        public override int ResourceCost => 20;
        public override ResourceType ResourceType => ResourceType.Stamina;

        // Scaling per rank
        private static readonly float[] DAMAGE_MULTIPLIER = { 1.5f, 1.75f, 2.0f, 2.3f, 2.6f };
        private static readonly float[] KNOCKBACK_BONUS = { 3f, 4f, 5f, 6f, 8f };
        private static readonly float[] RADIUS = { 80f, 90f, 100f, 110f, 120f };

        public override bool CanUse(Player player)
        {
            if (!base.CanUse(player))
                return false;

            if (player.HeldItem == null || player.HeldItem.IsAir)
            {
                ShowMessage(player, "Need a weapon equipped!", Color.Red);
                return false;
            }

            if (!player.HeldItem.CountsAsClass(DamageClass.Melee))
            {
                ShowMessage(player, "Need a melee weapon!", Color.Orange);
                return false;
            }

            return true;
        }

        protected override void OnActivate(Player player)
        {
            int rank = Math.Max(1, CurrentRank);
            float damageMultiplier = DAMAGE_MULTIPLIER[rank - 1];
            float knockbackBonus = KNOCKBACK_BONUS[rank - 1];
            float radius = RADIUS[rank - 1];

            Item weapon = player.HeldItem;
            int baseDamage = weapon.damage;
            int skillDamage = (int)(baseDamage * damageMultiplier);
            DamageClass damageClass = weapon.DamageType;

            Vector2 strikeCenter = player.Center + new Vector2(player.direction * 40, 0);
            int hitCount = 0;

            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC npc = Main.npc[i];
                if (!npc.active || npc.friendly || npc.damage <= 0)
                    continue;

                float distance = Vector2.Distance(npc.Center, strikeCenter);
                if (distance > radius)
                    continue;

                int hitDirection = npc.Center.X >= player.Center.X ? 1 : -1;
                bool crit = RollCrit(player, damageClass);
                
                float totalKnockback = weapon.knockBack + knockbackBonus;
                float scaledDamage = GetScaledDamage(player, damageClass, skillDamage);
                int damage = ApplyDamageVariance(player, scaledDamage);

                player.ApplyDamageToNPC(npc, damage, totalKnockback, hitDirection, crit, damageClass, false);
                hitCount++;

                // Impact effect per enemy
                CreateImpactEffect(npc);
            }

            PlaySkillEffects(player, hitCount > 0);

            if (hitCount > 0)
            {
                ShowMessage(player, $"Power Strike! ({hitCount} hit)", Color.OrangeRed);
            }
        }

        private void CreateImpactEffect(NPC target)
        {
            for (int d = 0; d < 15; d++)
            {
                Dust dust = Dust.NewDustDirect(
                    target.position,
                    target.width,
                    target.height,
                    DustID.Torch,
                    Main.rand.NextFloat(-3f, 3f),
                    Main.rand.NextFloat(-3f, 3f),
                    100,
                    default,
                    1.5f
                );
                dust.noGravity = true;
            }
        }

        private void PlaySkillEffects(Player player, bool hitEnemy)
        {
            SoundEngine.PlaySound(SoundID.Item1 with { Pitch = -0.3f }, player.position);
            if (hitEnemy)
            {
                SoundEngine.PlaySound(SoundID.NPCHit1, player.position);
            }

            // Arc swing effect
            for (int i = 0; i < 20; i++)
            {
                float angle = player.direction > 0 
                    ? -0.8f + (i * 0.1f) 
                    : MathHelper.Pi + 0.8f - (i * 0.1f);
                Vector2 velocity = new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle)) * 4f;

                Dust dust = Dust.NewDustDirect(
                    player.Center + new Vector2(player.direction * 20, 0),
                    4,
                    4,
                    DustID.Torch,
                    velocity.X,
                    velocity.Y,
                    100,
                    default,
                    1.5f
                );
                dust.noGravity = true;
            }

            player.itemAnimation = 25;
            player.itemTime = 25;
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
