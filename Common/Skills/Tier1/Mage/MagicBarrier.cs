using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Rpg.Common.Base;

namespace Rpg.Common.Skills.Tier1.Mage
{
    /// <summary>
    /// Magic Barrier - 마법 장벽.
    /// 피해를 흡수하는 마나 보호막을 생성.
    /// 메이지의 핵심 방어기.
    /// 만렙 시 쿨타임 = 지속시간 (25초)
    /// </summary>
    public class MagicBarrier : BaseSkill
    {
        public override string InternalName => "MagicBarrier";
        public override string DisplayName => "Magic Barrier";
        public override string Description => "Conjure a barrier of magical energy that absorbs damage.";

        public override SkillType SkillType => SkillType.Buff;
        public override JobType RequiredJob => JobType.Mage;
        public override int RequiredLevel => 10;
        public override int SkillPointCost => 1;
        public override int MaxRank => 10;
        public override string IconTexture => $"{RpgConstants.ICON_PATH}Skills/MagicBarrier";
        
        // 만렙 시 쿨 = 지속 (25초)
        public override float CooldownSeconds => 40f - (CurrentRank * 1.5f); // 40 -> 25초
        public override int ResourceCost => 40;
        public override ResourceType ResourceType => ResourceType.Mana;

        private static readonly int[] DURATION_SECONDS = { 12, 14, 16, 18, 19, 20, 21, 22, 24, 25 };

        protected override void OnActivate(Player player)
        {
            int rank = System.Math.Max(1, CurrentRank);
            int duration = DURATION_SECONDS[rank - 1] * 60;

            // Nebula Shield 같은 방어 효과
            player.AddBuff(BuffID.ManaRegeneration, duration);
            player.AddBuff(BuffID.Ironskin, duration);

            PlayEffects(player);
            ShowMessage(player, "Barrier Up!", Color.Cyan);
        }

        private void PlayEffects(Player player)
        {
            SoundEngine.PlaySound(SoundID.Item28, player.position);
            
            // 마법 구체 이펙트
            for (int i = 0; i < 30; i++)
            {
                float angle = MathHelper.TwoPi * i / 30f;
                Vector2 offset = new Vector2((float)System.Math.Cos(angle), (float)System.Math.Sin(angle)) * 35f;
                Dust dust = Dust.NewDustDirect(player.Center + offset, 4, 4, DustID.BlueTorch,
                    0, 0, 100, Color.Cyan, 1.3f);
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
