using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using RpgMod.Common.Base;

namespace RpgMod.Common.Skills.Tier2.Shadow
{
    /// <summary>
    /// Ambush - Shadow's surprise attack skill.
    /// </summary>
    public class Ambush : BaseSkill
    {
        public override string InternalName => "Ambush";
        public override string DisplayName => "Ambush";
        public override string Description => "Strike from the shadows with a guaranteed critical hit and bonus damage.";

        public override SkillType SkillType => SkillType.Active;
        public override JobType RequiredJob => JobType.Shadow;
        public override int RequiredLevel => RpgConstants.SECOND_JOB_LEVEL + 10;
        public override int SkillPointCost => 1;
        public override int MaxRank => 5;
        public override string IconTexture => $"{RpgConstants.ICON_PATH}Skills/Ambush";
        public override float CooldownSeconds => 14f;
        public override int ResourceCost => 22;
        public override ResourceType ResourceType => ResourceType.Stamina;

        private static readonly int[] DAMAGE = { 50, 68, 88, 112, 150 };

        protected override void OnActivate(Player player)
        {
            int rank = System.Math.Max(1, CurrentRank);
            int damage = DAMAGE[rank - 1];

            NPC target = FindNearestEnemy(player, 200f);
            if (target != null)
            {
                int dir = target.Center.X >= player.Center.X ? 1 : -1;
                float scaledDamage = GetScaledDamage(player, DamageClass.Melee, damage);
                int finalDamage = ApplyDamageVariance(player, scaledDamage);
                player.ApplyDamageToNPC(target, finalDamage, 8f, dir, true, DamageClass.Melee, false);
                PlayEffects(target);
                ShowMessage(target, $"-{finalDamage} Ambush!", Color.Red);
            }
            else
            {
                ShowMessage(player, "No Target!", Color.Gray);
            }
        }

        private NPC FindNearestEnemy(Player player, float range)
        {
            NPC nearest = null;
            float nearestDist = range;
            for (int i = 0; i < Main.maxNPCs; i++)
            {
                NPC npc = Main.npc[i];
                if (!npc.active || npc.friendly || npc.lifeMax <= 5) continue;
                float dist = Vector2.Distance(npc.Center, player.Center);
                if (dist < nearestDist)
                {
                    nearestDist = dist;
                    nearest = npc;
                }
            }
            return nearest;
        }

        private void PlayEffects(NPC target)
        {
            SoundEngine.PlaySound(SoundID.DD2_DarkMageHurt, target.position);
            for (int i = 0; i < 15; i++)
            {
                Dust dust = Dust.NewDustDirect(target.position, target.width, target.height,
                    DustID.Shadowflame, Main.rand.NextFloat(-4f, 4f), Main.rand.NextFloat(-4f, 4f), 100, Color.Purple, 1.5f);
                dust.noGravity = true;
            }
        }

        private void ShowMessage(Entity target, string text, Color color)
        {
            if (Main.netMode != NetmodeID.Server)
                CombatText.NewText(target.Hitbox, color, text);
        }
    }
}
