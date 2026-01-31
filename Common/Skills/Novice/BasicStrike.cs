using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using RpgMod.Common.Base;

namespace RpgMod.Common.Skills.Novice
{
    /// <summary>
    /// Basic Strike - Novice starting skill.
    /// A simple melee attack that deals bonus damage and knocks back enemies.
    /// </summary>
    public class BasicStrike : BaseSkill
    {
        public override string InternalName => "BasicStrike";
        public override string DisplayName => "Basic Strike";
        public override string Description => "A powerful melee strike that deals bonus weapon damage and knocks back enemies.";

        public override SkillType SkillType => SkillType.Active;
        public override JobType RequiredJob => JobType.Novice;
        public override int RequiredLevel => 1;
        public override int SkillPointCost => 1;
        public override int MaxRank => 5;
        public override string IconTexture => $"{RpgConstants.ICON_PATH}Skills/BasicStrike";
        public override float CooldownSeconds => 3f;
        public override int ResourceCost => 15;
        public override ResourceType ResourceType => ResourceType.Stamina;

        private const int UseAnimation = 20;
        private const int UseTime = 20;

        private static readonly float[] DAMAGE_MULTIPLIER = { 1.5f, 1.65f, 1.8f, 2.0f, 2.25f };
        private static readonly float[] KNOCKBACK_BONUS = { 2f, 2.5f, 3f, 3.5f, 4f };

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

            Item weapon = player.HeldItem;
            int baseDamage = weapon.damage;
            int skillDamage = (int)(baseDamage * damageMultiplier);
            DamageClass damageClass = weapon.DamageType;

            Vector2 strikePosition = player.Center + new Vector2(player.direction * 40, 0);
            int strikeRadius = 60;

            bool hitEnemy = false;
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC npc = Main.npc[i];
                if (!npc.active || npc.friendly || npc.damage <= 0)
                    continue;

                float distance = Vector2.Distance(npc.Center, strikePosition);
                if (distance > strikeRadius)
                    continue;

                Vector2 knockbackDir = npc.Center - player.Center;
                if (knockbackDir != Vector2.Zero)
                    knockbackDir.Normalize();

                float totalKnockback = weapon.knockBack + knockbackBonus;
                float scaledDamage = GetScaledDamage(player, damageClass, skillDamage);
                int damage = ApplyDamageVariance(player, scaledDamage);
                bool crit = RollCrit(player, damageClass);

                player.ApplyDamageToNPC(npc, damage, totalKnockback, player.direction, crit, damageClass, false);
                npc.velocity += knockbackDir * knockbackBonus;

                hitEnemy = true;

                for (int d = 0; d < 10; d++)
                {
                    Dust dust = Dust.NewDustDirect(
                        npc.position,
                        npc.width,
                        npc.height,
                        DustID.Iron,
                        knockbackDir.X * 2f,
                        knockbackDir.Y * 2f,
                        100,
                        default,
                        1.5f
                    );
                    dust.noGravity = true;
                }
            }

            PlaySkillEffects(player, hitEnemy);

            if (hitEnemy)
            {
                ShowMessage(player, $"Strike! ({skillDamage} damage)", Color.Yellow);
            }
        }

        private void PlaySkillEffects(Player player, bool hitEnemy)
        {
            SoundEngine.PlaySound(SoundID.Item1, player.position);
            if (hitEnemy)
            {
                SoundEngine.PlaySound(SoundID.NPCHit1, player.position);
            }

            Vector2 weaponPos = player.Center + new Vector2(player.direction * 30, -10);
            for (int i = 0; i < 8; i++)
            {
                float angle = player.direction > 0 ? -0.5f + (i * 0.15f) : 3.14f + 0.5f - (i * 0.15f);
                Vector2 velocity = new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle)) * 3f;

                Dust dust = Dust.NewDustDirect(
                    weaponPos,
                    4,
                    4,
                    DustID.Iron,
                    velocity.X,
                    velocity.Y,
                    100,
                    Color.White,
                    1.2f
                );
                dust.noGravity = true;
            }

            player.itemAnimation = UseAnimation;
            player.itemTime = UseTime;
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
