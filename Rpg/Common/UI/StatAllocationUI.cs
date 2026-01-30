using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader;
using Terraria.UI;
using Terraria.ID;
using System.Collections.Generic;
using Rpg.Common.Players;

namespace Rpg.Common.UI
{
    /// <summary>
    /// UI for allocating stat points
    /// Supports: Left Click = 1, Shift+Left = 5, Ctrl+Left = 10, Ctrl+Shift+Left = All
    /// </summary>
    public class StatAllocationUI : UIState
    {
        private RpgPlayer rpgPlayer;
        private bool dragging;
        private Vector2 offset;
        
        private UIPanel mainPanel;
        private UIList statList;
        private UIScrollbar scrollbar;
        
        private Dictionary<StatType, UIPanel> statButtons = new Dictionary<StatType, UIPanel>();

        // UI scale helpers
        private Point MousePosition => GetScaledMouse();
        private Vector2 ScreenTopLeftUi => UiInput.ScreenTopLeftUi;
        private Vector2 ScreenBottomRightUi => UiInput.ScreenBottomRightUi;
        private int ScreenWidthUi => UiInput.ScreenWidthUi;
        private int ScreenHeightUi => UiInput.ScreenHeightUi;
        
        private const float PANEL_WIDTH = 420f;
        private const float PANEL_HEIGHT = 580f;
        private const float STAT_ROW_HEIGHT = 36f;
        private const float HEADER_HEIGHT = 60f;
        
        public override void OnInitialize()
        {
            // Main panel
            mainPanel = new UIPanel();
            mainPanel.Width.Set(PANEL_WIDTH, 0f);
            mainPanel.Height.Set(PANEL_HEIGHT, 0f);
            mainPanel.HAlign = 0.5f;
            mainPanel.VAlign = 0.5f;
            mainPanel.BackgroundColor = new Color(20, 25, 45, 240);
            mainPanel.BorderColor = new Color(89, 116, 213);
            Append(mainPanel);
            
            // Title
            var titleText = new UIText("Stat Allocation", 1.2f);
            titleText.HAlign = 0.5f;
            titleText.Top.Set(15, 0f);
            mainPanel.Append(titleText);
            
            // Stat list
            statList = new UIList();
            statList.Width.Set(-30, 1f);
            statList.Height.Set(-80, 1f);
            statList.Top.Set(60, 0f);
            statList.Left.Set(10, 0f);
            statList.ListPadding = 5f;
            mainPanel.Append(statList);
            
            // Scrollbar
            scrollbar = new UIScrollbar();
            scrollbar.Width.Set(20, 0f);
            scrollbar.Height.Set(-80, 1f);
            scrollbar.Top.Set(60, 0f);
            scrollbar.HAlign = 1f;
            scrollbar.Left.Set(-10, 0f);
            mainPanel.Append(scrollbar);
            
            statList.SetScrollbar(scrollbar);
        }
        
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            
            rpgPlayer = Main.LocalPlayer.GetModPlayer<RpgPlayer>();

            // Handle dragging
            Point mouse = MousePosition;
            Vector2 topLeft = ScreenTopLeftUi;
            Rectangle headerBounds = new Rectangle(
                (int)(topLeft.X + mainPanel.Left.Pixels + ScreenWidthUi * mainPanel.HAlign),
                (int)(topLeft.Y + mainPanel.Top.Pixels + ScreenHeightUi * mainPanel.VAlign),
                (int)mainPanel.Width.Pixels,
                (int)HEADER_HEIGHT
            );
            
            if (!dragging && Main.mouseLeft && Main.mouseLeftRelease && headerBounds.Contains(mouse))
            {
                dragging = true;
                offset = new Vector2(mouse.X - headerBounds.X, mouse.Y - headerBounds.Y);
            }
            else if (dragging && !Main.mouseLeft)
            {
                dragging = false;
            }

            if (dragging)
            {
                mainPanel.Left.Set(mouse.X - offset.X - topLeft.X - ScreenWidthUi * mainPanel.HAlign, 0f);
                mainPanel.Top.Set(mouse.Y - offset.Y - topLeft.Y - ScreenHeightUi * mainPanel.VAlign, 0f);
                mainPanel.Recalculate();
            }
            
            // Close with ESC
            if (Main.keyState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Escape))
            {
                var uiSystem = ModContent.GetInstance<StatUISystem>();
                uiSystem.HideUI();
            }
            
            // Refresh stats periodically
            if (Main.GameUpdateCount % 30 == 0) // Every half second
            {
                PopulateStatList();
            }
            
