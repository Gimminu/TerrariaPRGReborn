using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Rpg.Common;

namespace Rpg.Common.Systems
{
    /// <summary>
    /// Comprehensive biome-based level system
    /// 
    /// Design principles from prompt:
    /// 1. Each biome has base level, level cap per progression stage
    /// 2. Pre-hardmode monsters cap when hardmode starts
    /// 3. World level affects monsters up to biome's current cap
    /// 4. Monster level = BaseLevel + WorldLevelBonus (capped by biome)
    /// 5. XP penalty when player level is too low for the area (anti-farming)
    /// </summary>
    public static class BiomeLevelSystem
    {
        #region Biome Data Structures
        
        /// <summary>
        /// Complete biome level configuration
        /// </summary>
        public readonly struct BiomeLevelData
        {
            public readonly int BaseLevel;           // Starting level for this biome
            public readonly int PreHardmodeCap;      // Max level during pre-hardmode
            public readonly int EarlyHardmodeCap;    // Max after WoF, before mech bosses
            public readonly int MidHardmodeCap;      // Max after mech bosses
            public readonly int LateHardmodeCap;     // Max after Plantera/Golem
            public readonly int PostMoonLordCap;     // Max after Moon Lord (can grow further)
            public readonly float GrowthRate;        // How fast world level affects this biome
            public readonly bool IsHardmodeOnly;     // Only spawns in hardmode
            
            public BiomeLevelData(
                int baseLevel, 
                int preHardCap, 
                int earlyHardCap, 
                int midHardCap, 
                int lateHardCap, 
                int postMoonCap, 
                float growthRate,
                bool hardmodeOnly = false)
            {
                BaseLevel = baseLevel;
                PreHardmodeCap = preHardCap;
                EarlyHardmodeCap = earlyHardCap;
                MidHardmodeCap = midHardCap;
                LateHardmodeCap = lateHardCap;
                PostMoonLordCap = postMoonCap;
                GrowthRate = growthRate;
                IsHardmodeOnly = hardmodeOnly;
            }
            
            /// <summary>
            /// Get the current level cap based on game progression
            /// </summary>
            public int GetCurrentCap()
            {
                if (NPC.downedMoonlord)
                    return PostMoonLordCap;
                if (NPC.downedGolemBoss || NPC.downedPlantBoss)
                    return LateHardmodeCap;
                if (NPC.downedMechBossAny)
                    return MidHardmodeCap;
                if (Main.hardMode)
                    return EarlyHardmodeCap;
                return PreHardmodeCap;
            }
        }
        
        #endregion
        
        #region Biome Level Database
        
