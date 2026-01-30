using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using System.Collections.Generic;
using Rpg.Common.Compatibility;

namespace Rpg.Common.Systems
{
    /// <summary>
    /// Tracks boss progression and updates World Level accordingly
    /// Uses vanilla downedBoss flags to detect progression
    /// </summary>
    public class WorldProgression : ModSystem
    {
        // Track which bosses we've already processed (prevent duplicate world level increases)
        private static HashSet<int> processedBosses = new();

        public override void OnWorldLoad()
        {
            processedBosses.Clear();
            SeedProcessedBossesFromProgress();
        }

        public override void OnWorldUnload()
        {
            processedBosses.Clear();
        }

        #region Boss Detection

        /// <summary>
        /// Called when an NPC is killed - check if it's a boss
        /// </summary>
        public static void OnNPCKilled(NPC npc)
        {
            if (!npc.boss)
                return;

            // Get boss type
            int bossType = npc.type;

            // Already processed this boss?
            if (processedBosses.Contains(bossType))
                return;

            // Check which boss was killed and increase world level
            ProcessBossKill(bossType);
        }

        #endregion

        #region Progress Sync

        private static void SeedProcessedBossesFromProgress()
        {
            if (NPC.downedSlimeKing)
                processedBosses.Add(NPCID.KingSlime);

            if (NPC.downedBoss1)
                processedBosses.Add(NPCID.EyeofCthulhu);

            if (NPC.downedBoss2)
            {
                processedBosses.Add(NPCID.EaterofWorldsHead);
                processedBosses.Add(NPCID.BrainofCthulhu);
            }

            if (NPC.downedQueenBee)
                processedBosses.Add(NPCID.QueenBee);

            if (NPC.downedBoss3)
                processedBosses.Add(NPCID.SkeletronHead);

            if (NPC.downedDeerclops)
                processedBosses.Add(NPCID.Deerclops);

            if (Main.hardMode)
                processedBosses.Add(NPCID.WallofFlesh);

            if (NPC.downedQueenSlime)
                processedBosses.Add(NPCID.QueenSlimeBoss);

            if (NPC.downedMechBoss1)
                processedBosses.Add(NPCID.TheDestroyer);

            if (NPC.downedMechBoss2)
            {
                processedBosses.Add(NPCID.Retinazer);
                processedBosses.Add(NPCID.Spazmatism);
            }

            if (NPC.downedMechBoss3)
                processedBosses.Add(NPCID.SkeletronPrime);

            if (NPC.downedPlantBoss)
                processedBosses.Add(NPCID.Plantera);

            if (NPC.downedGolemBoss)
                processedBosses.Add(NPCID.Golem);

            if (NPC.downedFishron)
                processedBosses.Add(NPCID.DukeFishron);

            if (NPC.downedEmpressOfLight)
                processedBosses.Add(NPCID.HallowBoss);

            if (NPC.downedAncientCultist)
                processedBosses.Add(NPCID.CultistBoss);

            if (NPC.downedMoonlord)
                processedBosses.Add(NPCID.MoonLordCore);
        }

        #endregion

        #region World Level Updates

        /// <summary>
        /// Increase world level based on boss killed
        /// </summary>
        private static void ProcessBossKill(int bossType)
        {
            int worldLevelIncrease = 0;

            // Pre-Hardmode bosses
            if (bossType == NPCID.KingSlime)
                worldLevelIncrease = RpgConstants.WL_INCREASE_SMALL;
            
            else if (bossType == NPCID.EyeofCthulhu)
                worldLevelIncrease = RpgConstants.WL_INCREASE_SMALL;
            
            else if (bossType == NPCID.EaterofWorldsHead || bossType == NPCID.BrainofCthulhu)
                worldLevelIncrease = RpgConstants.WL_INCREASE_MEDIUM;
            
            else if (bossType == NPCID.QueenBee)
                worldLevelIncrease = RpgConstants.WL_INCREASE_SMALL;
            
            else if (bossType == NPCID.SkeletronHead)
                worldLevelIncrease = RpgConstants.WL_INCREASE_MEDIUM;
            
            else if (bossType == NPCID.Deerclops)
                worldLevelIncrease = RpgConstants.WL_INCREASE_SMALL;
            
            else if (bossType == NPCID.WallofFlesh)
                worldLevelIncrease = RpgConstants.WL_INCREASE_LARGE; // Big jump to Hardmode
            
            // Hardmode bosses
            else if (bossType == NPCID.QueenSlimeBoss)
                worldLevelIncrease = RpgConstants.WL_INCREASE_MEDIUM;
            
            else if (bossType == NPCID.TheDestroyer || bossType == NPCID.Retinazer || 
                     bossType == NPCID.Spazmatism || bossType == NPCID.SkeletronPrime)
                worldLevelIncrease = RpgConstants.WL_INCREASE_MEDIUM;
            
            else if (bossType == NPCID.Plantera)
                worldLevelIncrease = RpgConstants.WL_INCREASE_LARGE;
            
            else if (bossType == NPCID.Golem)
                worldLevelIncrease = RpgConstants.WL_INCREASE_MEDIUM;
            
            else if (bossType == NPCID.DukeFishron)
                worldLevelIncrease = RpgConstants.WL_INCREASE_MEDIUM;
            
            else if (bossType == NPCID.HallowBoss) // Empress of Light
                worldLevelIncrease = RpgConstants.WL_INCREASE_SMALL;
            
            else if (bossType == NPCID.CultistBoss)
                worldLevelIncrease = RpgConstants.WL_INCREASE_MEDIUM;
            
            else if (bossType == NPCID.MoonLordCore)
                worldLevelIncrease = RpgConstants.WL_INCREASE_LARGE;
            
            // Check for mod bosses
            else
            {
                var modBossData = ModCompatibilitySystem.GetModBossData(bossType);
                if (modBossData.HasValue)
                {
                    worldLevelIncrease = modBossData.Value.worldLevel;
                    bool firstKill = ModCompatibilitySystem.RegisterModBossKill(bossType);
                    if (firstKill && modBossData.Value.levelCap > 0)
                    {
                        AnnounceModBossCapIncrease(modBossData.Value.levelCap);
                    }
                }
            }

            // Apply world level increase
            if (worldLevelIncrease > 0)
            {
                RpgWorld.IncreaseWorldLevel(worldLevelIncrease);
                processedBosses.Add(bossType);
            }
        }

