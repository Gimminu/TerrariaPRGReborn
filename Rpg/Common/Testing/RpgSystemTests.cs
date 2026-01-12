using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Terraria;
using Terraria.ModLoader;
using Rpg.Common.Base;
using Rpg.Common.Jobs;
using Rpg.Common.Skills;
using Rpg.Common.Players;

namespace Rpg.Common.Testing
{
    /// <summary>
    /// Comprehensive testing system for the RPG mod.
    /// Run tests via chat command: /rpgtest [category]
    /// Categories: all, skills, jobs, balance, performance, integrity
    /// </summary>
    public class RpgTestSystem : ModSystem
    {
        private static List<TestResult> _results = new();
        private static Stopwatch _stopwatch = new();
        
        public static void RunAllTests(bool verbose = true)
        {
            _results.Clear();
            _stopwatch.Restart();
            
            Log("=== RPG System Test Suite ===", ConsoleColor.Cyan);
            Log($"Started at: {DateTime.Now:HH:mm:ss}");
            
            // Category 1: Skill System Tests
            RunSkillRegistrationTests();
            RunSkillDataIntegrityTests();
            RunSkillPrerequisiteTests();
            
            // Category 2: Job System Tests
            RunJobDefinitionTests();
            RunJobProgressionTests();
            
            // Category 3: Balance Tests
            RunBalanceTests();
            
            // Category 4: Performance Tests
            RunPerformanceTests();
            
            // Category 5: Data Integrity Tests
            RunDataIntegrityTests();
            
            _stopwatch.Stop();
            
            // Summary
            PrintSummary();
        }
        
        #region Skill Tests
        
        private static void RunSkillRegistrationTests()
        {
            Log("\n--- Skill Registration Tests ---", ConsoleColor.Yellow);
            
            // Test: All skills in database can be instantiated
            var allSkillNames = SkillDatabase.GetAllSkillNames();
            int validCount = 0;
            int invalidCount = 0;
            List<string> invalidSkills = new();
            
            foreach (var name in allSkillNames)
            {
                try
                {
                    var skill = SkillDatabase.GetSkill(name);
                    if (skill != null && !string.IsNullOrEmpty(skill.InternalName))
                    {
                        validCount++;
                    }
                    else
                    {
                        invalidCount++;
                        invalidSkills.Add(name);
                    }
                }
                catch (Exception ex)
                {
                    invalidCount++;
                    invalidSkills.Add($"{name} (Exception: {ex.Message})");
                }
            }
            
            AddResult("Skill Registration", 
                invalidCount == 0, 
                $"Valid: {validCount}, Invalid: {invalidCount}",
                invalidSkills);
            
            // Test: Skill names are unique
            var duplicates = allSkillNames.GroupBy(x => x).Where(g => g.Count() > 1).Select(g => g.Key).ToList();
            AddResult("Skill Name Uniqueness", 
                duplicates.Count == 0, 
                $"Duplicate names: {duplicates.Count}",
                duplicates);
        }
        
