using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Rpg.Common.Base;

namespace Rpg.Common.Skills.Tier2.Berserker
{
    /// <summary>
    /// Frenzy - 광란.
    /// 공격 속도를 크게 높임.
    /// 만렙 시 쿨 = 지속
    /// </summary>
    public class Frenzy : BaseSkill
    {
        public override string InternalName => "Frenzy";
        public override string DisplayName => "Frenzy";
        public override string Description => "Enter a frenzy, greatly increasing attack speed.";

        public override SkillType SkillType => SkillType.Buff;
        public override JobType RequiredJob => JobType.Berserker;
        public override int RequiredLevel => 77;
        public override int SkillPointCost => 1;
        public override int MaxRank => 10;
        public override string IconTexture => $"{RpgConstants.ICON_PATH}Skills/Frenzy";
        
        public override float CooldownSeconds => 35f - (CurrentRank * 1.5f); // 35 -> 20초
        public override int ResourceCost => 35;
        public override ResourceType ResourceType => ResourceType.Stamina;

        private static readonly int[] DURATION_SECONDS = { 10, 12, 13, 14, 15, 16, 17, 18, 19, 20 };

        protected override void OnActivate(Player player)
        {
            int rank = System.Math.Max(1, CurrentRank);
            int duration = DURATION_SECONDS[rank - 1] * 60;

            player.AddBuff(BuffID.Tipsy, duration); // 공격력+, 방어-

            PlayEffects(player);
            ShowMessage(player, "FRENZY!", Color.OrangeRed);
        }

        private void PlayEffects(Player player)
        {
            SoundEngine.PlaySound(SoundID.Roar with { Pitch = 0.5f }, player.position);
            
            for (int i = 0; i < 30; i++)
            {
                float angle = MathHelper.TwoPi * i / 30f;
                Vector2 offset = new Vector2((float)System.Math.Cos(angle), (float)System.Math.Sin(angle)) * 30f;
                Dust dust = Dust.NewDustDirect(player.Center + offset, 4, 4, DustID.Torch,
                    offset.X * 0.2f, offset.Y * 0.2f, 100, Color.OrangeRed, 1.2f);
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
