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
    /// Corruption - Warlock's damage over time curse.
    /// </summary>
    public class Corruption : BaseSkill
    {
        public override string InternalName => "Corruption";
        public override string DisplayName => "Corruption";
        public override string Description => "Curse enemies with a corrupting darkness that deals damage over time.";

        public override SkillType SkillType => SkillType.Active;
        public override JobType RequiredJob => JobType.Warlock;
        public override int RequiredLevel => RpgConstants.SECOND_JOB_LEVEL;
        public override int SkillPointCost => 1;
        public override int MaxRank => 5;
        public override string IconTexture => $"{RpgConstants.ICON_PATH}Skills/Corruption";
        public override float CooldownSeconds => 12f;
        public override int ResourceCost => 20;
        public override ResourceType ResourceType => ResourceType.Mana;

        private static readonly float[] RADIUS = { 120f, 140f, 160f, 180f, 220f };
        private static readonly int[] DOT_DURATION = { 180, 210, 240, 270, 360 };

        protected override void OnActivate(Player player)
        {
            int rank = Math.Max(1, CurrentRank);
            float radius = RADIUS[rank - 1];
            int duration = DOT_DURATION[rank - 1];

            int hitCount = 0;
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC npc = Main.npc[i];
                if (!npc.active || npc.friendly || npc.lifeMax <= 5) continue;
                if (Vector2.Distance(npc.Center, player.Center) > radius) continue;

                npc.AddBuff(BuffID.ShadowFlame, duration);
                npc.AddBuff(BuffID.Ichor, duration);
                CreateCorruptEffect(npc);
                hitCount++;
            }

            PlayEffects(player, radius);
            ShowMessage(player, $"Corrupted {hitCount}!", Color.DarkMagenta);
        }

        private void CreateCorruptEffect(NPC target)
        {
            for (int i = 0; i < 10; i++)
            {
                Dust dust = Dust.NewDustDirect(target.position, target.width, target.height,
                    DustID.Shadowflame, Main.rand.NextFloat(-2f, 2f), Main.rand.NextFloat(-2f, 2f), 100, Color.Purple, 1.3f);
                dust.noGravity = true;
            }
        }

        private void PlayEffects(Player player, float radius)
        {
            SoundEngine.PlaySound(SoundID.NPCDeath52, player.position);
            for (int i = 0; i < 30; i++)
            {
                float angle = MathHelper.TwoPi * i / 30f;
                Vector2 dir = new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle));
                Dust dust = Dust.NewDustDirect(player.Center + dir * radius * 0.7f, 4, 4,
                    DustID.Shadowflame, dir.X, dir.Y, 100, default, 1.5f);
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
