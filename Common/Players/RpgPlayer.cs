using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.DataStructures;
using RpgMod.Common.Skills;
using RpgMod.Common.Compatibility;
using RpgMod.Common.Systems;
using RpgMod.Common.Config;

namespace RpgMod.Common.Players
{
    /// <summary>
    /// Core RPG player data - all stats, levels, XP, jobs stored here
    /// This is the central data hub for the entire mod
    /// </summary>
    public class RpgPlayer : ModPlayer
    {
        #region Level & XP

        public int Level { get; set; } = 1;
        public long CurrentXP { get; set; } = 0;
        public long RequiredXP 
        {
            get
            {
                long baseXP = RpgFormulas.GetRequiredXP(Level);
                // Increase requirement for higher levels
                if (Level >= 100) return (long)(baseXP * 1.5f);
                if (Level >= 50) return (long)(baseXP * 1.2f);
                return baseXP;
            }
        }

        #endregion

        #region Job System

        public JobType CurrentJob { get; set; } = JobType.Novice;
        public JobTier CurrentTier => RpgFormulas.GetJobTier(CurrentJob);
        
        // Track selected jobs per tier for proper cancellation
        public Dictionary<JobTier, JobType> SelectedJobs { get; set; } = new Dictionary<JobTier, JobType>();

        #endregion

        #region Stats (12-stat system)

        // Base stats (manual allocation)
        public int Strength { get; set; } = 0;      // Physical attack
        public int Dexterity { get; set; } = 0;     // Ranged, attack speed
        public int Rogue { get; set; } = 0;         // Finesse damage, crit chance
        public int Intelligence { get; set; } = 0;  // Magic attack
        public int Focus { get; set; } = 0;         // Summon damage, minion slots
        public int Vitality { get; set; } = 0;      // HP, HP regen
        public int StaminaStat { get; set; } = 0;   // Stamina resource
        public int Defense { get; set; } = 0;       // Damage reduction
        public int Agility { get; set; } = 0;       // Move speed, dodge
        public int Wisdom { get; set; } = 0;        // Mana, mana regen
        public int Fortitude { get; set; } = 0;     // Status resist
        public int Luck { get; set; } = 0;          // Crit, luck

        public int StatPoints { get; set; } = 0;    // Unspent stat points
        public int PendingStatPoints { get; set; } = 0;

        // Auto-growth stats (separate from manual allocation)
        public int AutoStrength { get; set; } = 0;
        public int AutoDexterity { get; set; } = 0;
        public int AutoRogue { get; set; } = 0;
        public int AutoIntelligence { get; set; } = 0;
        public int AutoFocus { get; set; } = 0;
        public int AutoVitality { get; set; } = 0;
        public int AutoStamina { get; set; } = 0;
        public int AutoDefense { get; set; } = 0;
        public int AutoAgility { get; set; } = 0;
        public int AutoWisdom { get; set; } = 0;
        public int AutoFortitude { get; set; } = 0;
        public int AutoLuck { get; set; } = 0;

        #endregion

        #region Auto Growth

        private float[] autoGrowthRemainders;

        #endregion

        #region Skills

        public int SkillPoints { get; set; } = 0; // Unspent skill points
        public int PendingSkillPoints { get; set; } = 0;
        
        #endregion

        #region Resources

        public int Stamina { get; set; } = 100;
        public int MaxStamina { get; set; } = 100;
        public float StaminaRegen { get; set; } = RpgConstants.BASE_STAMINA_REGEN; // per second
        private float staminaRegenDelay = 0f;
        private float staminaRegenRemainder = 0f;

        public int Rage { get; set; } = 0;
        public int MaxRage { get; set; } = 100;

        #endregion

        #region Modifiers

        public float CooldownReduction { get; set; } = 0f; // 0-0.8 (0-80%)
        public float ExpMultiplier { get; set; } = 1f;
        public float DropRateBonus { get; set; } = 0f;
        public float EquipmentCDR { get; set; } = 0f; // CDR from equipment

        private const float RangerAgilityDodgePerRank = 0.005f;
        private const float DeadlyReflexesDodgePerRank = 0.004f;

        #endregion

        #region Temporary Skill Buffs

        // Temporary buff storage (duration in frames, value is the amount)
        private int tempDefenseBonus = 0;
        private int tempDefenseDuration = 0;
        
        private float tempAttackSpeedBonus = 0f;
        private int tempAttackSpeedDuration = 0;

        private float tempRangedAttackSpeedBonus = 0f;
        private int tempRangedAttackSpeedDuration = 0;
        
        private float tempSummonDamageBonus = 0f;
        private int tempSummonDamageDuration = 0;
        
        private float tempDamageBonus = 0f;
        private int tempDamageDuration = 0;

        private float tempRangedDamageBonus = 0f;
        private int tempRangedDamageDuration = 0;

        private float tempMagicDamageBonus = 0f;
        private int tempMagicDamageDuration = 0;

        private float tempRangedCritBonus = 0f;
        private int tempRangedCritDuration = 0;

        private float tempKnockbackResist = 0f;
        private int tempKnockbackResistDuration = 0;
        private int tempNoKnockbackDuration = 0;
        
        private float tempDamageTakenMult = 0f;
        private int tempDamageTakenDuration = 0;
        
        private float tempDamageReduction = 0f;
        private int tempDamageReductionDuration = 0;
        
        private float tempLifesteal = 0f;
        private int tempLifestealDuration = 0;

        #endregion

        #region Debug DPS Test

        public bool DpsTestActive { get; private set; } = false;
        private int dpsTestFramesRemaining = 0;
        private long dpsTestTotalDamage = 0;
        private int dpsTestDurationFrames = 0;

        #endregion

        #region Passive Stack Buffs

        private int righteousFuryStacks = 0;
        private int righteousFuryTimer = 0;
        private int carnageStacks = 0;
        private int carnageTimer = 0;
        private int lastStandCooldownFrames = 0;
        private bool undyingRageUsed = false;
        private int lastAttackFrame = 0;

        private const int CamouflageIdleFrames = 120;

        #endregion

        #region Bonus Stats (from job/skills)

        public int BonusStrength { get; set; } = 0;
        public int BonusDexterity { get; set; } = 0;
        public int BonusRogue { get; set; } = 0;
        public int BonusIntelligence { get; set; } = 0;
        public int BonusFocus { get; set; } = 0;
        public int BonusVitality { get; set; } = 0;
        public int BonusStamina { get; set; } = 0;
        public int BonusDefense { get; set; } = 0;
        public int BonusAgility { get; set; } = 0;
        public int BonusWisdom { get; set; } = 0;
        public int BonusFortitude { get; set; } = 0;
        public int BonusLuck { get; set; } = 0;

        #endregion

        #region Total Stats (for calculations)

        public int TotalStrength => Strength + AutoStrength + BonusStrength;
        public int TotalDexterity => Dexterity + AutoDexterity + BonusDexterity;
        public int TotalRogue => Rogue + AutoRogue + BonusRogue;
        public int TotalIntelligence => Intelligence + AutoIntelligence + BonusIntelligence;
        public int TotalFocus => Focus + AutoFocus + BonusFocus;
        public int TotalVitality => Vitality + AutoVitality + BonusVitality;
        public int TotalStaminaStat => StaminaStat + AutoStamina + BonusStamina;
        public int TotalDefense => Defense + AutoDefense + BonusDefense;
        public int TotalAgility => Agility + AutoAgility + BonusAgility;
        public int TotalWisdom => Wisdom + AutoWisdom + BonusWisdom;
        public int TotalFortitude => Fortitude + AutoFortitude + BonusFortitude;
        public int TotalLuck => Luck + AutoLuck + BonusLuck;

        #endregion

        #region Lifecycle

