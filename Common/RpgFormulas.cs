using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Rpg.Common.Compatibility;
using Rpg.Common.Config;
using Rpg.Common.Systems;

namespace Rpg.Common
{
    /// <summary>
    /// Core formula calculations for the entire mod
    /// All balance-sensitive formulas are centralized here for easy tuning
    /// </summary>
    public static class RpgFormulas
    {
        #region Level Cap System

        private enum LevelCapStepType
        {
            AtLeast,
            Add,
            Unlimited
        }

        private readonly struct LevelCapStep
        {
            public LevelCapStep(Func<bool> isUnlocked, LevelCapStepType type, int value, string requirement)
            {
                IsUnlocked = isUnlocked;
                Type = type;
                Value = value;
                Requirement = requirement;
            }

            public Func<bool> IsUnlocked { get; }
            public LevelCapStepType Type { get; }
            public int Value { get; }
            public string Requirement { get; }
        }

        private static readonly LevelCapStep[] LevelCapSteps =
        {
            new LevelCapStep(() => NPC.downedSlimeKing, LevelCapStepType.AtLeast, 10, "King Slime"),
            new LevelCapStep(() => NPC.downedBoss1, LevelCapStepType.AtLeast, 15, "Eye of Cthulhu"),
            new LevelCapStep(() => NPC.downedBoss2, LevelCapStepType.AtLeast, 25, "Eater/Brain"),
            new LevelCapStep(() => NPC.downedQueenBee, LevelCapStepType.AtLeast, 30, "Queen Bee"),
            new LevelCapStep(() => NPC.downedBoss3, LevelCapStepType.AtLeast, 40, "Skeletron"),
            new LevelCapStep(() => NPC.downedDeerclops, LevelCapStepType.AtLeast, 45, "Deerclops"),
            new LevelCapStep(() => Main.hardMode, LevelCapStepType.AtLeast, 60, "Wall of Flesh"),
            new LevelCapStep(() => NPC.downedQueenSlime, LevelCapStepType.AtLeast, 65, "Queen Slime"),
            new LevelCapStep(() => NPC.downedMechBoss1, LevelCapStepType.Add, 10, "The Destroyer"),
            new LevelCapStep(() => NPC.downedMechBoss2, LevelCapStepType.Add, 10, "The Twins"),
            new LevelCapStep(() => NPC.downedMechBoss3, LevelCapStepType.Add, 10, "Skeletron Prime"),
            new LevelCapStep(() => NPC.downedPlantBoss, LevelCapStepType.Add, 10, "Plantera"),
            new LevelCapStep(() => NPC.downedGolemBoss, LevelCapStepType.Add, 5, "Golem"),
            new LevelCapStep(() => NPC.downedFishron, LevelCapStepType.Add, 5, "Duke Fishron"),
            new LevelCapStep(() => NPC.downedEmpressOfLight, LevelCapStepType.Add, 5, "Empress of Light"),
            new LevelCapStep(() => NPC.downedMoonlord, LevelCapStepType.Unlimited, 0, "Moon Lord")
        };

        private static int ApplyLevelCapStep(int currentCap, LevelCapStep step)
        {
            return step.Type switch
            {
                LevelCapStepType.AtLeast => Math.Max(currentCap, step.Value),
                LevelCapStepType.Add => currentCap + step.Value,
                _ => int.MaxValue
            };
        }

        /// <summary>
        /// Get maximum achievable level based on boss progression
        /// CRITICAL: This prevents over-leveling before content
        /// </summary>
        public static int GetMaxLevel()
        {
            var serverConfig = ModContent.GetInstance<RpgServerConfig>();
            if (serverConfig != null && !serverConfig.EnableBossLevelCaps)
                return int.MaxValue;

            int maxLevel = RpgConstants.BASE_LEVEL_CAP + ModCompatibilitySystem.GetTotalModLevelCapBonus();

            foreach (var step in LevelCapSteps)
            {
                if (!step.IsUnlocked())
                    continue;

                if (step.Type == LevelCapStepType.Unlimited)
                    return int.MaxValue;

                maxLevel = ApplyLevelCapStep(maxLevel, step);
            }

            return maxLevel;
        }

