using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace RpgMod.Common.Systems
{
    /// <summary>
    /// Cooldown Reduction System - Reduces skill cooldowns based on stats and equipment
    /// </summary>
    public static class CooldownReductionSystem
    {
        // Base cooldown reduction caps
        public const float MAX_CDR_PERCENT = 50f; // Maximum 50% cooldown reduction
        public const float CDR_PER_INT = 0.1f; // 0.1% CDR per INT point
        public const float CDR_PER_DEX = 0.05f; // 0.05% CDR per DEX point
        
        // Cooldown reduction sources
        public enum CDRSource
        {
            BaseStats,      // From INT/DEX stats
            Equipment,      // From equipment bonuses
            Buffs,          // From active buffs
            SetBonus,       // From equipment set bonuses
            JobPassive      // From job passive abilities
        }
        
        /// <summary>
        /// Calculate total cooldown reduction percentage for a player
        /// </summary>
        public static float CalculateTotalCDR(Player player)
        {
            float totalCDR = 0f;
            
            var rpgPlayer = player.GetModPlayer<Players.RpgPlayer>();
            
            // CDR from INT stat
            totalCDR += rpgPlayer.TotalIntelligence * CDR_PER_INT;
            
            // CDR from DEX stat (smaller bonus)
            totalCDR += rpgPlayer.TotalDexterity * CDR_PER_DEX;
            
            // CDR from equipment (stored in player)
            totalCDR += rpgPlayer.EquipmentCDR;
            
            // CDR from buffs
            totalCDR += GetBuffCDR(player);
            
            // CDR from set bonuses
            totalCDR += GetSetBonusCDR(player);
            
            // CDR from job passives
            totalCDR += GetJobPassiveCDR(rpgPlayer);
            
            // Cap at maximum
            return Math.Min(totalCDR, MAX_CDR_PERCENT);
        }
        
        /// <summary>
        /// Apply cooldown reduction to a base cooldown
        /// </summary>
        public static int ApplyCDR(int baseCooldown, float cdrPercent)
        {
            float reduction = 1f - (cdrPercent / 100f);
            int reducedCooldown = (int)(baseCooldown * reduction);
            return Math.Max(reducedCooldown, 1); // Minimum 1 tick cooldown
        }
        
        /// <summary>
        /// Get cooldown reduction from active buffs
        /// </summary>
        private static float GetBuffCDR(Player player)
        {
            float buffCDR = 0f;
            
            // Swiftness potion: 2% CDR
            if (player.HasBuff(Terraria.ID.BuffID.Swiftness))
                buffCDR += 2f;
            
            // Magic Power: 3% CDR
            if (player.HasBuff(Terraria.ID.BuffID.MagicPower))
                buffCDR += 3f;
            
            // Wrath: 2% CDR
            if (player.HasBuff(Terraria.ID.BuffID.Wrath))
                buffCDR += 2f;
            
            // Endurance: 1% CDR
            if (player.HasBuff(Terraria.ID.BuffID.Endurance))
                buffCDR += 1f;
            
            // Well Fed variants
            if (player.HasBuff(Terraria.ID.BuffID.WellFed3)) // Exquisitely Stuffed
                buffCDR += 3f;
            else if (player.HasBuff(Terraria.ID.BuffID.WellFed2)) // Plenty Satisfied
                buffCDR += 2f;
            else if (player.HasBuff(Terraria.ID.BuffID.WellFed)) // Well Fed
                buffCDR += 1f;
            
            return buffCDR;
        }
        
        /// <summary>
        /// Get cooldown reduction from equipment set bonuses
        /// </summary>
        private static float GetSetBonusCDR(Player player)
        {
            float setCDR = 0f;
            
            // Check for specific armor sets
            // Meteor Set: 5% CDR
            if (player.setBonus.Contains("meteor"))
                setCDR += 5f;
            
            // Spectre Set: 7% CDR
            if (player.setBonus.Contains("spectre"))
                setCDR += 7f;
            
            // Nebula Set: 10% CDR
            if (player.setBonus.Contains("nebula"))
                setCDR += 10f;
            
            // Stardust Set: 5% CDR
            if (player.setBonus.Contains("stardust"))
                setCDR += 5f;
            
            // Vortex Set: 5% CDR
            if (player.setBonus.Contains("vortex"))
                setCDR += 5f;
            
            // Solar Set: 3% CDR
            if (player.setBonus.Contains("solar"))
                setCDR += 3f;
            
            // Titanium Set: 5% CDR
            if (player.setBonus.Contains("titanium"))
                setCDR += 5f;
            
            // Hallowed Set: 5% CDR
            if (player.setBonus.Contains("hallowed"))
                setCDR += 5f;
            
            return setCDR;
        }
        
        /// <summary>
        /// Get cooldown reduction from job passives
        /// </summary>
        private static float GetJobPassiveCDR(Players.RpgPlayer rpgPlayer)
        {
            float jobCDR = 0f;
            
            // Job-based CDR bonuses
            switch (rpgPlayer.CurrentJob)
            {
                case JobType.Mage:
                case JobType.Archmage:
                case JobType.Sorcerer:
                    // Mage line gets more CDR
                    jobCDR += (int)rpgPlayer.CurrentTier * 3f;
                    break;
                    
                case JobType.Assassin:
                case JobType.Shadow:
                    // Assassin line gets moderate CDR
                    jobCDR += (int)rpgPlayer.CurrentTier * 2f;
                    break;
                    
                case JobType.Ranger:
                case JobType.Sniper:
                case JobType.Deadeye:
                    // Ranger line gets small CDR
                    jobCDR += (int)rpgPlayer.CurrentTier * 1.5f;
                    break;
                    
                default:
                    // Other jobs get minimal CDR
                    jobCDR += (int)rpgPlayer.CurrentTier * 1f;
                    break;
            }
            
            return jobCDR;
        }
        
        /// <summary>
        /// Get detailed CDR breakdown for UI display
        /// </summary>
        public static Dictionary<CDRSource, float> GetCDRBreakdown(Player player)
        {
            var breakdown = new Dictionary<CDRSource, float>();
            var rpgPlayer = player.GetModPlayer<Players.RpgPlayer>();
            
            // Base stats
            float statCDR = (rpgPlayer.TotalIntelligence * CDR_PER_INT) + (rpgPlayer.TotalDexterity * CDR_PER_DEX);
            breakdown[CDRSource.BaseStats] = statCDR;
            
            // Equipment
            breakdown[CDRSource.Equipment] = rpgPlayer.EquipmentCDR;
            
            // Buffs
            breakdown[CDRSource.Buffs] = GetBuffCDR(player);
            
            // Set Bonus
            breakdown[CDRSource.SetBonus] = GetSetBonusCDR(player);
            
            // Job Passive
            breakdown[CDRSource.JobPassive] = GetJobPassiveCDR(rpgPlayer);
            
            return breakdown;
        }
    }
}
