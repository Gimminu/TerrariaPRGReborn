using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader;
using Terraria.UI;
using RpgMod.Common.Base;
using RpgMod.Common.Systems;

namespace RpgMod.Common.UI
{
    /// <summary>
    /// Always-visible skill bar showing hotkeyed skills and cooldowns
    /// </summary>
    public class SkillBarUI : UIState
    {
        private UIPanel[] skillSlots;
        private const int SLOT_COUNT = 9;
        private const float SLOT_SIZE = 44f;
        private const float SLOT_SPACING = 2f;
        
        public override void OnInitialize()
        {
            skillSlots = new UIPanel[SLOT_COUNT];
            
            float totalWidth = (SLOT_SIZE * SLOT_COUNT) + (SLOT_SPACING * (SLOT_COUNT - 1));
            float startX = (Main.screenWidth - totalWidth) / 2f;
            
            for (int i = 0; i < SLOT_COUNT; i++)
            {
                var slot = new SkillSlotPanel(i);
                slot.Width.Set(SLOT_SIZE, 0f);
                slot.Height.Set(SLOT_SIZE, 0f);
                slot.Left.Set(startX + (i * (SLOT_SIZE + SLOT_SPACING)), 0f);
                slot.Top.Set(Main.screenHeight - 90, 0f);
                slot.BackgroundColor = new Color(33, 43, 79) * 0.8f;
                slot.BorderColor = new Color(89, 116, 213) * 0.9f;
                
                Append(slot);
                skillSlots[i] = slot;
            }
        }
        
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            
            // Update slot positions (in case of screen resize)
            float totalWidth = (SLOT_SIZE * SLOT_COUNT) + (SLOT_SPACING * (SLOT_COUNT - 1));
            float startX = (Main.screenWidth - totalWidth) / 2f;
            
            for (int i = 0; i < SLOT_COUNT; i++)
            {
                skillSlots[i].Left.Set(startX + (i * (SLOT_SIZE + SLOT_SPACING)), 0f);
                skillSlots[i].Top.Set(Main.screenHeight - 90, 0f);
                skillSlots[i].Recalculate();
            }
        }
        
        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            // Hide when inventory or menus are open
            if (Main.playerInventory || Main.ingameOptionsWindow || Main.inFancyUI || Main.gameMenu)
                return;
                
