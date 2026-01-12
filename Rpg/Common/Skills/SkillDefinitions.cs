using System.Collections.Generic;
using Rpg.Common;
using Rpg.Common.Base;
using Terraria.ID;
using Terraria.ModLoader;

namespace Rpg.Common.Skills
{
    public sealed class SkillDefinition
    {
        public string InternalName { get; set; }
        public string DisplayName { get; set; }
        public string Description { get; set; }
        public SkillType SkillType { get; set; }
        public JobType RequiredJob { get; set; }
        public int RequiredLevel { get; set; }
        public int SkillPointCost { get; set; }
        public int MaxRank { get; set; }
        public string IconTexture { get; set; }
        public float CooldownSeconds { get; set; }
        public int ResourceCost { get; set; }
        public ResourceType ResourceType { get; set; }

        public float PassiveDamageBonus { get; set; }
        public float PassiveCritBonus { get; set; }
        public float PassiveAttackSpeedBonus { get; set; }
        public float PassiveMoveSpeedBonus { get; set; }
        public int PassiveDefenseBonus { get; set; }
        public int PassiveMinionSlots { get; set; }
        public float PassiveManaCostReduction { get; set; }
        public int PassiveLifeRegenBonus { get; set; }
        public int PassiveMaxLifeBonus { get; set; }
        public int PassiveMaxManaBonus { get; set; }

        public int[] BuffIds { get; set; }
        public int BuffDurationSeconds { get; set; }
        public int HealAmount { get; set; }
        public int RestoreMana { get; set; }
        public int RestoreStamina { get; set; }
        public int AoEDamage { get; set; }
        public float AoERadius { get; set; }
        public float AoEKnockback { get; set; } = 4f;
        public DamageClass AoEDamageClass { get; set; } = DamageClass.Generic;
    }

    public static class SkillDefinitions
    {
        public static IReadOnlyList<SkillDefinition> All { get; } = Build();

