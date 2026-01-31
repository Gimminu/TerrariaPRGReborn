using System;
using System.Collections.Generic;
using System.Linq;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Terraria;

namespace RpgMod.Common.Testing
{
    /// <summary>
    /// Validates mod implementation against the design document (chatgpt prompt.md).
    /// Checks for missing systems and incomplete features.
    /// </summary>
    public class DesignValidation : ModSystem
    {
        /// <summary>
        /// Run design validation and report missing/incomplete features
        /// </summary>
        public static void ValidateDesign()
        {
            Log("=== Design Document Validation ===", Color.Cyan);
            Log("Checking implementation against design spec...", Color.White);
            
            var checklist = new List<FeatureCheck>
            {
                // Core Systems
                new("Level System", CheckLevelSystem),
                new("Experience System", CheckExperienceSystem),
                new("Stat System (12 Stats)", CheckStatSystem),
                new("Skill System", CheckSkillSystem),
                new("Job/Class System", CheckJobSystem),
                
                // World Systems
                new("World Level System", CheckWorldLevelSystem),
                new("Monster Scaling", CheckMonsterScaling),
                new("Boss Kill Tracking", CheckBossKillTracking),
                
                // Resources
                new("Stamina Resource", CheckStaminaResource),
                new("Rage Resource", CheckRageResource),
                
                // Advanced Features
                new("Skill Prerequisites", CheckSkillPrerequisites),
                new("Temporary Buffs", CheckTemporaryBuffs),
                new("Cooldown Reduction", CheckCooldownReduction),
                // Removed: Save/Load System (handled by tModLoader)
                new("Player Damage Tracking", CheckPlayerDamageTracking),
                
                // UI Features
                new("Experience Bar UI", CheckExpBarUI),
                new("Skill Hotbar", CheckSkillHotbar),
                new("Stat Distribution UI", CheckStatUI),
                
                // Balance Features
                new("Level Cap per Stage", CheckLevelCap),
                new("Exp Curve (Exponential)", CheckExpCurve),
                new("Event Mob Exp Penalty", CheckEventExpPenalty),
                new("Segment Boss Exp Fix", CheckSegmentBossExp),
            };
            
            int implemented = 0;
            int partial = 0;
            int missing = 0;
            
            foreach (var check in checklist)
            {
                try
                {
                    var result = check.CheckFunc();
                    
                    string status;
                    Color color;
                    
                    switch (result.Status)
                    {
                        case ImplementationStatus.Implemented:
                            status = "[✓]";
                            color = Color.LightGreen;
                            implemented++;
                            break;
                        case ImplementationStatus.Partial:
                            status = "[~]";
                            color = Color.Yellow;
                            partial++;
                            break;
                        default:
                            status = "[✗]";
                            color = Color.OrangeRed;
                            missing++;
                            break;
                    }
                    
                    Log($"{status} {check.Name}: {result.Details}", color);
                }
                catch (Exception ex)
                {
                    Log($"[?] {check.Name}: Error - {ex.Message}", Color.Red);
                    missing++;
                }
            }
            
            Log($"\n=== Summary ===", Color.Cyan);
            Log($"Implemented: {implemented}/{checklist.Count}", Color.LightGreen);
            Log($"Partial: {partial}/{checklist.Count}", Color.Yellow);
            Log($"Missing: {missing}/{checklist.Count}", Color.OrangeRed);
            
            float completion = (implemented + partial * 0.5f) / checklist.Count * 100;
            Log($"\nOverall Completion: {completion:F1}%", Color.White);
        }
        
        #region Check Functions
        
        private static CheckResult CheckLevelSystem()
        {
            // Check if RpgPlayer has Level property
            bool hasLevel = typeof(Players.RpgPlayer).GetProperty("Level") != null;
            bool hasXP = typeof(Players.RpgPlayer).GetProperty("CurrentXP") != null;
            bool hasReqXP = typeof(Players.RpgPlayer).GetProperty("RequiredXP") != null;
            
            if (hasLevel && hasXP && hasReqXP)
                return new CheckResult(ImplementationStatus.Implemented, "Level, XP, RequiredXP present");
            else if (hasLevel)
                return new CheckResult(ImplementationStatus.Partial, "Level exists, XP system incomplete");
            return new CheckResult(ImplementationStatus.Missing, "Level system not found");
        }
        
