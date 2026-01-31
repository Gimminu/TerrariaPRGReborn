using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using RpgMod.Common.Base;
using RpgMod.Common;
using Terraria.ModLoader;
using Rpg;

namespace RpgMod.Common.Skills
{
    /// <summary>
    /// Central registry for all skills in the mod
    /// </summary>
    public static class SkillDatabase
    {
        // Registry: skill internal name -> factory
        private static Dictionary<string, System.Func<BaseSkill>> skillRegistry = new Dictionary<string, System.Func<BaseSkill>>();
        private static Dictionary<JobType, List<string>> jobSkillCache = new Dictionary<JobType, List<string>>();
        
        /// <summary>
        /// Initialize and register all skills
        /// </summary>
        public static void Initialize()
        {
            skillRegistry.Clear();
            jobSkillCache.Clear();
            RegisterCustomSkillClasses();

            // ============================================
            // Register remaining skills from SkillDefinitions (GenericSkill fallback)
            // These are skills that don't have custom class implementations yet
            // ============================================
            foreach (var definition in SkillDefinitions.All)
            {
                // Only register if not already registered with a custom class
                if (!skillRegistry.ContainsKey(definition.InternalName))
                {
                    RegisterSkill(() => new GenericSkill(definition));
                }
            }
        }
        
        /// <summary>
        /// Register a skill to the database
        /// </summary>
        private static void RegisterSkill(System.Func<BaseSkill> factory)
        {
            RegisterSkill(factory, null);
        }

        private static void RegisterSkill(System.Func<BaseSkill> factory, Type skillType)
        {
            if (factory == null)
                return;

            BaseSkill skill;
            try
            {
                skill = factory();
            }
            catch (Exception ex)
            {
                LogWarning($"Failed to construct skill instance{FormatSkillType(skillType)}.", ex);
                return;
            }

            if (skill == null || string.IsNullOrWhiteSpace(skill.InternalName))
            {
                LogWarning($"Skipping skill with missing InternalName{FormatSkillType(skillType)}.");
                return;
            }

            if (!skillRegistry.ContainsKey(skill.InternalName))
                skillRegistry.Add(skill.InternalName, factory);
        }

        /// <summary>
        /// Get all registered skill names for a specific job without relying on JobDatabase
        /// </summary>
        public static List<string> GetRegisteredSkillNamesForJob(JobType jobType)
        {
            if (!jobSkillCache.TryGetValue(jobType, out var cached))
            {
                var names = new List<string>();

                foreach (var factory in skillRegistry.Values)
                {
                    BaseSkill skill;
                    try
                    {
                        skill = factory();
                    }
                    catch (Exception ex)
                    {
                        LogWarning("Failed to construct skill while building job cache.", ex);
                        continue;
                    }

                    if (skill == null || string.IsNullOrWhiteSpace(skill.InternalName))
                        continue;

                    if (skill.RequiredJob == jobType)
                        names.Add(skill.InternalName);
                }

                cached = names
                    .Distinct(StringComparer.Ordinal)
                    .OrderBy(name => name, StringComparer.Ordinal)
                    .ToList();

                jobSkillCache[jobType] = cached;
            }

            return new List<string>(cached);
        }

        private static void RegisterCustomSkillClasses()
        {
            Type baseSkillType = typeof(BaseSkill);
            IEnumerable<Type> skillTypes;

            try
            {
                skillTypes = baseSkillType.Assembly.GetTypes();
            }
            catch (ReflectionTypeLoadException ex)
            {
                skillTypes = ex.Types.Where(t => t != null);
                LogWarning("Some skill types failed to load and will be skipped.", ex);
            }

            skillTypes = skillTypes
                .Where(t => baseSkillType.IsAssignableFrom(t)
                            && !t.IsAbstract
                            && t != typeof(GenericSkill)
                            && t.GetConstructor(Type.EmptyTypes) != null)
                .OrderBy(t => t.FullName ?? t.Name);

            foreach (var skillType in skillTypes)
            {
                RegisterSkill(() => (BaseSkill)Activator.CreateInstance(skillType), skillType);
            }
        }
        
