using System.Collections.Generic;
using Terraria;
using Rpg.Common;
using Rpg.Common.Players;
using Rpg.Common.Skills;

namespace Rpg.Common.Jobs
{
    /// <summary>
    /// Complete database of all jobs in the RPG system
    /// Defines stat bonuses, requirements, and skill unlocks for each job
    /// </summary>
    public static class JobDatabase
    {
        // Performance: Cache JobData to avoid creating new objects every frame
        private static readonly Dictionary<JobType, JobData> _jobCache = new();
        
        #region Job Definitions
        
        /// <summary>
        /// Get complete job data for a specific job type (cached)
        /// </summary>
        public static JobData GetJobData(JobType jobType)
        {
            // Return cached version if available
            if (_jobCache.TryGetValue(jobType, out var cached))
                return cached;
            
            // Create and cache the JobData
            var data = CreateJobData(jobType);
            _jobCache[jobType] = data;
            return data;
        }
        
        /// <summary>
        /// Internal: Create JobData (called once per job type)
        /// </summary>
        private static JobData CreateJobData(JobType jobType)
        {
            return jobType switch
            {
                // Novice
                JobType.Novice => new JobData
                {
                    Type = JobType.Novice,
                    Tier = JobTier.Novice,
                    DisplayName = "Novice",
                    Description = "A beginner with no specialization. Balanced stat growth.",
                    Requirements = new JobRequirements
                    {
                        MinLevel = 1,
                        RequiredJob = JobType.None,
                        RequiredBossKills = new List<int>() // No requirements
                    },
                    StatBonuses = new Dictionary<StatType, int>(), // No bonuses
                    SkillUnlocks = GetSkillUnlocks(JobType.Novice)
                  },
                
                // === FIRST CLASS ===
                
                JobType.Warrior => new JobData
                {
                    Type = JobType.Warrior,
                    Tier = JobTier.Tier1,
                    DisplayName = "Warrior",
                    Description = "A melee combatant specializing in Strength and Vitality. High HP and physical damage.",
                    Requirements = CreateTier1Requirements(),
                    StatBonuses = new Dictionary<StatType, int>
                    {
                        { StatType.Strength, 5 },
                        { StatType.Vitality, 10 },
                        { StatType.Defense, 3 }
                    },
                    SkillUnlocks = GetSkillUnlocks(JobType.Warrior)
                },
                
                JobType.Ranger => new JobData
                {
                    Type = JobType.Ranger,
                    Tier = JobTier.Tier1,
                    DisplayName = "Ranger",
                    Description = "A ranged specialist focusing on Dexterity and Agility. High accuracy and mobility.",
                    Requirements = CreateTier1Requirements(),
                    StatBonuses = new Dictionary<StatType, int>
                    {
                        { StatType.Dexterity, 8 },
                        { StatType.Agility, 5 },
                        { StatType.Rogue, 3 }
                    },
                    SkillUnlocks = GetSkillUnlocks(JobType.Ranger)
                },
                
                JobType.Mage => new JobData
                {
                    Type = JobType.Mage,
                    Tier = JobTier.Tier1,
                    DisplayName = "Mage",
                    Description = "A magic user specializing in Intelligence and Wisdom. High mana and spell power.",
                    Requirements = CreateTier1Requirements(),
                    StatBonuses = new Dictionary<StatType, int>
                    {
                        { StatType.Intelligence, 15 },
                        { StatType.Wisdom, 8 }
                    },
                    SkillUnlocks = GetSkillUnlocks(JobType.Mage)
                },
                
                JobType.Summoner => new JobData
                {
                    Type = JobType.Summoner,
                    Tier = JobTier.Tier1,
                    DisplayName = "Summoner",
                    Description = "A minion master focusing on Focus and Wisdom. Command powerful allies.",
                    Requirements = CreateTier1Requirements(),
                    StatBonuses = new Dictionary<StatType, int>
                    {
                        { StatType.Focus, 15 },
                        { StatType.Wisdom, 5 }
                    },
                    SkillUnlocks = GetSkillUnlocks(JobType.Summoner)
                },
                
                // === SECOND CLASS: WARRIOR ===
                
                JobType.Knight => new JobData
                {
                    Type = JobType.Knight,
                    Tier = JobTier.Tier2,
                    DisplayName = "Knight",
                    Description = "An armored defender. Exceptional defense and HP, protecting allies.",
                    Requirements = CreateTier2Requirements(JobType.Warrior),
                    StatBonuses = new Dictionary<StatType, int>
                    {
                        { StatType.Strength, 8 },
                        { StatType.Vitality, 20 },
                        { StatType.Defense, 10 },
                        { StatType.Fortitude, 8 }
                    },
                    SkillUnlocks = GetSkillUnlocks(JobType.Knight)
                },
                
                JobType.Berserker => new JobData
                {
                    Type = JobType.Berserker,
                    Tier = JobTier.Tier2,
                    DisplayName = "Berserker",
                    Description = "A furious warrior who grows stronger as HP decreases. High risk, high reward.",
                    Requirements = CreateTier2Requirements(JobType.Warrior),
                    StatBonuses = new Dictionary<StatType, int>
                    {
                        { StatType.Strength, 15 },
                        { StatType.Vitality, 10 },
                        { StatType.Agility, 5 }
                    },
                    SkillUnlocks = GetSkillUnlocks(JobType.Berserker)
                },
                
                JobType.Paladin => new JobData
                {
                    Type = JobType.Paladin,
                    Tier = JobTier.Tier2,
                    DisplayName = "Paladin",
                    Description = "A holy warrior combining melee prowess with healing magic.",
                    Requirements = CreateTier2Requirements(JobType.Warrior),
                    StatBonuses = new Dictionary<StatType, int>
                    {
                        { StatType.Strength, 10 },
                        { StatType.Vitality, 15 },
                        { StatType.Wisdom, 8 },
                        { StatType.Fortitude, 5 }
                    },
                    SkillUnlocks = GetSkillUnlocks(JobType.Paladin)
                },
                
                JobType.DeathKnight => new JobData
                {
                    Type = JobType.DeathKnight,
                    Tier = JobTier.Tier2,
                    DisplayName = "Death Knight",
                    Description = "A cursed warrior wielding dark magic. Drains life from enemies.",
                    Requirements = CreateTier2Requirements(JobType.Warrior),
                    StatBonuses = new Dictionary<StatType, int>
                    {
                        { StatType.Strength, 12 },
                        { StatType.Intelligence, 8 },
                        { StatType.Vitality, 12 }
                    },
                    SkillUnlocks = GetSkillUnlocks(JobType.DeathKnight)
                },
                
                // === SECOND CLASS: RANGER ===
                
                JobType.Sniper => new JobData
                {
                    Type = JobType.Sniper,
                    Tier = JobTier.Tier2,
                    DisplayName = "Sniper",
                    Description = "A precision marksman with devastating critical hits.",
                    Requirements = CreateTier2Requirements(JobType.Ranger),
                    StatBonuses = new Dictionary<StatType, int>
                    {
                        { StatType.Dexterity, 20 },
                        { StatType.Rogue, 10 }
                    },
                    SkillUnlocks = GetSkillUnlocks(JobType.Sniper)
                },
                
                JobType.Assassin => new JobData
                {
                    Type = JobType.Assassin,
                    Tier = JobTier.Tier2,
                    DisplayName = "Assassin",
                    Description = "A stealthy killer who strikes from shadows with poison and criticals.",
                    Requirements = CreateTier2Requirements(JobType.Ranger),
                    StatBonuses = new Dictionary<StatType, int>
                    {
                        { StatType.Rogue, 15 },
                        { StatType.Dexterity, 10 },
                        { StatType.Agility, 8 }
                    },
                    SkillUnlocks = GetSkillUnlocks(JobType.Assassin)
                },
                
                JobType.Gunslinger => new JobData
                {
                    Type = JobType.Gunslinger,
                    Tier = JobTier.Tier2,
                    DisplayName = "Gunslinger",
                    Description = "A gun specialist with rapid fire and explosive rounds.",
                    Requirements = CreateTier2Requirements(JobType.Ranger),
                    StatBonuses = new Dictionary<StatType, int>
                    {
                        { StatType.Dexterity, 12 },
                        { StatType.Rogue, 8 },
                        { StatType.Luck, 5 }
                    },
                    SkillUnlocks = GetSkillUnlocks(JobType.Gunslinger)
                },
                
                // === SECOND CLASS: MAGE ===
                
                JobType.Sorcerer => new JobData
                {
                    Type = JobType.Sorcerer,
                    Tier = JobTier.Tier2,
                    DisplayName = "Sorcerer",
                    Description = "An elemental spellcaster who wields raw arcane power.",
                    Requirements = CreateTier2Requirements(JobType.Mage),
                    StatBonuses = new Dictionary<StatType, int>
                    {
                        { StatType.Intelligence, 30 },
                        { StatType.Wisdom, 8 }
                    },
                    SkillUnlocks = GetSkillUnlocks(JobType.Sorcerer)
                },
                
                JobType.Cleric => new JobData
                {
                    Type = JobType.Cleric,
                    Tier = JobTier.Tier2,
                    DisplayName = "Cleric",
                    Description = "A support caster who heals allies and shields the party.",
                    Requirements = CreateTier2Requirements(JobType.Mage),
                    StatBonuses = new Dictionary<StatType, int>
                    {
                        { StatType.Intelligence, 10 },
                        { StatType.Wisdom, 18 },
                        { StatType.Vitality, 10 },
                        { StatType.Fortitude, 6 }
                    },
                    SkillUnlocks = GetSkillUnlocks(JobType.Cleric)
                },
                
                JobType.Archmage => new JobData
                {
                    Type = JobType.Archmage,
                    Tier = JobTier.Tier2,
                    DisplayName = "Archmage",
                    Description = "A master of arcane magic with devastating spell power.",
                    Requirements = CreateTier2Requirements(JobType.Mage),
                    StatBonuses = new Dictionary<StatType, int>
                    {
                        { StatType.Intelligence, 30 },
                        { StatType.Wisdom, 10 }
                    },
                    SkillUnlocks = GetSkillUnlocks(JobType.Archmage)
                },
                
                JobType.Warlock => new JobData
                {
                    Type = JobType.Warlock,
                    Tier = JobTier.Tier2,
                    DisplayName = "Warlock",
                    Description = "A dark mage using curses and damage-over-time spells.",
                    Requirements = CreateTier2Requirements(JobType.Mage),
                    StatBonuses = new Dictionary<StatType, int>
                    {
                        { StatType.Intelligence, 15 },
                        { StatType.Wisdom, 12 },
                        { StatType.Vitality, 5 }
                    },
                    SkillUnlocks = GetSkillUnlocks(JobType.Warlock)
                },
                
                JobType.Spellblade => new JobData
                {
                    Type = JobType.Spellblade,
                    Tier = JobTier.Tier2,
                    DisplayName = "Spellblade",
                    Description = "A hybrid warrior-mage who infuses weapons with magic.",
                    Requirements = CreateTier2Requirements(JobType.Mage),
                    StatBonuses = new Dictionary<StatType, int>
                    {
                        { StatType.Intelligence, 20 },
                        { StatType.Strength, 10 }
                    },
                    SkillUnlocks = GetSkillUnlocks(JobType.Spellblade)
                },
                
                JobType.BattleMage => new JobData
                {
                    Type = JobType.BattleMage,
                    Tier = JobTier.Tier2,
                    DisplayName = "Battle Mage",
                    Description = "A tanky mage who fights on the front lines with magic armor.",
                    Requirements = CreateTier2Requirements(JobType.Mage),
                    StatBonuses = new Dictionary<StatType, int>
                    {
                        { StatType.Intelligence, 12 },
                        { StatType.Vitality, 10 },
                        { StatType.Defense, 8 }
                    },
                    SkillUnlocks = GetSkillUnlocks(JobType.BattleMage)
                },
                
                // === SECOND CLASS: SUMMONER ===
                
                JobType.Beastmaster => new JobData
                {
                    Type = JobType.Beastmaster,
                    Tier = JobTier.Tier2,
                    DisplayName = "Beastmaster",
                    Description = "A primal summoner who commands beasts and strengthens companions.",
                    Requirements = CreateTier2Requirements(JobType.Summoner),
                    StatBonuses = new Dictionary<StatType, int>
                    {
                        { StatType.Focus, 6 },
                        { StatType.Wisdom, 6 },
                        { StatType.Vitality, 10 },
                        { StatType.Agility, 8 },
                        { StatType.Luck, 5 }
                    },
                    SkillUnlocks = GetSkillUnlocks(JobType.Beastmaster)
                },
                
                JobType.Necromancer => new JobData
                {
                    Type = JobType.Necromancer,
                    Tier = JobTier.Tier2,
                    DisplayName = "Necromancer",
                    Description = "A dark summoner who raises undead minions from fallen foes.",
                    Requirements = CreateTier2Requirements(JobType.Summoner),
                    StatBonuses = new Dictionary<StatType, int>
                    {
                        { StatType.Focus, 8 },
                        { StatType.Wisdom, 7 },
                        { StatType.Intelligence, 10 },
                        { StatType.Fortitude, 5 }
                    },
                    SkillUnlocks = GetSkillUnlocks(JobType.Necromancer)
                },
                
                JobType.Druid => new JobData
                {
                    Type = JobType.Druid,
                    Tier = JobTier.Tier2,
                    DisplayName = "Druid",
                    Description = "A nature mage who summons beasts and controls the elements.",
                    Requirements = CreateTier2Requirements(JobType.Summoner),
                    StatBonuses = new Dictionary<StatType, int>
                    {
                        { StatType.Focus, 8 },
                        { StatType.Wisdom, 7 },
                        { StatType.Vitality, 10 },
                        { StatType.Intelligence, 5 }
                    },
                    SkillUnlocks = GetSkillUnlocks(JobType.Druid)
                },
                
                // === HYBRID ===
                
                JobType.Shadow => new JobData
                {
                    Type = JobType.Shadow,
                    Tier = JobTier.Tier2,
                    DisplayName = "Shadow",
                    Description = "A stealthy hybrid mixing ranged precision with close-range strikes.",
                    Requirements = CreateTier2Requirements(JobType.Ranger),
                    StatBonuses = new Dictionary<StatType, int>
                    {
                        { StatType.Dexterity, 10 },
                        { StatType.Strength, 8 },
                        { StatType.Rogue, 12 },
                        { StatType.Agility, 8 }
                    },
                    SkillUnlocks = GetSkillUnlocks(JobType.Shadow)
                },
                
                JobType.Spellthief => new JobData
                {
                    Type = JobType.Spellthief,
                    Tier = JobTier.Tier2,
                    DisplayName = "Spellthief",
                    Description = "A rogue-mage hybrid who steals buffs and deals magical criticals.",
                    Requirements = CreateTier2Requirements(JobType.Ranger),
                    StatBonuses = new Dictionary<StatType, int>
                    {
                        { StatType.Intelligence, 10 },
                        { StatType.Rogue, 10 },
                        { StatType.Agility, 8 }
                    },
                    SkillUnlocks = GetSkillUnlocks(JobType.Spellthief)
                },

                // === THIRD CLASS ===

                JobType.Guardian => new JobData
                {
                    Type = JobType.Guardian,
                    Tier = JobTier.Tier3,
                    DisplayName = "Guardian",
                    Description = "An ultimate defender with overwhelming protection and resilience.",
                    Requirements = new JobRequirements
                    {
                        MinLevel = RpgConstants.THIRD_JOB_LEVEL,
                        RequiredJob = JobType.Knight,
                        RequiredBossKills = new List<int> { Terraria.ID.NPCID.MoonLordCore }
                    },
                    StatBonuses = new Dictionary<StatType, int>
                    {
                        { StatType.Strength, 15 },
                        { StatType.Vitality, 30 },
                        { StatType.Defense, 20 },
                        { StatType.Fortitude, 12 }
                    },
                    SkillUnlocks = GetSkillUnlocks(JobType.Guardian)
                },

                JobType.BloodKnight => new JobData
                {
                    Type = JobType.BloodKnight,
                    Tier = JobTier.Tier3,
                    DisplayName = "Blood Knight",
                    Description = "A berserker who thrives on lifesteal and relentless assault.",
                    Requirements = new JobRequirements
                    {
                        MinLevel = RpgConstants.THIRD_JOB_LEVEL,
                        RequiredJob = JobType.Berserker,
                        RequiredBossKills = new List<int> { Terraria.ID.NPCID.MoonLordCore }
                    },
                    StatBonuses = new Dictionary<StatType, int>
                    {
                        { StatType.Strength, 20 },
                        { StatType.Vitality, 18 },
                        { StatType.Rogue, 10 },
                        { StatType.Fortitude, 8 }
                    },
                    SkillUnlocks = GetSkillUnlocks(JobType.BloodKnight)
                },

                JobType.Deadeye => new JobData
                {
                    Type = JobType.Deadeye,
                    Tier = JobTier.Tier3,
                    DisplayName = "Deadeye",
                    Description = "A master marksman who never misses and punishes weak points.",
                    Requirements = new JobRequirements
                    {
                        MinLevel = RpgConstants.THIRD_JOB_LEVEL,
                        RequiredJob = JobType.Sniper,
                        RequiredBossKills = new List<int> { Terraria.ID.NPCID.MoonLordCore }
                    },
                    StatBonuses = new Dictionary<StatType, int>
                    {
                        { StatType.Dexterity, 30 },
                        { StatType.Rogue, 15 }
                    },
                    SkillUnlocks = GetSkillUnlocks(JobType.Deadeye)
                },

                JobType.Gunmaster => new JobData
                {
                    Type = JobType.Gunmaster,
                    Tier = JobTier.Tier3,
                    DisplayName = "Gunmaster",
                    Description = "A gun virtuoso with rapid volleys and perfect timing.",
                    Requirements = new JobRequirements
                    {
                        MinLevel = RpgConstants.THIRD_JOB_LEVEL,
                        RequiredJob = JobType.Gunslinger,
                        RequiredBossKills = new List<int> { Terraria.ID.NPCID.MoonLordCore }
                    },
                    StatBonuses = new Dictionary<StatType, int>
                    {
                        { StatType.Dexterity, 18 },
                        { StatType.Rogue, 12 },
                        { StatType.Agility, 12 },
                        { StatType.Luck, 8 }
                    },
                    SkillUnlocks = GetSkillUnlocks(JobType.Gunmaster)
                },

                JobType.Archbishop => new JobData
                {
                    Type = JobType.Archbishop,
                    Tier = JobTier.Tier3,
                    DisplayName = "Archbishop",
                    Description = "A supreme healer who empowers allies with holy blessings.",
                    Requirements = new JobRequirements
                    {
                        MinLevel = RpgConstants.THIRD_JOB_LEVEL,
                        RequiredJob = JobType.Cleric,
                        RequiredBossKills = new List<int> { Terraria.ID.NPCID.MoonLordCore }
                    },
                    StatBonuses = new Dictionary<StatType, int>
                    {
                        { StatType.Intelligence, 18 },
                        { StatType.Wisdom, 25 },
                        { StatType.Vitality, 15 },
                        { StatType.Fortitude, 12 }
                    },
                    SkillUnlocks = GetSkillUnlocks(JobType.Archbishop)
                },

                JobType.Overlord => new JobData
                {
                    Type = JobType.Overlord,
                    Tier = JobTier.Tier3,
                    DisplayName = "Overlord",
                    Description = "A beast commander who overwhelms foes with summoned forces.",
                    Requirements = new JobRequirements
                    {
                        MinLevel = RpgConstants.THIRD_JOB_LEVEL,
                        RequiredJob = JobType.Beastmaster,
                        RequiredBossKills = new List<int> { Terraria.ID.NPCID.MoonLordCore }
                    },
                    StatBonuses = new Dictionary<StatType, int>
                    {
                        { StatType.Focus, 11 },
                        { StatType.Wisdom, 11 },
                        { StatType.Vitality, 18 },
                        { StatType.Agility, 12 },
                        { StatType.Luck, 10 }
                    },
                    SkillUnlocks = GetSkillUnlocks(JobType.Overlord)
                },

                JobType.LichKing => new JobData
                {
                    Type = JobType.LichKing,
                    Tier = JobTier.Tier3,
                    DisplayName = "Lich King",
                    Description = "An undead sovereign who commands legions and drains souls.",
                    Requirements = new JobRequirements
                    {
                        MinLevel = RpgConstants.THIRD_JOB_LEVEL,
                        RequiredJob = JobType.Necromancer,
                        RequiredBossKills = new List<int> { Terraria.ID.NPCID.MoonLordCore }
                    },
                    StatBonuses = new Dictionary<StatType, int>
                    {
                        { StatType.Focus, 11 },
                        { StatType.Wisdom, 11 },
                        { StatType.Intelligence, 15 },
                        { StatType.Fortitude, 12 },
                        { StatType.Vitality, 12 }
                    },
                    SkillUnlocks = GetSkillUnlocks(JobType.LichKing)
                },
                
                _ => GetDefaultJobData()
            };
        }
        