        public override void Initialize()
        {
            Level = 1;
            CurrentXP = 0;
            CurrentJob = JobType.Novice;
            SelectedJobs = new Dictionary<JobTier, JobType>();
            SelectedJobs[JobTier.Novice] = JobType.Novice;
            
            // Initialize all 12 stats to 0
            Strength = Dexterity = Rogue = Intelligence = Focus = Vitality = 0;
            StaminaStat = Defense = Agility = Wisdom = Fortitude = Luck = 0;
            StatPoints = 0;
            SkillPoints = 0;
            PendingStatPoints = 0;
            PendingSkillPoints = 0;
            AutoStrength = AutoDexterity = AutoRogue = AutoIntelligence = AutoFocus = AutoVitality = 0;
            AutoStamina = AutoDefense = AutoAgility = AutoWisdom = AutoFortitude = AutoLuck = 0;
            
            Stamina = MaxStamina = 100;
            Rage = 0;
            MaxRage = 100;
            staminaRegenDelay = 0f;
            staminaRegenRemainder = 0f;
            
            CooldownReduction = 0f;
            ExpMultiplier = 1f;
            DropRateBonus = 0f;

            autoGrowthRemainders = new float[System.Enum.GetValues(typeof(StatType)).Length];

            righteousFuryStacks = 0;
            righteousFuryTimer = 0;
            carnageStacks = 0;
            carnageTimer = 0;
            lastStandCooldownFrames = 0;
            undyingRageUsed = false;
            lastAttackFrame = 0;
        }

        public override void OnEnterWorld()
        {
            RequestProgressSync();
        }

        public override void OnRespawn()
        {
            righteousFuryStacks = 0;
            righteousFuryTimer = 0;
            carnageStacks = 0;
            carnageTimer = 0;
            lastStandCooldownFrames = 0;
            undyingRageUsed = false;
        }

        public override void ResetEffects()
        {
            // Reset temporary bonuses each frame (all 12 stats)
            BonusStrength = BonusDexterity = BonusRogue = BonusIntelligence = BonusFocus = BonusVitality = 0;
            BonusStamina = BonusDefense = BonusAgility = BonusWisdom = BonusFortitude = BonusLuck = 0;
            CooldownReduction = 0f;
            ExpMultiplier = 1f;
            DropRateBonus = 0f;
            
            // Apply job bonuses
            ApplyJobBonuses();
        }
        
        /// <summary>
        /// Apply stat bonuses from current job
        /// </summary>
        private void ApplyJobBonuses()
        {
            var jobData = Jobs.JobDatabase.GetJobData(CurrentJob);
            if (jobData == null || jobData.StatBonuses == null)
                return;
            
            foreach (var bonus in jobData.StatBonuses)
            {
                switch (bonus.Key)
                {
                    case StatType.Strength: BonusStrength += bonus.Value; break;
                    case StatType.Dexterity: BonusDexterity += bonus.Value; break;
                    case StatType.Rogue: BonusRogue += bonus.Value; break;
                    case StatType.Intelligence: BonusIntelligence += bonus.Value; break;
                    case StatType.Focus: BonusFocus += bonus.Value; break;
                    case StatType.Vitality: BonusVitality += bonus.Value; break;
                    case StatType.Stamina: BonusStamina += bonus.Value; break;
                    case StatType.Defense: BonusDefense += bonus.Value; break;
                    case StatType.Agility: BonusAgility += bonus.Value; break;
                    case StatType.Wisdom: BonusWisdom += bonus.Value; break;
                    case StatType.Fortitude: BonusFortitude += bonus.Value; break;
                    case StatType.Luck: BonusLuck += bonus.Value; break;
                }
            }
        }

        public override void PostUpdate()
        {
            // Enforce level cap
            int maxLevel = RpgFormulas.GetMaxLevel();
            if (Level >= maxLevel)
            {
                // Can't gain XP beyond cap
                if (CurrentXP >= RequiredXP)
                {
                    CurrentXP = RequiredXP - 1;
                }
            }

            // Regen stamina
            RegenerateStamina();

            // Update skill cooldowns
            UpdateSkillCooldowns();

            // Update temporary buff durations
            UpdateTemporaryBuffs();

            // Reduce debuff durations from Fortitude
            ApplyStatusResistance();

            // Track active attacking to support stealth/aggro effects
            if (Player.itemAnimation > 0 || Player.itemTime > 0)
            {
                RegisterAttack();
            }

            UpdateStackTimers();

            // Update DPS test timer
            UpdateDpsTest();
        }

        #endregion

        #region Stat Application

        public override void PostUpdateMiscEffects()
        {
            // Apply stat bonuses to player (12-stat system)
            
            // HP from Vitality
            int bonusHP = TotalVitality * RpgConstants.VITALITY_HP_PER_POINT;
            Player.statLifeMax2 += bonusHP;

            int lifeRegen = (int)System.Math.Round(
                TotalVitality * RpgConstants.VITALITY_HP_REGEN_PER_POINT * 2f);
            if (lifeRegen > 0)
            {
                Player.lifeRegen += lifeRegen;
            }

            // Defense from Defense stat
            int bonusDefense = (TotalDefense / 5) * RpgConstants.DEFENSE_ARMOR_PER_5_POINTS;
            Player.statDefense += bonusDefense;

            float damageReduction = TotalDefense * RpgConstants.DEFENSE_DAMAGE_REDUCTION_PER_POINT;
            if (damageReduction > 0f)
            {
                Player.endurance += damageReduction;
            }

            // Fortitude auxiliary defense
            float fortitudeReduction = TotalFortitude * RpgConstants.FORTITUDE_DEFENSE_PER_POINT;
            if (fortitudeReduction > 0f)
            {
                Player.endurance += fortitudeReduction;
            }

            // Damage/crit bonuses from stats (applies to weapons, skills, and minions)
            ApplyStatDamageBonuses();
            ApplyStatCritBonuses();

            // Apply temporary skill buffs
            ApplyTemporaryBuffs();
            ApplyStackedDamageBonuses();
            ApplyCamouflageAggro();

            // Mana from Wisdom
            int bonusMana = TotalWisdom * RpgConstants.WISDOM_MANA_PER_POINT;
            Player.statManaMax2 += bonusMana;

            int manaRegen = (int)System.Math.Round(TotalWisdom * RpgConstants.WISDOM_MANA_REGEN_PER_POINT);
            if (manaRegen > 0)
            {
                Player.manaRegen += manaRegen;
            }

            // Stamina max from Stamina stat
            MaxStamina = RpgConstants.BASE_MAX_STAMINA + (TotalStaminaStat * RpgConstants.STAMINA_MAX_PER_POINT);
            
            // Stamina regen from Stamina stat
            StaminaRegen = RpgConstants.BASE_STAMINA_REGEN + (TotalStaminaStat * RpgConstants.STAMINA_REGEN_PER_POINT);
            if (Stamina > MaxStamina)
                Stamina = MaxStamina;
            if (Stamina < 0)
                Stamina = 0;

            // Attack speed from Dexterity
            float attackSpeed = TotalDexterity * RpgConstants.DEXTERITY_ATTACK_SPEED_PER_POINT;
            Player.GetAttackSpeed(DamageClass.Generic) += attackSpeed;

            // Movement speed from Agility (capped)
            float moveSpeed = System.Math.Min(
                TotalAgility * RpgConstants.AGILITY_MOVEMENT_SPEED_PER_POINT,
                RpgConstants.AGILITY_MAX_MOVEMENT_SPEED
            );
            Player.moveSpeed += moveSpeed;

            // Minion slots from Focus with increasing thresholds
            int bonusSlots = RpgFormulas.GetFocusMinionSlotBonus(TotalFocus);
            Player.maxMinions += bonusSlots;

            DropRateBonus = TotalLuck * RpgConstants.LUCK_DROP_RATE_PER_POINT;
            if (DropRateBonus > 0f)
            {
                Player.luck += DropRateBonus;
            }

            float totalCdrPercent = CooldownReductionSystem.CalculateTotalCDR(Player);
            CooldownReduction = System.Math.Min(totalCdrPercent / 100f, RpgConstants.MAX_COOLDOWN_REDUCTION);
        }

