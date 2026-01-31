using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.Config;
using Terraria.UI;
using RpgMod.Common.Players;
using RpgMod.Common.Skills;
using RpgMod.Common.Base;
using RpgMod.Common.Systems;
using RpgMod.Common.Config;
using RpgMod.Common.Stats;

namespace RpgMod.Common.UI
{
    /// <summary>
    /// Master UI - Unified window for Stats, Job, Skills, and Achievements
    /// Accessed with a single keybind (default: C)
    /// </summary>
    public class MasterUI : UIState
    {
        public enum Tab { Stats, Job, Skills, Achieve, Damage }
        private enum JobFilter { All, Next, Available, Tier1, Tier2, Tier3 }
        private enum SkillFilter { All, Novice, Tier1, Tier2, Tier3 }
        
        private RpgPlayer rpgPlayer;
        private SkillManager skillManager;
        
        // UI State
        private Tab currentTab = Tab.Stats;
        private bool dragging = false;
        private Vector2 dragOffset;
        private static Rectangle? lastBounds;
        private JobFilter jobFilter = JobFilter.Next;
        private SkillFilter skillFilter = SkillFilter.All;
        private SkillFilter lastSkillFilter = SkillFilter.All;
        private JobType lastSkillJob = JobType.None;
        private bool leftClick;
        private bool wasLeftDown;
        
        // Panel dimensions
        private Rectangle bounds;
        private const int PANEL_WIDTH = 600;
        private const int PANEL_HEIGHT = 650;
        private const int HEADER_HEIGHT = 50;
        private const int TAB_HEIGHT = 40;
        
        // Tab buttons
        private Rectangle tabStatsBtn;
        private Rectangle tabJobBtn;
        private Rectangle tabSkillsBtn;
        private Rectangle tabAchieveBtn;
        private Rectangle tabDamageBtn;
        
        // Stat allocation buttons
        private Dictionary<StatType, Rectangle> statPlusBtns = new();
        private Dictionary<StatType, Rectangle> statMinusBtns = new();
        private Dictionary<StatType, Rectangle> statRowBounds = new();
        
        // Skill list scroll
        private int skillScrollOffset = 0;
        private List<BaseSkill> allSkills = new();
        private List<BaseSkill> availableSkills = new();
        
        // Job selection
        private List<JobType> availableJobs = new();
        private int jobScrollOffset = 0;

        // UI scale helpers (UI draws in scaled space; convert using UIScale)
        private Point MousePosition => GetScaledMouse();
        private Vector2 ScreenTopLeftUi => UiInput.ScreenTopLeftUi;
        private Vector2 ScreenBottomRightUi => UiInput.ScreenBottomRightUi;
        private int ScreenWidthUi => UiInput.ScreenWidthUi;
        private int ScreenHeightUi => UiInput.ScreenHeightUi;
        
        public override void OnInitialize()
        {
            if (lastBounds.HasValue)
            {
                bounds = lastBounds.Value;
                return;
            }

            var config = ModContent.GetInstance<RpgClientConfig>();
            float anchorX = MathHelper.Clamp(config?.RpgMenuPosX ?? 0.5f, 0f, 1f);
            float anchorY = MathHelper.Clamp(config?.RpgMenuPosY ?? 0.5f, 0f, 1f);
            bounds = BuildMenuBounds(anchorX, anchorY);
        }

        public override void Recalculate()
        {
            var config = ModContent.GetInstance<RpgClientConfig>();
            float anchorX = MathHelper.Clamp(config?.RpgMenuPosX ?? 0.5f, 0f, 1f);
            float anchorY = MathHelper.Clamp(config?.RpgMenuPosY ?? 0.5f, 0f, 1f);
            bounds = BuildMenuBounds(anchorX, anchorY);
        }
        
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            bool leftDown = Main.mouseLeft;
            leftClick = leftDown && !wasLeftDown;
            wasLeftDown = leftDown;

            if (Main.LocalPlayer == null || !Main.LocalPlayer.active)
                return;
                
            rpgPlayer = Main.LocalPlayer.GetModPlayer<RpgPlayer>();
            skillManager = Main.LocalPlayer.GetModPlayer<SkillManager>();
            
            // Update available data based on current tab
            if (currentTab == Tab.Skills)
            {
                UpdateSkillList();
            }
            else if (currentTab == Tab.Job)
            {
                availableJobs = Jobs.JobDatabase.GetAllJobs();
            }
            
            // Handle dragging
            HandleDragging();
            
            // Handle scrolling
            if (bounds.Contains(MousePosition))
            {
                int scroll = Terraria.GameInput.PlayerInput.ScrollWheelDelta;
                if (currentTab == Tab.Skills)
                {
                    skillScrollOffset -= scroll / 30;
                    skillScrollOffset = System.Math.Max(0, skillScrollOffset);
                }
                else if (currentTab == Tab.Job)
                {
                    jobScrollOffset -= scroll / 30;
                    jobScrollOffset = System.Math.Max(0, jobScrollOffset);
                }
            }
            
            // Close on ESC
            if (Main.keyState.IsKeyDown(Keys.Escape))
            {
                ModContent.GetInstance<MasterUISystem>().HideUI();
            }
        }
        
        private void HandleDragging()
        {
            Point mouse = MousePosition;
            Rectangle header = new Rectangle(bounds.X, bounds.Y, bounds.Width, HEADER_HEIGHT);
            Rectangle closeBtn = new Rectangle(bounds.X + bounds.Width - 35, bounds.Y + 10, 25, 25);
            
            if (!dragging && Main.mouseLeft && header.Contains(mouse) && !closeBtn.Contains(mouse))
            {
                dragging = true;
                dragOffset = new Vector2(mouse.X - bounds.X, mouse.Y - bounds.Y);
            }
            else if (dragging && !Main.mouseLeft)
            {
                dragging = false;
                lastBounds = bounds;
                SaveMenuAnchor();
            }
            
            if (dragging)
            {
                bounds.X = (int)(mouse.X - dragOffset.X);
                bounds.Y = (int)(mouse.Y - dragOffset.Y);
                
                // Clamp to screen
                Vector2 topLeft = ScreenTopLeftUi;
                bounds.X = Utils.Clamp(bounds.X, (int)topLeft.X, (int)(topLeft.X + ScreenWidthUi - PANEL_WIDTH));
                bounds.Y = Utils.Clamp(bounds.Y, (int)topLeft.Y, (int)(topLeft.Y + ScreenHeightUi - PANEL_HEIGHT));
            }
        }
        
        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            if (rpgPlayer == null || skillManager == null)
                return;
            
            // Main panel background
            DrawPanel(spriteBatch, bounds, new Color(20, 25, 40, 245), new Color(60, 70, 100));
            
            // Header
            DrawHeader(spriteBatch);
            
            // Tabs
            DrawTabs(spriteBatch);
            
            // Content area
            Rectangle contentArea = new Rectangle(
                bounds.X + 10,
                bounds.Y + HEADER_HEIGHT + TAB_HEIGHT + 10,
                bounds.Width - 20,
                bounds.Height - HEADER_HEIGHT - TAB_HEIGHT - 20
            );
            
            switch (currentTab)
            {
                case Tab.Stats:
                    DrawStatsTab(spriteBatch, contentArea);
                    break;
                case Tab.Job:
                    DrawJobTab(spriteBatch, contentArea);
                    break;
                case Tab.Skills:
                    DrawSkillsTab(spriteBatch, contentArea);
                    break;
                case Tab.Achieve:
                    DrawAchievementsTab(spriteBatch, contentArea);
                    break;
                case Tab.Damage:
                    DrawDamageTab(spriteBatch, contentArea);
                    break;
            }
            
