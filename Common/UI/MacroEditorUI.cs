using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.UI.Elements;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI;
using RpgMod.Common.Players;
using RpgMod.Common.Skills;
using RpgMod.Common.Systems;
using RpgMod.Common.Base;

namespace RpgMod.Common.UI
{
    /// <summary>
    /// UI System for managing macro editor visibility
    /// </summary>
    public class MacroUISystem : ModSystem
    {
        private UserInterface _userInterface;
        private MacroEditorUI _macroUI;
        
        public bool IsUIOpen { get; private set; }
        
        public override void Load()
        {
            if (!Main.dedServ)
            {
                _macroUI = new MacroEditorUI();
                _macroUI.Activate();
                _userInterface = new UserInterface();
            }
        }
        
        public override void Unload()
        {
            _macroUI = null;
            _userInterface = null;
        }
        
        public void ToggleUI()
        {
            IsUIOpen = !IsUIOpen;
            if (IsUIOpen)
            {
                _userInterface?.SetState(_macroUI);
                _macroUI?.Refresh();
                SoundEngine.PlaySound(SoundID.MenuOpen);
            }
            else
            {
                _userInterface?.SetState(null);
                SoundEngine.PlaySound(SoundID.MenuClose);
            }
        }
        
        public void CloseUI()
        {
            if (IsUIOpen)
            {
                IsUIOpen = false;
                _userInterface?.SetState(null);
            }
        }
        
        public override void UpdateUI(GameTime gameTime)
        {
            if (IsUIOpen)
            {
                _userInterface?.Update(gameTime);
            }
        }
        
        public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
        {
            int inventoryIndex = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Inventory"));
            if (inventoryIndex != -1 && IsUIOpen)
            {
                layers.Insert(inventoryIndex + 1, new LegacyGameInterfaceLayer(
                    "Rpg: Macro Editor",
                    delegate
                    {
                        _userInterface?.Draw(Main.spriteBatch, new GameTime());
                        return true;
                    },
                    InterfaceScaleType.UI));
            }
        }
    }
    
    /// <summary>
    /// Macro Editor UI Panel
    /// </summary>
    public class MacroEditorUI : UIState
    {
        private UIPanel _mainPanel;
        private int _selectedMacroIndex = 0;
        private List<MacroSlotElement> _macroSlots;
        private List<MacroSkillDropSlot> _skillSlots;
        private UIText _macroNameText;
        private UIPanel _dropZone;
        private UIPanel _hotbarZone;
        private List<UITextPanel<string>> _macroHotbarButtons;
        private bool _dragging;
        private Vector2 _dragOffset;
        private Vector2 _panelPosition;
        private Vector2 MouseUi => GetScaledMouse();
        private Point MouseUiPoint => MouseUi.ToPoint();
        private Vector2 MouseScreen => Main.MouseScreen;
        private Vector2 ScreenTopLeftUi => UiInput.ScreenTopLeftUi;
        private Vector2 ScreenBottomRightUi => UiInput.ScreenBottomRightUi;
        private float ScreenWidthUi => ScreenBottomRightUi.X - ScreenTopLeftUi.X;
        private float ScreenHeightUi => ScreenBottomRightUi.Y - ScreenTopLeftUi.Y;
        
        private const float PANEL_WIDTH = 500f;
        private const float PANEL_HEIGHT = 420f;
        private const float HEADER_HEIGHT = 35f;
        private static Vector2? _lastPosition;
        
        /// <summary>
        /// Skill pending to be added (set from SkillLearningUI)
        /// </summary>
        public static string PendingSkillToAdd { get; set; }
        