        /// <summary>
        /// Complete biome level data
        /// Format: (Base, PreHard, EarlyHard, MidHard, LateHard, PostMoon, GrowthRate, HardmodeOnly)
        /// </summary>
        private static readonly Dictionary<BiomeType, BiomeLevelData> BiomeData = new()
        {
            // ========== SURFACE BIOMES (Easy Start) ==========
            // Forest - safest starting area
            [BiomeType.Forest] = new(1, 15, 25, 35, 45, 60, 0.4f),
            
            // Desert - slightly harder
            [BiomeType.Desert] = new(3, 18, 28, 40, 50, 65, 0.5f),
            
            // Snow - cold environment
            [BiomeType.Snow] = new(4, 20, 30, 42, 52, 68, 0.5f),
            
            // Ocean - mid-difficulty surface
            [BiomeType.Ocean] = new(6, 22, 35, 48, 58, 72, 0.6f),
            
            // Space - floating islands
            [BiomeType.Space] = new(10, 28, 40, 55, 68, 85, 0.7f),
            
            // Graveyards - spooky
            [BiomeType.Graveyard] = new(8, 25, 38, 52, 65, 80, 0.6f),

            // Meteor crash sites
            [BiomeType.Meteor] = new(10, 28, 42, 56, 70, 88, 0.7f),
            
            // ========== SURFACE SPECIAL BIOMES ==========
            // Surface Jungle - dangerous early game
            [BiomeType.SurfaceJungle] = new(12, 32, 45, 60, 75, 95, 0.8f),
            
            // Surface Evil (Corruption/Crimson)
            [BiomeType.SurfaceCorruption] = new(10, 28, 42, 56, 70, 88, 0.7f),
            [BiomeType.SurfaceCrimson] = new(10, 28, 42, 56, 70, 88, 0.7f),
            [BiomeType.SurfaceEvil] = new(10, 28, 42, 56, 70, 88, 0.7f),
            
            // Surface Hallow (Hardmode only)
            [BiomeType.SurfaceHallow] = new(35, 35, 50, 65, 80, 100, 0.9f, true),
            
            // ========== UNDERGROUND BIOMES ==========
            // Underground (just below surface)
            [BiomeType.Underground] = new(6, 22, 35, 48, 60, 75, 0.6f),
            
            // Caverns (deeper)
            [BiomeType.Caverns] = new(10, 28, 42, 58, 72, 90, 0.7f),
            
            // Ice Caves
            [BiomeType.IceCaves] = new(12, 30, 45, 60, 75, 92, 0.75f),
            
            // Underground Desert
            [BiomeType.UndergroundDesert] = new(14, 32, 48, 65, 80, 98, 0.8f),
            
            // Marble Caves
            [BiomeType.Marble] = new(15, 35, 50, 68, 82, 100, 0.8f),
            
            // Granite Caves
            [BiomeType.Granite] = new(15, 35, 50, 68, 82, 100, 0.8f),
            
            // Spider Caves
            [BiomeType.SpiderNest] = new(16, 36, 52, 70, 85, 102, 0.85f),
            
            // Glowing Mushroom Biome
            [BiomeType.MushroomBiome] = new(18, 38, 55, 72, 88, 105, 0.85f),
            
            // Bee Hive
            [BiomeType.BeeHive] = new(15, 35, 50, 65, 80, 95, 0.75f),
            
            // ========== UNDERGROUND SPECIAL ==========
            // Underground Jungle - very dangerous
            [BiomeType.UndergroundJungle] = new(20, 42, 58, 78, 95, 115, 0.95f),
            
            // Underground Evil
            [BiomeType.UndergroundCorruption] = new(18, 38, 55, 72, 88, 108, 0.9f),
            [BiomeType.UndergroundCrimson] = new(18, 38, 55, 72, 88, 108, 0.9f),
            [BiomeType.UndergroundEvil] = new(18, 38, 55, 72, 88, 108, 0.9f),
            
            // Underground Hallow (Hardmode only)
            [BiomeType.UndergroundHallow] = new(40, 40, 58, 75, 92, 112, 1.0f, true),
            
            // ========== DANGEROUS BIOMES ==========
            // Dungeon - Pre-Skeletron is death
            [BiomeType.Dungeon] = new(30, 45, 62, 80, 98, 120, 1.1f),
            
            // Post-Plantera Dungeon (harder)
            [BiomeType.DungeonPostPlantera] = new(70, 70, 70, 85, 105, 130, 1.2f, true),
            
            // Hell/Underworld
            [BiomeType.Hell] = new(25, 42, 58, 75, 92, 115, 1.0f),
            
            // Lihzahrd Temple (Post-Golem area)
            [BiomeType.Temple] = new(55, 55, 55, 75, 100, 135, 1.3f),
            
            // ========== EVENT/INVASION BIOMES ==========
            // Blood Moon
            [BiomeType.BloodMoon] = new(8, 25, 40, 55, 70, 90, 0.7f),
            
            // Goblin Army
            [BiomeType.GoblinInvasion] = new(12, 30, 45, 60, 75, 95, 0.8f),
            
            // Pirate Invasion (Hardmode)
            [BiomeType.PirateInvasion] = new(45, 45, 55, 70, 85, 105, 0.9f, true),
            
            // Solar Eclipse (Hardmode)
            [BiomeType.SolarEclipse] = new(50, 50, 60, 78, 95, 118, 1.0f, true),
            
            // Pumpkin Moon
            [BiomeType.PumpkinMoon] = new(65, 65, 65, 80, 100, 125, 1.1f, true),
            
            // Frost Moon
            [BiomeType.FrostMoon] = new(70, 70, 70, 85, 105, 130, 1.15f, true),
            
            // Martian Madness
            [BiomeType.MartianInvasion] = new(72, 72, 72, 88, 108, 135, 1.2f, true),
            
            // Lunar Events (Post-Cultist)
            [BiomeType.LunarEvent] = new(90, 90, 90, 95, 115, 145, 1.3f, true),
            
            // Old One's Army
            [BiomeType.OldOnesArmy] = new(20, 35, 50, 70, 90, 115, 0.9f),
        };
        
