using Terraria.ModLoader;

namespace RpgMod.Common.Skills
{
    /// <summary>
    /// Manages keybinds for skill hotkeys (1-9)
    /// </summary>
    public class SkillHotkeySystem : ModSystem
    {
        // Skill hotkeys (1-9)
        public static ModKeybind[] SkillKeybinds { get; private set; }
        
        public override void Load()
        {
            // Register 9 skill hotkeys, customizable in Controls settings
            SkillKeybinds = new ModKeybind[9];
            
            SkillKeybinds[0] = KeybindLoader.RegisterKeybind(Mod, "Skill Slot 1", "D1"); // 1 key
            SkillKeybinds[1] = KeybindLoader.RegisterKeybind(Mod, "Skill Slot 2", "D2"); // 2 key
            SkillKeybinds[2] = KeybindLoader.RegisterKeybind(Mod, "Skill Slot 3", "D3"); // 3 key
            SkillKeybinds[3] = KeybindLoader.RegisterKeybind(Mod, "Skill Slot 4", "D4"); // 4 key
            SkillKeybinds[4] = KeybindLoader.RegisterKeybind(Mod, "Skill Slot 5", "D5"); // 5 key
            SkillKeybinds[5] = KeybindLoader.RegisterKeybind(Mod, "Skill Slot 6", "D6"); // 6 key
            SkillKeybinds[6] = KeybindLoader.RegisterKeybind(Mod, "Skill Slot 7", "D7"); // 7 key
            SkillKeybinds[7] = KeybindLoader.RegisterKeybind(Mod, "Skill Slot 8", "D8"); // 8 key
            SkillKeybinds[8] = KeybindLoader.RegisterKeybind(Mod, "Skill Slot 9", "D9"); // 9 key
        }
        
        public override void Unload()
        {
            SkillKeybinds = null;
        }
    }
}