        public override void OnInitialize()
        {
            // Main panel
            _mainPanel = new UIPanel();
            _mainPanel.Width.Set(PANEL_WIDTH, 0f);
            _mainPanel.Height.Set(PANEL_HEIGHT, 0f);
            _mainPanel.HAlign = 0f;
            _mainPanel.VAlign = 0f;
            _mainPanel.SetPadding(10f);
            _mainPanel.BackgroundColor = new Color(73, 94, 171) * 0.9f;
            Append(_mainPanel);

            Vector2 topLeft = ScreenTopLeftUi;
            _panelPosition = _lastPosition ?? new Vector2(
                topLeft.X + ScreenWidthUi / 2f - PANEL_WIDTH / 2f,
                topLeft.Y + ScreenHeightUi / 2f - PANEL_HEIGHT / 2f
            );
            SetPanelPosition(_panelPosition);
            
            // Title
            UIText title = new UIText("Macro Editor", 1.2f);
            title.HAlign = 0.5f;
            title.Top.Set(5f, 0f);
            _mainPanel.Append(title);
            
            // Close button
            UITextPanel<string> closeButton = new UITextPanel<string>("X", 0.8f);
            closeButton.Width.Set(30f, 0f);
            closeButton.Height.Set(30f, 0f);
            closeButton.Left.Set(-35f, 1f);
            closeButton.Top.Set(5f, 0f);
            closeButton.OnLeftClick += (evt, elem) =>
            {
                ModContent.GetInstance<MacroUISystem>()?.CloseUI();
            };
            _mainPanel.Append(closeButton);
            
            // Macro selection buttons
            _macroSlots = new List<MacroSlotElement>();
            float macroSlotWidth = 70f;
            float macroSlotHeight = 34f;
            float macroSlotSpacing = 10f;
            float macroRowWidth = (SkillMacroSystem.MAX_MACROS * macroSlotWidth) +
                ((SkillMacroSystem.MAX_MACROS - 1) * macroSlotSpacing);
            float macroStartX = (PANEL_WIDTH - macroRowWidth) / 2f;
            for (int i = 0; i < SkillMacroSystem.MAX_MACROS; i++)
            {
                MacroSlotElement slot = new MacroSlotElement(i);
                slot.Width.Set(macroSlotWidth, 0f);
                slot.Height.Set(macroSlotHeight, 0f);
                slot.Left.Set(macroStartX + (i * (macroSlotWidth + macroSlotSpacing)), 0f);
                slot.Top.Set(42f, 0f);
                slot.OnLeftClick += OnMacroSlotClicked;
                slot.OnLeftMouseDown += (evt, elem) =>
                {
                    SkillDragDropSystem.StartMacroDrag(((MacroSlotElement)elem).MacroIndex);
                };
                _mainPanel.Append(slot);
                _macroSlots.Add(slot);
            }
            
            // Macro name display
            _macroNameText = new UIText("Macro 1", 1f);
            _macroNameText.HAlign = 0.5f;
            _macroNameText.Top.Set(92f, 0f);
            _mainPanel.Append(_macroNameText);
            
            // Skill slots for selected macro (with drop support)
            _skillSlots = new List<MacroSkillDropSlot>();
            _dropZone = new UIPanel();
            _dropZone.Width.Set(-20f, 1f);
            _dropZone.Height.Set(150f, 0f);
            _dropZone.Left.Set(10f, 0f);
            _dropZone.Top.Set(120f, 0f);
            _dropZone.BackgroundColor = new Color(50, 60, 120) * 0.8f;
            _mainPanel.Append(_dropZone);
            
            UIText slotLabel = new UIText("Macro Skills (drag here):", 0.9f);
            slotLabel.Top.Set(5f, 0f);
            slotLabel.Left.Set(5f, 0f);
            _dropZone.Append(slotLabel);

            float slotSize = 60f;
            float slotSpacing = 10f;
            float slotRowWidth = (SkillMacroSystem.MAX_SKILLS_PER_MACRO * slotSize) +
                ((SkillMacroSystem.MAX_SKILLS_PER_MACRO - 1) * slotSpacing);
            float slotStartX = (PANEL_WIDTH - 20f - slotRowWidth) / 2f;
            for (int i = 0; i < SkillMacroSystem.MAX_SKILLS_PER_MACRO; i++)
            {
                int slotIndex = i;
                MacroSkillDropSlot slot = new MacroSkillDropSlot(i, () => _selectedMacroIndex);
                slot.Width.Set(slotSize, 0f);
                slot.Height.Set(slotSize, 0f);
                slot.Left.Set(slotStartX + (i * (slotSize + slotSpacing)), 0f);
                slot.Top.Set(40f, 0f);
                slot.OnRightClick += (evt, elem) => OnSkillSlotRightClicked(evt, elem, slotIndex);
                _dropZone.Append(slot);
                _skillSlots.Add(slot);
            }

            _hotbarZone = new UIPanel();
            _hotbarZone.Width.Set(-20f, 1f);
            _hotbarZone.Height.Set(70f, 0f);
            _hotbarZone.Left.Set(10f, 0f);
            _hotbarZone.Top.Set(285f, 0f);
            _hotbarZone.BackgroundColor = new Color(45, 55, 90) * 0.8f;
            _mainPanel.Append(_hotbarZone);

            UIText hotbarLabel = new UIText("Assign macro to hotbar (1-9):", 0.85f);
            hotbarLabel.Top.Set(6f, 0f);
            hotbarLabel.Left.Set(8f, 0f);
            _hotbarZone.Append(hotbarLabel);

            _macroHotbarButtons = new List<UITextPanel<string>>();
            float hotbarButtonSize = 24f;
            float hotbarButtonSpacing = 6f;
            float hotbarRowWidth = (hotbarButtonSize * 9f) + (hotbarButtonSpacing * 8f);
            float hotbarStartX = (PANEL_WIDTH - 20f - hotbarRowWidth) / 2f;
            for (int i = 0; i < 9; i++)
            {
                int slotIndex = i;
                var slotBtn = new UITextPanel<string>($"{i + 1}", 0.7f);
                slotBtn.Width.Set(hotbarButtonSize, 0f);
                slotBtn.Height.Set(hotbarButtonSize, 0f);
                slotBtn.Left.Set(hotbarStartX + (i * (hotbarButtonSize + hotbarButtonSpacing)), 0f);
                slotBtn.Top.Set(36f, 0f);
                slotBtn.OnLeftClick += (evt, elem) =>
                {
                    var skillManager = Main.LocalPlayer.GetModPlayer<SkillManager>();
                    if (skillManager.AssignMacroToSlot(_selectedMacroIndex, slotIndex))
                    {
                        Refresh();
                    }
                };
                slotBtn.OnMouseOver += (evt, elem) =>
                {
                    Main.hoverItemName = $"Assign to slot {slotIndex + 1}";
                };
                _hotbarZone.Append(slotBtn);
                _macroHotbarButtons.Add(slotBtn);
            }
            
            // Pending skill add button
            UITextPanel<string> addPendingBtn = new UITextPanel<string>("+ Add Skill", 0.8f);
            addPendingBtn.Width.Set(120f, 0f);
            addPendingBtn.Height.Set(26f, 0f);
            addPendingBtn.Left.Set(-130f, 1f);
            addPendingBtn.Top.Set(4f, 0f);
            addPendingBtn.OnLeftClick += OnAddPendingClicked;
            addPendingBtn.OnMouseOver += (evt, elem) =>
            {
                if (!string.IsNullOrEmpty(PendingSkillToAdd))
                {
                    Main.hoverItemName = $"Add: {PendingSkillToAdd}";
                }
                else
                {
                    Main.hoverItemName = "Use [M] on a skill in the Skills tab";
                }
            };
            _dropZone.Append(addPendingBtn);
            
            // Execution mode toggle
            UITextPanel<string> modeButton = new UITextPanel<string>("Mode: Simultaneous", 0.8f);
            modeButton.Width.Set(200f, 0f);
            modeButton.Height.Set(35f, 0f);
            modeButton.Left.Set(10f, 0f);
            modeButton.Top.Set(-50f, 1f);
            modeButton.OnLeftClick += OnModeToggleClicked;
            _mainPanel.Append(modeButton);
            
            // Clear macro button
            UITextPanel<string> clearButton = new UITextPanel<string>("Clear Macro", 0.8f);
            clearButton.Width.Set(120f, 0f);
            clearButton.Height.Set(35f, 0f);
            clearButton.Left.Set(-130f, 1f);
            clearButton.Top.Set(-50f, 1f);
            clearButton.OnLeftClick += OnClearClicked;
            _mainPanel.Append(clearButton);
            
            // Instructions
            UIText instructions = new UIText("Drag skills here or use [M] in the Skills tab. Assign macro to hotbar below.", 0.7f);
            instructions.HAlign = 0.5f;
            instructions.Top.Set(-20f, 1f);
            _mainPanel.Append(instructions);
        }
        
