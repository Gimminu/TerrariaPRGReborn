using Terraria;
using Terraria.ModLoader;
using Rpg.Content.Buffs;

namespace Rpg.Common.GlobalClasses
{
    public class SpawnRateGlobalNPC : GlobalNPC
    {
        public override void EditSpawnRate(Player player, ref int spawnRate, ref int maxSpawns)
        {
            // Check if player has the Monster Magnet buff
            if (player.HasBuff(ModContent.BuffType<MonsterMagnetBuff>()))
            {
                // Increase spawn rate significantly
                // spawnRate: Lower is faster (frames between spawns)
                // maxSpawns: Higher is more enemies
                
                // 5x spawn speed (0.2 multiplier) - 10x was too laggy/dangerous
                spawnRate = (int)(spawnRate * 0.2f); 
                
                // 3x max enemies - 5x caused sprite limit issues
                maxSpawns = (int)(maxSpawns * 3f);   
                
                // Safety caps to prevent crashes/unplayable lag
                if (spawnRate < 1) spawnRate = 1;
                if (maxSpawns > 100) maxSpawns = 100;
            }
        }
    }
}