        private static List<SkillDefinition> Build()
        {
            const int maxRank = 5;
            const int skillPointCost = 1;

            int tier1Base = RpgConstants.FIRST_JOB_LEVEL;
            int tier2Base = RpgConstants.SECOND_JOB_LEVEL;
            int tier3Base = RpgConstants.THIRD_JOB_LEVEL;

            int t1Skill1 = tier1Base;
            int t1Skill2 = tier1Base + 5;
            int t1Skill3 = tier1Base + 10;

            int t2Skill1 = tier2Base;
            int t2Skill2 = tier2Base + 10;
            int t2Skill3 = tier2Base + 15;

            int t3Skill1 = tier3Base;
            int t3Skill2 = tier3Base + 10;
            int t3Skill3 = tier3Base + 20;

            var skills = new List<SkillDefinition>
            {
                // Warrior
                CreateAoeSkill("PowerStrike", "Power Strike", "A heavy swing that hits nearby enemies.", JobType.Warrior, t1Skill1, ResourceType.Stamina, 20, 6f, 35, 90f, DamageClass.Melee, maxRank, skillPointCost),
                CreateBuffSkill("Fortify", "Fortify", "Boost defense for a short time.", JobType.Warrior, t1Skill2, ResourceType.Stamina, 20, 20f, new[] { BuffID.Ironskin, BuffID.Endurance }, 12, maxRank, skillPointCost),
                CreatePassiveSkill("BattleRage", "Battle Rage", "Increase damage and crit chance.", JobType.Warrior, t1Skill3, maxRank, skillPointCost, damageBonus: 0.05f, critBonus: 4f),

                // Ranger
                CreateBuffSkill("RapidFire", "Rapid Fire", "Increase ranged tempo temporarily.", JobType.Ranger, t1Skill1, ResourceType.Stamina, 20, 20f, new[] { BuffID.Swiftness, BuffID.Archery }, 10, maxRank, skillPointCost),
                CreateBuffSkill("EvasiveRoll", "Evasive Roll", "Quickly evade incoming danger.", JobType.Ranger, t1Skill2, ResourceType.Stamina, 15, 15f, new[] { BuffID.Swiftness }, 6, maxRank, skillPointCost),
                CreatePassiveSkill("SteadyAim", "Steady Aim", "Increase crit chance and ranged damage.", JobType.Ranger, t1Skill3, maxRank, skillPointCost, damageBonus: 0.05f, critBonus: 6f),

                // Mage
                CreateAoeSkill("Fireball", "Fireball", "Explode arcane fire around you.", JobType.Mage, t1Skill1, ResourceType.Mana, 20, 6f, 40, 120f, DamageClass.Magic, maxRank, skillPointCost),
                CreateBuffSkill("ManaShield", "Mana Shield", "Reduce damage taken with magical protection.", JobType.Mage, t1Skill2, ResourceType.Mana, 25, 20f, new[] { BuffID.Endurance, BuffID.ManaRegeneration }, 12, maxRank, skillPointCost),
                CreatePassiveSkill("ArcaneIntellect", "Arcane Intellect", "Increase magic output and mana efficiency.", JobType.Mage, t1Skill3, maxRank, skillPointCost, damageBonus: 0.05f, manaCostReduction: 0.04f),

                // Summoner
                CreateBuffSkill("SummonAlly", "Summon Ally", "Empower your summoning for a short time.", JobType.Summoner, t1Skill1, ResourceType.Mana, 20, 20f, new[] { BuffID.Summoning, BuffID.Bewitched }, 10, maxRank, skillPointCost),
                CreateBuffSkill("CommandMinions", "Command Minions", "Boost minion damage temporarily.", JobType.Summoner, t1Skill2, ResourceType.Mana, 15, 18f, new[] { BuffID.Summoning }, 12, maxRank, skillPointCost),
                CreatePassiveSkill("SummonMastery", "Summon Mastery", "Increase maximum minion slots.", JobType.Summoner, t1Skill3, maxRank, skillPointCost, minionSlots: 1),

                // Knight
                CreateBuffSkill("ShieldWall", "Shield Wall", "Brace behind a solid defense.", JobType.Knight, t2Skill1, ResourceType.Stamina, 25, 25f, new[] { BuffID.Endurance, BuffID.Ironskin }, 12, maxRank, skillPointCost),
                CreateBuffSkill("Taunt", "Taunt", "Draw enemy attention and harden yourself.", JobType.Knight, t2Skill2, ResourceType.Stamina, 15, 18f, new[] { BuffID.Thorns, BuffID.Endurance }, 10, maxRank, skillPointCost),
                CreatePassiveSkill("IronSkin", "Iron Skin", "Gain permanent defense.", JobType.Knight, t2Skill3, maxRank, skillPointCost, defenseBonus: 8),

                // Berserker
                CreateBuffSkill("Berserk", "Berserk", "Unleash fury for massive power.", JobType.Berserker, t2Skill1, ResourceType.Stamina, 25, 25f, new[] { BuffID.Rage, BuffID.Wrath }, 10, maxRank, skillPointCost),
                CreateHealSkill("Bloodlust", "Bloodlust", "Recover health through battle instinct.", JobType.Berserker, t2Skill2, ResourceType.Stamina, 20, 25f, 40, new[] { BuffID.Regeneration }, 8, maxRank, skillPointCost),
                CreatePassiveSkill("LowHPPower", "Low HP Power", "Gain damage when wounded.", JobType.Berserker, t2Skill3, maxRank, skillPointCost, damageBonus: 0.06f),

                // Paladin
                CreateHealSkill("LayOnHands", "Lay on Hands", "Restore a large amount of health.", JobType.Paladin, t2Skill1, ResourceType.Mana, 25, 25f, 60, new[] { BuffID.Regeneration }, 10, maxRank, skillPointCost),
                CreateBuffSkill("DivineBless", "Divine Bless", "Bless allies with holy power.", JobType.Paladin, t2Skill2, ResourceType.Mana, 20, 25f, new[] { BuffID.Wrath, BuffID.Regeneration }, 12, maxRank, skillPointCost),
                CreatePassiveSkill("HolyAura", "Holy Aura", "Boost defense and regeneration.", JobType.Paladin, t2Skill3, maxRank, skillPointCost, defenseBonus: 6, lifeRegenBonus: 2),

                // Death Knight
                CreateAoeHealSkill("DeathCoil", "Death Coil", "Damage enemies and siphon life.", JobType.DeathKnight, t2Skill1, ResourceType.Mana, 25, 18f, 40, 120f, DamageClass.Magic, 20, maxRank, skillPointCost),
                CreateBuffSkill("UnholyAura", "Unholy Aura", "Empower yourself with dark energy.", JobType.DeathKnight, t2Skill2, ResourceType.Mana, 20, 20f, new[] { BuffID.Thorns, BuffID.Rage }, 12, maxRank, skillPointCost),
                CreatePassiveSkill("Lichdom", "Lichdom", "Improve mana efficiency and damage.", JobType.DeathKnight, t2Skill3, maxRank, skillPointCost, damageBonus: 0.04f, manaCostReduction: 0.05f),

                // Sniper
                CreateAoeSkill("Headshot", "Headshot", "Strike with a precise lethal shot.", JobType.Sniper, t2Skill1, ResourceType.Stamina, 20, 12f, 60, 80f, DamageClass.Ranged, maxRank, skillPointCost),
                CreateBuffSkill("SteadyBreath", "Steady Breath", "Focus to boost critical precision.", JobType.Sniper, t2Skill2, ResourceType.Stamina, 15, 18f, new[] { BuffID.Archery, BuffID.Rage }, 10, maxRank, skillPointCost),
                CreatePassiveSkill("Sharpshooter", "Sharpshooter", "Increase crit chance and damage.", JobType.Sniper, t2Skill3, maxRank, skillPointCost, damageBonus: 0.05f, critBonus: 8f),

                // Assassin
                CreateAoeSkill("Backstab", "Backstab", "Deliver a devastating close strike.", JobType.Assassin, t2Skill1, ResourceType.Stamina, 20, 12f, 55, 90f, DamageClass.Melee, maxRank, skillPointCost),
                CreateBuffSkill("Vanish", "Vanish", "Fade from sight briefly.", JobType.Assassin, t2Skill2, ResourceType.Stamina, 15, 20f, new[] { BuffID.Invisibility, BuffID.Swiftness }, 6, maxRank, skillPointCost),
                CreatePassiveSkill("PoisonMastery", "Poison Mastery", "Improve damage output and crit chance.", JobType.Assassin, t2Skill3, maxRank, skillPointCost, damageBonus: 0.04f, critBonus: 4f),

                // Gunslinger
                CreateAoeSkill("FanTheHammer", "Fan the Hammer", "Unload rapid shots nearby.", JobType.Gunslinger, t2Skill1, ResourceType.Stamina, 20, 12f, 35, 110f, DamageClass.Ranged, maxRank, skillPointCost),
                CreateAoeSkill("ExplosiveRound", "Explosive Round", "Blast enemies in a wider area.", JobType.Gunslinger, t2Skill2, ResourceType.Stamina, 25, 18f, 45, 140f, DamageClass.Ranged, maxRank, skillPointCost),
                CreatePassiveSkill("QuickDraw", "Quick Draw", "Increase attack speed.", JobType.Gunslinger, t2Skill3, maxRank, skillPointCost, attackSpeedBonus: 0.1f),

                // Sorcerer
                CreateAoeSkill("ElementalBurst", "Elemental Burst", "Release volatile elemental energy.", JobType.Sorcerer, t2Skill1, ResourceType.Mana, 25, 12f, 50, 140f, DamageClass.Magic, maxRank, skillPointCost),
                CreateBuffSkill("ElementalBarrier", "Elemental Barrier", "Raise an elemental ward.", JobType.Sorcerer, t2Skill2, ResourceType.Mana, 25, 20f, new[] { BuffID.Endurance, BuffID.MagicPower }, 12, maxRank, skillPointCost),
                CreatePassiveSkill("ElementalMastery", "Elemental Mastery", "Increase elemental damage.", JobType.Sorcerer, t2Skill3, maxRank, skillPointCost, damageBonus: 0.08f),

                // Cleric
                CreateHealSkill("HealingPrayer", "Healing Prayer", "Heal yourself or allies.", JobType.Cleric, t2Skill1, ResourceType.Mana, 20, 18f, 50, new[] { BuffID.Regeneration }, 8, maxRank, skillPointCost),
                CreateBuffSkill("Sanctuary", "Sanctuary", "Create a protective sanctuary.", JobType.Cleric, t2Skill2, ResourceType.Mana, 25, 20f, new[] { BuffID.Regeneration, BuffID.Ironskin }, 12, maxRank, skillPointCost),
                CreatePassiveSkill("DivineGrace", "Divine Grace", "Improve regeneration and mana control.", JobType.Cleric, t2Skill3, maxRank, skillPointCost, lifeRegenBonus: 2, manaCostReduction: 0.05f),

                // Archmage
                CreateAoeSkill("Meteor", "Meteor", "Call down a devastating meteor.", JobType.Archmage, t2Skill1, ResourceType.Mana, 35, 20f, 70, 160f, DamageClass.Magic, maxRank, skillPointCost),
                CreateBuffSkill("TimeWarp", "Time Warp", "Warp time to improve your speed.", JobType.Archmage, t2Skill2, ResourceType.Mana, 20, 18f, new[] { BuffID.Swiftness, BuffID.MagicPower }, 10, maxRank, skillPointCost),
                CreatePassiveSkill("ArcaneMastery", "Arcane Mastery", "Greatly reduce mana costs.", JobType.Archmage, t2Skill3, maxRank, skillPointCost, damageBonus: 0.05f, manaCostReduction: 0.08f),

                // Warlock
                CreateAoeSkill("Corruption", "Corruption", "Spread dark energy around you.", JobType.Warlock, t2Skill1, ResourceType.Mana, 20, 12f, 45, 140f, DamageClass.Magic, maxRank, skillPointCost),
                CreateAoeHealSkill("DrainLife", "Drain Life", "Drain vitality from nearby foes.", JobType.Warlock, t2Skill2, ResourceType.Mana, 25, 18f, 35, 120f, DamageClass.Magic, 25, maxRank, skillPointCost),
                CreatePassiveSkill("CurseExpert", "Curse Expert", "Increase damage and crit chance.", JobType.Warlock, t2Skill3, maxRank, skillPointCost, damageBonus: 0.05f, critBonus: 4f),

                // Spellblade
                CreateBuffSkill("EnchantWeapon", "Enchant Weapon", "Infuse your weapon with magic.", JobType.Spellblade, t2Skill1, ResourceType.Mana, 20, 18f, new[] { BuffID.Wrath, BuffID.MagicPower }, 10, maxRank, skillPointCost),
                CreateAoeSkill("ArcaneSlash", "Arcane Slash", "Strike enemies with magic-infused blades.", JobType.Spellblade, t2Skill2, ResourceType.Mana, 15, 12f, 45, 110f, DamageClass.Magic, maxRank, skillPointCost),
                CreatePassiveSkill("SpellStrike", "Spell Strike", "Improve mana efficiency and damage.", JobType.Spellblade, t2Skill3, maxRank, skillPointCost, damageBonus: 0.03f, manaCostReduction: 0.04f),

                // Battle Mage
                CreateBuffSkill("MagicArmor", "Magic Armor", "Harden your defenses with mana.", JobType.BattleMage, t2Skill1, ResourceType.Mana, 25, 20f, new[] { BuffID.Ironskin, BuffID.MagicPower }, 12, maxRank, skillPointCost),
                CreateBuffSkill("CounterSpell", "Counter Spell", "Brace against incoming attacks.", JobType.BattleMage, t2Skill2, ResourceType.Mana, 25, 20f, new[] { BuffID.Endurance, BuffID.ObsidianSkin }, 10, maxRank, skillPointCost),
                CreatePassiveSkill("BattleCaster", "Battle Caster", "Move and attack faster while casting.", JobType.BattleMage, t2Skill3, maxRank, skillPointCost, moveSpeedBonus: 0.05f, attackSpeedBonus: 0.05f),

                // Beastmaster
                CreateBuffSkill("BeastCall", "Beast Call", "Summon beasts to fight by your side.", JobType.Beastmaster, t2Skill1, ResourceType.Mana, 25, 20f, new[] { BuffID.Summoning, BuffID.Bewitched }, 12, maxRank, skillPointCost),
                CreateBuffSkill("PackTactics", "Pack Tactics", "Enhance pet coordination.", JobType.Beastmaster, t2Skill2, ResourceType.Mana, 20, 18f, new[] { BuffID.Summoning, BuffID.Wrath }, 10, maxRank, skillPointCost),
                CreatePassiveSkill("WildBond", "Wild Bond", "Improve summon strength.", JobType.Beastmaster, t2Skill3, maxRank, skillPointCost, minionSlots: 1, damageBonus: 0.03f),

                // Necromancer
                CreateBuffSkill("RaiseUndead", "Raise Undead", "Call undead minions to battle.", JobType.Necromancer, t2Skill1, ResourceType.Mana, 25, 20f, new[] { BuffID.Summoning, BuffID.Bewitched }, 12, maxRank, skillPointCost),
                CreateBuffSkill("BoneArmor", "Bone Armor", "Harden with skeletal protection.", JobType.Necromancer, t2Skill2, ResourceType.Mana, 20, 20f, new[] { BuffID.Ironskin, BuffID.Endurance }, 12, maxRank, skillPointCost),
                CreatePassiveSkill("UndeadMastery", "Undead Mastery", "Increase minion capacity.", JobType.Necromancer, t2Skill3, maxRank, skillPointCost, minionSlots: 2),

                // Druid
                CreateBuffSkill("SummonBeast", "Summon Beast", "Call a primal ally.", JobType.Druid, t2Skill1, ResourceType.Mana, 20, 18f, new[] { BuffID.Summoning }, 12, maxRank, skillPointCost),
                CreateAoeSkill("NaturesWrath", "Nature's Wrath", "Release nature energy around you.", JobType.Druid, t2Skill2, ResourceType.Mana, 20, 12f, 45, 140f, DamageClass.Magic, maxRank, skillPointCost),
                CreatePassiveSkill("Shapeshifter", "Shapeshifter", "Gain speed and resilience.", JobType.Druid, t2Skill3, maxRank, skillPointCost, moveSpeedBonus: 0.05f, defenseBonus: 4),

                // Shadow
                CreateBuffSkill("ShadowStep", "Shadow Step", "Blink through the shadows.", JobType.Shadow, t2Skill1, ResourceType.Stamina, 15, 14f, new[] { BuffID.Invisibility, BuffID.Swiftness }, 6, maxRank, skillPointCost),
                CreateAoeSkill("Ambush", "Ambush", "Strike nearby foes unexpectedly.", JobType.Shadow, t2Skill2, ResourceType.Stamina, 20, 12f, 55, 100f, DamageClass.Melee, maxRank, skillPointCost),
                CreatePassiveSkill("SilentBlades", "Silent Blades", "Increase critical potential.", JobType.Shadow, t2Skill3, maxRank, skillPointCost, critBonus: 6f),

                // Spellthief
                CreateBuffSkill("StealBuff", "Steal Buff", "Steal power and restore mana.", JobType.Spellthief, t2Skill1, ResourceType.Stamina, 15, 18f, new[] { BuffID.MagicPower }, 8, maxRank, skillPointCost, restoreMana: 20),
                CreateAoeSkill("ArcaneTheft", "Arcane Theft", "Drain magical energy from foes.", JobType.Spellthief, t2Skill2, ResourceType.Mana, 20, 12f, 35, 120f, DamageClass.Magic, maxRank, skillPointCost, restoreMana: 15),
                CreatePassiveSkill("MagicRogue", "Magic Rogue", "Increase crit chance and damage.", JobType.Spellthief, t2Skill3, maxRank, skillPointCost, damageBonus: 0.03f, critBonus: 6f),

                // Guardian
                CreateBuffSkill("AegisWall", "Aegis Wall", "Raise an impenetrable barrier.", JobType.Guardian, t3Skill1, ResourceType.Stamina, 35, 30f, new[] { BuffID.Endurance, BuffID.Ironskin }, 15, maxRank, skillPointCost),
                CreateHealSkill("GuardianSpirit", "Guardian Spirit", "Restore health and bolster defenses.", JobType.Guardian, t3Skill2, ResourceType.Stamina, 30, 30f, 70, new[] { BuffID.Regeneration }, 10, maxRank, skillPointCost),
                CreatePassiveSkill("Unbreakable", "Unbreakable", "Greatly increase defense.", JobType.Guardian, t3Skill3, maxRank, skillPointCost, defenseBonus: 15),

                // Blood Knight
                CreateBuffSkill("BloodRite", "Blood Rite", "Sacrifice life for power.", JobType.BloodKnight, t3Skill1, ResourceType.Life, 30, 30f, new[] { BuffID.Wrath, BuffID.Rage }, 12, maxRank, skillPointCost),
                CreateBuffSkill("CrimsonFrenzy", "Crimson Frenzy", "Unleash relentless speed.", JobType.BloodKnight, t3Skill2, ResourceType.Stamina, 30, 25f, new[] { BuffID.Rage, BuffID.Swiftness }, 10, maxRank, skillPointCost),
                CreatePassiveSkill("SanguineArmor", "Sanguine Armor", "Increase damage and regeneration.", JobType.BloodKnight, t3Skill3, maxRank, skillPointCost, damageBonus: 0.08f, lifeRegenBonus: 2),

                // Deadeye
                CreateAoeSkill("MarkedShot", "Marked Shot", "Exploit enemy weak points.", JobType.Deadeye, t3Skill1, ResourceType.Stamina, 30, 20f, 80, 90f, DamageClass.Ranged, maxRank, skillPointCost),
                CreateBuffSkill("DeadeyeFocus", "Deadeye Focus", "Sharpen your aim for critical strikes.", JobType.Deadeye, t3Skill2, ResourceType.Stamina, 25, 20f, new[] { BuffID.Archery, BuffID.Rage }, 10, maxRank, skillPointCost),
                CreatePassiveSkill("SniperInstinct", "Sniper Instinct", "Greatly increase critical chance.", JobType.Deadeye, t3Skill3, maxRank, skillPointCost, critBonus: 10f),

                // Gunmaster
                CreateAoeSkill("BulletStorm", "Bullet Storm", "Fire a barrage of bullets.", JobType.Gunmaster, t3Skill1, ResourceType.Stamina, 35, 20f, 60, 140f, DamageClass.Ranged, maxRank, skillPointCost),
                CreateBuffSkill("QuickReload", "Quick Reload", "Reload instantly for rapid fire.", JobType.Gunmaster, t3Skill2, ResourceType.Stamina, 20, 18f, new[] { BuffID.Swiftness, BuffID.AmmoBox }, 10, maxRank, skillPointCost),
                CreatePassiveSkill("RicochetMastery", "Ricochet Mastery", "Boost attack speed and damage.", JobType.Gunmaster, t3Skill3, maxRank, skillPointCost, attackSpeedBonus: 0.12f, damageBonus: 0.04f),

                // Archbishop
                CreateHealSkill("MassHeal", "Mass Heal", "Restore health to all allies.", JobType.Archbishop, t3Skill1, ResourceType.Mana, 35, 30f, 80, new[] { BuffID.Regeneration }, 12, maxRank, skillPointCost),
                CreateBuffSkill("SacredWard", "Sacred Ward", "Shield allies with holy power.", JobType.Archbishop, t3Skill2, ResourceType.Mana, 30, 25f, new[] { BuffID.Endurance, BuffID.Regeneration }, 12, maxRank, skillPointCost),
                CreatePassiveSkill("DivineBlessing", "Divine Blessing", "Boost regeneration and mana efficiency.", JobType.Archbishop, t3Skill3, maxRank, skillPointCost, lifeRegenBonus: 3, manaCostReduction: 0.08f),

                // Overlord
                CreateBuffSkill("AlphaRoar", "Alpha Roar", "Empower all summons.", JobType.Overlord, t3Skill1, ResourceType.Mana, 35, 25f, new[] { BuffID.Summoning, BuffID.Wrath }, 12, maxRank, skillPointCost),
                CreateAoeSkill("BeastStampede", "Beast Stampede", "Charge through nearby enemies.", JobType.Overlord, t3Skill2, ResourceType.Mana, 30, 20f, 70, 160f, DamageClass.Melee, maxRank, skillPointCost),
                CreatePassiveSkill("OverlordCommand", "Overlord Command", "Increase summon potency.", JobType.Overlord, t3Skill3, maxRank, skillPointCost, minionSlots: 2, damageBonus: 0.05f),

                // Lich King
                CreateAoeHealSkill("SoulHarvest", "Soul Harvest", "Drain souls to heal yourself.", JobType.Lichking, t3Skill1, ResourceType.Mana, 30, 20f, 50, 140f, DamageClass.Magic, 35, maxRank, skillPointCost),
                CreateBuffSkill("UndeadLegion", "Undead Legion", "Summon a legion of undead.", JobType.Lichking, t3Skill2, ResourceType.Mana, 40, 25f, new[] { BuffID.Summoning, BuffID.Bewitched }, 15, maxRank, skillPointCost),
                CreatePassiveSkill("Phylactery", "Phylactery", "Improve defense and mana efficiency.", JobType.Lichking, t3Skill3, maxRank, skillPointCost, defenseBonus: 8, manaCostReduction: 0.05f)
            };

            return skills;
        }

