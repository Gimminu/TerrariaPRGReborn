using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using RpgMod.Common.Base;

namespace RpgMod.Common.Skills.Novice
{
    /// <summary>
    /// Toughness - 일시적으로 방어력을 높이는 버프.
    /// 초보자가 위험한 상황에서 버틸 수 있게 해주는 스킬.
    /// 만렙 시 쿨타임 = 지속시간 (15초)
    /// </summary>
    public class Toughness : BaseSkill
    {
        public override string InternalName => "Toughness";
        public override string DisplayName => "Toughness";
        public override string Description => "Brace yourself, temporarily increasing defense.";

        public override SkillType SkillType => SkillType.Buff;
        public override JobType RequiredJob => JobType.Novice;
        public override int RequiredLevel => 3;
        public override int SkillPointCost => 1;
        public override int MaxRank => 5;
        public override string IconTexture => $"{RpgConstants.ICON_PATH}Skills/Toughness";
        
        // 만렙 시 쿨 = 지속 (15초)
        public override float CooldownSeconds => 25f - (CurrentRank * 2f); // 25 -> 15초
        public override int ResourceCost => 12;
        public override ResourceType ResourceType => ResourceType.Stamina;

        private static readonly int[] DURATION_SECONDS = { 8, 10, 12, 13, 15 };

        protected override void OnActivate(Player player)
        {
            int rank = System.Math.Max(1, CurrentRank);
            int duration = DURATION_SECONDS[rank - 1] * 60;

            player.AddBuff(BuffID.Ironskin, duration);

            PlayEffects(player);
            ShowMessage(player, "Toughened!", Color.Gray);
        }

        private void PlayEffects(Player player)
        {
            SoundEngine.PlaySound(SoundID.Item37, player.position);
            for (int i = 0; i < 15; i++)
            {
                Dust dust = Dust.NewDustDirect(player.position, player.width, player.height,
                    DustID.Iron, Main.rand.NextFloat(-2f, 2f), Main.rand.NextFloat(-2f, 0f), 100, Color.Gray, 1.2f);
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