        // Default fallback
        private static readonly BiomeLevelData DefaultBiomeData = new(5, 20, 35, 50, 65, 80, 0.6f);

        private static readonly Dictionary<int, BiomeLevelData> ModBiomeData = new();
        private static readonly BiomeLevelData ModBiomeLowData = DefaultBiomeData;
        private static readonly BiomeLevelData ModBiomeMediumData = new(12, 32, 45, 60, 75, 95, 0.8f);
        private static readonly BiomeLevelData ModBiomeHighData = new(20, 42, 58, 78, 95, 115, 0.95f);
        private static readonly BiomeLevelData ModBiomeEnvironmentData = new(8, 25, 38, 52, 65, 80, 0.6f);
        private static readonly BiomeLevelData ModBiomeEventData = new(20, 35, 50, 70, 90, 115, 0.9f);
        private static readonly BiomeLevelData ModBiomeBossLowData = new(30, 45, 62, 80, 98, 120, 1.1f);
        private static readonly BiomeLevelData ModBiomeBossMediumData = new(55, 55, 55, 75, 100, 135, 1.3f);
        private static readonly BiomeLevelData ModBiomeBossHighData = new(90, 90, 90, 95, 115, 145, 1.3f);
        
        #endregion
        
        #region Biome Detection
        
        /// <summary>
        /// Get the biome type at NPC position with full detection
        /// </summary>
        public static BiomeType GetBiomeAt(NPC npc)
        {
            return GetBiomeAt((int)(npc.Center.X / 16), (int)(npc.Center.Y / 16));
        }
        
        /// <summary>
        /// Get the biome type at tile position
        /// </summary>
        public static BiomeType GetBiomeAt(int tileX, int tileY)
        {
            // Check for active events first (take priority)
            BiomeType eventBiome = GetActiveEventBiome();
            if (eventBiome != BiomeType.None)
                return eventBiome;
            
            // Depth calculations
            float worldSurface = (float)Main.worldSurface;
            float rockLayer = (float)Main.rockLayer;
            float hellLayer = Main.maxTilesY - 200;
            
            bool isSurface = tileY <= worldSurface;
            bool isUnderground = tileY > worldSurface && tileY <= rockLayer;
            bool isCavern = tileY > rockLayer && tileY <= hellLayer;
            bool isHell = tileY > hellLayer;
            
            // Hell always takes priority when deep enough
            if (isHell)
                return BiomeType.Hell;
            
            // Find nearest player for zone detection
            Player nearestPlayer = Main.player[Player.FindClosest(
                new Vector2(tileX * 16, tileY * 16), 16, 16)];
            
            if (nearestPlayer == null || !nearestPlayer.active)
                return GetDefaultBiomeByDepth(isSurface, isUnderground, isCavern);
            
            // Special structure biomes (highest priority after hell)
            if (nearestPlayer.ZoneLihzhardTemple)
                return BiomeType.Temple;
            
            if (nearestPlayer.ZoneDungeon)
                return NPC.downedPlantBoss ? BiomeType.DungeonPostPlantera : BiomeType.Dungeon;
            
            // Check special underground structures
            if (!isSurface)
            {
                BiomeType structureBiome = GetUndergroundStructureBiome(tileX, tileY);
                if (structureBiome != BiomeType.None)
                    return structureBiome;
            }

            // Meteor biome
            if (nearestPlayer.ZoneMeteor)
                return BiomeType.Meteor;
            
            // Mushroom biome
            if (nearestPlayer.ZoneGlowshroom)
                return BiomeType.MushroomBiome;
            
            // Evil biomes (Corruption/Crimson)
            if (nearestPlayer.ZoneCorrupt)
                return isSurface ? BiomeType.SurfaceCorruption : BiomeType.UndergroundCorruption;
            if (nearestPlayer.ZoneCrimson)
                return isSurface ? BiomeType.SurfaceCrimson : BiomeType.UndergroundCrimson;
            
            // Hallow (hardmode only)
            if (nearestPlayer.ZoneHallow)
                return isSurface ? BiomeType.SurfaceHallow : BiomeType.UndergroundHallow;
            
            // Jungle
            if (nearestPlayer.ZoneJungle)
                return isSurface ? BiomeType.SurfaceJungle : BiomeType.UndergroundJungle;
            
            // Desert
            if (nearestPlayer.ZoneDesert || nearestPlayer.ZoneUndergroundDesert)
                return nearestPlayer.ZoneUndergroundDesert || !isSurface ? BiomeType.UndergroundDesert : BiomeType.Desert;
            
            // Snow/Ice
            if (nearestPlayer.ZoneSnow)
                return isSurface ? BiomeType.Snow : BiomeType.IceCaves;
            
            // Graveyard
            if (nearestPlayer.ZoneGraveyard)
                return BiomeType.Graveyard;
            
            // Ocean
            if (nearestPlayer.ZoneBeach)
                return BiomeType.Ocean;
            
            // Space
            if (nearestPlayer.ZoneSkyHeight)
                return BiomeType.Space;
            
            // Default by depth
            return GetDefaultBiomeByDepth(isSurface, isUnderground, isCavern);
        }
        