        /// <summary>
        /// Default/fallback job data
        /// </summary>
        private static JobData GetDefaultJobData()
        {
            return new JobData
            {
                Type = JobType.Novice,
                Tier = JobTier.Novice,
                DisplayName = "Unknown",
                Description = "Unknown job",
                Requirements = new JobRequirements
                {
                    MinLevel = 1,
                    RequiredJob = JobType.None,
                    RequiredBossKills = new List<int>()
                },
                StatBonuses = new Dictionary<StatType, int>(),
                SkillUnlocks = new List<string>()
            };
        }

        private static List<string> GetSkillUnlocks(JobType jobType)
        {
            return SkillDatabase.GetRegisteredSkillNamesForJob(jobType);
        }
        
        #endregion
        
        #region Helper Methods
        
        /// <summary>
        /// Check if player meets job requirements
        /// </summary>
        public static bool CanUnlockJob(RpgPlayer rpgPlayer, JobType targetJob)
        {
            JobData jobData = GetJobData(targetJob);
            if (jobData == null || jobData.Requirements == null)
                return false;

            if (jobData.Requirements.MinLevel > 0 && rpgPlayer.Level < jobData.Requirements.MinLevel)
                return false;
            
            // Previous job check
            if (jobData.Requirements.RequiredJob != JobType.None &&
                rpgPlayer.CurrentJob != jobData.Requirements.RequiredJob)
            {
                return false;
            }
            
            // Hardmode requirement
            if (jobData.Requirements.RequiresHardmode && !Main.hardMode)
                return false;

            // Specific boss requirements (if any)
            if (jobData.Requirements.RequiredBossKills != null && jobData.Requirements.RequiredBossKills.Count > 0)
            {
                if (jobData.Requirements.BossRequirementMode == BossRequirementMode.Any)
                {
                    bool anyDefeated = false;
                    foreach (int bossId in jobData.Requirements.RequiredBossKills)
                    {
                        if (IsBossDefeated(bossId))
                        {
                            anyDefeated = true;
                            break;
                        }
                    }

                    if (!anyDefeated)
                        return false;
                }
                else
                {
                    foreach (int bossId in jobData.Requirements.RequiredBossKills)
                    {
                        if (!IsBossDefeated(bossId))
                            return false;
                    }
                }
            }
            
            return true;
        }
        