            base.DrawSelf(spriteBatch);
        }
    }
    
    /// <summary>
    /// Individual skill slot showing skill icon, cooldown, and keybind
    /// </summary>
    public class SkillSlotPanel : UIPanel
    {
        private int slotIndex;
        private UIText keybindText;
        private UIText cooldownText;
        
        public SkillSlotPanel(int index)
        {
            slotIndex = index;
            
            // Keybind number
            keybindText = new UIText($"{index + 1}", 0.7f);
            keybindText.Top.Set(2, 0f);
            keybindText.Left.Set(4, 0f);
            keybindText.TextColor = Color.White * 0.8f;
            Append(keybindText);
            
            // Cooldown overlay
            cooldownText = new UIText("", 1.2f);
            cooldownText.HAlign = 0.5f;
            cooldownText.VAlign = 0.5f;
            cooldownText.TextColor = Color.Yellow;
            Append(cooldownText);
        }
        
        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            base.DrawSelf(spriteBatch);
            
            var player = Main.LocalPlayer;
            var skillManager = player.GetModPlayer<Skills.SkillManager>();
            
            string skillName = skillManager.SkillHotbar[slotIndex];

            if (Skills.SkillManager.TryParseMacroEntry(skillName, out int macroIndex))
            {
                DrawMacroSlot(spriteBatch, macroIndex);
                return;
            }
            
            if (string.IsNullOrEmpty(skillName))
            {
                // Empty slot - draw placeholder
                DrawEmptySlot(spriteBatch);
                return;
            }
            
            if (!skillManager.LearnedSkills.ContainsKey(skillName))
            {
                // Invalid skill - draw error
                DrawErrorSlot(spriteBatch);
                return;
            }
            
            var skill = skillManager.LearnedSkills[skillName];
            var dimensions = GetDimensions();
            Rectangle iconRect = new Rectangle(
                (int)dimensions.X + 3,
                (int)dimensions.Y + 3,
                (int)dimensions.Width - 6,
                (int)dimensions.Height - 6
            );

            Texture2D icon = AssetLoader.GetTexture(skill.IconTexture);
            spriteBatch.Draw(icon, iconRect, Color.White);
            
            // Draw cooldown overlay
            if (skill.CurrentCooldown > 0f)
            {
                float remaining = skill.CurrentCooldown;
                cooldownText.SetText($"{remaining:F1}");
                
                // Draw dark overlay
                Rectangle rect = dimensions.ToRectangle();
                spriteBatch.Draw(
                    Terraria.GameContent.TextureAssets.MagicPixel.Value,
                    rect,
                    Color.Black * 0.6f
                );
            }
            else
            {
                cooldownText.SetText("");
            }
            
            // Highlight if hovered
            if (IsMouseHovering)
            {
                BorderColor = Color.White;
            }
            else
            {
                BorderColor = new Color(89, 116, 213) * 0.8f;
            }
        }
        
        private void DrawEmptySlot(SpriteBatch spriteBatch)
        {
            var dimensions = GetDimensions();
            Vector2 center = new Vector2(dimensions.X + dimensions.Width / 2, dimensions.Y + dimensions.Height / 2);
            
            Utils.DrawBorderStringFourWay(
                spriteBatch,
                Terraria.GameContent.FontAssets.MouseText.Value,
                "Empty",
                center.X,
                center.Y,
                Color.Gray,
                Color.Black,
                Vector2.One * 0.5f,
                0.5f
            );
        }
        
        private void DrawErrorSlot(SpriteBatch spriteBatch)
        {
            var dimensions = GetDimensions();
            Vector2 center = new Vector2(dimensions.X + dimensions.Width / 2, dimensions.Y + dimensions.Height / 2);
            
            Utils.DrawBorderStringFourWay(
                spriteBatch,
                Terraria.GameContent.FontAssets.MouseText.Value,
                "Error",
                center.X,
                center.Y,
                Color.Red,
                Color.Black,
                Vector2.One * 0.5f,
                0.5f
            );
        }

        private void DrawMacroSlot(SpriteBatch spriteBatch, int macroIndex)
        {
            var dimensions = GetDimensions();
            Rectangle rect = dimensions.ToRectangle();
            Vector2 center = new Vector2(dimensions.X + dimensions.Width / 2, dimensions.Y + dimensions.Height / 2);

            string label = $"M{macroIndex + 1}";
            Utils.DrawBorderStringFourWay(
                spriteBatch,
                Terraria.GameContent.FontAssets.MouseText.Value,
                label,
                center.X,
                center.Y,
                Color.Cyan,
                Color.Black,
                Vector2.One * 0.5f,
                0.8f
            );

            var macroSystem = Main.LocalPlayer.GetModPlayer<SkillMacroSystem>();
            float cd = macroSystem?.GetMacroCooldownRemaining(macroIndex) ?? 0f;
            if (cd > 0f)
            {
                cooldownText.SetText($"{cd:F1}");
                spriteBatch.Draw(
                    Terraria.GameContent.TextureAssets.MagicPixel.Value,
                    rect,
                    Color.Black * 0.6f
                );
            }
            else
            {
                cooldownText.SetText("");
            }
        }
        
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (SkillDragDropSystem.DropPending && IsMouseHovering)
            {
                if (SkillDragDropSystem.TryConsumeMacroDrop(out int macroIndex))
                {
                    var skillManager = Main.LocalPlayer.GetModPlayer<Skills.SkillManager>();
                    skillManager.AssignMacroToSlot(macroIndex, slotIndex);
                    return;
                }

                if (SkillDragDropSystem.TryConsumeDrop(out string skillId))
                {
                    var skillManager = Main.LocalPlayer.GetModPlayer<Skills.SkillManager>();
                    skillManager.AssignSkillToSlot(skillId, slotIndex);
                    return;
                }
            }

            // Tooltip on hover
            if (IsMouseHovering)
            {
                var player = Main.LocalPlayer;
                var skillManager = player.GetModPlayer<Skills.SkillManager>();
                string skillName = skillManager.SkillHotbar[slotIndex];

                if (Skills.SkillManager.TryParseMacroEntry(skillName, out int macroIndex))
                {
                    var macroSystem = player.GetModPlayer<SkillMacroSystem>();
                    var macro = macroSystem?.GetMacro(macroIndex);
                    string macroName = macro?.Name ?? $"Macro {macroIndex + 1}";
                    int count = macro?.SkillIds.Count ?? 0;
                    Main.hoverItemName = $"{macroName}\nSkills: {count}";
                    return;
                }
                
                if (!string.IsNullOrEmpty(skillName) && skillManager.LearnedSkills.ContainsKey(skillName))
                {
                    var skill = skillManager.LearnedSkills[skillName];
                    
                    string tooltip = $"{skill.DisplayName} (Rank {skill.CurrentRank}/{skill.MaxRank})\n";
                    tooltip += $"{skill.Description}\n";

                    if (skill.SkillType == SkillType.Passive)
                    {
                        tooltip += "Passive Skill\n";
                    }
                    else if (skill.ResourceCost > 0)
                    {
                        int effectiveCost = skill.GetEffectiveResourceCost(player);
                        string resourceName = skill.ResourceType switch
                        {
                            ResourceType.Stamina => "Stamina",
                            ResourceType.Mana => "Mana",
                            ResourceType.Life => "Life",
                            _ => "Cost"
                        };
                        if (effectiveCost != skill.ResourceCost)
                            tooltip += $"{resourceName}: {effectiveCost} (base {skill.ResourceCost})\n";
                        else
                            tooltip += $"{resourceName}: {skill.ResourceCost}\n";
                    }

                    if (skill.CooldownSeconds > 0)
                        tooltip += $"Cooldown: {skill.CooldownSeconds}s\n";
                    
                    Main.hoverItemName = tooltip;
                }
            }
        }
        
        public override void LeftClick(UIMouseEvent evt)
        {
            base.LeftClick(evt);
            
            // Check if dragging a skill - assign to this slot
            if (SkillDragDropSystem.IsDragging && !string.IsNullOrEmpty(SkillDragDropSystem.DraggedSkillId))
            {
                var skillManager = Main.LocalPlayer.GetModPlayer<Skills.SkillManager>();
                if (SkillDragDropSystem.TryConsumeMacroDrop(out int macroIndex))
                {
                    skillManager.AssignMacroToSlot(macroIndex, slotIndex);
                    return;
                }
                if (SkillDragDropSystem.IsSkillDrag)
                {
                    skillManager.AssignSkillToSlot(SkillDragDropSystem.DraggedSkillId, slotIndex);
                    SkillDragDropSystem.StopDrag();
                    return;
                }
            }
            
            // Use skill on click
            var sm = Main.LocalPlayer.GetModPlayer<Skills.SkillManager>();
            sm.UseSkillInSlot(slotIndex);
        }
        
        public override void RightClick(UIMouseEvent evt)
        {
            base.RightClick(evt);
            
            // Clear skill from slot on right-click
            var skillManager = Main.LocalPlayer.GetModPlayer<Skills.SkillManager>();
            skillManager.SkillHotbar[slotIndex] = "";
        }
    }
}
