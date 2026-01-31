using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.GameInput;
using Terraria.ModLoader;
using Terraria.UI;

namespace RpgMod.Common.UI
{
    /// <summary>
    /// UI System for managing Job Advancement UI
    /// </summary>
    public class JobUISystem : ModSystem
    {
        private UserInterface jobInterface;
        private JobAdvancementUI jobUI;
        
        public override void Load()
        {
            if (!Main.dedServ)
            {
                jobInterface = new UserInterface();
                jobUI = new JobAdvancementUI();
                jobUI.Activate();
            }
        }
        
        public override void Unload()
        {
            jobInterface = null;
            jobUI = null;
        }
        
        public override void UpdateUI(GameTime gameTime)
        {
            if (jobInterface?.CurrentState != null)
            {
                jobInterface.Update(gameTime);
            }
        }
        
        public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
        {
            int mouseTextIndex = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Mouse Text"));
            if (mouseTextIndex != -1)
            {
                layers.Insert(mouseTextIndex, new LegacyGameInterfaceLayer(
                    "Rpg: Job UI",
                    delegate
                    {
                        if (jobInterface?.CurrentState != null)
                        {
                            jobInterface.Draw(Main.spriteBatch, new GameTime());
                        }
                        return true;
                    },
                    InterfaceScaleType.UI
                ));
            }
        }
        
        public void ShowUI()
        {
            jobInterface?.SetState(jobUI);
        }
        
        public void HideUI()
        {
            jobInterface?.SetState(null);
        }
        
        public void ToggleUI()
        {
            if (jobInterface?.CurrentState != null)
                HideUI();
            else
                ShowUI();
        }
    }
    
    // JobHotkeySystem and JobUIPlayer removed - use MasterUISystem instead
    // Job tab is now integrated into the unified RPG Menu (C key)
}
