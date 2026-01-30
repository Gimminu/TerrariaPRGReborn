using Terraria;
using Terraria.ModLoader;
using Terraria.GameInput;

namespace Rpg.Common.Input
{
    /// <summary>
    /// DEPRECATED: UI keybinds are now unified in MasterUISystem.
    /// Use MasterUISystem.MasterUIKey (default: C) for the unified RPG Menu.
    /// This class is kept for compatibility only.
    /// </summary>
    public class RpgKeybindPlayer : ModPlayer
    {
        public override void ProcessTriggers(TriggersSet triggersSet)
        {
            // All UI keybinds are now handled by MasterUISystem
            // This class is kept for compatibility only
        }
    }
    
    // StatUIKeybind and SkillUIKeybind removed - use MasterUISystem instead
}
