using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Rpg.Common.Base;

namespace Rpg.Common.Skills.Tier2.Spellthief
{
    /// <summary>
    /// Spell Snatch - 마법 탈취.
    /// 잠시 마법 피해 대폭 증가 + 마나 비용 감소.
    /// 스펠시프의 버프. 강한 버프: 쿨 > 지속.
    /// </summary>
    public class SpellSnatch : BaseSkill
    {
        public override string InternalName => "SpellSnatch";
        public override string DisplayName => "Spell Snatch";
        public override string Description => "Temporarily boost magic damage and reduce mana cost.";

        public override SkillType SkillType => SkillType.Buff;
        public override JobType RequiredJob => JobType.Spellthief;
        public override int RequiredLevel => 68;
        public override int SkillPointCost => 1;
        public override int MaxRank => 10;
        public override string IconTexture => $"{RpgConstants.ICON_PATH}Skills/SpellSnatch";
        
        // 강한 버프: 쿨 > 지속
        public override float CooldownSeconds => 30f - (CurrentRank * 0.8f); // 30 -> 22초
        public override int ResourceCost => 30;
        public override ResourceType ResourceType => ResourceType.Mana;

        private static readonly int[] DURATION_SECONDS = { 8, 9, 10, 10, 11, 11, 12, 12, 13, 15 };

        protected override void OnActivate(Player player)
        {
            int rank = System.Math.Max(1, CurrentRank);
            int duration = DURATION_SECONDS[rank - 1] * 60;
            
            player.AddBuff(BuffID.MagicPower, duration);
            player.AddBuff(BuffID.Clairvoyance, duration);
            player.AddBuff(BuffID.Swiftness, duration);
            
            PlayEffects(player);
            ShowMessage(player, "Spell Snatch!", Color.Cyan);
        }

        private void PlayEffects(Player player)
        {
            SoundEngine.PlaySound(SoundID.Item29, player.position);
            
            for (int i = 0; i < 25; i++)
            {
                Dust dust = Dust.NewDustDirect(player.position, player.width, player.height,
                    DustID.BlueCrystalShard, Main.rand.NextFloat(-2f, 2f), Main.rand.NextFloat(-2f, 0f), 150, Color.Cyan, 1.2f);
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