        private void ApplyStatDamageBonuses()
        {
            float strBonus = RpgFormulas.GetStrengthDamageBonus(TotalStrength, CurrentJob);
            if (strBonus != 0f)
            {
                Player.GetDamage(DamageClass.Melee) += strBonus;
            }

            float dexBonus = RpgFormulas.GetDexterityDamageBonus(TotalDexterity, CurrentJob);
            if (dexBonus != 0f)
            {
                Player.GetDamage(DamageClass.Ranged) += dexBonus;
            }

            float rogueBonus = RpgFormulas.GetRogueDamageBonus(TotalRogue, CurrentJob);
            if (rogueBonus != 0f)
            {
                Player.GetDamage(DamageClass.Melee) += rogueBonus;
                Player.GetDamage(DamageClass.Ranged) += rogueBonus;
            }

            float intBonus = RpgFormulas.GetIntelligenceDamageBonus(TotalIntelligence, CurrentJob);
            float spellPower = TotalIntelligence * RpgConstants.INTELLIGENCE_SPELL_POWER_PER_POINT;
            float magicBonus = intBonus + spellPower;
            if (magicBonus != 0f)
            {
                Player.GetDamage(DamageClass.Magic) += magicBonus;
            }

            float focBonus = RpgFormulas.GetSummonDamageBonus(TotalFocus, CurrentJob);
            if (focBonus != 0f)
            {
                Player.GetDamage(DamageClass.Summon) += focBonus;
            }

            float lukBonus = TotalLuck * RpgConstants.LUCK_ALL_DAMAGE_PER_POINT;
            if (lukBonus != 0f)
            {
                Player.GetDamage(DamageClass.Generic) += lukBonus;
            }

            // Mod damage classes (best-effort mappings)
            DamageClass calamityRogue = ModCompatibilitySystem.GetCalamityRogueClass();
            if (calamityRogue != null && rogueBonus != 0f)
            {
                Player.GetDamage(calamityRogue) += rogueBonus;
            }

            DamageClass thoriumBard = ModCompatibilitySystem.GetThoriumBardClass();
            if (thoriumBard != null)
            {
                if (dexBonus != 0f)
                    Player.GetDamage(thoriumBard) += dexBonus;
                if (rogueBonus != 0f)
                    Player.GetDamage(thoriumBard) += rogueBonus;
            }

            DamageClass thoriumHealer = ModCompatibilitySystem.GetThoriumHealerClass();
            if (thoriumHealer != null && magicBonus != 0f)
            {
                Player.GetDamage(thoriumHealer) += magicBonus;
            }
        }

        private void ApplyStatCritBonuses()
        {
            float dexCrit = TotalDexterity * RpgConstants.DEXTERITY_RANGED_CRIT_PER_POINT;
            if (dexCrit != 0f)
            {
                Player.GetCritChance(DamageClass.Ranged) += dexCrit;
            }
            
            float rogueCrit = TotalRogue * RpgConstants.ROGUE_CRIT_CHANCE_PER_POINT;
            if (rogueCrit != 0f)
            {
                Player.GetCritChance(DamageClass.Melee) += rogueCrit;
                Player.GetCritChance(DamageClass.Ranged) += rogueCrit;
            }
            
            float intCrit = TotalIntelligence * RpgConstants.INTELLIGENCE_MAGIC_CRIT_PER_POINT;
            if (intCrit != 0f)
            {
                Player.GetCritChance(DamageClass.Magic) += intCrit;
            }

            float lukCrit = TotalLuck * RpgConstants.LUCK_CRIT_PER_POINT;
            if (lukCrit != 0f)
            {
                Player.GetCritChance(DamageClass.Generic) += lukCrit;
            }

            // Mod damage classes (best-effort crit mappings)
            DamageClass calamityRogue = ModCompatibilitySystem.GetCalamityRogueClass();
            if (calamityRogue != null && rogueCrit != 0f)
            {
                Player.GetCritChance(calamityRogue) += rogueCrit;
            }

            DamageClass thoriumBard = ModCompatibilitySystem.GetThoriumBardClass();
            if (thoriumBard != null && dexCrit != 0f)
            {
                Player.GetCritChance(thoriumBard) += dexCrit;
            }
            if (thoriumBard != null && rogueCrit != 0f)
            {
                Player.GetCritChance(thoriumBard) += rogueCrit;
            }

            DamageClass thoriumHealer = ModCompatibilitySystem.GetThoriumHealerClass();
            if (thoriumHealer != null && intCrit != 0f)
            {
                Player.GetCritChance(thoriumHealer) += intCrit;
            }
        }

        public override void ModifyManaCost(Item item, ref float reduce, ref float mult)
        {
            // Intelligence mana cost reduction (capped at 50%)
            float intReduction = System.Math.Min(
                TotalIntelligence * RpgConstants.INTELLIGENCE_MANA_COST_REDUCTION_PER_POINT,
                RpgConstants.INTELLIGENCE_MAX_MANA_COST_REDUCTION
            );
            mult *= (1f - intReduction); // Reduce final cost
        }

        #endregion

        #region Resource Management

        private void RegenerateStamina()
        {
            if (Stamina < MaxStamina)
            {
                // Delay regen after use
                if (staminaRegenDelay > 0)
                {
                    staminaRegenDelay -= 1f / 60f; // Frame time
                    if (staminaRegenDelay < 0f)
                        staminaRegenDelay = 0f;
                    return;
                }
                
                // Regen per frame (StaminaRegen is per second)
                float regenPerFrame = StaminaRegen / 60f;
                staminaRegenRemainder += regenPerFrame;
                int regenWhole = (int)staminaRegenRemainder;
                if (regenWhole > 0)
                {
                    Stamina += regenWhole;
                    staminaRegenRemainder -= regenWhole;
                }
                
                if (Stamina > MaxStamina)
                    Stamina = MaxStamina;
            }
        }

        private void UpdateSkillCooldowns()
        {
            // Skill cooldowns are now handled by SkillManager.PostUpdate()
            // This method is kept for potential future per-player cooldown modifications
        }

        public bool ConsumeStamina(int amount)
        {
            if (Stamina >= amount)
            {
                Stamina -= amount;
                staminaRegenDelay = RpgConstants.STAMINA_REGEN_DELAY;
                return true;
            }
            return false;
        }

        public void AddRage(int amount)
        {
            Rage += amount;
            if (Rage > MaxRage)
                Rage = MaxRage;
        }

        #endregion

        #region Temporary Buff Methods

        /// <summary>
        /// Add temporary defense bonus from skills
        /// </summary>
        public void AddTemporaryDefense(int amount, int durationFrames)
        {
            tempDefenseBonus = amount;
            tempDefenseDuration = durationFrames;
        }

        /// <summary>
        /// Add temporary attack speed bonus from skills
        /// </summary>
        public void AddTemporaryAttackSpeed(float bonus, int durationFrames)
        {
            tempAttackSpeedBonus = bonus;
            tempAttackSpeedDuration = durationFrames;
        }

        /// <summary>
        /// Add temporary ranged attack speed bonus from skills
        /// </summary>
        public void AddTemporaryRangedAttackSpeed(float bonus, int durationFrames)
        {
            tempRangedAttackSpeedBonus = bonus;
            tempRangedAttackSpeedDuration = durationFrames;
        }

        /// <summary>
        /// Add temporary summon damage bonus from skills
        /// </summary>
        public void AddTemporarySummonDamage(float bonus, int durationFrames)
        {
            tempSummonDamageBonus = bonus;
            tempSummonDamageDuration = durationFrames;
        }

        /// <summary>
        /// Add temporary damage bonus from skills
        /// </summary>
        public void AddTemporaryDamage(float bonus, int durationFrames)
        {
            tempDamageBonus = bonus;
            tempDamageDuration = durationFrames;
        }

        /// <summary>
        /// Add temporary ranged damage bonus from skills
        /// </summary>
        public void AddTemporaryRangedDamage(float bonus, int durationFrames)
        {
            tempRangedDamageBonus = bonus;
            tempRangedDamageDuration = durationFrames;
        }

        /// <summary>
        /// Add temporary magic damage bonus from skills
        /// </summary>
        public void AddTemporaryMagicDamage(float bonus, int durationFrames)
        {
            tempMagicDamageBonus = bonus;
            tempMagicDamageDuration = durationFrames;
        }

        /// <summary>
        /// Add temporary ranged crit bonus from skills
        /// </summary>
        public void AddTemporaryRangedCrit(float bonus, int durationFrames)
        {
            tempRangedCritBonus = bonus;
            tempRangedCritDuration = durationFrames;
        }

        /// <summary>
        /// Add temporary knockback resistance from skills
        /// </summary>
        public void AddTemporaryKnockbackResist(float bonus, int durationFrames)
        {
            tempKnockbackResist = bonus;
            tempKnockbackResistDuration = durationFrames;
        }

        /// <summary>
        /// Add temporary knockback immunity from skills
        /// </summary>
        public void AddTemporaryNoKnockback(int durationFrames)
        {
            tempNoKnockbackDuration = durationFrames;
        }

        /// <summary>
        /// Add temporary damage taken multiplier (for berserk-style skills)
        /// </summary>
        public void AddTemporaryDamageTaken(float mult, int durationFrames)
        {
            tempDamageTakenMult = mult;
            tempDamageTakenDuration = durationFrames;
        }

