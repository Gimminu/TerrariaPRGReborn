using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;
using Terraria.GameInput;
using Terraria.UI;
using System.Collections.Generic;
using RpgMod.Common.Systems;
using RpgMod.Common.Config;
using System;
using Terraria.ModLoader.Config;

namespace RpgMod.Common.UI
{
    /// <summary>
    /// UI for displaying active quests
    /// </summary>
    public class QuestUI : UIState
    {
        private const int BASE_PANEL_WIDTH = 380;
        private const int BASE_PANEL_HEIGHT = 420;
        private Rectangle bounds;
        private static readonly Color PANEL_BG = new Color(18, 20, 32, 220);
        private static readonly Color PANEL_BORDER = new Color(70, 90, 140, 230);
        private static readonly Color CARD_BG = new Color(32, 38, 58, 200);
        private static readonly Color PROGRESS_BG = new Color(50, 60, 80, 220);
        private static readonly Color PROGRESS_FG = new Color(110, 200, 255, 255);
        private bool isDragging = false;
        private Vector2 dragOffset = Vector2.Zero;
        private Point MousePosition => GetScaledMouse();
        private Vector2 ScreenTopLeftUi => UiInput.ScreenTopLeftUi;
        private Vector2 ScreenBottomRightUi => UiInput.ScreenBottomRightUi;
        private int ScreenWidthUi => UiInput.ScreenWidthUi;
        private int ScreenHeightUi => UiInput.ScreenHeightUi;

        public override void OnInitialize()
        {
            Vector2 topLeft = ScreenTopLeftUi;
            bounds = new Rectangle(
                (int)(topLeft.X + ScreenWidthUi - BASE_PANEL_WIDTH - 10),
                (int)(topLeft.Y + ScreenHeightUi / 2 - BASE_PANEL_HEIGHT / 2),
                BASE_PANEL_WIDTH,
                BASE_PANEL_HEIGHT
            );
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            var config = ModContent.GetInstance<RpgClientConfig>();

            if (config != null && !config.ShowQuestUI)
                return;

            if (QuestSystem.ActiveQuests.Count == 0 && (config == null || config.HideQuestUIWhenEmpty))
                return;

            float uiScale = GetScale(config);
            bounds = BuildBounds(config, uiScale);
            HandleLayoutDrag(config, uiScale);

            // Draw panel background
            DrawPanel(spriteBatch, bounds, PANEL_BG, PANEL_BORDER);

            // Title
            string title = "Active Quests";
            float titleScale = MathHelper.Clamp(uiScale, 0.75f, 1.1f);
            Vector2 titleSize = FontAssets.MouseText.Value.MeasureString(title) * titleScale;
            Vector2 titlePos = new Vector2(
                bounds.X + bounds.Width / 2 - titleSize.X / 2,
                bounds.Y + (int)(10 * uiScale)
            );
            DrawText(spriteBatch, title, titlePos, new Color(255, 220, 150), titleScale);

            // Draw quests
            int yOffset = (int)(44 * uiScale);
            foreach (var quest in QuestSystem.ActiveQuests)
            {
                int questHeight = (int)(92 * uiScale);
                if (yOffset + questHeight > bounds.Height)
                    break; // Don't draw if it would overflow

                DrawQuest(spriteBatch, quest, yOffset, uiScale);
                yOffset += (int)(96 * uiScale);
            }
        }