        private static BiomeType GetDefaultBiomeByDepth(bool surface, bool underground, bool cavern)
        {
            if (surface) return BiomeType.Forest;
            if (underground) return BiomeType.Underground;
            if (cavern) return BiomeType.Caverns;
            return BiomeType.Forest;
        }
        
        private static BiomeType GetActiveEventBiome()
        {
            // Lunar events
            if (NPC.LunarApocalypseIsUp)
                return BiomeType.LunarEvent;
            
            // Invasions
            if (Main.invasionType == InvasionID.MartianMadness)
                return BiomeType.MartianInvasion;
            if (Main.invasionType == InvasionID.PirateInvasion)
                return BiomeType.PirateInvasion;
            if (Main.invasionType == InvasionID.GoblinArmy)
                return BiomeType.GoblinInvasion;
            
            // Moon events
            if (Main.pumpkinMoon)
                return BiomeType.PumpkinMoon;
            if (Main.snowMoon)
                return BiomeType.FrostMoon;
            
            // Other events
            if (Main.eclipse)
                return BiomeType.SolarEclipse;
            if (Main.bloodMoon)
                return BiomeType.BloodMoon;
            
            // Old One's Army (DD2)
            if (Terraria.GameContent.Events.DD2Event.Ongoing)
                return BiomeType.OldOnesArmy;
            
            return BiomeType.None;
        }

        private struct ModBiomeCacheEntry
        {
            public int Frame;
            public ModBiome Biome;
        }

        private static readonly Dictionary<int, ModBiomeCacheEntry> ModBiomeCache = new();

        public static ModBiome GetActiveModBiomeAt(int tileX, int tileY)
        {
            Player nearestPlayer = Main.player[Player.FindClosest(
                new Vector2(tileX * 16, tileY * 16), 16, 16)];

            if (nearestPlayer == null || !nearestPlayer.active)
                return null;

            return GetActiveModBiome(nearestPlayer);
        }

        public static ModBiome GetActiveModBiomeAt(NPC npc)
        {
            if (npc == null)
                return null;

            return GetActiveModBiomeAt((int)(npc.Center.X / 16), (int)(npc.Center.Y / 16));
        }

        private static ModBiome GetActiveModBiome(Player player)
        {
            if (player == null || !player.active)
                return null;

            int frame = (int)Main.GameUpdateCount;
            if (ModBiomeCache.TryGetValue(player.whoAmI, out ModBiomeCacheEntry cache) && cache.Frame == frame)
                return cache.Biome;

            ModBiome bestBiome = null;
            float bestWeight = float.MinValue;

            foreach (ModBiome modBiome in ModContent.GetContent<ModBiome>())
            {
                if (!player.InModBiome(modBiome))
                    continue;

                float weight = GetModBiomePriorityWeight(modBiome.Priority);
                if (weight > bestWeight)
                {
                    bestWeight = weight;
                    bestBiome = modBiome;
                }
            }

            ModBiomeCache[player.whoAmI] = new ModBiomeCacheEntry { Frame = frame, Biome = bestBiome };
            return bestBiome;
        }

        private static float GetModBiomePriorityWeight(SceneEffectPriority priority)
        {
            return priority switch
            {
                SceneEffectPriority.BossHigh => 8f,
                SceneEffectPriority.BossMedium => 7f,
                SceneEffectPriority.BossLow => 6f,
                SceneEffectPriority.Event => 5f,
                SceneEffectPriority.Environment => 4f,
                SceneEffectPriority.BiomeHigh => 3f,
                SceneEffectPriority.BiomeMedium => 2f,
                SceneEffectPriority.BiomeLow => 1f,
                _ => 0f
            };
        }
        
