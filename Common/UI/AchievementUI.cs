using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader;
using Terraria.UI;
using RpgMod.Common.Systems;

namespace RpgMod.Common.UI
{
    /// <summary>
    /// Achievement UI - displays achievement progress and unlocked achievements
    /// </summary>
    public class AchievementUI : UIState
    {
        private UIPanel mainPanel;
        private UIList achievementList;
        private UIScrollbar scrollbar;
        private UIText titleText;
        private UIText progressText;
        
        private AchievementCategory currentCategory = AchievementCategory.Leveling;
        
        public override void OnInitialize()
        {
            // Main panel
            mainPanel = new UIPanel();
            mainPanel.Width.Set(500f, 0f);
            mainPanel.Height.Set(450f, 0f);
            mainPanel.HAlign = 0.5f;
            mainPanel.VAlign = 0.5f;
            mainPanel.BackgroundColor = new Color(35, 45, 55) * 0.95f;
            mainPanel.BorderColor = new Color(70, 90, 110);
            Append(mainPanel);
            
            // Title
            titleText = new UIText("Achievements", 1.2f);
            titleText.HAlign = 0.5f;
            titleText.Top.Set(10f, 0f);
            mainPanel.Append(titleText);
            
            // Category buttons
            float categoryY = 45f;
            float categoryX = 10f;
            
            foreach (AchievementCategory category in System.Enum.GetValues(typeof(AchievementCategory)))
            {
                var btn = new UITextPanel<string>(category.ToString(), 0.8f, false);
                btn.Width.Set(65f, 0f);
                btn.Height.Set(25f, 0f);
                btn.Left.Set(categoryX, 0f);
                btn.Top.Set(categoryY, 0f);
                
                var cat = category;
                btn.OnLeftClick += (evt, elem) => SetCategory(cat);
                mainPanel.Append(btn);
                
                categoryX += 70f;
            }
            
            // Progress text
            progressText = new UIText("0%", 0.9f);
            progressText.Top.Set(80f, 0f);
            progressText.Left.Set(10f, 0f);
            mainPanel.Append(progressText);
            
            // Achievement list
            achievementList = new UIList();
            achievementList.Width.Set(-25f, 1f);
            achievementList.Height.Set(-120f, 1f);
            achievementList.Left.Set(10f, 0f);
            achievementList.Top.Set(110f, 0f);
            achievementList.ListPadding = 5f;
            mainPanel.Append(achievementList);
            
            // Scrollbar
            scrollbar = new UIScrollbar();
            scrollbar.Height.Set(-120f, 1f);
            scrollbar.Left.Set(-20f, 1f);
            scrollbar.Top.Set(110f, 0f);
            mainPanel.Append(scrollbar);
            achievementList.SetScrollbar(scrollbar);
            
            // Close button
            var closeBtn = new UITextPanel<string>("X", 0.8f, false);
            closeBtn.Width.Set(30f, 0f);
            closeBtn.Height.Set(25f, 0f);
            closeBtn.Left.Set(-35f, 1f);
            closeBtn.Top.Set(5f, 0f);
            closeBtn.BackgroundColor = new Color(150, 50, 50);
            closeBtn.OnLeftClick += (evt, elem) =>
            {
                ModContent.GetInstance<AchievementUISystem>()?.HideUI();
            };
            mainPanel.Append(closeBtn);
        }
        
        public void SetCategory(AchievementCategory category)
        {
            currentCategory = category;
            RefreshList();
        }
        
        public void RefreshList()
        {
            achievementList.Clear();
            
            var achievements = Main.LocalPlayer?.GetModPlayer<AchievementSystem>();
            if (achievements == null) return;
            
            int unlocked = 0;
            int total = 0;
            
            foreach (var kvp in AchievementSystem.Achievements)
            {
                if (kvp.Value.Category == currentCategory)
                {
                    bool isUnlocked = achievements.IsUnlocked(kvp.Key);
                    var entry = new AchievementEntry(kvp.Key, kvp.Value, isUnlocked);
                    achievementList.Add(entry);
                    total++;
                    if (isUnlocked) unlocked++;
                }
            }
            
            // Update progress
            float percent = achievements.GetCompletionPercentage();
            progressText.SetText($"Overall Progress: {percent:F1}% ({achievements.UnlockedAchievements.Count}/{AchievementSystem.Achievements.Count})");
        }
        
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            
            // Prevent clicks going through
            if (mainPanel.ContainsPoint(GetScaledMouse()))
            {
                Main.LocalPlayer.mouseInterface = true;
            }
        }