        private static SkillDefinition CreateBase(
            string internalName,
            string displayName,
            string description,
            JobType requiredJob,
            int requiredLevel,
            SkillType skillType,
            ResourceType resourceType,
            int resourceCost,
            float cooldownSeconds,
            int maxRank,
            int skillPointCost)
        {
            return new SkillDefinition
            {
                InternalName = internalName,
                DisplayName = displayName,
                Description = description,
                RequiredJob = requiredJob,
                RequiredLevel = requiredLevel,
                SkillType = skillType,
                ResourceType = resourceType,
                ResourceCost = resourceCost,
                CooldownSeconds = cooldownSeconds,
                MaxRank = maxRank,
                SkillPointCost = skillPointCost,
                IconTexture = $"{RpgConstants.ICON_PATH}Skills/{internalName}"
            };
        }

        private static SkillDefinition CreateAoeSkill(
            string internalName,
            string displayName,
            string description,
            JobType requiredJob,
            int requiredLevel,
            ResourceType resourceType,
            int resourceCost,
            float cooldownSeconds,
            int aoeDamage,
            float aoeRadius,
            DamageClass damageClass,
            int maxRank,
            int skillPointCost,
            int restoreMana = 0)
        {
            return new SkillDefinition
            {
                InternalName = internalName,
                DisplayName = displayName,
                Description = description,
                RequiredJob = requiredJob,
                RequiredLevel = requiredLevel,
                SkillType = SkillType.Active,
                ResourceType = resourceType,
                ResourceCost = resourceCost,
                CooldownSeconds = cooldownSeconds,
                MaxRank = maxRank,
                SkillPointCost = skillPointCost,
                IconTexture = $"{RpgConstants.ICON_PATH}Skills/{internalName}",
                AoEDamage = aoeDamage,
                AoERadius = aoeRadius,
                AoEDamageClass = damageClass,
                RestoreMana = restoreMana
            };
        }