        private const int StructureScanRadius = 30;
        private const int SpiderScoreThreshold = 28;
        private const int HiveScoreThreshold = 28;
        private const int MarbleScoreThreshold = 22;
        private const int GraniteScoreThreshold = 22;

        private static BiomeType GetUndergroundStructureBiome(int tileX, int tileY)
        {
            int startX = Math.Max(0, tileX - StructureScanRadius);
            int endX = Math.Min(Main.maxTilesX - 1, tileX + StructureScanRadius);
            int startY = Math.Max(0, tileY - StructureScanRadius);
            int endY = Math.Min(Main.maxTilesY - 1, tileY + StructureScanRadius);

            int spiderScore = 0;
            int hiveScore = 0;
            int marbleScore = 0;
            int graniteScore = 0;

            for (int x = startX; x <= endX; x++)
            {
                for (int y = startY; y <= endY; y++)
                {
                    Tile tile = Main.tile[x, y];
                    if (tile == null)
                        continue;

                    if (tile.HasTile)
                    {
                        ushort tileType = tile.TileType;
                        if (tileType == TileID.Cobweb)
                            spiderScore++;
                        if (tileType == TileID.HoneyBlock)
                            hiveScore++;
                        if (tileType == TileID.MarbleBlock)
                            marbleScore++;
                        if (tileType == TileID.GraniteBlock)
                            graniteScore++;
                    }

                    ushort wallType = tile.WallType;
                    if (wallType == 0)
                        continue;

                    if (wallType == WallID.SpiderUnsafe)
                        spiderScore += 2;
                    if (wallType == WallID.HiveUnsafe)
                        hiveScore += 2;
                    if (wallType == WallID.MarbleUnsafe)
                        marbleScore += 2;
                    if (wallType == WallID.GraniteUnsafe)
                        graniteScore += 2;
                }
            }

            if (spiderScore >= SpiderScoreThreshold)
                return BiomeType.SpiderNest;
            if (hiveScore >= HiveScoreThreshold)
                return BiomeType.BeeHive;
            if (marbleScore >= MarbleScoreThreshold)
                return BiomeType.Marble;
            if (graniteScore >= GraniteScoreThreshold)
                return BiomeType.Granite;

            return BiomeType.None;
        }
        
        #endregion
        
        #region Level Calculations
        
        /// <summary>
        /// Get biome data, with fallback
        /// </summary>
        public static BiomeLevelData GetBiomeData(BiomeType biome)
        {
            return BiomeData.TryGetValue(biome, out var data) ? data : DefaultBiomeData;
        }

        public static void RegisterModBiomeData(ModBiome biome, BiomeLevelData data)
        {
            if (biome == null)
                return;

            ModBiomeData[biome.Type] = data;
        }

        public static BiomeLevelData GetModBiomeData(ModBiome biome)
        {
            if (biome == null)
                return DefaultBiomeData;

            if (ModBiomeData.TryGetValue(biome.Type, out var data))
                return data;

            data = GetDefaultModBiomeData(biome);
            ModBiomeData[biome.Type] = data;
            return data;
        }

        private static BiomeLevelData GetDefaultModBiomeData(ModBiome biome)
        {
            if (biome == null)
                return DefaultBiomeData;

            return biome.Priority switch
            {
                SceneEffectPriority.BiomeLow => ModBiomeLowData,
                SceneEffectPriority.BiomeMedium => ModBiomeMediumData,
                SceneEffectPriority.BiomeHigh => ModBiomeHighData,
                SceneEffectPriority.Environment => ModBiomeEnvironmentData,
                SceneEffectPriority.Event => ModBiomeEventData,
                SceneEffectPriority.BossLow => ModBiomeBossLowData,
                SceneEffectPriority.BossMedium => ModBiomeBossMediumData,
                SceneEffectPriority.BossHigh => ModBiomeBossHighData,
                _ => ModBiomeLowData
            };
        }
        
