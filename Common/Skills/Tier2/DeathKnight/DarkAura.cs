using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using RpgMod.Common.Base;

namespace RpgMod.Common.Skills.Tier2.DeathKnight
{
    /// <summary>
    /// Dark Aura - 어둠의 오라.
    /// 주변 적에게 지속 피해.
    /// </summary>
    public class DarkAura : BaseSkill
    {
        public override string InternalName => "DarkAura";
        public override string DisplayName => "Dark Aura";
        public override string Description => "Emanate a dark aura that damages nearby enemies over time.";

        public override SkillType SkillType => SkillType.Buff;
        public override JobType RequiredJob => JobType.DeathKnight;
        public override int RequiredLevel => 65;
        public override int SkillPointCost => 1;
        public override int MaxRank => 10;
        public override string IconTexture => $"{RpgConstants.ICON_PATH}Skills/DarkAura";
        
        public override float CooldownSeconds => 35f - (CurrentRank * 1.5f); // 35 -> 20초
        public override int ResourceCost => 40;
        public override ResourceType ResourceType => ResourceType.Mana;

        private static readonly int[] DURATION_SECONDS = { 12, 13, 14, 15, 16, 17, 18, 19, 19, 20 };

        protected override void OnActivate(Player player)
        {
            int rank = System.Math.Max(1, CurrentRank);
            int duration = DURATION_SECONDS[rank - 1] * 60;

            // 어둠 오라 버프 (Thorns 효과로 대체)
            player.AddBuff(BuffID.Thorns, duration);
            player.AddBuff(BuffID.Inferno, duration);

            PlayEffects(player);
        }

        private void PlayEffects(Player player)
        {
            SoundEngine.PlaySound(SoundID.NPCDeath52, player.position);
            
            for (int i = 0; i < 40; i++)
            {
                float angle = MathHelper.TwoPi * i / 40f;
                Vector2 offset = new Vector2((float)System.Math.Cos(angle), (float)System.Math.Sin(angle)) * 100f;
                Dust dust = Dust.NewDustDirect(player.Center + offset, 4, 4, DustID.Shadowflame,
                    0, 0, 100, Color.Purple, 1.0f);
                dust.noGravity = true;
            }
        }
    }
}
