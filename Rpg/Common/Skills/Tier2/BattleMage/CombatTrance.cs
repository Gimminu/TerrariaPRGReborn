using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Rpg.Common.Base;
using Rpg.Common.Players;

namespace Rpg.Common.Skills.Tier2.BattleMage
{
    /// <summary>
    /// Combat Trance - 전투 무아지경.
    /// 피해와 방어 모두 증가.
    /// 배틀메이지의 하이브리드 버프. 만렙 시 쿨 = 지속 (20초).
    /// </summary>
    public class CombatTrance : BaseSkill
    {
        public override string InternalName => "CombatTrance";
        public override string DisplayName => "Combat Trance";
        public override string Description => "Enter a combat trance, increasing damage and defense.";

        public override SkillType SkillType => SkillType.Buff;
        public override JobType RequiredJob => JobType.BattleMage;
        public override int RequiredLevel => 68;
        public override int SkillPointCost => 1;
        public override int MaxRank => 10;
        public override string IconTexture => $"{RpgConstants.ICON_PATH}Skills/CombatTrance";
        
        // 만렙 시 쿨 = 지속 (20초)
        public override float CooldownSeconds => 30f - (CurrentRank * 1f); // 30 -> 20초
        public override int ResourceCost => 35;
        public override ResourceType ResourceType => ResourceType.Mana;

        private static readonly int[] DURATION_SECONDS = { 12, 13, 14, 15, 16, 16, 17, 18, 19, 20 };
        private static readonly int[] DEFENSE_BONUS = { 5, 7, 9, 11, 14, 17, 20, 24, 28, 35 };

        protected override void OnActivate(Player player)
        {
            int rank = System.Math.Max(1, CurrentRank);
            int durationTicks = DURATION_SECONDS[rank - 1] * 60;
            
            player.AddBuff(BuffID.Rage, durationTicks);
            player.AddBuff(BuffID.Wrath, durationTicks);
            
            var rpg = player.GetModPlayer<RpgPlayer>();
            rpg.AddTemporaryDefense(DEFENSE_BONUS[rank - 1], durationTicks);
            
            PlayEffects(player);
            ShowMessage(player, "Combat Trance!", Color.OrangeRed);
        }

        private void PlayEffects(Player player)
        {
            SoundEngine.PlaySound(SoundID.Item29, player.position);
            
            for (int i = 0; i < 30; i++)
            {
                Dust dust = Dust.NewDustDirect(player.position, player.width, player.height,
                    DustID.Torch, Main.rand.NextFloat(-3f, 3f), Main.rand.NextFloat(-3f, 0f), 150, Color.OrangeRed, 1.2f);
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
