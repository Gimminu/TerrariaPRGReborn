using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using Rpg.Common.Config;
using Rpg.Common.Systems;

namespace Rpg.Common.Compatibility
{
    /// <summary>
    /// Handles compatibility with other popular mods
    /// </summary>
    public class ModCompatibilitySystem : ModSystem
    {
        public static bool CalamityLoaded { get; private set; }
        public static bool ThoriumLoaded { get; private set; }
        public static bool SpiritLoaded { get; private set; }
        public static bool StarsAboveLoaded { get; private set; }
        public static bool FargosLoaded { get; private set; }
        
        private static Mod calamityMod;
        private static Mod thoriumMod;
        
        /// <summary>
        /// Boss progression data for mod bosses
        /// Key: NPC type ID, Value: (World Level Increase, Level Cap Increase)
        /// </summary>
        private static readonly Dictionary<int, (int worldLevel, int levelCap)> modBossData = new();
        
        public override void Load()
        {
            // Detect loaded mods
            CalamityLoaded = ModLoader.TryGetMod("CalamityMod", out calamityMod);
            ThoriumLoaded = ModLoader.TryGetMod("ThoriumMod", out thoriumMod);
            SpiritLoaded = ModLoader.HasMod("SpiritMod");
            StarsAboveLoaded = ModLoader.HasMod("StarsAbove");
            FargosLoaded = ModLoader.HasMod("FargowiltasSouls");
            
            if (CalamityLoaded)
                RegisterCalamityBosses();
            
            if (ThoriumLoaded)
                RegisterThoriumBosses();
            
            LogModCompatibility();
        }
        
        public override void Unload()
        {
            modBossData.Clear();
            calamityMod = null;
            thoriumMod = null;
        }
        
        private void LogModCompatibility()
        {
            Mod.Logger.Info("=== Mod Compatibility Status ===");
            Mod.Logger.Info($"Calamity: {(CalamityLoaded ? "Detected" : "Not Found")}");
            Mod.Logger.Info($"Thorium: {(ThoriumLoaded ? "Detected" : "Not Found")}");
            Mod.Logger.Info($"Spirit: {(SpiritLoaded ? "Detected" : "Not Found")}");
            Mod.Logger.Info($"Stars Above: {(StarsAboveLoaded ? "Detected" : "Not Found")}");
            Mod.Logger.Info($"Fargos: {(FargosLoaded ? "Detected" : "Not Found")}");
        }
        
        #region Calamity Support
        
        private void RegisterCalamityBosses()
        {
            var config = ModContent.GetInstance<RpgServerConfig>();
            if (config?.EnableCalamityCompatibility != true)
                return;
            
            // Register Calamity bosses with their world level contributions
            // Pre-Hardmode
            RegisterModBoss("CalamityMod", "DesertScourgeHead", 2, 5);      // Desert Scourge
            RegisterModBoss("CalamityMod", "Crabulon", 2, 5);               // Crabulon
            RegisterModBoss("CalamityMod", "HiveMind", 3, 8);               // Hive Mind
            RegisterModBoss("CalamityMod", "PerforatorHive", 3, 8);         // Perforators
            RegisterModBoss("CalamityMod", "SlimeGodCore", 4, 10);          // Slime God
            
            // Hardmode
            RegisterModBoss("CalamityMod", "Cryogen", 4, 10);               // Cryogen
            RegisterModBoss("CalamityMod", "AquaticScourgeHead", 4, 10);    // Aquatic Scourge
            RegisterModBoss("CalamityMod", "BrimstoneElemental", 5, 12);    // Brimstone Elemental
            RegisterModBoss("CalamityMod", "CalamitasClone", 6, 15);        // Calamitas Clone
            RegisterModBoss("CalamityMod", "Leviathan", 5, 12);             // Leviathan
            RegisterModBoss("CalamityMod", "AstrumAureus", 5, 12);          // Astrum Aureus
            RegisterModBoss("CalamityMod", "PlaguebringerGoliath", 6, 15);  // Plaguebringer
            RegisterModBoss("CalamityMod", "RavagerBody", 7, 18);           // Ravager
            RegisterModBoss("CalamityMod", "AstrumDeusHead", 7, 18);        // Astrum Deus
            
            // Post-Moon Lord
            RegisterModBoss("CalamityMod", "Bumblefuck", 8, 20);            // Dragonfolly
            RegisterModBoss("CalamityMod", "Providence", 10, 25);           // Providence
            RegisterModBoss("CalamityMod", "CeaselessVoid", 8, 20);         // Ceaseless Void
            RegisterModBoss("CalamityMod", "StormWeaverHead", 8, 20);       // Storm Weaver
            RegisterModBoss("CalamityMod", "Signus", 8, 20);                // Signus
            RegisterModBoss("CalamityMod", "Polterghast", 10, 25);          // Polterghast
            RegisterModBoss("CalamityMod", "OldDuke", 10, 25);              // Old Duke
            RegisterModBoss("CalamityMod", "DevourerofGodsHead", 12, 30);   // Devourer of Gods
            RegisterModBoss("CalamityMod", "Yharon", 15, 40);               // Yharon
            RegisterModBoss("CalamityMod", "SupremeCalamitas", 20, 50);     // Supreme Calamitas
            
            Mod.Logger.Info($"Registered {modBossData.Count} Calamity bosses");
        }
        
