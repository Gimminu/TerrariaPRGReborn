namespace Rpg.Common
{
    /// <summary>
    /// Central constants for the entire mod - modify here for balance adjustments
    /// </summary>
    public static class RpgConstants
    {
        #region Level System

        public const int STARTING_LEVEL = 1;
        public const int BASE_LEVEL_CAP = 8; // Cap without any boss kills
        public const int MAX_NOVICE_LEVEL = 10;

        // Level caps by boss progression (see GetMaxLevel in RpgFormulas)
        public const int PRE_HARDMODE_CAP = 50;
        public const int EARLY_HARDMODE_CAP = 60;
        public const int POST_MECH_CAP = 95;
        public const int PRE_MOONLORD_CAP = 120;
        // Post-Moon Lord = unlimited (int.MaxValue)
        
        // Monster level caps by progression
        // Per design doc: Pre-hardmode monsters stop scaling in hardmode
        public const int PREHARDMODE_MONSTER_LEVEL_CAP = 6; // Max world level for pre-hm monsters
        public const int PREHARDMODE_MONSTER_MIN_LEVEL = 1; // Floor for pre-hm monsters (pre-hardmode)
        public const int PREHARDMODE_MONSTER_MIN_LEVEL_HARDMODE = 8; // Floor for pre-hm monsters in hardmode
        public const int HARDMODE_MONSTER_MIN_LEVEL = 25; // Floor for hardmode monsters

        #endregion

        #region Level Difference System (XP Scaling)

        // Monster level randomization range (±2 from base)
        public const int MONSTER_LEVEL_RANDOM_RANGE = 2;
        
        // Level difference XP multipliers (레벨 차이에 따른 경험치 배율)
        // Sweet Spot: Player +1~+3 = 110~120% XP (권장 난이도)
        public const float LEVEL_DIFF_SWEETSPOT_START = 1f;      // +1 레벨부터 보너스 시작
        public const float LEVEL_DIFF_SWEETSPOT_END = 3f;        // +3 레벨까지 보너스
        public const float LEVEL_DIFF_SWEETSPOT_BONUS = 1.2f;    // 120% XP (최적 효율)
        
        // High level monsters (위험하지만 비효율)
        public const float LEVEL_DIFF_PLUS_5_MULT = 0.60f;       // +5 레벨: 60% XP
        public const float LEVEL_DIFF_PLUS_7_MULT = 0.30f;       // +7 레벨: 30% XP
        public const float LEVEL_DIFF_PLUS_10_MULT = 0.10f;      // +10 레벨 이상: 10% XP (폭업 방지)
        
        // Low level monsters (낮은 레벨 사냥 방지)
        public const float LEVEL_DIFF_MINUS_2_MULT = 1.00f;      // -2 레벨: 100% XP
        public const float LEVEL_DIFF_MINUS_5_MULT = 0.70f;      // -5 레벨: 70% XP
        public const float LEVEL_DIFF_MINUS_8_MULT = 0.40f;      // -8 레벨: 40% XP
        public const float LEVEL_DIFF_MINUS_10_MULT = 0.10f;     // -10 레벨 이하: 10% XP (폭업 방지)
        
        // Sweet spot combo system (연속 처치 보너스)
        public const int SWEETSPOT_COMBO_THRESHOLD = 5;           // 5킬 이상
        public const float SWEETSPOT_COMBO_MAX_BONUS = 1.5f;      // 최대 1.5배
        
        // Level cap proximity bonuses (레벨 캡 근접 보정)
        public const int LEVEL_CAP_CATCHUP_RANGE = 5;             // 캡에서 5 이하일 때 따라잡기 보너스
        public const float LEVEL_CAP_CATCHUP_BONUS = 1.2f;        // 120% XP
        public const int LEVEL_CAP_THROTTLE_RANGE = 3;            // 캡 -3 이내에서 감소 시작
        public const float LEVEL_CAP_THROTTLE_MULT = 0.8f;        // 80% XP
        
        // Party level difference (파티 플레이 레벨 차이 보정)
        public const int PARTY_LEVEL_DIFF_THRESHOLD = 5;          // 5 레벨 차이부터 보정
        public const float PARTY_HIGH_LEVEL_MULT = 0.7f;          // 높은 레벨: 70% XP
        public const float PARTY_LOW_LEVEL_MULT = 1.3f;           // 낮은 레벨: 130% XP (캐치업)

        #endregion

        #region Stat System

        // Stat points per level
        public const int STAT_POINTS_PER_LEVEL = 5;   // Manual allocation per level
        public const int SKILL_POINTS_PER_LEVEL = 3;  // Default fallback (tier-based values below)
        
        public const int NOVICE_STAT_POINTS = 5;
        public const int TIER1_STAT_POINTS = 5;
        public const int TIER2_STAT_POINTS = 5;
        public const int TIER3_STAT_POINTS = 5;

        // 12-Stat System Effects
        public const float STRENGTH_MELEE_DAMAGE_PER_POINT = 0.01f;         // 1% melee damage
        public const float DEXTERITY_RANGED_DAMAGE_PER_POINT = 0.01f;      // 1% ranged damage
        public const float DEXTERITY_ATTACK_SPEED_PER_POINT = 0.003f;      // 0.3% attack speed
        public const float DEXTERITY_RANGED_CRIT_PER_POINT = 0.003f;       // 0.3% ranged crit
        public const float ROGUE_FINESSE_DAMAGE_PER_POINT = 0.008f;        // 0.8% melee/ranged damage
        public const float ROGUE_CRIT_CHANCE_PER_POINT = 0.003f;           // 0.3% crit chance
        public const float INTELLIGENCE_MAGIC_DAMAGE_PER_POINT = 0.015f;   // 1.5% magic damage
        public const float INTELLIGENCE_MAGIC_CRIT_PER_POINT = 0.007f;     // 0.7% magic crit
        public const float INTELLIGENCE_SPELL_POWER_PER_POINT = 0.005f;    // 0.5% spell power
        public const float INTELLIGENCE_MANA_COST_REDUCTION_PER_POINT = 0.002f; // 0.2% mana cost reduction
        public const float INTELLIGENCE_MAX_MANA_COST_REDUCTION = 0.5f;    // 50% max
        public const int VITALITY_HP_PER_POINT = 5;                        // +5 HP
        public const float VITALITY_HP_REGEN_PER_POINT = 0.02f;            // +0.02 HP/sec
        public const int STAMINA_MAX_PER_POINT = 2;                        // +2 max stamina
        public const float STAMINA_REGEN_PER_POINT = 0.05f;                // +0.05 stamina/sec
        public const float DEFENSE_DAMAGE_REDUCTION_PER_POINT = 0.003f;    // 0.3% damage reduction
        public const int DEFENSE_ARMOR_PER_5_POINTS = 1;                   // +1 armor per 5 points
        public const float AGILITY_MOVEMENT_SPEED_PER_POINT = 0.003f;      // 0.3% move speed
        public const float AGILITY_DODGE_PER_POINT = 0.002f;               // 0.2% dodge
        public const float AGILITY_MAX_MOVEMENT_SPEED = 0.5f;              // 50% max
        public const float AGILITY_MAX_DODGE = 0.35f;                      // 35% max dodge
        public const int WISDOM_MANA_PER_POINT = 5;                        // +5 mana
        public const float WISDOM_MANA_REGEN_PER_POINT = 0.03f;            // +0.03 mana/sec
        public const float FOCUS_SUMMON_DAMAGE_PER_POINT = 0.012f;         // 1.2% summon damage
        public const int FOCUS_MINION_SLOT_BASE_POINTS = 10;               // First slot at 10 FOC
        public const int FOCUS_MINION_SLOT_STEP_POINTS = 10;               // +10 per additional slot step
        public const float FORTITUDE_STATUS_RESISTANCE_PER_POINT = 0.005f; // 0.5% status resist
        public const float FORTITUDE_KNOCKBACK_RESIST_PER_POINT = 0.003f;  // 0.3% knockback resist
        public const float FORTITUDE_DEFENSE_PER_POINT = 0.002f;           // 0.2% defense
        public const float LUCK_CRIT_PER_POINT = 0.005f;                   // 0.5% crit
        public const float LUCK_DROP_RATE_PER_POINT = 0.002f;              // 0.2% drop rate
        public const float LUCK_ALL_DAMAGE_PER_POINT = 0.002f;             // 0.2% all damage

        // Auto-growth ratios (30~40% auto, 60~70% manual)
        public const float AUTO_GROWTH_RATIO = 0.35f;  // 35% from auto-growth
        public const float MANUAL_GROWTH_RATIO = 0.65f; // 65% from manual

        // Class-specific stat efficiency multipliers
        public const float PRIMARY_STAT_EFFICIENCY = 1.5f;          // Main stat for class
        public const float SECONDARY_STAT_EFFICIENCY = 1.0f;        // Other stats

        #endregion

        #region Skill System

        // Skill points per level
        public const int NOVICE_SKILL_POINTS = 6;
        public const int TIER1_SKILL_POINTS = 4;
        public const int TIER2_SKILL_POINTS = 3;
        public const int TIER3_SKILL_POINTS = 3;

        // Skill slot limits
        public const int TIER1_PASSIVE_SLOTS = 3;
        public const int TIER1_ACTIVE_SLOTS = 2;
        public const int TIER2_PASSIVE_SLOTS = 5;
        public const int TIER2_ACTIVE_SLOTS = 4;
        public const int TIER3_PASSIVE_SLOTS = 7;
        public const int TIER3_ACTIVE_SLOTS = 6;

        // Cooldown reduction cap
        public const float MAX_COOLDOWN_REDUCTION = 0.8f; // 80% max CDR

        #endregion

        #region Resources

        // Stamina (physical skills)
        public const int BASE_MAX_STAMINA = 100;
        public const float BASE_STAMINA_REGEN = 6f; // per second
        public const float STAMINA_REGEN_DELAY = 1f; // seconds after use

        // Mana (magic skills) - vanilla Terraria handles this
        public const float MANA_REGEN_BOOST_PER_WISDOM = 0.01f;

        #endregion

        #region Job System

        // Class damage bonuses
        public const float PURE_CLASS_DAMAGE_BONUS = 0.2f;          // 20% for pure classes
        public const float HYBRID_CLASS_DAMAGE_BONUS = 0.1f;        // 10% per type for hybrids

        // Job advancement requirements
        public const int FIRST_JOB_LEVEL = 10;
        public const int SECOND_JOB_LEVEL = 60;
        public const int THIRD_JOB_LEVEL = 120;

        #endregion

        #region Auto-Growth Curves (per level)

        // Warrior auto-growth per level
        public static readonly float[] WARRIOR_AUTO_GROWTH = new float[12]
        {
            0.7f,  // Strength (primary)
            0.2f,  // Dexterity
            0.1f,  // Rogue
            0.1f,  // Intelligence
            0.1f,  // Focus
            0.8f,  // Vitality (primary)
            0.4f,  // Stamina
            0.5f,  // Defense
            0.2f,  // Agility
            0.1f,  // Wisdom
            0.4f,  // Fortitude
            0.2f   // Luck
        };

        // Ranger auto-growth per level
        public static readonly float[] RANGER_AUTO_GROWTH = new float[12]
        {
            0.2f,  // Strength
            0.7f,  // Dexterity (primary)
            0.7f,  // Rogue (primary)
            0.0f,  // Intelligence
            0.1f,  // Focus
            0.3f,  // Vitality
            0.5f,  // Stamina
            0.2f,  // Defense
            0.5f,  // Agility
            0.1f,  // Wisdom
            0.1f,  // Fortitude
            0.3f   // Luck
        };

        // Mage auto-growth per level
        public static readonly float[] MAGE_AUTO_GROWTH = new float[12]
        {
            0.1f,  // Strength
            0.1f,  // Dexterity
            0.0f,  // Rogue
            0.9f,  // Intelligence (primary)
            0.1f,  // Focus
            0.3f,  // Vitality
            0.1f,  // Stamina
            0.2f,  // Defense
            0.1f,  // Agility
            0.8f,  // Wisdom (primary)
            0.2f,  // Fortitude
            0.2f   // Luck
        };

        // Summoner auto-growth per level
        public static readonly float[] SUMMONER_AUTO_GROWTH = new float[12]
        {
            0.1f,  // Strength
            0.2f,  // Dexterity
            0.1f,  // Rogue
            0.5f,  // Intelligence
            0.8f,  // Focus
            0.4f,  // Vitality
            0.2f,  // Stamina
            0.2f,  // Defense
            0.2f,  // Agility
            0.3f,  // Wisdom
            0.3f,  // Fortitude
            0.3f   // Luck
        };

        #endregion

        #region XP System

        // Base XP formula: (HP/100) × (1+Def/10) × (1+Dmg/25)
        public const float XP_HP_DIVISOR = 100f;
        public const float XP_DEFENSE_DIVISOR = 10f;
        public const float XP_DAMAGE_DIVISOR = 25f;
        public const int MIN_NPC_XP = 5;

        // World level XP multiplier
        public const float XP_WORLD_LEVEL_MULTIPLIER = 0.1f; // +10% per world level
        public const float XP_MONSTER_LEVEL_MULTIPLIER = 0.01f; // +1% per monster level
        public const float XP_MONSTER_LEVEL_MAX_MULTIPLIER = 2.5f; // Cap to avoid runaway scaling

        // Special case multipliers
        public const float EVENT_XP_MULTIPLIER = 0.5f;       // 50% for event monsters
        public const float STATUE_XP_MULTIPLIER = 0f;        // 0% for statue spawns
        public const float BOSS_XP_MULTIPLIER = 500f;        // Boss level × 500
        public const float BOSS_XP_BONUS_MULTIPLIER = 1f;    // Additional multiplier on boss XP gain

        // Multiplayer
        public const float MULTIPLAYER_XP_SHARE_RADIUS = 5000f; // Tiles

        #endregion

        #region World Level System

        // Monster scaling per world level
        public const float MONSTER_HP_SCALE_PER_LEVEL = 0.03f;      // +3% HP per WL

        // Damage scaling by difficulty
        public const float CLASSIC_DAMAGE_SCALE = 0.008f;           // +0.8% per WL
        public const float EXPERT_DAMAGE_SCALE = 0.006f;            // +0.6% per WL
        public const float MASTER_DAMAGE_SCALE = 0.004f;            // +0.4% per WL
        public const float LEGENDARY_DAMAGE_SCALE = 0.002f;         // +0.2% per WL

        // World level increase per boss
        public const int WL_INCREASE_SMALL = 1;
        public const int WL_INCREASE_MEDIUM = 2;
        public const int WL_INCREASE_LARGE = 3;
        public const int MAX_WORLD_LEVEL = int.MaxValue;

        #endregion

        #region UI Constants

        public const int UI_PADDING = 10;
        public const int UI_ICON_SIZE = 32;
        public const int UI_BUTTON_WIDTH = 100;
        public const int UI_BUTTON_HEIGHT = 30;

        public const float UI_FADE_SPEED = 0.05f;
        public const int UI_ANIMATION_FRAMES = 60;

        #endregion

        #region Asset Paths

        public const string TEXTURE_PATH = "Rpg/Assets/Textures/";
        public const string ICON_PATH = "Rpg/Assets/Icons/";
        public const string PARTICLE_PATH = "Rpg/Assets/Particles/";
        public const string SOUND_PATH = "Rpg/Assets/Sounds/";
        public const string SHADER_PATH = "Rpg/Assets/Shaders/";

        #endregion

        #region Balance Tuning

        // Player damage scaling (prevent one-shotting)
        public const float PLAYER_DAMAGE_CAP_MULTIPLIER = 2.0f; // Max 200% of expected damage

        // Monster HP floor (prevent too weak)
        public const int MONSTER_MIN_HP = 10;

        // Boss HP cap for XP calculation
        public const int BOSS_XP_PLAYER_LEVEL_MULTIPLIER = 1000;

        #endregion

        #region Debug & Testing

        public const bool ENABLE_DEBUG_LOGS = false;
        public const bool ENABLE_CHEAT_COMMANDS = false;
        public const bool SHOW_MONSTER_LEVELS = true;
        public const bool SHOW_XP_NUMBERS = true;
        public static bool DEBUG_SHOW_XP_VALUES = false;

        #endregion
    }
}
