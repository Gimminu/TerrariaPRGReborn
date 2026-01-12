using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;
using Terraria.UI;
using Rpg.Common.Base;

namespace Rpg.Common.UI
{
    /// <summary>
    /// Centralized tooltip rendering system for RPG mod
    /// </summary>
    public static class RpgTooltipRenderer
    {
        private const int PADDING = 8;
        private const int LINE_HEIGHT = 22;
        private const int HEADER_HEIGHT = 28;
        
        /// <summary>
        /// Render a skill tooltip at the given position
        /// </summary>
        public static void DrawSkillTooltip(
            SpriteBatch spriteBatch,
            string name,
            string description,
            int currentRank,
            int maxRank,
            float cooldown,
            int resourceCost,
            ResourceType resourceType,
            SkillType skillType,
            int requiredLevel,
            JobType requiredJob,
            Vector2 position)
        {
            var font = FontAssets.MouseText.Value;
            var smallFont = FontAssets.MouseText.Value;
            
            // Calculate tooltip size
            int width = 280;
            int lineCount = 5; // Name, desc, rank, cooldown, requirements
            if (skillType == SkillType.Passive) lineCount--;
            if (description.Length > 40) lineCount++;
            
            int height = PADDING * 2 + HEADER_HEIGHT + LINE_HEIGHT * lineCount;
            
            // Clamp to screen bounds
            if (position.X + width > Main.screenWidth)
                position.X = Main.screenWidth - width - 10;
            if (position.Y + height > Main.screenHeight)
                position.Y = Main.screenHeight - height - 10;
            
            // Draw background
            Rectangle bgRect = new Rectangle(
                (int)position.X,
                (int)position.Y,
                width,
                height
            );
            
            DrawTooltipBackground(spriteBatch, bgRect);
            
            // Draw content
            float y = position.Y + PADDING;
            float x = position.X + PADDING;
            
            // Name (header)
            Color nameColor = GetSkillTypeColor(skillType);
            Utils.DrawBorderStringFourWay(
                spriteBatch, font, name,
                x, y, nameColor, Color.Black,
                Vector2.Zero, 1.0f
            );
            y += HEADER_HEIGHT;
            
            // Type indicator
            string typeStr = skillType == SkillType.Passive ? "[Passive]" : "[Active]";
            Utils.DrawBorderStringFourWay(
                spriteBatch, smallFont, typeStr,
                x, y, Color.Gray, Color.Black,
                Vector2.Zero, 0.85f
            );
            y += LINE_HEIGHT;
            
            // Description
            string[] descLines = WrapText(description, width - PADDING * 2, smallFont);
            foreach (string line in descLines)
            {
                Utils.DrawBorderStringFourWay(
                    spriteBatch, smallFont, line,
                    x, y, Color.White, Color.Black,
                    Vector2.Zero, 0.85f
                );
                y += LINE_HEIGHT - 4;
            }
            y += 4;
            
            // Rank
            string rankStr = $"Rank: {currentRank}/{maxRank}";
            Color rankColor = currentRank >= maxRank ? Color.Gold : Color.LightGreen;
            Utils.DrawBorderStringFourWay(
                spriteBatch, smallFont, rankStr,
                x, y, rankColor, Color.Black,
                Vector2.Zero, 0.85f
            );
            y += LINE_HEIGHT;
            
            // Cooldown & Cost (only for active skills)
            if (skillType != SkillType.Passive)
            {
                string cdStr = cooldown > 0 ? $"Cooldown: {cooldown:0.#}s" : "No Cooldown";
                Utils.DrawBorderStringFourWay(
                    spriteBatch, smallFont, cdStr,
                    x, y, Color.Cyan, Color.Black,
                    Vector2.Zero, 0.85f
                );
                
                // Resource cost on same line, right side
                if (resourceCost > 0)
                {
                    string costStr = $"{resourceCost} {GetResourceName(resourceType)}";
                    Color costColor = GetResourceColor(resourceType);
                    float costX = position.X + width - PADDING - smallFont.MeasureString(costStr).X * 0.85f;
                    Utils.DrawBorderStringFourWay(
                        spriteBatch, smallFont, costStr,
                        costX, y, costColor, Color.Black,
                        Vector2.Zero, 0.85f
                    );
                }
                y += LINE_HEIGHT;
            }
            
            // Requirements
            string reqStr = $"Requires: Lv.{requiredLevel} {requiredJob}";
            Utils.DrawBorderStringFourWay(
                spriteBatch, smallFont, reqStr,
                x, y, Color.Orange, Color.Black,
                Vector2.Zero, 0.85f
            );
        }
        
        /// <summary>
        /// Render a stat tooltip
        /// </summary>
        public static void DrawStatTooltip(
            SpriteBatch spriteBatch,
            string statName,
            int baseValue,
            int bonusValue,
            string description,
            Vector2 position)
        {
            var font = FontAssets.MouseText.Value;
            
            int width = 250;
            int height = PADDING * 2 + HEADER_HEIGHT + LINE_HEIGHT * 3;
            
            // Clamp to screen
            if (position.X + width > Main.screenWidth)
                position.X = Main.screenWidth - width - 10;
            if (position.Y + height > Main.screenHeight)
                position.Y = Main.screenHeight - height - 10;
            
            Rectangle bgRect = new Rectangle(
                (int)position.X,
                (int)position.Y,
                width,
                height
            );
            
            DrawTooltipBackground(spriteBatch, bgRect);
            
            float y = position.Y + PADDING;
            float x = position.X + PADDING;
            
            // Stat name
            Utils.DrawBorderStringFourWay(
                spriteBatch, font, statName,
                x, y, Color.White, Color.Black,
                Vector2.Zero, 1.0f
            );
            y += HEADER_HEIGHT;
            
            // Value
            int total = baseValue + bonusValue;
            string valueStr = bonusValue > 0 
                ? $"Value: {baseValue} (+{bonusValue}) = {total}" 
                : $"Value: {baseValue}";
            Color valueColor = bonusValue > 0 ? Color.LightGreen : Color.White;
            Utils.DrawBorderStringFourWay(
                spriteBatch, font, valueStr,
                x, y, valueColor, Color.Black,
                Vector2.Zero, 0.85f
            );
            y += LINE_HEIGHT;
            
            // Description
            Utils.DrawBorderStringFourWay(
                spriteBatch, font, description,
                x, y, Color.Gray, Color.Black,
                Vector2.Zero, 0.8f
            );
        }
        
