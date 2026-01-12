using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI;

namespace Rpg.Common.UI
{
    /// <summary>
    /// DEPRECATED: Skill Learning UI is now integrated into MasterUI
    /// This system is kept for reference only - keybind removed
    /// Use MasterUI's Skills tab instead (C key -> Skills tab)
    /// </summary>
    public class SkillLearningUISystem : ModSystem
    {
        // Keybind removed - use MasterUISystem.MasterUIKey instead
        
        private UserInterface skillInterface;
        private SkillLearningUI skillUI;
        
        public override void Load()
        {
            // Keybind registration removed - now uses MasterUI
            
            if (!Main.dedServ)
            {
                skillInterface = new UserInterface();
                skillUI = new SkillLearningUI();
                skillUI.Activate();
            }
        }
        
        public override void Unload()
        {
            skillInterface = null;
            skillUI = null;
        }
        
        public override void UpdateUI(GameTime gameTime)
        {
            if (skillInterface?.CurrentState != null)
            {
                skillInterface.Update(gameTime);
            }
            
            // Keybind check removed - now uses MasterUI
        }
        
        public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
        {
            int mouseTextIndex = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Mouse Text"));
            if (mouseTextIndex != -1)
            {
                layers.Insert(mouseTextIndex, new LegacyGameInterfaceLayer(
                    "Rpg: Skill Learning UI",
                    delegate
                    {
                        if (skillInterface?.CurrentState != null)
                        {
                            skillInterface.Draw(Main.spriteBatch, new GameTime());
                        }
                        return true;
                    },
                    InterfaceScaleType.UI
                ));
            }
        }
        
        public void ShowUI()
        {
            skillInterface?.SetState(skillUI);
        }
        
        public void HideUI()
        {
            skillInterface?.SetState(null);
        }
        
        public void ToggleUI()
        {
            if (skillInterface?.CurrentState != null)
                HideUI();
            else
                ShowUI();
        }
    }
    
    /// <summary>
    /// UI System for Skill Bar (always visible)
    /// </summary>
    public class SkillBarUISystem : ModSystem
    {
        private UserInterface skillBarInterface;
        private SkillBarUI skillBarUI;
        
        public override void Load()
        {
            if (!Main.dedServ)
            {
                skillBarInterface = new UserInterface();
                skillBarUI = new SkillBarUI();
                skillBarUI.Activate();
                
                // Automatically show skill bar
                skillBarInterface?.SetState(skillBarUI);
            }
        }
        
        public override void Unload()
        {
            skillBarInterface = null;
            skillBarUI = null;
        }
        
        public override void UpdateUI(GameTime gameTime)
        {
            if (skillBarInterface?.CurrentState != null)
            {
                skillBarInterface.Update(gameTime);
            }
        }
        
        public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
        {
            int mouseTextIndex = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Mouse Text"));
            if (mouseTextIndex != -1)
            {
                layers.Insert(mouseTextIndex, new LegacyGameInterfaceLayer(
                    "Rpg: Skill Bar",
                    delegate
                    {
                        if (skillBarInterface?.CurrentState != null)
                        {
                            skillBarInterface.Draw(Main.spriteBatch, new GameTime());
                        }
                        return true;
                    },
                    InterfaceScaleType.UI
                ));
            }
        }
    }
}
