using Terraria;
using Terraria.ID;
using Terraria.DataStructures;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using RpgMod.Common;
using RpgMod.Common.Players;
using RpgMod.Common.Compatibility;
using RpgMod.Common.Systems;
using System;

namespace RpgMod.Common.Commands
{
    /// <summary>
    /// Debug command to set player level
    /// Usage: /rpglevel [level]
    /// </summary>
    public class LevelCommand : ModCommand
    {
        public override string Command => "rpglevel";
        public override string Description => "Set player level. Usage: /rpglevel [level]";
        public override CommandType Type => CommandType.Chat;
        
        public override void Action(CommandCaller caller, string input, string[] args)
        {
            if (args.Length < 1 || !int.TryParse(args[0], out int level))
            {
                Main.NewText("Usage: /rpglevel [level]", Color.Yellow);
                return;
            }
            
            var rpgPlayer = caller.Player.GetModPlayer<RpgPlayer>();
            rpgPlayer.Level = Math.Clamp(level, 1, 999);
            rpgPlayer.CurrentXP = 0;
            
            Main.NewText($"Level set to {rpgPlayer.Level}", Color.Green);
        }
    }
    
    /// <summary>
    /// Debug command to add XP
    /// Usage: /rpgxp [amount]
    /// </summary>
    public class XPCommand : ModCommand
    {
        public override string Command => "rpgxp";
        public override string Description => "Add XP. Usage: /rpgxp [amount]";
        public override CommandType Type => CommandType.Chat;
        
        public override void Action(CommandCaller caller, string input, string[] args)
        {
            if (args.Length < 1 || !long.TryParse(args[0], out long xp))
            {
                Main.NewText("Usage: /rpgxp [amount]", Color.Yellow);
                return;
            }
            
            var playerLevel = caller.Player.GetModPlayer<PlayerLevel>();
            playerLevel.GainExperience(xp, XPSource.Quest, 0);
            
            Main.NewText($"Added {xp} XP", Color.Green);
        }
    }
    
    /// <summary>
    /// Debug command to set job
    /// Usage: /rpgjob [jobname]
    /// </summary>
    public class JobCommand : ModCommand
    {
        public override string Command => "rpgjob";
        public override string Description => "Set job. Usage: /rpgjob [jobname]";
        public override CommandType Type => CommandType.Chat;
        
        public override void Action(CommandCaller caller, string input, string[] args)
        {
            if (args.Length < 1)
            {
                Main.NewText("Usage: /rpgjob [jobname]", Color.Yellow);
                Main.NewText("Available: Novice, Warrior, Ranger, Mage, Summoner, Knight, Berserker, etc.", Color.Gray);
                return;
            }
            
            string jobName = args[0];
            if (Enum.TryParse<JobType>(jobName, true, out JobType jobType))
            {
                var rpgPlayer = caller.Player.GetModPlayer<RpgPlayer>();
                rpgPlayer.CurrentJob = jobType;
                Main.NewText($"Job set to {jobType}", Color.Green);
            }
            else
            {
                Main.NewText($"Unknown job: {jobName}", Color.Red);
            }
        }
    }
    
    /// <summary>
    /// Debug command to add stat points
    /// Usage: /rpgstats [amount]
    /// </summary>
    public class StatsCommand : ModCommand
    {
        public override string Command => "rpgstats";
        public override string Description => "Add stat points. Usage: /rpgstats [amount]";
        public override CommandType Type => CommandType.Chat;
        
        public override void Action(CommandCaller caller, string input, string[] args)
        {
            if (args.Length < 1 || !int.TryParse(args[0], out int points))
            {
                Main.NewText("Usage: /rpgstats [amount]", Color.Yellow);
                return;
            }
            
            var rpgPlayer = caller.Player.GetModPlayer<RpgPlayer>();
            rpgPlayer.StatPoints += points;
            
            Main.NewText($"Added {points} stat points (Total: {rpgPlayer.StatPoints})", Color.Green);
        }
    }
    