        /// <summary>
        /// Add temporary damage reduction from skills
        /// </summary>
        public void AddTemporaryDamageReduction(float reduction, int durationFrames)
        {
            tempDamageReduction = reduction;
            tempDamageReductionDuration = durationFrames;
        }

        /// <summary>
        /// Add temporary lifesteal from skills
        /// </summary>
        public void AddTemporaryLifesteal(float amount, int durationFrames)
        {
            tempLifesteal = amount;
            tempLifestealDuration = durationFrames;
        }

        /// <summary>
        /// Update all temporary buff durations - optimized to avoid redundant checks
        /// </summary>
        private void UpdateTemporaryBuffs()
        {
            // Batch update - avoid individual if checks when no buffs active
            if (tempDefenseDuration > 0 && --tempDefenseDuration <= 0)
                tempDefenseBonus = 0;

            if (tempAttackSpeedDuration > 0 && --tempAttackSpeedDuration <= 0)
                tempAttackSpeedBonus = 0f;

            if (tempRangedAttackSpeedDuration > 0 && --tempRangedAttackSpeedDuration <= 0)
                tempRangedAttackSpeedBonus = 0f;

            if (tempSummonDamageDuration > 0 && --tempSummonDamageDuration <= 0)
                tempSummonDamageBonus = 0f;

            if (tempDamageDuration > 0 && --tempDamageDuration <= 0)
                tempDamageBonus = 0f;

            if (tempRangedDamageDuration > 0 && --tempRangedDamageDuration <= 0)
                tempRangedDamageBonus = 0f;

            if (tempMagicDamageDuration > 0 && --tempMagicDamageDuration <= 0)
                tempMagicDamageBonus = 0f;

            if (tempRangedCritDuration > 0 && --tempRangedCritDuration <= 0)
                tempRangedCritBonus = 0f;

            if (tempKnockbackResistDuration > 0 && --tempKnockbackResistDuration <= 0)
                tempKnockbackResist = 0f;

            if (tempNoKnockbackDuration > 0)
                tempNoKnockbackDuration--;

            if (tempDamageTakenDuration > 0 && --tempDamageTakenDuration <= 0)
                tempDamageTakenMult = 0f;

            if (tempDamageReductionDuration > 0 && --tempDamageReductionDuration <= 0)
                tempDamageReduction = 0f;

            if (tempLifestealDuration > 0 && --tempLifestealDuration <= 0)
                tempLifesteal = 0f;
        }

        /// <summary>
        /// Apply all temporary stat bonuses to the player - optimized with early exit
        /// </summary>
        private void ApplyTemporaryBuffs()
        {
            // Early exit if no buffs active
            if (tempDefenseBonus == 0 && tempAttackSpeedBonus == 0f && 
                tempRangedAttackSpeedBonus == 0f &&
                tempSummonDamageBonus == 0f && tempDamageBonus == 0f &&
                tempRangedDamageBonus == 0f && tempMagicDamageBonus == 0f &&
                tempRangedCritBonus == 0f && tempNoKnockbackDuration <= 0)
                return;

            // Apply active buffs
            if (tempDefenseBonus > 0)
                Player.statDefense += tempDefenseBonus;

            if (tempAttackSpeedBonus > 0f)
                Player.GetAttackSpeed(DamageClass.Generic) += tempAttackSpeedBonus;

            if (tempRangedAttackSpeedBonus > 0f)
                Player.GetAttackSpeed(DamageClass.Ranged) += tempRangedAttackSpeedBonus;

            if (tempSummonDamageBonus > 0f)
                Player.GetDamage(DamageClass.Summon) += tempSummonDamageBonus;

            if (tempDamageBonus > 0f)
                Player.GetDamage(DamageClass.Generic) += tempDamageBonus;

            if (tempRangedDamageBonus > 0f)
                Player.GetDamage(DamageClass.Ranged) += tempRangedDamageBonus;

            if (tempMagicDamageBonus > 0f)
                Player.GetDamage(DamageClass.Magic) += tempMagicDamageBonus;

            if (tempRangedCritBonus > 0f)
                Player.GetCritChance(DamageClass.Ranged) += tempRangedCritBonus;

            if (tempNoKnockbackDuration > 0)
                Player.noKnockback = true;
        }

        private void ApplyStackedDamageBonuses()
        {
            if (righteousFuryStacks > 0)
            {
                int rank = GetLearnedSkillRank("RighteousFury");
                if (rank > 0)
                {
                    float perStack = RpgMod.Common.Skills.Tier2.Paladin.RighteousFury.GetDamagePerStack(rank);
                    Player.GetDamage(DamageClass.Generic) += perStack * righteousFuryStacks;
                }
            }

            if (carnageStacks > 0)
            {
                int rank = GetLearnedSkillRank("Carnage");
                if (rank > 0)
                {
                    float perStack = RpgMod.Common.Skills.Tier2.Berserker.Carnage.GetDamagePerStack(rank);
                    Player.GetDamage(DamageClass.Melee) += perStack * carnageStacks;
                }
            }
        }

        private void ApplyCamouflageAggro()
        {
            int rank = GetLearnedSkillRank("CamouflageExpert");
            if (rank <= 0)
                return;

            int framesSinceAttack = (int)Main.GameUpdateCount - lastAttackFrame;
            if (framesSinceAttack < CamouflageIdleFrames)
                return;

            int reduction = 20 * rank;
            Player.aggro -= reduction;
        }

        private void UpdateStackTimers()
        {
            if (lastStandCooldownFrames > 0)
                lastStandCooldownFrames--;

            if (righteousFuryTimer > 0 && --righteousFuryTimer <= 0)
                righteousFuryStacks = 0;

            if (carnageTimer > 0 && --carnageTimer <= 0)
                carnageStacks = 0;
        }

        public void RegisterAttack()
        {
            lastAttackFrame = (int)Main.GameUpdateCount;
        }

        public void RegisterCarnageKill()
        {
            AddCarnageStack();
        }

        private void AddRighteousFuryStack()
        {
            int rank = GetLearnedSkillRank("RighteousFury");
            if (rank <= 0)
                return;

            int maxStacks = RpgMod.Common.Skills.Tier2.Paladin.RighteousFury.GetMaxStacks(rank);
            righteousFuryStacks = System.Math.Min(righteousFuryStacks + 1, maxStacks);
            righteousFuryTimer = RpgMod.Common.Skills.Tier2.Paladin.RighteousFury.GetDurationSeconds(rank) * 60;
        }

        private void AddCarnageStack()
        {
            int rank = GetLearnedSkillRank("Carnage");
            if (rank <= 0)
                return;

            int maxStacks = RpgMod.Common.Skills.Tier2.Berserker.Carnage.GetMaxStacks(rank);
            carnageStacks = System.Math.Min(carnageStacks + 1, maxStacks);
            carnageTimer = RpgMod.Common.Skills.Tier2.Berserker.Carnage.GetDurationSeconds(rank) * 60;
        }

        private void ApplyStatusResistance()
        {
            float resist = TotalFortitude * RpgConstants.FORTITUDE_STATUS_RESISTANCE_PER_POINT;
            if (resist <= 0f)
                return;

            resist = System.Math.Min(resist, 0.5f);

            for (int i = 0; i < Player.MaxBuffs; i++)
            {
                int buffType = Player.buffType[i];
                if (buffType <= 0 || Player.buffTime[i] <= 0)
                    continue;
                if (buffType < Player.buffImmune.Length && Player.buffImmune[buffType])
                    continue;
                if (!Main.debuff[buffType])
                    continue;

                if (Main.rand.NextFloat() < resist)
                {
                    Player.buffTime[i] = System.Math.Max(0, Player.buffTime[i] - 1);
                }
            }
        }

        private void ApplyFortitudeKnockbackResistance(ref Player.HurtModifiers modifiers)
        {
            if (Player.noKnockback || tempNoKnockbackDuration > 0)
            {
                modifiers.Knockback *= 0f;
                return;
            }

            float resist = 0f;
            if (TotalFortitude > 0)
            {
                resist = TotalFortitude * RpgConstants.FORTITUDE_KNOCKBACK_RESIST_PER_POINT;
                resist = System.Math.Min(resist, 0.5f);
            }

            resist += GetSkillKnockbackResist() + tempKnockbackResist;
            if (resist <= 0f)
                return;

            resist = MathHelper.Clamp(resist, 0f, 1f);
            modifiers.Knockback *= (1f - resist);
        }

