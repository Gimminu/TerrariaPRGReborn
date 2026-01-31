using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI;

namespace RpgMod.Common.UI
{
    /// <summary>
    /// UI System for managing Stat Allocation UI
    /// </summary>
    public class StatUISystem : ModSystem
    {
        private UserInterface statInterface;
        private StatAllocationUI statUI;
        
        public override void Load()
        {
            if (!Main.dedServ)
            {
                statInterface = new UserInterface();
                statUI = new StatAllocationUI();
                statUI.Activate();
            }
        }
        
        public override void UpdateUI(GameTime gameTime)
        {
            if (statInterface?.CurrentState != null)
            {
                statInterface.Update(gameTime);
            }
        }
        
        public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
        {
            int mouseTextIndex = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Mouse Text"));
            if (mouseTextIndex != -1)
            {
                layers.Insert(mouseTextIndex, new LegacyGameInterfaceLayer(
                    "Rpg: Stat UI",
                    delegate
                    {
                        if (statInterface?.CurrentState != null)
                        {
                            statInterface.Draw(Main.spriteBatch, new GameTime());
                        }
                        return true;
                    },
                    InterfaceScaleType.UI
                ));
            }
        }
        
        /// <summary>
        /// Show the stat allocation UI
        /// </summary>
        public void ShowUI()
        {
            statInterface?.SetState(statUI);
        }
        
        /// <summary>
        /// Hide the stat allocation UI
        /// </summary>
        public void HideUI()
        {
            statInterface?.SetState(null);
        }
        
        /// <summary>
        /// Toggle the stat allocation UI
        /// </summary>
        public void ToggleUI()
        {
            if (statInterface?.CurrentState != null)
                HideUI();
            else
                ShowUI();
        }
    }
}
