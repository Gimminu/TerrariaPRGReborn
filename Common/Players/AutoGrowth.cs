using System.Collections.Generic;

namespace RpgMod.Common.Players
{
    /// <summary>
    /// Auto-growth system (30~40% of total growth)
    /// Provides job-specific stat curves that grow automatically on level-up
    /// </summary>
    public static class AutoGrowth
    {
        #region Growth Curves

        /// <summary>
        /// Warrior growth curve - Strength/Vitality/Defense focus
        /// </summary>
        private static readonly Dictionary<StatType, float> WarriorGrowth = new()
        {
            { StatType.Strength, 0.7f },      // Primary: Melee damage
            { StatType.Dexterity, 0.2f },
            { StatType.Rogue, 0.1f },
            { StatType.Intelligence, 0.1f },
            { StatType.Focus, 0.1f },
            { StatType.Vitality, 0.8f },      // High HP
            { StatType.Stamina, 0.4f },       // Physical skills
            { StatType.Defense, 0.5f },       // Tanky
            { StatType.Agility, 0.2f },
            { StatType.Wisdom, 0.1f },
            { StatType.Fortitude, 0.4f },     // Status resist
            { StatType.Luck, 0.2f }
        };

        /// <summary>
        /// Ranger growth curve - Dexterity/Rogue/Agility focus
        /// </summary>
        private static readonly Dictionary<StatType, float> RangerGrowth = new()
        {
            { StatType.Strength, 0.2f },
            { StatType.Dexterity, 0.7f },     // Primary: Ranged damage + attack speed
            { StatType.Rogue, 0.7f },         // Finesse/crit
            { StatType.Intelligence, 0.1f },
            { StatType.Focus, 0.2f },
            { StatType.Vitality, 0.4f },
            { StatType.Stamina, 0.5f },       // Physical skills
            { StatType.Defense, 0.2f },
            { StatType.Agility, 0.5f },       // Mobility
            { StatType.Wisdom, 0.1f },
            { StatType.Fortitude, 0.2f },
            { StatType.Luck, 0.3f }           // Crit synergy
        };

        /// <summary>
        /// Mage growth curve - Intelligence/Focus/Wisdom focus
        /// </summary>
        private static readonly Dictionary<StatType, float> MageGrowth = new()
        {
            { StatType.Strength, 0.1f },
            { StatType.Dexterity, 0.1f },
            { StatType.Rogue, 0.1f },
            { StatType.Intelligence, 0.9f },  // Primary: Magic damage
            { StatType.Focus, 0.1f },         // Summon support (minor)
            { StatType.Vitality, 0.3f },      // Low HP
            { StatType.Stamina, 0.1f },
            { StatType.Defense, 0.2f },
            { StatType.Agility, 0.1f },
            { StatType.Wisdom, 0.8f },        // Mana pool
            { StatType.Fortitude, 0.2f },
            { StatType.Luck, 0.2f }
        };

        /// <summary>
        /// Summoner growth curve - Wisdom/Intelligence/Focus focus
        /// </summary>
        private static readonly Dictionary<StatType, float> SummonerGrowth = new()
        {
            { StatType.Strength, 0.1f },
            { StatType.Dexterity, 0.2f },
            { StatType.Rogue, 0.1f },
            { StatType.Intelligence, 0.5f },  // Magic synergy
            { StatType.Focus, 0.8f },         // Summon focus
            { StatType.Vitality, 0.4f },
            { StatType.Stamina, 0.2f },
            { StatType.Defense, 0.2f },
            { StatType.Agility, 0.2f },
            { StatType.Wisdom, 0.3f },        // Mana support
            { StatType.Fortitude, 0.3f },
            { StatType.Luck, 0.3f }
        };

        /// <summary>
        /// Novice growth curve - Balanced (all stats grow evenly)
        /// </summary>
        private static readonly Dictionary<StatType, float> NoviceGrowth = new()
        {
            { StatType.Strength, 0.3f },
            { StatType.Dexterity, 0.3f },
            { StatType.Rogue, 0.2f },
            { StatType.Intelligence, 0.3f },
            { StatType.Focus, 0.2f },
            { StatType.Vitality, 0.4f },
            { StatType.Stamina, 0.3f },
            { StatType.Defense, 0.3f },
            { StatType.Agility, 0.3f },
            { StatType.Wisdom, 0.3f },
            { StatType.Fortitude, 0.2f },
            { StatType.Luck, 0.2f }
        };

        #endregion

        #region Growth Application