        private static void RunSkillDataIntegrityTests()
        {
            Log("\n--- Skill Data Integrity Tests ---", ConsoleColor.Yellow);
            
            var allSkillNames = SkillDatabase.GetAllSkillNames();
            List<string> missingDisplayName = new();
            List<string> missingDescription = new();
            List<string> invalidCooldown = new();
            List<string> invalidResourceCost = new();
            List<string> invalidMaxRank = new();
            List<string> activeWithNoCooldown = new();
            
            foreach (var name in allSkillNames)
            {
                var skill = SkillDatabase.GetSkill(name);
                if (skill == null) continue;
                
                // Check display name
                if (string.IsNullOrWhiteSpace(skill.DisplayName))
                    missingDisplayName.Add(name);
                
                // Check description
                if (string.IsNullOrWhiteSpace(skill.Description))
                    missingDescription.Add(name);
                
                // Check cooldown (active skills should have cooldown)
                if (skill.SkillType == SkillType.Active && skill.CooldownSeconds <= 0)
                    activeWithNoCooldown.Add(name);
                
                // Check for negative values
                if (skill.CooldownSeconds < 0)
                    invalidCooldown.Add(name);
                
                if (skill.ResourceCost < 0)
                    invalidResourceCost.Add(name);
                
                if (skill.MaxRank <= 0 || skill.MaxRank > 20)
                    invalidMaxRank.Add($"{name} (MaxRank: {skill.MaxRank})");
            }
            
            AddResult("Display Names", missingDisplayName.Count == 0, 
                $"Missing: {missingDisplayName.Count}", missingDisplayName.Take(10).ToList());
            AddResult("Descriptions", missingDescription.Count == 0, 
                $"Missing: {missingDescription.Count}", missingDescription.Take(10).ToList());
            AddResult("Active Skills Cooldown", activeWithNoCooldown.Count == 0, 
                $"Active skills without cooldown: {activeWithNoCooldown.Count}", activeWithNoCooldown);
            AddResult("Valid MaxRank", invalidMaxRank.Count == 0, 
                $"Invalid: {invalidMaxRank.Count}", invalidMaxRank);
        }
        
        private static void RunSkillPrerequisiteTests()
        {
            Log("\n--- Skill Prerequisite Tests ---", ConsoleColor.Yellow);
            
            var allSkillNames = SkillDatabase.GetAllSkillNames();
            List<string> invalidPrereqs = new();
            List<string> circularRefs = new();
            
            foreach (var name in allSkillNames)
            {
                var skill = SkillDatabase.GetSkill(name);
                if (skill?.PrerequisiteSkills == null || skill.PrerequisiteSkills.Length == 0) 
                    continue;
                
                foreach (var prereq in skill.PrerequisiteSkills)
                {
                    string[] parts = prereq.Split(':');
                    string prereqName = parts[0];
                    
                    // Check if prerequisite exists
                    if (!SkillDatabase.SkillExists(prereqName))
                    {
                        invalidPrereqs.Add($"{name} -> {prereqName} (not found)");
                    }
                    
                    // Check for self-reference
                    if (prereqName == name)
                    {
                        circularRefs.Add($"{name} -> self");
                    }
                    
                    // Check for circular reference (2-level deep)
                    var prereqSkill = SkillDatabase.GetSkill(prereqName);
                    if (prereqSkill?.PrerequisiteSkills != null)
                    {
                        foreach (var subPrereq in prereqSkill.PrerequisiteSkills)
                        {
                            if (subPrereq.Split(':')[0] == name)
                            {
                                circularRefs.Add($"{name} <-> {prereqName}");
                            }
                        }
                    }
                }
            }
            
            AddResult("Valid Prerequisites", invalidPrereqs.Count == 0, 
                $"Invalid: {invalidPrereqs.Count}", invalidPrereqs);
            AddResult("No Circular References", circularRefs.Count == 0, 
                $"Circular: {circularRefs.Count}", circularRefs);
        }
        
        #endregion
        
        #region Job Tests
        
        private static void RunJobDefinitionTests()
        {
            Log("\n--- Job Definition Tests ---", ConsoleColor.Yellow);
            
            List<string> missingData = new();
            List<string> invalidRequirements = new();
            int validJobs = 0;
            
            foreach (JobType jobType in Enum.GetValues(typeof(JobType)))
            {
                if (jobType == JobType.None) continue;
                
                try
                {
                    var jobData = JobDatabase.GetJobData(jobType);
                    
                    if (string.IsNullOrWhiteSpace(jobData.DisplayName))
                        missingData.Add($"{jobType}: Missing DisplayName");
                    
                    if (string.IsNullOrWhiteSpace(jobData.Description))
                        missingData.Add($"{jobType}: Missing Description");
                    
                    if (jobData.Requirements == null)
                        invalidRequirements.Add($"{jobType}: Null Requirements");
                    else if (jobData.Requirements.MinLevel < 1)
                        invalidRequirements.Add($"{jobType}: Invalid MinLevel ({jobData.Requirements.MinLevel})");
                    
                    validJobs++;
                }
                catch (Exception ex)
                {
                    missingData.Add($"{jobType}: Exception - {ex.Message}");
                }
            }
            
            AddResult("Job Definitions", missingData.Count == 0, 
                $"Valid jobs: {validJobs}, Issues: {missingData.Count}", missingData);
            AddResult("Job Requirements", invalidRequirements.Count == 0, 
                $"Invalid: {invalidRequirements.Count}", invalidRequirements);
        }
        
