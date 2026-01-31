using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using RpgMod.Common.Base;

namespace RpgMod.Common.Skills.Tier1.Ranger
{
    /// <summary>
    /// Evasive Maneuver - 회피 기동.
    /// 빠르게 후방으로 이동하며 잠시 무적.
    /// 레인저의 도주/회피 스킬.
    /// </summary>
    public class EvasiveManeuver : BaseSkill
    {
        public override string InternalName => "EvasiveManeuver";
        public override string DisplayName => "Evasive Maneuver";
        public override string Description => "Quickly dash backwards, becoming briefly invincible.";

        public override SkillType SkillType => SkillType.Active;
        public override JobType RequiredJob => JobType.Ranger;
        public override int RequiredLevel => 13;
        public override int SkillPointCost => 1;
        public override int MaxRank => 10;
        public override string IconTexture => $"{RpgConstants.ICON_PATH}Skills/EvasiveManeuver";
        
        public override float CooldownSeconds => 10f - (CurrentRank * 0.5f); // 10 -> 5초
        public override int ResourceCost => 20 - CurrentRank;
        public override ResourceType ResourceType => ResourceType.Stamina;

        private static readonly int[] INVINCIBILITY_FRAMES = { 10, 12, 14, 16, 18, 20, 22, 24, 26, 30 };

        protected override void OnActivate(Player player)
        {
            int rank = System.Math.Max(1, CurrentRank);
            
            // 후방으로 대시
            Vector2 direction = player.direction == 1 ? -Vector2.UnitX : Vector2.UnitX;
            player.velocity = direction * 18f + new Vector2(0, -5f);
            
            // 무적 시간
            player.immune = true;
            player.immuneTime = INVINCIBILITY_FRAMES[rank - 1];
            
            PlayEffects(player);
        }

        private void PlayEffects(Player player)
        {
            SoundEngine.PlaySound(SoundID.Item24, player.position);
            
            // 잔상 이펙트
            for (int i = 0; i < 15; i++)
            {
                Dust dust = Dust.NewDustDirect(player.position, player.width, player.height,
                    DustID.Smoke, Main.rand.NextFloat(-2f, 2f), Main.rand.NextFloat(-1f, 1f), 150, Color.Gray, 1.0f);
                dust.noGravity = true;
            }
        }
    }
}