        public static bool TryGetNextCapInfo(out int nextCap, out string requirement)
        {
            int maxLevel = RpgConstants.BASE_LEVEL_CAP + ModCompatibilitySystem.GetTotalModLevelCapBonus();

            foreach (var step in LevelCapSteps)
            {
                if (step.IsUnlocked())
                {
                    if (step.Type == LevelCapStepType.Unlimited)
                    {
                        nextCap = int.MaxValue;
                        requirement = null;
                        return false;
                    }

                    maxLevel = ApplyLevelCapStep(maxLevel, step);
                    continue;
                }

                if (step.Type == LevelCapStepType.Unlimited)
                {
                    nextCap = int.MaxValue;
                    requirement = step.Requirement;
                    return true;
                }

                nextCap = ApplyLevelCapStep(maxLevel, step);
                requirement = step.Requirement;
                return true;
            }

            nextCap = maxLevel;
            requirement = null;
            return false;
        }

        #endregion

        #region XP Calculation

        /// <summary>
        /// Get XP from killing an NPC
        /// </summary>
        public static long GetNPCExperience(NPC npc)
        {
            // No XP for friendly or town NPCs
            if (npc.friendly || npc.townNPC)
                return 0;

            // Body segments give no XP (worm bosses)
            if (IsBodySegment(npc.type))
                return 0;

            long baseXP;

            // Boss XP
            if (npc.boss || IsBossHead(npc.type))
            {
                baseXP = CalculateBossXP(npc, Main.LocalPlayer.GetModPlayer<Players.RpgPlayer>().Level);
            }
            else
            {
                // Regular NPC XP
                baseXP = CalculateBaseXP(npc);
            }

            // Apply event multiplier
            float eventMult = GetEventXPMultiplier();
            baseXP = (long)(baseXP * eventMult);

            return Math.Max(1, baseXP);
        }

        /// <summary>
        /// Calculate base XP from NPC stats
        /// Formula: (HP/100) × (1+Def/10) × (1+Dmg/25)
        /// </summary>
        public static int CalculateBaseXP(NPC npc)
        {
            float hpFactor = npc.lifeMax / RpgConstants.XP_HP_DIVISOR;
            float defFactor = 1f + (npc.defense / RpgConstants.XP_DEFENSE_DIVISOR);
            float dmgFactor = 1f + (npc.damage / RpgConstants.XP_DAMAGE_DIVISOR);

            int baseXP = (int)(hpFactor * defFactor * dmgFactor);
            return Math.Max(RpgConstants.MIN_NPC_XP, baseXP);
        }

        /// <summary>
        /// Calculate boss XP with cap to prevent explosion
        /// </summary>
        public static int CalculateBossXP(NPC boss, int averagePlayerLevel)
        {
            int bossLevel = GetBossLevel(boss.type);
            return CalculateBossXP(bossLevel, averagePlayerLevel);
        }

        /// <summary>
        /// Calculate boss XP from a boss level and player level
        /// </summary>
        public static int CalculateBossXP(int bossLevel, int averagePlayerLevel)
        {
            int baseXP = (int)(bossLevel * RpgConstants.BOSS_XP_MULTIPLIER);
            int safeLevel = Math.Max(1, averagePlayerLevel);

            // Cap to prevent excessive gains
            int maxXP = safeLevel * RpgConstants.BOSS_XP_PLAYER_LEVEL_MULTIPLIER;
            return Math.Min(baseXP, maxXP);
        }

        /// <summary>
        /// Get world level multiplier for XP
        /// </summary>
        public static float GetWorldLevelXPMultiplier(int worldLevel)
        {
            float baseMultiplier = 1f + (worldLevel * RpgConstants.XP_WORLD_LEVEL_MULTIPLIER);
            float logBonus = (float)(Math.Log(1.0 + worldLevel) * 0.05f);
            return baseMultiplier + logBonus;
        }

        /// <summary>
        /// Get event XP multiplier (50% reduction)
        /// </summary>
        public static float GetEventXPMultiplier()
        {
            if (Main.bloodMoon || Main.eclipse || 
                Main.pumpkinMoon || Main.snowMoon ||
                Main.invasionType > 0)
            {
                var serverConfig = ModContent.GetInstance<RpgServerConfig>();
                return serverConfig?.EventXPMultiplier ?? RpgConstants.EVENT_XP_MULTIPLIER;
            }
            return 1f;
        }

