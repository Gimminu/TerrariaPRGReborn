using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Rpg.Common;
using Rpg.Common.Players;
using Rpg.Common.Systems;
using System;

namespace Rpg.Common.Commands
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
            float generic = player.GetDamage(DamageClass.Generic).Additive;

            Main.NewText($"Final Additive: Melee x{melee:0.###} | Ranged x{ranged:0.###} | Magic x{magic:0.###}", Color.Yellow);
            Main.NewText($"Final Additive: Summon x{summon:0.###} | Generic x{generic:0.###}", Color.Yellow);

            float meleeCrit = player.GetCritChance(DamageClass.Melee);
            float rangedCrit = player.GetCritChance(DamageClass.Ranged);
            float magicCrit = player.GetCritChance(DamageClass.Magic);
            float summonCrit = player.GetCritChance(DamageClass.Summon);
            float genericCrit = player.GetCritChance(DamageClass.Generic);

            Main.NewText($"Crit: Melee {meleeCrit:0.#}% | Ranged {rangedCrit:0.#}% | Magic {magicCrit:0.#}% | Summon {summonCrit:0.#}% | Generic {genericCrit:0.#}%", Color.LightGray);
        }
    }
}