        /// <summary>
        /// Get skill by internal name (creates new instance)
        /// </summary>
        public static BaseSkill GetSkill(string internalName)
        {
            if (string.IsNullOrWhiteSpace(internalName))
                return null;

            if (!skillRegistry.TryGetValue(internalName, out var factory))
                return null;

            try
            {
                return factory();
            }
            catch (Exception ex)
            {
                LogWarning($"Failed to construct skill '{internalName}'.", ex);
                return null;
            }
        }
        
        /// <summary>
        /// Get all skills for a specific job
        /// </summary>
        public static List<BaseSkill> GetSkillsForJob(JobType jobType)
        {
            List<BaseSkill> skills = new List<BaseSkill>();
            var added = new HashSet<string>();

            var lineage = Jobs.JobDatabase.GetJobLineage(jobType);
            if (lineage.Count == 0)
                return skills;

            foreach (JobType lineageJob in lineage)
            {
                var jobData = Jobs.JobDatabase.GetJobData(lineageJob);
                if (jobData?.SkillUnlocks == null)
                    continue;

                foreach (string skillName in jobData.SkillUnlocks)
                {
                    if (!added.Add(skillName))
                        continue;

                    BaseSkill skill = GetSkill(skillName);
                    if (skill != null)
                        skills.Add(skill);
                }
            }

            return skills;
        }

        /// <summary>
        /// Get all registered skills (new instances).
        /// </summary>
        public static List<BaseSkill> GetAllSkills()
        {
            var skills = new List<BaseSkill>();
            foreach (var entry in skillRegistry)
            {
                BaseSkill skill;
                try
                {
                    skill = entry.Value();
                }
                catch (Exception ex)
                {
                    LogWarning($"Failed to construct skill '{entry.Key}'.", ex);
                    continue;
                }

                if (skill != null)
                    skills.Add(skill);
            }
            return skills;
        }

        /// <summary>
        /// Get total skill point cost to max all skills up to a tier for the job lineage.
        /// </summary>
        public static int GetTotalSkillPointCostForLineage(JobType jobType, JobTier maxTier, int maxLevel)
        {
            var lineage = Jobs.JobDatabase.GetJobLineage(jobType);
            var allowedJobs = new HashSet<JobType>();

            foreach (var job in lineage)
            {
                if (RpgFormulas.GetJobTier(job) <= maxTier)
                    allowedJobs.Add(job);
            }

            int total = 0;
            foreach (var entry in skillRegistry)
            {
                BaseSkill skill;
                try
                {
                    skill = entry.Value();
                }
                catch (Exception ex)
                {
                    LogWarning($"Failed to construct skill '{entry.Key}' for cost calculation.", ex);
                    continue;
                }

                if (skill == null)
                    continue;

                JobType requiredJob = skill.RequiredJob;
                JobTier skillTier = requiredJob == JobType.None ? JobTier.Novice : RpgFormulas.GetJobTier(requiredJob);
                if (skillTier > maxTier)
                    continue;

                if (requiredJob != JobType.None && !allowedJobs.Contains(requiredJob))
                    continue;

                if (maxLevel > 0 && skill.RequiredLevel > maxLevel)
                    continue;

                int cost = Math.Max(1, skill.SkillPointCost);
                int ranks = Math.Max(1, skill.MaxRank);
                total += cost * ranks;
            }

            return total;
        }
        
        /// <summary>
        /// Get all registered skill names
        /// </summary>
        public static List<string> GetAllSkillNames()
        {
            return new List<string>(skillRegistry.Keys);
        }
        
        /// <summary>
        /// Check if skill exists
        /// </summary>
        public static bool SkillExists(string internalName)
        {
            if (string.IsNullOrWhiteSpace(internalName))
                return false;

            return skillRegistry.ContainsKey(internalName);
        }

        private static void LogWarning(string message, Exception ex = null)
        {
            var logger = ModContent.GetInstance<global::Rpg.Rpg>()?.Logger;
            if (logger == null)
                return;

            if (ex != null)
                logger.Warn($"{message} {ex.GetType().Name}: {ex.Message}");
            else
                logger.Warn(message);
        }

        private static string FormatSkillType(Type skillType)
        {
            return skillType == null ? string.Empty : $" ({skillType.FullName ?? skillType.Name})";
        }
    }
}
