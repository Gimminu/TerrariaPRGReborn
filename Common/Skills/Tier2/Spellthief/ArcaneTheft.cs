using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using RpgMod.Common.Base;

namespace RpgMod.Common.Skills.Tier2.Spellthief
{
    /// <summary>
    /// Arcane Theft - Spellthief's damage + mana steal skill.
    /// </summary>
    public class ArcaneTheft : BaseSkill
    {
        public override string InternalName => "ArcaneTheft";
        public override string DisplayName => "Arcane Theft";
        public override string Description => "Damage enemies and steal their mana.";

        public override SkillType SkillType => SkillType.Active;
        public override JobType RequiredJob => JobType.Spellthief;
        public override int RequiredLevel => RpgConstants.SECOND_JOB_LEVEL + 10;
        public override int SkillPointCost => 1;
        public override int MaxRank => 5;
        public override string IconTexture => $"{RpgConstants.ICON_PATH}Skills/ArcaneTheft";
        public override float CooldownSeconds => 14f;
        public override int ResourceCost => 15;
        public override ResourceType ResourceType => ResourceType.Mana;

        private static readonly int[] DAMAGE = { 30, 42, 55, 70, 95 };
        private static readonly int[] MANA_STEAL = { 10, 14, 18, 22, 30 };
        private static readonly float RANGE = 150f;

        protected override void OnActivate(Player player)
        {
            int rank = Math.Max(1, CurrentRank);
            int damage = DAMAGE[rank - 1];
            int manaSteal = MANA_STEAL[rank - 1];

            int totalMana = 0;
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC npc = Main.npc[i];
                if (!npc.active || npc.friendly || npc.lifeMax <= 5) continue;
                if (Vector2.Distance(npc.Center, player.Center) > RANGE) continue;

                int dir = npc.Center.X >= player.Center.X ? 1 : -1;
                float scaledDamage = GetScaledDamage(player, DamageClass.Magic, damage);
                bool crit = RollCrit(player, DamageClass.Magic);
                int finalDamage = ApplyDamageVariance(player, scaledDamage);
                player.ApplyDamageToNPC(npc, finalDamage, 3f, dir, crit, DamageClass.Magic, false);
                totalMana += manaSteal;
                CreateStealEffect(player, npc);
            }

            if (totalMana > 0)
            {
                player.statMana = Math.Min(player.statMana + totalMana, player.statManaMax2);
                ShowMessage(player, $"+{totalMana} MP", Color.Blue);
            }

            PlayEffects(player);
        }

        private void CreateStealEffect(Player player, NPC target)
        {
            Vector2 dir = (player.Center - target.Center).SafeNormalize(Vector2.Zero);
            for (int i = 0; i < 6; i++)
            {
                Dust dust = Dust.NewDustDirect(target.Center, 4, 4, DustID.MagicMirror, dir.X * 3f, dir.Y * 3f, 100, Color.Blue, 1.2f);
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
