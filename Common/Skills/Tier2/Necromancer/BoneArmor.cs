using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using RpgMod.Common.Base;
using RpgMod.Common.Players;

namespace RpgMod.Common.Skills.Tier2.Necromancer
{
    /// <summary>
    /// Bone Armor - Necromancer's defensive skill.
    /// </summary>
    public class BoneArmor : BaseSkill
    {
        public override string InternalName => "BoneArmor";
        public override string DisplayName => "Bone Armor";
        public override string Description => "Surround yourself with bones for protection.";

        public override SkillType SkillType => SkillType.Active;
        public override JobType RequiredJob => JobType.Necromancer;
        public override int RequiredLevel => RpgConstants.SECOND_JOB_LEVEL + 10;
        public override int SkillPointCost => 1;
        public override int MaxRank => 5;
        public override string IconTexture => $"{RpgConstants.ICON_PATH}Skills/BoneArmor";
        public override float CooldownSeconds => 22f;
        public override int ResourceCost => 22;
        public override ResourceType ResourceType => ResourceType.Mana;

        private static readonly int[] DURATION_SECONDS = { 12, 15, 18, 21, 28 };
        private static readonly int[] DEFENSE_BONUS = { 10, 15, 20, 26, 35 };

        protected override void OnActivate(Player player)
        {
            int rank = System.Math.Max(1, CurrentRank);
            int duration = DURATION_SECONDS[rank - 1] * 60;
            int defense = DEFENSE_BONUS[rank - 1];

            var rpgPlayer = player.GetModPlayer<RpgPlayer>();
            rpgPlayer.AddTemporaryDefense(defense, duration);

            player.AddBuff(BuffID.Thorns, duration);

            PlayEffects(player);
            ShowMessage(player, "Bone Armor!", Color.LightGray);
        }

        private void PlayEffects(Player player)
        {
            SoundEngine.PlaySound(SoundID.NPCDeath2, player.position);
            for (int i = 0; i < 20; i++)
            {
                Dust dust = Dust.NewDustDirect(player.position, player.width, player.height,
                    DustID.Bone, Main.rand.NextFloat(-3f, 3f), Main.rand.NextFloat(-3f, 0f), 100, Color.LightGray, 1.3f);
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
