using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using RpgMod.Common.Players;
using RpgMod.Common.Skills;

namespace RpgMod.Common.Systems
{
    /// <summary>
    /// Skill Macro System - allows players to bind multiple skills to a single key
    /// Macros can execute skills sequentially or simultaneously
    /// </summary>
    public class SkillMacroSystem : ModPlayer
    {
        /// <summary>
        /// Maximum number of macro slots
        /// </summary>
        public const int MAX_MACROS = 5;
        
        /// <summary>
        /// Maximum skills per macro
        /// </summary>
        public const int MAX_SKILLS_PER_MACRO = 6;
        
        /// <summary>
        /// Stored macros
        /// </summary>
        public SkillMacro[] Macros { get; private set; }
        
        /// <summary>
        /// Cooldown between macro uses (ticks)
        /// </summary>
        private int[] macroCooldowns;
        
        private const int MACRO_COOLDOWN_TICKS = 30; // 0.5 seconds
        
        public override void Initialize()
        {
            Macros = new SkillMacro[MAX_MACROS];
            macroCooldowns = new int[MAX_MACROS];
            
            for (int i = 0; i < MAX_MACROS; i++)
            {
                Macros[i] = new SkillMacro($"Macro {i + 1}");
            }
        }
        
        public override void PreUpdate()
        {
            // Decrease cooldowns
            for (int i = 0; i < MAX_MACROS; i++)
            {
                if (macroCooldowns[i] > 0)
                    macroCooldowns[i]--;
            }
        }
        
        #region Macro Execution
        
        /// <summary>
        /// Execute a macro by index
        /// </summary>
        public bool ExecuteMacro(int macroIndex)
        {
            if (macroIndex < 0 || macroIndex >= MAX_MACROS)
                return false;
            
            if (macroCooldowns[macroIndex] > 0)
                return false;
            
            var macro = Macros[macroIndex];
            if (macro == null || macro.SkillIds.Count == 0)
                return false;
            
            var skillManager = Player.GetModPlayer<SkillManager>();
            
            if (skillManager == null)
                return false;
            
            bool anySuccess = false;
            int successCount = 0;
            
            // Execute skills based on mode
            foreach (string skillId in macro.SkillIds)
            {
                if (!skillManager.LearnedSkills.TryGetValue(skillId, out var skill))
                    continue;
                
                if (skill == null || skill.CurrentRank <= 0)
                    continue;
                
                // Check if skill is on cooldown
                if (skill.CurrentCooldown > 0f)
                    continue;
                
                // Use the skill
                skillManager.UseSkill(skillId);
                anySuccess = true;
                successCount++;
                
                // Sequential mode: add delay between skills
                if (macro.ExecutionMode == MacroExecutionMode.Sequential)
                {
                    // Small delay will be handled by individual skill cooldowns
                }
            }
            
            if (anySuccess)
            {
                macroCooldowns[macroIndex] = MACRO_COOLDOWN_TICKS;
                
                // Visual feedback
                if (successCount > 0)
                {
                    ShowMacroFeedback(macro.Name, successCount);
                }
            }
            
            return anySuccess;
        }
        
        private void ShowMacroFeedback(string macroName, int skillCount)
        {
            if (Main.netMode == Terraria.ID.NetmodeID.Server)
                return;
            
            // Optional: show small text above player
            // CombatText.NewText(Player.Hitbox, Color.Cyan, $"{macroName} ({skillCount})", false, false);
        }
        
        #endregion
        
        #region Macro Management
        
        /// <summary>
        /// Add a skill to a macro
        /// </summary>
        public bool AddSkillToMacro(int macroIndex, string skillId)
        {
            if (macroIndex < 0 || macroIndex >= MAX_MACROS)
                return false;

            if (string.IsNullOrEmpty(skillId))
                return false;

            var skillManager = Player.GetModPlayer<SkillManager>();
            if (skillManager == null || !skillManager.LearnedSkills.ContainsKey(skillId))
                return false;
            
            var macro = Macros[macroIndex];
            if (macro.SkillIds.Count >= MAX_SKILLS_PER_MACRO)
                return false;
            
            if (macro.SkillIds.Contains(skillId))
                return false;
            
            macro.SkillIds.Add(skillId);
            return true;
        }
        
        /// <summary>
        /// Remove a skill from a macro
        /// </summary>
        public bool RemoveSkillFromMacro(int macroIndex, string skillId)
        {
            if (macroIndex < 0 || macroIndex >= MAX_MACROS)
                return false;
            
            return Macros[macroIndex].SkillIds.Remove(skillId);
        }
        
