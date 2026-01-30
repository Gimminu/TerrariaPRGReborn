using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Rpg.Common.Base;

namespace Rpg.Common.Skills.Tier2.Druid
{
    /// <summary>
    /// Nature's Blessing - 자연의 축복.
    /// 회복과 재생 버프.
    /// 드루이드의 힐/버프. 만렙 시 쿨 = 지속 (25초).
    /// </summary>
    public class NaturesBlessing : BaseSkill
    {
        public override string InternalName => "NaturesBlessing";
        public override string DisplayName => "Nature's Blessing";
        public override string Description => "Call upon nature to heal and regenerate.";

        public override SkillType SkillType => SkillType.Buff;
        public override JobType RequiredJob => JobType.Druid;
        public override int RequiredLevel => 60;
        public override int SkillPointCost => 1;
        public override int MaxRank => 10;
        public override string IconTexture => $"{RpgConstants.ICON_PATH}Skills/NaturesBlessing";
        
        // 만렙 시 쿨 = 지속 (25초)
        public override float CooldownSeconds => 35f - (CurrentRank * 1f); // 35 -> 25초
        public override int ResourceCost => 30;
        public override ResourceType ResourceType => ResourceType.Mana;

        private static readonly int[] HEAL_AMOUNT = { 30, 40, 50, 65, 80, 100, 125, 155, 190, 250 };
        private static readonly int[] DURATION_SECONDS = { 15, 16, 17, 18, 19, 20, 21, 22, 23, 25 };

        protected override void OnActivate(Player player)
        {
            int rank = System.Math.Max(1, CurrentRank);
            int duration = DURATION_SECONDS[rank - 1] * 60;
            
            player.Heal(HEAL_AMOUNT[rank - 1]);
            player.AddBuff(BuffID.Regeneration, duration);
            player.AddBuff(BuffID.ManaRegeneration, duration);
            player.AddBuff(BuffID.Thorns, duration);
            
            PlayEffects(player);
            ShowMessage(player, "Nature's Blessing!", Color.LimeGreen);
        }

        private void PlayEffects(Player player)
        {
            SoundEngine.PlaySound(SoundID.Item4, player.position);
            
            for (int i = 0; i < 30; i++)
            {
                Dust dust = Dust.NewDustDirect(player.position, player.width, player.height,
                    DustID.GreenFairy, Main.rand.NextFloat(-2f, 2f), Main.rand.NextFloat(-3f, 0f), 150, Color.LimeGreen, 1.2f);
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