        private bool TryDodgeHit()
        {
            float chance = GetTotalDodgeChance();
            if (chance <= 0f)
                return false;

            return Main.rand.NextFloat() < chance;
        }

        private float GetTotalDodgeChance()
        {
            float dodge = TotalAgility * RpgConstants.AGILITY_DODGE_PER_POINT;
            dodge += GetSkillDodgeBonus();
            return System.Math.Min(dodge, RpgConstants.AGILITY_MAX_DODGE);
        }

        private float GetSkillDodgeBonus()
        {
            var skillManager = Player.GetModPlayer<SkillManager>();
            if (skillManager == null || skillManager.LearnedSkills == null || skillManager.LearnedSkills.Count == 0)
                return 0f;

            float bonus = 0f;

            if (skillManager.LearnedSkills.TryGetValue("Evasion", out var evasion))
            {
                bonus += Skills.Tier2.Assassin.Evasion.GetDodgeChance(evasion.CurrentRank);
            }

            if (skillManager.LearnedSkills.TryGetValue("Agility", out var agility))
            {
                bonus += agility.CurrentRank * RangerAgilityDodgePerRank;
            }

            if (skillManager.LearnedSkills.TryGetValue("DeadlyReflexes", out var reflexes))
            {
                bonus += reflexes.CurrentRank * DeadlyReflexesDodgePerRank;
            }

            return bonus;
        }

        public override void ModifyHitByNPC(NPC npc, ref Player.HurtModifiers modifiers)
        {
            if (TryDodgeHit())
            {
                modifiers.FinalDamage *= 0f;
                return;
            }

            ApplyFortitudeKnockbackResistance(ref modifiers);

            // Apply damage taken multiplier (berserk)
            if (tempDamageTakenMult > 0f)
            {
                modifiers.FinalDamage *= (1f + tempDamageTakenMult);
            }

            // Apply damage reduction
            if (tempDamageReduction > 0f)
            {
                modifiers.FinalDamage *= (1f - tempDamageReduction);
            }
        }

        public override void ModifyHitByProjectile(Projectile proj, ref Player.HurtModifiers modifiers)
        {
            if (TryDodgeHit())
            {
                modifiers.FinalDamage *= 0f;
                return;
            }

            ApplyFortitudeKnockbackResistance(ref modifiers);

            // Apply damage taken multiplier (berserk)
            if (tempDamageTakenMult > 0f)
            {
                modifiers.FinalDamage *= (1f + tempDamageTakenMult);
            }

            // Apply damage reduction
            if (tempDamageReduction > 0f)
            {
                modifiers.FinalDamage *= (1f - tempDamageReduction);
            }
        }

        public override void OnHitByNPC(NPC npc, Player.HurtInfo hurtInfo)
        {
            if (hurtInfo.Damage <= 0)
                return;

            AddRighteousFuryStack();
        }

        public override void OnHitByProjectile(Projectile proj, Player.HurtInfo hurtInfo)
        {
            if (hurtInfo.Damage <= 0)
                return;

            AddRighteousFuryStack();
        }

        public override void ModifyHitNPCWithItem(Item item, NPC target, ref NPC.HitModifiers modifiers)
        {
            if (item == null)
                return;

            ApplySkillHitDamageBonuses(target, ref modifiers, item.DamageType, isMinion: false);
        }

        public override void ModifyHitNPCWithProj(Projectile proj, NPC target, ref NPC.HitModifiers modifiers)
        {
            if (proj == null)
                return;

            bool isMinion = IsMinionProjectile(proj);
            bool applySummonFallback = isMinion || IsSummonDamageClass(proj.DamageType);
            ApplySummonMinionDamageFallback(proj.DamageType, applySummonFallback, ref modifiers);
            ApplySkillHitDamageBonuses(target, ref modifiers, proj.DamageType, isMinion);
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            // Apply lifesteal
            if (tempLifesteal > 0f && damageDone > 0)
            {
                int heal = (int)(damageDone * tempLifesteal);
                if (heal > 0)
                {
                    Player.statLife = System.Math.Min(Player.statLife + heal, Player.statLifeMax2);
                    Player.HealEffect(heal, true);
                }
            }

            TryApplyBloodPactLifesteal(damageDone);

            // DPS test tracking
            if (DpsTestActive && damageDone > 0)
            {
                dpsTestTotalDamage += damageDone;
            }
        }

        public override void OnHitNPCWithItem(Item item, NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (damageDone <= 0 || item == null)
                return;

            RegisterAttack();
            TryTriggerManaSiphon(isMinion: false);
            TryRestoreManaBlade(item.DamageType, isMinion: false);
        }

        public override void OnHitNPCWithProj(Projectile proj, NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (damageDone <= 0 || proj == null)
                return;

            RegisterAttack();
            bool isMinion = IsMinionProjectile(proj);
            TryTriggerManaSiphon(isMinion);
            TryRestoreManaBlade(proj.DamageType, isMinion);
            TryApplySoulBondLifesteal(damageDone, isMinion);
        }

        public override bool PreKill(double damage, int hitDirection, bool pvp, ref bool playSound, ref bool genDust, ref PlayerDeathReason damageSource)
        {
            int lastStandRank = GetLearnedSkillRank("LastStand");
            if (lastStandRank > 0 && lastStandCooldownFrames <= 0)
            {
                lastStandCooldownFrames = RpgMod.Common.Skills.Tier2.Knight.LastStand.GetCooldownSeconds(lastStandRank) * 60;
                Player.statLife = 1;
                Player.immune = true;
                Player.immuneTime = 60;
                if (Main.netMode != NetmodeID.Server)
                    CombatText.NewText(Player.Hitbox, Color.Gold, "Last Stand!");
                return false;
            }

            int undyingRank = GetLearnedSkillRank("UndyingRage");
            if (undyingRank > 0 && !undyingRageUsed)
            {
                Player.statLife = System.Math.Max(1, RpgMod.Common.Skills.Tier2.Berserker.UndyingRage.GetSurviveHp(undyingRank));
                undyingRageUsed = true;
                Player.immune = true;
                Player.immuneTime = 60;
                if (Main.netMode != NetmodeID.Server)
                    CombatText.NewText(Player.Hitbox, Color.DarkRed, "Undying Rage!");
                return false;
            }

            return true;
        }

        public override bool CanConsumeAmmo(Item weapon, Item ammo)
        {
            var skillManager = Player.GetModPlayer<SkillManager>();
            if (skillManager == null || skillManager.LearnedSkills == null || skillManager.LearnedSkills.Count == 0)
                return true;

            if (TryGetSkillRank(skillManager, "AmmoExpert", out int ammoExpertRank) &&
                Main.rand.NextFloat() < RpgMod.Common.Skills.Tier2.Gunslinger.AmmoExpert.GetAmmoSaveChance(ammoExpertRank))
            {
                return false;
            }

            if (TryGetSkillRank(skillManager, "InfiniteAmmo", out int infiniteAmmoRank) &&
                Main.rand.NextFloat() < Skills.Tier3.Gunmaster.InfiniteAmmo.GetAmmoSaveChance(infiniteAmmoRank))
            {
                return false;
            }

            return true;
        }

        private void ApplySkillHitDamageBonuses(NPC target, ref NPC.HitModifiers modifiers, DamageClass damageClass, bool isMinion)
        {
            var skillManager = Player.GetModPlayer<SkillManager>();
            if (skillManager == null || skillManager.LearnedSkills == null || skillManager.LearnedSkills.Count == 0)
                return;

            if (!isMinion && Player.HasBuff(BuffID.Invisibility) &&
                TryGetSkillRank(skillManager, "AmbushMaster", out int ambushRank))
            {
                float bonus = RpgMod.Common.Skills.Tier2.Assassin.AmbushMaster.GetAmbushBonus(ambushRank);
                modifiers.SourceDamage += bonus;
            }

            if (TryGetSkillRank(skillManager, "CrimsonVitality", out int crimsonRank))
            {
                float hpRatio = Player.statLifeMax2 > 0 ? Player.statLife / (float)Player.statLifeMax2 : 1f;
                float scale = 0f;

                if (hpRatio <= 0.25f)
                    scale = 1f;
                else if (hpRatio < 1f)
                    scale = 1f - ((hpRatio - 0.25f) / 0.75f);

                if (scale > 0f)
                {
                    float bonus = Skills.Tier3.BloodKnight.CrimsonVitality.GetLowHpBonus(crimsonRank) * scale;
                    modifiers.SourceDamage += bonus;
                }
            }

            if (damageClass != null && damageClass.CountsAsClass(DamageClass.Ranged) &&
                TryGetSkillRank(skillManager, "LongRange", out int longRangeRank))
            {
                float distance = Vector2.Distance(Player.Center, target.Center);
                if (distance > RpgConstants.LONG_RANGE_MIN_DISTANCE)
                {
                    float denom = System.Math.Max(1f, RpgConstants.LONG_RANGE_MAX_DISTANCE - RpgConstants.LONG_RANGE_MIN_DISTANCE);
                    float scale = MathHelper.Clamp((distance - RpgConstants.LONG_RANGE_MIN_DISTANCE) / denom, 0f, 1f);
                    if (scale > 0f)
                    {
                        float bonus = RpgMod.Common.Skills.Tier2.Sniper.LongRange.GetDistanceBonus(longRangeRank) * scale;
                        modifiers.SourceDamage += bonus;
                    }
                }
            }
        }