        /// <summary>
        /// Get all jobs available to player
        /// </summary>
        public static List<JobType> GetAvailableJobs(RpgPlayer rpgPlayer)
        {
            List<JobType> available = new List<JobType>();
            
            foreach (JobType job in System.Enum.GetValues(typeof(JobType)))
            {
                if (job == JobType.None)
                    continue;
                    
                if (CanUnlockJob(rpgPlayer, job))
                {
                    available.Add(job);
                }
            }
            
            return available;
        }

        /// <summary>
        /// Get all defined jobs (excluding None)
        /// </summary>
        public static List<JobType> GetAllJobs()
        {
            List<JobType> jobs = new List<JobType>();
            foreach (JobType job in System.Enum.GetValues(typeof(JobType)))
            {
                if (job == JobType.None)
                    continue;

                jobs.Add(job);
            }
            return jobs;
        }

        public static bool IsJobInLineage(JobType currentJob, JobType requiredJob)
        {
            if (requiredJob == JobType.None)
                return true;

            if (currentJob == requiredJob)
                return true;

            var visited = new HashSet<JobType>();
            JobType job = currentJob;

            while (job != JobType.None && visited.Add(job))
            {
                var data = GetJobData(job);
                if (data?.Requirements == null)
                    break;

                job = data.Requirements.RequiredJob;
                if (job == requiredJob)
                    return true;
            }

            return false;
        }