    /// <summary>
    /// Debug command to add skill points
    /// Usage: /rpgskillpoints [amount]
    /// </summary>
    public class SkillPointsCommand : ModCommand
    {
        public override string Command => "rpgskillpoints";
        public override string Description => "Add skill points. Usage: /rpgskillpoints [amount]";
        public override CommandType Type => CommandType.Chat;
        
        public override void Action(CommandCaller caller, string input, string[] args)
        {
            if (args.Length < 1 || !int.TryParse(args[0], out int points))
            {
                Main.NewText("Usage: /rpgskillpoints [amount]", Color.Yellow);
                return;
            }
            
            var rpgPlayer = caller.Player.GetModPlayer<RpgPlayer>();
            rpgPlayer.SkillPoints += points;
            
            Main.NewText($"Added {points} skill points (Total: {rpgPlayer.SkillPoints})", Color.Green);
        }
    }
    
    /// <summary>
    /// Debug command to set world level
    /// Usage: /rpgworldlevel [level]
    /// </summary>
    public class WorldLevelCommand : ModCommand
    {
        public override string Command => "rpgworldlevel";
        public override string Description => "Set world level. Usage: /rpgworldlevel [level]";
        public override CommandType Type => CommandType.Chat;
        
        public override void Action(CommandCaller caller, string input, string[] args)
        {
            if (args.Length < 1 || !int.TryParse(args[0], out int level))
            {
                Main.NewText("Usage: /rpgworldlevel [level]", Color.Yellow);
                return;
            }
            
            RpgWorld.SetWorldLevel(level);
            Main.NewText($"World level set to {level}", Color.Green);
        }
    }
    
    /// <summary>
    /// Debug command to show player info
    /// Usage: /rpginfo
    /// </summary>
    public class InfoCommand : ModCommand
    {
        public override string Command => "rpginfo";
        public override string Description => "Show player RPG info";
        public override CommandType Type => CommandType.Chat;
        
        public override void Action(CommandCaller caller, string input, string[] args)
        {
            var rpgPlayer = caller.Player.GetModPlayer<RpgPlayer>();
            
            Main.NewText("=== RPG Player Info ===", Color.Cyan);
            Main.NewText($"Level: {rpgPlayer.Level} | XP: {rpgPlayer.CurrentXP}/{rpgPlayer.RequiredXP}", Color.White);
            Main.NewText($"Job: {rpgPlayer.CurrentJob} (Tier {rpgPlayer.CurrentTier})", Color.White);
            Main.NewText($"Stat Points: {rpgPlayer.StatPoints} | Skill Points: {rpgPlayer.SkillPoints}", Color.White);
            Main.NewText($"STR: {rpgPlayer.Strength} DEX: {rpgPlayer.Dexterity} INT: {rpgPlayer.Intelligence}", Color.LightGreen);
            Main.NewText($"VIT: {rpgPlayer.Vitality} AGI: {rpgPlayer.Agility} LUK: {rpgPlayer.Luck}", Color.LightGreen);
            Main.NewText($"World Level: {RpgWorld.GetWorldLevel()} | Level Cap: {RpgFormulas.GetMaxLevel()}", Color.Yellow);
        }
    }
    
    /// <summary>
    /// Debug command to reset character
    /// Usage: /rpgreset [confirm]
    /// </summary>
    public class ResetCommand : ModCommand
    {
        public override string Command => "rpgreset";
        public override string Description => "Reset RPG character. Usage: /rpgreset confirm";
        public override CommandType Type => CommandType.Chat;
        
        public override void Action(CommandCaller caller, string input, string[] args)
        {
            if (args.Length < 1 || args[0].ToLower() != "confirm")
            {
                Main.NewText("This will reset your RPG character to Level 1 Novice!", Color.Red);
                Main.NewText("Type '/rpgreset confirm' to proceed", Color.Yellow);
                return;
            }
            
            var rpgPlayer = caller.Player.GetModPlayer<RpgPlayer>();
            rpgPlayer.Initialize();

            // Clear learned skills and refund points
            var skillManager = caller.Player.GetModPlayer<Skills.SkillManager>();
            skillManager?.ResetAllSkills();

            // Reset pending/available points
            rpgPlayer.StatPoints = 0;
            rpgPlayer.SkillPoints = 0;
            rpgPlayer.PendingStatPoints = 0;
            rpgPlayer.PendingSkillPoints = 0;

            Main.NewText("Character reset to Level 1 Novice! Stat/Skill points refunded.", Color.Green);
        }
    }