        /// <summary>
        /// Render a simple text tooltip
        /// </summary>
        public static void DrawSimpleTooltip(
            SpriteBatch spriteBatch,
            string text,
            Vector2 position,
            Color? textColor = null)
        {
            var font = FontAssets.MouseText.Value;
            Color color = textColor ?? Color.White;
            
            Vector2 textSize = font.MeasureString(text) * 0.9f;
            int width = (int)textSize.X + PADDING * 2;
            int height = (int)textSize.Y + PADDING * 2;
            
            // Clamp to screen
            if (position.X + width > Main.screenWidth)
                position.X = Main.screenWidth - width - 10;
            if (position.Y + height > Main.screenHeight)
                position.Y = Main.screenHeight - height - 10;
            
            Rectangle bgRect = new Rectangle(
                (int)position.X,
                (int)position.Y,
                width,
                height
            );
            
            DrawTooltipBackground(spriteBatch, bgRect);
            
            Utils.DrawBorderStringFourWay(
                spriteBatch, font, text,
                position.X + PADDING, position.Y + PADDING,
                color, Color.Black,
                Vector2.Zero, 0.9f
            );
        }
        
        #region Helper Methods
        
        private static void DrawTooltipBackground(SpriteBatch spriteBatch, Rectangle rect)
        {
            // Main background
            spriteBatch.Draw(
                TextureAssets.MagicPixel.Value,
                rect,
                new Color(20, 25, 45) * 0.95f
            );
            
            // Border
            DrawBorder(spriteBatch, rect, new Color(60, 80, 120) * 0.9f);
            
            // Inner highlight (top-left)
            spriteBatch.Draw(
                TextureAssets.MagicPixel.Value,
                new Rectangle(rect.X + 1, rect.Y + 1, rect.Width - 2, 1),
                new Color(100, 120, 160) * 0.3f
            );
            spriteBatch.Draw(
                TextureAssets.MagicPixel.Value,
                new Rectangle(rect.X + 1, rect.Y + 1, 1, rect.Height - 2),
                new Color(100, 120, 160) * 0.3f
            );
        }
        
        private static void DrawBorder(SpriteBatch spriteBatch, Rectangle rect, Color color)
        {
            // Top
            spriteBatch.Draw(TextureAssets.MagicPixel.Value,
                new Rectangle(rect.X, rect.Y, rect.Width, 1), color);
            // Bottom
            spriteBatch.Draw(TextureAssets.MagicPixel.Value,
                new Rectangle(rect.X, rect.Y + rect.Height - 1, rect.Width, 1), color);
            // Left
            spriteBatch.Draw(TextureAssets.MagicPixel.Value,
                new Rectangle(rect.X, rect.Y, 1, rect.Height), color);
            // Right
            spriteBatch.Draw(TextureAssets.MagicPixel.Value,
                new Rectangle(rect.X + rect.Width - 1, rect.Y, 1, rect.Height), color);
        }
        
        private static Color GetSkillTypeColor(SkillType type)
        {
            return type switch
            {
                SkillType.Passive => new Color(100, 200, 100),
                SkillType.Active => new Color(100, 150, 255),
                SkillType.Buff => new Color(255, 200, 100),
                SkillType.Debuff => new Color(200, 100, 200),
                SkillType.Ultimate => new Color(255, 100, 100),
                _ => Color.White
            };
        }
        
        private static Color GetResourceColor(ResourceType type)
        {
            return type switch
            {
                ResourceType.Mana => new Color(100, 150, 255),
                ResourceType.Stamina => new Color(255, 200, 100),
                ResourceType.Life => new Color(255, 100, 100),
                ResourceType.Rage => new Color(255, 80, 80),
                ResourceType.Energy => new Color(255, 255, 100),
                _ => Color.White
            };
        }
        
        private static string GetResourceName(ResourceType type)
        {
            return type switch
            {
                ResourceType.Mana => "MP",
                ResourceType.Stamina => "SP",
                ResourceType.Life => "HP",
                ResourceType.Rage => "Rage",
                ResourceType.Energy => "Energy",
                _ => ""
            };
        }
        
        private static string[] WrapText(string text, float maxWidth, ReLogic.Graphics.DynamicSpriteFont font)
        {
            if (string.IsNullOrEmpty(text))
                return new[] { "" };
            
            float scale = 0.85f;
            var words = text.Split(' ');
            var lines = new System.Collections.Generic.List<string>();
            string currentLine = "";
            
            foreach (string word in words)
            {
                string testLine = string.IsNullOrEmpty(currentLine) ? word : currentLine + " " + word;
                float width = font.MeasureString(testLine).X * scale;
                
                if (width > maxWidth && !string.IsNullOrEmpty(currentLine))
                {
                    lines.Add(currentLine);
                    currentLine = word;
                }
                else
                {
                    currentLine = testLine;
                }
            }
            
            if (!string.IsNullOrEmpty(currentLine))
                lines.Add(currentLine);
            
            return lines.ToArray();
        }
        
        #endregion
    }
}