        private static bool IsMinionProjectile(Projectile proj)
        {
            if (proj == null)
                return false;

            if (proj.minion || proj.sentry || proj.minionSlots > 0f)
                return true;

            int type = proj.type;
            return type >= 0 &&
                   type < ProjectileID.Sets.MinionShot.Length &&
                   ProjectileID.Sets.MinionShot[type];
        }

        private static bool IsSummonDamageClass(DamageClass damageClass)
        {
            return damageClass != null && damageClass.CountsAsClass(DamageClass.Summon);
        }

        private void ApplySummonMinionDamageFallback(DamageClass damageClass, bool applyFallback, ref NPC.HitModifiers modifiers)
        {
            if (!applyFallback)
                return;

            float genericAdditive = Player.GetDamage(DamageClass.Generic).Additive;
            float summonSpecific = Player.GetDamage(DamageClass.Summon).Additive - genericAdditive;
            if (summonSpecific <= 0f)
                return;

            float classSpecific = 0f;
            if (damageClass != null)
            {
                float classAdditive = Player.GetDamage(damageClass).Additive;
                classSpecific = classAdditive - genericAdditive;
            }

            float bonus = summonSpecific - classSpecific;
            if (bonus > 0f)
                modifiers.SourceDamage += bonus;
        }

        private float GetSkillKnockbackResist()
        {
            float resist = 0f;

            int shieldExpertiseRank = GetLearnedSkillRank("ShieldExpertise");
            if (shieldExpertiseRank > 0)
            {
                resist += RpgMod.Common.Skills.Tier2.Knight.ShieldExpertise.GetKnockbackResist(shieldExpertiseRank);
            }

            int shieldMasteryRank = GetLearnedSkillRank("ShieldMastery");
            if (shieldMasteryRank > 0)
            {
                resist += RpgMod.Common.Skills.Tier2.Knight.ShieldMastery.GetKnockbackResist(shieldMasteryRank);
            }

            int unbreakableRank = GetLearnedSkillRank("Unbreakable");
            if (unbreakableRank > 0)
            {
                resist += RpgMod.Common.Skills.Tier3.Guardian.Unbreakable.GetKnockbackResist(unbreakableRank);
            }

            return resist;
        }

        private void TryTriggerManaSiphon(bool isMinion)
        {
            if (isMinion)
                return;

            int rank = GetLearnedSkillRank("ManaSiphon");
            if (rank <= 0)
                return;

            float chance = Skills.Tier2.Spellthief.ManaSiphon.GetSiphonChance(rank);
            if (chance <= 0f || Main.rand.NextFloat() >= chance)
                return;

            RestoreMana(Skills.Tier2.Spellthief.ManaSiphon.GetManaAmount(rank));
        }

        private void TryRestoreManaBlade(DamageClass damageClass, bool isMinion)
        {
            if (isMinion || damageClass == null || !damageClass.CountsAsClass(DamageClass.Melee))
                return;

            int rank = GetLearnedSkillRank("ManaBlade");
            if (rank <= 0)
                return;

            RestoreMana(Skills.Tier2.Spellblade.ManaBlade.GetManaOnHit(rank));
        }

        private void TryApplySoulBondLifesteal(int damageDone, bool isMinion)
        {
            if (!isMinion || damageDone <= 0)
                return;

            int rank = GetLearnedSkillRank("SoulBond");
            if (rank <= 0)
                return;

            float lifesteal = RpgMod.Common.Skills.Tier1.Summoner.SoulBond.GetLifeSteal(rank);
            if (lifesteal <= 0f)
                return;

            int heal = (int)System.Math.Floor(damageDone * lifesteal);
            if (heal <= 0)
                return;

            Player.statLife = System.Math.Min(Player.statLife + heal, Player.statLifeMax2);
            Player.HealEffect(heal, true);
        }

        private void TryApplyBloodPactLifesteal(int damageDone)
        {
            if (damageDone <= 0)
                return;

            int rank = GetLearnedSkillRank("BloodPact");
            if (rank <= 0)
                return;

            float lifesteal = Skills.Tier3.BloodKnight.BloodPact.GetLifesteal(rank);
            if (lifesteal <= 0f)
                return;

            int heal = (int)System.Math.Floor(damageDone * lifesteal);
            if (heal <= 0)
                return;

            Player.statLife = System.Math.Min(Player.statLife + heal, Player.statLifeMax2);
            Player.HealEffect(heal, true);
        }

        private void RestoreMana(int amount)
        {
            if (amount <= 0)
                return;

            int before = Player.statMana;
            Player.statMana = System.Math.Min(Player.statMana + amount, Player.statManaMax2);
            int gained = Player.statMana - before;
            if (gained <= 0)
                return;

            if (Main.netMode != NetmodeID.Server && Player.whoAmI == Main.myPlayer)
            {
                CombatText.NewText(Player.Hitbox, Color.LightBlue, $"+{gained} Mana");
            }
        }

        private int GetLearnedSkillRank(string internalName)
        {
            var skillManager = Player.GetModPlayer<SkillManager>();
            if (skillManager == null || skillManager.LearnedSkills == null)
                return 0;

            return skillManager.LearnedSkills.TryGetValue(internalName, out var skill) ? skill.CurrentRank : 0;
        }

        private static bool TryGetSkillRank(SkillManager skillManager, string internalName, out int rank)
        {
            rank = 0;
            if (skillManager == null || skillManager.LearnedSkills == null)
                return false;

            if (!skillManager.LearnedSkills.TryGetValue(internalName, out var skill) || skill.CurrentRank <= 0)
                return false;

            rank = skill.CurrentRank;
            return true;
        }

        public void StartDpsTest(int durationSeconds)
        {
            int seconds = System.Math.Max(1, durationSeconds);
            dpsTestDurationFrames = seconds * 60;
            dpsTestFramesRemaining = dpsTestDurationFrames;
            dpsTestTotalDamage = 0;
            DpsTestActive = true;
        }

        public void CancelDpsTest()
        {
            DpsTestActive = false;
            dpsTestFramesRemaining = 0;
            dpsTestDurationFrames = 0;
            dpsTestTotalDamage = 0;
        }

        private void UpdateDpsTest()
        {
            if (!DpsTestActive)
                return;

            if (dpsTestFramesRemaining > 0)
            {
                dpsTestFramesRemaining--;
                return;
            }

            DpsTestActive = false;

            if (dpsTestDurationFrames <= 0)
                return;

            float seconds = dpsTestDurationFrames / 60f;
            float dps = dpsTestTotalDamage / System.Math.Max(1f, seconds);
            if (Main.netMode != NetmodeID.Server)
            {
                Main.NewText($"[DPS Test] {dpsTestTotalDamage} total in {seconds:0.0}s → {dps:0.0} DPS", Microsoft.Xna.Framework.Color.Gold);
            }
        }

        #endregion

        #region Save/Load