        /// <summary>
        /// Calculate monster level with all factors
        /// </summary>
        public static int CalculateMonsterLevel(NPC npc)
        {
            BiomeType eventBiome = GetActiveEventBiome();
            if (eventBiome != BiomeType.None)
            {
                BiomeLevelData eventData = GetBiomeData(eventBiome);
                int eventLevel = CalculateMonsterLevelForData(eventData);
                int eventMinLevel = GetMonsterMinimumLevel(npc, eventData.BaseLevel);
                return Math.Max(eventMinLevel, eventLevel);
            }

            ModBiome modBiome = GetActiveModBiomeAt(npc);
            if (modBiome != null)
            {
                BiomeLevelData modData = GetModBiomeData(modBiome);
                int modLevel = CalculateMonsterLevelForData(modData);
                int modMinLevel = GetMonsterMinimumLevel(npc, modData.BaseLevel);
                return Math.Max(modMinLevel, modLevel);
            }

            BiomeType biome = GetBiomeAt(npc);
            BiomeLevelData data = GetBiomeData(biome);
            int level = CalculateMonsterLevelForData(data);
            int minLevel = GetMonsterMinimumLevel(npc, data.BaseLevel);
            return Math.Max(minLevel, level);
        }
        
        /// <summary>
        /// Calculate level for a specific biome
        /// </summary>
        public static int CalculateMonsterLevelForBiome(BiomeType biome)
        {
            return CalculateMonsterLevelForData(GetBiomeData(biome));
        }

        public static int CalculateMonsterLevelForModBiome(ModBiome biome)
        {
            return CalculateMonsterLevelForData(GetModBiomeData(biome));
        }

        private static int CalculateMonsterLevelForData(BiomeLevelData data)
        {
            // Hardmode-only biomes don't exist in pre-hardmode
            if (data.IsHardmodeOnly && !Main.hardMode)
                return 1;
            
            // Base level for this biome (바이옴 기본 레벨)
            int baseLevel = data.BaseLevel;
            
            // Apply random variation to base level (바이옴 레벨 ±2 랜덤)
            int randomOffset = Main.rand.Next(-RpgConstants.MONSTER_LEVEL_RANDOM_RANGE, 
                                              RpgConstants.MONSTER_LEVEL_RANDOM_RANGE + 1);
            int randomizedBase = Math.Max(1, baseLevel + randomOffset);
            
            // World level contribution (scaled by biome's growth rate)
            int worldLevel = RpgWorld.GetWorldLevel();
            int worldLevelBonus = (int)((worldLevel - 1) * data.GrowthRate);
            
            // Total before cap
            int totalLevel = randomizedBase + worldLevelBonus;
            
            // Apply current progression cap (레벨 캡 제한)
            int currentCap = data.GetCurrentCap();
            totalLevel = Math.Min(totalLevel, currentCap);
            
            // Ensure minimum of base level (최소 레벨 보장)
            return Math.Max(Math.Max(1, baseLevel - RpgConstants.MONSTER_LEVEL_RANDOM_RANGE), totalLevel);
        }

        private static int GetMonsterMinimumLevel(NPC npc, int baseLevel)
        {
            if (Main.hardMode)
            {
                if (IsHardmodeMonster(npc))
                    return Math.Max(baseLevel, RpgConstants.HARDMODE_MONSTER_MIN_LEVEL);

                return Math.Max(baseLevel, RpgConstants.PREHARDMODE_MONSTER_MIN_LEVEL_HARDMODE);
            }

            return Math.Max(baseLevel, RpgConstants.PREHARDMODE_MONSTER_MIN_LEVEL);
        }

        private static bool IsHardmodeMonster(NPC npc)
        {
            return npc.type >= NPCID.Pixie && npc.type <= NPCID.DungeonSpirit ||
                   npc.type >= NPCID.Wraith && npc.type <= NPCID.Clown ||
                   npc.type >= NPCID.Mummy && npc.type <= NPCID.DarkMummy ||
                   npc.type >= NPCID.Corruptor && npc.type <= NPCID.CorruptSlime ||
                   npc.type >= NPCID.Gastropod && npc.type <= NPCID.IlluminantSlime ||
                   npc.type >= NPCID.GiantBat && npc.type <= NPCID.IceGolem ||
                   npc.type == NPCID.TheDestroyer ||
                   npc.type == NPCID.Retinazer ||
                   npc.type == NPCID.Spazmatism ||
                   npc.type == NPCID.SkeletronPrime ||
                   npc.type == NPCID.Plantera ||
                   npc.type == NPCID.Golem ||
                   npc.type == NPCID.DukeFishron ||
                   npc.type == NPCID.CultistBoss ||
                   npc.type == NPCID.MoonLordCore ||
                   (npc.HitSound == SoundID.NPCHit1 && npc.lifeMax > 100 && Main.hardMode);
        }
        