    /// <summary>
    /// Clear all skill hotbar assignments (skills/macros)
    /// Usage: /rpgclearhotbar
    /// </summary>
    public class ClearHotbarCommand : ModCommand
    {
        public override string Command => "rpgclearhotbar";
        public override string Description => "Clear all skill hotbar assignments";
        public override CommandType Type => CommandType.Chat;

        public override void Action(CommandCaller caller, string input, string[] args)
        {
            var skillManager = caller.Player.GetModPlayer<Skills.SkillManager>();
            skillManager?.ClearHotbar();
        }
    }
    
    /// <summary>
    /// Debug command to unlock all bosses for testing
    /// Usage: /rpgunlockbosses
    /// </summary>
    public class UnlockBossesCommand : ModCommand
    {
        public override string Command => "rpgunlockbosses";
        public override string Description => "Unlock all boss flags for testing";
        public override CommandType Type => CommandType.Chat;
        
        public override void Action(CommandCaller caller, string input, string[] args)
        {
            // Pre-Hardmode
            NPC.downedSlimeKing = true;
            NPC.downedBoss1 = true; // Eye
            NPC.downedBoss2 = true; // Eater/Brain
            NPC.downedQueenBee = true;
            NPC.downedBoss3 = true; // Skeletron
            NPC.downedDeerclops = true;
            
            // Enter hardmode
            Main.hardMode = true;
            
            // Hardmode
            NPC.downedQueenSlime = true;
            NPC.downedMechBoss1 = true;
            NPC.downedMechBoss2 = true;
            NPC.downedMechBoss3 = true;
            NPC.downedPlantBoss = true;
            NPC.downedGolemBoss = true;
            NPC.downedFishron = true;
            NPC.downedEmpressOfLight = true;
            NPC.downedAncientCultist = true;
            NPC.downedMoonlord = true;
            
            // Recalculate world level
            WorldProgression.RecalculateWorldLevel();
            
            Main.NewText("All bosses unlocked! Level cap is now unlimited.", Color.Green);
            Main.NewText($"World Level: {RpgWorld.GetWorldLevel()} | Level Cap: {RpgFormulas.GetMaxLevel()}", Color.Yellow);
        }
    }

    /// <summary>
    /// Debug command to show damage and crit multipliers
    /// Usage: /rpgdmg
    /// </summary>
    public class DamageInfoCommand : ModCommand
    {
        public override string Command => "rpgdmg";
        public override string Description => "Show stat bonuses and final damage multipliers";
        public override CommandType Type => CommandType.Chat;