        private void DrawQuest(SpriteBatch spriteBatch, QuestSystem.QuestEntry entry, int yOffset, float uiScale)
        {
            int padding = (int)(10 * uiScale);
            Rectangle questBounds = new Rectangle(
                bounds.X + padding,
                bounds.Y + yOffset,
                bounds.Width - padding * 2,
                (int)(86 * uiScale)
            );

            // Background
            DrawPanel(spriteBatch, questBounds, CARD_BG, PANEL_BORDER * 0.8f);

            // Title
            float titleScale = MathHelper.Clamp(0.9f * uiScale, 0.6f, 1.2f);
            Vector2 titlePos = new Vector2(questBounds.X + padding, questBounds.Y + (int)(5 * uiScale));
            DrawText(spriteBatch, entry.Quest.Title, titlePos, new Color(255, 255, 150), titleScale);

            // Owner tag (if applicable)
            if (entry.OwnerIndex >= 0 && entry.OwnerIndex < Main.maxPlayers)
            {
                var owner = Main.player[entry.OwnerIndex];
                if (owner != null && owner.active)
                {
                    string ownerText = $"Owner: {owner.name}";
                    float ownerScale = MathHelper.Clamp(0.7f * uiScale, 0.55f, 1f);
                    Vector2 ownerSize = FontAssets.MouseText.Value.MeasureString(ownerText) * ownerScale;
                    Vector2 ownerPos = new Vector2(questBounds.Right - ownerSize.X - padding, questBounds.Y + (int)(8 * uiScale));
                    DrawText(spriteBatch, ownerText, ownerPos, new Color(150, 200, 255), ownerScale);
                }
            }

            // Progress
            float progressScale = MathHelper.Clamp(0.8f * uiScale, 0.6f, 1.1f);
            Vector2 progressPos = new Vector2(questBounds.X + padding, questBounds.Y + (int)(26 * uiScale));
            DrawText(spriteBatch, entry.Quest.GetProgressText(), progressPos, Color.White, progressScale);

            // Progress bar (KillQuest)
            if (entry.Quest is KillQuest killQuest)
            {
                float progress = killQuest.RequiredKills > 0
                    ? MathHelper.Clamp((float)killQuest.CurrentKills / killQuest.RequiredKills, 0f, 1f)
                    : 0f;

                Rectangle barBounds = new Rectangle(questBounds.X + padding, questBounds.Y + (int)(44 * uiScale), questBounds.Width - padding * 2, (int)(12 * uiScale));
                DrawBar(spriteBatch, barBounds, PROGRESS_BG, PROGRESS_FG, progress);

                string barText = $"{killQuest.CurrentKills}/{killQuest.RequiredKills}";
                float barTextScale = MathHelper.Clamp(0.65f * uiScale, 0.5f, 0.95f);
                Vector2 textSize = FontAssets.MouseText.Value.MeasureString(barText) * barTextScale;
                Vector2 textPos = new Vector2(
                    barBounds.X + barBounds.Width - textSize.X - 2,
                    barBounds.Y - 2
                );
                DrawText(spriteBatch, barText, textPos, Color.LightGray, barTextScale);
            }

            // Rewards
            string rewards = $"Rewards: {entry.Quest.XPReward} XP";
            if (entry.Quest.StatPointReward > 0) rewards += $", {entry.Quest.StatPointReward} Stat";
            if (entry.Quest.SkillPointReward > 0) rewards += $", {entry.Quest.SkillPointReward} Skill";
            float rewardScale = MathHelper.Clamp(0.7f * uiScale, 0.55f, 1f);
            Vector2 rewardPos = new Vector2(questBounds.X + padding, questBounds.Y + (int)(62 * uiScale));
            DrawText(spriteBatch, rewards, rewardPos, new Color(150, 255, 150), rewardScale);
        }

        private float GetScale(RpgClientConfig config)
        {
            return MathHelper.Clamp(config?.QuestUIScale ?? 1f, 0.5f, 1.5f);
        }

        private Rectangle BuildBounds(RpgClientConfig config, float uiScale)
        {
            int width = (int)(BASE_PANEL_WIDTH * uiScale);
            int height = GetPanelHeight(uiScale);
            float anchorX = MathHelper.Clamp(config?.QuestUIPosX ?? 0.85f, 0f, 1f);
            float anchorY = MathHelper.Clamp(config?.QuestUIPosY ?? 0.5f, 0f, 1f);

            Vector2 topLeft = ScreenTopLeftUi;
            int x = (int)(topLeft.X + ScreenWidthUi * anchorX - width / 2);
            int y = (int)(topLeft.Y + ScreenHeightUi * anchorY - height / 2);
            x = (int)MathHelper.Clamp(x, topLeft.X, Math.Max(topLeft.X, topLeft.X + ScreenWidthUi - width));
            y = (int)MathHelper.Clamp(y, topLeft.Y, Math.Max(topLeft.Y, topLeft.Y + ScreenHeightUi - height));

            return new Rectangle(x, y, width, height);
        }

        private int GetPanelHeight(float uiScale)
        {
            int baseHeight = (int)(BASE_PANEL_HEIGHT * uiScale);
            int questCount = QuestSystem.ActiveQuests.Count;
            int headerHeight = (int)(44 * uiScale);
            int questHeight = (int)(92 * uiScale);
            int questSpacing = (int)(96 * uiScale);

            int visibleCount = Math.Max(questCount, 1);
            int desiredHeight = headerHeight + questHeight + (visibleCount - 1) * questSpacing;
            int minHeight = (int)(80 * uiScale);
            desiredHeight = Math.Max(desiredHeight, minHeight);

            int maxHeight = Math.Max(minHeight, ScreenHeightUi - (int)(20 * uiScale));
            return Math.Min(baseHeight, Math.Min(desiredHeight, maxHeight));
        }

        private void HandleLayoutDrag(RpgClientConfig config, float uiScale)
        {
            if (config == null || !config.EnableUILayoutMode)
                return;

            Vector2 mouse = MousePosition.ToVector2();
            bool hovering = bounds.Contains(MousePosition);

            // Start drag when clicking inside the panel
            if (!isDragging && hovering && Main.mouseLeft && Main.mouseLeftRelease)
            {
                isDragging = true;
                dragOffset = mouse - bounds.Location.ToVector2();
                Main.LocalPlayer.mouseInterface = true;
            }

            if (isDragging)
            {
                Main.LocalPlayer.mouseInterface = true;

                if (!Main.mouseLeft)
                {
                    isDragging = false;
                    SaveQuestAnchor(config);
                    return;
                }

                int newX = (int)(mouse.X - dragOffset.X);
                int newY = (int)(mouse.Y - dragOffset.Y);

                Vector2 topLeft = ScreenTopLeftUi;
                bounds.X = (int)MathHelper.Clamp(newX, topLeft.X, Math.Max(topLeft.X, topLeft.X + ScreenWidthUi - bounds.Width));
                bounds.Y = (int)MathHelper.Clamp(newY, topLeft.Y, Math.Max(topLeft.Y, topLeft.Y + ScreenHeightUi - bounds.Height));

                // Save anchor back into config.
                config.QuestUIPosX = MathHelper.Clamp((float)(bounds.Center.X - topLeft.X) / ScreenWidthUi, 0f, 1f);
                config.QuestUIPosY = MathHelper.Clamp((float)(bounds.Center.Y - topLeft.Y) / ScreenHeightUi, 0f, 1f);
            }
        }

