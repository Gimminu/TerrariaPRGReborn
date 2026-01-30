using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Rpg.Common.Base;

namespace Rpg.Common.Skills.Tier2.Shadow
{
    /// <summary>
    /// Shadow Cloak - 그림자 외투.
    /// 투명화 + 회피 증가.
    /// 그림자의 은신 버프. 만렙 시 쿨 = 지속 (18초).
    /// </summary>
    public class ShadowCloak : BaseSkill
    {
        public override string InternalName => "ShadowCloak";
        public override string DisplayName => "Shadow Cloak";
        public override string Description => "Cloak yourself in shadows, becoming invisible and evasive.";

        public override SkillType SkillType => SkillType.Buff;
        public override JobType RequiredJob => JobType.Shadow;
        public override int RequiredLevel => 65;
        public override int SkillPointCost => 1;
        public override int MaxRank => 10;
        public override string IconTexture => $"{RpgConstants.ICON_PATH}Skills/ShadowCloak";
        
        // 만렙 시 쿨 = 지속 (18초)
        public override float CooldownSeconds => 28f - (CurrentRank * 1f); // 28 -> 18초
        public override int ResourceCost => 25;
        public override ResourceType ResourceType => ResourceType.Stamina;

        private static readonly int[] DURATION_SECONDS = { 10, 11, 12, 12, 13, 14, 15, 15, 16, 18 };

        protected override void OnActivate(Player player)
        {
            int rank = System.Math.Max(1, CurrentRank);
            int duration = DURATION_SECONDS[rank - 1] * 60;
            
            player.AddBuff(BuffID.Invisibility, duration);
            player.AddBuff(BuffID.Swiftness, duration);
            
            PlayEffects(player);
            ShowMessage(player, "Shadow Cloak!", Color.DarkViolet);
        }

        private void PlayEffects(Player player)
        {
            SoundEngine.PlaySound(SoundID.Item8, player.position);
            
            for (int i = 0; i < 30; i++)
            {
                Dust dust = Dust.NewDustDirect(player.position, player.width, player.height,
                    DustID.Shadowflame, Main.rand.NextFloat(-3f, 3f), Main.rand.NextFloat(-3f, 0f), 200, Color.Black, 1.2f);
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
