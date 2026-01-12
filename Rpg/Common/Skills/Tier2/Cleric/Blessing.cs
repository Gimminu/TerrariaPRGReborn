using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Rpg.Common.Base;

namespace Rpg.Common.Skills.Tier2.Cleric
{
    /// <summary>
    /// Blessing - 축복.
    /// 모든 능력치 소폭 증가.
    /// 만렙 시 쿨 = 지속
    /// </summary>
    public class Blessing : BaseSkill
    {
        public override string InternalName => "Blessing";
        public override string DisplayName => "Blessing";
        public override string Description => "Bestow a divine blessing, increasing all stats.";

        public override SkillType SkillType => SkillType.Buff;
        public override JobType RequiredJob => JobType.Cleric;
        public override int RequiredLevel => 65;
        public override int SkillPointCost => 1;
        public override int MaxRank => 10;
        public override string IconTexture => $"{RpgConstants.ICON_PATH}Skills/Blessing";
        
        public override float CooldownSeconds => 50f - (CurrentRank * 2f); // 50 -> 30초
        public override int ResourceCost => 50;
        public override ResourceType ResourceType => ResourceType.Mana;

        private static readonly int[] DURATION_SECONDS = { 15, 18, 20, 22, 24, 26, 27, 28, 29, 30 };

        protected override void OnActivate(Player player)
        {
            int rank = System.Math.Max(1, CurrentRank);
            int duration = DURATION_SECONDS[rank - 1] * 60;

            player.AddBuff(BuffID.Ironskin, duration);
            player.AddBuff(BuffID.Regeneration, duration);
            player.AddBuff(BuffID.Swiftness, duration);
            player.AddBuff(BuffID.ManaRegeneration, duration);

            PlayEffects(player);
        }

        private void PlayEffects(Player player)
        {
            SoundEngine.PlaySound(SoundID.Item4, player.position);
            
            for (int i = 0; i < 40; i++)
            {
                Dust dust = Dust.NewDustDirect(player.position, player.width, player.height,
                    DustID.GoldCoin, Main.rand.NextFloat(-3f, 3f), Main.rand.NextFloat(-3f, -1f), 100, Color.Gold, 1.0f);
                dust.noGravity = true;
            }
        }
    }
}