        private void SaveQuestAnchor(RpgClientConfig config)
        {
            if (config == null)
                return;

            config.SaveChanges(config, null, true, true);
        }

        private static Point GetScaledMouse()
        {
            return UiInput.GetUiMousePoint();
        }

        private void DrawPanel(SpriteBatch spriteBatch, Rectangle bounds, Color bg, Color border)
        {
            Texture2D texture = Terraria.GameContent.TextureAssets.MagicPixel.Value;
            spriteBatch.Draw(texture, bounds, bg);
            // Border
            spriteBatch.Draw(texture, new Rectangle(bounds.X, bounds.Y, bounds.Width, 2), border);
            spriteBatch.Draw(texture, new Rectangle(bounds.X, bounds.Bottom - 2, bounds.Width, 2), border);
            spriteBatch.Draw(texture, new Rectangle(bounds.X, bounds.Y, 2, bounds.Height), border);
            spriteBatch.Draw(texture, new Rectangle(bounds.Right - 2, bounds.Y, 2, bounds.Height), border);
        }

        private void DrawText(SpriteBatch spriteBatch, string text, Vector2 position, Color color, float scale)
        {
            Utils.DrawBorderString(spriteBatch, text, position, color, scale);
        }

        private void DrawBar(SpriteBatch spriteBatch, Rectangle bounds, Color bg, Color fg, float fill)
        {
            Texture2D texture = Terraria.GameContent.TextureAssets.MagicPixel.Value;
            spriteBatch.Draw(texture, bounds, bg);
            int fillWidth = (int)(bounds.Width * MathHelper.Clamp(fill, 0f, 1f));
            if (fillWidth > 0)
            {
                var fillRect = new Rectangle(bounds.X, bounds.Y, fillWidth, bounds.Height);
                spriteBatch.Draw(texture, fillRect, fg);
            }
        }
    }

    public class QuestUISystem : ModSystem
    {
        public static ModKeybind ToggleKey { get; private set; }
        internal UserInterface questInterface;
        internal QuestUI questUI;

        public override void Load()
        {
            ToggleKey = KeybindLoader.RegisterKeybind(Mod, "Toggle Quest UI", "O");
            if (!Main.dedServ)
            {
                questUI = new QuestUI();
                questInterface = new UserInterface();
                questInterface.SetState(questUI);
            }
        }

        public override void Unload()
        {
            ToggleKey = null;
            questInterface = null;
            questUI = null;
        }

        public override void UpdateUI(GameTime gameTime)
        {
            questInterface?.Update(gameTime);
        }

        public override void ModifyInterfaceLayers(System.Collections.Generic.List<GameInterfaceLayer> layers)
        {
            int inventoryIndex = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Inventory"));
            if (inventoryIndex != -1)
            {
                layers.Insert(inventoryIndex + 1, new LegacyGameInterfaceLayer(
                    "Rpg: Quests",
                    () =>
                    {
                        questInterface?.Draw(Main.spriteBatch, new GameTime());
                        return true;
                    },
                    InterfaceScaleType.UI
                ));
            }
        }

        public void SetQuestUIVisible(bool visible, bool showMessage)
        {
            var config = ModContent.GetInstance<RpgClientConfig>();
            if (config == null)
                return;

            if (config.ShowQuestUI == visible)
                return;

            config.ShowQuestUI = visible;
            config.SaveChanges(config, null, true, true);
            if (showMessage)
            {
                string message = config.ShowQuestUI ? "Quest UI: On" : "Quest UI: Off";
                Main.NewText(message, new Color(150, 200, 255));
            }
        }

        public void ToggleQuestUI(bool showMessage = true)
        {
            var config = ModContent.GetInstance<RpgClientConfig>();
            if (config == null)
                return;

            SetQuestUIVisible(!config.ShowQuestUI, showMessage);
        }
    }

    public class QuestUIPlayer : ModPlayer
    {
        public override void ProcessTriggers(TriggersSet triggersSet)
        {
            if (QuestUISystem.ToggleKey == null)
                return;

            if (Main.drawingPlayerChat || Main.editSign || Main.editChest ||
                Main.gameMenu || Main.ingameOptionsWindow || Main.playerInventory || Main.inFancyUI)
                return;

            if (QuestUISystem.ToggleKey.JustPressed)
            {
                ModContent.GetInstance<QuestUISystem>().ToggleQuestUI();
            }
        }
    }
}