        /// <summary>
        /// Get XP multiplier based on monster level (ties XP to biome/world scaling)
        /// </summary>
        public static float GetMonsterLevelXPMultiplier(int monsterLevel)
        {
            if (monsterLevel <= 1)
                return 1f;

            float multiplier = 1f + ((monsterLevel - 1) * RpgConstants.XP_MONSTER_LEVEL_MULTIPLIER);
            return Math.Min(multiplier, RpgConstants.XP_MONSTER_LEVEL_MAX_MULTIPLIER);
        }

        /// <summary>
        /// Check if NPC is a body segment (should not give XP)
        /// </summary>
        public static bool IsBodySegment(int npcType)
        {
            return npcType == NPCID.EaterofWorldsBody ||
                   npcType == NPCID.EaterofWorldsTail ||
                   npcType == NPCID.TheDestroyerBody ||
                   npcType == NPCID.TheDestroyerTail;
        }

        /// <summary>
        /// Check if NPC is a boss head (gives full XP)
        /// </summary>
        public static bool IsBossHead(int npcType)
        {
            return npcType == NPCID.EaterofWorldsHead ||
                   npcType == NPCID.TheDestroyer;
        }

        #endregion

        #region Level-Up Requirements

        /// <summary>
        /// Get XP required for next level (exponential curve)
        /// </summary>
        public static long GetRequiredXP(int currentLevel)
        {
            // Inspired by AnotherRPG: a smooth exponential/log composite curve
            double baseGrowth = Math.Pow(1.12 + Math.Log(1.0 + currentLevel) * 0.01, currentLevel);
            double tierBonus = currentLevel switch
            {
                < 10 => 0.7,
                < 60 => 1.0,
                < 120 => 1.35,
                _ => 1.75
            };

            long target = (long)(baseGrowth * 90 * tierBonus);
            return Math.Max(100, Math.Min(target, long.MaxValue / 2));
        }

        #endregion

        #region Monster Scaling

        private static float GetHealthScalingPower()
        {
            if (NPC.downedPlantBoss || NPC.downedGolemBoss)
                return 1.2f;
            if (Main.hardMode)
                return 1.1f;
            return 1.0f;
        }

        /// <summary>
        /// Get monster HP multiplier based on world level
        /// </summary>
        public static float GetMonsterHPMultiplier(int worldLevel)
        {
            var serverConfig = ModContent.GetInstance<RpgServerConfig>();
            float scalePerLevel = serverConfig?.MonsterHPScalePerLevel ?? RpgConstants.MONSTER_HP_SCALE_PER_LEVEL;
            float baseMultiplier = 1f + (worldLevel * scalePerLevel);
            return (float)Math.Pow(baseMultiplier, GetHealthScalingPower());
        }

        /// <summary>
        /// Get monster HP multiplier based on monster level (for biome-scaled monsters)
        /// </summary>
        public static float GetMonsterHPMultiplierForLevel(int monsterLevel)
        {
            if (monsterLevel <= 1)
                return 1f;

            var serverConfig = ModContent.GetInstance<RpgServerConfig>();
            float scalePerLevel = serverConfig?.MonsterHPScalePerLevel ?? RpgConstants.MONSTER_HP_SCALE_PER_LEVEL;
            
            // Use monster level directly for scaling
            float linear = 1f + (monsterLevel * scalePerLevel);
            float exponential = (float)Math.Pow(1.01f, monsterLevel);
            return Math.Max(linear, exponential);
        }

        /// <summary>
        /// Get monster damage multiplier based on world level and difficulty
        /// </summary>
        public static float GetMonsterDamageMultiplier(int worldLevel)
        {
            var serverConfig = ModContent.GetInstance<RpgServerConfig>();
            float baseScaleRate = serverConfig?.MonsterDamageScalePerLevel ?? RpgConstants.CLASSIC_DAMAGE_SCALE;

            float scaleRate = baseScaleRate;

            if (Main.masterMode)
                scaleRate = baseScaleRate * (serverConfig?.MasterModeScaleReduction ?? RpgConstants.MASTER_DAMAGE_SCALE / RpgConstants.CLASSIC_DAMAGE_SCALE);
            else if (Main.getGoodWorld)
                scaleRate = baseScaleRate * (serverConfig?.ExpertModeScaleReduction ?? RpgConstants.EXPERT_DAMAGE_SCALE / RpgConstants.CLASSIC_DAMAGE_SCALE);
            else if (Main.expertMode)
                scaleRate = baseScaleRate * (serverConfig?.ExpertModeScaleReduction ?? RpgConstants.EXPERT_DAMAGE_SCALE / RpgConstants.CLASSIC_DAMAGE_SCALE);

            float linear = 1f + (worldLevel * scaleRate);
            float exponential = (float)Math.Pow(1.01f, worldLevel);
            float bonus = (exponential - 1f) * 0.4f;
            float combined = linear + bonus;
            float taperedMax = 1f + (worldLevel * scaleRate * 2f);
            return Math.Min(combined, taperedMax);
        }

