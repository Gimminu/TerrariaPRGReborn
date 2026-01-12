using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameInput;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Rpg.Common.Base;
using Rpg.Common.Systems;

namespace Rpg.Common.Skills
{
    /// <summary>
    /// Manages player skills: learning, cooldowns, passive effects, hotbar use.
    /// </summary>
    public class SkillManager : ModPlayer
    {
        public Dictionary<string, BaseSkill> LearnedSkills { get; private set; } = new();
        public string[] SkillHotbar { get; private set; } = new string[9];

        public const string MacroHotbarPrefix = "macro:";
        
        // Performance: Cache passive skills separately to avoid type checks every frame
        private List<BaseSkill> _cachedPassiveSkills = new();
        private bool _passiveCacheDirty = true;

        public override void Initialize()
        {
            LearnedSkills.Clear();
            SkillHotbar = new string[9];
            _cachedPassiveSkills.Clear();
            _passiveCacheDirty = true;
        }

        public override void ResetEffects()
        {
            // Rebuild passive cache if dirty
            if (_passiveCacheDirty)
            {
                _cachedPassiveSkills.Clear();
                foreach (var skill in LearnedSkills.Values)
                {
                    if (skill.SkillType == SkillType.Passive)
                        _cachedPassiveSkills.Add(skill);
                }
                _passiveCacheDirty = false;
            }
            
            // Apply cached passive skills (no type check needed)
            for (int i = 0; i < _cachedPassiveSkills.Count; i++)
            {
                _cachedPassiveSkills[i].ApplyPassive(Player);
            }
        }

        public override void PostUpdate()
        {
            // Update cooldowns for all learned skills.
            foreach (var skill in LearnedSkills.Values)
            {
                skill.UpdateCooldown();
            }
        }

        public override void ProcessTriggers(TriggersSet triggersSet)
        {
            if (SkillHotkeySystem.SkillKeybinds == null || SkillHotkeySystem.SkillKeybinds.Length < 9)
                return;

            for (int i = 0; i < 9; i++)
            {
                if (SkillHotkeySystem.SkillKeybinds[i].JustPressed)
                {
                    UseSkillInSlot(i);
                }
            }
        }

        /// <summary>
        /// Learn or upgrade a skill (spends skill points).
        /// </summary>
        public bool LearnSkill(BaseSkill skill)
        {
            if (skill == null)
            {
                ShowMessage("Skill not found.", Color.Gray);
                return false;
            }

            var rpgPlayer = Player.GetModPlayer<Players.RpgPlayer>();

            if (LearnedSkills.TryGetValue(skill.InternalName, out var learnedSkill))
            {
                if (!learnedSkill.CanLearn(Player))
                {
                    ShowMessage("Cannot upgrade this skill yet.", Color.Orange);
                    return false;
                }

                rpgPlayer.SkillPoints -= learnedSkill.SkillPointCost;
                learnedSkill.OnLearn(Player, learnedSkill.CurrentRank + 1);
                ShowMessage($"{learnedSkill.DisplayName} upgraded to Rank {learnedSkill.CurrentRank}!", Color.LightGreen);
                return true;
            }

            if (!skill.CanLearn(Player))
            {
                ShowMessage("Cannot learn this skill yet.", Color.Orange);
                return false;
            }

            rpgPlayer.SkillPoints -= skill.SkillPointCost;
            skill.OnLearn(Player, 1);
            LearnedSkills.Add(skill.InternalName, skill);
            
            // Mark passive cache dirty when learning new skills
            if (skill.SkillType == SkillType.Passive)
                _passiveCacheDirty = true;
                
            ShowMessage($"Learned {skill.DisplayName}!", Color.LightGreen);
            return true;
        }

        /// <summary>
        /// Assign skill to hotbar slot.
        /// </summary>
        public void AssignSkillToSlot(string skillName, int slot)
        {
            if (slot < 0 || slot >= 9)
                return;

            if (!LearnedSkills.ContainsKey(skillName))
            {
                ShowMessage("You haven't learned this skill yet!", Color.OrangeRed);
                return;
            }

            SkillHotbar[slot] = skillName;
            ShowMessage($"{LearnedSkills[skillName].DisplayName} assigned to slot {slot + 1}", Color.LightBlue);
        }

        public bool AssignMacroToSlot(int macroIndex, int slot)
        {
            if (slot < 0 || slot >= 9)
                return false;

            var macroSystem = Player.GetModPlayer<SkillMacroSystem>();
            if (macroSystem == null)
                return false;

            if (macroIndex < 0 || macroIndex >= SkillMacroSystem.MAX_MACROS)
                return false;

            var macro = macroSystem.GetMacro(macroIndex);
            if (macro == null || macro.SkillIds.Count == 0)
            {
                ShowMessage("Macro is empty.", Color.OrangeRed);
                return false;
            }

            SkillHotbar[slot] = BuildMacroEntry(macroIndex);
            ShowMessage($"{macro.Name} assigned to slot {slot + 1}", Color.LightBlue);
            return true;
        }

        /// <summary>
        /// Use skill in hotbar slot.
        /// </summary>
        public void UseSkillInSlot(int slot)
        {
            if (slot < 0 || slot >= 9)
                return;

            string skillName = SkillHotbar[slot];
            if (string.IsNullOrEmpty(skillName))
                return;

            if (TryParseMacroEntry(skillName, out int macroIndex))
            {
                var macroSystem = Player.GetModPlayer<SkillMacroSystem>();
                if (macroSystem == null || !macroSystem.ExecuteMacro(macroIndex))
                {
                    ShowMessage("Macro cannot be used.", Color.Gray);
                }
                return;
            }

            if (!LearnedSkills.ContainsKey(skillName))
            {
                SkillHotbar[slot] = null;
                return;
            }

            UseSkill(skillName);
        }