        public override void SaveData(TagCompound tag)
        {
            tag["level"] = Level;
            tag["currentXP"] = CurrentXP;
            tag["job"] = (int)CurrentJob;
            
            // Save selected jobs per tier
            var selectedJobsTag = new TagCompound();
            foreach (var kvp in SelectedJobs)
            {
                selectedJobsTag[kvp.Key.ToString()] = (int)kvp.Value;
            }
            tag["selectedJobs"] = selectedJobsTag;
            
            // Save all 12 stats
            tag["strength"] = Strength;
            tag["dexterity"] = Dexterity;
            tag["rogue"] = Rogue;
            tag["intelligence"] = Intelligence;
            tag["focus"] = Focus;
            tag["vitality"] = Vitality;
            tag["staminaStat"] = StaminaStat;
            tag["defense"] = Defense;
            tag["agility"] = Agility;
            tag["wisdom"] = Wisdom;
            tag["fortitude"] = Fortitude;
            tag["luck"] = Luck;
            
            tag["statPoints"] = StatPoints;
            tag["pendingStatPoints"] = PendingStatPoints;
            tag["skillPoints"] = SkillPoints;
            tag["pendingSkillPoints"] = PendingSkillPoints;

            tag["autoStrength"] = AutoStrength;
            tag["autoDexterity"] = AutoDexterity;
            tag["autoRogue"] = AutoRogue;
            tag["autoIntelligence"] = AutoIntelligence;
            tag["autoFocus"] = AutoFocus;
            tag["autoVitality"] = AutoVitality;
            tag["autoStamina"] = AutoStamina;
            tag["autoDefense"] = AutoDefense;
            tag["autoAgility"] = AutoAgility;
            tag["autoWisdom"] = AutoWisdom;
            tag["autoFortitude"] = AutoFortitude;
            tag["autoLuck"] = AutoLuck;
            
            tag["stamina"] = Stamina;
            tag["maxStamina"] = MaxStamina;

            if (autoGrowthRemainders != null)
            {
                tag["autoGrowthRemainders"] = new List<float>(autoGrowthRemainders);
            }
        }

        public override void LoadData(TagCompound tag)
        {
            Level = tag.GetInt("level");
            CurrentXP = tag.GetLong("currentXP");
            if (tag.ContainsKey("job"))
            {
                CurrentJob = (JobType)tag.GetInt("job");
            }
            else
            {
                CurrentJob = JobType.Novice;
            }
            
            // Load selected jobs per tier
            SelectedJobs = new Dictionary<JobTier, JobType>();
            if (tag.ContainsKey("selectedJobs"))
            {
                var selectedJobsTag = tag.Get<TagCompound>("selectedJobs");
                foreach (var key in selectedJobsTag)
                {
                    JobTier tier = (JobTier)int.Parse(key.Key);
                    JobType job = (JobType)selectedJobsTag.GetInt(key.Key);
                    SelectedJobs[tier] = job;
                }
            }
            else
            {
                // Fallback for old saves
                SelectedJobs[JobTier.Novice] = JobType.Novice;
                SelectedJobs[CurrentTier] = CurrentJob;
            }
            
            // Load all 12 stats
            Strength = tag.GetInt("strength");
            Dexterity = tag.GetInt("dexterity");
            Rogue = tag.GetInt("rogue");
            Intelligence = tag.GetInt("intelligence");
            Focus = tag.GetInt("focus");
            Vitality = tag.GetInt("vitality");
            StaminaStat = tag.GetInt("staminaStat");
            Defense = tag.GetInt("defense");
            Agility = tag.GetInt("agility");
            Wisdom = tag.GetInt("wisdom");
            Fortitude = tag.GetInt("fortitude");
            Luck = tag.GetInt("luck");
            
            StatPoints = tag.GetInt("statPoints");
            PendingStatPoints = tag.ContainsKey("pendingStatPoints") ? tag.GetInt("pendingStatPoints") : 0;
            SkillPoints = tag.GetInt("skillPoints");
            PendingSkillPoints = tag.ContainsKey("pendingSkillPoints") ? tag.GetInt("pendingSkillPoints") : 0;

            AutoStrength = tag.ContainsKey("autoStrength") ? tag.GetInt("autoStrength") : 0;
            AutoDexterity = tag.ContainsKey("autoDexterity") ? tag.GetInt("autoDexterity") : 0;
            AutoRogue = tag.ContainsKey("autoRogue") ? tag.GetInt("autoRogue") : 0;
            AutoIntelligence = tag.ContainsKey("autoIntelligence") ? tag.GetInt("autoIntelligence") : 0;
            AutoFocus = tag.ContainsKey("autoFocus") ? tag.GetInt("autoFocus") : 0;
            AutoVitality = tag.ContainsKey("autoVitality") ? tag.GetInt("autoVitality") : 0;
            AutoStamina = tag.ContainsKey("autoStamina") ? tag.GetInt("autoStamina") : 0;
            AutoDefense = tag.ContainsKey("autoDefense") ? tag.GetInt("autoDefense") : 0;
            AutoAgility = tag.ContainsKey("autoAgility") ? tag.GetInt("autoAgility") : 0;
            AutoWisdom = tag.ContainsKey("autoWisdom") ? tag.GetInt("autoWisdom") : 0;
            AutoFortitude = tag.ContainsKey("autoFortitude") ? tag.GetInt("autoFortitude") : 0;
            AutoLuck = tag.ContainsKey("autoLuck") ? tag.GetInt("autoLuck") : 0;
            
            Stamina = tag.GetInt("stamina");
            MaxStamina = tag.GetInt("maxStamina");
            
            // Validate loaded values
            if (!System.Enum.IsDefined(typeof(JobType), CurrentJob) || CurrentJob == JobType.None)
                CurrentJob = JobType.Novice;
            if (Level < 1) Level = 1;
            if (MaxStamina < 100) MaxStamina = 100;
            if (Stamina < 0) Stamina = 0;
            if (Stamina > MaxStamina) Stamina = MaxStamina;
            if (PendingStatPoints < 0) PendingStatPoints = 0;
            if (PendingSkillPoints < 0) PendingSkillPoints = 0;

            autoGrowthRemainders = new float[System.Enum.GetValues(typeof(StatType)).Length];
            if (tag.ContainsKey("autoGrowthRemainders"))
            {
                var remainders = tag.GetList<float>("autoGrowthRemainders");
                for (int i = 0; i < remainders.Count && i < autoGrowthRemainders.Length; i++)
                {
                    autoGrowthRemainders[i] = remainders[i];
                }
            }
            staminaRegenDelay = 0f;
            staminaRegenRemainder = 0f;
        }

        #endregion

        #region Utility Methods

        /// <summary>
        /// Allocate stat point(s) - supports multiple allocation
        /// </summary>
        /// <param name="stat">Stat type to increase</param>
        /// <param name="amount">Amount to allocate (1, 5, 10, or all)</param>
        /// <param name="silent">If true, don't show messages (for auto-allocation)</param>
        /// <returns>True if successful</returns>
        public bool AllocateStatPoint(StatType stat, int amount = 1, bool silent = false)
        {
            // Clamp amount to available points
            amount = System.Math.Min(amount, StatPoints);
            
            if (amount <= 0)
                return false;

            switch (stat)
            {
                case StatType.Strength: Strength += amount; break;
                case StatType.Dexterity: Dexterity += amount; break;
                case StatType.Rogue: Rogue += amount; break;
                case StatType.Intelligence: Intelligence += amount; break;
                case StatType.Focus: Focus += amount; break;
                case StatType.Vitality: Vitality += amount; break;
                case StatType.Stamina: StaminaStat += amount; break;
                case StatType.Defense: Defense += amount; break;
                case StatType.Agility: Agility += amount; break;
                case StatType.Wisdom: Wisdom += amount; break;
                case StatType.Fortitude: Fortitude += amount; break;
                case StatType.Luck: Luck += amount; break;
            }

            StatPoints -= amount;
            return true;
        }

        /// <summary>
        /// Deallocates stat points from a specific stat (removes allocated points)
        /// </summary>
        /// <param name="stat">The stat to deallocate from</param>
        /// <param name="amount">Number of points to deallocate</param>
        /// <returns>True if successful</returns>
        public bool DeallocateStatPoint(StatType stat, int amount = 1)
        {
            // Clamp amount to allocated points
            int currentValue = GetBaseStatValue(stat);
            amount = System.Math.Min(amount, currentValue);
            
            if (amount <= 0)
                return false;

            switch (stat)
            {
                case StatType.Strength: Strength -= amount; break;
                case StatType.Dexterity: Dexterity -= amount; break;
                case StatType.Rogue: Rogue -= amount; break;
                case StatType.Intelligence: Intelligence -= amount; break;
                case StatType.Focus: Focus -= amount; break;
                case StatType.Vitality: Vitality -= amount; break;
                case StatType.Stamina: StaminaStat -= amount; break;
                case StatType.Defense: Defense -= amount; break;
                case StatType.Agility: Agility -= amount; break;
                case StatType.Wisdom: Wisdom -= amount; break;
                case StatType.Fortitude: Fortitude -= amount; break;
                case StatType.Luck: Luck -= amount; break;
            }

            StatPoints += amount;
            return true;
        }