        /// <summary>
        /// Get monster damage multiplier based on monster level
        /// </summary>
        public static float GetMonsterDamageMultiplierForLevel(int monsterLevel)
        {
            if (monsterLevel <= 1)
                return 1f;

            var serverConfig = ModContent.GetInstance<RpgServerConfig>();
            float baseScaleRate = serverConfig?.MonsterDamageScalePerLevel ?? RpgConstants.CLASSIC_DAMAGE_SCALE;

            float scaleRate = baseScaleRate;

            if (Main.masterMode)
                scaleRate = baseScaleRate * (serverConfig?.MasterModeScaleReduction ?? RpgConstants.MASTER_DAMAGE_SCALE / RpgConstants.CLASSIC_DAMAGE_SCALE);
            else if (Main.getGoodWorld)
                scaleRate = baseScaleRate * (serverConfig?.ExpertModeScaleReduction ?? RpgConstants.EXPERT_DAMAGE_SCALE / RpgConstants.CLASSIC_DAMAGE_SCALE);
            else if (Main.expertMode)
                scaleRate = baseScaleRate * (serverConfig?.ExpertModeScaleReduction ?? RpgConstants.EXPERT_DAMAGE_SCALE / RpgConstants.CLASSIC_DAMAGE_SCALE);

            float linear = 1f + (monsterLevel * scaleRate);
            float exponential = (float)Math.Pow(1.01f, monsterLevel);
            float bonus = (exponential - 1f) * 0.4f;
            float combined = linear + bonus;
            float taperedMax = 1f + (monsterLevel * scaleRate * 2f);
            return Math.Min(combined, taperedMax);
        }

        /// <summary>
        /// Get boss HP multiplier based on world level
        /// </summary>
        public static float GetBossHPMultiplier(int worldLevel)
        {
            var serverConfig = ModContent.GetInstance<RpgServerConfig>();
            float scalePerLevel = serverConfig?.BossHPScalePerLevel ?? 0.05f;
            float baseMultiplier = 1f + (worldLevel * scalePerLevel);
            return (float)Math.Pow(baseMultiplier, GetHealthScalingPower());
        }

        /// <summary>
        /// Get boss HP multiplier based on boss level
        /// </summary>
        public static float GetBossHPMultiplierForLevel(int bossLevel)
        {
            if (bossLevel <= 1)
                return 1f;

            var serverConfig = ModContent.GetInstance<RpgServerConfig>();
            float scalePerLevel = serverConfig?.BossHPScalePerLevel ?? 0.15f;
            
            float linear = 1f + (bossLevel * scalePerLevel);
            float exponential = (float)Math.Pow(1.01f, bossLevel);
            return Math.Max(linear, exponential);
        }

        /// <summary>
        /// Get boss damage multiplier based on world level and difficulty
        /// </summary>
        public static float GetBossDamageMultiplier(int worldLevel)
        {
            var serverConfig = ModContent.GetInstance<RpgServerConfig>();
            float baseScaleRate = serverConfig?.BossDamageScalePerLevel ?? 0.012f;

            float scaleRate = baseScaleRate;

            if (Main.masterMode)
                scaleRate = baseScaleRate * (serverConfig?.MasterModeScaleReduction ?? RpgConstants.MASTER_DAMAGE_SCALE / RpgConstants.CLASSIC_DAMAGE_SCALE);
            else if (Main.getGoodWorld)
                scaleRate = baseScaleRate * (serverConfig?.ExpertModeScaleReduction ?? RpgConstants.EXPERT_DAMAGE_SCALE / RpgConstants.CLASSIC_DAMAGE_SCALE);
            else if (Main.expertMode)
                scaleRate = baseScaleRate * (serverConfig?.ExpertModeScaleReduction ?? RpgConstants.EXPERT_DAMAGE_SCALE / RpgConstants.CLASSIC_DAMAGE_SCALE);

            float linear = 1f + (worldLevel * scaleRate);
            float exponential = (float)Math.Pow(1.01f, worldLevel);
            float bonus = (exponential - 1f) * 0.4f;
            float combined = linear + bonus;
            float taperedMax = 1f + (worldLevel * scaleRate * 2f);
            return Math.Min(combined, taperedMax);
        }