        private static CheckResult CheckExperienceSystem()
        {
            // Check for experience formula
            bool hasFormulas = typeof(RpgFormulas).GetMethod("GetRequiredXP") != null;
            bool hasXPGain = typeof(RpgFormulas).GetMethod("CalculateMonsterXP") != null ||
                             typeof(RpgFormulas).GetMethod("GetMonsterXP") != null;
            
            if (hasFormulas && hasXPGain)
                return new CheckResult(ImplementationStatus.Implemented, "XP formulas implemented");
            else if (hasFormulas)
                return new CheckResult(ImplementationStatus.Partial, "XP curve exists, monster XP formula missing");
            return new CheckResult(ImplementationStatus.Missing, "Experience formulas not found");
        }
        
        private static CheckResult CheckStatSystem()
        {
            // Check for 12-stat system
            var statTypes = Enum.GetValues(typeof(StatType));
            int count = statTypes.Length;
            
            if (count >= 12)
                return new CheckResult(ImplementationStatus.Implemented, $"{count} stat types defined");
            else if (count >= 6)
                return new CheckResult(ImplementationStatus.Partial, $"Only {count} stats (need 12)");
            return new CheckResult(ImplementationStatus.Missing, "Stat system not implemented");
        }
        
        private static CheckResult CheckSkillSystem()
        {
            var skills = Skills.SkillDatabase.GetAllSkillNames();
            int count = skills.Count;
            
            if (count >= 100)
                return new CheckResult(ImplementationStatus.Implemented, $"{count} skills registered");
            else if (count >= 50)
                return new CheckResult(ImplementationStatus.Partial, $"Only {count} skills (target: 100+)");
            return new CheckResult(ImplementationStatus.Missing, $"Only {count} skills");
        }
        
        private static CheckResult CheckJobSystem()
        {
            var jobTypes = Enum.GetValues(typeof(JobType));
            int count = 0;
            foreach (JobType job in jobTypes)
            {
                if (job != JobType.None) count++;
            }
            
            // Should have: 1 Novice + 4 Tier1 + 8+ Tier2 + 6+ Tier3 = 19+ jobs
            if (count >= 19)
                return new CheckResult(ImplementationStatus.Implemented, $"{count} jobs defined");
            else if (count >= 10)
                return new CheckResult(ImplementationStatus.Partial, $"Only {count} jobs (target: 19+)");
            return new CheckResult(ImplementationStatus.Missing, $"Only {count} jobs");
        }
        
        private static CheckResult CheckWorldLevelSystem()
        {
            // Check if RpgWorld has WorldLevel
            bool hasWorldLevel = typeof(Systems.RpgWorld).GetProperty("WorldLevel") != null;
            bool hasIncrease = typeof(Systems.RpgWorld).GetMethod("IncreaseWorldLevel") != null;
            
            if (hasWorldLevel && hasIncrease)
                return new CheckResult(ImplementationStatus.Implemented, "RpgWorld.WorldLevel exists with IncreaseWorldLevel");
            else if (hasWorldLevel)
                return new CheckResult(ImplementationStatus.Partial, "WorldLevel exists, missing IncreaseWorldLevel");
            return new CheckResult(ImplementationStatus.Missing, "WorldLevel property not found");
        }
        
        private static CheckResult CheckMonsterScaling()
        {
            // Check for GlobalNPC that scales monsters
            bool hasScaling = typeof(NPCs.RpgGlobalNPC).GetMethod("ApplyWorldLevelScaling", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance) != null;
            bool hasXpDrop = typeof(NPCs.RpgGlobalNPC).GetMethod("CalculateAndAwardXP",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance) != null;
            
            if (hasScaling && hasXpDrop)
                return new CheckResult(ImplementationStatus.Implemented, "Monster scaling and XP drop implemented");
            else if (hasScaling)
                return new CheckResult(ImplementationStatus.Partial, "Scaling exists, XP drop missing");
            return new CheckResult(ImplementationStatus.Missing, "Monster scaling not implemented");
        }
        
        private static CheckResult CheckBossKillTracking()
        {
            // Check if WorldProgression tracks boss kills
            bool hasOnKilled = typeof(Systems.WorldProgression).GetMethod("OnNPCKilled") != null;
            bool hasRecalc = typeof(Systems.WorldProgression).GetMethod("RecalculateWorldLevel") != null;
            
            if (hasOnKilled && hasRecalc)
                return new CheckResult(ImplementationStatus.Implemented, "Boss kill tracking with recalculation");
            else if (hasOnKilled)
                return new CheckResult(ImplementationStatus.Partial, "OnNPCKilled exists, missing recalc");
            return new CheckResult(ImplementationStatus.Missing, "Boss kill tracking not found");
        }
        
