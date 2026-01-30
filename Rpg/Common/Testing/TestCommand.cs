using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Terraria;

namespace Rpg.Common.Testing
{
    /// <summary>
    /// Chat command to run RPG system tests.
    /// Usage: /rpgtest [category]
    /// Categories: all, skills, jobs, balance, performance
    /// </summary>
    public class TestCommand : ModCommand
    {
        public override string Command => "rpgtest";
        
        public override string Description => "Run RPG system tests. Usage: /rpgtest [all|skills|jobs|balance|perf]";
        
        public override CommandType Type => CommandType.Chat;
        
        public override void Action(CommandCaller caller, string input, string[] args)
        {
            string category = args.Length > 0 ? args[0].ToLower() : "all";
            
            Main.NewText("=== Starting RPG System Tests ===", Color.Cyan);
            Main.NewText($"Category: {category}", Color.Gray);
            
            try
            {
                RpgTestSystem.RunTests(category, verbose: true);
            }
            catch (System.Exception ex)
            {
                Main.NewText($"[ERROR] Test failed with exception: {ex.Message}", Color.Red);
                Mod.Logger.Error($"Test exception: {ex}");
            }
        }
    }
}
