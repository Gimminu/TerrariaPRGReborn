using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI;
using Rpg.Common.Skills;
using Rpg.Common.Systems;
using Rpg.Common.Base;

namespace Rpg.Common.UI
{
    /// <summary>
    /// Global system for managing skill drag-and-drop operations
    /// </summary>
    public class SkillDragDropSystem : ModSystem
    {
        /// <summary>
        /// Currently dragged skill (null if not dragging)
        /// </summary>
        public static string DraggedSkillId { get; private set; }

        /// <summary>
        /// Drop pending after mouse release
        /// </summary>
        public static bool DropPending { get; private set; }
        
        /// <summary>
        /// Is a skill currently being dragged?
        /// </summary>
        public static bool IsDragging => !string.IsNullOrEmpty(DraggedSkillId);

        public static bool IsMacroDrag => IsDragging && Skills.SkillManager.TryParseMacroEntry(DraggedSkillId, out _);
        public static bool IsSkillDrag => IsDragging && !IsMacroDrag;
        
        /// <summary>
        /// Position where dragging started
        /// </summary>
        public static Vector2 DragStartPosition { get; private set; }
        
        /// <summary>
        /// Start dragging a skill
        /// </summary>
        public static void StartDrag(string skillId)
        {
            if (string.IsNullOrEmpty(skillId))
                return;

            DraggedSkillId = skillId;
            DragStartPosition = Main.MouseScreen;
            DropPending = false;
            _dragMouseDown = true;
            _dropPendingFrames = 0;
        }

        public static void StartMacroDrag(int macroIndex)
        {
            DraggedSkillId = Skills.SkillManager.BuildMacroEntry(macroIndex);
            DragStartPosition = Main.MouseScreen;
            DropPending = false;
            _dragMouseDown = true;
            _dropPendingFrames = 0;
        }
        
        /// <summary>
        /// Stop dragging
        /// </summary>
        public static void StopDrag()
        {
            DraggedSkillId = null;
            DropPending = false;
            _dragMouseDown = false;
            _dropPendingFrames = 0;
        }

        /// <summary>
        /// Consume a pending drop (returns skill id if valid)
        /// </summary>
        public static bool TryConsumeDrop(out string skillId)
        {
            skillId = null;
            if (!DropPending || string.IsNullOrEmpty(DraggedSkillId) || IsMacroDrag)
                return false;

            skillId = DraggedSkillId;
            StopDrag();
            return true;
        }

        public static bool TryConsumeMacroDrop(out int macroIndex)
        {
            macroIndex = -1;
            if (!DropPending || !IsMacroDrag)
                return false;

            if (!Skills.SkillManager.TryParseMacroEntry(DraggedSkillId, out macroIndex))
                return false;

            StopDrag();
            return true;
        }
        
        /// <summary>
        /// Cancel drag if mouse released
        /// </summary>
        public override void UpdateUI(GameTime gameTime)
        {
            if (!IsDragging)
                return;

            if (_dragMouseDown && !Main.mouseLeft)
            {
                DropPending = true;
                _dragMouseDown = false;
                _dropPendingFrames = DROP_PENDING_FRAMES;
            }
            else if (Main.mouseLeft)
            {
                _dragMouseDown = true;
            }

            if (DropPending)
            {
                _dropPendingFrames--;
                if (_dropPendingFrames <= 0 || Main.mouseLeft)
                {
                    StopDrag();
                }
            }
        }
        
        /// <summary>
        /// Draw dragged skill icon at mouse position
        /// </summary>
        public override void PostDrawInterface(SpriteBatch spriteBatch)
        {
            if (!IsDragging)
                return;

            Vector2 pos = Main.MouseScreen + new Vector2(15, 15);
            string displayName;
            Texture2D icon = null;

            if (IsMacroDrag)
            {
                if (!Skills.SkillManager.TryParseMacroEntry(DraggedSkillId, out int macroIndex))
                    return;

                var macroSystem = Main.LocalPlayer.GetModPlayer<SkillMacroSystem>();
                var macro = macroSystem?.GetMacro(macroIndex);
                displayName = macro?.Name ?? $"Macro {macroIndex + 1}";
            }
            else
            {
                var skillManager = Main.LocalPlayer.GetModPlayer<SkillManager>();
                if (!skillManager.LearnedSkills.TryGetValue(DraggedSkillId, out var skill))
                    return;

                displayName = skill.DisplayName;
                icon = AssetLoader.GetSkillIcon(skill.InternalName);
            }

            if (displayName.Length > 12)
                displayName = displayName.Substring(0, 11) + "..";
            
            // Background
            Vector2 textSize = Terraria.GameContent.FontAssets.MouseText.Value.MeasureString(displayName) * 0.8f;
            int iconSize = icon != null ? 22 : 0;
            int padding = icon != null ? 6 : 0;
            Rectangle bgRect = new Rectangle(
                (int)pos.X - 5,
                (int)pos.Y - 3,
                (int)textSize.X + 10 + iconSize + padding,
                (int)textSize.Y + 6
            );
            
            spriteBatch.Draw(
                Terraria.GameContent.TextureAssets.MagicPixel.Value,
                bgRect,
                new Color(40, 50, 100) * 0.9f
            );
            
            // Border
            Utils.DrawRect(spriteBatch, bgRect, Color.Cyan * 0.8f);
            
            // Text
            Utils.DrawBorderStringFourWay(
                spriteBatch,
                Terraria.GameContent.FontAssets.MouseText.Value,
                displayName,
                pos.X + iconSize + padding,
                pos.Y,
                Color.White,
                Color.Black,
                Vector2.Zero,
                0.8f
            );

            if (icon != null)
            {
                Rectangle iconRect = new Rectangle((int)pos.X, (int)pos.Y - 1, iconSize, iconSize);
                spriteBatch.Draw(icon, iconRect, Color.White);
            }

        }

        private static bool _dragMouseDown;
        private const int DROP_PENDING_FRAMES = 6;
        private static int _dropPendingFrames;
    }
}
