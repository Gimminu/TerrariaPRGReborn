using System;
using System.Collections.Generic;
using Rpg.Common.Base;
using Rpg.Common;

namespace Rpg.Common.Skills
{
    /// <summary>
    /// Central registry for all skills in the mod
    /// </summary>
    public static class SkillDatabase
    {
        // Registry: skill internal name -> factory
        private static Dictionary<string, System.Func<BaseSkill>> skillRegistry = new Dictionary<string, System.Func<BaseSkill>>();
        
        /// <summary>
        /// Initialize and register all skills
        /// </summary>
        public static void Initialize()
        {
            skillRegistry.Clear();
            
            // ============================================
            // NOVICE (Tier 0) Skills
            // ============================================
            RegisterSkill(() => new Novice.BasicStrike());
            RegisterSkill(() => new Novice.SecondWind());
            RegisterSkill(() => new Novice.Resourceful());
            RegisterSkill(() => new Novice.Perseverance());
            RegisterSkill(() => new Novice.QuickRecovery());
            RegisterSkill(() => new Novice.Focus());
            RegisterSkill(() => new Novice.Toughness());
            RegisterSkill(() => new Novice.Nimble());
            RegisterSkill(() => new Novice.AdventurerSpirit());
            RegisterSkill(() => new Novice.LuckyFind());

            // ============================================
            // TIER 1 - First Job Skills
            // ============================================
            
            // Warrior
            RegisterSkill(() => new Tier1.Warrior.PowerStrike());
            RegisterSkill(() => new Tier1.Warrior.WarCry());
            RegisterSkill(() => new Tier1.Warrior.IronWill());
            RegisterSkill(() => new Tier1.Warrior.Charge());
            RegisterSkill(() => new Tier1.Warrior.GroundSlam());
            RegisterSkill(() => new Tier1.Warrior.Endurance());
            RegisterSkill(() => new Tier1.Warrior.WeaponMastery());
            RegisterSkill(() => new Tier1.Warrior.VitalForce());
            RegisterSkill(() => new Tier1.Warrior.ArmorMastery());
            RegisterSkill(() => new Tier1.Warrior.CombatRegeneration());
            
            // Ranger
            RegisterSkill(() => new Tier1.Ranger.RapidFire());
            RegisterSkill(() => new Tier1.Ranger.EvasiveManeuver());
            RegisterSkill(() => new Tier1.Ranger.Precision());
            RegisterSkill(() => new Tier1.Ranger.Multishot());
            RegisterSkill(() => new Tier1.Ranger.Camouflage());
            RegisterSkill(() => new Tier1.Ranger.ExplosiveArrow());
            RegisterSkill(() => new Tier1.Ranger.SharpEye());
            RegisterSkill(() => new Tier1.Ranger.FleetFoot());
            RegisterSkill(() => new Tier1.Ranger.AmmoConservation());
            RegisterSkill(() => new Tier1.Ranger.Agility());
            
            // Mage
            RegisterSkill(() => new Tier1.Mage.MagicBarrier());
            RegisterSkill(() => new Tier1.Mage.Blink());
            RegisterSkill(() => new Tier1.Mage.ArcanePower());
            RegisterSkill(() => new Tier1.Mage.Fireball());
            RegisterSkill(() => new Tier1.Mage.FrostNova());
            RegisterSkill(() => new Tier1.Mage.ManaShield());
            RegisterSkill(() => new Tier1.Mage.MagicMastery());
            RegisterSkill(() => new Tier1.Mage.ManaFlow());
            RegisterSkill(() => new Tier1.Mage.SpellEfficiency());
            RegisterSkill(() => new Tier1.Mage.ArcaneKnowledge());
            
            // Summoner
            RegisterSkill(() => new Tier1.Summoner.SummonAlly());
            RegisterSkill(() => new Tier1.Summoner.CommandMinions());
            RegisterSkill(() => new Tier1.Summoner.SummonMastery());
            RegisterSkill(() => new Tier1.Summoner.MinionFrenzy());
            RegisterSkill(() => new Tier1.Summoner.GuardianSpirit());
            RegisterSkill(() => new Tier1.Summoner.SoulHarvest());
            RegisterSkill(() => new Tier1.Summoner.SpiritLink());
            RegisterSkill(() => new Tier1.Summoner.CommandFocus());
            RegisterSkill(() => new Tier1.Summoner.MinionMastery());
            RegisterSkill(() => new Tier1.Summoner.WhipMastery());
            RegisterSkill(() => new Tier1.Summoner.SoulBond());
            RegisterSkill(() => new Tier1.Summoner.SpiritEndurance());
            RegisterSkill(() => new Tier1.Summoner.SentryMastery());

            // ============================================
            // TIER 2 - Second Job Skills
            // ============================================
            
            // Knight (Warrior -> Knight)
            RegisterSkill(() => new Tier2.Knight.ShieldWall());
            RegisterSkill(() => new Tier2.Knight.Taunt());
            RegisterSkill(() => new Tier2.Knight.IronSkin());
            
            // Berserker (Warrior -> Berserker)
            RegisterSkill(() => new Tier2.Berserker.Berserk());
            RegisterSkill(() => new Tier2.Berserker.Bloodlust());
            RegisterSkill(() => new Tier2.Berserker.LowHPPower());
            
            // Paladin (Warrior + Mage -> Paladin)
            RegisterSkill(() => new Tier2.Paladin.LayOnHands());
            RegisterSkill(() => new Tier2.Paladin.DivineBless());
            RegisterSkill(() => new Tier2.Paladin.HolyAura());
            
            // DeathKnight (Warrior + Mage -> DeathKnight)
            RegisterSkill(() => new Tier2.DeathKnight.DeathCoil());
            RegisterSkill(() => new Tier2.DeathKnight.UnholyAura());
            RegisterSkill(() => new Tier2.DeathKnight.Lichdom());
            
            // Sniper (Ranger -> Sniper)
            RegisterSkill(() => new Tier2.Sniper.Headshot());
            RegisterSkill(() => new Tier2.Sniper.SteadyBreath());
            RegisterSkill(() => new Tier2.Sniper.Sharpshooter());
            
            // Assassin (Ranger -> Assassin)
            RegisterSkill(() => new Tier2.Assassin.Backstab());
            RegisterSkill(() => new Tier2.Assassin.Vanish());
            RegisterSkill(() => new Tier2.Assassin.PoisonMastery());
            RegisterSkill(() => new Tier2.Assassin.ToxicBlade());
            RegisterSkill(() => new Tier2.Assassin.LethalPrecision());
            RegisterSkill(() => new Tier2.Assassin.Evasion());
            RegisterSkill(() => new Tier2.Assassin.AmbushMaster());
            
            // Gunslinger (Ranger -> Gunslinger)
            RegisterSkill(() => new Tier2.Gunslinger.FanTheHammer());
            RegisterSkill(() => new Tier2.Gunslinger.ExplosiveRound());
            RegisterSkill(() => new Tier2.Gunslinger.QuickDraw());
            RegisterSkill(() => new Tier2.Gunslinger.HighNoon());
            RegisterSkill(() => new Tier2.Gunslinger.GunTempo());
            RegisterSkill(() => new Tier2.Gunslinger.AmmoExpert());
            RegisterSkill(() => new Tier2.Gunslinger.SteadyHand());
            RegisterSkill(() => new Tier2.Gunslinger.GunMastery());
            
            // Sorcerer (Mage -> Sorcerer)
            RegisterSkill(() => new Tier2.Sorcerer.ElementalBurst());
            RegisterSkill(() => new Tier2.Sorcerer.ElementalBarrier());
            RegisterSkill(() => new Tier2.Sorcerer.ElementalMastery());
            RegisterSkill(() => new Tier2.Sorcerer.MeteorStrike());
            RegisterSkill(() => new Tier2.Sorcerer.ArcaneSurge());
            RegisterSkill(() => new Tier2.Sorcerer.ManaShieldSorcerer());
            RegisterSkill(() => new Tier2.Sorcerer.ManaRegeneration());
            RegisterSkill(() => new Tier2.Sorcerer.SpellAmplification());
            
            // Cleric (Mage -> Cleric)
            RegisterSkill(() => new Tier2.Cleric.HealingPrayer());
            RegisterSkill(() => new Tier2.Cleric.Sanctuary());
            RegisterSkill(() => new Tier2.Cleric.HolyLight());
            RegisterSkill(() => new Tier2.Cleric.HolyFortitude());
            RegisterSkill(() => new Tier2.Cleric.HolyArmor());
            RegisterSkill(() => new Tier2.Cleric.Purify());
            
            // Archmage (Mage -> Archmage)
            RegisterSkill(() => new Tier2.Archmage.Meteor());
            RegisterSkill(() => new Tier2.Archmage.TimeWarp());
            RegisterSkill(() => new Tier2.Archmage.ArcaneMastery());
            RegisterSkill(() => new Tier2.Archmage.ArcaneBarrage());
            RegisterSkill(() => new Tier2.Archmage.SupremeIntellect());
            RegisterSkill(() => new Tier2.Archmage.SpellPenetration());
            
            // Warlock (Mage -> Warlock)
            RegisterSkill(() => new Tier2.Warlock.Corruption());
            RegisterSkill(() => new Tier2.Warlock.DrainLife());
            RegisterSkill(() => new Tier2.Warlock.CurseExpert());
            RegisterSkill(() => new Tier2.Warlock.DarkBolt());
            RegisterSkill(() => new Tier2.Warlock.DarkPact());
            
            // Spellblade (Warrior + Mage Hybrid)
            RegisterSkill(() => new Tier2.Spellblade.EnchantWeapon());
            RegisterSkill(() => new Tier2.Spellblade.ArcaneSlash());
            RegisterSkill(() => new Tier2.Spellblade.SpellStrike());
            RegisterSkill(() => new Tier2.Spellblade.ArcaneStrike());
            RegisterSkill(() => new Tier2.Spellblade.EnchantedBlade());
            RegisterSkill(() => new Tier2.Spellblade.HybridMastery());
            RegisterSkill(() => new Tier2.Spellblade.ManaBlade());
            
            // BattleMage (Warrior + Mage Hybrid)
            RegisterSkill(() => new Tier2.BattleMage.MagicArmor());
            RegisterSkill(() => new Tier2.BattleMage.CounterSpell());
            RegisterSkill(() => new Tier2.BattleMage.BattleCaster());
            RegisterSkill(() => new Tier2.BattleMage.WarMagic());
            RegisterSkill(() => new Tier2.BattleMage.CombatTrance());
            RegisterSkill(() => new Tier2.BattleMage.BattleArcana());
            
            // Beastmaster (Ranger + Summoner Hybrid)
            RegisterSkill(() => new Tier2.Beastmaster.BeastCall());
            RegisterSkill(() => new Tier2.Beastmaster.PackTactics());
            RegisterSkill(() => new Tier2.Beastmaster.WildBond());
            RegisterSkill(() => new Tier2.Beastmaster.WildCall());
            RegisterSkill(() => new Tier2.Beastmaster.BeastBond());
            RegisterSkill(() => new Tier2.Beastmaster.PackLeader());
            
            // Necromancer (Summoner -> Necromancer)
            RegisterSkill(() => new Tier2.Necromancer.RaiseUndead());
            RegisterSkill(() => new Tier2.Necromancer.BoneArmor());
            RegisterSkill(() => new Tier2.Necromancer.UndeadMastery());
            RegisterSkill(() => new Tier2.Necromancer.RaiseDead());
            RegisterSkill(() => new Tier2.Necromancer.DeathCoil());
            RegisterSkill(() => new Tier2.Necromancer.DarkMastery());
            RegisterSkill(() => new Tier2.Necromancer.UndyingWill());
            
            // Druid (Mage + Summoner Hybrid)
            RegisterSkill(() => new Tier2.Druid.SummonBeast());
            RegisterSkill(() => new Tier2.Druid.NaturesWrath());
            RegisterSkill(() => new Tier2.Druid.Shapeshifter());
            RegisterSkill(() => new Tier2.Druid.NaturesBlessing());
            RegisterSkill(() => new Tier2.Druid.VineWhip());
            RegisterSkill(() => new Tier2.Druid.NatureAffinity());
            
            // Shadow (Warrior + Ranger Hybrid)
            RegisterSkill(() => new Tier2.Shadow.ShadowStep());
            RegisterSkill(() => new Tier2.Shadow.Ambush());
            RegisterSkill(() => new Tier2.Shadow.SilentBlades());
            RegisterSkill(() => new Tier2.Shadow.ShadowStrike());
            RegisterSkill(() => new Tier2.Shadow.ShadowCloak());
            RegisterSkill(() => new Tier2.Shadow.ShadowMastery());
            RegisterSkill(() => new Tier2.Shadow.DeadlyReflexes());
            
            // Spellthief (Ranger + Mage Hybrid)
            RegisterSkill(() => new Tier2.Spellthief.StealBuff());
            RegisterSkill(() => new Tier2.Spellthief.ArcaneTheft());
            RegisterSkill(() => new Tier2.Spellthief.MagicRogue());
            RegisterSkill(() => new Tier2.Spellthief.ManaSteal());
            RegisterSkill(() => new Tier2.Spellthief.SpellSnatch());
            RegisterSkill(() => new Tier2.Spellthief.ArcaneLarceny());
            RegisterSkill(() => new Tier2.Spellthief.ManaSiphon());

            // ============================================
            // TIER 3 - Third Job Skills
            // ============================================
            
            // Guardian (Knight -> Guardian)
            RegisterSkill(() => new Tier3.Guardian.AegisWall());
            RegisterSkill(() => new Tier3.Guardian.GuardianSpirit());
            RegisterSkill(() => new Tier3.Guardian.Unbreakable());
            RegisterSkill(() => new Tier3.Guardian.DivineBastion());
            RegisterSkill(() => new Tier3.Guardian.ImmortalWill());
            RegisterSkill(() => new Tier3.Guardian.AegisMaster());
            
            // Blood Knight (Berserker -> Blood Knight)
            RegisterSkill(() => new Tier3.BloodKnight.BloodRite());
            RegisterSkill(() => new Tier3.BloodKnight.CrimsonFrenzy());
            RegisterSkill(() => new Tier3.BloodKnight.SanguineArmor());
            RegisterSkill(() => new Tier3.BloodKnight.BloodStorm());
            RegisterSkill(() => new Tier3.BloodKnight.BloodPact());
            RegisterSkill(() => new Tier3.BloodKnight.CrimsonVitality());
            
            // Deadeye (Sniper -> Deadeye)
            RegisterSkill(() => new Tier3.Deadeye.MarkedShot());
            RegisterSkill(() => new Tier3.Deadeye.DeadeyeFocus());
            RegisterSkill(() => new Tier3.Deadeye.SniperInstinct());
            RegisterSkill(() => new Tier3.Deadeye.PerfectShot());
            RegisterSkill(() => new Tier3.Deadeye.EagleVision());
            RegisterSkill(() => new Tier3.Deadeye.MarkedForDeath());
            
            // Gunmaster (Gunslinger -> Gunmaster)
            RegisterSkill(() => new Tier3.Gunmaster.BulletStorm());
            RegisterSkill(() => new Tier3.Gunmaster.QuickReload());
            RegisterSkill(() => new Tier3.Gunmaster.RicochetMastery());
            RegisterSkill(() => new Tier3.Gunmaster.GunGod());
            RegisterSkill(() => new Tier3.Gunmaster.InfiniteAmmo());
            
            // Archbishop (Cleric -> Archbishop)
            RegisterSkill(() => new Tier3.Archbishop.MassHeal());
            RegisterSkill(() => new Tier3.Archbishop.SacredWard());
            RegisterSkill(() => new Tier3.Archbishop.DivineBlessing());
            RegisterSkill(() => new Tier3.Archbishop.DivineIntervention());
            RegisterSkill(() => new Tier3.Archbishop.SanctifiedBody());
            
            // Overlord (Beastmaster -> Overlord)
            RegisterSkill(() => new Tier3.Overlord.AlphaRoar());
            RegisterSkill(() => new Tier3.Overlord.BeastStampede());
            RegisterSkill(() => new Tier3.Overlord.OverlordCommand());
            RegisterSkill(() => new Tier3.Overlord.ArmyOfDarkness());
            RegisterSkill(() => new Tier3.Overlord.SupremeCommander());
            RegisterSkill(() => new Tier3.Overlord.DarkDominion());
            
            // Lich King (Necromancer -> Lich King)
            RegisterSkill(() => new Tier3.Lichking.SoulHarvest());
            RegisterSkill(() => new Tier3.Lichking.UndeadLegion());
            RegisterSkill(() => new Tier3.Lichking.Phylactery());
            RegisterSkill(() => new Tier3.Lichking.DeathsEmbrace());
            RegisterSkill(() => new Tier3.Lichking.NecroticMastery());
            RegisterSkill(() => new Tier3.Lichking.EternalDarkness());

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
            BaseSkill skill = factory();
            if (!skillRegistry.ContainsKey(skill.InternalName))
                skillRegistry.Add(skill.InternalName, factory);
        }
        
        /// <summary>
        /// Get skill by internal name (creates new instance)
        /// </summary>
        public static BaseSkill GetSkill(string internalName)
        {
            if (!skillRegistry.ContainsKey(internalName))
                return null;
            
            return skillRegistry[internalName]();
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
                BaseSkill skill = entry.Value();
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
                BaseSkill skill = entry.Value();
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
            return skillRegistry.ContainsKey(internalName);
        }
    }
}