        #endregion
        
        #region Thorium Support
        
        private void RegisterThoriumBosses()
        {
            var config = ModContent.GetInstance<RpgServerConfig>();
            if (config?.EnableThoriumCompatibility != true)
                return;
            
            // Pre-Hardmode
            RegisterModBoss("ThoriumMod", "TheGrandThunderBird", 2, 5);
            RegisterModBoss("ThoriumMod", "QueenJellyfish", 2, 5);
            RegisterModBoss("ThoriumMod", "Viscount", 3, 8);
            RegisterModBoss("ThoriumMod", "GraniteEnergyStorm", 3, 8);
            RegisterModBoss("ThoriumMod", "BuriedChampion", 4, 10);
            RegisterModBoss("ThoriumMod", "StarScouter", 4, 10);
            
            // Hardmode
            RegisterModBoss("ThoriumMod", "BoreanStrider", 5, 12);
            RegisterModBoss("ThoriumMod", "FallenBeholder", 5, 12);
            RegisterModBoss("ThoriumMod", "Lich", 6, 15);
            RegisterModBoss("ThoriumMod", "ForgottenOne", 7, 18);
            RegisterModBoss("ThoriumMod", "Primordials", 10, 25);
            
            Mod.Logger.Info($"Registered Thorium bosses");
        }
        
        #endregion
        
        #region Boss Registration
        
        private void RegisterModBoss(string modName, string npcName, int worldLevelIncrease, int levelCapIncrease)
        {
            if (!ModLoader.TryGetMod(modName, out var mod))
                return;
            
            if (mod.TryFind<ModNPC>(npcName, out var modNPC))
            {
                modBossData[modNPC.Type] = (worldLevelIncrease, levelCapIncrease);
            }
        }
        
        /// <summary>
        /// Get boss data for a mod boss (if registered)
        /// </summary>
        public static (int worldLevel, int levelCap)? GetModBossData(int npcType)
        {
            if (modBossData.TryGetValue(npcType, out var data))
                return data;
            return null;
        }
        
        /// <summary>
        /// Check if an NPC is a registered mod boss
        /// </summary>
        public static bool IsModBoss(int npcType)
        {
            return modBossData.ContainsKey(npcType);
        }
        
        #endregion
        
        #region XP Scaling for Mod Content
        
        /// <summary>
        /// Get XP multiplier for mod-added content
        /// </summary>
        public static float GetModXPMultiplier(NPC npc)
        {
            // Calamity enemies tend to be tankier, give more XP
            if (CalamityLoaded && IsFromMod(npc, "CalamityMod"))
                return 1.2f;
            
            // Thorium is more balanced with vanilla
            if (ThoriumLoaded && IsFromMod(npc, "ThoriumMod"))
                return 1.1f;
            
            // Spirit mod enemies
            if (SpiritLoaded && IsFromMod(npc, "SpiritMod"))
                return 1.1f;
            
            return 1.0f;
        }
        
        private static bool IsFromMod(NPC npc, string modName)
        {
            return npc.ModNPC?.Mod?.Name == modName;
        }
        
        #endregion
        
        #region Difficulty Scaling
        
        /// <summary>
        /// Get scaling adjustment for mod difficulty
        /// Calamity's Death/Revengeance modes need special handling
        /// </summary>
        public static float GetModDifficultyScaling()
        {
            if (CalamityLoaded)
            {
                // Check for Calamity difficulty modes via ModCalls
                try
                {
                    if (calamityMod != null)
                    {
                        // Revengeance mode check
                        var revengeance = calamityMod.Call("GetDifficultyActive", "revengeance");
                        if (revengeance is true)
                            return 0.7f; // Reduce RPG scaling in Revengeance
                        
                        // Death mode check
                        var death = calamityMod.Call("GetDifficultyActive", "death");
                        if (death is true)
                            return 0.5f; // Further reduce in Death mode
                    }
                }
                catch
                {
                    // ModCall failed, use default
                }
            }
            
            return 1.0f;
        }
        
        #endregion
    }
}