        public override void Action(CommandCaller caller, string input, string[] args)
        {
            Player player = caller.Player;
            var rpgPlayer = player.GetModPlayer<RpgPlayer>();

            float strBonus = RpgFormulas.GetStrengthDamageBonus(rpgPlayer.TotalStrength, rpgPlayer.CurrentJob);
            float dexBonus = RpgFormulas.GetDexterityDamageBonus(rpgPlayer.TotalDexterity, rpgPlayer.CurrentJob);
            float rogueBonus = RpgFormulas.GetRogueDamageBonus(rpgPlayer.TotalRogue, rpgPlayer.CurrentJob);
            float intBonus = RpgFormulas.GetIntelligenceDamageBonus(rpgPlayer.TotalIntelligence, rpgPlayer.CurrentJob);
            float spellPower = rpgPlayer.TotalIntelligence * RpgConstants.INTELLIGENCE_SPELL_POWER_PER_POINT;
            float focBonus = RpgFormulas.GetSummonDamageBonus(rpgPlayer.TotalFocus, rpgPlayer.CurrentJob);
            float lukBonus = rpgPlayer.TotalLuck * RpgConstants.LUCK_ALL_DAMAGE_PER_POINT;

            Main.NewText("=== RPG Damage Info ===", Color.Cyan);
            Main.NewText($"STR +{strBonus * 100f:0.#}% | DEX +{dexBonus * 100f:0.#}% | ROG +{rogueBonus * 100f:0.#}%", Color.LightGreen);
            Main.NewText($"INT +{intBonus * 100f:0.#}% + SP {spellPower * 100f:0.#}% | FOC +{focBonus * 100f:0.#}% | LUK +{lukBonus * 100f:0.#}%", Color.LightGreen);

            float melee = player.GetDamage(DamageClass.Melee).Additive;
            float ranged = player.GetDamage(DamageClass.Ranged).Additive;
            float magic = player.GetDamage(DamageClass.Magic).Additive;
            float summon = player.GetDamage(DamageClass.Summon).Additive;
            float summonMelee = player.GetDamage(DamageClass.SummonMeleeSpeed).Additive;
            float generic = player.GetDamage(DamageClass.Generic).Additive;

            Main.NewText($"Final Additive: Melee x{melee:0.###} | Ranged x{ranged:0.###} | Magic x{magic:0.###}", Color.Yellow);
            Main.NewText($"Final Additive: Summon x{summon:0.###} | Whip x{summonMelee:0.###} | Generic x{generic:0.###}", Color.Yellow);

            float meleeCrit = player.GetCritChance(DamageClass.Melee);
            float rangedCrit = player.GetCritChance(DamageClass.Ranged);
            float magicCrit = player.GetCritChance(DamageClass.Magic);
            float summonCrit = player.GetCritChance(DamageClass.Summon);
            float genericCrit = player.GetCritChance(DamageClass.Generic);

            Main.NewText($"Crit: Melee {meleeCrit:0.#}% | Ranged {rangedCrit:0.#}% | Magic {magicCrit:0.#}% | Summon {summonCrit:0.#}% | Generic {genericCrit:0.#}%", Color.LightGray);
        }
    }

    /// <summary>
    /// Debug command to run a simple DPS test
    /// Usage: /rpgdps [seconds] | /rpgdps cancel
    /// </summary>
    public class DpsTestCommand : ModCommand
    {
        public override string Command => "rpgdps";
        public override string Description => "Start a DPS test for the given seconds (default 10). Add 'dummy' to spawn target.";
        public override CommandType Type => CommandType.Chat;

        public override void Action(CommandCaller caller, string input, string[] args)
        {
            var rpgPlayer = caller.Player.GetModPlayer<RpgPlayer>();

            if (args.Length > 0 && args[0].Equals("cancel", StringComparison.OrdinalIgnoreCase))
            {
                rpgPlayer.CancelDpsTest();
                Main.NewText("DPS test cancelled.", Color.Yellow);
                return;
            }

            int seconds = 10;
            if (args.Length > 0 && int.TryParse(args[0], out int parsed))
                seconds = Math.Clamp(parsed, 1, 120);

            bool spawnDummy = false;
            for (int i = 0; i < args.Length; i++)
            {
                if (args[i].Equals("dummy", StringComparison.OrdinalIgnoreCase))
                {
                    spawnDummy = true;
                    break;
                }
            }

            if (spawnDummy && Main.netMode != NetmodeID.MultiplayerClient)
            {
                Vector2 spawnPos = caller.Player.Center + new Vector2(caller.Player.direction * 120f, 0f);
                NPC.NewNPC(new EntitySource_Misc("RpgDpsDummy"), (int)spawnPos.X, (int)spawnPos.Y, NPCID.TargetDummy);
            }

            rpgPlayer.StartDpsTest(seconds);
            Main.NewText($"DPS test started for {seconds}s. Deal damage now.", Color.LightGreen);
        }
    }

