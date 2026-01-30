namespace Rpg.Common
{
    /// <summary>
    /// All job tiers in progression order
    /// </summary>
    public enum JobTier
    {
        Novice,     // Tier 0: Lv.1-10
        Tier1,      // 1st job: Lv.10-60
        Tier2,      // 2nd job: Lv.60-120
        Tier3       // 3rd job: Lv.120+
    }

    /// <summary>
    /// Base job types (1st advancement)
    /// </summary>
    public enum JobType
    {
        None,

        // Tier 0
        Novice,

        // Tier 1 - Pure Classes (4)
        Warrior,        // Melee
        Ranger,         // Ranged
        Mage,           // Magic
        Summoner,       // Summon

        // Tier 2 - Warrior branches
        Knight,         // Tank
        Berserker,      // DPS
        Paladin,        // Warrior + Cleric
        DeathKnight,    // Dark melee

        // Tier 2 - Ranger branches
        Sniper,         // Crit specialist
        Gunslinger,     // Multi-hit specialist
        Assassin,       // Stealth crit

        // Tier 2 - Mage branches
        Sorcerer,       // Elemental DPS
        Cleric,         // Support/Heal
        Archmage,       // Arcane mastery
        Warlock,        // Dark mage
        Spellblade,     // Warrior + Mage
        BattleMage,     // Tanky mage

        // Tier 2 - Summoner branches
        Beastmaster,    // Pet summons
        Necromancer,    // Undead summons
        Druid,          // Nature mage

        // Tier 2 - Hybrid Classes (Optional)
        Shadow,         // Ranger + Warrior
        Spellthief,     // Ranger + Mage

        // Tier 3 - Ultimate Forms
        Guardian,       // Knight ultimate
        BloodKnight,    // Berserker ultimate
        Deadeye,        // Sniper ultimate
        Gunmaster,      // Gunslinger ultimate
        Archbishop,     // Cleric ultimate
        Overlord,       // Beastmaster ultimate
        LichKing        // Necromancer ultimate
    }

    /// <summary>
    /// Stat types for character customization
    /// </summary>
    public enum StatType
    {
        Strength,       // Melee damage
        Dexterity,      // Ranged damage, attack speed
        Rogue,          // Finesse damage, crit chance
        Intelligence,   // Magic damage
        Focus,          // Summon damage, minion slots
        Vitality,       // HP, regen
        Stamina,        // Physical resource
        Defense,        // Damage reduction
        Agility,        // Movement, dodge
        Wisdom,         // Mana, summon support
        Fortitude,      // Status resist
        Luck            // Crit, drop rate, luck
    }

    /// <summary>
    /// Skill categories
    /// </summary>
    public enum SkillType
    {
        Passive,        // Always active
        Active,         // Player-triggered
        Buff,           // Self/party buff
        Debuff,         // Enemy debuff
        Movement,       // Movement/dash skills
        Utility,        // Utility skills (teleport, summon, etc.)
        Ultimate        // High-cost powerful skill
    }

    /// <summary>
    /// Skill elements for damage calculation
    /// </summary>
    public enum ElementType
    {
        None,
        Physical,
        Fire,
        Ice,
        Lightning,
        Holy,
        Dark,
        Nature,
        Arcane
    }

    /// <summary>
    /// Particle effect types for reusability
    /// </summary>
    public enum ParticleType
    {
        Sparkle,
        Smoke,
        Fire,
        Ice,
        Lightning,
        Blood,
        Holy,
        Dark,
        Heal,
        LevelUp,
        Critical
    }

    /// <summary>
    /// UI anchor positions
    /// </summary>
    public enum AnchorType
    {
        TopLeft,
        TopCenter,
        TopRight,
        CenterLeft,
        Center,
        CenterRight,
        BottomLeft,
        BottomCenter,
        BottomRight
    }

    /// <summary>
    /// XP gain sources for tracking
    /// </summary>
    public enum XPSource
    {
        Monster,
        Boss,
        Quest,
        Event,
        Discovery
    }
}
