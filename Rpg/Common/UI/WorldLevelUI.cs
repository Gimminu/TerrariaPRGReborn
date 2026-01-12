using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;
using Terraria.UI;
using Rpg.Common.Systems;
using Rpg.Common.Players;

namespace Rpg.Common.UI
{
    /// <summary>
    /// Displays World Level in the corner of the screen
    /// Per design doc: "월드 레벨은 플레이어에게 HUD 등을 통해 명확히 표시"
    /// </summary>
    public class WorldLevelUI : UIState
    {
        private const float PADDING = 8f;
        private const float BOX_WIDTH = 240f;
        private const float BOX_HEIGHT = 72f;
        
        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);
            
            if (Main.gameMenu || Main.LocalPlayer == null || !Main.LocalPlayer.active)
                return;
            
            DrawWorldLevelBox(spriteBatch);
        }
        
        private void DrawWorldLevelBox(SpriteBatch spriteBatch)
        {
            // Position at top-center
            float x = (Main.screenWidth - BOX_WIDTH) / 2f;
            float y = PADDING;
            
            Rectangle boxRect = new Rectangle((int)x, (int)y, (int)BOX_WIDTH, (int)BOX_HEIGHT);
            
            // Draw background
            DrawBox(spriteBatch, boxRect, new Color(22, 26, 38, 220));
            
            // Get world level and calculate color
            int worldLevel = RpgWorld.GetWorldLevel();
            Color levelColor = GetWorldLevelColor(worldLevel);

            // Accent bar
            Rectangle accent = new Rectangle(boxRect.X, boxRect.Y, boxRect.Width, 3);
            spriteBatch.Draw(TextureAssets.MagicPixel.Value, accent, levelColor * 0.9f);

            // Text layout
            string labelText = "WORLD LEVEL";
            string valueText = $"WL {worldLevel}";
            string tagText = GetWorldLevelTag(worldLevel);

            float labelScale = 0.8f;
            float valueScale = 1.1f;
            float tagScale = 0.75f;

            Vector2 labelSize = FontAssets.MouseText.Value.MeasureString(labelText) * labelScale;
            Vector2 valueSize = FontAssets.MouseText.Value.MeasureString(valueText) * valueScale;
            Vector2 tagSize = FontAssets.MouseText.Value.MeasureString(tagText) * tagScale;

            float topPadding = 6f;
            float bottomPadding = 6f;
            float gap = 4f;

            float labelY = y + topPadding;
            float valueY = labelY + labelSize.Y + gap;
            float tagY = y + BOX_HEIGHT - tagSize.Y - bottomPadding;
            if (tagY < valueY + valueSize.Y + gap)
                tagY = valueY + valueSize.Y + gap;

            Vector2 labelPos = new Vector2(x + BOX_WIDTH / 2f - labelSize.X / 2f, labelY);
            Vector2 valuePos = new Vector2(x + BOX_WIDTH / 2f - valueSize.X / 2f, valueY);
            Vector2 tagPos = new Vector2(x + BOX_WIDTH / 2f - tagSize.X / 2f, tagY);

            Utils.DrawBorderString(spriteBatch, labelText, labelPos, Color.LightGray, labelScale);
            Utils.DrawBorderString(spriteBatch, tagText, tagPos, levelColor, tagScale);
            Utils.DrawBorderString(spriteBatch, valueText, valuePos, levelColor, valueScale);
        }
        
        private void DrawBox(SpriteBatch spriteBatch, Rectangle rect, Color color)
        {
            Texture2D pixel = TextureAssets.MagicPixel.Value;
            
            // Main background
            spriteBatch.Draw(pixel, rect, color);
            
            // Border
            int borderWidth = 2;
            Color borderColor = new Color(90, 110, 140, 220);
            
            // Top
            spriteBatch.Draw(pixel, new Rectangle(rect.X, rect.Y, rect.Width, borderWidth), borderColor);
            // Bottom
            spriteBatch.Draw(pixel, new Rectangle(rect.X, rect.Bottom - borderWidth, rect.Width, borderWidth), borderColor);
            // Left
            spriteBatch.Draw(pixel, new Rectangle(rect.X, rect.Y, borderWidth, rect.Height), borderColor);
            // Right
            spriteBatch.Draw(pixel, new Rectangle(rect.Right - borderWidth, rect.Y, borderWidth, rect.Height), borderColor);
        }
        
        private Color GetWorldLevelColor(int worldLevel)
        {
            if (worldLevel <= 3)
                return Color.LightGreen;    // Easy
            if (worldLevel <= 6)
                return Color.Yellow;        // Normal
            if (worldLevel <= 10)
                return Color.Orange;        // Challenging
            if (worldLevel <= 15)
                return Color.OrangeRed;     // Hard
            
            return Color.Red;               // Extreme
        }

        private string GetWorldLevelTag(int worldLevel)
        {
            if (worldLevel <= 3)
                return "Easy";
            if (worldLevel <= 6)
                return "Normal";
            if (worldLevel <= 10)
                return "Challenging";
            if (worldLevel <= 15)
                return "Hard";

            return "Extreme";
        }
    }
    
    /// <summary>
    /// System to manage World Level UI
    /// </summary>
    public class WorldLevelUISystem : ModSystem
    {
        internal UserInterface worldLevelInterface;
        internal WorldLevelUI worldLevelUI;
        
        public override void Load()
        {
            if (!Main.dedServ)
            {
                worldLevelUI = new WorldLevelUI();
                worldLevelInterface = new UserInterface();
                worldLevelInterface.SetState(worldLevelUI);
            }
        }
        
        public override void UpdateUI(GameTime gameTime)
        {
            worldLevelInterface?.Update(gameTime);
        }
        
        public override void ModifyInterfaceLayers(System.Collections.Generic.List<GameInterfaceLayer> layers)
        {
            int inventoryIndex = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Inventory"));
            if (inventoryIndex != -1)
            {
                layers.Insert(inventoryIndex + 1, new LegacyGameInterfaceLayer(
                    "Rpg: World Level",
                    () =>
                    {
                        worldLevelInterface?.Draw(Main.spriteBatch, new GameTime());
                        return true;
                    },
                    InterfaceScaleType.UI
                ));
            }
        }
    }
}
