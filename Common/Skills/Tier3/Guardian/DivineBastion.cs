using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Rpg.Common.Base;
using Rpg.Common.Players;

namespace Rpg.Common.Skills.Tier3.Guardian
{
    /// <summary>
    /// Divine Bastion - 신성 방벽.
    /// 모든 피해 대폭 감소.
    /// 가디언의 궁극 방어기. 강한 버프: 쿨 > 지속.
    /// </summary>
    public class DivineBastion : BaseSkill
    {
        public override string InternalName => "DivineBastion";
        public override string DisplayName => "Divine Bastion";
        public override string Description => "Create an impenetrable divine barrier, greatly reducing all damage.";

        public override SkillType SkillType => SkillType.Buff;
        public override JobType RequiredJob => JobType.Guardian;
        public override int RequiredLevel => 120;
        public override int SkillPointCost => 1;
        public override int MaxRank => 10;
        public override string IconTexture => $"{RpgConstants.ICON_PATH}Skills/DivineBastion";
        
        // 강한 버프: 쿨 > 지속
        public override float CooldownSeconds => 50f - (CurrentRank * 1.5f); // 50 -> 35초
        public override int ResourceCost => 50;
        public override ResourceType ResourceType => ResourceType.Mana;

        private static readonly int[] DURATION_SECONDS = { 6, 7, 8, 8, 9, 9, 10, 10, 11, 12 };
        private static readonly int[] DEFENSE_BONUS = { 30, 38, 46, 55, 65, 76, 88, 102, 118, 150 };

        protected override void OnActivate(Player player)
        {
            int rank = System.Math.Max(1, CurrentRank);
            int durationTicks = DURATION_SECONDS[rank - 1] * 60;
            
            player.AddBuff(BuffID.Ironskin, durationTicks);
            player.AddBuff(BuffID.Endurance, durationTicks);
            
            var rpg = player.GetModPlayer<RpgPlayer>();
            rpg.AddTemporaryDefense(DEFENSE_BONUS[rank - 1], durationTicks);
            
            PlayEffects(player);
            ShowMessage(player, "Divine Bastion!", Color.Gold);
        }

        private void PlayEffects(Player player)
        {
            SoundEngine.PlaySound(SoundID.Item29 with { Pitch = 0.5f }, player.position);
            
            for (int i = 0; i < 40; i++)
            {
                float angle = (float)i / 40f * MathHelper.TwoPi;
                Vector2 offset = new Vector2((float)System.Math.Cos(angle), (float)System.Math.Sin(angle)) * 60f;
                
                Dust dust = Dust.NewDustDirect(player.Center + offset, 6, 6, DustID.GoldFlame,
                    0f, 0f, 150, Color.Gold, 1.5f);
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