            // Prevent clicking through
            if (bounds.Contains(MousePosition))
            {
                Main.LocalPlayer.mouseInterface = true;
            }
        }
        
        private void DrawHeader(SpriteBatch spriteBatch)
        {
            Rectangle headerBounds = new Rectangle(bounds.X, bounds.Y, bounds.Width, HEADER_HEIGHT);
            DrawPanel(spriteBatch, headerBounds, new Color(30, 40, 60, 255), new Color(80, 100, 140));
            
            // Title
            string title = "RPG Menu";
            Vector2 titleSize = FontAssets.DeathText.Value.MeasureString(title) * 0.5f;
            Vector2 titlePos = new Vector2(
                bounds.X + bounds.Width / 2 - titleSize.X / 2,
                bounds.Y + HEADER_HEIGHT / 2 - titleSize.Y / 2
            );
            Utils.DrawBorderString(spriteBatch, title, titlePos, Color.Gold, 0.6f);

            DrawQuestToggle(spriteBatch);
            
            // Close button
            Rectangle closeBtn = new Rectangle(bounds.X + bounds.Width - 35, bounds.Y + 10, 25, 25);
            DrawPanel(spriteBatch, closeBtn, new Color(180, 50, 50), new Color(220, 80, 80));
            
            string x = "X";
            Vector2 xSize = FontAssets.MouseText.Value.MeasureString(x);
            Vector2 xPos = new Vector2(closeBtn.X + closeBtn.Width / 2 - xSize.X / 2, closeBtn.Y + closeBtn.Height / 2 - xSize.Y / 2);
            Utils.DrawBorderString(spriteBatch, x, xPos, Color.White, 1f);
            
            if (closeBtn.Contains(MousePosition) && leftClick)
            {
                ModContent.GetInstance<MasterUISystem>().HideUI();
                SoundEngine.PlaySound(SoundID.MenuClose);
            }
        }

        private void DrawQuestToggle(SpriteBatch spriteBatch)
        {
            var config = ModContent.GetInstance<RpgClientConfig>();
            if (config == null)
                return;

            string label = config.ShowQuestUI ? "Quest UI: On" : "Quest UI: Off";
            int buttonWidth = 110;
            int buttonHeight = 22;
            int rightEdge = bounds.X + bounds.Width - 35 - 8;
            Rectangle toggleBtn = new Rectangle(rightEdge - buttonWidth, bounds.Y + 14, buttonWidth, buttonHeight);
            bool hover = toggleBtn.Contains(MousePosition);

            Color bg = config.ShowQuestUI ? new Color(55, 85, 120) : new Color(40, 50, 70);
            Color border = config.ShowQuestUI ? new Color(110, 150, 210) : new Color(80, 95, 120);
            if (hover)
                bg = config.ShowQuestUI ? new Color(70, 110, 150) : new Color(55, 70, 95);

            DrawPanel(spriteBatch, toggleBtn, bg, border);

            Vector2 textSize = FontAssets.MouseText.Value.MeasureString(label) * 0.7f;
            Vector2 textPos = new Vector2(
                toggleBtn.X + toggleBtn.Width / 2 - textSize.X / 2,
                toggleBtn.Y + toggleBtn.Height / 2 - textSize.Y / 2
            );
            Utils.DrawBorderString(spriteBatch, label, textPos, Color.White, 0.7f);

            if (hover)
                Main.hoverItemName = "Toggle Quest UI (O)";

            if (hover && leftClick)
            {
                ModContent.GetInstance<QuestUISystem>()?.ToggleQuestUI(false);
                SoundEngine.PlaySound(SoundID.MenuTick);
            }
        }
        
        private void DrawTabs(SpriteBatch spriteBatch)
        {
            var clientConfig = ModContent.GetInstance<RpgClientConfig>();
            var serverConfig = ModContent.GetInstance<RpgServerConfig>();
            
            // Determine which tabs to show
            bool showJobTab = serverConfig?.EnableJobSystem ?? true;
            bool showSkillTab = serverConfig?.EnableSkillSystem ?? true;
            
            // Calculate tab layout based on enabled tabs
            List<Tab> enabledTabs = new List<Tab> { Tab.Stats };
            if (showJobTab) enabledTabs.Add(Tab.Job);
            if (showSkillTab) enabledTabs.Add(Tab.Skills);
            enabledTabs.Add(Tab.Achieve);
            enabledTabs.Add(Tab.Damage);
            
            int tabCount = enabledTabs.Count;
            int tabWidth = (bounds.Width - 60) / tabCount;
            int tabY = bounds.Y + HEADER_HEIGHT + 5;
            
            // Reset tab buttons
            tabStatsBtn = Rectangle.Empty;
            tabJobBtn = Rectangle.Empty;
            tabSkillsBtn = Rectangle.Empty;
            tabAchieveBtn = Rectangle.Empty;
            tabDamageBtn = Rectangle.Empty;
            
            for (int i = 0; i < enabledTabs.Count; i++)
            {
                Tab tab = enabledTabs[i];
                Rectangle tabRect = new Rectangle(bounds.X + 10 + i * (tabWidth + 5), tabY, tabWidth, TAB_HEIGHT - 10);
                
                string label = tab switch
                {
                    Tab.Stats => "Stats",
                    Tab.Job => "Job",
                    Tab.Skills => "Skills",
                    Tab.Achieve => "Achieve",
                    Tab.Damage => "Damage",
                    _ => "Unknown"
                };
                
                DrawTabButton(spriteBatch, tabRect, label, tab);
                
                // Assign to appropriate button rect
                switch (tab)
                {
                    case Tab.Stats: tabStatsBtn = tabRect; break;
                    case Tab.Job: tabJobBtn = tabRect; break;
                    case Tab.Skills: tabSkillsBtn = tabRect; break;
                    case Tab.Achieve: tabAchieveBtn = tabRect; break;
                    case Tab.Damage: tabDamageBtn = tabRect; break;
                }
            }
            
            // If current tab is disabled, switch to Stats
            if ((currentTab == Tab.Job && !showJobTab) || (currentTab == Tab.Skills && !showSkillTab))
            {
                currentTab = Tab.Stats;
            }
        }
        
        private void DrawTabButton(SpriteBatch spriteBatch, Rectangle rect, string label, Tab tab)
        {
            bool active = currentTab == tab;
            bool hover = rect.Contains(MousePosition);
            
            Color bg = active ? new Color(60, 80, 120) : (hover ? new Color(50, 60, 90) : new Color(35, 45, 70));
            Color border = active ? new Color(100, 150, 220) : new Color(70, 80, 100);
            
            DrawPanel(spriteBatch, rect, bg, border);
            
            Vector2 textSize = FontAssets.MouseText.Value.MeasureString(label);
            Vector2 textPos = new Vector2(rect.X + rect.Width / 2 - textSize.X / 2, rect.Y + rect.Height / 2 - textSize.Y / 2);
            Utils.DrawBorderString(spriteBatch, label, textPos, active ? Color.White : Color.LightGray, 1.1f);
            
            if (hover && leftClick)
            {
                currentTab = tab;
                SoundEngine.PlaySound(SoundID.MenuTick);
            }
        }
        
        #region Stats Tab
        
        private void DrawStatsTab(SpriteBatch spriteBatch, Rectangle area)
        {
            int y = area.Y;
            
            // Available points
            string pointsText = rpgPlayer.PendingStatPoints > 0
                ? $"Stat Points: {rpgPlayer.StatPoints} (Pending: {rpgPlayer.PendingStatPoints})"
                : $"Stat Points: {rpgPlayer.StatPoints}";
            Utils.DrawBorderString(spriteBatch, pointsText, new Vector2(area.X + area.Width / 2 - FontAssets.MouseText.Value.MeasureString(pointsText).X / 2, y), Color.Gold, 1.1f);
            y += 30;
            
            // Instructions
            string instr = "Click: +1 | Shift: +5 | Ctrl: +10 | Ctrl+Shift: All";
            Utils.DrawBorderString(spriteBatch, instr, new Vector2(area.X + 10, y), Color.Gray, 0.8f);
            y += 18;

            string autoHint = "Auto growth shown as A (Base/Auto/Bonus).";
            Utils.DrawBorderString(spriteBatch, autoHint, new Vector2(area.X + 10, y), Color.Gray, 0.7f);
            y += 22;
            
            // Stats
            statPlusBtns.Clear();
            statMinusBtns.Clear();
            statRowBounds.Clear();
            DrawStatRow(spriteBatch, area, ref y, StatType.Strength, rpgPlayer.Strength, rpgPlayer.AutoStrength, rpgPlayer.BonusStrength);
            DrawStatRow(spriteBatch, area, ref y, StatType.Dexterity, rpgPlayer.Dexterity, rpgPlayer.AutoDexterity, rpgPlayer.BonusDexterity);
            DrawStatRow(spriteBatch, area, ref y, StatType.Rogue, rpgPlayer.Rogue, rpgPlayer.AutoRogue, rpgPlayer.BonusRogue);
            DrawStatRow(spriteBatch, area, ref y, StatType.Intelligence, rpgPlayer.Intelligence, rpgPlayer.AutoIntelligence, rpgPlayer.BonusIntelligence);
            DrawStatRow(spriteBatch, area, ref y, StatType.Focus, rpgPlayer.Focus, rpgPlayer.AutoFocus, rpgPlayer.BonusFocus);
            DrawStatRow(spriteBatch, area, ref y, StatType.Vitality, rpgPlayer.Vitality, rpgPlayer.AutoVitality, rpgPlayer.BonusVitality);
            DrawStatRow(spriteBatch, area, ref y, StatType.Stamina, rpgPlayer.StaminaStat, rpgPlayer.AutoStamina, rpgPlayer.BonusStamina);
            DrawStatRow(spriteBatch, area, ref y, StatType.Defense, rpgPlayer.Defense, rpgPlayer.AutoDefense, rpgPlayer.BonusDefense);
            DrawStatRow(spriteBatch, area, ref y, StatType.Agility, rpgPlayer.Agility, rpgPlayer.AutoAgility, rpgPlayer.BonusAgility);
            DrawStatRow(spriteBatch, area, ref y, StatType.Wisdom, rpgPlayer.Wisdom, rpgPlayer.AutoWisdom, rpgPlayer.BonusWisdom);
            DrawStatRow(spriteBatch, area, ref y, StatType.Fortitude, rpgPlayer.Fortitude, rpgPlayer.AutoFortitude, rpgPlayer.BonusFortitude);
            DrawStatRow(spriteBatch, area, ref y, StatType.Luck, rpgPlayer.Luck, rpgPlayer.AutoLuck, rpgPlayer.BonusLuck);
        }
        
        private void DrawStatRow(SpriteBatch spriteBatch, Rectangle area, ref int y, StatType stat, int baseVal, int autoVal, int bonus)
        {
            var def = StatDefinitions.GetDefinition(stat);
            string abbr = def.Abbrev;
            Color color = def.Color;
            Point mouse = MousePosition;
            int rowHeight = 36;
            Rectangle rowRect = new Rectangle(area.X, y, area.Width, rowHeight);
            
            // Stat name
            Utils.DrawBorderString(spriteBatch, abbr, new Vector2(area.X, y + 5), color, 1.1f);
            
            // Values
            int totalValue = baseVal + autoVal + bonus;
            string total = $"{totalValue}";
            string detail = BuildStatDetail(baseVal, autoVal, bonus);
            
            Utils.DrawBorderString(spriteBatch, total, new Vector2(area.X + 60, y + 5), Color.White, 1.1f);
            Utils.DrawBorderString(spriteBatch, detail, new Vector2(area.X + 110, y + 5), Color.Gray, 0.9f);
            
            // + Button
            Rectangle plusBtn = new Rectangle(area.X + area.Width - 40, y, 35, rowHeight - 4);
            bool hoverPlus = plusBtn.Contains(mouse);
            bool canAllocate = rpgPlayer.StatPoints > 0;
            
            Color btnColor = canAllocate ? (hoverPlus ? new Color(80, 150, 80) : new Color(50, 120, 50)) : new Color(60, 60, 60);
            DrawPanel(spriteBatch, plusBtn, btnColor, canAllocate ? new Color(100, 180, 100) : new Color(80, 80, 80));
            
            string plus = "+";
            Vector2 plusSize = FontAssets.MouseText.Value.MeasureString(plus);
            Utils.DrawBorderString(spriteBatch, plus, new Vector2(plusBtn.X + plusBtn.Width / 2 - plusSize.X / 2, plusBtn.Y + plusBtn.Height / 2 - plusSize.Y / 2), canAllocate ? Color.White : Color.Gray);
            
            statPlusBtns[stat] = plusBtn;
            
            // - Button
            Rectangle minusBtn = new Rectangle(area.X + area.Width - 80, y, 35, rowHeight - 4);
            bool hoverMinus = minusBtn.Contains(mouse);
            bool canDeallocate = baseVal > 0;
            
            Color minusBtnColor = canDeallocate ? (hoverMinus ? new Color(150, 80, 80) : new Color(120, 50, 50)) : new Color(60, 60, 60);
            DrawPanel(spriteBatch, minusBtn, minusBtnColor, canDeallocate ? new Color(180, 100, 100) : new Color(80, 80, 80));
            
            string minus = "-";
            Vector2 minusSize = FontAssets.MouseText.Value.MeasureString(minus);
            Utils.DrawBorderString(spriteBatch, minus, new Vector2(minusBtn.X + minusBtn.Width / 2 - minusSize.X / 2, minusBtn.Y + minusBtn.Height / 2 - minusSize.Y / 2), canDeallocate ? Color.White : Color.Gray);
            
            statMinusBtns[stat] = minusBtn;

            statRowBounds[stat] = rowRect;

            if (rowRect.Contains(mouse))
            {
                DrawStatTooltip(spriteBatch, stat, baseVal, autoVal, bonus);
            }
            
            // Handle clicks
            if (hoverPlus && leftClick && canAllocate)
            {
                int amount = 1;
                if (Main.keyState.IsKeyDown(Keys.LeftControl) && Main.keyState.IsKeyDown(Keys.LeftShift))
                    amount = rpgPlayer.StatPoints;
                else if (Main.keyState.IsKeyDown(Keys.LeftControl))
                    amount = 10;
                else if (Main.keyState.IsKeyDown(Keys.LeftShift))
                    amount = 5;
                    
                rpgPlayer.AllocateStatPoint(stat, amount);
                SoundEngine.PlaySound(SoundID.MenuTick);
            }
            else if (hoverMinus && leftClick && canDeallocate)
            {
                int amount = 1;
                if (Main.keyState.IsKeyDown(Keys.LeftControl) && Main.keyState.IsKeyDown(Keys.LeftShift))
                    amount = baseVal; // Max available
                else if (Main.keyState.IsKeyDown(Keys.LeftControl))
                    amount = 10;
                else if (Main.keyState.IsKeyDown(Keys.LeftShift))
                    amount = 5;
                    
                amount = System.Math.Min(amount, baseVal);
                rpgPlayer.DeallocateStatPoint(stat, amount);
                SoundEngine.PlaySound(SoundID.MenuTick);
            }
            
            y += rowHeight;
        }
        
        #endregion
        
        #region Job Tab
        
        private void DrawJobTab(SpriteBatch spriteBatch, Rectangle area)
        {
            var serverConfig = ModContent.GetInstance<RpgServerConfig>();
            if (serverConfig != null && !serverConfig.EnableJobSystem)
            {
                string disabledText = "Job system is disabled on this server.";
                Vector2 textSize = FontAssets.MouseText.Value.MeasureString(disabledText);
                Vector2 textPos = new Vector2(area.X + area.Width / 2f - textSize.X / 2f, area.Y + area.Height / 2f - textSize.Y / 2f);
                Utils.DrawBorderString(spriteBatch, disabledText, textPos, Color.Red, 1.1f);
                return;
            }
            
            int y = area.Y;
            
            // Current job
            var currentJobData = Jobs.JobDatabase.GetJobData(rpgPlayer.CurrentJob);
            if (currentJobData != null)
            {
                string currentText = $"Current: {currentJobData.DisplayName}";
                Color tierColor = GetJobTierColor(currentJobData.Tier);
                Utils.DrawBorderString(spriteBatch, currentText, new Vector2(area.X, y), tierColor, 1.1f);
                y += 25;
                
                // Description
                Utils.DrawBorderString(spriteBatch, currentJobData.Description, new Vector2(area.X, y), Color.LightGray, 0.85f);
                y += 20;
                
                // Resign button (if not Novice)
                if (currentJobData.Tier != JobTier.Novice)
                {
                    Rectangle resignBtn = new Rectangle(area.X + area.Width - 100, y - 35, 90, 30);
                    bool hoverResign = resignBtn.Contains(MousePosition);
                    
                    Color resignColor = hoverResign ? new Color(180, 80, 80) : new Color(150, 60, 60);
                    DrawPanel(spriteBatch, resignBtn, resignColor, new Color(200, 100, 100));
                    
                    string resignText = "Resign";
                    Vector2 resignSize = FontAssets.MouseText.Value.MeasureString(resignText);
                    Utils.DrawBorderString(spriteBatch, resignText, 
                        new Vector2(resignBtn.X + resignBtn.Width / 2 - resignSize.X / 2, resignBtn.Y + resignBtn.Height / 2 - resignSize.Y / 2), 
                        Color.White);
                    
                    // Handle click
                    if (hoverResign && leftClick)
                    {
                        if (!Main.keyState.IsKeyDown(Keys.LeftShift) && !Main.keyState.IsKeyDown(Keys.RightShift))
                        {
                            Main.NewText("Hold Shift and click to resign from your current job.", Color.Yellow);
                        }
                        else
                        {
                            // Resign to Novice
                            var levelSystem = Main.LocalPlayer.GetModPlayer<Players.PlayerLevel>();
                            if (levelSystem != null && levelSystem.ResignJob())
                            {
                                Main.NewText("You have resigned from your job and returned to Novice.", Color.Orange);
                                SoundEngine.PlaySound(SoundID.MenuTick);
                            }
                        }
                    }
                }
                
                y += 20;
            }
            else
            {
                // Fallback if job data not found
                string fallbackText = $"Current: {rpgPlayer.CurrentJob.ToString()} (Data not found)";
                Utils.DrawBorderString(spriteBatch, fallbackText, new Vector2(area.X, y), Color.Red, 1.1f);
                y += 25;
            }
            
            // Separator
            spriteBatch.Draw(TextureAssets.MagicPixel.Value, new Rectangle(area.X, y, area.Width, 2), new Color(60, 70, 90));
            y += 10;
            
            // Filter row
            DrawJobFilterRow(spriteBatch, area, ref y);
            
            // Job list (name-only)
            int rowHeight = 36;
            int rowSpacing = 4;
            List<JobType> filteredJobs = GetFilteredJobs();
            int visibleCards = (area.Y + area.Height - y) / (rowHeight + rowSpacing);
            int maxOffset = System.Math.Max(0, filteredJobs.Count - visibleCards);
            jobScrollOffset = System.Math.Min(jobScrollOffset, maxOffset);
            
            for (int i = jobScrollOffset; i < filteredJobs.Count && i < jobScrollOffset + visibleCards; i++)
            {
                var job = filteredJobs[i];
                
                var jobData = Jobs.JobDatabase.GetJobData(job);
                if (jobData == null) continue;
                
                DrawJobRow(spriteBatch, new Rectangle(area.X, y, area.Width, rowHeight), jobData, job);
                y += rowHeight + rowSpacing;
            }

            if (filteredJobs.Count == 0)
            {
                Utils.DrawBorderString(spriteBatch, "No jobs for this filter.", new Vector2(area.X, y), Color.Gray);
            }
        }
        
        private void DrawJobRow(SpriteBatch spriteBatch, Rectangle rect, Jobs.JobData jobData, JobType jobType)
        {
            bool hover = rect.Contains(MousePosition);
            bool canChange = Jobs.JobDatabase.CanUnlockJob(rpgPlayer, jobType);
            
            Color bg = canChange
                ? (hover ? new Color(50, 70, 100) : new Color(35, 50, 80))
                : (hover ? new Color(45, 45, 55) : new Color(35, 35, 45));
            Color border = canChange ? GetJobTierColor(jobData.Tier) : new Color(70, 70, 80);
            
            DrawPanel(spriteBatch, rect, bg, border);
            
            string tierText = GetJobTierLabel(jobData.Tier);
            Vector2 tierSize = FontAssets.MouseText.Value.MeasureString(tierText) * 0.7f;
            Vector2 tierPos = new Vector2(rect.Right - tierSize.X - 10, rect.Y + 6);
            Color nameColor = canChange ? Color.White : Color.Gray;
            
            Utils.DrawBorderString(spriteBatch, jobData.DisplayName, new Vector2(rect.X + 10, rect.Y + 6), nameColor, 1.0f);
            Utils.DrawBorderString(spriteBatch, tierText, tierPos, Color.Gray, 0.8f);
            
            if (hover)
            {
                Main.hoverItemName = BuildJobTooltip(jobData, canChange);
                if (leftClick && canChange)
                {
                    var levelSystem = Main.LocalPlayer.GetModPlayer<Players.PlayerLevel>();
                    if (levelSystem == null || !levelSystem.AdvanceJob(jobType))
                    {
                        Main.NewText("Cannot change job yet.", Color.Red);
                    }
                }
            }
        }

        private void DrawJobFilterRow(SpriteBatch spriteBatch, Rectangle area, ref int y)
        {
            Point mouse = MousePosition;
            string label = "Filter:";
            Utils.DrawBorderString(spriteBatch, label, new Vector2(area.X, y), Color.White, 0.8f);
            float labelWidth = FontAssets.MouseText.Value.MeasureString(label).X * 0.8f;

            JobFilter[] filters = { JobFilter.All, JobFilter.Next, JobFilter.Available, JobFilter.Tier1, JobFilter.Tier2, JobFilter.Tier3 };
            string[] names = { "All", "Next", "Avail", "T1", "T2", "T3" };

            int buttonWidth = 56;
            int buttonHeight = 22;
            int spacing = 6;
            int startX = (int)(area.X + labelWidth + 10);
            int top = y - 2;

            for (int i = 0; i < filters.Length; i++)
            {
                Rectangle btn = new Rectangle(startX + i * (buttonWidth + spacing), top, buttonWidth, buttonHeight);
                bool hovered = btn.Contains(mouse);
                bool selected = jobFilter == filters[i];
                Color bg = selected ? new Color(90, 120, 170) : (hovered ? new Color(70, 90, 130) : new Color(50, 70, 100));
                Color border = selected ? new Color(140, 170, 220) : new Color(90, 110, 150);
                DrawPanel(spriteBatch, btn, bg, border);

                Utils.DrawBorderString(spriteBatch, names[i], new Vector2(btn.X + 8, btn.Y + 3), Color.White, 0.7f);

                if (hovered && leftClick && jobFilter != filters[i])
                {
                    jobFilter = filters[i];
                    jobScrollOffset = 0;
                }
            }

            y += buttonHeight + 8;
        }

        private List<JobType> GetFilteredJobs()
        {
            var list = new List<JobType>();
            foreach (var job in availableJobs)
            {
                if (job == rpgPlayer.CurrentJob)
                    continue;

                var jobData = Jobs.JobDatabase.GetJobData(job);
                if (jobData == null)
                    continue;

                if (!MatchesJobFilter(job, jobData))
                    continue;

                list.Add(job);
            }
            return list;
        }

        private bool MatchesJobFilter(JobType job, Jobs.JobData jobData)
        {
            return jobFilter switch
            {
                JobFilter.All => true,
                JobFilter.Next => jobData.Requirements != null && jobData.Requirements.RequiredJob == rpgPlayer.CurrentJob,
                JobFilter.Available => Jobs.JobDatabase.CanUnlockJob(rpgPlayer, job),
                JobFilter.Tier1 => jobData.Tier == JobTier.Tier1,
                JobFilter.Tier2 => jobData.Tier == JobTier.Tier2,
                JobFilter.Tier3 => jobData.Tier == JobTier.Tier3,
                _ => true
            };
        }

        private string BuildJobTooltip(Jobs.JobData jobData, bool canChange)
        {
            var lines = new List<string>
            {
                $"{jobData.DisplayName} ({GetJobTierLabel(jobData.Tier)})",
                jobData.Description
            };

            if (jobData.StatBonuses != null && jobData.StatBonuses.Count > 0)
            {
                var bonusParts = new List<string>();
                foreach (var bonus in jobData.StatBonuses)
                {
                    bonusParts.Add($"{GetStatShortName(bonus.Key)}+{bonus.Value}");
                }
                lines.Add("Bonuses: " + string.Join(", ", bonusParts));
            }

            string req = BuildRequirementsText(jobData, rpgPlayer);
            if (!string.IsNullOrEmpty(req))
                lines.Add(req);

            if (canChange)
                lines.Add("Click to select");

            return string.Join("\n", lines);
        }

        private string GetJobTierLabel(JobTier tier)
        {
            return tier switch
            {
                JobTier.Novice => "Novice",
                JobTier.Tier1 => "Tier 1",
                JobTier.Tier2 => "Tier 2",
                JobTier.Tier3 => "Tier 3",
                _ => "Tier ?"
            };
        }

        private string GetStatShortName(StatType stat)
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
                _ => stat.ToString()
            };
        }
        
        private Color GetJobTierColor(JobTier tier)
        {
            return tier switch
            {
                JobTier.Novice => Color.White,
                JobTier.Tier1 => new Color(150, 200, 255),
                JobTier.Tier2 => new Color(100, 255, 100),
                JobTier.Tier3 => new Color(255, 200, 100),
                _ => Color.White
            };
        }
        
        #endregion
        
        #region Skills Tab

        private void UpdateSkillList()
        {
            JobType currentJob = rpgPlayer.CurrentJob;
            bool jobChanged = lastSkillJob != currentJob;
            if (jobChanged || allSkills.Count == 0)
            {
                allSkills = SkillDatabase.GetSkillsForJob(currentJob);
                lastSkillJob = currentJob;
                skillScrollOffset = 0;
            }

            if (jobChanged || lastSkillFilter != skillFilter || availableSkills.Count == 0)
            {
                availableSkills = GetFilteredSkills(allSkills);
                lastSkillFilter = skillFilter;
            }
        }

        private List<BaseSkill> GetFilteredSkills(List<BaseSkill> source)
        {
            var list = new List<BaseSkill>();
            foreach (var skill in source)
            {
                if (skill == null)
                    continue;

                if (!MatchesSkillFilter(skill))
                    continue;

                list.Add(skill);
            }

            list.Sort(CompareSkills);
            return list;
        }

        private bool MatchesSkillFilter(BaseSkill skill)
        {
            JobTier tier = GetSkillTier(skill);
            return skillFilter switch
            {
                SkillFilter.All => true,
                SkillFilter.Novice => tier == JobTier.Novice,
                SkillFilter.Tier1 => tier == JobTier.Tier1,
                SkillFilter.Tier2 => tier == JobTier.Tier2,
                SkillFilter.Tier3 => tier == JobTier.Tier3,
                _ => true
            };
        }

        private JobTier GetSkillTier(BaseSkill skill)
        {
            if (skill == null)
                return JobTier.Novice;

            JobType requiredJob = skill.RequiredJob;
            if (requiredJob == JobType.None || requiredJob == JobType.Novice)
                return JobTier.Novice;

            return RpgFormulas.GetJobTier(requiredJob);
        }

        private int CompareSkills(BaseSkill left, BaseSkill right)
        {
            if (left == null && right == null)
                return 0;
            if (left == null)
                return 1;
            if (right == null)
                return -1;

            JobTier leftTier = GetSkillTier(left);
            JobTier rightTier = GetSkillTier(right);
            int tierCompare = leftTier.CompareTo(rightTier);
            if (tierCompare != 0)
                return tierCompare;

            int jobCompare = left.RequiredJob.CompareTo(right.RequiredJob);
            if (jobCompare != 0)
                return jobCompare;

            int levelCompare = left.RequiredLevel.CompareTo(right.RequiredLevel);
            if (levelCompare != 0)
                return levelCompare;

            return string.Compare(left.DisplayName, right.DisplayName, System.StringComparison.OrdinalIgnoreCase);
        }
        
        private void DrawSkillsTab(SpriteBatch spriteBatch, Rectangle area)
        {
            var serverConfig = ModContent.GetInstance<RpgServerConfig>();
            if (serverConfig != null && !serverConfig.EnableSkillSystem)
            {
                string disabledText = "Skill system is disabled on this server.";
                Vector2 textSize = FontAssets.MouseText.Value.MeasureString(disabledText);
                Vector2 textPos = new Vector2(area.X + area.Width / 2f - textSize.X / 2f, area.Y + area.Height / 2f - textSize.Y / 2f);
                Utils.DrawBorderString(spriteBatch, disabledText, textPos, Color.Red, 1.1f);
                return;
            }
            
            int y = area.Y;
            Point mouse = MousePosition;
            
            // Skill points
            string pointsText = rpgPlayer.PendingSkillPoints > 0
                ? $"Skill Points: {rpgPlayer.SkillPoints} (Pending: {rpgPlayer.PendingSkillPoints})"
                : $"Skill Points: {rpgPlayer.SkillPoints}";
            float pointsScale = 1f;
            Vector2 pointsSize = FontAssets.MouseText.Value.MeasureString(pointsText) * pointsScale;
            Utils.DrawBorderString(
                spriteBatch,
                pointsText,
                new Vector2(area.X + area.Width / 2f - pointsSize.X / 2f, y),
                Color.Gold,
                pointsScale
            );
            y += (int)pointsSize.Y + 6;

            // Macro editor / hotbar buttons
            const int buttonWidth = 120;
            const int buttonHeight = 20;
            const int buttonSpacing = 6;
            int buttonsTotalWidth = buttonWidth * 2 + buttonSpacing;
            int buttonsX = area.Right - buttonsTotalWidth;
            int buttonsY = y;

            Rectangle macroBtn = new Rectangle(buttonsX, buttonsY, buttonWidth, buttonHeight);
            bool macroHover = macroBtn.Contains(mouse);
            DrawPanel(spriteBatch, macroBtn, macroHover ? new Color(70, 90, 130) : new Color(50, 70, 100), new Color(90, 120, 170));
            string macroLabel = "Macro Editor";
            Vector2 macroSize = FontAssets.MouseText.Value.MeasureString(macroLabel) * 0.8f;
            Vector2 macroPos = new Vector2(macroBtn.X + macroBtn.Width / 2f - macroSize.X / 2f, macroBtn.Y + macroBtn.Height / 2f - macroSize.Y / 2f);
            Utils.DrawBorderString(spriteBatch, macroLabel, macroPos, Color.White, 0.8f);
            if (macroHover && leftClick)
            {
                ModContent.GetInstance<MacroUISystem>()?.ToggleUI();
                SoundEngine.PlaySound(SoundID.MenuTick);
            }

            // Clear hotbar button
            Rectangle clearBtn = new Rectangle(buttonsX + buttonWidth + buttonSpacing, buttonsY, buttonWidth, buttonHeight);
            bool clearHover = clearBtn.Contains(mouse);
            DrawPanel(spriteBatch, clearBtn, clearHover ? new Color(70, 90, 130) : new Color(50, 70, 100), new Color(90, 120, 170));
            string clearLabel = "Clear Hotbar";
            Vector2 clearSize = FontAssets.MouseText.Value.MeasureString(clearLabel) * 0.8f;
            Vector2 clearPos = new Vector2(clearBtn.X + clearBtn.Width / 2f - clearSize.X / 2f, clearBtn.Y + clearBtn.Height / 2f - clearSize.Y / 2f);
            Utils.DrawBorderString(spriteBatch, clearLabel, clearPos, Color.White, 0.8f);
            if (clearHover && leftClick)
            {
                skillManager.ClearHotbar();
                SoundEngine.PlaySound(SoundID.MenuTick);
            }

            y += buttonHeight + 8;

            string instr = "Drag skill icon to slots or macro. Use the M button to add to a macro.";
            Utils.DrawBorderString(spriteBatch, instr, new Vector2(area.X + 10, y), Color.Gray, 0.8f);
            y += 22;

            y = DrawSkillFilters(spriteBatch, area, y);
            
            // Skill cards
            int cardHeight = 90;
            int visibleCards = (area.Y + area.Height - y) / (cardHeight + 5);

            if (visibleCards < 1)
                visibleCards = 1;

            int maxOffset = System.Math.Max(0, availableSkills.Count - visibleCards);
            skillScrollOffset = Utils.Clamp(skillScrollOffset, 0, maxOffset);

            Rectangle listArea = new Rectangle(area.X, y, area.Width, area.Y + area.Height - y);
            DrawSkillScrollBar(spriteBatch, listArea, availableSkills.Count, visibleCards, skillScrollOffset);
            
            for (int i = skillScrollOffset; i < availableSkills.Count && i < skillScrollOffset + visibleCards; i++)
            {
                var skill = availableSkills[i];
                DrawSkillCard(spriteBatch, new Rectangle(area.X, y, area.Width, cardHeight), skill);
                y += cardHeight + 5;
            }
            
            if (availableSkills.Count == 0)
            {
                Utils.DrawBorderString(spriteBatch, "No skills available for this filter.", new Vector2(area.X, y), Color.Gray);
            }
        }

        private int DrawSkillFilters(SpriteBatch spriteBatch, Rectangle area, int y)
        {
            Point mouse = MousePosition;
            string label = "Filter:";
            Utils.DrawBorderString(spriteBatch, label, new Vector2(area.X, y), Color.White, 0.8f);
            float labelWidth = FontAssets.MouseText.Value.MeasureString(label).X * 0.8f;

            SkillFilter[] filters = { SkillFilter.All, SkillFilter.Novice, SkillFilter.Tier1, SkillFilter.Tier2, SkillFilter.Tier3 };
            string[] names = { "All", "Nov", "T1", "T2", "T3" };

            int buttonWidth = 52;
            int buttonHeight = 22;
            int spacing = 6;
            int startX = (int)(area.X + labelWidth + 10);
            int top = y - 2;

            for (int i = 0; i < filters.Length; i++)
            {
                Rectangle btn = new Rectangle(startX + i * (buttonWidth + spacing), top, buttonWidth, buttonHeight);
                bool hovered = btn.Contains(mouse);
                bool selected = skillFilter == filters[i];
                Color bg = selected ? new Color(90, 120, 170) : (hovered ? new Color(70, 90, 130) : new Color(50, 70, 100));
                Color border = selected ? new Color(140, 170, 220) : new Color(90, 110, 150);
                DrawPanel(spriteBatch, btn, bg, border);

                Utils.DrawBorderString(spriteBatch, names[i], new Vector2(btn.X + 8, btn.Y + 3), Color.White, 0.7f);

                if (hovered && leftClick && skillFilter != filters[i])
                {
                    skillFilter = filters[i];
                    skillScrollOffset = 0;
                }
            }

            return y + buttonHeight + 8;
        }

        private void DrawSkillScrollBar(SpriteBatch spriteBatch, Rectangle area, int totalItems, int visibleItems, int offset)
        {
            if (totalItems <= visibleItems || visibleItems <= 0)
                return;

            int trackWidth = 10;
            int trackX = area.Right - trackWidth - 2;
            int trackY = area.Y;
            int trackHeight = area.Height;

            Rectangle track = new Rectangle(trackX, trackY, trackWidth, trackHeight);
            DrawPanel(spriteBatch, track, new Color(30, 40, 60), new Color(70, 90, 120));

            float ratio = visibleItems / (float)totalItems;
            int thumbHeight = (int)System.Math.Max(16, ratio * trackHeight);
            int maxOffset = totalItems - visibleItems;
            float scrollRatio = maxOffset > 0 ? offset / (float)maxOffset : 0f;
            int thumbY = trackY + (int)((trackHeight - thumbHeight) * scrollRatio);

            Rectangle thumb = new Rectangle(trackX + 1, thumbY + 1, trackWidth - 2, thumbHeight - 2);
            DrawPanel(spriteBatch, thumb, new Color(120, 150, 200), new Color(160, 190, 230));
        }
        
        private void DrawSkillCard(SpriteBatch spriteBatch, Rectangle rect, BaseSkill skill)
        {
            Point mouse = MousePosition;
            bool isLearned = skillManager.LearnedSkills.ContainsKey(skill.InternalName);
            bool hover = rect.Contains(mouse);
            bool canLearn = isLearned
                ? skillManager.LearnedSkills[skill.InternalName].CanLearn(Main.LocalPlayer)
                : skill.CanLearn(Main.LocalPlayer);
            
            Color bg = isLearned ? new Color(40, 60, 40) : (canLearn ? (hover ? new Color(50, 60, 80) : new Color(35, 45, 65)) : new Color(40, 40, 50));
            Color border = isLearned ? new Color(100, 180, 100) : (canLearn ? new Color(80, 100, 140) : new Color(60, 60, 70));
            
            DrawPanel(spriteBatch, rect, bg, border);

            Rectangle iconRect = new Rectangle(rect.X + 8, rect.Y + 8, 32, 32);
            DrawPanel(spriteBatch, iconRect, new Color(30, 40, 60), new Color(70, 90, 120));
            var icon = AssetLoader.GetTexture(skill.IconTexture);
            spriteBatch.Draw(icon, iconRect, isLearned ? Color.White : Color.Gray);

            if (isLearned && iconRect.Contains(mouse))
            {
                Main.hoverItemName = "Drag to slot or macro";
                if (leftClick)
                {
                    SkillDragDropSystem.StartDrag(skill.InternalName);
                }
            }
            
            // Skill name
            string name = skill.DisplayName;
            if (isLearned)
            {
                var learned = skillManager.LearnedSkills[skill.InternalName];
                name += $" (Rank {learned.CurrentRank}/{learned.MaxRank})";
            }
            int contentX = rect.X + 50;
            Utils.DrawBorderString(spriteBatch, name, new Vector2(contentX, rect.Y + 8), isLearned ? Color.LightGreen : Color.White);
            
            // Type
            string type = skill.SkillType.ToString();
            Utils.DrawBorderString(spriteBatch, type, new Vector2(rect.X + rect.Width - 80, rect.Y + 8), Color.Gray, 0.75f);
            
            // Description
            string desc = skill.Description.Length > 70 ? skill.Description.Substring(0, 67) + "..." : skill.Description;
            Utils.DrawBorderString(spriteBatch, desc, new Vector2(contentX, rect.Y + 30), Color.LightGray, 0.65f);

            string reqJob = skill.RequiredJob == JobType.None ? "Any" : skill.RequiredJob.ToString();
            string reqText = $"Req: {reqJob} Lv {skill.RequiredLevel}";
            Utils.DrawBorderString(spriteBatch, reqText, new Vector2(contentX, rect.Y + 46), Color.Gray, 0.6f);
            
            // Learn/Upgrade button
            bool showLearnButton = canLearn && rpgPlayer.SkillPoints >= skill.SkillPointCost;
            if (showLearnButton)
            {
                Rectangle learnBtn = new Rectangle(rect.X + rect.Width - 70, rect.Y + rect.Height - 25, 60, 20);
                DrawPanel(spriteBatch, learnBtn, hover ? new Color(80, 120, 180) : new Color(50, 90, 150), new Color(100, 150, 220));
                
                string btnText = isLearned ? "Upgrade" : "Learn";
                Utils.DrawBorderString(spriteBatch, btnText, new Vector2(learnBtn.X + 5, learnBtn.Y + 2), Color.White, 0.7f);
                
                if (hover && learnBtn.Contains(mouse) && leftClick)
                {
                    skillManager.LearnSkill(skill);
                    SoundEngine.PlaySound(SoundID.Item4);
                }
            }

            bool showSlots = isLearned && skill.SkillType != SkillType.Passive;
            if (showSlots)
            {
                DrawSkillSlotsRow(spriteBatch, rect, skill, showLearnButton, contentX);
            }
            else
            {
                // Cost
                string cost = $"Cost: {skill.SkillPointCost} SP";
                Utils.DrawBorderString(spriteBatch, cost, new Vector2(contentX, rect.Y + rect.Height - 18), rpgPlayer.SkillPoints >= skill.SkillPointCost ? Color.Yellow : Color.Red, 0.65f);
            }
        }

        private void DrawSkillSlotsRow(SpriteBatch spriteBatch, Rectangle rect, BaseSkill skill, bool showLearnButton, int contentX)
        {
            Point mouse = MousePosition;
            string slotLabel = "Slots:";
            float labelScale = 0.6f;
            Vector2 labelSize = FontAssets.MouseText.Value.MeasureString(slotLabel) * labelScale;

            int buttonSize = 14;
            int spacing = 2;
            float rowY = rect.Bottom - 20f;

            Vector2 labelPos = new Vector2(contentX, rowY - 2f);
            Utils.DrawBorderString(spriteBatch, slotLabel, labelPos, Color.LightGray, labelScale);

            int startX = (int)(contentX + labelSize.X + 6f);
            int slotRowWidth = (buttonSize * 9) + (spacing * 8);
            int reservedRight = showLearnButton ? 80 : 12;
            int maxRight = rect.Right - reservedRight;

            for (int i = 0; i < 9; i++)
            {
                int x = startX + i * (buttonSize + spacing);
                Rectangle slotRect = new Rectangle(x, (int)rowY - 2, buttonSize, buttonSize);
                if (slotRect.Right > maxRight)
                    break;

                bool assigned = skillManager.SkillHotbar[i] == skill.InternalName;
                Color bg = assigned ? new Color(220, 200, 80) : new Color(60, 70, 90);
                Color border = assigned ? new Color(255, 230, 120) : new Color(90, 110, 140);
                DrawPanel(spriteBatch, slotRect, bg, border);

                string label = (i + 1).ToString();
                Vector2 numSize = FontAssets.MouseText.Value.MeasureString(label) * 0.55f;
                Vector2 numPos = new Vector2(
                    slotRect.X + slotRect.Width / 2 - numSize.X / 2,
                    slotRect.Y + slotRect.Height / 2 - numSize.Y / 2
                );
                Utils.DrawBorderString(spriteBatch, label, numPos, Color.White, 0.55f);

                if (slotRect.Contains(mouse))
                {
                    Main.hoverItemName = $"Assign to slot {i + 1}";
                    if (leftClick)
                    {
                        skillManager.AssignSkillToSlot(skill.InternalName, i);
                        SoundEngine.PlaySound(SoundID.MenuTick);
                    }
                }
            }

            int macroX = startX + slotRowWidth + 6;
            int macroSize = 16;
            if (macroX + macroSize <= maxRight)
            {
                Rectangle macroRect = new Rectangle(macroX, (int)rowY - 2, macroSize, macroSize);
                bool macroHover = macroRect.Contains(mouse);
                DrawPanel(spriteBatch, macroRect, macroHover ? new Color(80, 60, 120) : new Color(60, 50, 90), new Color(110, 90, 160));
                Utils.DrawBorderString(spriteBatch, "M", new Vector2(macroRect.X + 4, macroRect.Y + 1), Color.Cyan, 0.6f);

                if (macroHover)
                {
                    Main.hoverItemName = "Add to macro";
                    if (leftClick)
                    {
                        MacroEditorUI.PendingSkillToAdd = skill.InternalName;
                        var macroUISystem = ModContent.GetInstance<MacroUISystem>();
                        if (macroUISystem != null && !macroUISystem.IsUIOpen)
                        {
                            macroUISystem.ToggleUI();
                        }
                        SoundEngine.PlaySound(SoundID.MenuTick);
                    }
                }
            }
        }
        
        #endregion
        
        #region Achievements Tab
        
        private int achievementScrollOffset = 0;
        
        private void DrawAchievementsTab(SpriteBatch spriteBatch, Rectangle area)
        {
            // AchievementSystem is a ModPlayer, get from local player
            var achievementPlayer = Main.LocalPlayer.GetModPlayer<AchievementSystem>();
            if (achievementPlayer == null)
            {
                Utils.DrawBorderString(spriteBatch, "Achievement system not loaded.", new Vector2(area.X, area.Y), Color.Gray);
                return;
            }
            
            int y = area.Y;
            
            // Progress header - count unlocked achievements
            var achievements = AchievementSystem.Achievements;
            var unlockedSet = achievementPlayer.UnlockedAchievements;
            int unlocked = unlockedSet.Count;
            int total = achievements.Count;
            
            string progressText = $"Achievements: {unlocked}/{total}";
            Utils.DrawBorderString(spriteBatch, progressText, new Vector2(area.X + area.Width / 2 - FontAssets.MouseText.Value.MeasureString(progressText).X / 2, y), Color.Gold);
            y += 30;
            
            // Achievement list
            int cardHeight = 60;
            int visibleCards = (area.Y + area.Height - y) / (cardHeight + 5);
            
            // Convert to list for indexed access
            var achievementList = new List<KeyValuePair<string, AchievementData>>(achievements);
            
            // Handle scroll
            if (area.Contains(MousePosition))
            {
                int scroll = Terraria.GameInput.PlayerInput.ScrollWheelDelta;
                achievementScrollOffset -= scroll / 30;
                achievementScrollOffset = System.Math.Max(0, System.Math.Min(achievementScrollOffset, System.Math.Max(0, achievementList.Count - visibleCards)));
            }
            
            for (int i = achievementScrollOffset; i < achievementList.Count && i < achievementScrollOffset + visibleCards; i++)
            {
                var kvp = achievementList[i];
                bool isUnlocked = unlockedSet.Contains(kvp.Key);
                DrawAchievementCard(spriteBatch, new Rectangle(area.X, y, area.Width, cardHeight), kvp.Value, isUnlocked);
                y += cardHeight + 5;
            }
            
            if (achievementList.Count == 0)
            {
                Utils.DrawBorderString(spriteBatch, "No achievements available.", new Vector2(area.X, y), Color.Gray);
            }
        }
        
        private void DrawAchievementCard(SpriteBatch spriteBatch, Rectangle rect, AchievementData ach, bool isUnlocked)
        {
            Color bg = isUnlocked ? new Color(40, 60, 40) : new Color(35, 40, 50);
            Color border = isUnlocked ? new Color(100, 180, 100) : new Color(60, 70, 90);
            
            DrawPanel(spriteBatch, rect, bg, border);
            
            // Name
            Utils.DrawBorderString(spriteBatch, ach.Name, new Vector2(rect.X + 10, rect.Y + 6), isUnlocked ? Color.LightGreen : Color.White);
            
            // Status and reward
            string status = isUnlocked ? "✓ Complete" : $"+{ach.XpReward} XP";
            Color statusColor = isUnlocked ? Color.LightGreen : Color.Yellow;
            Utils.DrawBorderString(spriteBatch, status, new Vector2(rect.X + rect.Width - 90, rect.Y + 6), statusColor, 0.8f);
            
            // Description
            string desc = ach.Description.Length > 70 ? ach.Description.Substring(0, 67) + "..." : ach.Description;
            Utils.DrawBorderString(spriteBatch, desc, new Vector2(rect.X + 10, rect.Y + 28), Color.LightGray, 0.65f);
        }
        
        #endregion
        
        #region Damage Tab
        
        private void DrawDamageTab(SpriteBatch spriteBatch, Rectangle area)
        {
            int y = area.Y + 10;
            
            Utils.DrawBorderString(spriteBatch, "Damage Class Inheritance Map", new Vector2(area.X + area.Width / 2 - FontAssets.MouseText.Value.MeasureString("Damage Class Inheritance Map").X / 2, y), Color.Gold, 1.2f);
            y += 40;
            
            // Get all damage classes
            var damageClasses = new List<DamageClass>
            {
                DamageClass.Generic,
                DamageClass.Melee,
                DamageClass.Ranged,
                DamageClass.Magic,
                DamageClass.Summon,
                DamageClass.Throwing
            };
            
            foreach (var dc in damageClasses)
            {
                string baseName = dc == DamageClass.Generic ? "None" : "Generic";
                string text = $"{dc.DisplayName} -> {baseName}";
                Utils.DrawBorderString(spriteBatch, text, new Vector2(area.X + 10, y), Color.White, 0.9f);
                y += 20;
            }
        }
        
        #endregion
        
        #region Helpers

        private void DrawStatTooltip(SpriteBatch spriteBatch, StatType stat, int baseVal, int autoVal, int bonusVal)
        {
            Point mouse = MousePosition;
            List<string> lines = GetStatTooltipLines(stat, baseVal, autoVal, bonusVal);
            if (lines.Count == 0)
                return;

            int width = 320;
            int height = lines.Count * 18 + 10;

            Rectangle tooltipBounds = new Rectangle(
                mouse.X + 20,
                mouse.Y - height / 2,
                width,
                height
            );

            Vector2 topLeft = ScreenTopLeftUi;
            int screenRight = (int)(topLeft.X + ScreenWidthUi);
            int screenBottom = (int)(topLeft.Y + ScreenHeightUi);

            if (tooltipBounds.Right > screenRight)
                tooltipBounds.X = mouse.X - width - 20;
            if (tooltipBounds.Bottom > screenBottom)
                tooltipBounds.Y = screenBottom - height;
            if (tooltipBounds.Y < topLeft.Y)
                tooltipBounds.Y = (int)topLeft.Y;

            DrawPanel(spriteBatch, tooltipBounds, new Color(10, 15, 30, 240), new Color(60, 80, 120));

            int y = tooltipBounds.Y + 5;
            foreach (string line in lines)
            {
                Utils.DrawBorderStringFourWay(
                    spriteBatch,
                    FontAssets.MouseText.Value,
                    line,
                    tooltipBounds.X + 6,
                    y,
                    Color.White,
                    Color.Black,
                    Vector2.Zero,
                    0.75f
                );
                y += 18;
            }
        }

        private List<string> GetStatTooltipLines(StatType stat, int baseVal, int autoVal, int bonusVal)
        {
            var lines = new List<string>
            {
                $"{stat}  (Base {baseVal} / Auto {autoVal} / Bonus {bonusVal})"
            };

            lines.AddRange(StatDefinitions.GetDefinition(stat).EffectLines);

            return lines;
        }
        
        /// <summary>
        /// Build detailed requirements text for job advancement
        /// </summary>
        private string BuildRequirementsText(Jobs.JobData jobData, RpgPlayer player)
        {
            var missing = Jobs.JobDatabase.GetMissingRequirementDescriptions(player, jobData.Type);
            return missing.Count > 0 ? "Req: " + string.Join(", ", missing) : "Requirements met!";
        }

        private string BuildStatDetail(int baseVal, int autoVal, int bonus)
        {
            if (autoVal == 0 && bonus == 0)
                return $"(B{baseVal})";

            return $"(B{baseVal} A{autoVal} +{bonus})";
        }

        private Point GetScaledMouse()
        {
            return UiInput.GetUiMousePoint();
        }

        private Rectangle BuildMenuBounds(float anchorX, float anchorY)
        {
            Vector2 topLeft = ScreenTopLeftUi;
            int x = (int)(topLeft.X + ScreenWidthUi * anchorX - PANEL_WIDTH / 2);
            int y = (int)(topLeft.Y + ScreenHeightUi * anchorY - PANEL_HEIGHT / 2);

            x = Utils.Clamp(x, (int)topLeft.X, (int)(topLeft.X + ScreenWidthUi - PANEL_WIDTH));
            y = Utils.Clamp(y, (int)topLeft.Y, (int)(topLeft.Y + ScreenHeightUi - PANEL_HEIGHT));

            return new Rectangle(x, y, PANEL_WIDTH, PANEL_HEIGHT);
        }

        private void SaveMenuAnchor()
        {
            var config = ModContent.GetInstance<RpgClientConfig>();
            if (config == null || ScreenWidthUi <= 0 || ScreenHeightUi <= 0)
                return;

            Vector2 topLeft = ScreenTopLeftUi;
            config.RpgMenuPosX = MathHelper.Clamp((bounds.Center.X - topLeft.X) / ScreenWidthUi, 0f, 1f);
            config.RpgMenuPosY = MathHelper.Clamp((bounds.Center.Y - topLeft.Y) / ScreenHeightUi, 0f, 1f);
            config.SaveChanges(config, null, true, true);
        }
        
        private void DrawPanel(SpriteBatch spriteBatch, Rectangle rect, Color bg, Color border)
        {
            // Background
            spriteBatch.Draw(TextureAssets.MagicPixel.Value, rect, bg);
            
            // Border
            spriteBatch.Draw(TextureAssets.MagicPixel.Value, new Rectangle(rect.X, rect.Y, rect.Width, 2), border);
            spriteBatch.Draw(TextureAssets.MagicPixel.Value, new Rectangle(rect.X, rect.Y + rect.Height - 2, rect.Width, 2), border);
            spriteBatch.Draw(TextureAssets.MagicPixel.Value, new Rectangle(rect.X, rect.Y, 2, rect.Height), border);
            spriteBatch.Draw(TextureAssets.MagicPixel.Value, new Rectangle(rect.X + rect.Width - 2, rect.Y, 2, rect.Height), border);
        }
        
        private void DrawPanel(SpriteBatch spriteBatch, Rectangle rect, Color bg)
        {
            DrawPanel(spriteBatch, rect, bg, new Color(60, 70, 90));
        }
        
        #endregion
    }
}
