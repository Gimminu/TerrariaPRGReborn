using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using Rpg.Common.Base;

namespace Rpg.Common.Skills.Tier2.BattleMage
{
    /// <summary>
    /// Counter Spell - Battle Mage's reactive damage skill.
    /// </summary>
    public class CounterSpell : BaseSkill
    {
        public override string InternalName => "CounterSpell";
        public override string DisplayName => "Counter Spell";
        public override string Description => "Release stored magical energy as an explosion.";

        public override SkillType SkillType => SkillType.Active;
        public override JobType RequiredJob => JobType.BattleMage;
        public override int RequiredLevel => RpgConstants.SECOND_JOB_LEVEL + 10;
        public override int SkillPointCost => 1;
        public override int MaxRank => 5;
        public override string IconTexture => $"{RpgConstants.ICON_PATH}Skills/CounterSpell";
        public override float CooldownSeconds => 16f;
        public override int ResourceCost => 20;
        public override ResourceType ResourceType => ResourceType.Mana;

        private static readonly int[] DAMAGE = { 35, 50, 65, 85, 115 };
        private static readonly float[] RADIUS = { 100f, 120f, 140f, 160f, 200f };

        protected override void OnActivate(Player player)
        {
            int rank = Math.Max(1, CurrentRank);
            int damage = DAMAGE[rank - 1];
            float radius = RADIUS[rank - 1];

            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC npc = Main.npc[i];
                if (!npc.active || npc.friendly || npc.lifeMax <= 5) continue;
                if (Vector2.Distance(npc.Center, player.Center) > radius) continue;

                int dir = npc.Center.X >= player.Center.X ? 1 : -1;
                bool crit = Main.rand.NextFloat(100f) < player.GetCritChance(DamageClass.Magic);
                player.ApplyDamageToNPC(npc, damage, 6f, dir, crit, DamageClass.Magic, false);
            }

            PlayEffects(player, radius);
        }

        private void PlayEffects(Player player, float radius)
        {
            SoundEngine.PlaySound(SoundID.Item45, player.position);
            for (int i = 0; i < 40; i++)
            {
                float angle = MathHelper.TwoPi * i / 40f;
                Vector2 dir = new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle));
                Dust dust = Dust.NewDustDirect(player.Center + dir * radius * 0.3f, 4, 4,
                    DustID.MagicMirror, dir.X * 5f, dir.Y * 5f, 100, Color.Purple, 1.8f);
                dust.noGravity = true;
            }
        }
    }
}