        private void OnMacroSlotClicked(UIMouseEvent evt, UIElement elem)
        {
            if (elem is MacroSlotElement slot)
            {
                _selectedMacroIndex = slot.MacroIndex;
                Refresh();
                SoundEngine.PlaySound(SoundID.MenuTick);
            }
        }
        
        private void OnSkillSlotRightClicked(UIMouseEvent evt, UIElement elem, int slotIndex)
        {
            var player = Main.LocalPlayer.GetModPlayer<SkillMacroSystem>();
            var macro = player.GetMacro(_selectedMacroIndex);
            if (macro != null && slotIndex < macro.SkillIds.Count)
            {
                string skillToRemove = macro.SkillIds[slotIndex];
                player.RemoveSkillFromMacro(_selectedMacroIndex, skillToRemove);
            }
            Refresh();
            SoundEngine.PlaySound(SoundID.MenuClose);
        }
        
        private void OnAddPendingClicked(UIMouseEvent evt, UIElement elem)
        {
            if (string.IsNullOrEmpty(PendingSkillToAdd))
            {
                Main.NewText("No skill selected. Click [M] button next to a skill first.", Color.Yellow);
                return;
            }
            
            var player = Main.LocalPlayer.GetModPlayer<SkillMacroSystem>();
            bool success = player.AddSkillToMacro(_selectedMacroIndex, PendingSkillToAdd);
            
            if (success)
            {
                Main.NewText($"Added {PendingSkillToAdd} to Macro {_selectedMacroIndex + 1}", Color.LightGreen);
                PendingSkillToAdd = null;
                Refresh();
                SoundEngine.PlaySound(SoundID.Item4);
            }
            else
            {
                Main.NewText("Macro is full or skill already in macro.", Color.Red);
                SoundEngine.PlaySound(SoundID.MenuClose);
            }
        }
        
