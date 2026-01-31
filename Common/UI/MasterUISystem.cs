using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI;
using Terraria.GameInput;
using Terraria.Audio;
using Terraria.ID;

namespace RpgMod.Common.UI
{
    /// <summary>
    /// System for the unified Master UI (Stats + Job + Skills in one window)
    /// Single keybind: C (configurable)
    /// </summary>
    public class MasterUISystem : ModSystem
    {
        public static ModKeybind MasterUIKey { get; private set; }
        
        private UserInterface masterInterface;
        private MasterUI masterUI;
        private bool isVisible = false;
        
        public bool IsVisible => isVisible;
        
        public override void Load()
        {
            // Single unified keybind for the master UI
            MasterUIKey = KeybindLoader.RegisterKeybind(Mod, "Open RPG Menu", "C");
            
            if (!Main.dedServ)
            {
                masterInterface = new UserInterface();
                masterUI = new MasterUI();
                masterUI.Activate();
            }
        }
        
        public override void Unload()
        {
            MasterUIKey = null;
            masterInterface = null;
            masterUI = null;
        }
        
        public override void UpdateUI(GameTime gameTime)
        {
            // Auto-close when other menus open (keep inventory open)
            if (isVisible && (Main.ingameOptionsWindow || Main.inFancyUI || Main.gameMenu))
            {
                HideUI();
                return;
            }
            
            if (masterInterface?.CurrentState != null)
            {
                masterInterface.Update(gameTime);
            }
        }
        
        public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
        {
            int mouseTextIndex = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Mouse Text"));
            if (mouseTextIndex != -1)
            {
                layers.Insert(mouseTextIndex, new LegacyGameInterfaceLayer(
                    "Rpg: Master UI",
                    delegate
                    {
                        if (masterInterface?.CurrentState != null)
                        {
                            masterInterface.Draw(Main.spriteBatch, new GameTime());
                        }
                        return true;
                    },
                    InterfaceScaleType.UI
                ));
            }
        }
        
        public void ShowUI()
        {
            if (!isVisible)
            {
                masterInterface?.SetState(masterUI);
                isVisible = true;
                SoundEngine.PlaySound(SoundID.MenuOpen);
            }
        }
        
        public void HideUI()
        {
            if (isVisible)
            {
                masterInterface?.SetState(null);
                isVisible = false;
                SoundEngine.PlaySound(SoundID.MenuClose);
            }
        }
        
        public void ToggleUI()
        {
            if (isVisible)
                HideUI();
            else
                ShowUI();
        }
    }
    
    /// <summary>
    /// Player input handler for Master UI keybind
    /// </summary>
    public class MasterUIPlayer : ModPlayer
    {
        public override void ProcessTriggers(TriggersSet triggersSet)
        {
            // Don't process keybinds while in chat/menus
            if (Main.drawingPlayerChat || Main.editSign || Main.editChest ||
                Main.gameMenu || Main.ingameOptionsWindow || Main.inFancyUI)
                return;
                
            if (MasterUISystem.MasterUIKey.JustPressed)
            {
                var uiSystem = ModContent.GetInstance<MasterUISystem>();
                uiSystem.ToggleUI();
            }
        }
    }
}
