using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Rpg.Common.Base
{
    /// <summary>
    /// Centralized asset loading system - ensures textures are loaded once and reused
    /// Call AssetLoader.GetTexture() instead of directly loading assets
    /// Uses vanilla Terraria textures as fallback when mod textures don't exist
    /// </summary>
    public static class AssetLoader
    {
        // Cache for loaded textures
        private static readonly Dictionary<string, Texture2D> _textureCache = new();
        
        // Fallback textures from vanilla Terraria (buff icons)
        private static readonly Dictionary<string, int> _skillToBuffFallback = new()
        {
            // Warrior skills -> vanilla buff icons
            { "PowerStrike", BuffID.Wrath },
            { "Fortify", BuffID.Ironskin },
            { "BattleRage", BuffID.Rage },
            { "WarCry", BuffID.Wrath },
            { "IronWill", BuffID.Endurance },
            { "Charge", BuffID.Swiftness },
            { "GroundSlam", BuffID.Wrath },
            { "Endurance", BuffID.Endurance },
            { "WeaponMastery", BuffID.Sharpened },
            { "VitalForce", BuffID.Regeneration },
            { "ArmorMastery", BuffID.Ironskin },
            { "CombatRegeneration", BuffID.Regeneration },
            
            // Ranger skills
            { "RapidFire", BuffID.Archery },
            { "EvasiveRoll", BuffID.Swiftness },
            { "SteadyAim", BuffID.Archery },
            { "EvasiveManeuver", BuffID.Swiftness },
            { "Precision", BuffID.Archery },
            { "Multishot", BuffID.Archery },
            { "Camouflage", BuffID.Invisibility },
            { "ExplosiveArrow", BuffID.Wrath },
            { "SharpEye", BuffID.Hunter },
            { "FleetFoot", BuffID.Swiftness },
            { "AmmoConservation", BuffID.AmmoReservation },
            { "Agility", BuffID.Swiftness },
            
            // Mage skills
            { "Fireball", BuffID.MagicPower },
            { "ManaShield", BuffID.ManaRegeneration },
            { "ArcaneIntellect", BuffID.Clairvoyance },
            { "MagicBarrier", BuffID.ManaRegeneration },
            { "Blink", BuffID.Swiftness },
            { "ArcanePower", BuffID.MagicPower },
            { "FrostNova", BuffID.Frostburn },
            { "MagicMastery", BuffID.MagicPower },
            { "ManaFlow", BuffID.ManaRegeneration },
            { "SpellEfficiency", BuffID.Clairvoyance },
            
            // Summoner skills
            { "SummonAlly", BuffID.Summoning },
            { "CommandMinions", BuffID.Summoning },
            { "SummonMastery", BuffID.Summoning },
            { "MinionFrenzy", BuffID.Summoning },
            { "GuardianSpirit", BuffID.Summoning },
            { "SoulHarvest", BuffID.Summoning },
            { "SpiritLink", BuffID.Summoning },
            { "CommandFocus", BuffID.Summoning },
            { "MinionMastery", BuffID.Summoning },
            { "WhipMastery", BuffID.Summoning },
            { "SoulBond", BuffID.Summoning },
            { "SpiritEndurance", BuffID.Endurance },
            { "SentryMastery", BuffID.Summoning },
            
            // Knight skills
            { "ShieldWall", BuffID.Ironskin },
            { "Taunt", BuffID.Wrath },
            { "ShieldBash", BuffID.Ironskin },
            { "GuardiansOath", BuffID.Ironskin },
            { "CounterStrike", BuffID.Thorns },
            { "Fortress", BuffID.Ironskin },
            { "ShieldExpertise", BuffID.Ironskin },
            { "Stalwart", BuffID.Endurance },
            { "Provoke", BuffID.Wrath },
            { "Bulwark", BuffID.Ironskin },
            { "HolyGuard", BuffID.Ironskin },
            { "FortressMind", BuffID.Ironskin },
            { "ShieldMastery", BuffID.Ironskin },
            { "Vigilance", BuffID.Hunter },
            { "Retaliation", BuffID.Thorns },
            { "LastStand", BuffID.Endurance },
            
            // Berserker skills
            { "Berserk", BuffID.Rage },
            { "Bloodlust", BuffID.Rage },
            { "LowHPPower", BuffID.Rage },
            { "RagingBlow", BuffID.Wrath },
            { "Whirlwind", BuffID.Wrath },
            { "Frenzy", BuffID.Rage },
            { "UndyingRage", BuffID.Rage },
            { "SavageStrikes", BuffID.Wrath },
            { "RagingStrike", BuffID.Wrath },
            { "Fury", BuffID.Rage },
            { "RecklessPower", BuffID.Rage },
            { "Carnage", BuffID.Wrath },
            
            // Healer/Cleric skills
            { "Heal", BuffID.Regeneration },
            { "HealingPrayer", BuffID.Regeneration },
            { "Blessing", BuffID.Regeneration },
            { "Smite", BuffID.MagicPower },
            { "DivineProtection", BuffID.Endurance },
            { "ManaFont", BuffID.ManaRegeneration },
            { "Sanctuary", BuffID.Endurance },
            { "DivineGrace", BuffID.Regeneration },
            { "HolyLight", BuffID.MagicPower },
            { "DivineShield", BuffID.Endurance },
            { "HolyFortitude", BuffID.Endurance },
            { "HolyArmor", BuffID.Ironskin },
            { "Purify", BuffID.Regeneration },
            { "MassHeal", BuffID.Regeneration },
            { "DivineIntervention", BuffID.Regeneration },
            { "SacredWard", BuffID.Endurance },
            { "SanctifiedBody", BuffID.Regeneration },
            { "DivineBlessing", BuffID.Regeneration },
            
            // Necromancer skills
            { "RaiseUndead", BuffID.Summoning },
            { "SoulDrain", BuffID.MagicPower },
            { "BoneArmor", BuffID.Ironskin },
            { "UndeadMastery", BuffID.Summoning },
            { "RaiseDead", BuffID.Summoning },
            { "DeathCoil", BuffID.MagicPower },
            { "Curse", BuffID.Cursed },
            { "DeathAffinity", BuffID.Summoning },
            { "DarkMastery", BuffID.MagicPower },
            { "UndyingWill", BuffID.Endurance },
            { "UndeadArmy", BuffID.Summoning },
            
            // Default fallback for unknown skills
        };

        #region Texture Loading

        /// <summary>
        /// Get texture from cache or load it. Falls back to vanilla textures if mod texture doesn't exist.
        /// </summary>
        public static Texture2D GetTexture(string path)
        {
            if (_textureCache.TryGetValue(path, out Texture2D texture))
            {
                return texture;
            }

            try
            {
                // Try to load mod texture
                texture = ModContent.Request<Texture2D>(path, ReLogic.Content.AssetRequestMode.ImmediateLoad).Value;
                _textureCache[path] = texture;
                return texture;
            }
            catch
            {
                // Texture doesn't exist, return a fallback
                return GetFallbackTexture(path);
            }
        }
        
        /// <summary>
        /// Get a vanilla Terraria texture as fallback
        /// </summary>
        private static Texture2D GetFallbackTexture(string path)
        {
            // Check if it's a skill icon request
            if (path.Contains("Skills/"))
            {
                string skillName = path.Substring(path.LastIndexOf("/") + 1);
                if (_skillToBuffFallback.TryGetValue(skillName, out int buffId))
                {
                    return TextureAssets.Buff[buffId].Value;
                }
                // Default to a generic buff icon
                return TextureAssets.Buff[BuffID.Regeneration].Value;
            }
            
            // Check if it's a job icon request
            if (path.Contains("Jobs/"))
            {
                // Use item textures for jobs
                return TextureAssets.Item[ItemID.Book].Value;
            }
            
            // Check if it's a stat icon request
            if (path.Contains("Stats/"))
            {
                return TextureAssets.Buff[BuffID.WellFed].Value;
            }
            
            // Default fallback - use a common vanilla texture
            return TextureAssets.MagicPixel.Value;
        }

        /// <summary>
        /// Preload a texture into cache
        /// </summary>
        public static void PreloadTexture(string path)
        {
            if (!_textureCache.ContainsKey(path))
            {
                GetTexture(path);
            }
        }

        /// <summary>
        /// Unload a specific texture from cache
        /// </summary>
        public static void UnloadTexture(string path)
        {
            if (_textureCache.ContainsKey(path))
            {
                _textureCache.Remove(path);
            }
        }

        /// <summary>
        /// Clear entire texture cache
        /// </summary>
        public static void ClearCache()
        {
            _textureCache.Clear();
        }

        #endregion

        #region Specific Asset Loaders (Convenience)

        /// <summary>
        /// Load skill icon
        /// </summary>
        public static Texture2D GetSkillIcon(string skillName)
        {
            return GetTexture($"{RpgConstants.ICON_PATH}Skills/{skillName}");
        }

        /// <summary>
        /// Load job icon
        /// </summary>
        public static Texture2D GetJobIcon(JobType jobType)
        {
            return GetTexture($"{RpgConstants.ICON_PATH}Jobs/{jobType}");
        }

        /// <summary>
        /// Load stat icon
        /// </summary>
        public static Texture2D GetStatIcon(StatType statType)
        {
            return GetTexture($"{RpgConstants.ICON_PATH}Stats/{statType}");
        }

        /// <summary>
        /// Load particle texture
        /// </summary>
        public static Texture2D GetParticleTexture(ParticleType particleType)
        {
            return GetTexture($"{RpgConstants.PARTICLE_PATH}{particleType}");
        }

        /// <summary>
        /// Load UI element
        /// </summary>
        public static Texture2D GetUITexture(string elementName)
        {
            return GetTexture($"{RpgConstants.TEXTURE_PATH}UI/{elementName}");
        }

        #endregion

        #region Batch Loading

        /// <summary>
        /// Preload all skill icons
        /// </summary>
        public static void PreloadAllSkillIcons()
        {
            // Get all skill names and preload their icons
            foreach (string skillName in Skills.SkillDatabase.GetAllSkillNames())
            {
                try
                {
                    GetSkillIcon(skillName);
                }
                catch
                {
                    // Icon doesn't exist yet, skip
                }
            }
        }

        /// <summary>
        /// Preload all job icons
        /// </summary>
        public static void PreloadAllJobIcons()
        {
            foreach (JobType jobType in System.Enum.GetValues(typeof(JobType)))
            {
                try
                {
                    GetJobIcon(jobType);
                }
                catch
                {
                    // Icon doesn't exist yet, skip
                }
            }
        }

        /// <summary>
        /// Preload common UI elements
        /// </summary>
        public static void PreloadCommonUI()
        {
            string[] commonElements = 
            {
                "Button",
                "ButtonHover",
                "Panel",
                "ProgressBar",
                "ProgressBarFill",
                "Divider",
                "Background"
            };

            foreach (string element in commonElements)
            {
                try
                {
                    GetUITexture(element);
                }
                catch
                {
                    // Element doesn't exist yet, skip
                }
            }
        }

        #endregion

        #region Utility

        /// <summary>
        /// Check if texture exists in cache
        /// </summary>
        public static bool IsTextureLoaded(string path)
        {
            return _textureCache.ContainsKey(path);
        }

        /// <summary>
        /// Get number of cached textures
        /// </summary>
        public static int GetCachedTextureCount()
        {
            return _textureCache.Count;
        }

        #endregion
    }
}