        private void OnModeToggleClicked(UIMouseEvent evt, UIElement elem)
        {
            var player = Main.LocalPlayer.GetModPlayer<SkillMacroSystem>();
            var macro = player.GetMacro(_selectedMacroIndex);
            if (macro != null)
            {
                macro.ExecutionMode = macro.ExecutionMode == MacroExecutionMode.Simultaneous
                    ? MacroExecutionMode.Sequential
                    : MacroExecutionMode.Simultaneous;
                
                if (elem is UITextPanel<string> button)
                {
                    button.SetText($"Mode: {macro.ExecutionMode}");
                }
                SoundEngine.PlaySound(SoundID.MenuTick);
            }
        }
        
        private void OnClearClicked(UIMouseEvent evt, UIElement elem)
        {
            var player = Main.LocalPlayer.GetModPlayer<SkillMacroSystem>();
            player.ClearMacro(_selectedMacroIndex);
            Refresh();
            SoundEngine.PlaySound(SoundID.MenuClose);
        }
        
        public void Refresh()
        {
            var player = Main.LocalPlayer.GetModPlayer<SkillMacroSystem>();
            var macro = player.GetMacro(_selectedMacroIndex);
            
            // Update macro name
            string defaultName = $"Macro {_selectedMacroIndex + 1}";
            string macroName = macro?.Name ?? defaultName;
            if (string.IsNullOrWhiteSpace(macroName) || macroName == defaultName)
            {
                _macroNameText?.SetText(defaultName);
            }
            else
            {
                _macroNameText?.SetText($"{defaultName}: {macroName}");
            }
            
            // Update macro slot highlights
            foreach (var slot in _macroSlots)
            {
                slot.IsSelected = slot.MacroIndex == _selectedMacroIndex;
            }
            
            // Update skill slots
            for (int i = 0; i < _skillSlots.Count; i++)
            {
                string skillId = null;
                if (macro != null && i < macro.SkillIds.Count)
                {
                    skillId = macro.SkillIds[i];
                }
                _skillSlots[i].SetSkill(skillId);
            }

            var skillManager = Main.LocalPlayer.GetModPlayer<SkillManager>();
            if (_macroHotbarButtons != null && skillManager != null)
            {
                for (int i = 0; i < _macroHotbarButtons.Count; i++)
                {
                    bool assigned = SkillManager.TryParseMacroEntry(skillManager.SkillHotbar[i], out int macroIndex)
                        && macroIndex == _selectedMacroIndex;
                    _macroHotbarButtons[i].BackgroundColor = assigned ? new Color(200, 180, 80) : new Color(60, 70, 100);
                }
            }
        }
        
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (_mainPanel != null)
            {
                CalculatedStyle dims = _mainPanel.GetDimensions();
                Rectangle header = new Rectangle(
                    (int)dims.X,
                    (int)dims.Y,
                    (int)dims.Width,
                    (int)HEADER_HEIGHT
                );

                if (!_dragging && Main.mouseLeft && header.Contains(MouseUiPoint))
                {
                    _dragging = true;
                    _dragOffset = new Vector2(MouseUi.X - dims.X, MouseUi.Y - dims.Y);
                }
                else if (_dragging && !Main.mouseLeft)
                {
                    _dragging = false;
                    _lastPosition = _panelPosition;
                }

                if (_dragging)
                {
                    SetPanelPosition(new Vector2(MouseUi.X - _dragOffset.X, MouseUi.Y - _dragOffset.Y));
                }
            }