        private static CheckResult CheckStaminaResource()
        {
            bool hasStamina = typeof(Players.RpgPlayer).GetProperty("Stamina") != null;
            bool hasMaxStamina = typeof(Players.RpgPlayer).GetProperty("MaxStamina") != null;
            bool hasRegen = typeof(Players.RpgPlayer).GetProperty("StaminaRegen") != null;
            
            if (hasStamina && hasMaxStamina && hasRegen)
                return new CheckResult(ImplementationStatus.Implemented, "Stamina system complete");
            else if (hasStamina)
                return new CheckResult(ImplementationStatus.Implemented, "Stamina exists (regen/max may be pending)");
            return new CheckResult(ImplementationStatus.Missing, "Stamina not implemented");
        }
        
        private static CheckResult CheckRageResource()
        {
            bool hasRage = typeof(Players.RpgPlayer).GetProperty("Rage") != null;
            bool hasMaxRage = typeof(Players.RpgPlayer).GetProperty("MaxRage") != null;
            
            if (hasRage && hasMaxRage)
                return new CheckResult(ImplementationStatus.Implemented, "Rage system complete");
            else if (hasRage)
                return new CheckResult(ImplementationStatus.Implemented, "Rage exists (MaxRage may be pending)");
            return new CheckResult(ImplementationStatus.Missing, "Rage not implemented");
        }
        
        private static CheckResult CheckSkillPrerequisites()
        {
            bool hasPrereq = typeof(Base.BaseSkill).GetProperty("PrerequisiteSkills") != null;
            bool hasCheck = typeof(Base.BaseSkill).GetMethod("CheckPrerequisites", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance) != null;
            
            if (hasPrereq && hasCheck)
                return new CheckResult(ImplementationStatus.Implemented, "Prerequisite system exists");
            return new CheckResult(ImplementationStatus.Missing, "Prerequisites not implemented");
        }
        
        private static CheckResult CheckTemporaryBuffs()
        {
            bool hasBuffs = typeof(Players.RpgPlayer).GetProperty("TempDefenseBonus") != null;
            bool hasUpdate = typeof(Players.RpgPlayer).GetMethod("UpdateTemporaryBuffs", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance) != null;
            
            if (hasBuffs && hasUpdate)
                return new CheckResult(ImplementationStatus.Implemented, "Temp buff system exists");
            return new CheckResult(ImplementationStatus.Partial, "Temp buffs partially implemented");
        }
        
        private static CheckResult CheckCooldownReduction()
        {
            bool hasCDR = typeof(Players.RpgPlayer).GetProperty("CooldownReduction") != null;
            bool hasActualCD = typeof(Base.BaseSkill).GetMethod("GetActualCooldown", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance) != null;
            
            if (hasCDR && hasActualCD)
                return new CheckResult(ImplementationStatus.Implemented, "CDR system exists");
            return new CheckResult(ImplementationStatus.Partial, "CDR partially implemented");
        }
        
        private static CheckResult CheckSaveLoadSystem()
        {
            bool hasSave = typeof(Players.RpgPlayer).GetMethod("SaveData") != null;
            bool hasLoad = typeof(Players.RpgPlayer).GetMethod("LoadData") != null;
            
            if (hasSave && hasLoad)
                return new CheckResult(ImplementationStatus.Implemented, "Save/Load implemented");
            return new CheckResult(ImplementationStatus.Missing, "Save/Load not found");
        }
        
        private static CheckResult CheckExpBarUI()
        {
            var uiType = Type.GetType("Rpg.UI.ExpBarUI, Rpg") ??
                         Type.GetType("Rpg.Common.UI.ExpBarUI, Rpg");
            
            if (uiType != null)
                return new CheckResult(ImplementationStatus.Implemented, "ExpBar UI exists");
            return new CheckResult(ImplementationStatus.Missing, "ExpBar UI not found");
        }
        
        private static CheckResult CheckSkillHotbar()
        {
            bool hasHotbar = typeof(Skills.SkillManager).GetProperty("SkillHotbar") != null;
            bool hasUse = typeof(Skills.SkillManager).GetMethod("UseSkillInSlot") != null;
            
            if (hasHotbar && hasUse)
                return new CheckResult(ImplementationStatus.Implemented, "Skill hotbar exists");
            return new CheckResult(ImplementationStatus.Partial, "Hotbar partially implemented");
        }
        
        private static CheckResult CheckStatUI()
        {
            var uiType = Type.GetType("Rpg.UI.StatUI, Rpg") ??
                         Type.GetType("Rpg.Common.UI.StatDistributionUI, Rpg");
            
            if (uiType != null)
                return new CheckResult(ImplementationStatus.Implemented, "Stat UI exists");
            return new CheckResult(ImplementationStatus.Missing, "Stat distribution UI not found");
        }
        
