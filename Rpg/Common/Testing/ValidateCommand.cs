using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Terraria;

namespace Rpg.Common.Testing
{
    /// <summary>
    /// Chat command to validate mod against design document.
    /// Usage: /rpgvalidate
    /// </summary>
    public class ValidateCommand : ModCommand
    {
        public override string Command => "rpgvalidate";
        
        public override string Description => "Validate RPG mod implementation against design document";
        
        public override CommandType Type => CommandType.Chat;
        
        public override void Action(CommandCaller caller, string input, string[] args)
        {
            Main.NewText("=== Validating RPG Implementation ===", Color.Cyan);
            
            try
            {
                DesignValidation.ValidateDesign();
            }
            catch (System.Exception ex)
            {
                Main.NewText($"[ERROR] Validation failed: {ex.Message}", Color.Red);
                Mod.Logger.Error($"Validation exception: {ex}");
            }
        }
    }
}
