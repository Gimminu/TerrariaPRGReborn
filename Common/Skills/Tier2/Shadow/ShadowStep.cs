using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using RpgMod.Common.Base;
using RpgMod.Common.Players;

namespace RpgMod.Common.Skills.Tier2.Shadow
{
    /// <summary>
    /// Shadow Step - Shadow's teleport skill.
    /// </summary>
    public class ShadowStep : BaseSkill
    {
        public override string InternalName => "ShadowStep";
        public override string DisplayName => "Shadow Step";
        public override string Description => "Teleport to the cursor position through the shadows.";

        public override SkillType SkillType => SkillType.Active;
        public override JobType RequiredJob => JobType.Shadow;
        public override int RequiredLevel => RpgConstants.SECOND_JOB_LEVEL;
        public override int SkillPointCost => 1;
        public override int MaxRank => 5;
        public override string IconTexture => $"{RpgConstants.ICON_PATH}Skills/ShadowStep";
        public override float CooldownSeconds => 12f;
        public override int ResourceCost => 18;
        public override ResourceType ResourceType => ResourceType.Stamina;

        private static readonly float[] MAX_DISTANCE = { 200f, 250f, 300f, 350f, 450f };

        protected override void OnActivate(Player player)
        {
            int rank = System.Math.Max(1, CurrentRank);
            float maxDist = MAX_DISTANCE[rank - 1];

            Vector2 targetPos = Main.MouseWorld;
            Vector2 direction = (targetPos - player.Center).SafeNormalize(Vector2.UnitX);
            float distance = Vector2.Distance(targetPos, player.Center);
            if (distance > maxDist) distance = maxDist;

            Vector2 newPos = player.Center + direction * distance;
            newPos.X -= player.width / 2f;
            newPos.Y -= player.height / 2f;

            PlayDisappearEffects(player);
            player.Teleport(newPos, 1);
            PlayAppearEffects(player);
        }

        private void PlayDisappearEffects(Player player)
        {
            SoundEngine.PlaySound(SoundID.Item8, player.position);
            for (int i = 0; i < 20; i++)
            {
                Dust dust = Dust.NewDustDirect(player.position, player.width, player.height,
                    DustID.Smoke, Main.rand.NextFloat(-3f, 3f), Main.rand.NextFloat(-3f, 3f), 150, Color.DarkGray, 1.5f);
                dust.noGravity = true;
            }
        }

        private void PlayAppearEffects(Player player)
        {
            for (int i = 0; i < 20; i++)
            {
                Dust dust = Dust.NewDustDirect(player.position, player.width, player.height,
                    DustID.Shadowflame, Main.rand.NextFloat(-3f, 3f), Main.rand.NextFloat(-3f, 3f), 100, Color.Purple, 1.5f);
                dust.noGravity = true;
            }
        }
    }
}