        private static Vector2 GetScaledMouse()
        {
            return Main.MouseScreen;
        }
    }
    
    /// <summary>
    /// Individual achievement entry in the list
    /// </summary>
    public class AchievementEntry : UIPanel
    {
        private string achievementId;
        private AchievementData data;
        private bool unlocked;
        
        public AchievementEntry(string id, AchievementData data, bool unlocked)
        {
            this.achievementId = id;
            this.data = data;
            this.unlocked = unlocked;
            
            Width.Set(0f, 1f);
            Height.Set(50f, 0f);
            
            BackgroundColor = unlocked 
                ? new Color(40, 80, 40) * 0.9f 
                : new Color(50, 50, 50) * 0.8f;
            BorderColor = unlocked ? Color.Gold : Color.Gray;
            
            // Achievement name
            var nameText = new UIText(data.Name, 0.9f);
            nameText.Top.Set(5f, 0f);
            nameText.Left.Set(10f, 0f);
            nameText.TextColor = unlocked ? Color.Gold : Color.White;
            Append(nameText);
            
            // Description
            var descText = new UIText(data.Description, 0.7f);
            descText.Top.Set(25f, 0f);
            descText.Left.Set(10f, 0f);
            descText.TextColor = unlocked ? Color.LightGray : Color.Gray;
            Append(descText);
            
            // XP Reward
            if (data.XpReward > 0)
            {
                string xpText = unlocked ? $"+{data.XpReward} XP (Claimed)" : $"+{data.XpReward} XP";
                var rewardText = new UIText(xpText, 0.7f);
                rewardText.Top.Set(15f, 0f);
                rewardText.Left.Set(-80f, 1f);
                rewardText.TextColor = unlocked ? Color.Green : Color.Yellow;
                Append(rewardText);
            }
            
            // Status icon
            string status = unlocked ? "✓" : "○";
            var statusText = new UIText(status, 1.1f);
            statusText.Top.Set(15f, 0f);
            statusText.Left.Set(-25f, 1f);
            statusText.TextColor = unlocked ? Color.Green : Color.Gray;
            Append(statusText);
        }
    }
    
    /// <summary>
    /// UI System for achievements
    /// </summary>
    public class AchievementUISystem : ModSystem
    {
        internal UserInterface achievementInterface;
        internal AchievementUI achievementUI;
        
        public bool IsVisible { get; private set; }
        
        public override void Load()
        {
            if (!Main.dedServ)
            {
                achievementUI = new AchievementUI();
                achievementInterface = new UserInterface();
            }
        }
        
        public override void Unload()
        {
            achievementUI = null;
            achievementInterface = null;
        }
        
        public void ShowUI()
        {
            achievementInterface?.SetState(achievementUI);
            achievementUI?.RefreshList();
            IsVisible = true;
        }
        
        public void HideUI()
        {
            achievementInterface?.SetState(null);
            IsVisible = false;
        }
        
        public void ToggleUI()
        {
            if (IsVisible)
                HideUI();
            else
                ShowUI();
        }
        
        public override void UpdateUI(GameTime gameTime)
        {
            if (IsVisible)
            {
                achievementInterface?.Update(gameTime);
            }
        }
        
        public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
        {
            int mouseIndex = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Mouse Text"));
            if (mouseIndex != -1)
            {
                layers.Insert(mouseIndex, new LegacyGameInterfaceLayer(
                    "Rpg: Achievements",
                    delegate
                    {
                        if (IsVisible)
                        {
                            achievementInterface?.Draw(Main.spriteBatch, new GameTime());
                        }
                        return true;
                    },
                    InterfaceScaleType.UI));
            }
        }
    }
}
