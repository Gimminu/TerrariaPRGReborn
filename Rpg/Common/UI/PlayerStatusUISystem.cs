using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI;
using Rpg.Common.Config;

namespace Rpg.Common.UI
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
            var config = ModContent.GetInstance<RpgClientConfig>();
            bool hideVanillaBars = config?.HideVanillaResourceBars ?? true;
            
            // Hide vanilla HP and Mana bars when our custom UI is active
            if (hideVanillaBars && !Main.gameMenu && !Main.ingameOptionsWindow && !Main.playerInventory && !Main.inFancyUI)
            {
                // Find and hide vanilla resource bars
                int resourceBarIndex = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Resource Bars"));
                if (resourceBarIndex != -1)
                {
                    layers.RemoveAt(resourceBarIndex);
                }
            }
            
            int mouseTextIndex = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Mouse Text"));
            if (mouseTextIndex != -1)
            {
                layers.Insert(mouseTextIndex, new LegacyGameInterfaceLayer(
                    "Rpg: Player Status",
                    delegate
                    {
                        // Don't draw when menus are open
                        if (Main.gameMenu || Main.ingameOptionsWindow || Main.playerInventory || Main.inFancyUI)
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
