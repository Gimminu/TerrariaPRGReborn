using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.Audio;
using Terraria.ModLoader;
using Terraria.UI;
using Terraria.ID;
using Terraria.GameInput;
using System.Collections.Generic;
using System.Linq;
using Rpg.Common.Players;

namespace Rpg.Common.UI
{
    /// <summary>
    /// Job advancement UI - allows players to change jobs when requirements are met
    /// </summary>
    public class JobAdvancementUI : UIState
    {
        private RpgPlayer rpgPlayer;
        private Rectangle bounds;
        
        private const int PANEL_WIDTH = 600;
        private const int PANEL_HEIGHT = 500;
        private const int JOB_CARD_WIDTH = 270;
        private const int JOB_CARD_HEIGHT = 120;
        private const int CARD_SPACING = 15;
        
        private List<JobType> availableJobs;
        private int scrollOffset = 0;

        // UI scale helpers (work in UI space so hitboxes and drawing match)
        private Point MousePosition => GetScaledMouse();
        private Vector2 ScreenTopLeftUi => UiInput.ScreenTopLeftUi;
        private Vector2 ScreenBottomRightUi => UiInput.ScreenBottomRightUi;
        private int ScreenWidthUi => UiInput.ScreenWidthUi;
        private int ScreenHeightUi => UiInput.ScreenHeightUi;
        
        public override void OnInitialize()
        {
            Vector2 topLeft = ScreenTopLeftUi;
            bounds = new Rectangle(
                (int)(topLeft.X + ScreenWidthUi / 2 - PANEL_WIDTH / 2),
                (int)(topLeft.Y + ScreenHeightUi / 2 - PANEL_HEIGHT / 2),
                PANEL_WIDTH,
                PANEL_HEIGHT
            );
        }
        
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            
            rpgPlayer = Main.LocalPlayer.GetModPlayer<RpgPlayer>();
            
            // Refresh available jobs
            availableJobs = Jobs.JobDatabase.GetAllJobs();
            
            // Close with ESC
            if (Main.keyState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Escape))
            {
                var uiSystem = ModContent.GetInstance<JobUISystem>();
                uiSystem.HideUI();
            }
            