        private static void RunJobProgressionTests()
        {
            Log("\n--- Job Progression Tests ---", ConsoleColor.Yellow);
            
            // Test: All jobs have skill unlocks
            List<string> jobsWithNoSkills = new();
            List<string> jobsWithInvalidSkills = new();
            
            foreach (JobType jobType in Enum.GetValues(typeof(JobType)))
            {
                if (jobType == JobType.None) continue;
                
                var jobData = JobDatabase.GetJobData(jobType);
                
                if (jobData.SkillUnlocks == null || jobData.SkillUnlocks.Count == 0)
                {
                    jobsWithNoSkills.Add(jobType.ToString());
                    continue;
                }
                
                foreach (var skillName in jobData.SkillUnlocks)
                {
                    if (!SkillDatabase.SkillExists(skillName))
                    {
                        jobsWithInvalidSkills.Add($"{jobType}: {skillName}");
                    }
                }
            }
            
            AddResult("Jobs Have Skills", jobsWithNoSkills.Count == 0, 
                $"Jobs without skills: {jobsWithNoSkills.Count}", jobsWithNoSkills);
            AddResult("Valid Skill References", jobsWithInvalidSkills.Count == 0, 
                $"Invalid skill refs: {jobsWithInvalidSkills.Count}", jobsWithInvalidSkills);
        }
        
        #endregion
        
        #region Balance Tests
        
        private static void RunBalanceTests()
        {
            Log("\n--- Balance Tests ---", ConsoleColor.Yellow);
            
            var allSkillNames = SkillDatabase.GetAllSkillNames();
            List<string> highCooldown = new();
            List<string> highResourceCost = new();
            List<string> zeroCostActive = new();
            
            foreach (var name in allSkillNames)
            {
                var skill = SkillDatabase.GetSkill(name);
                if (skill == null) continue;
                
                // Check for extreme cooldowns (> 120 seconds)
                if (skill.CooldownSeconds > 120)
                    highCooldown.Add($"{name}: {skill.CooldownSeconds}s");
                
                // Check for extreme resource costs (> 200)
                if (skill.ResourceCost > 200)
                    highResourceCost.Add($"{name}: {skill.ResourceCost}");
                
                // Active skills with no resource cost
                if (skill.SkillType == SkillType.Active && skill.ResourceCost == 0 && skill.ResourceType != ResourceType.None)
                    zeroCostActive.Add(name);
            }
            
            AddResult("Reasonable Cooldowns", highCooldown.Count == 0, 
                $"High cooldown skills: {highCooldown.Count}", highCooldown, isWarning: true);
            AddResult("Reasonable Resource Costs", highResourceCost.Count == 0, 
                $"High cost skills: {highResourceCost.Count}", highResourceCost, isWarning: true);
            
            // Skill distribution by tier
            int noviceCount = 0, tier1Count = 0, tier2Count = 0, tier3Count = 0;
            foreach (var name in allSkillNames)
            {
                var skill = SkillDatabase.GetSkill(name);
                if (skill == null) continue;
                
                int level = skill.RequiredLevel;
                if (level < RpgConstants.FIRST_JOB_LEVEL) noviceCount++;
                else if (level < RpgConstants.SECOND_JOB_LEVEL) tier1Count++;
                else if (level < RpgConstants.THIRD_JOB_LEVEL) tier2Count++;
                else tier3Count++;
            }
            
            Log($"  Skill Distribution: Novice={noviceCount}, Tier1={tier1Count}, Tier2={tier2Count}, Tier3={tier3Count}");
        }
        
