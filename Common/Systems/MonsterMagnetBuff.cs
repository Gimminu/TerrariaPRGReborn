using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace RpgMod.Content.Buffs
{
    public class MonsterMagnetBuff : ModBuff
    {
        // 텍스처 파일이 없으므로 바닐라 '전투 포션' 버프 아이콘을 임시로 사용
        public override string Texture => "Terraria/Images/Buff_" + BuffID.Battle;

        public override void SetStaticDefaults()
        {
            Main.buffNoSave[Type] = true;
            Main.buffNoTimeDisplay[Type] = true; // Infinite duration look
        }

        public override void Update(Player player, ref int buffIndex)
        {
            player.buffTime[buffIndex] = 18000; // Keep refreshing (infinite until cancelled)
            
            // Visual Effect: Red particles drawn to player
            if (Main.rand.NextBool(3)) // 33% chance per tick
            {
                // Spawn dust in a circle around player
                Vector2 offset = Main.rand.NextVector2Circular(40f, 40f);
                Dust dust = Dust.NewDustDirect(player.Center + offset, 0, 0, DustID.LifeDrain, 0, 0, 100, default, 1.2f);
                dust.velocity = -offset * 0.1f; // Move towards player
                dust.noGravity = true;
                dust.fadeIn = 1.5f;
            }
        }
    }
}
