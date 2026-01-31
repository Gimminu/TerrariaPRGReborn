using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using RpgMod.Common.Base;

namespace RpgMod.Common.Skills.Tier2.Spellblade
{
    /// <summary>
    /// Arcane Slash - Spellblade's magic-infused melee attack.
    /// </summary>
    public class ArcaneSlash : BaseSkill
    {
        public override string InternalName => "ArcaneSlash";
        public override string DisplayName => "Arcane Slash";
        public override string Description => "Unleash a wide arc of arcane energy.";

        public override SkillType SkillType => SkillType.Active;
        public override JobType RequiredJob => JobType.Spellblade;
        public override int RequiredLevel => RpgConstants.SECOND_JOB_LEVEL + 10;
        public override int SkillPointCost => 1;
        public override int MaxRank => 5;
        public override string IconTexture => $"{RpgConstants.ICON_PATH}Skills/ArcaneSlash";
        public override float CooldownSeconds => 10f;
        public override int ResourceCost => 18;
        public override ResourceType ResourceType => ResourceType.Mana;

        private static readonly int[] DAMAGE = { 40, 55, 70, 90, 120 };
        private static readonly float[] ARC_RANGE = { 100f, 120f, 140f, 160f, 200f };

        protected override void OnActivate(Player player)
        {
            int rank = Math.Max(1, CurrentRank);
            int damage = DAMAGE[rank - 1];
            float range = ARC_RANGE[rank - 1];

            Vector2 direction = (Main.MouseWorld - player.Center).SafeNormalize(Vector2.UnitX);
            float baseAngle = (float)Math.Atan2(direction.Y, direction.X);

            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC npc = Main.npc[i];
                if (!npc.active || npc.friendly || npc.lifeMax <= 5) continue;

                float dist = Vector2.Distance(npc.Center, player.Center);
                if (dist > range) continue;

                Vector2 toEnemy = (npc.Center - player.Center).SafeNormalize(Vector2.Zero);
                float enemyAngle = (float)Math.Atan2(toEnemy.Y, toEnemy.X);
                float angleDiff = Math.Abs(MathHelper.WrapAngle(enemyAngle - baseAngle));

                if (angleDiff <= MathHelper.PiOver4)
                {
                    int dir = npc.Center.X >= player.Center.X ? 1 : -1;
                    float scaledDamage = GetScaledDamage(player, DamageClass.Melee, damage);
                    bool crit = RollCrit(player, DamageClass.Melee);
                    int finalDamage = ApplyDamageVariance(player, scaledDamage);
                    player.ApplyDamageToNPC(npc, finalDamage, 5f, dir, crit, DamageClass.Melee, false);
                }
            }

            PlayEffects(player, direction, range);
        }

        private void PlayEffects(Player player, Vector2 direction, float range)
        {
            SoundEngine.PlaySound(SoundID.Item71, player.position);
            float baseAngle = (float)Math.Atan2(direction.Y, direction.X);

            for (int i = 0; i < 15; i++)
            {
                float angle = baseAngle + MathHelper.Lerp(-MathHelper.PiOver4, MathHelper.PiOver4, i / 14f);
                Vector2 dir = new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle));
                for (int j = 0; j < 5; j++)
                {
                    Vector2 pos = player.Center + dir * (range * j / 5f);
                    Dust dust = Dust.NewDustDirect(pos, 4, 4, DustID.MagicMirror, dir.X * 2f, dir.Y * 2f, 100, Color.Cyan, 1.5f);
                    dust.noGravity = true;
                }
            }
        }
    }
}
