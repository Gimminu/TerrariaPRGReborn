using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using RpgMod.Common.Players;
using RpgMod.Common.Systems;

namespace RpgMod.Common.Commands
{
    /// <summary>
    /// Help command to show available RPG commands
    /// </summary>
    public class HelpCommand : ModCommand
    {
        public override string Command => "rpghelp";
        public override string Description => "Show available RPG commands";
        public override CommandType Type => CommandType.Chat;
        
        public override void Action(CommandCaller caller, string input, string[] args)
        {
            Main.NewText("=== RPG Mod Commands ===", Color.Cyan);
            Main.NewText("/rpginfo - Show your RPG character info", Color.White);
            Main.NewText("/rpglevel [level] - Set level (debug)", Color.Gray);
            Main.NewText("/rpgxp [amount] - Add XP (debug)", Color.Gray);
            Main.NewText("/rpgjob [name] - Change job (debug)", Color.Gray);
            Main.NewText("/rpgstats [amount] - Add stat points (debug)", Color.Gray);
            Main.NewText("/rpgskillpoints [amount] - Add skill points (debug)", Color.Gray);
            Main.NewText("/rpgworldlevel [level] - Set world level (debug)", Color.Gray);
            Main.NewText("/rpgreset confirm - Reset character (debug)", Color.Gray);
            Main.NewText("/rpgunlockbosses - Unlock all bosses (debug)", Color.Gray);
            Main.NewText("/rpgclearhotbar - Clear all skill hotbar assignments", Color.White);
        }
    }
    
    /// <summary>
    /// Command to show current keybinds
    /// </summary>
    public class KeybindsCommand : ModCommand
    {
        public override string Command => "rpgkeys";
        public override string Description => "Show RPG keybinds";
        public override CommandType Type => CommandType.Chat;
        
        public override void Action(CommandCaller caller, string input, string[] args)
        {
            Main.NewText("=== RPG Mod Keybinds ===", Color.Cyan);
            Main.NewText("C - Open RPG Menu", Color.White);
            Main.NewText("O - Toggle Quest UI", Color.White);
            Main.NewText("1-9 - Use Skill in Hotbar Slot", Color.White);
            Main.NewText("NumPad 1-5 - Execute Macro", Color.White);
            Main.NewText("F - Dash", Color.White);
            Main.NewText("/rpgclearhotbar - Clear skill slot assignments", Color.Gray);
            Main.NewText("Customize in Settings > Controls > Mod Controls", Color.Yellow);
        }
    }
    
    /// <summary>
    /// Command to show progression info
    /// </summary>
    public class ProgressionCommand : ModCommand
    {
        public override string Command => "rpgprogression";
        public override string Description => "Show world progression and level caps";
        public override CommandType Type => CommandType.Chat;
        
        public override void Action(CommandCaller caller, string input, string[] args)
        {
            Main.NewText("=== World Progression ===", Color.Cyan);
            Main.NewText($"World Level: {RpgWorld.GetWorldLevel()}", Color.White);
            Main.NewText($"Progression Stage: {RpgWorld.GetProgressionStageName()}", Color.White);
            Main.NewText($"Current Level Cap: {RpgFormulas.GetMaxLevel()}", Color.Yellow);
            Main.NewText("", Color.White);
            
            Main.NewText("--- Level Cap by Progression ---", Color.Gray);
            Main.NewText($"Pre-Hardmode: Lv.{RpgConstants.PRE_HARDMODE_CAP}", Color.White);
            Main.NewText($"Early Hardmode: Lv.{RpgConstants.EARLY_HARDMODE_CAP}", Color.White);
            Main.NewText($"Post-Mech Bosses: Lv.{RpgConstants.POST_MECH_CAP}", Color.White);
            Main.NewText($"Pre-Moon Lord: Lv.{RpgConstants.PRE_MOONLORD_CAP}", Color.White);
            Main.NewText("Post-Moon Lord: Unlimited", Color.Gold);
        }
    }
    
    /// <summary>
    /// Command to show detailed stats breakdown
    /// </summary>
    public class DetailedStatsCommand : ModCommand
    {
        public override string Command => "rpgdetails";
        public override string Description => "Show detailed stat breakdown";
        public override CommandType Type => CommandType.Chat;
        
        public override void Action(CommandCaller caller, string input, string[] args)
        {
            var rpgPlayer = caller.Player.GetModPlayer<RpgPlayer>();
            
            Main.NewText("=== Detailed Stats ===", Color.Cyan);
            Main.NewText($"STR: {rpgPlayer.Strength} (+{rpgPlayer.Strength * RpgConstants.STRENGTH_MELEE_DAMAGE_PER_POINT * 100:0.#}% melee dmg)", Color.Red);
            Main.NewText($"DEX: {rpgPlayer.Dexterity} (+{rpgPlayer.Dexterity * RpgConstants.DEXTERITY_RANGED_DAMAGE_PER_POINT * 100:0.#}% ranged dmg)", Color.Green);
            Main.NewText($"INT: {rpgPlayer.Intelligence} (+{rpgPlayer.Intelligence * RpgConstants.INTELLIGENCE_MAGIC_DAMAGE_PER_POINT * 100:0.#}% magic dmg)", Color.Blue);
            Main.NewText($"VIT: {rpgPlayer.Vitality} (+{rpgPlayer.Vitality * RpgConstants.VITALITY_HP_PER_POINT} HP)", Color.Orange);
            Main.NewText($"AGI: {rpgPlayer.Agility} (+{rpgPlayer.Agility * RpgConstants.AGILITY_MOVEMENT_SPEED_PER_POINT * 100:0.#}% move speed)", Color.Cyan);
            Main.NewText($"LUK: {rpgPlayer.Luck} (+{rpgPlayer.Luck * RpgConstants.LUCK_CRIT_PER_POINT * 100:0.#}% crit)", Color.Yellow);
            
            Main.NewText("", Color.White);
            Main.NewText($"Exp Multiplier: {rpgPlayer.ExpMultiplier:0.##}x", Color.LightGreen);
        }
    }
}