        /// <summary>
        /// Get boss damage multiplier based on boss level
        /// </summary>
        public static float GetBossDamageMultiplierForLevel(int bossLevel)
        {
            if (bossLevel <= 1)
                return 1f;

            var serverConfig = ModContent.GetInstance<RpgServerConfig>();
            float baseScaleRate = serverConfig?.BossDamageScalePerLevel ?? 0.015f;

            float scaleRate = baseScaleRate;

            if (Main.masterMode)
                scaleRate = baseScaleRate * (serverConfig?.MasterModeScaleReduction ?? RpgConstants.MASTER_DAMAGE_SCALE / RpgConstants.CLASSIC_DAMAGE_SCALE);
            else if (Main.getGoodWorld)
                scaleRate = baseScaleRate * (serverConfig?.ExpertModeScaleReduction ?? RpgConstants.EXPERT_DAMAGE_SCALE / RpgConstants.CLASSIC_DAMAGE_SCALE);
            else if (Main.expertMode)
                scaleRate = baseScaleRate * (serverConfig?.ExpertModeScaleReduction ?? RpgConstants.EXPERT_DAMAGE_SCALE / RpgConstants.CLASSIC_DAMAGE_SCALE);

            float linear = 1f + (bossLevel * scaleRate);
            float exponential = (float)Math.Pow(1.01f, bossLevel);
            float bonus = (exponential - 1f) * 0.4f;
            float combined = linear + bonus;
            float taperedMax = 1f + (bossLevel * scaleRate * 2f);
            return Math.Min(combined, taperedMax);
        }

        /// <summary>
        /// Get monster defense multiplier based on world level
        /// </summary>
        public static float GetMonsterDefenseMultiplier(int worldLevel)
        {
            var serverConfig = ModContent.GetInstance<RpgServerConfig>();
            if (serverConfig == null || !serverConfig.ScaleMonsterDefense)
                return 1f;
                
            float scalePerLevel = serverConfig.MonsterDefenseScalePerLevel;
            return 1f + (worldLevel * scalePerLevel);
        }

        /// <summary>
        /// Get monster defense multiplier based on monster level
        /// </summary>
        public static float GetMonsterDefenseMultiplierForLevel(int monsterLevel)
        {
            if (monsterLevel <= 1)
                return 1f;

            var serverConfig = ModContent.GetInstance<RpgServerConfig>();
            if (serverConfig == null || !serverConfig.ScaleMonsterDefense)
                return 1f;
                
            float scalePerLevel = serverConfig.MonsterDefenseScalePerLevel;
            return 1f + (monsterLevel * scalePerLevel);
        }

        /// <summary>
        /// Get boss defense multiplier based on world level
        /// </summary>
        public static float GetBossDefenseMultiplier(int worldLevel)
        {
            var serverConfig = ModContent.GetInstance<RpgServerConfig>();
            if (serverConfig == null || !serverConfig.ScaleMonsterDefense)
                return 1f;
                
            // Bosses use the same defense scaling as monsters for now
            float scalePerLevel = serverConfig.MonsterDefenseScalePerLevel;
            return 1f + (worldLevel * scalePerLevel);
        }

        /// <summary>
        /// Get boss defense multiplier based on boss level
        /// </summary>
        public static float GetBossDefenseMultiplierForLevel(int bossLevel)
        {
            if (bossLevel <= 1)
                return 1f;

            var serverConfig = ModContent.GetInstance<RpgServerConfig>();
            if (serverConfig == null || !serverConfig.ScaleMonsterDefense)
                return 1f;
                
            // Bosses use the same defense scaling as monsters for now
            float scalePerLevel = serverConfig.MonsterDefenseScalePerLevel;
            return 1f + (bossLevel * scalePerLevel);
        }

        /// <summary>
        /// Get monster level based on base level + world level
        /// </summary>
        public static int GetMonsterLevel(int baseLevel, int worldLevel)
        {
            return baseLevel + worldLevel;
        }

        #endregion

        #region Boss Levels

        /// <summary>
        /// Get boss level based on base tier and progression
        /// </summary>
        public static int GetBossLevel(int npcType)
        {
            int baseLevel = GetBossBaseLevel(npcType);
            int progressionLevel = GetBossProgressionLevel();
            return Math.Max(baseLevel, progressionLevel);
        }