        #endregion
        
        #region Performance Tests
        
        private static void RunPerformanceTests()
        {
            Log("\n--- Performance Tests ---", ConsoleColor.Yellow);
            
            const int ITERATIONS = 10000;
            
            // Test: Skill lookup performance
            var skillNames = SkillDatabase.GetAllSkillNames();
            var testName = skillNames.FirstOrDefault() ?? "BasicStrike";
            
            var sw = Stopwatch.StartNew();
            for (int i = 0; i < ITERATIONS; i++)
            {
                _ = SkillDatabase.GetSkill(testName);
            }
            sw.Stop();
            double skillLookupMs = sw.Elapsed.TotalMilliseconds;
            
            AddResult("Skill Lookup Speed", skillLookupMs < 100, 
                $"{ITERATIONS} lookups in {skillLookupMs:F2}ms ({skillLookupMs/ITERATIONS*1000:F3}μs/lookup)");
            
            // Test: JobData lookup performance (should be cached now)
            sw.Restart();
            for (int i = 0; i < ITERATIONS; i++)
            {
                _ = JobDatabase.GetJobData(JobType.Warrior);
            }
            sw.Stop();
            double jobLookupMs = sw.Elapsed.TotalMilliseconds;
            
            AddResult("Job Lookup Speed (Cached)", jobLookupMs < 50, 
                $"{ITERATIONS} lookups in {jobLookupMs:F2}ms ({jobLookupMs/ITERATIONS*1000:F3}μs/lookup)");
            
            // Test: Skill instantiation performance
            sw.Restart();
            for (int i = 0; i < 1000; i++)
            {
                foreach (var name in skillNames.Take(10))
                {
                    _ = SkillDatabase.GetSkill(name);
                }
            }
            sw.Stop();
            double massInstMs = sw.Elapsed.TotalMilliseconds;
            
            AddResult("Mass Skill Instantiation", massInstMs < 500, 
                $"10000 instantiations in {massInstMs:F2}ms");
            
            // Stress test: Simulate many passive skill applications
            sw.Restart();
            var passiveSkills = new List<BaseSkill>();
            foreach (var name in skillNames.Take(50))
            {
                var skill = SkillDatabase.GetSkill(name);
                if (skill != null && skill.SkillType == SkillType.Passive)
                {
                    skill.CurrentRank = 1;
                    passiveSkills.Add(skill);
                }
            }
            
            // Simulate 60 FPS for 10 seconds (600 frames)
            for (int frame = 0; frame < 600; frame++)
            {
                foreach (var skill in passiveSkills)
                {
                    // Just call the method without actual player
                    // skill.ApplyPassive(null); // Can't test without player
                }
            }
            sw.Stop();
            
            Log($"  Passive Skills Found: {passiveSkills.Count}");
        }
        
        #endregion
        
        #region Data Integrity Tests
        
        private static void RunDataIntegrityTests()
        {
            Log("\n--- Data Integrity Tests ---", ConsoleColor.Yellow);
            
            // Check RpgConstants
            AddResult("FIRST_JOB_LEVEL Valid", 
                RpgConstants.FIRST_JOB_LEVEL > 1 && RpgConstants.FIRST_JOB_LEVEL <= 20,
                $"Value: {RpgConstants.FIRST_JOB_LEVEL}");
            
            AddResult("SECOND_JOB_LEVEL Valid", 
                RpgConstants.SECOND_JOB_LEVEL > RpgConstants.FIRST_JOB_LEVEL,
                $"Value: {RpgConstants.SECOND_JOB_LEVEL}");
            
            AddResult("THIRD_JOB_LEVEL Valid", 
                RpgConstants.THIRD_JOB_LEVEL > RpgConstants.SECOND_JOB_LEVEL,
                $"Value: {RpgConstants.THIRD_JOB_LEVEL}");
            
            // Check stat constants
            AddResult("Base HP Valid", 
                RpgConstants.VITALITY_HP_PER_POINT > 0,
                $"HP per VIT: {RpgConstants.VITALITY_HP_PER_POINT}");
            
            AddResult("Base Stamina Valid", 
                RpgConstants.BASE_MAX_STAMINA > 0,
                $"Base Stamina: {RpgConstants.BASE_MAX_STAMINA}");
            
            // Count total skills vs expected
            int totalSkills = SkillDatabase.GetAllSkillNames().Count;
            int expectedMin = 100; // We should have at least 100 skills
            
            AddResult("Minimum Skills Met", 
                totalSkills >= expectedMin,
                $"Total Skills: {totalSkills} (expected >= {expectedMin})");
        }
        