        public static List<JobType> GetJobLineage(JobType jobType)
        {
            var lineage = new List<JobType>();
            var visited = new HashSet<JobType>();
            JobType current = jobType;

            while (current != JobType.None && visited.Add(current))
            {
                lineage.Add(current);
                var data = GetJobData(current);
                if (data?.Requirements == null)
                    break;

                current = data.Requirements.RequiredJob;
            }

            lineage.Reverse();
            return lineage;
        }
        
        private static JobRequirements CreateTier1Requirements()
        {
            return new JobRequirements
            {
                MinLevel = RpgConstants.FIRST_JOB_LEVEL,
                RequiredJob = JobType.Novice,
                BossRequirementMode = BossRequirementMode.Any,
                RequiredBossKills = new List<int>
                {
                    Terraria.ID.NPCID.KingSlime,
                    Terraria.ID.NPCID.EyeofCthulhu,
                    Terraria.ID.NPCID.EaterofWorldsHead,
                    Terraria.ID.NPCID.BrainofCthulhu
                }
            };
        }

        private static JobRequirements CreateTier2Requirements(JobType requiredJob)
        {
            return new JobRequirements
            {
                MinLevel = RpgConstants.SECOND_JOB_LEVEL,
                RequiredJob = requiredJob,
                RequiresHardmode = true,
                BossRequirementMode = BossRequirementMode.Any,
                RequiredBossKills = new List<int>
                {
                    Terraria.ID.NPCID.TheDestroyer,
                    Terraria.ID.NPCID.Retinazer,
                    Terraria.ID.NPCID.SkeletronPrime
                }
            };
        }