            // Mouse scroll
            if (bounds.Contains(MousePosition))
            {
                scrollOffset -= PlayerInput.ScrollWheelDelta / 60;
                scrollOffset = System.Math.Max(0, scrollOffset);
            }
        }
        
        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            // Safety check
            if (rpgPlayer == null)
                return;
                
            // Draw panel background
            DrawPanel(spriteBatch, bounds, new Color(20, 25, 45, 240), new Color(60, 70, 90));
            
            // Draw header
            Rectangle headerBounds = new Rectangle(bounds.X, bounds.Y, bounds.Width, 50);
            DrawPanel(spriteBatch, headerBounds, new Color(30, 35, 60, 255), new Color(80, 90, 120));
            
            string title = "Job Advancement";
            Vector2 titleSize = FontAssets.MouseText.Value.MeasureString(title);
            Vector2 titlePos = new Vector2(
                bounds.X + bounds.Width / 2 - titleSize.X / 2,
                bounds.Y + 15
            );
            DrawText(spriteBatch, title, titlePos, Color.Gold, 1.2f);
            
            // Current job display
            int yOffset = bounds.Y + 60;
            DrawCurrentJob(spriteBatch, ref yOffset);
            
            yOffset += 10;
            
            // Available jobs
            string availText = "Jobs:";
            Vector2 availPos = new Vector2(bounds.X + 20, yOffset);
            DrawText(spriteBatch, availText, availPos, Color.LightGray, 0.9f);
            yOffset += 25;
            
            // Draw job cards (2 columns)
            DrawJobCards(spriteBatch, yOffset);
            
            // Instructions
            DrawInstructions(spriteBatch);
        }
        
        private void DrawCurrentJob(SpriteBatch spriteBatch, ref int yOffset)
        {
            if (rpgPlayer == null)
                return;
                
            var currentJobData = Jobs.JobDatabase.GetJobData(rpgPlayer.CurrentJob);
            if (currentJobData == null)
                return;
            
            Rectangle cardBounds = new Rectangle(
                bounds.X + 20,
                yOffset,
                bounds.Width - 40,
                80
            );
            
            DrawPanel(spriteBatch, cardBounds, new Color(40, 50, 80, 220), GetJobTierColor(currentJobData.Tier));
            
            // Job name
            string jobName = $"Current: {currentJobData.DisplayName}";
            Vector2 namePos = new Vector2(cardBounds.X + 10, cardBounds.Y + 10);
            DrawText(spriteBatch, jobName, namePos, GetJobTierColor(currentJobData.Tier), 1.0f);
            
            // Description
            string desc = currentJobData.Description;
            Vector2 descPos = new Vector2(cardBounds.X + 10, cardBounds.Y + 35);
            DrawWrappedText(spriteBatch, desc, descPos, cardBounds.Width - 20, Color.LightGray, 0.75f);
            
            yOffset += 90;
        }
        
        private void DrawJobCards(SpriteBatch spriteBatch, int startY)
        {
            int xOffset = bounds.X + 20;
            int yOffset = startY - scrollOffset;
            int column = 0;
            Point mouse = MousePosition;

            int totalCards = availableJobs.Count - 1;
            if (totalCards < 0)
                totalCards = 0;
            int totalRows = (totalCards + 1) / 2;
            int visibleHeight = bounds.Bottom - startY;
            int totalHeight = totalRows * (JOB_CARD_HEIGHT + CARD_SPACING);
            int maxScroll = System.Math.Max(0, totalHeight - visibleHeight);
            if (scrollOffset > maxScroll)
                scrollOffset = maxScroll;
            
            foreach (JobType job in availableJobs)
            {
                if (job == rpgPlayer.CurrentJob)
                    continue; // Skip current job
                
                var jobData = Jobs.JobDatabase.GetJobData(job);
                bool canUnlock = Jobs.JobDatabase.CanUnlockJob(rpgPlayer, job);
                
                Rectangle cardBounds = new Rectangle(
                    xOffset + (column * (JOB_CARD_WIDTH + CARD_SPACING)),
                    yOffset,
                    JOB_CARD_WIDTH,
                    JOB_CARD_HEIGHT
                );
                
                // Skip if off-screen
                if (cardBounds.Bottom < bounds.Y + 50 || cardBounds.Y > bounds.Bottom)
                {
                    column = (column + 1) % 2;
                    if (column == 0)
                        yOffset += JOB_CARD_HEIGHT + CARD_SPACING;
                    continue;
                }
                
                // Check if hovered
                bool hovered = cardBounds.Contains(mouse) && canUnlock;
                
                // Background color
                Color bgColor = canUnlock 
                    ? (hovered ? new Color(60, 80, 120, 240) : new Color(40, 50, 80, 220))
                    : new Color(30, 30, 30, 180);
                
                Color borderColor = canUnlock ? GetJobTierColor(jobData.Tier) : Color.DarkGray;
                
                DrawPanel(spriteBatch, cardBounds, bgColor, borderColor);
                
                // Job name
                Color nameColor = canUnlock ? GetJobTierColor(jobData.Tier) : Color.Gray;
                Vector2 namePos = new Vector2(cardBounds.X + 8, cardBounds.Y + 8);
                DrawText(spriteBatch, jobData.DisplayName, namePos, nameColor, 0.9f);
                
                // Tier
                string tierText = $"[{GetJobTierLabel(jobData.Tier)}]";
                Vector2 tierSize = FontAssets.MouseText.Value.MeasureString(tierText);
                Vector2 tierPos = new Vector2(cardBounds.Right - tierSize.X * 0.7f - 8, cardBounds.Y + 8);
                DrawText(spriteBatch, tierText, tierPos, nameColor, 0.7f);
                
                // Description
                Vector2 descPos = new Vector2(cardBounds.X + 8, cardBounds.Y + 30);
                DrawWrappedText(spriteBatch, jobData.Description, descPos, cardBounds.Width - 16, Color.LightGray, 0.65f);
                
                // Requirements (if locked)
                if (!canUnlock)
                {
                    var missingReqs = Jobs.JobDatabase.GetMissingRequirementDescriptions(rpgPlayer, job);
                    if (missingReqs.Count == 0)
                    {
                        missingReqs.Add("Requirements not met");
                    }

                    float reqScale = 0.6f;
                    float lineHeight = FontAssets.MouseText.Value.LineSpacing * reqScale;
                    float reqStartY = cardBounds.Bottom - (missingReqs.Count * lineHeight) - 6f;
                    Vector2 reqPos = new Vector2(cardBounds.X + 8, reqStartY);

                    for (int i = 0; i < missingReqs.Count; i++)
                    {
                        DrawText(
                            spriteBatch,
                            missingReqs[i],
                            new Vector2(reqPos.X, reqPos.Y + (i * lineHeight)),
                            Color.IndianRed,
                            reqScale
                        );
                    }
                }
                else
                {
                    // Click to change
                    string clickText = "Click to change job";
                    Vector2 clickSize = FontAssets.MouseText.Value.MeasureString(clickText);
                    Vector2 clickPos = new Vector2(
                        cardBounds.X + cardBounds.Width / 2 - clickSize.X * 0.6f / 2,
                        cardBounds.Bottom - 20
                    );
                    Color clickColor = hovered ? Color.Yellow : Color.LightGreen;
                    DrawText(spriteBatch, clickText, clickPos, clickColor, 0.6f);
                }
                
                // Handle click
                if (hovered && Main.mouseLeft && Main.mouseLeftRelease)
                {
                    ChangeJob(job);
                }
                
                // Next column
                column = (column + 1) % 2;
                if (column == 0)
                    yOffset += JOB_CARD_HEIGHT + CARD_SPACING;
            }
        }
        
        private void ChangeJob(JobType newJob)
        {
            var jobData = Jobs.JobDatabase.GetJobData(newJob);
            
            var levelSystem = Main.LocalPlayer.GetModPlayer<Players.PlayerLevel>();
            if (levelSystem == null || !levelSystem.AdvanceJob(newJob))
            {
                Main.NewText("Cannot change job yet.", Color.Red);
                return;
            }

            // Show bonus summary
            Main.NewText($"Gained stat bonuses: {GetBonusText(jobData)}", Color.LightGreen);
            
            // Close UI
            var uiSystem = ModContent.GetInstance<JobUISystem>();
            uiSystem.HideUI();
        }
        
        private string GetBonusText(Jobs.JobData jobData)
        {
            if (jobData.StatBonuses.Count == 0)
                return "None";
            
            var bonuses = jobData.StatBonuses
                .OrderByDescending(kv => kv.Value)
                .Take(3)
                .Select(kv => $"+{kv.Value} {GetStatShortName(kv.Key)}");
            
            return string.Join(", ", bonuses);
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
                _ => "???"
            };
        }
        
        private void DrawInstructions(SpriteBatch spriteBatch)
        {
            string[] instructions = {
                "ESC: Close",
                "Mouse Wheel: Scroll"
            };
            
            int yOffset = bounds.Bottom - 30;
            foreach (string instruction in instructions)
            {
                Vector2 instrSize = FontAssets.MouseText.Value.MeasureString(instruction);
                Vector2 instrPos = new Vector2(
                    bounds.X + bounds.Width / 2 - instrSize.X * 0.7f / 2,
                    yOffset
                );
                DrawText(spriteBatch, instruction, instrPos, Color.Gray, 0.7f);
                yOffset -= 15;
            }
        }
        
        private Color GetJobTierColor(JobTier tier)
        {
            return tier switch
            {
                JobTier.Novice => Color.LightGray,
                JobTier.Tier1 => new Color(100, 255, 100),
                JobTier.Tier2 => new Color(100, 150, 255),
                JobTier.Tier3 => new Color(200, 100, 255),
                _ => Color.White
            };
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

        private Point GetScaledMouse()
        {
            return UiInput.GetUiMousePoint();
        }
        
        private void DrawPanel(SpriteBatch spriteBatch, Rectangle bounds, Color bgColor, Color borderColor)
        {
            Texture2D pixel = TextureAssets.MagicPixel.Value;
            spriteBatch.Draw(pixel, bounds, bgColor);
            
            // Border
            spriteBatch.Draw(pixel, new Rectangle(bounds.X - 2, bounds.Y - 2, bounds.Width + 4, 2), borderColor);
            spriteBatch.Draw(pixel, new Rectangle(bounds.X - 2, bounds.Bottom, bounds.Width + 4, 2), borderColor);
            spriteBatch.Draw(pixel, new Rectangle(bounds.X - 2, bounds.Y, 2, bounds.Height), borderColor);
            spriteBatch.Draw(pixel, new Rectangle(bounds.Right, bounds.Y, 2, bounds.Height), borderColor);
        }

        private void DrawText(SpriteBatch spriteBatch, string text, Vector2 position, Color color, float scale)
        {
            Utils.DrawBorderStringFourWay(spriteBatch, FontAssets.MouseText.Value, text, position.X, position.Y, color, Color.Black, Vector2.Zero, scale);
        }
        
        private void DrawWrappedText(SpriteBatch spriteBatch, string text, Vector2 position, int maxWidth, Color color, float scale)
        {
            string[] words = text.Split(' ');
            string currentLine = "";
            float yOffset = 0;
            
            foreach (string word in words)
            {
                string testLine = currentLine + (currentLine.Length > 0 ? " " : "") + word;
                Vector2 testSize = FontAssets.MouseText.Value.MeasureString(testLine);
                
                if (testSize.X * scale > maxWidth && currentLine.Length > 0)
                {
                    DrawText(spriteBatch, currentLine, position + new Vector2(0, yOffset), color, scale);
                    yOffset += testSize.Y * scale;
                    currentLine = word;
                }
                else
                {
                    currentLine = testLine;
                }
            }
            
            if (currentLine.Length > 0)
            {
                DrawText(spriteBatch, currentLine, position + new Vector2(0, yOffset), color, scale);
            }
        }
    }
}
