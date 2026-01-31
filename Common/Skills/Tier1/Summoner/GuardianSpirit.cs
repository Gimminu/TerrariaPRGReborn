using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using RpgMod.Common.Base;

namespace RpgMod.Common.Skills.Tier1.Summoner
{
    /// <summary>
    /// Guardian Spirit - 수호 정령.
    /// 자신을 보호하는 정령을 소환하여 방어력 증가.
    /// 소환사의 방어 버프.
    /// 만렙 시 쿨타임 = 지속시간 (30초)
    /// </summary>
    public class GuardianSpirit : BaseSkill
    {
        public override string InternalName => "GuardianSpirit";
        public override string DisplayName => "Guardian Spirit";
        public override string Description => "Summon a protective spirit that increases defense and reduces damage taken.";

        public override SkillType SkillType => SkillType.Buff;
        public override JobType RequiredJob => JobType.Summoner;
        public override int RequiredLevel => 14;
        public override int SkillPointCost => 1;
        public override int MaxRank => 10;
        public override string IconTexture => $"{RpgConstants.ICON_PATH}Skills/GuardianSpirit";
        
        // 만렙 시 쿨 = 지속 (30초)
        public override float CooldownSeconds => 45f - (CurrentRank * 1.5f); // 45 -> 30초
        public override int ResourceCost => 40;
        public override ResourceType ResourceType => ResourceType.Mana;

        private static readonly int[] DURATION_SECONDS = { 15, 18, 20, 22, 24, 26, 27, 28, 29, 30 };

        protected override void OnActivate(Player player)
        {
            int rank = System.Math.Max(1, CurrentRank);
            int duration = DURATION_SECONDS[rank - 1] * 60;

            player.AddBuff(BuffID.Ironskin, duration);
            player.AddBuff(BuffID.Endurance, duration);

            PlayEffects(player);
            ShowMessage(player, "Spirit Guard!", Color.LightBlue);
        }

        private void PlayEffects(Player player)
        {
            SoundEngine.PlaySound(SoundID.Item44, player.position);
            
            // 정령 소환 이펙트
            for (int i = 0; i < 20; i++)
            {
                float angle = MathHelper.TwoPi * i / 20f;
                Vector2 offset = new Vector2((float)System.Math.Cos(angle), (float)System.Math.Sin(angle)) * 40f;
                Dust dust = Dust.NewDustDirect(player.Center + offset, 4, 4, DustID.SpectreStaff,
                    0, 0, 100, Color.LightBlue, 1.0f);
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
