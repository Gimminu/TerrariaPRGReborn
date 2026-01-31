using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using RpgMod.Common.Base;

namespace RpgMod.Common.Skills.Tier2.DeathKnight
{
    /// <summary>
    /// Death Coil - Death Knight's damage + heal skill.
    /// </summary>
    public class DeathCoil : BaseSkill
    {
        public override string InternalName => "DeathCoil";
        public override string DisplayName => "Death Coil";
        public override string Description => "Damage enemies and siphon their life force.";

        public override SkillType SkillType => SkillType.Active;
        public override JobType RequiredJob => JobType.DeathKnight;
        public override int RequiredLevel => RpgConstants.SECOND_JOB_LEVEL;
        public override int SkillPointCost => 1;
        public override int MaxRank => 5;
        public override string IconTexture => $"{RpgConstants.ICON_PATH}Skills/DeathCoil";
        public override float CooldownSeconds => 18f;
        public override int ResourceCost => 25;
        public override ResourceType ResourceType => ResourceType.Mana;

        private static readonly int[] DAMAGE = { 35, 45, 55, 70, 90 };
        private static readonly float[] RADIUS = { 110f, 120f, 130f, 140f, 160f };
        private static readonly int[] HEAL_PER_HIT = { 8, 12, 16, 20, 28 };

        protected override void OnActivate(Player player)
        {
            int rank = Math.Max(1, CurrentRank);
            int damage = DAMAGE[rank - 1];
            float radius = RADIUS[rank - 1];
            int healPerHit = HEAL_PER_HIT[rank - 1];

            int totalHeal = 0;
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC npc = Main.npc[i];
                if (!npc.active || npc.friendly || npc.lifeMax <= 5) continue;
                if (Vector2.Distance(npc.Center, player.Center) > radius) continue;

                int dir = npc.Center.X >= player.Center.X ? 1 : -1;
                float scaledDamage = GetScaledDamage(player, DamageClass.Magic, damage);
                bool crit = RollCrit(player, DamageClass.Magic);
                int finalDamage = ApplyDamageVariance(player, scaledDamage);
                player.ApplyDamageToNPC(npc, finalDamage, 3f, dir, crit, DamageClass.Magic, false);
                totalHeal += healPerHit;
                CreateDrainEffect(player, npc);
            }

            if (totalHeal > 0)
            {
                player.statLife = Math.Min(player.statLife + totalHeal, player.statLifeMax2);
                ShowMessage(player, $"+{totalHeal} HP", Color.DarkRed);
            }

            PlayEffects(player, radius);
        }

        private void CreateDrainEffect(Player player, NPC target)
        {
            Vector2 dir = (player.Center - target.Center).SafeNormalize(Vector2.Zero);
            for (int i = 0; i < 8; i++)
            {
                Dust dust = Dust.NewDustDirect(target.Center, 4, 4, DustID.LifeDrain, dir.X * 4f, dir.Y * 4f, 100, default, 1.2f);
                dust.noGravity = true;
            }
        }

        private void PlayEffects(Player player, float radius)
        {
            SoundEngine.PlaySound(SoundID.NPCDeath6, player.position);
            for (int i = 0; i < 30; i++)
            {
                float angle = MathHelper.TwoPi * i / 30f;
                Vector2 dir = new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle));
                Dust dust = Dust.NewDustDirect(player.Center + dir * radius * 0.5f, 4, 4, DustID.Shadowflame, dir.X * 2f, dir.Y * 2f, 100, Color.Purple, 1.5f);
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