            // Keep panel on screen
            if (_mainPanel.ContainsPoint(MouseScreen))
            {
                Main.LocalPlayer.mouseInterface = true;
            }

            if (SkillDragDropSystem.DropPending && SkillDragDropSystem.IsMacroDrag && _macroHotbarButtons != null)
            {
                for (int i = 0; i < _macroHotbarButtons.Count; i++)
                {
                    if (_macroHotbarButtons[i].ContainsPoint(MouseScreen))
                    {
                        if (SkillDragDropSystem.TryConsumeMacroDrop(out int macroIndex))
                        {
                            var skillManager = Main.LocalPlayer.GetModPlayer<SkillManager>();
                            if (skillManager.AssignMacroToSlot(macroIndex, i))
                            {
                                Refresh();
                            }
                        }
                        break;
                    }
                }
            }
            
            // Auto-refresh when drag ends to show updated slots
            if (_needsRefresh)
            {
                _needsRefresh = false;
                Refresh();
            }
        }
        
        private bool _needsRefresh = false;
        
        /// <summary>
        /// Request a refresh on next update (called from drop slots)
        /// </summary>
        public void RequestRefresh()
        {
            _needsRefresh = true;
        }

        private Vector2 GetScaledMouse()
        {
            return UiInput.GetUiMouse();
        }