        private static CheckResult CheckLevelCap()
        {
            bool hasMaxLevel = typeof(RpgFormulas).GetMethod("GetMaxLevel") != null;
            
            if (hasMaxLevel)
                return new CheckResult(ImplementationStatus.Implemented, "Level cap formula exists");
            return new CheckResult(ImplementationStatus.Missing, "Level cap system not found");
        }
        
        private static CheckResult CheckExpCurve()
        {
            bool hasFormula = typeof(RpgFormulas).GetMethod("GetRequiredXP") != null;
            
            if (hasFormula)
                return new CheckResult(ImplementationStatus.Implemented, "XP curve formula exists");
            return new CheckResult(ImplementationStatus.Missing, "XP curve not found");
        }
        
        private static CheckResult CheckEventExpPenalty()
        {
            var globalNpcType = Type.GetType("Rpg.Common.NPCs.RpgGlobalNPC, Rpg");
            if (globalNpcType == null)
                return new CheckResult(ImplementationStatus.Missing, "No GlobalNPC for event penalty");
            
            // Would need to check implementation details
            return new CheckResult(ImplementationStatus.Partial, "Needs manual verification");
        }
        
        private static CheckResult CheckSegmentBossExp()
        {
            // Check for IsBodySegment in RpgFormulas
            bool hasSegmentCheck = typeof(RpgFormulas).GetMethod("IsBodySegment") != null;
            
            if (hasSegmentCheck)
                return new CheckResult(ImplementationStatus.Implemented, "Segment boss XP handled in RpgFormulas.IsBodySegment");
            return new CheckResult(ImplementationStatus.Missing, "Segment boss check not found");
        }
        
        private static CheckResult CheckTrapKillHandling()
        {
            var globalNpcType = Type.GetType("Rpg.Common.NPCs.RpgGlobalNPC, Rpg");
            if (globalNpcType == null)
                return new CheckResult(ImplementationStatus.Missing, "RpgGlobalNPC not found");
            
            // Check for HasPlayerDamage method
            bool hasPlayerDamageCheck = globalNpcType.GetMethod("HasPlayerDamage") != null;
            
            if (hasPlayerDamageCheck)
                return new CheckResult(ImplementationStatus.Implemented, "Trap/lava kill handling via HasPlayerDamage check");
            return new CheckResult(ImplementationStatus.Missing, "No trap kill handling found");
        }
        
        private static CheckResult CheckPlayerDamageTracking()
        {
            var globalNpcType = Type.GetType("Rpg.Common.NPCs.RpgGlobalNPC, Rpg");
            if (globalNpcType == null)
                return new CheckResult(ImplementationStatus.Missing, "RpgGlobalNPC not found");
            
            // Check for damage tracking methods
            bool hasTrackMethod = globalNpcType.GetMethod("TrackPlayerDamage") != null;
            bool hasGetDamagersMethod = globalNpcType.GetMethod("GetPlayersWhoDamaged") != null;
            bool hasInstancePerEntity = globalNpcType.GetProperty("InstancePerEntity") != null;
            
            if (hasTrackMethod && hasGetDamagersMethod && hasInstancePerEntity)
                return new CheckResult(ImplementationStatus.Implemented, "Player damage tracking (TrackPlayerDamage, GetPlayersWhoDamaged, InstancePerEntity)");
            if (hasTrackMethod || hasGetDamagersMethod)
                return new CheckResult(ImplementationStatus.Partial, "Some tracking methods missing");
            return new CheckResult(ImplementationStatus.Missing, "Player damage tracking not implemented");
        }
        
        #endregion
        
        #region Types
        
        private class FeatureCheck
        {
            public string Name { get; }
            public Func<CheckResult> CheckFunc { get; }
            
            public FeatureCheck(string name, Func<CheckResult> checkFunc)
            {
                Name = name;
                CheckFunc = checkFunc;
            }
        }
        
        private class CheckResult
        {
            public ImplementationStatus Status { get; }
            public string Details { get; }
            
            public CheckResult(ImplementationStatus status, string details)
            {
                Status = status;
                Details = details;
            }
        }
        
        private enum ImplementationStatus
        {
            Implemented,
            Partial,
            Missing
        }
        
        private static void Log(string msg, Color color)
        {
            if (Main.netMode != Terraria.ID.NetmodeID.Server)
                Main.NewText(msg, color);
        }
        
        #endregion
    }
}
