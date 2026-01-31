using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI;
using RpgMod.Common.Config;

namespace RpgMod.Common.UI
{
    /// <summary>
    /// UI System for managing Player Status UI (always visible)
    /// </summary>
    public class PlayerStatusUISystem : ModSystem
    {
        private UserInterface statusInterface;
        private PlayerStatusUI statusUI;
        
        public override void Load()
        {
            if (!Main.dedServ)
            {
                statusInterface = new UserInterface();
                statusUI = new PlayerStatusUI();
                statusUI.Activate();
                
                // Automatically show UI when loaded
                statusInterface?.SetState(statusUI);
            }
        }
        
        public override void Unload()
        {
            statusInterface = null;
            statusUI = null;
        }
        
        public override void UpdateUI(GameTime gameTime)
        {
            if (statusInterface?.CurrentState != null)
            {
                statusInterface.Update(gameTime);
            }
        }
        
        public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
        {
            int mouseTextIndex = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Mouse Text"));
            if (mouseTextIndex != -1)
            {
                layers.Insert(mouseTextIndex, new LegacyGameInterfaceLayer(
                    "Rpg: Player Status",
                    delegate
                    {
                        if (Main.gameMenu)
                            return true;
                            
                        if (statusInterface?.CurrentState != null)
                        {
                            statusInterface.Draw(Main.spriteBatch, new GameTime());
                        }
                        return true;
                    },
                    InterfaceScaleType.UI
                ));
            }
        }
    }
    
}