        /// <summary>
        /// Get the level range for a biome (min - max for current progression)
        /// </summary>
        public static (int min, int max) GetBiomeLevelRange(BiomeType biome)
        {
            BiomeLevelData data = GetBiomeData(biome);
            int min = data.BaseLevel;
            int max = data.GetCurrentCap();
            return (min, max);
        }
        
        /// <summary>
        /// Get base level for backward compatibility
        /// </summary>
        public static int GetBiomeBaseLevel(BiomeType biome)
        {
            return GetBiomeData(biome).BaseLevel;
        }
        
        /// <summary>
        /// Get current cap for backward compatibility
        /// </summary>
        public static int GetBiomeLevelCap(BiomeType biome)
        {
            return GetBiomeData(biome).GetCurrentCap();
        }
        
        /// <summary>
        /// Get growth rate for backward compatibility
        /// </summary>
        public static float GetBiomeGrowthRate(BiomeType biome)
        {
            return GetBiomeData(biome).GrowthRate;
        }
        
        #endregion
        
        #region XP Calculations
        
        /// <summary>
        /// Calculate XP multiplier based on player level vs monster level
        /// 레벨 차이에 따른 경험치 배율 계산 (폭업 방지 + 스위트 스팟 시스템)
        /// </summary>
        public static float GetLevelDifferenceXPMultiplier(int playerLevel, int monsterLevel)
        {
            int levelDiff = monsterLevel - playerLevel;
            
            // === 높은 레벨 몬스터 (High Level Monsters) ===
            // 너무 높으면 위험하지만 경험치도 적음 (폭업 방지)
            if (levelDiff >= 10)
                return RpgConstants.LEVEL_DIFF_PLUS_10_MULT;  // 10% XP - 너무 높은 몬스터
            if (levelDiff >= 7)
                return RpgConstants.LEVEL_DIFF_PLUS_7_MULT;   // 30% XP - 매우 높은 몬스터
            if (levelDiff >= 5)
                return RpgConstants.LEVEL_DIFF_PLUS_5_MULT;   // 60% XP - 높은 몬스터
            
            // === 스위트 스팟 (Sweet Spot) ===
            // +1~+3 레벨: 최적 난이도, 보너스 경험치 (110~120%)
            if (levelDiff >= RpgConstants.LEVEL_DIFF_SWEETSPOT_START && 
                levelDiff <= RpgConstants.LEVEL_DIFF_SWEETSPOT_END)
            {
                // 레벨 차이에 비례한 보너스 (1.10 → 1.15 → 1.20)
                float bonusStep = (RpgConstants.LEVEL_DIFF_SWEETSPOT_BONUS - 1.0f) / 
                                  (RpgConstants.LEVEL_DIFF_SWEETSPOT_END - RpgConstants.LEVEL_DIFF_SWEETSPOT_START);
                return 1.10f + (levelDiff - RpgConstants.LEVEL_DIFF_SWEETSPOT_START) * bonusStep;
            }
            
            // === 동등/약간 낮은 레벨 (Equal/Slightly Lower) ===
            // -2~+1: 표준 경험치 (100~110%)
            if (levelDiff >= -2)
                return 1.0f + (levelDiff * 0.025f); // -2레벨: 95%, 0레벨: 100%, +1레벨: 102.5%
            
            // === 낮은 레벨 몬스터 (Low Level Monsters) ===
            // 레벨이 많이 낮으면 경험치 감소 (하위 사냥 방지)
            if (levelDiff >= -5)
            {
                // -3 ~ -5: 선형 감소 (90% → 70%)
                float ratio = (levelDiff + 5) / 3f; // -5일 때 0, -3일 때 0.667, -2일 때 1
                return RpgConstants.LEVEL_DIFF_MINUS_5_MULT + 
                       (RpgConstants.LEVEL_DIFF_MINUS_2_MULT - RpgConstants.LEVEL_DIFF_MINUS_5_MULT) * ratio;
            }
            
            if (levelDiff >= -8)
            {
                // -6 ~ -8: 선형 감소 (70% → 40%)
                float ratio = (levelDiff + 8) / 3f;
                return RpgConstants.LEVEL_DIFF_MINUS_8_MULT + 
                       (RpgConstants.LEVEL_DIFF_MINUS_5_MULT - RpgConstants.LEVEL_DIFF_MINUS_8_MULT) * ratio;
            }
            
            // -9 이하: 극단적으로 낮음, 최소 경험치 (10%)
            return RpgConstants.LEVEL_DIFF_MINUS_10_MULT;
        }
        
