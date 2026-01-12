using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
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
        private Rectangle bounds;
        
        private const int PANEL_WIDTH = 420;
        private const int PANEL_HEIGHT = 580;
        private const int STAT_ROW_HEIGHT = 36;
        private const int HEADER_HEIGHT = 60;
        
        public override void OnInitialize()
        {
            bounds = new Rectangle(
                Main.screenWidth / 2 - PANEL_WIDTH / 2,
                Main.screenHeight / 2 - PANEL_HEIGHT / 2,
                PANEL_WIDTH,
                PANEL_HEIGHT
            );
        }
        
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            
            rpgPlayer = Main.LocalPlayer.GetModPlayer<RpgPlayer>();

            Rectangle headerBounds = new Rectangle(bounds.X, bounds.Y, bounds.Width, HEADER_HEIGHT);
            if (!dragging && Main.mouseLeft && Main.mouseLeftRelease && headerBounds.Contains(Main.mouseX, Main.mouseY))
            {
                dragging = true;
                offset = new Vector2(Main.mouseX - bounds.X, Main.mouseY - bounds.Y);
            }
            else if (dragging && !Main.mouseLeft)
            {
                dragging = false;
            }

            // Handle dragging
            if (dragging)
            {
                bounds.X = (int)(Main.mouseX - offset.X);
                bounds.Y = (int)(Main.mouseY - offset.Y);
            }
            
            // Close with ESC
            if (Main.keyState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Escape))
            {
                var uiSystem = ModContent.GetInstance<StatUISystem>();
                uiSystem.HideUI();
            }
        }
        
        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            // Safety check
            if (rpgPlayer == null)
                return;
                
            // Draw panel background
            DrawPanel(spriteBatch, bounds, new Color(20, 25, 45, 240));
            
            // Draw header
            Rectangle headerBounds = new Rectangle(bounds.X, bounds.Y, bounds.Width, HEADER_HEIGHT);
            DrawPanel(spriteBatch, headerBounds, new Color(30, 35, 60, 255));
            
            // Title
            string title = "Stat Allocation";
            Vector2 titleSize = FontAssets.MouseText.Value.MeasureString(title);
            Vector2 titlePos = new Vector2(
                bounds.X + bounds.Width / 2 - titleSize.X / 2,
                bounds.Y + 12
            );
            DrawText(spriteBatch, title, titlePos, Color.White, 1f);
            
            // Stat points remaining
            string pointsText = rpgPlayer.PendingStatPoints > 0
                ? $"Points: {rpgPlayer.StatPoints} (Pending: {rpgPlayer.PendingStatPoints})"
                : $"Points: {rpgPlayer.StatPoints}";
            Vector2 pointsSize = FontAssets.MouseText.Value.MeasureString(pointsText);
            Vector2 pointsPos = new Vector2(
                bounds.X + bounds.Width / 2 - pointsSize.X / 2,
                bounds.Y + 35
            );
            DrawText(spriteBatch, pointsText, pointsPos, Color.Gold, 1f);
            
            // Draw stats
            int yOffset = HEADER_HEIGHT + 10;
            DrawStatRow(spriteBatch, StatType.Strength, ref yOffset);
            DrawStatRow(spriteBatch, StatType.Dexterity, ref yOffset);
            DrawStatRow(spriteBatch, StatType.Rogue, ref yOffset);
            DrawStatRow(spriteBatch, StatType.Intelligence, ref yOffset);
            DrawStatRow(spriteBatch, StatType.Focus, ref yOffset);
            DrawStatRow(spriteBatch, StatType.Vitality, ref yOffset);
            DrawStatRow(spriteBatch, StatType.Stamina, ref yOffset);
            DrawStatRow(spriteBatch, StatType.Defense, ref yOffset);
            DrawStatRow(spriteBatch, StatType.Agility, ref yOffset);
            DrawStatRow(spriteBatch, StatType.Wisdom, ref yOffset);
            DrawStatRow(spriteBatch, StatType.Fortitude, ref yOffset);
            DrawStatRow(spriteBatch, StatType.Luck, ref yOffset);
            
            // Instructions
            yOffset += 10;
            string[] instructions = {
                "Left Click: +1",
                "Shift + Click: +5",
                "Ctrl + Click: +10",
                "Ctrl + Shift + Click: All"
            };
            
            foreach (string instruction in instructions)
            {
                Vector2 instrSize = FontAssets.MouseText.Value.MeasureString(instruction);
                Vector2 instrPos = new Vector2(
                    bounds.X + bounds.Width / 2 - instrSize.X / 2,
                    bounds.Y + yOffset
                );
                DrawText(spriteBatch, instruction, instrPos, Color.LightGray, 0.8f);
                yOffset += 18;
            }
        }
        
        private void DrawStatRow(SpriteBatch spriteBatch, StatType stat, ref int yOffset)
        {
            int baseValue = rpgPlayer.GetBaseStatValue(stat);
            int autoValue = rpgPlayer.GetAutoStatValue(stat);
            int bonusValue = rpgPlayer.GetBonusStatValue(stat);
            int totalValue = baseValue + autoValue + bonusValue;
            string statName = GetStatName(stat);
            Color statColor = GetStatColor(stat);
            
            Rectangle rowBounds = new Rectangle(
                bounds.X + 10,
                bounds.Y + yOffset,
                bounds.Width - 20,
                STAT_ROW_HEIGHT - 4
            );
            
            // Hover effect
            bool hovered = rowBounds.Contains(Main.mouseX, Main.mouseY);
            if (hovered)
            {
                DrawPanel(spriteBatch, rowBounds, new Color(40, 50, 80, 200));
            }
            
            // Stat name
            Vector2 namePos = new Vector2(rowBounds.X + 10, rowBounds.Y + 8);
            DrawText(spriteBatch, statName, namePos, statColor, 1f);
            
            // Stat value
            string valueText = totalValue.ToString();
            Vector2 valuePos = new Vector2(rowBounds.X + 180, rowBounds.Y + 8);
            DrawText(spriteBatch, valueText, valuePos, Color.White, 1f);

            string detail = BuildStatDetail(baseValue, autoValue, bonusValue);
            Vector2 detailPos = new Vector2(rowBounds.X + 230, rowBounds.Y + 10);
            DrawText(spriteBatch, detail, detailPos, Color.Gray, 0.75f);
            
            // [+] Button
            Rectangle buttonBounds = new Rectangle(
                rowBounds.X + rowBounds.Width - 60,
                rowBounds.Y + 4,
                50,
                STAT_ROW_HEIGHT - 12
            );
            
            bool buttonHovered = buttonBounds.Contains(Main.mouseX, Main.mouseY);
            Color buttonColor = rpgPlayer.StatPoints > 0 
                ? (buttonHovered ? new Color(80, 200, 80) : new Color(60, 150, 60))
                : new Color(80, 80, 80);
                
            DrawPanel(spriteBatch, buttonBounds, buttonColor);
            
            string buttonText = "+";
            Vector2 buttonTextSize = FontAssets.MouseText.Value.MeasureString(buttonText);
            Vector2 buttonTextPos = new Vector2(
                buttonBounds.X + buttonBounds.Width / 2 - buttonTextSize.X / 2,
                buttonBounds.Y + buttonBounds.Height / 2 - buttonTextSize.Y / 2
            );
            DrawText(spriteBatch, buttonText, buttonTextPos, Color.White, 1f);
            
            // Handle clicks
            if (buttonHovered && Main.mouseLeft && Main.mouseLeftRelease && rpgPlayer.StatPoints > 0)
            {
                int amount = GetClickAmount();
                rpgPlayer.AllocateStatPoint(stat, amount);
                SoundEngine.PlaySound(SoundID.MenuTick);
            }
            
            // Tooltip
            if (hovered)
            {
                DrawStatTooltip(spriteBatch, stat);
            }
            
            yOffset += STAT_ROW_HEIGHT;
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
        
        private void DrawStatTooltip(SpriteBatch spriteBatch, StatType stat)
        {
            List<string> tooltipLines = GetStatTooltip(stat);
            
            int tooltipWidth = 300;
            int tooltipHeight = tooltipLines.Count * 20 + 10;
            
            Rectangle tooltipBounds = new Rectangle(
                Main.mouseX + 20,
                Main.mouseY - tooltipHeight / 2,
                tooltipWidth,
                tooltipHeight
            );
            
            // Keep on screen
            if (tooltipBounds.Right > Main.screenWidth)
                tooltipBounds.X = Main.mouseX - tooltipWidth - 20;
            if (tooltipBounds.Bottom > Main.screenHeight)
                tooltipBounds.Y = Main.screenHeight - tooltipHeight;
            if (tooltipBounds.Y < 0)
                tooltipBounds.Y = 0;
            
            DrawPanel(spriteBatch, tooltipBounds, new Color(10, 15, 30, 250));
            
            int yPos = tooltipBounds.Y + 5;
            foreach (string line in tooltipLines)
            {
                DrawText(spriteBatch, line, new Vector2(tooltipBounds.X + 5, yPos), Color.White, 0.8f);
                yPos += 20;
            }
        }

        private void DrawText(SpriteBatch spriteBatch, string text, Vector2 position, Color color, float scale)
        {
            Utils.DrawBorderStringFourWay(spriteBatch, FontAssets.MouseText.Value, text, position.X, position.Y, color, Color.Black, Vector2.Zero, scale);
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
                    lines.Add("+5 Max HP per point");
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