        /// <summary>
        /// Clear all skills from a macro
        /// </summary>
        public void ClearMacro(int macroIndex)
        {
            if (macroIndex < 0 || macroIndex >= MAX_MACROS)
                return;
            
            Macros[macroIndex].SkillIds.Clear();
        }
        
        /// <summary>
        /// Rename a macro
        /// </summary>
        public void RenameMacro(int macroIndex, string newName)
        {
            if (macroIndex < 0 || macroIndex >= MAX_MACROS)
                return;
            
            Macros[macroIndex].Name = newName;
        }
        
        /// <summary>
        /// Set macro execution mode
        /// </summary>
        public void SetMacroMode(int macroIndex, MacroExecutionMode mode)
        {
            if (macroIndex < 0 || macroIndex >= MAX_MACROS)
                return;
            
            Macros[macroIndex].ExecutionMode = mode;
        }
        
        /// <summary>
        /// Get macro info for UI display
        /// </summary>
        public SkillMacro GetMacro(int macroIndex)
        {
            if (macroIndex < 0 || macroIndex >= MAX_MACROS)
                return null;
            
            return Macros[macroIndex];
        }
        
        /// <summary>
        /// Check if macro is on cooldown
        /// </summary>
        public bool IsMacroOnCooldown(int macroIndex)
        {
            if (macroIndex < 0 || macroIndex >= MAX_MACROS)
                return true;
            
            return macroCooldowns[macroIndex] > 0;
        }
        
        /// <summary>
        /// Get remaining cooldown for a macro
        /// </summary>
        public float GetMacroCooldownRemaining(int macroIndex)
        {
            if (macroIndex < 0 || macroIndex >= MAX_MACROS)
                return 0f;
            
            return macroCooldowns[macroIndex] / 60f;
        }
        
        #endregion
        
        #region Save/Load
        
        public override void SaveData(TagCompound tag)
        {
            if (Macros == null)
                return;
                
            var macroData = new List<TagCompound>();
            
            for (int i = 0; i < MAX_MACROS; i++)
            {
                var macro = Macros[i];
                if (macro == null)
                    continue;
                    
                // Filter out null skill IDs
                var validSkillIds = (macro.SkillIds ?? new List<string>())
                    .Where(s => !string.IsNullOrEmpty(s))
                    .ToList();
                    
                var macroTag = new TagCompound
                {
                    ["name"] = macro.Name ?? $"Macro {i + 1}",
                    ["mode"] = (int)macro.ExecutionMode,
                    ["skills"] = validSkillIds
                };
                macroData.Add(macroTag);
            }
            
            tag["macros"] = macroData;
        }
        
        public override void LoadData(TagCompound tag)
        {
            // Ensure Macros array is initialized
            if (Macros == null)
            {
                Macros = new SkillMacro[MAX_MACROS];
                for (int i = 0; i < MAX_MACROS; i++)
                {
                    Macros[i] = new SkillMacro($"Macro {i + 1}");
                }
            }
            
            if (!tag.ContainsKey("macros"))
                return;
            
            var macroData = tag.GetList<TagCompound>("macros");
            
            for (int i = 0; i < Math.Min(macroData.Count, MAX_MACROS); i++)
            {
                var macroTag = macroData[i];
                
                if (Macros[i] == null)
                    Macros[i] = new SkillMacro($"Macro {i + 1}");
                
                Macros[i].Name = macroTag.GetString("name") ?? $"Macro {i + 1}";
                Macros[i].ExecutionMode = (MacroExecutionMode)macroTag.GetInt("mode");
                Macros[i].SkillIds = macroTag.GetList<string>("skills")?.ToList() ?? new List<string>();
            }
        }
        
        #endregion
    }
    
    /// <summary>
    /// Represents a skill macro
    /// </summary>
    public class SkillMacro
    {
        public string Name { get; set; }
        public List<string> SkillIds { get; set; }
        public MacroExecutionMode ExecutionMode { get; set; }
        
        public SkillMacro(string name = "Macro")
        {
            Name = name;
            SkillIds = new List<string>();
            ExecutionMode = MacroExecutionMode.Simultaneous;
        }
        
        /// <summary>
        /// Get skill count in this macro
        /// </summary>
        public int SkillCount => SkillIds?.Count ?? 0;
        
        /// <summary>
        /// Check if macro has any skills
        /// </summary>
        public bool IsEmpty => SkillIds == null || SkillIds.Count == 0;
    }
    
    /// <summary>
    /// How skills in a macro are executed
    /// </summary>
    public enum MacroExecutionMode
    {
        /// <summary>
        /// All skills fire at once
        /// </summary>
        Simultaneous,
        
        /// <summary>
        /// Skills fire one after another (respecting individual cooldowns)
        /// </summary>
        Sequential
    }
}