        public static List<string> GetMissingRequirementDescriptions(RpgPlayer rpgPlayer, JobType targetJob)
        {
            var missing = new List<string>();
            if (rpgPlayer == null)
                return missing;

            JobData jobData = GetJobData(targetJob);
            if (jobData == null)
                return missing;

            if (jobData.Requirements.MinLevel > 0 && rpgPlayer.Level < jobData.Requirements.MinLevel)
            {
                missing.Add($"Requires Level {jobData.Requirements.MinLevel}");
            }

            if (jobData.Requirements.RequiredJob != JobType.None &&
                rpgPlayer.CurrentJob != jobData.Requirements.RequiredJob)
            {
                var requiredJobData = GetJobData(jobData.Requirements.RequiredJob);
                string jobName = requiredJobData?.DisplayName ?? jobData.Requirements.RequiredJob.ToString();
                missing.Add($"Requires {jobName}");
            }

            if (jobData.Requirements.RequiresHardmode && !Main.hardMode)
            {
                missing.Add("Requires Hardmode");
            }

            if (jobData.Requirements.RequiredBossKills != null && jobData.Requirements.RequiredBossKills.Count > 0)
            {
                if (jobData.Requirements.BossRequirementMode == BossRequirementMode.Any)
                {
                    bool anyDefeated = false;
                    List<string> bossNames = new List<string>();
                    foreach (int bossId in jobData.Requirements.RequiredBossKills)
                    {
                        bossNames.Add(GetBossDisplayName(bossId));
                        if (IsBossDefeated(bossId))
                            anyDefeated = true;
                    }

                    if (!anyDefeated)
                        missing.Add($"Defeat one: {string.Join(" / ", bossNames)}");
                }
                else
                {
                    List<string> missingBosses = new List<string>();
                    foreach (int bossId in jobData.Requirements.RequiredBossKills)
                    {
                        if (!IsBossDefeated(bossId))
                            missingBosses.Add(GetBossDisplayName(bossId));
                    }

                    if (missingBosses.Count > 0)
                        missing.Add($"Defeat: {string.Join(", ", missingBosses)}");
                }
            }

            return missing;
        }

