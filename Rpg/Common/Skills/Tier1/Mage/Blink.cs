using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Rpg.Common.Base;

namespace Rpg.Common.Skills.Tier1.Mage
{
    /// <summary>
    /// Blink - 순간이동.
    /// 마우스 방향으로 짧은 거리를 순간이동.
    /// 메이지의 이동기.
    /// </summary>
    public class Blink : BaseSkill
    {
        public override string InternalName => "Blink";
        public override string DisplayName => "Blink";
        public override string Description => "Teleport a short distance in the direction of your cursor.";

        public override SkillType SkillType => SkillType.Active;
        public override JobType RequiredJob => JobType.Mage;
        public override int RequiredLevel => 12;
        public override int SkillPointCost => 1;
        public override int MaxRank => 10;
        public override string IconTexture => $"{RpgConstants.ICON_PATH}Skills/Blink";
        
        public override float CooldownSeconds => 8f - (CurrentRank * 0.4f); // 8 -> 4초
        public override int ResourceCost => 30 - CurrentRank; // 30 -> 20
        public override ResourceType ResourceType => ResourceType.Mana;

        private static readonly float[] BLINK_DISTANCE = { 150f, 180f, 210f, 240f, 270f, 300f, 330f, 360f, 390f, 450f };

        protected override void OnActivate(Player player)
        {
            int rank = System.Math.Max(1, CurrentRank);
            float distance = BLINK_DISTANCE[rank - 1];
            
            // 이전 위치 이펙트
            PlayEffects(player.Center, Color.Blue);
            
            // 마우스 방향으로 이동
            Vector2 direction = (Main.MouseWorld - player.Center).SafeNormalize(Vector2.UnitX);
            Vector2 newPosition = player.Center + direction * distance;
            
            // 벽 체크
            int tileX = (int)(newPosition.X / 16);
            int tileY = (int)(newPosition.Y / 16);
            
            if (!Collision.SolidCollision(newPosition - new Vector2(player.width / 2, player.height / 2), player.width, player.height))
            {
                player.Teleport(newPosition - new Vector2(player.width / 2, player.height / 2), TeleportationStyleID.RodOfDiscord);
            }
            else
            {
                // 가능한 최대 거리까지 이동
                for (float d = distance; d > 50; d -= 20)
                {
                    Vector2 tryPos = player.Center + direction * d;
                    if (!Collision.SolidCollision(tryPos - new Vector2(player.width / 2, player.height / 2), player.width, player.height))
                    {
                        player.Teleport(tryPos - new Vector2(player.width / 2, player.height / 2), TeleportationStyleID.RodOfDiscord);
                        break;
                    }
                }
            }
            
            // 도착 위치 이펙트
            PlayEffects(player.Center, Color.Cyan);
        }

        private void PlayEffects(Vector2 position, Color color)
        {
            SoundEngine.PlaySound(SoundID.Item8, position);
            
            for (int i = 0; i < 25; i++)
            {
                float angle = MathHelper.TwoPi * i / 25f;
                Vector2 offset = new Vector2((float)System.Math.Cos(angle), (float)System.Math.Sin(angle)) * 20f;
                Dust dust = Dust.NewDustDirect(position + offset, 4, 4, DustID.MagicMirror,
                    offset.X * 0.1f, offset.Y * 0.1f, 100, color, 1.2f);
                dust.noGravity = true;
            }
        }
    }
}