        public static string BuildMacroEntry(int macroIndex)
        {
            return $"{MacroHotbarPrefix}{macroIndex}";
        }

        public static bool TryParseMacroEntry(string entry, out int macroIndex)
        {
            macroIndex = -1;
            if (string.IsNullOrEmpty(entry) || !entry.StartsWith(MacroHotbarPrefix))
                return false;

            string value = entry.Substring(MacroHotbarPrefix.Length);
            return int.TryParse(value, out macroIndex);
        }

        /// <summary>
        /// Use skill by internal name.
        /// </summary>
        public void UseSkill(string skillName)
        {
            if (!LearnedSkills.TryGetValue(skillName, out var skill))
            {
                ShowMessage($"Skill not learned: {skillName}", Color.OrangeRed);
                return;
            }

            if (skill.SkillType == SkillType.Passive)
            {
                ShowMessage($"{skill.DisplayName} is a passive skill.", Color.Gray);
                return;
            }

            if (skill.CurrentCooldown > 0f)
            {
                ShowMessage($"{skill.DisplayName} on cooldown: {skill.CurrentCooldown:F1}s", Color.Khaki);
                return;
            }

            if (!HasEnoughResource(skill))
            {
                return;
            }

            if (!skill.CanUse(Player))
            {
                return;
            }

            skill.Activate(Player);
        }

        private bool HasEnoughResource(BaseSkill skill)
        {
            var rpgPlayer = Player.GetModPlayer<Players.RpgPlayer>();

            switch (skill.ResourceType)
            {
                case ResourceType.Stamina:
                    if (rpgPlayer.Stamina < skill.ResourceCost)
                    {
                        ShowMessage("Not enough stamina!", Color.OrangeRed);
                        return false;
                    }
                    break;
                case ResourceType.Mana:
                    if (Player.statMana < skill.ResourceCost)
                    {
                        ShowMessage("Not enough mana!", Color.OrangeRed);
                        return false;
                    }
                    break;
                case ResourceType.Life:
                    if (Player.statLife < skill.ResourceCost)
                    {
                        ShowMessage("Not enough life!", Color.OrangeRed);
                        return false;
                    }
                    break;
            }

            return true;
        }

        public override void SaveData(TagCompound tag)
        {
            // Ensure collections are not null before saving
            if (LearnedSkills != null && LearnedSkills.Count > 0)
            {
                // Filter out any null or empty keys
                var validNames = LearnedSkills.Keys.Where(k => !string.IsNullOrEmpty(k)).ToList();
                var validRanks = validNames.Select(k => LearnedSkills[k].CurrentRank).ToList();
                
                tag["LearnedSkillNames"] = validNames;
                tag["LearnedSkillRanks"] = validRanks;
            }
            
            if (SkillHotbar != null)
            {
                // Replace null with empty string for safe saving
                tag["SkillHotbar"] = SkillHotbar.Select(s => s ?? "").ToList();
            }
        }

        public override void LoadData(TagCompound tag)
        {
            // Ensure collections exist
            if (LearnedSkills == null)
                LearnedSkills = new Dictionary<string, BaseSkill>();
            else
                LearnedSkills.Clear();
                
            if (SkillHotbar == null)
                SkillHotbar = new string[9];
            
            _passiveCacheDirty = true; // Force rebuild on load

            if (tag.ContainsKey("LearnedSkillNames"))
            {
                var names = tag.GetList<string>("LearnedSkillNames");
                var ranks = tag.ContainsKey("LearnedSkillRanks") ? tag.GetList<int>("LearnedSkillRanks") : new List<int>();

                for (int i = 0; i < names.Count; i++)
                {
                    if (string.IsNullOrEmpty(names[i]))
                        continue;
                        
                    BaseSkill skill = SkillDatabase.GetSkill(names[i]);
                    if (skill == null)
                        continue;

                    int rank = ranks.Count > i ? ranks[i] : 1;
                    rank = Utils.Clamp(rank, 1, skill.MaxRank);
                    skill.OnLearn(Player, rank);
                    LearnedSkills[skill.InternalName] = skill;
                }
            }

            if (tag.ContainsKey("SkillHotbar"))
            {
                var hotbar = tag.GetList<string>("SkillHotbar");
                for (int i = 0; i < hotbar.Count && i < 9; i++)
                {
                    SkillHotbar[i] = string.IsNullOrEmpty(hotbar[i]) ? null : hotbar[i];
                }
            }
        }

        private void ShowMessage(string text, Color color)
        {
            if (Main.netMode != NetmodeID.Server)
            {
                Main.NewText(text, color);
            }
        }
        
        #region Reset System
        
        /// <summary>
        /// Get total allocated skill points across all learned skills
        /// </summary>
        public int GetTotalAllocatedPoints()
        {
            int total = 0;
            foreach (var skill in LearnedSkills.Values)
            {
                // Each rank costs the skill's base cost
                total += skill.CurrentRank * skill.SkillPointCost;
            }
            return total;
        }
        
        /// <summary>
        /// Reset all learned skills and return total points refunded
        /// </summary>
        public int ResetAllSkills()
        {
            int totalRefunded = GetTotalAllocatedPoints();
            
            // Clear all learned skills
            LearnedSkills.Clear();
            
            // Clear hotbar
            for (int i = 0; i < SkillHotbar.Length; i++)
            {
                SkillHotbar[i] = null;
            }
            
            // Mark cache as dirty
            _passiveCacheDirty = true;
            _cachedPassiveSkills.Clear();
            
            return totalRefunded;
        }
        
        #endregion
    }
}