        #endregion
        
        #region Helpers
        
        private static void AddResult(string testName, bool passed, string details, 
            List<string> issues = null, bool isWarning = false)
        {
            var result = new TestResult
            {
                Name = testName,
                Passed = passed,
                Details = details,
                Issues = issues ?? new List<string>(),
                IsWarning = isWarning
            };
            
            _results.Add(result);
            
            string status = passed ? (isWarning ? "[WARN]" : "[PASS]") : "[FAIL]";
            var color = passed ? (isWarning ? ConsoleColor.Yellow : ConsoleColor.Green) : ConsoleColor.Red;
            
            Log($"  {status} {testName}: {details}", color);
            
            if (!passed && issues != null && issues.Count > 0)
            {
                foreach (var issue in issues.Take(5))
                {
                    Log($"       - {issue}", ConsoleColor.DarkGray);
                }
                if (issues.Count > 5)
                    Log($"       ... and {issues.Count - 5} more", ConsoleColor.DarkGray);
            }
        }
        
        private static void PrintSummary()
        {
            int passed = _results.Count(r => r.Passed && !r.IsWarning);
            int warnings = _results.Count(r => r.Passed && r.IsWarning);
            int failed = _results.Count(r => !r.Passed);
            int total = _results.Count;
            
            Log("\n=== Test Summary ===", ConsoleColor.Cyan);
            Log($"Total Tests: {total}");
            Log($"Passed: {passed}", ConsoleColor.Green);
            if (warnings > 0)
                Log($"Warnings: {warnings}", ConsoleColor.Yellow);
            if (failed > 0)
                Log($"Failed: {failed}", ConsoleColor.Red);
            Log($"Time Elapsed: {_stopwatch.ElapsedMilliseconds}ms");
            
            if (failed == 0)
                Log("\n✓ All critical tests passed!", ConsoleColor.Green);
            else
                Log($"\n✗ {failed} test(s) failed - review issues above", ConsoleColor.Red);
        }
        
        private static void Log(string message, ConsoleColor color = ConsoleColor.White)
        {
            // For in-game display
            if (Main.netMode != Terraria.ID.NetmodeID.Server && Main.LocalPlayer != null)
            {
                var xnaColor = color switch
                {
                    ConsoleColor.Green => Microsoft.Xna.Framework.Color.LightGreen,
                    ConsoleColor.Red => Microsoft.Xna.Framework.Color.OrangeRed,
                    ConsoleColor.Yellow => Microsoft.Xna.Framework.Color.Yellow,
                    ConsoleColor.Cyan => Microsoft.Xna.Framework.Color.Cyan,
                    ConsoleColor.DarkGray => Microsoft.Xna.Framework.Color.Gray,
                    _ => Microsoft.Xna.Framework.Color.White
                };
                Main.NewText(message, xnaColor);
            }
            
            // Also log to mod logger
            Mod mod = ModContent.GetInstance<Rpg>();
            mod?.Logger.Info(message);
        }
        
        private class TestResult
        {
            public string Name { get; set; }
            public bool Passed { get; set; }
            public string Details { get; set; }
            public List<string> Issues { get; set; }
            public bool IsWarning { get; set; }
        }
        
        #endregion
    }
}