        #endregion

        #region Utility

        private static void AnnounceModBossCapIncrease(int capIncrease)
        {
            if (Main.netMode == NetmodeID.Server)
                return;

            string message = $"Mod Boss defeated! Level Cap increased by {capIncrease}.";
            Main.NewText(message, Microsoft.Xna.Framework.Color.LightSkyBlue);
        }

        /// <summary>
        /// Calculate world level from current boss flags (no side effects).
        /// </summary>
        public static int CalculateWorldLevelFromProgress()
        {
            int totalIncrease = 0;

            // Check all boss flags and sum up increases
            if (NPC.downedSlimeKing) totalIncrease += RpgConstants.WL_INCREASE_SMALL;
            if (NPC.downedBoss1) totalIncrease += RpgConstants.WL_INCREASE_SMALL; // Eye
            if (NPC.downedBoss2) totalIncrease += RpgConstants.WL_INCREASE_MEDIUM; // Eater/Brain
            if (NPC.downedQueenBee) totalIncrease += RpgConstants.WL_INCREASE_SMALL;
            if (NPC.downedBoss3) totalIncrease += RpgConstants.WL_INCREASE_MEDIUM; // Skeletron
            if (NPC.downedDeerclops) totalIncrease += RpgConstants.WL_INCREASE_SMALL;
            if (Main.hardMode) totalIncrease += RpgConstants.WL_INCREASE_LARGE; // Wall of Flesh

            if (NPC.downedQueenSlime) totalIncrease += RpgConstants.WL_INCREASE_MEDIUM;
            if (NPC.downedMechBoss1) totalIncrease += RpgConstants.WL_INCREASE_MEDIUM; // Destroyer
            if (NPC.downedMechBoss2) totalIncrease += RpgConstants.WL_INCREASE_MEDIUM; // Twins
            if (NPC.downedMechBoss3) totalIncrease += RpgConstants.WL_INCREASE_MEDIUM; // Prime
            if (NPC.downedPlantBoss) totalIncrease += RpgConstants.WL_INCREASE_LARGE; // Plantera
            if (NPC.downedGolemBoss) totalIncrease += RpgConstants.WL_INCREASE_MEDIUM;
            if (NPC.downedFishron) totalIncrease += RpgConstants.WL_INCREASE_MEDIUM;
            if (NPC.downedEmpressOfLight) totalIncrease += RpgConstants.WL_INCREASE_SMALL;
            if (NPC.downedAncientCultist) totalIncrease += RpgConstants.WL_INCREASE_MEDIUM;
            if (NPC.downedMoonlord) totalIncrease += RpgConstants.WL_INCREASE_LARGE;

            // Set world level (start at 1, add all increases)
            return 1 + totalIncrease;
        }

        /// <summary>
        /// Manually recalculate world level from current boss flags
        /// (useful for existing worlds)
        /// </summary>
        public static void RecalculateWorldLevel(bool showMessage = true)
        {
            processedBosses.Clear();
            int newWorldLevel = CalculateWorldLevelFromProgress();

            RpgWorld.SetWorldLevel(newWorldLevel);
            SeedProcessedBossesFromProgress();

            if (showMessage && Main.netMode != NetmodeID.Server)
                Main.NewText($"World Level recalculated: {newWorldLevel}", Microsoft.Xna.Framework.Color.Orange);
        }

        /// <summary>
        /// Get current progression stage based on processed bosses
        /// </summary>
        public static string GetCurrentProgressionStage()
        {
            if (processedBosses.Contains(NPCID.MoonLordCore))
                return "Post-Moon Lord";
            if (processedBosses.Contains(NPCID.Golem))
                return "Post-Golem";
            if (processedBosses.Contains(NPCID.Plantera))
                return "Post-Plantera";
            if (processedBosses.Contains(NPCID.TheDestroyer) || processedBosses.Contains(NPCID.Retinazer) || 
                processedBosses.Contains(NPCID.Spazmatism) || processedBosses.Contains(NPCID.SkeletronPrime))
                return "Mid Hardmode";
            if (processedBosses.Contains(NPCID.WallofFlesh))
                return "Early Hardmode";
            return "Pre-Hardmode";
        }

        #endregion
    }
}