        private void SetPanelPosition(Vector2 position)
        {
            float minX = ScreenTopLeftUi.X;
            float minY = ScreenTopLeftUi.Y;
            float maxX = ScreenTopLeftUi.X + ScreenWidthUi - PANEL_WIDTH;
            float maxY = ScreenTopLeftUi.Y + ScreenHeightUi - PANEL_HEIGHT;
            position.X = MathHelper.Clamp(position.X, minX, maxX);
            position.Y = MathHelper.Clamp(position.Y, minY, maxY);
            _panelPosition = position;
            _mainPanel.Left.Set(_panelPosition.X, 0f);
            _mainPanel.Top.Set(_panelPosition.Y, 0f);
            _mainPanel.Recalculate();
        }
    }
    
    /// <summary>
    /// UI element representing a macro selection slot
    /// </summary>
    public class MacroSlotElement : UIElement
    {
        public int MacroIndex { get; private set; }
        public bool IsSelected { get; set; }
        
        private UIText _text;
        
        public MacroSlotElement(int index)
        {
            MacroIndex = index;
        }
        
        public override void OnInitialize()
        {
            _text = new UIText($"M{MacroIndex + 1}", 0.9f);
            _text.HAlign = 0.5f;
            _text.VAlign = 0.5f;
            Append(_text);
        }
        
        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            CalculatedStyle dims = GetDimensions();
            Color bgColor = IsSelected ? new Color(100, 150, 255) : new Color(60, 70, 130);
            
            // Draw background
            spriteBatch.Draw(
                Terraria.GameContent.TextureAssets.MagicPixel.Value,
                dims.ToRectangle(),
                bgColor);
            
            // Draw border
            int borderWidth = 2;
            Rectangle rect = dims.ToRectangle();
            spriteBatch.Draw(Terraria.GameContent.TextureAssets.MagicPixel.Value, 
                new Rectangle(rect.X, rect.Y, rect.Width, borderWidth), Color.White);
            spriteBatch.Draw(Terraria.GameContent.TextureAssets.MagicPixel.Value, 
                new Rectangle(rect.X, rect.Y + rect.Height - borderWidth, rect.Width, borderWidth), Color.White);
            spriteBatch.Draw(Terraria.GameContent.TextureAssets.MagicPixel.Value, 
                new Rectangle(rect.X, rect.Y, borderWidth, rect.Height), Color.White);
            spriteBatch.Draw(Terraria.GameContent.TextureAssets.MagicPixel.Value, 
                new Rectangle(rect.X + rect.Width - borderWidth, rect.Y, borderWidth, rect.Height), Color.White);
        }
    }
    
    /// <summary>
    /// UI element representing a skill slot within a macro (with drag-drop support)
    /// </summary>
    public class MacroSkillDropSlot : UIElement
    {
        public int SlotIndex { get; private set; }
        private string _skillId = null;
        private UIText _skillText;
        private Texture2D _skillIcon;
        private string _skillDisplayName;
        private System.Func<int> _getMacroIndex;
        
        public MacroSkillDropSlot(int index, System.Func<int> getMacroIndex)
        {
            SlotIndex = index;
            _getMacroIndex = getMacroIndex;
        }
        
        public override void OnInitialize()
        {
            _skillText = new UIText("Drop\nHere", 0.6f);
            _skillText.HAlign = 0.5f;
            _skillText.VAlign = 0.5f;
            Append(_skillText);
        }
        
        public void SetSkill(string skillId)
        {
            _skillId = skillId;
            _skillIcon = null;
            _skillDisplayName = null;

            if (string.IsNullOrEmpty(skillId))
            {
                _skillText?.SetText("Drop\nHere");
                return;
            }

            var skillManager = Main.LocalPlayer.GetModPlayer<SkillManager>();
            if (skillManager?.LearnedSkills != null && skillManager.LearnedSkills.TryGetValue(skillId, out var skill))
            {
                _skillDisplayName = skill?.DisplayName ?? skillId;
                _skillIcon = AssetLoader.GetTexture(skill.IconTexture);
            }
            else
            {
                _skillDisplayName = skillId;
                var skillDef = SkillDatabase.GetSkill(skillId);
                if (skillDef != null)
                {
                    _skillDisplayName = skillDef.DisplayName;
                    _skillIcon = AssetLoader.GetTexture(skillDef.IconTexture);
                }
                else
                {
                    _skillIcon = AssetLoader.GetSkillIcon(skillId);
                }
            }

            _skillText?.SetText("");
        }
        
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            
            // Check for drag-drop
            if (SkillDragDropSystem.DropPending && SkillDragDropSystem.IsSkillDrag && IsMouseHovering)
            {
                // Mouse released over this slot - accept the drop
                var player = Main.LocalPlayer.GetModPlayer<SkillMacroSystem>();
                int macroIndex = _getMacroIndex?.Invoke() ?? 0;

                if (!SkillDragDropSystem.TryConsumeDrop(out string skillId))
                    return;

                bool success = player.AddSkillToMacro(macroIndex, skillId);
                if (success)
                {
                    Main.NewText($"Added skill to Macro {macroIndex + 1}", Color.LightGreen);
                    SoundEngine.PlaySound(SoundID.Item4);
                    
                    // Request refresh of parent UI
                    var macroUI = ModContent.GetInstance<MacroUISystem>();
                    if (macroUI != null)
                    {
                        // Get the MacroEditorUI state and refresh it
                        var state = (MacroEditorUI)typeof(MacroUISystem)
                            .GetField("_macroUI", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                            ?.GetValue(macroUI);
                        state?.RequestRefresh();
                    }
                }
                else
                {
                    Main.NewText("Macro is full or skill already exists.", Color.Red);
                }
            }
        }
        
        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            CalculatedStyle dims = GetDimensions();
            
            // Highlight if dragging over
            bool isDragTarget = SkillDragDropSystem.IsDragging && IsMouseHovering;
            Color bgColor;
            
            if (isDragTarget)
            {
                bgColor = new Color(100, 180, 100); // Highlight green when drag target
            }
            else if (!string.IsNullOrEmpty(_skillId))
            {
                bgColor = new Color(80, 120, 80);
            }
            else
            {
                bgColor = new Color(50, 50, 70);
            }
            
            // Draw background
            Rectangle rect = dims.ToRectangle();
            spriteBatch.Draw(
                Terraria.GameContent.TextureAssets.MagicPixel.Value,
                rect,
                bgColor);

            if (!string.IsNullOrEmpty(_skillId) && _skillIcon != null)
            {
                Rectangle iconRect = new Rectangle(rect.X + 4, rect.Y + 4, rect.Width - 8, rect.Height - 8);
                spriteBatch.Draw(_skillIcon, iconRect, Color.White);
            }
            
            // Draw border
            int borderWidth = 2;
            Color borderColor = isDragTarget ? Color.Lime : (IsMouseHovering ? Color.Gold : Color.Gray);
            spriteBatch.Draw(Terraria.GameContent.TextureAssets.MagicPixel.Value, 
                new Rectangle(rect.X, rect.Y, rect.Width, borderWidth), borderColor);
            spriteBatch.Draw(Terraria.GameContent.TextureAssets.MagicPixel.Value, 
                new Rectangle(rect.X, rect.Y + rect.Height - borderWidth, rect.Width, borderWidth), borderColor);
            spriteBatch.Draw(Terraria.GameContent.TextureAssets.MagicPixel.Value, 
                new Rectangle(rect.X, rect.Y, borderWidth, rect.Height), borderColor);
            spriteBatch.Draw(Terraria.GameContent.TextureAssets.MagicPixel.Value, 
                new Rectangle(rect.X + rect.Width - borderWidth, rect.Y, borderWidth, rect.Height), borderColor);
        }
        
        public override void MouseOver(UIMouseEvent evt)
        {
            base.MouseOver(evt);
            
            if (SkillDragDropSystem.IsDragging)
            {
                Main.hoverItemName = $"Drop: {SkillDragDropSystem.DraggedSkillId}";
            }
            else if (!string.IsNullOrEmpty(_skillId))
            {
                string name = string.IsNullOrEmpty(_skillDisplayName) ? _skillId : _skillDisplayName;
                Main.hoverItemName = $"{name}\nRight-click to remove";
            }
            else
            {
                Main.hoverItemName = "Drag skill here";
            }
        }
    }
}