    /// <summary>
    /// Debug command to spawn or clear target dummies
    /// Usage: /rpgdummy [count] | /rpgdummy clear
    /// </summary>
    public class DummyCommand : ModCommand
    {
        public override string Command => "rpgdummy";
        public override string Description => "Spawn target dummy for testing";
        public override CommandType Type => CommandType.Chat;

        public override void Action(CommandCaller caller, string input, string[] args)
        {
            if (Main.netMode == NetmodeID.MultiplayerClient)
            {
                Main.NewText("Dummy spawn is server-side only.", Color.Orange);
                return;
            }

            Player player = caller.Player;

            if (args.Length > 0 && args[0].Equals("clear", StringComparison.OrdinalIgnoreCase))
            {
                int cleared = 0;
                const float radius = 2000f;
                for (int i = 0; i < Main.maxNPCs; i++)
                {
                    NPC npc = Main.npc[i];
                    if (!npc.active || npc.type != NPCID.TargetDummy)
                        continue;
                    if (Vector2.Distance(npc.Center, player.Center) > radius)
                        continue;

                    npc.active = false;
                    npc.netUpdate = true;
                    cleared++;
                }

                Main.NewText($"Cleared {cleared} target dummies nearby.", Color.Yellow);
                return;
            }

            int count = 1;
            if (args.Length > 0 && int.TryParse(args[0], out int parsed))
                count = Math.Clamp(parsed, 1, 5);

            Vector2 basePos = player.Center + new Vector2(player.direction * 120f, 0f);
            for (int i = 0; i < count; i++)
            {
                Vector2 offset = new Vector2(i * 48f, 0f);
                Vector2 spawnPos = basePos + offset;
                NPC.NewNPC(new EntitySource_Misc("RpgDummy"), (int)spawnPos.X, (int)spawnPos.Y, NPCID.TargetDummy);
            }

            Main.NewText($"Spawned {count} target dummy(s).", Color.LightGreen);
        }
    }

    /// <summary>
    /// Debug command to show mod compatibility status
    /// Usage: /rpgmodinfo
    /// </summary>
    public class ModInfoCommand : ModCommand
    {
        public override string Command => "rpgmodinfo";
        public override string Description => "Show mod compatibility and damage class status";
        public override CommandType Type => CommandType.Chat;

        public override void Action(CommandCaller caller, string input, string[] args)
        {
            Main.NewText("=== Mod Compatibility ===", Color.Cyan);
            Main.NewText($"Calamity: {(ModCompatibilitySystem.CalamityLoaded ? "Detected" : "Not Found")}", Color.White);
            Main.NewText($"Thorium: {(ModCompatibilitySystem.ThoriumLoaded ? "Detected" : "Not Found")}", Color.White);
            Main.NewText($"Spirit: {(ModCompatibilitySystem.SpiritLoaded ? "Detected" : "Not Found")}", Color.White);
            Main.NewText($"Stars Above: {(ModCompatibilitySystem.StarsAboveLoaded ? "Detected" : "Not Found")}", Color.White);
            Main.NewText($"Fargo's Souls: {(ModCompatibilitySystem.FargosLoaded ? "Detected" : "Not Found")}", Color.White);

            var rogue = ModCompatibilitySystem.GetCalamityRogueClass();
            var bard = ModCompatibilitySystem.GetThoriumBardClass();
            var healer = ModCompatibilitySystem.GetThoriumHealerClass();

            Main.NewText($"Calamity Rogue Class: {(rogue != null ? rogue.Name : "None")}", Color.LightGray);
            Main.NewText($"Thorium Bard Class: {(bard != null ? bard.Name : "None")}", Color.LightGray);
            Main.NewText($"Thorium Healer Class: {(healer != null ? healer.Name : "None")}", Color.LightGray);
        }
    }

    /// <summary>
    /// Debug command to preview stat effects
    /// Usage: /rpgstatcalc [stat] [points]
    /// </summary>
    public class StatCalcCommand : ModCommand
    {
        public override string Command => "rpgstatcalc";
        public override string Description => "Preview stat effects for given points";
        public override CommandType Type => CommandType.Chat;

