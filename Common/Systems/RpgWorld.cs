using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using System;

namespace RpgMod.Common.Systems
{
    /// <summary>
    /// Manages World Level - increases as bosses are defeated
    /// World Level affects monster scaling (HP/Damage)
    /// </summary>
    public class RpgWorld : ModSystem
    {
        public static int WorldLevel { get; private set; } = 1;

        public override void OnWorldLoad()
        {
            WorldLevel = 1;
        }

        public override void OnWorldUnload()
        {
            WorldLevel = 1;
        }

        #region World Level Management

        /// <summary>
        /// Increase world level by specified amount
        /// </summary>
        public static void IncreaseWorldLevel(int amount)
        {
            WorldLevel += amount;
            
            // Cap at max level
            if (WorldLevel > RpgConstants.MAX_WORLD_LEVEL)
                WorldLevel = RpgConstants.MAX_WORLD_LEVEL;

            // Show notification
            ShowWorldLevelIncreaseMessage(amount);
        }
        
        /// <summary>
        /// Set world level to a specific value (for debug commands)
        /// </summary>
        public static void SetWorldLevel(int level)
        {
            WorldLevel = Math.Clamp(level, 1, RpgConstants.MAX_WORLD_LEVEL);
        }

        /// <summary>
        /// Get current world level
        /// </summary>
        public static int GetWorldLevel()
        {
            return WorldLevel;
        }
        
        /// <summary>
        /// Get current world level capped by progression stage
        /// Pre-hardmode: max 20
        /// Early Hardmode: max 40
        /// Mid Hardmode (mech bosses): max 60
        /// Late Hardmode (plantera/golem): max 80
        /// Post Moon Lord: uncapped (up to MAX_WORLD_LEVEL)
        /// </summary>
        public static int GetEffectiveWorldLevel()
        {
            int cap = GetProgressionWorldLevelCap();
            return Math.Min(WorldLevel, cap);
        }
        
        /// <summary>
        /// Get max world level for current progression
        /// </summary>
        public static int GetProgressionWorldLevelCap()
        {
            if (NPC.downedMoonlord)
                return RpgConstants.MAX_WORLD_LEVEL; // Uncapped
            if (NPC.downedGolemBoss || NPC.downedPlantBoss)
                return 80;  // Late hardmode
            if (NPC.downedMechBossAny)
                return 60;  // Mid hardmode
            if (Main.hardMode)
                return 40;  // Early hardmode
            return 20;      // Pre-hardmode
        }
        
        /// <summary>
        /// Get the current progression stage name
        /// </summary>
        public static string GetProgressionStageName()
        {
            if (NPC.downedMoonlord)
                return "Post-Moon Lord";
            if (NPC.downedGolemBoss)
                return "Post-Golem";
            if (NPC.downedPlantBoss)
                return "Post-Plantera";
            if (NPC.downedMechBossAny)
                return "Mid Hardmode";
            if (Main.hardMode)
                return "Early Hardmode";
            return "Pre-Hardmode";
        }

        /// <summary>
        /// Get world level XP multiplier
        /// </summary>
        public static float GetXPMultiplier()
        {
            return RpgFormulas.GetWorldLevelXPMultiplier(GetEffectiveWorldLevel());
        }

        /// <summary>
        /// Get monster HP multiplier for current world level
        /// </summary>
        public static float GetMonsterHPMultiplier()
        {
            return RpgFormulas.GetMonsterHPMultiplier(WorldLevel);
        }

        /// <summary>
        /// Get monster damage multiplier for current world level
        /// </summary>
        public static float GetMonsterDamageMultiplier()
        {
            return RpgFormulas.GetMonsterDamageMultiplier(WorldLevel);
        }

        #endregion

        #region Visual Feedback

        /// <summary>
        /// Show world level increase message to all players
        /// </summary>
        private static void ShowWorldLevelIncreaseMessage(int amount)
        {
            if (Main.netMode == NetmodeID.Server)
                return;

            string message = $"World Level increased by {amount}! (Now: {WorldLevel})";
            Main.NewText(message, Microsoft.Xna.Framework.Color.Orange);
        }

        #endregion

        #region Save/Load

        public override void SaveWorldData(TagCompound tag)
        {
            tag["worldLevel"] = WorldLevel;
        }

        public override void LoadWorldData(TagCompound tag)
        {
            int loadedLevel = tag.ContainsKey("worldLevel") ? tag.GetInt("worldLevel") : 1;
            WorldLevel = Math.Clamp(loadedLevel, 1, RpgConstants.MAX_WORLD_LEVEL);
            
            if (WorldLevel <= 1)
            {
                int progressLevel = WorldProgression.CalculateWorldLevelFromProgress();
                if (progressLevel > WorldLevel)
                    WorldLevel = Math.Min(progressLevel, RpgConstants.MAX_WORLD_LEVEL);
            }
        }

        #endregion

        #region Networking

        public override void NetSend(System.IO.BinaryWriter writer)
        {
            writer.Write(WorldLevel);
        }

        public override void NetReceive(System.IO.BinaryReader reader)
        {
            WorldLevel = reader.ReadInt32();
        }

        #endregion
    }
}