        private static bool IsBossDefeated(int bossNpcId)
        {
            return bossNpcId switch
            {
                Terraria.ID.NPCID.KingSlime => Terraria.NPC.downedSlimeKing,
                Terraria.ID.NPCID.EyeofCthulhu => Terraria.NPC.downedBoss1,
                Terraria.ID.NPCID.EaterofWorldsHead or Terraria.ID.NPCID.EaterofWorldsBody
                    or Terraria.ID.NPCID.EaterofWorldsTail => Terraria.NPC.downedBoss2,
                Terraria.ID.NPCID.BrainofCthulhu => Terraria.NPC.downedBoss2,
                Terraria.ID.NPCID.QueenBee => Terraria.NPC.downedQueenBee,
                Terraria.ID.NPCID.SkeletronHead or Terraria.ID.NPCID.SkeletronHand
                    => Terraria.NPC.downedBoss3,
                Terraria.ID.NPCID.Deerclops => Terraria.NPC.downedDeerclops,
                Terraria.ID.NPCID.QueenSlimeBoss => Terraria.NPC.downedQueenSlime,
                Terraria.ID.NPCID.TheDestroyer or Terraria.ID.NPCID.TheDestroyerBody
                    or Terraria.ID.NPCID.TheDestroyerTail => Terraria.NPC.downedMechBoss1,
                Terraria.ID.NPCID.Retinazer or Terraria.ID.NPCID.Spazmatism
                    => Terraria.NPC.downedMechBoss2,
                Terraria.ID.NPCID.SkeletronPrime => Terraria.NPC.downedMechBoss3,
                Terraria.ID.NPCID.Plantera => Terraria.NPC.downedPlantBoss,
                Terraria.ID.NPCID.Golem => Terraria.NPC.downedGolemBoss,
                Terraria.ID.NPCID.DukeFishron => Terraria.NPC.downedFishron,
                Terraria.ID.NPCID.HallowBoss => Terraria.NPC.downedEmpressOfLight,
                Terraria.ID.NPCID.CultistBoss => Terraria.NPC.downedAncientCultist,
                Terraria.ID.NPCID.MoonLordCore or Terraria.ID.NPCID.MoonLordHead
                    or Terraria.ID.NPCID.MoonLordHand => Terraria.NPC.downedMoonlord,
                _ => false
            };
        }