        /// <summary>
        /// Check if player can receive XP from a monster (anti-exploit)
        /// </summary>
        public static bool CanReceiveXPFromMonster(int playerLevel, int monsterLevel)
        {
            // Always allow XP if monster is within reasonable range
            int levelDiff = monsterLevel - playerLevel;
            
            // Can't gain XP from monsters more than 20 levels higher
            // (anti-boosting: getting carried through content)
            if (levelDiff > 20)
                return false;
            
            // Can gain (reduced) XP from lower level monsters
            return true;
        }
        
        #endregion
        
        #region Display

        public static bool IsEventBiome(BiomeType biome)
        {
            return biome == BiomeType.BloodMoon ||
                   biome == BiomeType.GoblinInvasion ||
                   biome == BiomeType.PirateInvasion ||
                   biome == BiomeType.SolarEclipse ||
                   biome == BiomeType.PumpkinMoon ||
                   biome == BiomeType.FrostMoon ||
                   biome == BiomeType.MartianInvasion ||
                   biome == BiomeType.LunarEvent ||
                   biome == BiomeType.OldOnesArmy;
        }
        
        /// <summary>
        /// Get color for monster level display
        /// </summary>
        public static Color GetLevelColor(int monsterLevel, int playerLevel)
        {
            int diff = monsterLevel - playerLevel;
            
            if (diff >= 15)
                return new Color(200, 0, 0);      // Dark red - extreme danger
            if (diff >= 10)
                return Color.Red;                  // Red - very dangerous
            if (diff >= 5)
                return Color.Orange;               // Orange - challenging
            if (diff >= 2)
                return Color.Yellow;               // Yellow - fair fight
            if (diff >= -2)
                return Color.White;                // White - equal
            if (diff >= -5)
                return Color.LightGreen;           // Light green - easy
            if (diff >= -10)
                return Color.Green;                // Green - very easy
            
            return Color.Gray;                     // Gray - trivial
        }
        
        /// <summary>
        /// Get text description of level difficulty
        /// </summary>
        public static string GetLevelDifficultyText(int monsterLevel, int playerLevel)
        {
            int diff = monsterLevel - playerLevel;
            
            if (diff >= 15) return "!!DEADLY!!";
            if (diff >= 10) return "Extreme";
            if (diff >= 5) return "Hard";
            if (diff >= 2) return "Challenging";
            if (diff >= -2) return "Normal";
            if (diff >= -5) return "Easy";
            if (diff >= -10) return "Very Easy";
            return "Trivial";
        }
        
        #endregion
    }
    
    /// <summary>
    /// All biome types for level calculation
    /// </summary>
    public enum BiomeType
    {
        None = 0,
        
        // Surface
        Forest,
        Desert,
        Snow,
        Ocean,
        Space,
        Graveyard,
        Meteor,
        
        // Surface Special
        SurfaceJungle,
        SurfaceCorruption,
        SurfaceCrimson,
        SurfaceEvil,
        SurfaceHallow,
        
        // Underground
        Underground,
        Caverns,
        IceCaves,
        UndergroundDesert,
        Marble,
        Granite,
        SpiderNest,
        MushroomBiome,
        BeeHive,
        
        // Underground Special
        UndergroundJungle,
        UndergroundCorruption,
        UndergroundCrimson,
        UndergroundEvil,
        UndergroundHallow,
        
        // Dangerous
        Dungeon,
        DungeonPostPlantera,
        Hell,
        Temple,
        
        // Events
        BloodMoon,
        GoblinInvasion,
        PirateInvasion,
        SolarEclipse,
        PumpkinMoon,
        FrostMoon,
        MartianInvasion,
        LunarEvent,
        OldOnesArmy
    }
}
