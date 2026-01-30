using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.UI.Elements;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI;
using Rpg.Common.Base;
using Rpg.Common.Jobs;
using Rpg.Common.Systems;

namespace Rpg.Common.UI
{
    /// <summary>
    /// UI for learning and upgrading skills
    /// </summary>
    public class SkillLearningUI : UIState
    {
        private UIPanel mainPanel;
        private UIText titleText;
        private UIScrollbar scrollbar;
        private UIList skillList;
        
        private const float PANEL_WIDTH = 600f;
        private const float PANEL_HEIGHT = 500f;
        private const float SKILL_CARD_HEIGHT = 100f;
        
        public override void OnInitialize()
        {
            // Main panel
            mainPanel = new UIPanel();
            mainPanel.Width.Set(PANEL_WIDTH, 0f);
            mainPanel.Height.Set(PANEL_HEIGHT, 0f);
            mainPanel.HAlign = 0.5f;
            mainPanel.VAlign = 0.5f;
            mainPanel.BackgroundColor = new Color(33, 43, 79) * 0.95f;
            mainPanel.BorderColor = new Color(89, 116, 213);
            Append(mainPanel);
            
            // Title
            titleText = new UIText("Skill Learning", 1.2f);
            titleText.HAlign = 0.5f;
            titleText.Top.Set(15, 0f);
            mainPanel.Append(titleText);
            
            // Skill list
            skillList = new UIList();
            skillList.Width.Set(-30, 1f);
            skillList.Height.Set(-80, 1f);
            skillList.Top.Set(60, 0f);
            skillList.Left.Set(10, 0f);
            skillList.ListPadding = 5f;
            mainPanel.Append(skillList);
            
            // Scrollbar
            scrollbar = new UIScrollbar();
            scrollbar.Width.Set(20, 0f);
            scrollbar.Height.Set(-80, 1f);
            scrollbar.Top.Set(60, 0f);
            scrollbar.HAlign = 1f;
            scrollbar.Left.Set(-10, 0f);
            mainPanel.Append(scrollbar);
            
            skillList.SetScrollbar(scrollbar);
            
            // Don't populate here - player isn't ready yet
            // PopulateSkillList() will be called in Update() when player is valid
        }
        
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            
            // Refresh on skill learn
            if (Main.GameUpdateCount % 60 == 0) // Every second
            {
                PopulateSkillList();
            }
        }
        
        private void PopulateSkillList()
        {
            skillList.Clear();
            
            // Check if player is valid
            var player = Main.LocalPlayer;
            if (player == null || !player.active || player.whoAmI < 0 || player.whoAmI >= Main.maxPlayers)
                return;
            
            Players.RpgPlayer rpgPlayer;
            Skills.SkillManager skillManager;
            
            try
            {
                rpgPlayer = player.GetModPlayer<Players.RpgPlayer>();
                skillManager = player.GetModPlayer<Skills.SkillManager>();
            }
            catch
            {
                // ModPlayer not ready yet
                return;
            }
            
            if (rpgPlayer == null || skillManager == null)
                return;
            
            // Get available skills for current job
            var jobData = JobDatabase.GetJobData(rpgPlayer.CurrentJob);
            if (jobData == null)
                return;
            
            // Display skill points
            string pointsLabel = rpgPlayer.PendingSkillPoints > 0
                ? $"Skill Points: {rpgPlayer.SkillPoints} (Pending: {rpgPlayer.PendingSkillPoints})"
                : $"Skill Points: {rpgPlayer.SkillPoints}";
            var pointsText = new UIText(pointsLabel, 1f);
            pointsText.TextColor = rpgPlayer.SkillPoints > 0
                ? Color.LightGreen
                : (rpgPlayer.PendingSkillPoints > 0 ? Color.Khaki : Color.Gray);
            skillList.Add(pointsText);
            
            var skillsForJob = Skills.SkillDatabase.GetSkillsForJob(rpgPlayer.CurrentJob);
            if (skillsForJob.Count == 0)
            {
                var emptyText = new UIText("No skills available for this job yet.", 0.9f);
                emptyText.TextColor = Color.Gray;
                skillList.Add(emptyText);
                return;
            }

            foreach (BaseSkill skill in skillsForJob)
            {
                if (skill == null)
                    continue;

                var skillCard = CreateSkillCard(skill, skillManager);
                skillList.Add(skillCard);
            }
        }
        