        private static int GetBossProgressionLevel()
        {
            int progressionLevel = 1;

            if (NPC.downedSlimeKing)
                progressionLevel = Math.Max(progressionLevel, GetBossBaseLevel(NPCID.KingSlime));
            if (NPC.downedBoss1)
                progressionLevel = Math.Max(progressionLevel, GetBossBaseLevel(NPCID.EyeofCthulhu));
            if (NPC.downedBoss2)
                progressionLevel = Math.Max(progressionLevel, GetBossBaseLevel(NPCID.EaterofWorldsHead));
            if (NPC.downedQueenBee)
                progressionLevel = Math.Max(progressionLevel, GetBossBaseLevel(NPCID.QueenBee));
            if (NPC.downedBoss3)
                progressionLevel = Math.Max(progressionLevel, GetBossBaseLevel(NPCID.SkeletronHead));
            if (NPC.downedDeerclops)
                progressionLevel = Math.Max(progressionLevel, GetBossBaseLevel(NPCID.Deerclops));
            if (Main.hardMode)
                progressionLevel = Math.Max(progressionLevel, GetBossBaseLevel(NPCID.WallofFlesh));
            if (NPC.downedQueenSlime)
                progressionLevel = Math.Max(progressionLevel, GetBossBaseLevel(NPCID.QueenSlimeBoss));
            if (NPC.downedMechBoss1)
                progressionLevel = Math.Max(progressionLevel, GetBossBaseLevel(NPCID.TheDestroyer));
            if (NPC.downedMechBoss2)
                progressionLevel = Math.Max(progressionLevel, GetBossBaseLevel(NPCID.Retinazer));
            if (NPC.downedMechBoss3)
                progressionLevel = Math.Max(progressionLevel, GetBossBaseLevel(NPCID.SkeletronPrime));
            if (NPC.downedPlantBoss)
                progressionLevel = Math.Max(progressionLevel, GetBossBaseLevel(NPCID.Plantera));
            if (NPC.downedGolemBoss)
                progressionLevel = Math.Max(progressionLevel, GetBossBaseLevel(NPCID.Golem));
            if (NPC.downedFishron)
                progressionLevel = Math.Max(progressionLevel, GetBossBaseLevel(NPCID.DukeFishron));
            if (NPC.downedEmpressOfLight)
                progressionLevel = Math.Max(progressionLevel, GetBossBaseLevel(NPCID.HallowBoss));
            if (NPC.downedAncientCultist)
                progressionLevel = Math.Max(progressionLevel, GetBossBaseLevel(NPCID.CultistBoss));
            if (NPC.downedMoonlord)
                progressionLevel = Math.Max(progressionLevel, GetBossBaseLevel(NPCID.MoonLordCore));

            int worldLevelProgression = GetBossProgressionLevelFromWorldLevel(RpgWorld.GetWorldLevel());
            return Math.Max(progressionLevel, worldLevelProgression);
        }

        private static int GetBossProgressionLevelFromWorldLevel(int worldLevel)
        {
            int safeWorldLevel = Math.Max(1, worldLevel);
            float scaled = (safeWorldLevel * RpgConstants.BOSS_PROGRESSION_WORLD_LEVEL_MULT) + RpgConstants.BOSS_PROGRESSION_WORLD_LEVEL_BASE;
            return Math.Max(1, (int)MathF.Round(scaled));
        }

        private static int GetBossBaseLevel(int npcType)
        {
            return npcType switch
            {
                // Pre-Hardmode
                NPCID.KingSlime => 5,
                NPCID.EyeofCthulhu => 8,
                NPCID.EaterofWorldsHead => 12,
                NPCID.BrainofCthulhu => 12,
                NPCID.QueenBee => 15,
                NPCID.SkeletronHead => 18,
                NPCID.Deerclops => 20,
                NPCID.WallofFlesh => 25,

                // Hardmode
                NPCID.QueenSlimeBoss => 30,
                NPCID.TheDestroyer => 35,
                NPCID.Retinazer => 35,
                NPCID.Spazmatism => 35,
                NPCID.SkeletronPrime => 40,
                NPCID.Plantera => 45,
                NPCID.Golem => 50,
                NPCID.DukeFishron => 55,
                NPCID.HallowBoss => 58, // Empress of Light
                NPCID.CultistBoss => 60,
                NPCID.MoonLordCore => 70,

                _ => 10 // Default for unknown bosses
            };
        }