        public void ApplyAutoGrowth(StatType stat, float amount)
        {
            if (amount <= 0f)
                return;

            // Apply auto growth percentage from config
            var serverConfig = ModContent.GetInstance<RpgServerConfig>();
            if (serverConfig != null)
            {
                amount *= serverConfig.AutoGrowthPercent;
            }

            if (autoGrowthRemainders == null)
            {
                autoGrowthRemainders = new float[System.Enum.GetValues(typeof(StatType)).Length];
            }

            int index = (int)stat;
            if (index < 0 || index >= autoGrowthRemainders.Length)
                return;

            float total = autoGrowthRemainders[index] + amount;
            int whole = (int)total;
            if (whole > 0)
            {
                AddAutoStat(stat, whole);
                total -= whole;
            }

            autoGrowthRemainders[index] = total;
        }

        private void AddStat(StatType stat, int amount)
        {
            if (amount <= 0)
                return;

            switch (stat)
            {
                case StatType.Strength: Strength += amount; break;
                case StatType.Dexterity: Dexterity += amount; break;
                case StatType.Rogue: Rogue += amount; break;
                case StatType.Intelligence: Intelligence += amount; break;
                case StatType.Focus: Focus += amount; break;
                case StatType.Vitality: Vitality += amount; break;
                case StatType.Stamina: StaminaStat += amount; break;
                case StatType.Defense: Defense += amount; break;
                case StatType.Agility: Agility += amount; break;
                case StatType.Wisdom: Wisdom += amount; break;
                case StatType.Fortitude: Fortitude += amount; break;
                case StatType.Luck: Luck += amount; break;
            }
        }

        private void AddAutoStat(StatType stat, int amount)
        {
            if (amount <= 0)
                return;

            switch (stat)
            {
                case StatType.Strength: AutoStrength += amount; break;
                case StatType.Dexterity: AutoDexterity += amount; break;
                case StatType.Rogue: AutoRogue += amount; break;
                case StatType.Intelligence: AutoIntelligence += amount; break;
                case StatType.Focus: AutoFocus += amount; break;
                case StatType.Vitality: AutoVitality += amount; break;
                case StatType.Stamina: AutoStamina += amount; break;
                case StatType.Defense: AutoDefense += amount; break;
                case StatType.Agility: AutoAgility += amount; break;
                case StatType.Wisdom: AutoWisdom += amount; break;
                case StatType.Fortitude: AutoFortitude += amount; break;
                case StatType.Luck: AutoLuck += amount; break;
            }
        }

        /// <summary>
        /// Allocate ALL remaining stat points to one stat
        /// </summary>
        public bool AllocateAllPoints(StatType stat)
        {
            return AllocateStatPoint(stat, StatPoints);
        }

        /// <summary>
        /// Get total allocated stat points (for reset validation)
        /// </summary>
        public int GetTotalAllocatedStats()
        {
            return Strength + Dexterity + Rogue + Intelligence + Focus + Vitality +
                   StaminaStat + Defense + Agility + Wisdom + Fortitude + Luck;
        }

        public int GetBaseStatValue(StatType stat)
        {
            return stat switch
            {
                StatType.Strength => Strength,
                StatType.Dexterity => Dexterity,
                StatType.Rogue => Rogue,
                StatType.Intelligence => Intelligence,
                StatType.Focus => Focus,
                StatType.Vitality => Vitality,
                StatType.Stamina => StaminaStat,
                StatType.Defense => Defense,
                StatType.Agility => Agility,
                StatType.Wisdom => Wisdom,
                StatType.Fortitude => Fortitude,
                StatType.Luck => Luck,
                _ => 0
            };
        }

        public int GetAutoStatValue(StatType stat)
        {
            return stat switch
            {
                StatType.Strength => AutoStrength,
                StatType.Dexterity => AutoDexterity,
                StatType.Rogue => AutoRogue,
                StatType.Intelligence => AutoIntelligence,
                StatType.Focus => AutoFocus,
                StatType.Vitality => AutoVitality,
                StatType.Stamina => AutoStamina,
                StatType.Defense => AutoDefense,
                StatType.Agility => AutoAgility,
                StatType.Wisdom => AutoWisdom,
                StatType.Fortitude => AutoFortitude,
                StatType.Luck => AutoLuck,
                _ => 0
            };
        }

        public int GetBonusStatValue(StatType stat)
        {
            return stat switch
            {
                StatType.Strength => BonusStrength,
                StatType.Dexterity => BonusDexterity,
                StatType.Rogue => BonusRogue,
                StatType.Intelligence => BonusIntelligence,
                StatType.Focus => BonusFocus,
                StatType.Vitality => BonusVitality,
                StatType.Stamina => BonusStamina,
                StatType.Defense => BonusDefense,
                StatType.Agility => BonusAgility,
                StatType.Wisdom => BonusWisdom,
                StatType.Fortitude => BonusFortitude,
                StatType.Luck => BonusLuck,
                _ => 0
            };
        }

        public int GetTotalStatValue(StatType stat)
        {
            return GetBaseStatValue(stat) + GetAutoStatValue(stat) + GetBonusStatValue(stat);
        }

        /// <summary>
        /// Reset all stats (requires special item)
        /// </summary>
        public void ResetStats()
        {
            int totalAllocated = GetTotalAllocatedStats();
            
            Strength = Dexterity = Rogue = Intelligence = Focus = Vitality = 0;
            StaminaStat = Defense = Agility = Wisdom = Fortitude = Luck = 0;
            StatPoints += totalAllocated;
        }
        
        /// <summary>
        /// Get total allocated skill points (for reset validation)
        /// </summary>
        public int GetTotalAllocatedSkillPoints()
        {
            var skillManager = Player.GetModPlayer<Skills.SkillManager>();
            return skillManager?.GetTotalAllocatedPoints() ?? 0;
        }
        
        /// <summary>
        /// Reset all learned skills and refund skill points
        /// </summary>
        public void ResetSkills()
        {
            var skillManager = Player.GetModPlayer<Skills.SkillManager>();
            if (skillManager != null)
            {
                int refunded = skillManager.ResetAllSkills();
                SkillPoints += refunded;
            }
        }

        public void ReleasePendingPoints()
        {
            if (PendingStatPoints > 0)
            {
                StatPoints += PendingStatPoints;
                PendingStatPoints = 0;
            }

            if (PendingSkillPoints > 0)
            {
                SkillPoints += PendingSkillPoints;
                PendingSkillPoints = 0;
            }
        }

        #endregion

        #region Experience & Leveling
        // XP and level-up logic lives in Players.PlayerLevel to avoid duplicate awarding.
        #endregion

        #region Networking

        internal void ApplyProgressSync(int level, long currentXP, JobType job)
        {
            Level = System.Math.Max(1, level);
            CurrentXP = System.Math.Max(0, currentXP);
            CurrentJob = System.Enum.IsDefined(typeof(JobType), job) && job != JobType.None
                ? job
                : JobType.Novice;
        }

        internal void ApplyJobSync(JobType job)
        {
            CurrentJob = System.Enum.IsDefined(typeof(JobType), job) && job != JobType.None
                ? job
                : JobType.Novice;
        }

        internal void RequestProgressSync()
        {
            if (Main.netMode != NetmodeID.MultiplayerClient || Player.whoAmI != Main.myPlayer)
                return;

            ModPacket packet = Mod.GetPacket();
            packet.Write((byte)global::Rpg.RpgMessageType.SyncPlayerProgress);
            packet.Write((byte)Player.whoAmI);
            packet.Write(Level);
            packet.Write(CurrentXP);
            packet.Write((int)CurrentJob);
            packet.Send();
        }

        internal void RequestJobSync()
        {
            if (Main.netMode != NetmodeID.MultiplayerClient || Player.whoAmI != Main.myPlayer)
                return;

            ModPacket packet = Mod.GetPacket();
            packet.Write((byte)global::Rpg.RpgMessageType.SyncPlayerJob);
            packet.Write((byte)Player.whoAmI);
            packet.Write((int)CurrentJob);
            packet.Send();
        }

        #endregion
    }
}