        private UIElement CreateSkillCard(BaseSkill skill, Skills.SkillManager skillManager)
        {
            var card = new UIPanel();
            card.Width.Set(0, 1f);
            card.Height.Set(SKILL_CARD_HEIGHT, 0f);
            card.BackgroundColor = new Color(44, 57, 105) * 0.8f;
            card.BorderColor = Color.Black;
            
            // Skill name
            var nameText = new UIText(skill.DisplayName, 1f);
            nameText.Top.Set(10, 0f);
            nameText.Left.Set(15, 0f);
            card.Append(nameText);
            
            // Skill type badge
            string typeStr = skill.SkillType switch
            {
                SkillType.Active => "[Active]",
                SkillType.Buff => "[Buff]",
                SkillType.Passive => "[Passive]",
                _ => "[Unknown]"
            };
            Color typeColor = skill.SkillType switch
            {
                SkillType.Active => Color.OrangeRed,
                SkillType.Buff => Color.Cyan,
                SkillType.Passive => Color.LimeGreen,
                _ => Color.Gray
            };
            var typeText = new UIText(typeStr, 0.75f);
            typeText.Top.Set(12, 0f);
            typeText.Left.Set(nameText.GetInnerDimensions().Width + 25, 0f);
            typeText.TextColor = typeColor;
            card.Append(typeText);
            
            // Rank info
            bool isLearned = skillManager.LearnedSkills.ContainsKey(skill.InternalName);
            int currentRank = isLearned ? skillManager.LearnedSkills[skill.InternalName].CurrentRank : 0;
            int maxRank = skill.MaxRank;
            
            var rankText = new UIText($"Rank: {currentRank}/{maxRank}", 0.85f);
            rankText.Top.Set(35, 0f);
            rankText.Left.Set(15, 0f);
            rankText.TextColor = currentRank >= maxRank ? Color.Gold : Color.LightGray;
            card.Append(rankText);
            
            // Cooldown and resource cost info (for non-passive skills)
            if (skill.SkillType != SkillType.Passive)
            {
                int effectiveCost = skill.GetEffectiveResourceCost(Main.LocalPlayer);
                string baseCostStr = skill.ResourceType switch
                {
                    ResourceType.Mana => $"{skill.ResourceCost} MP",
                    ResourceType.Stamina => $"{skill.ResourceCost} SP",
                    ResourceType.Life => $"{skill.ResourceCost} HP",
                    _ => ""
                };
                string costStr = baseCostStr;
                if (effectiveCost != skill.ResourceCost && effectiveCost > 0)
                {
                    string effectiveStr = skill.ResourceType switch
                    {
                        ResourceType.Mana => $"{effectiveCost} MP",
                        ResourceType.Stamina => $"{effectiveCost} SP",
                        ResourceType.Life => $"{effectiveCost} HP",
                        _ => ""
                    };
                    costStr = $"{effectiveStr} (base {baseCostStr})";
                }
                
                var statsText = new UIText($"CD: {skill.CooldownSeconds:F1}s  |  Cost: {costStr}", 0.75f);
                statsText.Top.Set(37, 0f);
                statsText.Left.Set(120, 0f);
                statsText.TextColor = Color.LightSkyBlue;
                card.Append(statsText);
            }

            // Description
            var descText = new UIText(skill.Description, 0.7f);
            descText.Top.Set(55, 0f);
            descText.Left.Set(15, 0f);
            descText.Width.Set(-130, 1f);
            descText.TextColor = Color.LightGray;
            card.Append(descText);
            
            // Learn/Upgrade button
            if (currentRank < maxRank)
            {
                var learnButton = new UIPanel();
                learnButton.Width.Set(100, 0f);
                learnButton.Height.Set(30, 0f);
                learnButton.Top.Set(60, 0f);
                learnButton.Left.Set(15, 0f);
                bool canLearn = isLearned
                    ? skillManager.LearnedSkills[skill.InternalName].CanLearn(Main.LocalPlayer)
                    : skill.CanLearn(Main.LocalPlayer);
                learnButton.BackgroundColor = canLearn ? Color.Green * 0.7f : Color.Gray * 0.5f;
                
                var buttonText = new UIText(currentRank == 0 ? "Learn" : "Upgrade", 0.9f);
                buttonText.HAlign = 0.5f;
                buttonText.VAlign = 0.5f;
                learnButton.Append(buttonText);
                
                learnButton.OnLeftClick += (evt, element) =>
                {
                    if (canLearn)
                    {
                        skillManager.LearnSkill(skill);
                        PopulateSkillList();
                    }
                };
                
                card.Append(learnButton);
                
                // Show reason why can't learn
                if (!canLearn)
                {
                    string reason = skill.GetCannotLearnReason(Main.LocalPlayer);
                    if (!string.IsNullOrEmpty(reason))
                    {
                        var reasonText = new UIText(reason, 0.65f);
                        reasonText.Top.Set(65, 0f);
                        reasonText.Left.Set(125, 0f);
                        reasonText.TextColor = Color.Salmon;
                        card.Append(reasonText);
                    }
                }
            }
            
            // Skill slot assignment buttons (1-9) - only for learned active/buff skills
            if (isLearned && skill.SkillType != SkillType.Passive)
            {
                var slotPanel = new UIPanel();
                slotPanel.Width.Set(220, 0f);
                slotPanel.Height.Set(28, 0f);
                slotPanel.Top.Set(8, 0f);
                slotPanel.HAlign = 1f;
                slotPanel.Left.Set(-10, 0f);
                slotPanel.BackgroundColor = new Color(20, 30, 50) * 0.6f;
                
                var slotLabel = new UIText("Slot:", 0.7f);
                slotLabel.Left.Set(5, 0f);
                slotLabel.VAlign = 0.5f;
                slotPanel.Append(slotLabel);
                
                for (int i = 0; i < 9; i++)
                {
                    int slot = i; // capture for closure
                    var slotBtn = new UIPanel();
                    slotBtn.Width.Set(16, 0f);
                    slotBtn.Height.Set(16, 0f);
                    slotBtn.Left.Set(40 + (i * 15), 0f);
                    slotBtn.VAlign = 0.5f;
                    
                    bool isAssigned = skillManager.SkillHotbar[i] == skill.InternalName;
                    slotBtn.BackgroundColor = isAssigned ? Color.Gold * 0.9f : Color.DarkGray * 0.5f;
                    
                    var slotNumText = new UIText($"{i + 1}", 0.6f);
                    slotNumText.HAlign = 0.5f;
                    slotNumText.VAlign = 0.5f;
                    slotBtn.Append(slotNumText);
                    
                    slotBtn.OnLeftClick += (evt, element) =>
                    {
                        skillManager.AssignSkillToSlot(skill.InternalName, slot);
                        PopulateSkillList();
                    };
                    
                    slotPanel.Append(slotBtn);
                }
                
                // Macro add button
                var macroBtn = new UIPanel();
                macroBtn.Width.Set(24, 0f);
                macroBtn.Height.Set(20, 0f);
                macroBtn.Left.Set(-30, 1f);
                macroBtn.VAlign = 0.5f;
                macroBtn.BackgroundColor = new Color(80, 60, 120) * 0.9f;
                
                var macroBtnText = new UIText("M", 0.7f);
                macroBtnText.HAlign = 0.5f;
                macroBtnText.VAlign = 0.5f;
                macroBtnText.TextColor = Color.Cyan;
                macroBtn.Append(macroBtnText);
                
                // Store skill name for closure
                string skillInternalName = skill.InternalName;
                
                macroBtn.OnLeftClick += (evt, element) =>
                {
                    // Open macro editor and set pending skill
                    var macroUISystem = ModContent.GetInstance<MacroUISystem>();
                    if (macroUISystem != null)
                    {
                        MacroEditorUI.PendingSkillToAdd = skillInternalName;
                        if (!macroUISystem.IsUIOpen)
                        {
                            macroUISystem.ToggleUI();
                        }
                        SoundEngine.PlaySound(SoundID.MenuTick);
                    }
                };
                
                macroBtn.OnMouseOver += (evt, element) =>
                {
                    Main.hoverItemName = "Add to Macro (Click or Drag)";
                };
                
                // Enable drag from this button using left click hold
                macroBtn.OnLeftMouseDown += (evt, element) =>
                {
                    SkillDragDropSystem.StartDrag(skillInternalName);
                };
                
                slotPanel.Append(macroBtn);
                
                card.Append(slotPanel);
            }
            
            return card;
        }
    }
}