        /// <summary>
        /// Get the growth curve for a specific job
        /// </summary>
        public static Dictionary<StatType, float> GetGrowthCurve(JobType job)
        {
            return job switch
            {
                // Pure classes
                JobType.Warrior => WarriorGrowth,
                JobType.Ranger => RangerGrowth,
                JobType.Mage => MageGrowth,
                JobType.Summoner => SummonerGrowth,

                // Warrior advanced
                JobType.Knight => MergeGrowthCurves(WarriorGrowth, WarriorGrowth, 0.7f, 0.3f),
                JobType.Berserker => MergeGrowthCurves(WarriorGrowth, WarriorGrowth, 0.8f, 0.2f),
                JobType.Paladin => MergeGrowthCurves(WarriorGrowth, MageGrowth, 0.7f, 0.3f),
                JobType.DeathKnight => MergeGrowthCurves(WarriorGrowth, MageGrowth, 0.6f, 0.4f),

                // Ranger advanced
                JobType.Sniper => MergeGrowthCurves(RangerGrowth, RangerGrowth, 0.8f, 0.2f),
                JobType.Assassin => MergeGrowthCurves(RangerGrowth, RangerGrowth, 0.7f, 0.3f),
                JobType.Gunslinger => MergeGrowthCurves(RangerGrowth, RangerGrowth, 0.75f, 0.25f),

                // Mage advanced
                JobType.Sorcerer => MageGrowth,
                JobType.Cleric => MergeGrowthCurves(MageGrowth, SummonerGrowth, 0.6f, 0.4f),
                JobType.Archmage => MergeGrowthCurves(MageGrowth, MageGrowth, 0.8f, 0.2f),
                JobType.Warlock => MergeGrowthCurves(MageGrowth, MageGrowth, 0.7f, 0.3f),
                JobType.Spellblade => MergeGrowthCurves(MageGrowth, WarriorGrowth, 0.6f, 0.4f),
                JobType.BattleMage => MergeGrowthCurves(MageGrowth, WarriorGrowth, 0.5f, 0.5f),

                // Summoner advanced
                JobType.Beastmaster => MergeGrowthCurves(SummonerGrowth, RangerGrowth, 0.7f, 0.3f),
                JobType.Necromancer => MergeGrowthCurves(SummonerGrowth, MageGrowth, 0.7f, 0.3f),
                JobType.Druid => MergeGrowthCurves(SummonerGrowth, MageGrowth, 0.6f, 0.4f),

                // Hybrid advanced
                JobType.Shadow => MergeGrowthCurves(RangerGrowth, WarriorGrowth, 0.6f, 0.4f),
                JobType.Spellthief => MergeGrowthCurves(RangerGrowth, MageGrowth, 0.5f, 0.5f),

                // Tier 3
                JobType.Guardian => WarriorGrowth,
                JobType.BloodKnight => MergeGrowthCurves(WarriorGrowth, RangerGrowth, 0.75f, 0.25f),
                JobType.Deadeye => RangerGrowth,
                JobType.Gunmaster => RangerGrowth,
                JobType.Archbishop => MergeGrowthCurves(MageGrowth, SummonerGrowth, 0.4f, 0.6f),
                JobType.Overlord => MergeGrowthCurves(SummonerGrowth, RangerGrowth, 0.8f, 0.2f),
                JobType.LichKing => MergeGrowthCurves(SummonerGrowth, MageGrowth, 0.7f, 0.3f),

                // Novice or unknown
                _ => NoviceGrowth
            };
        }

        /// <summary>
        /// Merge two growth curves (for hybrid classes)
        /// </summary>
        private static Dictionary<StatType, float> MergeGrowthCurves(
            Dictionary<StatType, float> curve1,
            Dictionary<StatType, float> curve2,
            float weight1,
            float weight2)
        {
            var merged = new Dictionary<StatType, float>();

            foreach (StatType stat in System.Enum.GetValues(typeof(StatType)))
            {
                float value1 = curve1.ContainsKey(stat) ? curve1[stat] : 0f;
                float value2 = curve2.ContainsKey(stat) ? curve2[stat] : 0f;
                merged[stat] = value1 * weight1 + value2 * weight2;
            }

            return merged;
        }

        /// <summary>
        /// Apply auto-growth for one level (called by PlayerLevel.LevelUp)
        /// This represents 30~40% of total stat growth
        /// </summary>
        public static void ApplyAutoGrowth(RpgPlayer rpgPlayer)
        {
            var growthCurve = GetGrowthCurve(rpgPlayer.CurrentJob);

            foreach (var kvp in growthCurve)
            {
                StatType stat = kvp.Key;
                float increase = kvp.Value;

                // Fractional growth accumulates (e.g., 0.7 STR/level = 7 STR per 10 levels)
                rpgPlayer.ApplyAutoGrowth(stat, increase);
            }
        }

        /// <summary>
        /// Calculate total auto-growth over a level range (for UI/preview)
        /// </summary>
        public static Dictionary<StatType, int> CalculateTotalAutoGrowth(JobType job, int fromLevel, int toLevel)
        {
            var growthCurve = GetGrowthCurve(job);
            var total = new Dictionary<StatType, int>();

            int levels = toLevel - fromLevel;

            foreach (var kvp in growthCurve)
            {
                total[kvp.Key] = (int)(kvp.Value * levels);
            }

            return total;
        }

        #endregion
    }
}