        public override void Action(CommandCaller caller, string input, string[] args)
        {
            if (args.Length < 2)
            {
                Main.NewText("Usage: /rpgstatcalc [stat] [points]", Color.Yellow);
                Main.NewText("Stats: STR DEX ROG INT FOC VIT STA DEF AGI WIS FOR LUK", Color.Gray);
                return;
            }

            if (!TryParseStat(args[0], out StatType stat))
            {
                Main.NewText($"Unknown stat: {args[0]}", Color.Red);
                return;
            }

            if (!int.TryParse(args[1], out int points) || points <= 0)
            {
                Main.NewText("Points must be a positive integer.", Color.Yellow);
                return;
            }

            var rpgPlayer = caller.Player.GetModPlayer<RpgPlayer>();
            Main.NewText($"=== Stat Preview: {stat} (+{points}) ===", Color.Cyan);

            switch (stat)
            {
                case StatType.Strength:
                    Main.NewText($"+{points * RpgConstants.STRENGTH_MELEE_DAMAGE_PER_POINT * 100f:0.#}% Melee Damage", Color.LightGreen);
                    break;
                case StatType.Dexterity:
                    Main.NewText($"+{points * RpgConstants.DEXTERITY_RANGED_DAMAGE_PER_POINT * 100f:0.#}% Ranged Damage", Color.LightGreen);
                    Main.NewText($"+{points * RpgConstants.DEXTERITY_ATTACK_SPEED_PER_POINT * 100f:0.#}% Attack Speed", Color.LightGreen);
                    Main.NewText($"+{points * RpgConstants.DEXTERITY_RANGED_CRIT_PER_POINT * 100f:0.#}% Ranged Crit", Color.LightGreen);
                    break;
                case StatType.Rogue:
                    Main.NewText($"+{points * RpgConstants.ROGUE_FINESSE_DAMAGE_PER_POINT * 100f:0.#}% Melee/Ranged Damage", Color.LightGreen);
                    Main.NewText($"+{points * RpgConstants.ROGUE_CRIT_CHANCE_PER_POINT * 100f:0.#}% Crit Chance", Color.LightGreen);
                    break;
                case StatType.Intelligence:
                    Main.NewText($"+{points * RpgConstants.INTELLIGENCE_MAGIC_DAMAGE_PER_POINT * 100f:0.#}% Magic Damage", Color.LightGreen);
                    Main.NewText($"+{points * RpgConstants.INTELLIGENCE_MAGIC_CRIT_PER_POINT * 100f:0.#}% Magic Crit", Color.LightGreen);
                    Main.NewText($"+{points * RpgConstants.INTELLIGENCE_SPELL_POWER_PER_POINT * 100f:0.#}% Spell Power", Color.LightGreen);
                    Main.NewText($"+{points * RpgConstants.INTELLIGENCE_MANA_COST_REDUCTION_PER_POINT * 100f:0.#}% Mana Cost Reduction", Color.LightGreen);
                    break;
                case StatType.Focus:
                    {
                        int currentFocus = rpgPlayer.TotalFocus;
                        int newFocus = currentFocus + points;
                        int slotsBefore = RpgFormulas.GetFocusMinionSlotBonus(currentFocus);
                        int slotsAfter = RpgFormulas.GetFocusMinionSlotBonus(newFocus);
                        int slotDelta = slotsAfter - slotsBefore;

                        Main.NewText($"+{points * RpgConstants.FOCUS_SUMMON_DAMAGE_PER_POINT * 100f:0.#}% Summon Damage", Color.LightGreen);
                        Main.NewText($"+{slotDelta} Minion Slot(s) (FOC {currentFocus} -> {newFocus})", Color.LightGreen);
                        break;
                    }
                case StatType.Vitality:
                    Main.NewText($"+{points * RpgConstants.VITALITY_HP_PER_POINT} Max HP", Color.LightGreen);
                    Main.NewText($"+{points * RpgConstants.VITALITY_HP_REGEN_PER_POINT:0.###} HP Regen/sec", Color.LightGreen);
                    break;
                case StatType.Stamina:
                    Main.NewText($"+{points * RpgConstants.STAMINA_MAX_PER_POINT} Max Stamina", Color.LightGreen);
                    Main.NewText($"+{points * RpgConstants.STAMINA_REGEN_PER_POINT:0.###} Stamina Regen/sec", Color.LightGreen);
                    break;
                case StatType.Defense:
                    {
                        int current = rpgPlayer.TotalDefense;
                        int newValue = current + points;
                        int armorBefore = (current / 5) * RpgConstants.DEFENSE_ARMOR_PER_5_POINTS;
                        int armorAfter = (newValue / 5) * RpgConstants.DEFENSE_ARMOR_PER_5_POINTS;
                        int armorDelta = armorAfter - armorBefore;

                        Main.NewText($"+{points * RpgConstants.DEFENSE_DAMAGE_REDUCTION_PER_POINT * 100f:0.#}% Damage Reduction", Color.LightGreen);
                        Main.NewText($"+{armorDelta} Armor (DEF {current} -> {newValue})", Color.LightGreen);
                        break;
                    }
                case StatType.Agility:
                    Main.NewText($"+{points * RpgConstants.AGILITY_MOVEMENT_SPEED_PER_POINT * 100f:0.#}% Move Speed", Color.LightGreen);
                    Main.NewText($"+{points * RpgConstants.AGILITY_DODGE_PER_POINT * 100f:0.#}% Dodge Chance", Color.LightGreen);
                    break;
                case StatType.Wisdom:
                    Main.NewText($"+{points * RpgConstants.WISDOM_MANA_PER_POINT} Max Mana", Color.LightGreen);
                    Main.NewText($"+{points * RpgConstants.WISDOM_MANA_REGEN_PER_POINT:0.###} Mana Regen/sec", Color.LightGreen);
                    break;
                case StatType.Fortitude:
                    Main.NewText($"+{points * RpgConstants.FORTITUDE_STATUS_RESISTANCE_PER_POINT * 100f:0.#}% Debuff Reduction", Color.LightGreen);
                    Main.NewText($"+{points * RpgConstants.FORTITUDE_KNOCKBACK_RESIST_PER_POINT * 100f:0.#}% Knockback Resist", Color.LightGreen);
                    Main.NewText($"+{points * RpgConstants.FORTITUDE_DEFENSE_PER_POINT * 100f:0.#}% Damage Reduction", Color.LightGreen);
                    break;
                case StatType.Luck:
                    Main.NewText($"+{points * RpgConstants.LUCK_CRIT_PER_POINT * 100f:0.#}% Crit Chance", Color.LightGreen);
                    Main.NewText($"+{points * RpgConstants.LUCK_DROP_RATE_PER_POINT * 100f:0.#}% Luck (drops/variance)", Color.LightGreen);
                    Main.NewText($"+{points * RpgConstants.LUCK_ALL_DAMAGE_PER_POINT * 100f:0.#}% All Damage", Color.LightGreen);
                    break;
            }
        }

        private static bool TryParseStat(string input, out StatType stat)
        {
            if (Enum.TryParse(input, true, out stat))
                return true;

            string key = input.Trim().ToUpperInvariant();
            stat = key switch
            {
                "STR" => StatType.Strength,
                "DEX" => StatType.Dexterity,
                "ROG" => StatType.Rogue,
                "INT" => StatType.Intelligence,
                "FOC" => StatType.Focus,
                "VIT" => StatType.Vitality,
                "STA" => StatType.Stamina,
                "DEF" => StatType.Defense,
                "AGI" => StatType.Agility,
                "WIS" => StatType.Wisdom,
                "FOR" => StatType.Fortitude,
                "LUK" => StatType.Luck,
                _ => StatType.Strength
            };

            return key is "STR" or "DEX" or "ROG" or "INT" or "FOC" or "VIT" or "STA" or "DEF" or "AGI" or "WIS" or "FOR" or "LUK";
        }
    }
}
