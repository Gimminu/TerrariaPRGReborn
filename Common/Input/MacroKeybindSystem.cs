using Microsoft.Xna.Framework.Input;
using Terraria;
using Terraria.ModLoader;
using RpgMod.Common.Players;
using RpgMod.Common.Systems;

namespace RpgMod.Common.Input
{
    /// <summary>
    /// Handles keybinds for skill macros
    /// </summary>
    public class MacroKeybindSystem : ModSystem
    {
        public static ModKeybind Macro1Key { get; private set; }
        public static ModKeybind Macro2Key { get; private set; }
        public static ModKeybind Macro3Key { get; private set; }
        public static ModKeybind Macro4Key { get; private set; }
        public static ModKeybind Macro5Key { get; private set; }
        
        public override void Load()
        {
            Macro1Key = KeybindLoader.RegisterKeybind(Mod, "Macro 1", Keys.NumPad1);
            Macro2Key = KeybindLoader.RegisterKeybind(Mod, "Macro 2", Keys.NumPad2);
            Macro3Key = KeybindLoader.RegisterKeybind(Mod, "Macro 3", Keys.NumPad3);
            Macro4Key = KeybindLoader.RegisterKeybind(Mod, "Macro 4", Keys.NumPad4);
            Macro5Key = KeybindLoader.RegisterKeybind(Mod, "Macro 5", Keys.NumPad5);
        }
        
        public override void Unload()
        {
            Macro1Key = null;
            Macro2Key = null;
            Macro3Key = null;
            Macro4Key = null;
            Macro5Key = null;
        }
    }
    
    /// <summary>
    /// Player-side macro keybind handler
    /// </summary>
    public class MacroKeybindPlayer : ModPlayer
    {
        public override void ProcessTriggers(Terraria.GameInput.TriggersSet triggersSet)
        {
            if (Main.netMode == Terraria.ID.NetmodeID.Server)
                return;
            
            // Don't process while typing
            if (Main.drawingPlayerChat || Main.editSign || Main.editChest ||
                Main.gameMenu || Main.ingameOptionsWindow || Main.playerInventory || Main.inFancyUI)
                return;
            
            var macroSystem = Player.GetModPlayer<SkillMacroSystem>();
            
            // Execute macros
            if (MacroKeybindSystem.Macro1Key?.JustPressed == true)
            {
                macroSystem.ExecuteMacro(0);
            }
            else if (MacroKeybindSystem.Macro2Key?.JustPressed == true)
            {
                macroSystem.ExecuteMacro(1);
            }
            else if (MacroKeybindSystem.Macro3Key?.JustPressed == true)
            {
                macroSystem.ExecuteMacro(2);
            }
            else if (MacroKeybindSystem.Macro4Key?.JustPressed == true)
            {
                macroSystem.ExecuteMacro(3);
            }
            else if (MacroKeybindSystem.Macro5Key?.JustPressed == true)
            {
                macroSystem.ExecuteMacro(4);
            }
            
        }
    }
}