        #endregion

        #region Stat Effects

        /// <summary>
        /// Calculate melee damage bonus from Strength
        /// </summary>
        public static float GetStrengthDamageBonus(int strengthPoints, JobType job)
        {
            float baseBonus = strengthPoints * RpgConstants.STRENGTH_MELEE_DAMAGE_PER_POINT;

            if (job == JobType.Warrior || job == JobType.Knight ||
                job == JobType.Berserker || job == JobType.Guardian ||
                job == JobType.BloodKnight || job == JobType.Paladin ||
                job == JobType.DeathKnight)
            {
                baseBonus *= RpgConstants.PRIMARY_STAT_EFFICIENCY;
            }

            return baseBonus;
        }

        /// <summary>
        /// Calculate ranged damage bonus from Dexterity
        /// </summary>
        public static float GetDexterityDamageBonus(int dexterityPoints, JobType job)
        {
            float baseBonus = dexterityPoints * RpgConstants.DEXTERITY_RANGED_DAMAGE_PER_POINT;

            if (job == JobType.Ranger || job == JobType.Sniper ||
                job == JobType.Gunslinger || job == JobType.Deadeye ||
                job == JobType.Gunmaster)
            {
                baseBonus *= RpgConstants.PRIMARY_STAT_EFFICIENCY;
            }

            return baseBonus;
        }

        /// <summary>
        /// Calculate finesse damage bonus from Rogue
        /// </summary>
        public static float GetRogueDamageBonus(int roguePoints, JobType job)
        {
            float baseBonus = roguePoints * RpgConstants.ROGUE_FINESSE_DAMAGE_PER_POINT;

            if (job == JobType.Assassin || job == JobType.Shadow || job == JobType.Spellthief)
            {
                baseBonus *= RpgConstants.PRIMARY_STAT_EFFICIENCY;
            }

            return baseBonus;
        }

        /// <summary>
        /// Calculate magic damage bonus from Intelligence
        /// </summary>
        public static float GetIntelligenceDamageBonus(int intelligencePoints, JobType job)
        {
            float baseBonus = intelligencePoints * RpgConstants.INTELLIGENCE_MAGIC_DAMAGE_PER_POINT;

            if (job == JobType.Mage || job == JobType.Sorcerer ||
                job == JobType.Cleric || job == JobType.Archmage ||
                job == JobType.Archbishop || job == JobType.Warlock ||
                job == JobType.BattleMage)
            {
                baseBonus *= RpgConstants.PRIMARY_STAT_EFFICIENCY;
            }

            return baseBonus;
        }

        /// <summary>
        /// Calculate summon damage bonus from Focus
        /// </summary>
        public static float GetSummonDamageBonus(int focusPoints, JobType job)
        {
            float baseBonus = focusPoints * RpgConstants.FOCUS_SUMMON_DAMAGE_PER_POINT;

            if (job == JobType.Summoner || job == JobType.Beastmaster ||
                job == JobType.Necromancer || job == JobType.Overlord ||
                job == JobType.LichKing || job == JobType.Druid)
            {
                baseBonus *= RpgConstants.PRIMARY_STAT_EFFICIENCY;
            }

            return baseBonus;
        }

        /// <summary>
        /// Calculate bonus minion slots from Focus with increasing thresholds
        /// </summary>
        public static int GetFocusMinionSlotBonus(int focusPoints)
        {
            int basePoints = RpgConstants.FOCUS_MINION_SLOT_BASE_POINTS;
            int stepPoints = RpgConstants.FOCUS_MINION_SLOT_STEP_POINTS;

            if (focusPoints < basePoints)
                return 0;

            int slots = 0;
            for (int n = 1; ; n++)
            {
                int required = basePoints * n + (stepPoints * (n - 1) * n) / 2;
                if (focusPoints < required)
                    break;
                slots = n;
            }

            return slots;
        }

        /// <summary>
        /// Calculate max HP bonus from Vitality
        /// </summary>
        public static int GetVitalityHealthBonus(int vitalityPoints)
        {
            return vitalityPoints * RpgConstants.VITALITY_HP_PER_POINT;
        }

        /// <summary>
        /// Calculate armor bonus from Defense
        /// </summary>
        public static int GetDefenseArmorBonus(int defensePoints)
        {
            return (defensePoints / 5) * RpgConstants.DEFENSE_ARMOR_PER_5_POINTS;
        }

