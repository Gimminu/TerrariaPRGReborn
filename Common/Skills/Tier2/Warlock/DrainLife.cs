using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using RpgMod.Common.Base;

namespace RpgMod.Common.Skills.Tier2.Warlock
{
    /// <summary>
    /// Drain Life - Warlock's life steal spell.
    /// </summary>
    public class DrainLife : BaseSkill
    {
        public override string InternalName => "DrainLife";
        public override string DisplayName => "Drain Life";
        public override string Description => "Drain life from nearby enemies.";

        public override SkillType SkillType => SkillType.Active;
        public override JobType RequiredJob => JobType.Warlock;
        public override int RequiredLevel => RpgConstants.SECOND_JOB_LEVEL + 10;
        public override int SkillPointCost => 1;
        public override int MaxRank => 5;
        public override string IconTexture => $"{RpgConstants.ICON_PATH}Skills/DrainLife";
        public override float CooldownSeconds => 18f;
        public override int ResourceCost => 25;
        public override ResourceType ResourceType => ResourceType.Mana;

        private static readonly int[] DAMAGE = { 25, 35, 45, 55, 75 };
        private static readonly float[] LIFESTEAL_PERCENT = { 0.30f, 0.35f, 0.40f, 0.45f, 0.55f };
        private static readonly float RANGE = 150f;

        protected override void OnActivate(Player player)
        {
            int rank = Math.Max(1, CurrentRank);
            int damage = DAMAGE[rank - 1];
            float lifesteal = LIFESTEAL_PERCENT[rank - 1];

            int totalHeal = 0;
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC npc = Main.npc[i];
                if (!npc.active || npc.friendly || npc.lifeMax <= 5) continue;
                if (Vector2.Distance(npc.Center, player.Center) > RANGE) continue;

                int dir = npc.Center.X >= player.Center.X ? 1 : -1;
                bool crit = RollCrit(player, DamageClass.Magic);
                float scaledDamage = GetScaledDamage(player, DamageClass.Magic, damage);
                int finalDamage = ApplyDamageVariance(player, scaledDamage);
                player.ApplyDamageToNPC(npc, finalDamage, 2f, dir, crit, DamageClass.Magic, false);

                int heal = (int)(finalDamage * lifesteal);
                totalHeal += heal;
                CreateDrainEffect(player, npc);
            }

            if (totalHeal > 0)
            {
                player.statLife = Math.Min(player.statLife + totalHeal, player.statLifeMax2);
                ShowMessage(player, $"+{totalHeal} HP", Color.Green);
            }

            PlayEffects(player);
        }

        private void CreateDrainEffect(Player player, NPC target)
        {
            Vector2 dir = (player.Center - target.Center).SafeNormalize(Vector2.Zero);
            for (int i = 0; i < 8; i++)
            {
                Dust dust = Dust.NewDustDirect(target.Center, 4, 4, DustID.LifeDrain, dir.X * 3f, dir.Y * 3f, 100, default, 1.2f);
                dust.noGravity = true;
            }
        }

        private void PlayEffects(Player player)
        {
            SoundEngine.PlaySound(SoundID.Item103, player.position);
        }

        private void ShowMessage(Player player, string text, Color color)
        {
            if (Main.netMode != NetmodeID.Server)
                CombatText.NewText(player.Hitbox, color, text);
        }
    }
}
