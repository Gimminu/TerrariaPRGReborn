using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace Rpg.Common.Systems
{
    /// <summary>
    /// Achievement/Challenge System for RPG progression milestones
    /// Tracks various accomplishments and provides rewards
    /// </summary>
    public class AchievementSystem : ModPlayer
    {
        public HashSet<string> UnlockedAchievements { get; private set; } = new();
        
        // Achievement definitions
        public static readonly Dictionary<string, AchievementData> Achievements = new()
        {
            // Level Milestones
            ["level_10"] = new AchievementData("Novice Adventurer", "Reach Level 10", AchievementCategory.Leveling, 100),
            ["level_25"] = new AchievementData("Seasoned Warrior", "Reach Level 25", AchievementCategory.Leveling, 250),
            ["level_50"] = new AchievementData("Veteran Hero", "Reach Level 50", AchievementCategory.Leveling, 500),
            ["level_75"] = new AchievementData("Elite Champion", "Reach Level 75", AchievementCategory.Leveling, 750),
            ["level_100"] = new AchievementData("Legendary Master", "Reach Level 100", AchievementCategory.Leveling, 1500),
            ["level_max"] = new AchievementData("Transcendent", "Reach Maximum Level", AchievementCategory.Leveling, 5000),
            
            // Job Advancement
            ["first_job"] = new AchievementData("First Steps", "Complete your first job advancement", AchievementCategory.Jobs, 200),
            ["second_job"] = new AchievementData("Rising Power", "Complete second job advancement", AchievementCategory.Jobs, 500),
            ["third_job"] = new AchievementData("True Mastery", "Complete third job advancement", AchievementCategory.Jobs, 1000),
            
            // Combat Achievements
            ["first_kill"] = new AchievementData("First Blood", "Defeat your first enemy", AchievementCategory.Combat, 10),
            ["kill_100"] = new AchievementData("Slayer", "Defeat 100 enemies", AchievementCategory.Combat, 100),
            ["kill_1000"] = new AchievementData("Executioner", "Defeat 1000 enemies", AchievementCategory.Combat, 500),
            ["kill_10000"] = new AchievementData("Annihilator", "Defeat 10000 enemies", AchievementCategory.Combat, 2000),
            
            // Boss Achievements
            ["boss_eye"] = new AchievementData("Eye Opener", "Defeat the Eye of Cthulhu", AchievementCategory.Bosses, 300),
            ["boss_wof"] = new AchievementData("Hell Breaker", "Defeat the Wall of Flesh", AchievementCategory.Bosses, 500),
            ["boss_mech"] = new AchievementData("Machine Breaker", "Defeat all Mechanical Bosses", AchievementCategory.Bosses, 800),
            ["boss_plantera"] = new AchievementData("Jungle Conqueror", "Defeat Plantera", AchievementCategory.Bosses, 1000),
            ["boss_golem"] = new AchievementData("Temple Raider", "Defeat Golem", AchievementCategory.Bosses, 1000),
            ["boss_cultist"] = new AchievementData("Cult Breaker", "Defeat the Lunatic Cultist", AchievementCategory.Bosses, 1200),
            ["boss_moonlord"] = new AchievementData("Moon's End", "Defeat the Moon Lord", AchievementCategory.Bosses, 3000),
            
            // Skill Achievements
            ["skill_first"] = new AchievementData("Skill Apprentice", "Learn your first skill", AchievementCategory.Skills, 50),
            ["skill_5"] = new AchievementData("Skill Student", "Learn 5 skills", AchievementCategory.Skills, 200),
            ["skill_10"] = new AchievementData("Skill Expert", "Learn 10 skills", AchievementCategory.Skills, 500),
            ["skill_max"] = new AchievementData("Skill Master", "Max out a skill", AchievementCategory.Skills, 300),
            
            // Stat Achievements
            ["stat_50"] = new AchievementData("Stat Builder", "Allocate 50 stat points", AchievementCategory.Stats, 100),
            ["stat_100"] = new AchievementData("Stat Specialist", "Allocate 100 stat points", AchievementCategory.Stats, 300),
            ["stat_200"] = new AchievementData("Stat Master", "Allocate 200 stat points", AchievementCategory.Stats, 600),
            
            // Special Achievements
            ["survive_death"] = new AchievementData("Cheating Death", "Survive a killing blow (if implemented)", AchievementCategory.Special, 500),
            ["party_play"] = new AchievementData("Teamwork", "Gain XP while in a party", AchievementCategory.Special, 200),
            ["macro_master"] = new AchievementData("Combo King", "Create and use a skill macro", AchievementCategory.Special, 150),
        };
        
        // Tracking stats
        public int TotalKills { get; set; }
        public int TotalSkillsLearned { get; set; }
        public int TotalStatPointsAllocated { get; set; }
        public int TotalXPEarned { get; set; }
        
        public override void Initialize()
        {
            UnlockedAchievements = new HashSet<string>();
            TotalKills = 0;
            TotalSkillsLearned = 0;
            TotalStatPointsAllocated = 0;
            TotalXPEarned = 0;
        }
        
        public override void SaveData(TagCompound tag)
        {
            // Filter out null or empty achievement IDs
            var validAchievements = UnlockedAchievements?
                .Where(s => !string.IsNullOrEmpty(s))
                .ToList() ?? new List<string>();
                
            tag["achievements"] = validAchievements;
            tag["totalKills"] = TotalKills;
            tag["totalSkillsLearned"] = TotalSkillsLearned;
            tag["totalStatPointsAllocated"] = TotalStatPointsAllocated;
            tag["totalXPEarned"] = TotalXPEarned;
        }
        
        public override void LoadData(TagCompound tag)
        {
            if (tag.TryGet("achievements", out List<string> achievements))
            {
                UnlockedAchievements = new HashSet<string>(achievements);
            }
            
            TotalKills = tag.GetInt("totalKills");
            TotalSkillsLearned = tag.GetInt("totalSkillsLearned");
            TotalStatPointsAllocated = tag.GetInt("totalStatPointsAllocated");
            TotalXPEarned = tag.GetInt("totalXPEarned");
        }
        
        /// <summary>
        /// Try to unlock an achievement
        /// </summary>
        public bool TryUnlock(string achievementId)
        {
            if (UnlockedAchievements.Contains(achievementId))
                return false;
            
            if (!Achievements.TryGetValue(achievementId, out var achievement))
                return false;
            
            UnlockedAchievements.Add(achievementId);
            OnAchievementUnlocked(achievementId, achievement);
            return true;
        }
        
        /// <summary>
        /// Check if an achievement is unlocked
        /// </summary>
        public bool IsUnlocked(string achievementId)
        {
            return UnlockedAchievements.Contains(achievementId);
        }
        
        /// <summary>
        /// Called when an achievement is unlocked
        /// </summary>
        private void OnAchievementUnlocked(string id, AchievementData achievement)
        {
            // Show notification
            Main.NewText($"[Achievement Unlocked] {achievement.Name}: {achievement.Description}", 255, 215, 0);
            
            // Grant bonus XP reward
            if (achievement.XpReward > 0)
            {
                var playerLevel = Player.GetModPlayer<Players.PlayerLevel>();
                playerLevel.GainExperience(achievement.XpReward, XPSource.Quest, 0);
            }
            
            // Play sound
            Terraria.Audio.SoundEngine.PlaySound(Terraria.ID.SoundID.AchievementComplete);
        }
        
        /// <summary>
        /// Check level-based achievements
        /// </summary>
        public void CheckLevelAchievements(int level)
        {
            if (level >= 10) TryUnlock("level_10");
            if (level >= 25) TryUnlock("level_25");
            if (level >= 50) TryUnlock("level_50");
            if (level >= 75) TryUnlock("level_75");
            if (level >= 100) TryUnlock("level_100");
            int maxLevel = RpgFormulas.GetMaxLevel();
            int targetLevel = maxLevel == int.MaxValue ? 200 : maxLevel;
            if (level >= targetLevel) TryUnlock("level_max");
        }
        
        /// <summary>
        /// Check job advancement achievements
        /// </summary>
        public void CheckJobAchievements(JobTier tier)
        {
            if ((int)tier >= 1) TryUnlock("first_job");
            if ((int)tier >= 2) TryUnlock("second_job");
            if ((int)tier >= 3) TryUnlock("third_job");
        }
        
        /// <summary>
        /// Record a kill and check kill achievements
        /// </summary>
        public void RecordKill()
        {
            TotalKills++;
            
            if (TotalKills >= 1) TryUnlock("first_kill");
            if (TotalKills >= 100) TryUnlock("kill_100");
            if (TotalKills >= 1000) TryUnlock("kill_1000");
            if (TotalKills >= 10000) TryUnlock("kill_10000");
        }
        
        /// <summary>
        /// Check boss achievements
        /// </summary>
        public void CheckBossAchievements()
        {
            if (NPC.downedBoss1) TryUnlock("boss_eye");
            if (Main.hardMode) TryUnlock("boss_wof");
            if (NPC.downedMechBoss1 && NPC.downedMechBoss2 && NPC.downedMechBoss3) TryUnlock("boss_mech");
            if (NPC.downedPlantBoss) TryUnlock("boss_plantera");
            if (NPC.downedGolemBoss) TryUnlock("boss_golem");
            if (NPC.downedAncientCultist) TryUnlock("boss_cultist");
            if (NPC.downedMoonlord) TryUnlock("boss_moonlord");
        }
        
        /// <summary>
        /// Record skill learning
        /// </summary>
        public void RecordSkillLearned(int totalSkills, bool isMaxLevel = false)
        {
            TotalSkillsLearned = totalSkills;
            
            if (totalSkills >= 1) TryUnlock("skill_first");
            if (totalSkills >= 5) TryUnlock("skill_5");
            if (totalSkills >= 10) TryUnlock("skill_10");
            if (isMaxLevel) TryUnlock("skill_max");
        }
        
        /// <summary>
        /// Record stat allocation
        /// </summary>
        public void RecordStatAllocation(int totalPoints)
        {
            TotalStatPointsAllocated = totalPoints;
            
            if (totalPoints >= 50) TryUnlock("stat_50");
            if (totalPoints >= 100) TryUnlock("stat_100");
            if (totalPoints >= 200) TryUnlock("stat_200");
        }
        
        /// <summary>
        /// Record macro usage
        /// </summary>
        public void RecordMacroUsed()
        {
            TryUnlock("macro_master");
        }
        
        /// <summary>
        /// Record party XP
        /// </summary>
        public void RecordPartyXP()
        {
            TryUnlock("party_play");
        }
        
        /// <summary>
        /// Get completion percentage
        /// </summary>
        public float GetCompletionPercentage()
        {
            return (float)UnlockedAchievements.Count / Achievements.Count * 100f;
        }
        
        /// <summary>
        /// Get total XP rewards earned from achievements
        /// </summary>
        public int GetTotalXPRewards()
        {
            int total = 0;
            foreach (var id in UnlockedAchievements)
            {
                if (Achievements.TryGetValue(id, out var achievement))
                {
                    total += achievement.XpReward;
                }
            }
            return total;
        }
    }
    
    public enum AchievementCategory
    {
        Leveling,
        Jobs,
        Combat,
        Bosses,
        Skills,
        Stats,
        Special
    }
    
    public class AchievementData
    {
        public string Name { get; }
        public string Description { get; }
        public AchievementCategory Category { get; }
        public int XpReward { get; }
        
        public AchievementData(string name, string description, AchievementCategory category, int xpReward)
        {
            Name = name;
            Description = description;
            Category = category;
            XpReward = xpReward;
        }
    }
}