        /// <summary>
        /// Calculate crit chance bonus from Luck
        /// </summary>
        public static float GetLuckCritBonus(int luckPoints)
        {
            return luckPoints * RpgConstants.LUCK_CRIT_PER_POINT;
        }

        /// <summary>
        /// Calculate attack speed bonus from Dexterity
        /// </summary>
        public static float GetDexterityAttackSpeedBonus(int dexterityPoints)
        {
            return dexterityPoints * RpgConstants.DEXTERITY_ATTACK_SPEED_PER_POINT;
        }

        /// <summary>
        /// Calculate mana cost reduction from Intelligence (capped at 50%)
        /// </summary>
        public static float GetIntelligenceManaCostReduction(int intelligencePoints)
        {
            float reduction = intelligencePoints * RpgConstants.INTELLIGENCE_MANA_COST_REDUCTION_PER_POINT;
            return Math.Min(reduction, RpgConstants.INTELLIGENCE_MAX_MANA_COST_REDUCTION);
        }

        /// <summary>
        /// Calculate movement speed bonus from Agility (capped at 50%)
        /// </summary>
        public static float GetAgilityMovementSpeedBonus(int agilityPoints)
        {
            float bonus = agilityPoints * RpgConstants.AGILITY_MOVEMENT_SPEED_PER_POINT;
            return Math.Min(bonus, RpgConstants.AGILITY_MAX_MOVEMENT_SPEED);
        }

        #endregion

        #region Stat Points

        /// <summary>
        /// Get stat points awarded per level
        /// </summary>
        public static int GetStatPointsPerLevel(JobTier tier)
        {
            return tier switch
            {
                JobTier.Novice => RpgConstants.NOVICE_STAT_POINTS,
                JobTier.Tier1 => RpgConstants.TIER1_STAT_POINTS,
                JobTier.Tier2 => RpgConstants.TIER2_STAT_POINTS,
                JobTier.Tier3 => RpgConstants.TIER3_STAT_POINTS,
                _ => 5
            };
        }

        /// <summary>
        /// Get skill points awarded per level
        /// </summary>
        public static int GetSkillPointsPerLevel(JobTier tier)
        {
            return tier switch
            {
                JobTier.Novice => RpgConstants.NOVICE_SKILL_POINTS,
                JobTier.Tier1 => RpgConstants.TIER1_SKILL_POINTS,
                JobTier.Tier2 => RpgConstants.TIER2_SKILL_POINTS,
                JobTier.Tier3 => RpgConstants.TIER3_SKILL_POINTS,
                _ => 2
            };
        }

        /// <summary>
        /// Get the level cap for the current job tier (used for pending point storage)
        /// </summary>
        public static int GetTierMaxLevel(JobTier tier)
        {
            return tier switch
            {
                JobTier.Novice => RpgConstants.FIRST_JOB_LEVEL,
                JobTier.Tier1 => RpgConstants.SECOND_JOB_LEVEL,
                JobTier.Tier2 => RpgConstants.THIRD_JOB_LEVEL,
                _ => int.MaxValue
            };
        }

        #endregion

        #region Job System

        /// <summary>
        /// Get job tier from job type
        /// </summary>
        public static JobTier GetJobTier(JobType job)
        {
            return job switch
            {
                JobType.None => JobTier.Novice,
                JobType.Novice => JobTier.Novice,
                
                JobType.Warrior or JobType.Ranger or JobType.Mage or JobType.Summoner 
                    => JobTier.Tier1,
                
                JobType.Knight or JobType.Berserker or JobType.Paladin or JobType.DeathKnight or
                JobType.Sniper or JobType.Assassin or JobType.Gunslinger or
                JobType.Sorcerer or JobType.Cleric or JobType.Archmage or JobType.Warlock or
                JobType.Spellblade or JobType.BattleMage or
                JobType.Beastmaster or JobType.Necromancer or JobType.Druid or
                JobType.Shadow or JobType.Spellthief
                    => JobTier.Tier2,
                
                _ => JobTier.Tier3
            };
        }

        /// <summary>
        /// Check if job advancement is allowed by progression conditions
        /// </summary>
        public static bool CanAdvanceJob(Players.RpgPlayer player)
        {
            if (player == null)
                return false;

            var available = Jobs.JobDatabase.GetAvailableJobs(player);
            return available != null && available.Count > 0;
        }

        #endregion
    }
}
