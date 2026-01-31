using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using RpgMod.Common.Base;
using RpgMod.Common.Players;

namespace RpgMod.Common.Skills.Tier2.Sorcerer
{
    /// <summary>
    /// Elemental Barrier - Sorcerer's defensive skill.
    /// </summary>
    public class ElementalBarrier : BaseSkill
    {
        public override string InternalName => "ElementalBarrier";
        public override string DisplayName => "Elemental Barrier";
        public override string Description => "Surround yourself with an elemental barrier, reducing damage and boosting magic power.";

        public override SkillType SkillType => SkillType.Active;
        public override JobType RequiredJob => JobType.Sorcerer;
        public override int RequiredLevel => RpgConstants.SECOND_JOB_LEVEL + 10;
        public override int SkillPointCost => 1;
        public override int MaxRank => 5;
        public override string IconTexture => $"{RpgConstants.ICON_PATH}Skills/ElementalBarrier";
        public override float CooldownSeconds => 22f;
        public override int ResourceCost => 25;
        public override ResourceType ResourceType => ResourceType.Mana;

        private static readonly int[] DURATION_SECONDS = { 10, 12, 14, 16, 20 };
        private static readonly float[] DAMAGE_REDUCTION = { 0.10f, 0.14f, 0.18f, 0.22f, 0.30f };

        protected override void OnActivate(Player player)
        {
            int rank = System.Math.Max(1, CurrentRank);
            int duration = DURATION_SECONDS[rank - 1] * 60;
            float reduction = DAMAGE_REDUCTION[rank - 1];

            var rpgPlayer = player.GetModPlayer<RpgPlayer>();
            rpgPlayer.AddTemporaryDamageReduction(reduction, duration);

            player.AddBuff(BuffID.MagicPower, duration);

            PlayEffects(player);
            ShowMessage(player, "Barrier Active!", Color.Cyan);
        }

        private void PlayEffects(Player player)
        {
            SoundEngine.PlaySound(SoundID.Item25, player.position);
            for (int i = 0; i < 30; i++)
            {
                float angle = MathHelper.TwoPi * i / 30f;
                Vector2 offset = new Vector2((float)System.Math.Cos(angle), (float)System.Math.Sin(angle)) * 40f;
                Dust dust = Dust.NewDustDirect(player.Center + offset, 4, 4, DustID.IceTorch, 0, 0, 100, Color.Cyan, 1.5f);
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
