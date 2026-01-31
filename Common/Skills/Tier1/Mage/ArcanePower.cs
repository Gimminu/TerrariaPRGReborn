using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using RpgMod.Common.Base;

namespace RpgMod.Common.Skills.Tier1.Mage
{
    /// <summary>
    /// Arcane Power - 비전 마력.
    /// 마법 피해를 크게 증가시킨다.
    /// 메이지의 공격 버프.
    /// 만렙 시 쿨타임 = 지속시간 (25초)
    /// </summary>
    public class ArcanePower : BaseSkill
    {
        public override string InternalName => "ArcanePower";
        public override string DisplayName => "Arcane Power";
        public override string Description => "Channel arcane energy to greatly increase magic damage.";

        public override SkillType SkillType => SkillType.Buff;
        public override JobType RequiredJob => JobType.Mage;
        public override int RequiredLevel => 15;
        public override int SkillPointCost => 1;
        public override int MaxRank => 10;
        public override string IconTexture => $"{RpgConstants.ICON_PATH}Skills/ArcanePower";
        
        // 만렙 시 쿨 = 지속 (25초)
        public override float CooldownSeconds => 40f - (CurrentRank * 1.5f); // 40 -> 25초
        public override int ResourceCost => 50;
        public override ResourceType ResourceType => ResourceType.Mana;

        private static readonly int[] DURATION_SECONDS = { 12, 14, 16, 18, 19, 20, 21, 22, 24, 25 };

        protected override void OnActivate(Player player)
        {
            int rank = System.Math.Max(1, CurrentRank);
            int duration = DURATION_SECONDS[rank - 1] * 60;

            player.AddBuff(BuffID.MagicPower, duration);
            player.AddBuff(BuffID.Clairvoyance, duration);

            PlayEffects(player);
            ShowMessage(player, "Arcane Power!", Color.Purple);
        }

        private void PlayEffects(Player player)
        {
            SoundEngine.PlaySound(SoundID.Item29, player.position);
            
            for (int i = 0; i < 25; i++)
            {
                Dust dust = Dust.NewDustDirect(player.position, player.width, player.height,
                    DustID.PurpleTorch, Main.rand.NextFloat(-3f, 3f), Main.rand.NextFloat(-3f, 0f), 100, Color.Purple, 1.3f);
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