        private static string GetBossDisplayName(int bossNpcId)
        {
            if (bossNpcId == Terraria.ID.NPCID.Retinazer || bossNpcId == Terraria.ID.NPCID.Spazmatism)
                return "The Twins";

            int displayId = bossNpcId switch
            {
                Terraria.ID.NPCID.EaterofWorldsBody or Terraria.ID.NPCID.EaterofWorldsTail
                    => Terraria.ID.NPCID.EaterofWorldsHead,
                Terraria.ID.NPCID.TheDestroyerBody or Terraria.ID.NPCID.TheDestroyerTail
                    => Terraria.ID.NPCID.TheDestroyer,
                Terraria.ID.NPCID.SkeletronHand => Terraria.ID.NPCID.SkeletronHead,
                Terraria.ID.NPCID.MoonLordHead or Terraria.ID.NPCID.MoonLordHand
                    => Terraria.ID.NPCID.MoonLordCore,
                _ => bossNpcId
            };

            return Lang.GetNPCNameValue(displayId);
        }
        
        #endregion
    }
    
    #region Data Structures
    
    /// <summary>
    /// Complete data for a job
    /// </summary>
    public class JobData
    {
        public JobType Type { get; set; }
        public JobTier Tier { get; set; }
        public string DisplayName { get; set; }
        public string Description { get; set; }
        public JobRequirements Requirements { get; set; }
        public Dictionary<StatType, int> StatBonuses { get; set; }
        public List<string> SkillUnlocks { get; set; }
    }

    public enum BossRequirementMode
    {
        All,
        Any
    }
    
    /// <summary>
    /// Requirements to unlock a job
    /// </summary>
    public class JobRequirements
    {
        public int MinLevel { get; set; }
        public JobType RequiredJob { get; set; }
        public bool RequiresHardmode { get; set; }
        public BossRequirementMode BossRequirementMode { get; set; } = BossRequirementMode.All;
        public List<int> RequiredBossKills { get; set; } // NPC IDs
    }
    
    #endregion
}
