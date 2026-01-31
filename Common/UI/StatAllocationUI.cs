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
using RpgMod.Common.Players;
using RpgMod.Common.Stats;

namespace RpgMod.Common.UI
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
        
        private Dictionary<StatType, (UIPanel plus, UIPanel minus)> statButtons = new Dictionary<StatType, (UIPanel plus, UIPanel minus)>();

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
                    if (kvp.Value.plus.IsMouseHovering)
                    {
                        HandleStatPlusClick(kvp.Key);
                        break;
                    }
                    if (kvp.Value.minus.IsMouseHovering)
                    {
                        HandleStatMinusClick(kvp.Key);
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
            var instructions = new UIText("Left Click: +1/-1  |  Shift: +5/-5  |  Ctrl: +10/-10  |  Ctrl+Shift: All/Max", 0.8f);
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
            var plusButtonPanel = new UIPanel();
            plusButtonPanel.Width.Set(25, 0f);
            plusButtonPanel.Height.Set(STAT_ROW_HEIGHT - 12, 0f);
            plusButtonPanel.Left.Set(-60, 1f);
            plusButtonPanel.Top.Set(4, 0f);
            plusButtonPanel.BackgroundColor = rpgPlayer.StatPoints > 0 
                ? new Color(60, 150, 60)
                : new Color(80, 80, 80);
            statCard.Append(plusButtonPanel);
            
            var plusButtonText = new UIText("+", 1f);
            plusButtonText.HAlign = 0.5f;
            plusButtonText.VAlign = 0.5f;
            plusButtonText.TextColor = Color.White;
            plusButtonPanel.Append(plusButtonText);
            
            // [-] Button
            var minusButtonPanel = new UIPanel();
            minusButtonPanel.Width.Set(25, 0f);
            minusButtonPanel.Height.Set(STAT_ROW_HEIGHT - 12, 0f);
            minusButtonPanel.Left.Set(-30, 1f);
            minusButtonPanel.Top.Set(4, 0f);
            minusButtonPanel.BackgroundColor = baseValue > 0 
                ? new Color(150, 60, 60)
                : new Color(80, 80, 80);
            statCard.Append(minusButtonPanel);
            
            var minusButtonText = new UIText("-", 1f);
            minusButtonText.HAlign = 0.5f;
            minusButtonText.VAlign = 0.5f;
            minusButtonText.TextColor = Color.White;
            minusButtonPanel.Append(minusButtonText);
            
            // Store buttons for click handling
            statButtons[stat] = (plusButtonPanel, minusButtonPanel);
            
            // Handle clicks on the button
            // Click handling moved to Update method
            
            statList.Add(statCard);
        }
        
        private void HandleStatPlusClick(StatType stat)
        {
            if (rpgPlayer == null || rpgPlayer.StatPoints <= 0)
                return;
                
            int amount = GetClickAmount();
            amount = System.Math.Min(amount, rpgPlayer.StatPoints);
            
            if (amount > 0)
            {
                rpgPlayer.AllocateStatPoint(stat, amount);
                SoundEngine.PlaySound(SoundID.MenuTick);
            }
        }
        
        private void HandleStatMinusClick(StatType stat)
        {
            if (rpgPlayer == null)
                return;
                
            int baseValue = rpgPlayer.GetBaseStatValue(stat);
            if (baseValue <= 0)
                return;
                
            int amount = GetClickAmount();
            amount = System.Math.Min(amount, baseValue);
            
            if (amount > 0)
            {
                rpgPlayer.DeallocateStatPoint(stat, amount);
                SoundEngine.PlaySound(SoundID.MenuTick);
            }
        }
        
        private int GetClickAmount()
        {
            bool shift = Main.keyState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.LeftShift) || 
                        Main.keyState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.RightShift);
            bool ctrl = Main.keyState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.LeftControl) || 
                       Main.keyState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.RightControl);
            
            if (ctrl && shift) return int.MaxValue; // Will be clamped by caller
            if (ctrl) return 10;
            if (shift) return 5;
            return 1;
        }
        
        
        private List<string> GetStatTooltip(StatType stat)
        {
            var lines = new List<string>();
            
            var def = StatDefinitions.GetDefinition(stat);
            lines.Add(def.Title);
            lines.AddRange(def.EffectLines);
            
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
            return StatDefinitions.GetDefinition(stat).Abbrev;
        }
        
        private Color GetStatColor(StatType stat)
        {
            return StatDefinitions.GetDefinition(stat).Color;
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