        private static SkillDefinition CreateBuffSkill(
            string internalName,
            string displayName,
            string description,
            JobType requiredJob,
            int requiredLevel,
            ResourceType resourceType,
            int resourceCost,
            float cooldownSeconds,
            int[] buffIds,
            int buffDurationSeconds,
            int maxRank,
            int skillPointCost,
            int restoreMana = 0)
        {
            SkillDefinition def = CreateBase(
                internalName,
                displayName,
                description,
                requiredJob,
                requiredLevel,
                SkillType.Active,
                resourceType,
                resourceCost,
                cooldownSeconds,
                maxRank,
                skillPointCost);

            def.BuffIds = buffIds;
            def.BuffDurationSeconds = buffDurationSeconds;
            def.RestoreMana = restoreMana;
            return def;
        }

        private static SkillDefinition CreateHealSkill(
            string internalName,
            string displayName,
            string description,
            JobType requiredJob,
            int requiredLevel,
            ResourceType resourceType,
            int resourceCost,
            float cooldownSeconds,
            int healAmount,
            int[] buffIds,
            int buffDurationSeconds,
            int maxRank,
            int skillPointCost)
        {
            SkillDefinition def = CreateBuffSkill(
                internalName,
                displayName,
                description,
                requiredJob,
                requiredLevel,
                resourceType,
                resourceCost,
                cooldownSeconds,
                buffIds,
                buffDurationSeconds,
                maxRank,
                skillPointCost);

            def.HealAmount = healAmount;
            return def;
        }

