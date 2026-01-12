# Terraria RPG Mod - Complete Design Specification

**Version**: 2.0  
**Last Updated**: January 8, 2026  
**Language**: English  
**Source Documents**: NEW_RPG_DESIGN_PROMPT.md + chatgpt prompt.md (integrated)

---

## 📖 Table of Contents

1. [Overview & Goals](#overview--goals)
2. [Level Cap & Progression System](#level-cap--progression-system)
3. [World Level & Monster Scaling](#world-level--monster-scaling)
4. [Class & Advancement System](#class--advancement-system)
5. [Stats & Skills](#stats--skills)
6. [Experience & Leveling](#experience--leveling)
7. [Mod Compatibility & Balance](#mod-compatibility--balance)
8. [Implementation Priority](#implementation-priority)

---

## Overview & Goals

This mod introduces comprehensive RPG elements to Terraria, featuring:

- **MapleStory-style class advancement** system (Novice → 1st → 2nd → 3rd)
- **4 base damage types**: Melee, Ranged, Magic, **Summon**
- **Boss-based level caps** preventing over-leveling
- **World Level system** scaling monsters with player progression
- **Full progression coverage**: Pre-Hardmode → Hardmode → Post-Moon Lord
- **Universal mod compatibility**: Automatically works with Calamity, Thorium, etc.

### Core Principles

1. **Balanced Growth**: Player power scales with monster difficulty
2. **Meaningful Choices**: Class selection and stat allocation matter
3. **No AI Changes**: Preserve vanilla Terraria feel, only modify stats
4. **Exploit Prevention**: Handle segmented bosses, events, statue spawns
5. **Multiplayer Support**: XP sharing within 5000 tiles

---

## Level Cap & Progression System

### Boss-Progression Level Caps

**Base System**: Level cap increases ONLY when defeating bosses, preventing farming abuse.

```csharp
public static int GetMaxLevel()
{
    int maxLevel = 8; // Starting cap
    
    // Pre-Hardmode
    if (NPC.downedSlimeKing) maxLevel = Math.Max(maxLevel, 10);     // King Slime: +2
    if (NPC.downedBoss1) maxLevel = Math.Max(maxLevel, 15);         // Eye of Cthulhu: +5
    if (NPC.downedBoss2) maxLevel = Math.Max(maxLevel, 25);         // Eater/Brain: +10
    if (NPC.downedQueenBee) maxLevel = Math.Max(maxLevel, 30);      // Queen Bee: +5
    if (NPC.downedBoss3) maxLevel = Math.Max(maxLevel, 40);         // Skeletron: +10
    if (NPC.downedDeerclops) maxLevel = Math.Max(maxLevel, 45);     // Deerclops: +5
    
    // Wall of Flesh = Hardmode Entry
    if (Main.hardMode) maxLevel = Math.Max(maxLevel, 60);           // Wall of Flesh: Lv.50 → Lv.60
    
    // Hardmode
    if (NPC.downedQueenSlime) maxLevel = Math.Max(maxLevel, 65);    // Queen Slime: +5
    if (NPC.downedMechBoss1) maxLevel += 10;                        // Any Mech Boss: +10
    if (NPC.downedMechBoss2) maxLevel += 10;                        // Second Mech: +10
    if (NPC.downedMechBoss3) maxLevel += 10;                        // Third Mech: +10
                                                                    // Total after Mechs: Lv.95
    if (NPC.downedPlantBoss) maxLevel += 10;                        // Plantera: +10 (Lv.105)
    if (NPC.downedGolemBoss) maxLevel += 5;                         // Golem: +5 (Lv.110)
    if (NPC.downedFishron) maxLevel += 5;                           // Duke Fishron: +5 (Lv.115)
    if (NPC.downedEmpressOfLight) maxLevel += 5;                    // Empress: +5 (Lv.120)
    
    // Post-Moon Lord: UNLIMITED
    if (NPC.downedMoonlord) maxLevel = int.MaxValue;                // Moon Lord: No cap
    
    return maxLevel;
}
```

### Level Ranges by Progression Stage

| Stage | Level Range | Purpose | Notes |
|-------|-------------|---------|-------|
| **Novice** | 1-10 | Tutorial, learn basics | Fast growth |
| **1st Job** | 10-60 | Pre-HM → Early HM | Medium growth |
| **2nd Job** | 60-120 | Hardmode main content | Slow growth |
| **3rd Job** | 120-150+ | Post-Moon Lord | Very slow growth |
| **Endgame** | 150+ | Unlimited | Endless scaling |

### Pre-Hardmode (Lv.1-50)

- **Max Level**: 50 (before Wall of Flesh)
- **Boss Progression**:
  - Start: Lv.8 cap
  - King Slime: Lv.10
  - Eye of Cthulhu: Lv.15
  - Eater/Brain: Lv.25
  - Queen Bee: Lv.30
  - Skeletron: Lv.40
  - Deerclops: Lv.45
  - Wall of Flesh: Lv.50 → **Hardmode Entry** (Lv.60)

### Hardmode (Lv.60-120)

- **Initial Cap**: 60 (prevents instant high-level on HM entry)
- **Mechanical Bosses**: +10 each (total +30 to Lv.95)
- **Plantera**: +10 (Lv.105)
- **Golem**: +5 (Lv.110)
- **Duke Fishron**: +5 (Lv.115)
- **Empress of Light**: +5 (Lv.120)

### Post-Moon Lord (Lv.120+)

- **Level Cap**: REMOVED (unlimited growth)
- **World Level**: Continues scaling infinitely
- **Mod Boss Support**: Calamity/Thorium endgame bosses add more levels

---

## World Level & Monster Scaling

### World Level System

**World Level (WL)** = Global difficulty indicator that increases with boss kills.

#### WL Increase Triggers

```csharp
public static void OnBossKilled(int bossType)
{
    int wlIncrease = 0;
    
    // Pre-Hardmode bosses: +1 or +2
    if (bossType == NPCID.KingSlime) wlIncrease = 1;
    if (bossType == NPCID.EyeofCthulhu) wlIncrease = 1;
    if (bossType == NPCID.EaterofWorldsHead || bossType == NPCID.BrainofCthulhu) wlIncrease = 2;
    // ... etc
    
    // Hardmode bosses: +2 or +3
    if (bossType == NPCID.TheDestroyer) wlIncrease = 2;
    if (bossType == NPCID.MoonLordCore) wlIncrease = 3;
    
    ModContent.GetInstance<RpgWorld>().worldLevel += wlIncrease;
}
```

### Monster Scaling Formula

**DO NOT TOUCH DEFENSE** - causes calculation errors!

```csharp
public void ScaleMonster(NPC npc, int worldLevel)
{
    // HP Scaling: +3% per world level
    float hpMultiplier = 1 + (worldLevel * 0.03f);
    npc.lifeMax = (int)(npc.lifeMax * hpMultiplier);
    npc.life = npc.lifeMax;
    
    // Damage Scaling: Varies by difficulty
    float damageMultiplier = 1 + (worldLevel * GetDamageScaleRate());
    npc.damage = (int)(npc.damage * damageMultiplier);
    
    // DEFENSE: DO NOT SCALE (or only +1 per 10 WL if absolutely necessary)
    // npc.defense = npc.defense; // Leave unchanged!
}

float GetDamageScaleRate()
{
    if (Main.masterMode) return 0.002f;      // +0.2%/level (slowest)
    if (Main.expertMode) return 0.004f;      // +0.4%/level
    if (Main.getGoodWorld) return 0.006f;    // +0.6%/level
    return 0.008f;                            // +0.8%/level (Classic, fastest)
}
```

### Monster Level Display

```csharp
public override void ModifyHitByItem(NPC npc, Player player, Item item, ref int damage, ref float knockback, ref bool crit)
{
    int monsterLevel = GetMonsterLevel(npc);
    int worldLevel = ModContent.GetInstance<RpgWorld>().worldLevel;
    
    // Display: "[Lv.15] Green Slime" with color-coded name
    string levelPrefix = $"[Lv.{monsterLevel}] ";
    // Color: White (normal), Yellow (dangerous), Red (deadly)
}
```

### Biome-Specific Scaling

```csharp
int GetMonsterLevel(NPC npc)
{
    int baseLevel = GetNPCBaseLevel(npc.type); // 1-10 depending on NPC
    int worldLevel = ModContent.GetInstance<RpgWorld>().worldLevel;
    int biomeBonus = GetBiomeBonus(npc);
    
    // Dungeon/Jungle/Hell: +5 levels
    // Surface: +0
    // Underground: +2
    
    return Math.Min(baseLevel + worldLevel + biomeBonus, GetBiomeLevelCap(npc));
}

int GetBiomeLevelCap(NPC npc)
{
    // Prevent early-game mobs from becoming endgame threats
    if (IsPreHardmodeMonster(npc)) return Main.hardMode ? 50 : int.MaxValue;
    return int.MaxValue;
}
```

---

## Class & Advancement System

### Class Tree Structure

```
Novice (Lv.1-10)
  ↓
1st Job (Lv.10-60) [4 classes]
├── Warrior (Melee)
├── Ranger (Ranged)
├── Mage (Magic)
└── Summoner (Summon)
  ↓
2nd Job (Lv.60-120) [2 branches per 1st job = 8 total]
├── Warrior → Knight (Tank) / Berserker (DPS)
├── Ranger → Sniper (Crit) / Gunslinger (Multi-hit)
├── Mage → Sorcerer (Elemental) / Cleric (Support)
└── Summoner → Beastmaster (Pets) / Necromancer (Undead)
  ↓
3rd Job (Lv.120+) [1 ultimate form per 2nd job = 8 total]
├── Knight → Guardian (Ultimate Tank)
├── Berserker → Blood Knight (Life-drain Burst DPS)
├── Sorcerer → Archmage (AoE Nuke)
├── Cleric → Archbishop (Mass Heal + Buffs)
└── ... etc
```

### Hybrid Classes (Optional 2nd Job Branch)

**Concept**: Combine two damage types at +10% each vs Pure +20% single type.

```csharp
// Example: Paladin = Warrior + Cleric hybrid
public class Paladin : ModClass
{
    public override void ModifyDamage(Player player, ref float meleeDamage, ref float healPower)
    {
        meleeDamage *= 1.1f;  // 10% melee boost (vs Warrior's 20%)
        healPower *= 1.1f;    // 10% heal boost (vs Cleric's 20%)
        
        // Total expected value: 1.1 * 2 = 2.2 vs Pure 1.2 * 1 = 1.2
        // Balanced if using BOTH weapon types equally
    }
}
```

**Hybrid Examples**:
- **Paladin**: Warrior + Cleric (Melee + Healing)
- **Spellblade**: Warrior + Mage (Melee + Magic)
- **Shadow**: Ranger + Warrior (Ranged + Melee stealth)
- **Druid**: Mage + Summoner (Magic + Nature summons)

### Job Advancement Requirements

| Tier | Level Req | Boss Req | Additional |
|------|-----------|----------|------------|
| Novice → 1st | Lv.10 | Any early boss (Slime/Eye/Eater/Brain) | - |
| 1st → 2nd | Lv.50-60 | Hardmode entry + 1 Mech boss | Class-specific quest item |
| 2nd → 3rd | Lv.120 | Moon Lord defeated | Master challenge quest |

### Job Change Mechanics

```csharp
public void ChangeJob(Player player, JobType newJob)
{
    if (!MeetsRequirements(player, newJob)) return;
    
    // Grant stat bonuses
    if (newJob == JobType.Warrior_1st)
    {
        player.GetModPlayer<RpgPlayer>().bonusSTR += 10;
        player.GetModPlayer<RpgPlayer>().bonusVIT += 20;
        // Unlock 1st job skill tree
    }
    
    // Permanent choice (no respec except endgame mechanic)
    player.GetModPlayer<RpgPlayer>().currentJob = newJob;
    player.GetModPlayer<RpgPlayer>().canChangeJob = false;
}
```

---

## Stats & Skills

### 6-Stat System (Added Spirit for Summoner)

| Stat | Abbr | Primary Effect | Secondary Effect |
|------|------|----------------|------------------|
| **Strength** | STR | Melee Damage +1%/pt | Minor throwing boost |
| **Dexterity** | DEX | Ranged Damage +1%/pt<br>Crit +0.5%/10pt | Minor melee boost<br>Movement +0.5%/10pt |
| **Intelligence** | INT | Magic Damage +1.5%/pt<br>Mana +5/pt | Mana Regen +0.2/5pt |
| **Spirit** | SPI | **Summon Damage +1%/pt**<br>**Minion Slots +1/20pt** | Minor magic boost |
| **Vitality** | VIT | HP +5/pt<br>Stamina +5/2pt | Defense +2/5pt<br>HP Regen boost |
| **Luck** | LUK | All Damage +0.2%/pt<br>Crit +0.5%/pt | Drop Rate +1%/10pt |

### Stat Points per Level

```csharp
public int GetStatPointsPerLevel(int level, JobTier tier)
{
    if (tier == JobTier.Tier1) return 6;  // 1st job: generous
    if (tier == JobTier.Tier2) return 4;  // 2nd job: moderate
    if (tier == JobTier.Tier3) return 3;  // 3rd job: focused
    return 5; // Novice
}
```

### Class-Specific Stat Efficiency

```csharp
float GetStatEfficiency(Player player, StatType stat)
{
    JobType job = player.GetModPlayer<RpgPlayer>().currentJob;
    
    // Warrior: STR efficiency 1.5x, others 1.0x
    if (job == JobType.Warrior && stat == StatType.STR) return 1.5f;
    
    // Mage: INT efficiency 1.5x, others 1.0x
    if (job == JobType.Mage && stat == StatType.INT) return 1.5f;
    
    // Summoner: SPI efficiency 1.5x, others 1.0x
    if (job == JobType.Summoner && stat == StatType.SPI) return 1.5f;
    
    return 1.0f; // Default efficiency
}
```

### Skill System

#### Skill Points

```csharp
public int GetSkillPointsPerLevel(JobTier tier)
{
    return tier switch
    {
        JobTier.Tier1 => 2,  // 1st job: 2 SP/level
        JobTier.Tier2 => 2,  // 2nd job: 2 SP/level
        JobTier.Tier3 => 3,  // 3rd job: 3 SP/level
        _ => 1               // Novice: 1 SP/level
    };
}
```

#### Active vs Passive Skills

**Passive Examples**:
- Warrior: "Iron Skin" (Max HP +20%, Defense +5)
- Ranger: "Eagle Eye" (Crit +10%, Accuracy +15%)
- Mage: "Arcane Wisdom" (Mana +50, Mana Regen +30%)
- Summoner: "Legion Commander" (Minion Slots +1)

**Active Examples**:
- Warrior-Berserker: "Frenzy" (15s: +50% ATK, -20% DEF)
- Mage-Archmage: "Meteor" (Massive AoE nuke)
- Summoner-Necromancer: "Army of the Dead" (Summon 10 skeletal warriors)

#### Skill Slots

| Tier | Passive Slots | Active Slots | Total Skills |
|------|---------------|--------------|--------------|
| Tier 1 | 3 | 2 | 5 |
| Tier 2 | 5 | 3 | 8 |
| Tier 3 | 7 | 4 | 11 |

---

## Experience & Leveling

### XP Formula (Precise from chatgpt prompt)

```csharp
int CalculateBaseXP(NPC npc)
{
    // Formula: (HP/100) × (1+Defense/10) × (1+Damage/25)
    float hpFactor = npc.lifeMax / 100f;
    float defFactor = 1 + (npc.defense / 10f);
    float dmgFactor = 1 + (npc.damage / 25f);
    
    int baseXP = (int)(hpFactor * defFactor * dmgFactor);
    
    // World Level multiplier
    int worldLevel = ModContent.GetInstance<RpgWorld>().worldLevel;
    float worldMultiplier = 1 + (worldLevel * 0.1f);
    
    return (int)(baseXP * worldMultiplier);
}
```

### Boss XP

```csharp
int CalculateBossXP(NPC boss)
{
    int bossLevel = GetBossLevel(boss.type);
    int baseXP = bossLevel * 500;
    
    // Cap to prevent explosion (max = player avg level * 1000)
    int avgPlayerLevel = GetAveragePlayerLevel();
    int maxXP = avgPlayerLevel * 1000;
    
    return Math.Min(baseXP, maxXP);
}
```

### CRITICAL: Special Case Handling

#### 1. Segmented Bosses (Eater of Worlds, Destroyer)

**Problem**: Each body segment is separate NPC → 50x XP exploit!

```csharp
public override void OnKill(NPC npc)
{
    // ONLY give XP for HEAD, ignore all body/tail segments
    if (npc.type == NPCID.EaterofWorldsBody || 
        npc.type == NPCID.EaterofWorldsTail ||
        npc.type == NPCID.TheDestroyerBody ||
        npc.type == NPCID.TheDestroyerTail)
    {
        return; // NO XP for segments!
    }
    
    // HEAD gives full XP once
    if (npc.type == NPCID.EaterofWorldsHead ||
        npc.type == NPCID.TheDestroyerHead)
    {
        int fullBossXP = CalculateBossXP(npc);
        GiveExperience(fullBossXP);
    }
}
```

#### 2. Event Monsters (Blood Moon, Invasions, Pumpkin Moon, etc.)

**Problem**: Massive XP farming potential!

```csharp
float GetEventXPMultiplier(NPC npc)
{
    // 50% XP reduction for event spawns
    if (Main.bloodMoon || Main.eclipse || 
        Main.pumpkinMoon || Main.snowMoon ||
        Main.invasionType > 0) // Goblin, Pirate, etc.
    {
        return 0.5f; // 50% penalty
    }
    
    return 1.0f; // Normal XP
}

int FinalXP = CalculateBaseXP(npc) * GetEventXPMultiplier(npc);
```

#### 3. Statue-Spawned Monsters

**Problem**: Infinite farming with statue-spawned mobs!

```csharp
public override void OnKill(NPC npc)
{
    // Check statue spawn flag
    if (npc.SpawnedFromStatue)
    {
        return; // ZERO XP for statue spawns
    }
    
    // Normal XP calculation...
}
```

### Multiplayer XP Sharing

```csharp
public void DistributeXP(int totalXP, Vector2 killPosition)
{
    const float SHARE_RADIUS = 5000f; // 5000 tiles
    
    foreach (Player player in Main.player)
    {
        if (!player.active) continue;
        
        float distance = Vector2.Distance(player.Center, killPosition);
        if (distance <= SHARE_RADIUS)
        {
            // FULL XP for everyone in range (not divided)
            player.GetModPlayer<RpgPlayer>().GainExperience(totalXP);
        }
    }
}
```

### Level-Up XP Requirements

```csharp
int GetRequiredXP(int currentLevel)
{
    // Exponential curve with tier-based scaling
    if (currentLevel < 10) // Novice: fast
        return (int)(100 * Math.Pow(1.5, currentLevel - 1));
    
    if (currentLevel < 60) // 1st job: moderate
        return 2000 + (int)(500 * Math.Pow(1.3, currentLevel - 10));
    
    if (currentLevel < 120) // 2nd job: slow
        return 50000 + (int)(5000 * Math.Pow(1.25, currentLevel - 60));
    
    // 3rd job+: very slow
    return 500000 + (int)(50000 * Math.Pow(1.2, currentLevel - 120));
}
```

---

## Mod Compatibility & Balance

### Automatic Damage Type Detection

```csharp
public void ApplyClassBonus(Player player, Item item)
{
    RpgPlayer rpgPlayer = player.GetModPlayer<RpgPlayer>();
    
    // Auto-detect damage type
    if (item.CountsAsClass(DamageClass.Melee))
    {
        if (rpgPlayer.currentJob == JobType.Warrior)
            item.damage = (int)(item.damage * 1.2f); // 20% boost
    }
    else if (item.CountsAsClass(DamageClass.Ranged))
    {
        if (rpgPlayer.currentJob == JobType.Ranger)
            item.damage = (int)(item.damage * 1.2f);
    }
    // ... etc for Magic, Summon
    
    // Handle mod-specific damage types
    if (ModLoader.TryGetMod("ThoriumMod", out Mod thorium))
    {
        // Check for Bard/Healer damage types
        // Fallback: Bard→Ranged, Healer→Magic
    }
}
```

### Calamity/Thorium Compatibility

```csharp
public void DetectModBosses()
{
    // Calamity Revengeance/Death mode detection
    if (ModLoader.TryGetMod("CalamityMod", out Mod calamity))
    {
        bool revengeanceMode = (bool)calamity.Call("GetDifficultyActive", "revengeance");
        if (revengeanceMode)
        {
            // Reduce monster HP scaling to compensate
            hpScaleMultiplier *= 0.8f;
        }
    }
    
    // Thorium boss detection
    if (ModLoader.TryGetMod("ThoriumMod", out Mod thorium))
    {
        // Add Thorium bosses to level cap progression
        // CheckCustomBossFlags(thorium);
    }
}
```

### Difficulty Scaling Adjustments

```csharp
float GetDifficultyDamageMultiplier()
{
    if (Main.masterMode) return 0.002f;      // Master: +0.2%/level (slowest)
    if (Main.expertMode) return 0.004f;      // Expert: +0.4%/level
    if (Main.getGoodWorld) return 0.006f;    // For the Worthy: +0.6%/level
    return 0.008f;                            // Classic: +0.8%/level (fastest)
}

float GetDifficultyHPMultiplier()
{
    if (Main.masterMode) return 0.05f;       // Master: +5%/level
    if (Main.expertMode) return 0.04f;       // Expert: +4%/level
    if (Main.getGoodWorld) return 0.06f;     // For the Worthy: +6%/level
    return 0.03f;                             // Classic: +3%/level
}
```

---

## Implementation Priority

### Phase 1: Core Foundation (Week 1-2)

**Files to Create**:
1. `Common/RpgConstants.cs` - All constants (XP formulas, scaling rates, etc.)
2. `Common/RpgEnums.cs` - JobType, StatType, SkillType enums
3. `Common/RpgFormulas.cs` - XP calculation, stat effects, scaling formulas
4. `Common/Players/RpgPlayer.cs` - Player data (level, XP, stats, job)
5. `Common/Players/PlayerLevel.cs` - Level-up logic, XP gain, cap checks

**Critical Implementation**:
```csharp
// RpgPlayer.cs
public class RpgPlayer : ModPlayer
{
    public int level = 1;
    public long currentXP = 0;
    public JobType currentJob = JobType.Novice;
    
    public int STR, DEX, INT, SPI, VIT, LUK; // 6 stats
    public int statPoints = 0;
    public int skillPoints = 0;
    
    public override void PostUpdate()
    {
        // Check level cap
        int maxLevel = RpgFormulas.GetMaxLevel();
        if (level >= maxLevel)
        {
            currentXP = Math.Min(currentXP, GetRequiredXP(level) - 1);
        }
    }
}
```

### Phase 2: World & NPC Systems (Week 3-4)

**Files to Create**:
1. `Common/Systems/RpgWorld.cs` - World Level tracking
2. `Common/Systems/WorldProgression.cs` - Boss kill detection
3. `Common/NPCs/RpgGlobalNPC.cs` - Monster scaling, XP drops

**Critical Implementation**:
```csharp
// RpgGlobalNPC.cs
public override void OnKill(NPC npc)
{
    // HANDLE ALL SPECIAL CASES
    
    // 1. Statue spawns → 0 XP
    if (npc.SpawnedFromStatue) return;
    
    // 2. Segmented bosses → Only head gives XP
    if (IsBodySegment(npc.type)) return;
    
    // 3. Event monsters → 50% XP
    float eventMultiplier = GetEventXPMultiplier();
    
    // 4. Calculate & distribute XP
    int baseXP = RpgFormulas.CalculateBaseXP(npc);
    int finalXP = (int)(baseXP * eventMultiplier);
    
    DistributeXPToNearbyPlayers(finalXP, npc.Center);
}
```

### Phase 3: Job System (Week 5-6)

**Files to Create**:
1. `Content/Jobs/JobDatabase.cs` - All job definitions
2. `Content/Jobs/JobManager.cs` - Job change logic, requirements
3. `Content/Skills/SkillDatabase.cs` - Skill definitions
4. `Content/Skills/SkillManager.cs` - Skill activation, cooldowns

### Phase 4: UI & Polish (Week 7-8)

**Files to Create**:
1. `UI/RpgHUD.cs` - Level, XP bar, World Level display
2. `UI/StatAllocationUI.cs` - Stat point distribution
3. `UI/JobTreeUI.cs` - Class advancement interface
4. `UI/SkillTreeUI.cs` - Skill learning interface

---

## Test Checklist

### Level Cap Tests
- [ ] Level 8 cap without any bosses killed
- [ ] Level 10 after King Slime
- [ ] Level 15 after Eye of Cthulhu
- [ ] Level 60 after Wall of Flesh (Hardmode entry)
- [ ] Level 95 after all 3 Mechanical Bosses
- [ ] Level 120 after Empress of Light
- [ ] Unlimited after Moon Lord

### XP Special Cases
- [ ] Eater of Worlds: Only HEAD gives XP, segments give 0
- [ ] The Destroyer: Only HEAD gives XP, segments give 0
- [ ] Blood Moon monsters: 50% XP
- [ ] Goblin Invasion: 50% XP
- [ ] Statue-spawned slimes: 0 XP
- [ ] Normal slimes in same area: Full XP

### Multiplayer
- [ ] Player A kills mob, Player B within 5000 tiles gets full XP
- [ ] Player C beyond 5000 tiles gets no XP
- [ ] Both get same amount (not divided)

### Mod Compatibility
- [ ] Calamity weapon auto-detected as correct damage type
- [ ] Thorium Bard weapon treated as Ranged fallback
- [ ] No errors with vanilla-only playthrough
- [ ] No errors with Calamity + Thorium together

---

## Quick Reference: Key Boss Names

**Correct English Names** (for code comments):

- **King Slime** (not "Slime King")
- **Eye of Cthulhu** (not "Evil Eye")
- **Eater of Worlds** (not "World Eater")
- **Brain of Cthulhu**
- **Queen Bee**
- **Skeletron**
- **Deerclops**
- **Wall of Flesh**
- **Queen Slime**
- **The Twins** (Retinazer + Spazmatism)
- **The Destroyer**
- **Skeletron Prime**
- **Plantera**
- **Golem**
- **Duke Fishron**
- **Empress of Light**
- **Lunatic Cultist**
- **Moon Lord**

---

**Document End** | Version 2.0 | English Complete