            // Handle stat button clicks
            if (Main.mouseLeftRelease)
            {
                foreach (var kvp in statButtons)
                {
                    if (kvp.Value.IsMouseHovering)
                    {
                        HandleStatClick(kvp.Key);
                        break;
                    }
                }
            }
        }
        
        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            // UIList handles drawing, just ensure it's populated
            if (statList.Count == 0)
            {
                PopulateStatList();
            }
        }
        
        private void PopulateStatList()
        {
            statList.Clear();
            statButtons.Clear();
            
            // Safety check
            if (rpgPlayer == null)
                return;
                
            // Stat points remaining
            string pointsText = rpgPlayer.PendingStatPoints > 0
                ? $"Points: {rpgPlayer.StatPoints} (Pending: {rpgPlayer.PendingStatPoints})"
                : $"Points: {rpgPlayer.StatPoints}";
            var pointsElement = new UIText(pointsText, 1f);
            pointsElement.TextColor = rpgPlayer.StatPoints > 0 ? Color.Gold : Color.Gray;
            statList.Add(pointsElement);
            
            // Instructions
            var instructions = new UIText("Left Click: +1  |  Shift: +5  |  Ctrl: +10  |  Ctrl+Shift: All", 0.8f);
            instructions.TextColor = Color.LightGray;
            statList.Add(instructions);
            
            // Add stat rows
            AddStatRow(StatType.Strength);
            AddStatRow(StatType.Dexterity);
            AddStatRow(StatType.Rogue);
            AddStatRow(StatType.Intelligence);
            AddStatRow(StatType.Focus);
            AddStatRow(StatType.Vitality);
            AddStatRow(StatType.Stamina);
            AddStatRow(StatType.Defense);
            AddStatRow(StatType.Agility);
            AddStatRow(StatType.Wisdom);
            AddStatRow(StatType.Fortitude);
            AddStatRow(StatType.Luck);
        }
        
        private void AddStatRow(StatType stat)
        {
            var statCard = new UIPanel();
            statCard.Width.Set(0, 1f);
            statCard.Height.Set(STAT_ROW_HEIGHT, 0f);
            statCard.BackgroundColor = new Color(44, 57, 105) * 0.8f;
            statCard.BorderColor = Color.Black;
            
            int baseValue = rpgPlayer.GetBaseStatValue(stat);
            int autoValue = rpgPlayer.GetAutoStatValue(stat);
            int bonusValue = rpgPlayer.GetBonusStatValue(stat);
            int totalValue = baseValue + autoValue + bonusValue;
            string statName = GetStatName(stat);
            Color statColor = GetStatColor(stat);
            
            // Stat name
            var nameText = new UIText(statName, 1f);
            nameText.Top.Set(8, 0f);
            nameText.Left.Set(15, 0f);
            nameText.TextColor = statColor;
            statCard.Append(nameText);
            
            // Stat value
            string valueText = totalValue.ToString();
            var valueElement = new UIText(valueText, 1f);
            valueElement.Top.Set(8, 0f);
            valueElement.Left.Set(180, 0f);
            valueElement.TextColor = Color.White;
            statCard.Append(valueElement);

            string detail = BuildStatDetail(baseValue, autoValue, bonusValue);
            var detailElement = new UIText(detail, 0.75f);
            detailElement.Top.Set(10, 0f);
            detailElement.Left.Set(230, 0f);
            detailElement.TextColor = Color.Gray;
            statCard.Append(detailElement);
            
            // [+] Button
            var buttonPanel = new UIPanel();
            buttonPanel.Width.Set(50, 0f);
            buttonPanel.Height.Set(STAT_ROW_HEIGHT - 12, 0f);
            buttonPanel.Left.Set(-60, 1f);
            buttonPanel.Top.Set(4, 0f);
            buttonPanel.BackgroundColor = rpgPlayer.StatPoints > 0 
                ? new Color(60, 150, 60)
                : new Color(80, 80, 80);
            statCard.Append(buttonPanel);
            
            var buttonText = new UIText("+", 1f);
            buttonText.HAlign = 0.5f;
            buttonText.VAlign = 0.5f;
            buttonText.TextColor = Color.White;
            buttonPanel.Append(buttonText);
            
            // Store button for click handling
            statButtons[stat] = buttonPanel;
            
            // Handle clicks on the button
            // Click handling moved to Update method
            
            statList.Add(statCard);
        }
        
        private void HandleStatClick(StatType stat)
        {
            if (rpgPlayer == null || rpgPlayer.StatPoints <= 0)
                return;
                
            int amount = 1;
            if (Main.keyState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.LeftShift) || 
                Main.keyState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.RightShift))
            {
                amount = 5;
            }
            if (Main.keyState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.LeftControl) || 
                Main.keyState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.RightControl))
            {
                if (Main.keyState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.LeftShift) || 
                    Main.keyState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.RightShift))
                {
                    amount = rpgPlayer.StatPoints; // All remaining
                }
                else
                {
                    amount = 10;
                }
            }
            
            amount = System.Math.Min(amount, rpgPlayer.StatPoints);
            
            if (amount > 0)
            {
                rpgPlayer.AllocateStatPoint(stat, amount);
                SoundEngine.PlaySound(SoundID.MenuTick);
            }
        }
        
        private int GetClickAmount()
        {
            bool shift = Main.keyState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.LeftShift) || 
                        Main.keyState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.RightShift);
            bool ctrl = Main.keyState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.LeftControl) || 
                       Main.keyState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.RightControl);
            
            if (ctrl && shift) return rpgPlayer.StatPoints; // All
            if (ctrl) return 10;
            if (shift) return 5;
            return 1;
        }
        
        
        private List<string> GetStatTooltip(StatType stat)
        {
            var lines = new List<string>();
            
            switch (stat)
            {
                case StatType.Strength:
                    lines.Add("Strength - Physical Power");
                    lines.Add("+1% Melee Damage per point");
                    break;
                case StatType.Dexterity:
                    lines.Add("Dexterity - Precision");
                    lines.Add("+1% Ranged Damage per point");
                    lines.Add("+0.3% Attack Speed per point");
                    lines.Add("+0.3% Ranged Crit per point");
                    break;
                case StatType.Rogue:
                    lines.Add("Rogue - Finesse");
                    lines.Add("+0.8% Melee/Ranged Damage per point");
                    lines.Add("+0.3% Critical Chance per point");
                    break;
                case StatType.Intelligence:
                    lines.Add("Intelligence - Arcane Power");
                    lines.Add("+1.5% Magic Damage per point");
                    lines.Add("+0.7% Magic Critical per point");
                    lines.Add("+0.5% Spell Power per point");
                    lines.Add("+0.2% Mana Cost Reduction per point");
                    break;
                case StatType.Focus:
                    lines.Add("Focus - Summoning");
                    lines.Add("+1.2% Summon Damage per point");
                    lines.Add("+1 Minion Slot at 10/30/60/100 FOC");
                    break;
                case StatType.Vitality:
                    lines.Add("Vitality - Life Force");
                    lines.Add("+10 Max HP per point");
                    lines.Add("+0.02 HP Regen per point");
                    break;
                case StatType.Stamina:
                    lines.Add("Stamina - Endurance");
                    lines.Add("+2 Max Stamina per point");
                    lines.Add("+0.05 Stamina Regen per point");
                    break;
                case StatType.Defense:
                    lines.Add("Defense - Protection");
                    lines.Add("+0.3% Damage Reduction per point");
                    lines.Add("+1 Armor per 5 points");
                    break;
                case StatType.Agility:
                    lines.Add("Agility - Mobility");
                    lines.Add("+0.3% Move Speed per point");
                    lines.Add("+0.2% Dodge Chance per point");
                    break;
                case StatType.Wisdom:
                    lines.Add("Wisdom - Mystic Knowledge");
                    lines.Add("+5 Max Mana per point");
                    lines.Add("+0.03 Mana Regen per point");
                    break;
                case StatType.Fortitude:
                    lines.Add("Fortitude - Resilience");
                    lines.Add("+0.5% Debuff Duration Reduction per point");
                    lines.Add("+0.3% Knockback Resist per point");
                    lines.Add("+0.2% Damage Reduction per point");
                    break;
                case StatType.Luck:
                    lines.Add("Luck - Fortune");
                    lines.Add("+0.5% Critical Chance per point");
                    lines.Add("+0.2% Luck (drops/variance) per point");
                    lines.Add("+0.2% All Damage per point");
                    break;
            }
            
            return lines;
        }

        private string BuildStatDetail(int baseValue, int autoValue, int bonusValue)
        {
            if (autoValue == 0 && bonusValue == 0)
                return $"(B{baseValue})";

            return $"(B{baseValue} A{autoValue} +{bonusValue})";
        }
        
        private string GetStatName(StatType stat)
        {
            return stat switch
            {
                StatType.Strength => "STR",
                StatType.Dexterity => "DEX",
                StatType.Rogue => "ROG",
                StatType.Intelligence => "INT",
                StatType.Focus => "FOC",
                StatType.Vitality => "VIT",
                StatType.Stamina => "STA",
                StatType.Defense => "DEF",
                StatType.Agility => "AGI",
                StatType.Wisdom => "WIS",
                StatType.Fortitude => "FOR",
                StatType.Luck => "LUK",
                _ => "???"
            };
        }
        
        private Color GetStatColor(StatType stat)
        {
            return stat switch
            {
                StatType.Strength => new Color(255, 100, 100),
                StatType.Dexterity => new Color(100, 255, 100),
                StatType.Rogue => new Color(180, 100, 255),
                StatType.Intelligence => new Color(100, 150, 255),
                StatType.Focus => new Color(150, 200, 255),
                StatType.Vitality => new Color(255, 80, 80),
                StatType.Stamina => new Color(255, 200, 80),
                StatType.Defense => new Color(200, 200, 200),
                StatType.Agility => new Color(100, 255, 200),
                StatType.Wisdom => new Color(200, 150, 255),
                StatType.Fortitude => new Color(160, 120, 80),
                StatType.Luck => new Color(255, 215, 0),
                _ => Color.White
            };
        }
        
        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);
            
            // Draw tooltips for stat cards
            if (rpgPlayer != null)
            {
                foreach (var element in statList._items)
                {
                    if (element is UIPanel panel && panel.IsMouseHovering)
                    {
                        // Find which stat this panel represents
                        int index = statList._items.IndexOf(panel);
                        if (index >= 2) // Skip title and instructions
                        {
                            StatType stat = (StatType)(index - 2);
                            DrawStatTooltip(spriteBatch, stat);
                        }
                    }
                }
            }
        }
        
        private void DrawStatTooltip(SpriteBatch spriteBatch, StatType stat)
        {
            Point mouse = MousePosition;
            List<string> tooltipLines = GetStatTooltip(stat);
            
            int tooltipWidth = 300;
            int tooltipHeight = tooltipLines.Count * 20 + 10;
            
            Rectangle tooltipBounds = new Rectangle(
                mouse.X + 20,
                mouse.Y - tooltipHeight / 2,
                tooltipWidth,
                tooltipHeight
            );
            
            // Keep on screen
            Vector2 topLeft = ScreenTopLeftUi;
            int screenRight = (int)(topLeft.X + ScreenWidthUi);
            int screenBottom = (int)(topLeft.Y + ScreenHeightUi);

            if (tooltipBounds.Right > screenRight)
                tooltipBounds.X = mouse.X - tooltipWidth - 20;
            if (tooltipBounds.Bottom > screenBottom)
                tooltipBounds.Y = screenBottom - tooltipHeight;
            if (tooltipBounds.Y < topLeft.Y)
                tooltipBounds.Y = (int)topLeft.Y;
            
            DrawPanel(spriteBatch, tooltipBounds, new Color(10, 15, 30, 250));
            
            int yPos = tooltipBounds.Y + 5;
            foreach (string line in tooltipLines)
            {
                DrawText(spriteBatch, line, new Vector2(tooltipBounds.X + 5, yPos), Color.White, 0.8f);
                yPos += 20;
            }
        }

        private Point GetScaledMouse()
        {
            return UiInput.GetUiMousePoint();
        }
        
        private void DrawText(SpriteBatch spriteBatch, string text, Vector2 position, Color color, float scale)
        {
            Utils.DrawBorderStringFourWay(spriteBatch, FontAssets.MouseText.Value, text, position.X, position.Y, color, Color.Black, Vector2.Zero, scale);
        }
        
        private void DrawPanel(SpriteBatch spriteBatch, Rectangle bounds, Color color)
        {
            // Border
            Texture2D pixel = TextureAssets.MagicPixel.Value;
            spriteBatch.Draw(pixel, new Rectangle(bounds.X - 2, bounds.Y - 2, bounds.Width + 4, 2), Color.Black);
            spriteBatch.Draw(pixel, new Rectangle(bounds.X - 2, bounds.Bottom, bounds.Width + 4, 2), Color.Black);
            spriteBatch.Draw(pixel, new Rectangle(bounds.X - 2, bounds.Y, 2, bounds.Height), Color.Black);
            spriteBatch.Draw(pixel, new Rectangle(bounds.Right, bounds.Y, 2, bounds.Height), Color.Black);
            
            // Background
            spriteBatch.Draw(pixel, bounds, color);
        }
        
        public override void OnDeactivate()
        {
            base.OnDeactivate();
        }
    }
}