        private static SkillDefinition CreateAoeHealSkill(
            string internalName,
            string displayName,
            string description,
            JobType requiredJob,
            int requiredLevel,
            ResourceType resourceType,
            int resourceCost,
            float cooldownSeconds,
            int aoeDamage,
            float aoeRadius,
            DamageClass damageClass,
            int healAmount,
            int maxRank,
            int skillPointCost)
        {
            SkillDefinition def = CreateAoeSkill(
                internalName,
                displayName,
                description,
                requiredJob,
                requiredLevel,
                resourceType,
                resourceCost,
                cooldownSeconds,
                aoeDamage,
                aoeRadius,
                damageClass,
                maxRank,
                skillPointCost);

            def.HealAmount = healAmount;
            return def;
        }

        private static SkillDefinition CreatePassiveSkill(
            string internalName,
            string displayName,
            string description,
            JobType requiredJob,
            int requiredLevel,
            int maxRank,
            int skillPointCost,
            float damageBonus = 0f,
            float critBonus = 0f,
            float attackSpeedBonus = 0f,
            float moveSpeedBonus = 0f,
            int defenseBonus = 0,
            int minionSlots = 0,
            float manaCostReduction = 0f,
            int lifeRegenBonus = 0)
        {
            SkillDefinition def = CreateBase(
                internalName,
                displayName,
                description,
                requiredJob,
                requiredLevel,
                SkillType.Passive,
                ResourceType.None,
                0,
                0f,
                maxRank,
                skillPointCost);

            def.PassiveDamageBonus = damageBonus;
            def.PassiveCritBonus = critBonus;
            def.PassiveAttackSpeedBonus = attackSpeedBonus;
            def.PassiveMoveSpeedBonus = moveSpeedBonus;
            def.PassiveDefenseBonus = defenseBonus;
            def.PassiveMinionSlots = minionSlots;
            def.PassiveManaCostReduction = manaCostReduction;
            def.PassiveLifeRegenBonus = lifeRegenBonus;
            return def;
        }
    }
}
