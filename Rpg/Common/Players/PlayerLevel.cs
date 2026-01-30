using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using System;
using Rpg.Common.Config;
using Rpg.Common.UI;
using Rpg.Common.Systems;
using Rpg.Common.Jobs;

namespace Rpg.Common.Players
{
    /// <summary>
    /// Handles all leveling logic - XP gain, level-up, point awards
    /// Separated from RpgPlayer for clean organization
    /// </summary>
    public class PlayerLevel : ModPlayer
    {
        private RpgPlayer rpgPlayer;
        
        // Sweet spot combo tracking (스위트 스팟 콤보 시스템)
        private int sweetSpotComboCount = 0;
        private int lastSweetSpotFrame = 0;
        private const int COMBO_TIMEOUT_FRAMES = 600; // 10 seconds

        public override void Initialize()
        {
            rpgPlayer = Player.GetModPlayer<RpgPlayer>();
            sweetSpotComboCount = 0;
            lastSweetSpotFrame = 0;
        }

        #region XP Gain

        /// <summary>
        /// Award XP to the player with all multipliers applied
        /// Per design doc: XP is throttled near level cap
        /// </summary>
        public void GainExperience(long baseXP, XPSource source = XPSource.Monster, int monsterLevel = 0, bool fromNetwork = false)
        {
            if (Main.netMode == NetmodeID.MultiplayerClient && !fromNetwork)
                return;

            // Check if at level cap
            int maxLevel = RpgFormulas.GetMaxLevel();
            if (rpgPlayer.Level >= maxLevel)
            {
                // At cap, can't gain XP
                return;
            }
            
            // Anti-exploit: Check if player can receive XP from this monster level
            if (monsterLevel > 0 && !BiomeLevelSystem.CanReceiveXPFromMonster(rpgPlayer.Level, monsterLevel))
            {
                // Monster too far above player level (anti-boosting)
                ShowXPGainEffect(0, "Too High Level!", Color.Red);
                return;
            }

            // Fix for low level monsters giving 0 XP due to low HP scaling
            if (source == XPSource.Monster && monsterLevel > 0)
            {
                long minXP = BiomeLevelSystem.GetMinimumXP(monsterLevel);
                if (baseXP < minXP)
                    baseXP = minXP;
            }

            // Apply player's XP multiplier (from buffs/items)
            long finalXP = (long)(baseXP * rpgPlayer.ExpMultiplier);

            // Apply source-specific bonus
            switch (source)
            {
                case XPSource.Boss:
                    finalXP = (long)(finalXP * RpgConstants.BOSS_XP_BONUS_MULTIPLIER);
                    break;
                case XPSource.Event:
                    finalXP = (long)(finalXP * RpgConstants.EVENT_XP_MULTIPLIER);
                    break;
                case XPSource.Quest:
                    // Quest XP already balanced, no multiplier
                    break;
            }

            // Apply global XP multiplier from config
            var serverConfig = ModContent.GetInstance<RpgServerConfig>();
            if (serverConfig != null)
            {
                finalXP = (long)(finalXP * serverConfig.GlobalXPMultiplier);
            }
            
            // Apply level difference XP modifier (anti-farming for both high and low level monsters)
            float levelDiffMultiplier = 1.0f;
            bool isSweetSpot = false;
            
            if (monsterLevel > 0)
            {
                levelDiffMultiplier = BiomeLevelSystem.GetLevelDifferenceXPMultiplier(rpgPlayer.Level, monsterLevel);
                finalXP = (long)(finalXP * levelDiffMultiplier);
                
                // Check if this is a sweet spot kill (권장 난이도)
                int levelDiff = monsterLevel - rpgPlayer.Level;
                isSweetSpot = (levelDiff >= RpgConstants.LEVEL_DIFF_SWEETSPOT_START && 
                              levelDiff <= RpgConstants.LEVEL_DIFF_SWEETSPOT_END);
                
                // Apply sweet spot combo bonus (연속 처치 보너스)
                if (isSweetSpot)
                {
                    UpdateSweetSpotCombo();
                    float comboBonus = GetSweetSpotComboBonus();
                    if (comboBonus > 1.0f)
                    {
                        finalXP = (long)(finalXP * comboBonus);
                    }
                }
                else
                {
                    // Reset combo if not in sweet spot
                    ResetSweetSpotCombo();
                }
            }
            
            // Apply level cap proximity modifiers (레벨 캡 근접 보정)
            finalXP = ApplyLevelCapModifiers(finalXP, maxLevel);

            long minFinalXP = BiomeLevelSystem.GetMinimumXP(Math.Max(monsterLevel, 1));
            finalXP = Math.Max(finalXP, minFinalXP);

            // Add to current XP
            rpgPlayer.CurrentXP += finalXP;

            // Show XP gain visual with level difference info
            ShowXPGainEffectWithContext(finalXP, monsterLevel, levelDiffMultiplier, isSweetSpot);

            // Check for level-up
            CheckLevelUp();

            if (Main.netMode == NetmodeID.Server)
            {
                SendXPGainPacket(baseXP, source, monsterLevel);
            }
        }
        
        #region Sweet Spot Combo System
        
        /// <summary>
        /// Update sweet spot combo counter (스위트 스팟 콤보 업데이트)
        /// </summary>
        private void UpdateSweetSpotCombo()
        {
            int currentFrame = (int)Main.GameUpdateCount;
            
            // Check if combo timed out
            if (currentFrame - lastSweetSpotFrame > COMBO_TIMEOUT_FRAMES)
            {
                sweetSpotComboCount = 1;
            }
            else
            {
                sweetSpotComboCount++;
            }
            
            lastSweetSpotFrame = currentFrame;
        }
        
        /// <summary>
        /// Reset sweet spot combo (콤보 리셋)
        /// </summary>
        private void ResetSweetSpotCombo()
        {
            sweetSpotComboCount = 0;
        }
        
        /// <summary>
        /// Get sweet spot combo bonus multiplier (콤보 보너스 배율)
        /// </summary>
        private float GetSweetSpotComboBonus()
        {
            if (sweetSpotComboCount < RpgConstants.SWEETSPOT_COMBO_THRESHOLD)
                return 1.0f;
            
            // Linear scaling: 5 kills = 1.1x, 10 kills = 1.2x, ... max 1.5x at 25+ kills
            float bonus = 1.0f + ((sweetSpotComboCount - RpgConstants.SWEETSPOT_COMBO_THRESHOLD) * 0.02f);
            return Math.Min(bonus, RpgConstants.SWEETSPOT_COMBO_MAX_BONUS);
        }
        
        #endregion
        
        /// <summary>
        /// Apply level cap proximity modifiers (따라잡기 보너스 & 캡 근접 감소)
        /// </summary>
        private long ApplyLevelCapModifiers(long xp, int maxLevel)
        {
            int levelsToGo = maxLevel - rpgPlayer.Level;
            
            // Catch-up bonus: 레벨 캡에서 많이 떨어져있으면 보너스
            if (levelsToGo >= RpgConstants.LEVEL_CAP_CATCHUP_RANGE)
            {
                xp = (long)(xp * RpgConstants.LEVEL_CAP_CATCHUP_BONUS); // 120% XP
            }
            // Throttle: 레벨 캡에 가까우면 감소
            else if (levelsToGo <= RpgConstants.LEVEL_CAP_THROTTLE_RANGE && levelsToGo > 0)
            {
                xp = (long)(xp * RpgConstants.LEVEL_CAP_THROTTLE_MULT); // 80% XP
            }
            // At cap: block XP gain
            else if (levelsToGo <= 0)
            {
                return 0;
            }
            
            return xp;
        }

        /// <summary>
        /// Continuously check for level-up (can level multiple times)
        /// </summary>
        private void CheckLevelUp()
        {
            while (rpgPlayer.CurrentXP >= rpgPlayer.RequiredXP)
            {
                // Check if at cap
                int maxLevel = RpgFormulas.GetMaxLevel();
                if (rpgPlayer.Level >= maxLevel)
                {
                    // Per design doc: XP stops at 99% at cap
                    rpgPlayer.CurrentXP = rpgPlayer.RequiredXP - 1;
                    return;
                }

                LevelUp();
            }
        }

        #endregion

        #region Level-Up

        /// <summary>
        /// Level up the player - award stat/skill points, play effects, apply auto-growth
        /// </summary>
        private void LevelUp()
        {
            // Deduct XP
            rpgPlayer.CurrentXP -= rpgPlayer.RequiredXP;

            // Increase level
            rpgPlayer.Level++;

            var serverConfig = ModContent.GetInstance<RpgServerConfig>();
            bool autoGrowthEnabled = serverConfig == null || serverConfig.EnableAutoGrowth;

            // Apply auto-growth FIRST (30~40% of total growth)
            if (autoGrowthEnabled)
            {
                AutoGrowth.ApplyAutoGrowth(rpgPlayer);
            }

            // Award manual stat points (60~70% of total growth)
            float statMultiplier = serverConfig?.StatPointMultiplier ?? 1f;
            int statPoints = (int)System.Math.Round(RpgFormulas.GetStatPointsPerLevel(rpgPlayer.CurrentTier) * statMultiplier);
            statPoints = System.Math.Max(0, statPoints);

            // Award skill points
            float skillMultiplier = serverConfig?.SkillPointMultiplier ?? 1f;
            int skillPoints = 0;
            if (serverConfig == null || serverConfig.EnableSkillSystem)
            {
                skillPoints = (int)System.Math.Round(RpgFormulas.GetSkillPointsPerLevel(rpgPlayer.CurrentTier) * skillMultiplier);
                skillPoints = System.Math.Max(0, skillPoints);
            }

            bool storePending = rpgPlayer.Level > RpgFormulas.GetTierMaxLevel(rpgPlayer.CurrentTier);
            if (storePending)
            {
                rpgPlayer.PendingStatPoints += statPoints;
                rpgPlayer.PendingSkillPoints += skillPoints;
            }
            else
            {
                rpgPlayer.StatPoints += statPoints;
                rpgPlayer.SkillPoints += skillPoints;
            }

            EnsureTierSkillPointMinimum();

            // Full heal on level-up
            Player.statLife = Player.statLifeMax2;
            Player.statMana = Player.statManaMax2;
            rpgPlayer.Stamina = rpgPlayer.MaxStamina;

            // Check level achievements
            var achievements = Player.GetModPlayer<AchievementSystem>();
            achievements?.CheckLevelAchievements(rpgPlayer.Level);

            // Add level-based quests
            AddLevelQuests();

            // Play level-up effects
            PlayLevelUpEffects();

            // Show level-up message
            ShowLevelUpMessage();

            // Check for job advancement availability
            CheckJobAdvancement();
        }

        private void AddLevelQuests()
        {
            // Add quests based on level milestones
            if (rpgPlayer.Level == 5)
            {
                var quest = new KillQuest(
                    "First Steps",
                    "Defeat 10 slimes to prove your worth",
                    "Slime",
                    10,
                    500,
                    1,
                    0
                );
                QuestSystem.AddQuest(quest, Player.whoAmI);
            }
            else if (rpgPlayer.Level == 10)
            {
                var quest = new KillQuest(
                    "Monster Hunter",
                    "Slay 5 zombies",
                    "Zombie",
                    5,
                    800,
                    1,
                    1
                );
                QuestSystem.AddQuest(quest, Player.whoAmI);
            }
            else if (rpgPlayer.Level == 15)
            {
                var quest = new KillQuest(
                    "Eye See You",
                    "Defeat the Eye of Cthulhu",
                    "Eye of Cthulhu",
                    1,
                    2000,
                    2,
                    1
                );
                QuestSystem.AddQuest(quest, Player.whoAmI);
            }
        }

        private void EnsureTierSkillPointMinimum()
        {
            int tierMaxLevel = RpgFormulas.GetTierMaxLevel(rpgPlayer.CurrentTier);
            if (tierMaxLevel == int.MaxValue || rpgPlayer.Level != tierMaxLevel)
                return;

            int required = Skills.SkillDatabase.GetTotalSkillPointCostForLineage(
                rpgPlayer.CurrentJob,
                rpgPlayer.CurrentTier,
                tierMaxLevel);

            if (required <= 0)
                return;

            int allocated = Player.GetModPlayer<Skills.SkillManager>()?.GetTotalAllocatedPoints() ?? 0;
            int total = allocated + rpgPlayer.SkillPoints + rpgPlayer.PendingSkillPoints;
            if (total >= required)
                return;

            rpgPlayer.SkillPoints += required - total;
        }

        #endregion

        #region Job System

        /// <summary>
        /// Check if player can advance to next job tier
        /// </summary>
        private void CheckJobAdvancement()
        {
            var availableJobs = JobDatabase.GetAvailableJobs(rpgPlayer);
            if (availableJobs != null && availableJobs.Count > 0)
            {
                ShowJobAdvancementAvailable();
            }
        }

        /// <summary>
        /// Advance to a new job (called from UI)
        /// </summary>
        public bool AdvanceJob(JobType newJob)
        {
            JobTier oldTier = rpgPlayer.CurrentTier;
            JobTier newTier = RpgFormulas.GetJobTier(newJob);

            // Validate condition requirements
            if (!JobDatabase.CanUnlockJob(rpgPlayer, newJob))
                return false;

            // Validate job tier progression
            if (newTier != rpgPlayer.CurrentTier + 1)
                return false;

            // Change job
            rpgPlayer.CurrentJob = newJob;

            if (newTier > oldTier)
            {
                rpgPlayer.ReleasePendingPoints();
            }

            rpgPlayer.RequestJobSync();

            // Play job change effects
            PlayJobChangeEffects();

            // Show message
            ShowJobChangeMessage(newJob);

            return true;
        }

        #endregion

        #region Visual Effects

        /// <summary>
        /// Show XP gain floating text with context (레벨 차이 정보 포함)
        /// </summary>
        private void ShowXPGainEffectWithContext(long xpAmount, int monsterLevel, float multiplier, bool isSweetSpot)
        {
            if (Main.netMode == NetmodeID.Server)
                return;

            var clientConfig = ModContent.GetInstance<RpgClientConfig>();
            if (clientConfig != null && !clientConfig.ShowXPGainText)
                return;

            // Determine color based on multiplier and sweet spot
            Color xpColor;
            string suffix = "";
            
            if (isSweetSpot)
            {
                // Sweet spot: bright gold with combo indicator
                xpColor = new Color(255, 223, 0); // Bright gold
                if (sweetSpotComboCount >= RpgConstants.SWEETSPOT_COMBO_THRESHOLD)
                {
                    suffix = $" x{sweetSpotComboCount}";
                    xpColor = new Color(255, 165, 0); // Orange for combo
                }
                else
                {
                    suffix = " ★"; // Sweet spot indicator
                }
            }
            else if (multiplier >= 1.0f)
            {
                xpColor = new Color(255, 215, 0); // Gold (normal/bonus)
            }
            else if (multiplier >= 0.5f)
            {
                xpColor = new Color(200, 200, 100); // Dim yellow (reduced)
            }
            else
            {
                xpColor = new Color(150, 150, 150); // Gray (very low)
            }

            // Show multiplier if not 100%
            string multiplierText = "";
            if (Math.Abs(multiplier - 1.0f) > 0.01f)
            {
                int percentage = (int)(multiplier * 100);
                multiplierText = $" ({percentage}%)";
            }

            // Create floating combat text
            CombatText.NewText(Player.Hitbox, xpColor, $"+{xpAmount} XP{multiplierText}{suffix}", false, false);
            
            // Show level difference warning/encouragement
            if (monsterLevel > 0)
            {
                int levelDiff = monsterLevel - rpgPlayer.Level;
                string diffText = BiomeLevelSystem.GetLevelDifficultyText(monsterLevel, rpgPlayer.Level);
                
                if (levelDiff >= 10)
                {
                    ShowXPGainEffect(0, "Too Dangerous!", new Color(255, 100, 100));
                }
                else if (isSweetSpot && sweetSpotComboCount >= RpgConstants.SWEETSPOT_COMBO_THRESHOLD)
                {
                    ShowXPGainEffect(0, $"Combo x{sweetSpotComboCount}!", new Color(255, 165, 0));
                }
                else if (isSweetSpot && sweetSpotComboCount == 1)
                {
                    ShowXPGainEffect(0, "Perfect Challenge!", new Color(255, 223, 0));
                }
            }
        }

        /// <summary>
        /// Show XP gain floating text (simple version)
        /// </summary>
        private void ShowXPGainEffect(long xpAmount, string customText = null, Color? customColor = null)
        {
            if (Main.netMode == NetmodeID.Server)
                return;

            var clientConfig = ModContent.GetInstance<RpgClientConfig>();
            if (clientConfig != null && !clientConfig.ShowXPGainText)
                return;

            // Create floating combat text
            Color xpColor = customColor ?? new Color(255, 215, 0); // Gold
            string text = customText ?? $"+{xpAmount} XP";
            CombatText.NewText(Player.Hitbox, xpColor, text, false, false);
        }

        /// <summary>
        /// Play level-up visual and audio effects
        /// </summary>
        private void PlayLevelUpEffects()
        {
            if (Main.netMode == NetmodeID.Server)
                return;

            // Enhanced sound effect - multiple sounds for epic feel
            SoundEngine.PlaySound(SoundID.Item4, Player.position);
            SoundEngine.PlaySound(SoundID.MenuOpen, Player.position); // Add fanfare sound

            // More spectacular dust effect - golden sparkles
            for (int i = 0; i < 80; i++)
            {
                Vector2 velocity = Main.rand.NextVector2Circular(12f, 12f);
                int dustType = Main.rand.NextBool() ? DustID.GoldCoin : DustID.Firework_Green;
                Dust dust = Dust.NewDustPerfect(
                    Player.Center,
                    dustType,
                    velocity,
                    Scale: Main.rand.NextFloat(1.2f, 2.0f)
                );
                dust.noGravity = true;
                dust.fadeIn = 0.5f;
            }

            // Add star particles for RPG feel
            for (int i = 0; i < 20; i++)
            {
                Vector2 velocity = Main.rand.NextVector2Circular(6f, 6f);
                Dust dust = Dust.NewDustPerfect(
                    Player.Center,
                    DustID.MagicMirror,
                    velocity,
                    Scale: Main.rand.NextFloat(0.8f, 1.5f)
                );
                dust.noGravity = true;
                dust.color = Color.Yellow;
            }

            // Screen flash effect for dramatic level up
            // Main.flashAlpha = 1f; // Brief white flash (removed for compatibility)
        }

        /// <summary>
        /// Show level-up message
        /// </summary>
        private void ShowLevelUpMessage()
        {
            string message = $"★ LEVEL UP! ★ Now level {rpgPlayer.Level}";
            Color levelColor = new Color(255, 255, 150); // Bright golden yellow
            
            if (Main.netMode != NetmodeID.Server)
            {
                Main.NewText(message, levelColor);
                
                // Additional celebratory messages based on level milestones
                if (rpgPlayer.Level % 10 == 0)
                {
                    Main.NewText("🎉 Major Level Milestone! 🎉", new Color(255, 100, 255));
                }
                else if (rpgPlayer.Level % 5 == 0)
                {
                    Main.NewText("✨ Significant Level Up! ✨", new Color(100, 255, 255));
                }
            }
        }

        /// <summary>
        /// Show job advancement available notification
        /// </summary>
        private void ShowJobAdvancementAvailable()
        {
            string message = "You can now advance to a new job class!";
            Color jobColor = new Color(100, 255, 255); // Cyan
            
            if (Main.netMode != NetmodeID.Server)
            {
                Main.NewText(message, jobColor);
            }
        }

        /// <summary>
        /// Play job change effects
        /// </summary>
        private void PlayJobChangeEffects()
        {
            if (Main.netMode == NetmodeID.Server)
                return;

            // Sound
            SoundEngine.PlaySound(SoundID.Item29, Player.position);

            // Visual effect
            for (int i = 0; i < 100; i++)
            {
                Vector2 velocity = Main.rand.NextVector2Circular(12f, 12f);
                Dust dust = Dust.NewDustPerfect(
                    Player.Center,
                    DustID.TintableDust,
                    velocity,
                    newColor: Color.Cyan,
                    Scale: 2f
                );
                dust.noGravity = true;
            }
        }

        /// <summary>
        /// Show job change message
        /// </summary>
        private void ShowJobChangeMessage(JobType newJob)
        {
            string message = $"Job changed to: {newJob}";
            Color jobColor = new Color(100, 255, 255);
            
            if (Main.netMode != NetmodeID.Server)
            {
                Main.NewText(message, jobColor);
            }
        }

        #endregion

        #region Multiplayer Support

        /// <summary>
        /// Get average player level within XP share radius
        /// </summary>
        public static int GetAveragePlayerLevel(Vector2 position, float radius)
        {
            int totalLevels = 0;
            int playersInRange = 0;

            foreach (Player player in Main.player)
            {
                if (!player.active || player.dead)
                    continue;

                float distance = Vector2.Distance(player.Center, position);
                if (distance <= radius)
                {
                    totalLevels += player.GetModPlayer<RpgPlayer>().Level;
                    playersInRange++;
                }
            }

            if (playersInRange == 0)
                return 1;

            return totalLevels / playersInRange;
        }

        /// <summary>
        /// Distribute XP to nearby players (5000 tile radius)
        /// </summary>
        public static void DistributeXPToNearbyPlayers(Vector2 position, long baseXP, XPSource source, int monsterLevel = 0)
        {
            var serverConfig = ModContent.GetInstance<RpgServerConfig>();
            float shareRadius = serverConfig?.MultiplayerXPShareRange ?? RpgConstants.MULTIPLAYER_XP_SHARE_RADIUS;

            // Find all players within radius
            int playersInRange = 0;
            foreach (Player player in Main.player)
            {
                if (!player.active || player.dead)
                    continue;

                float distance = Vector2.Distance(player.Center, position);
                if (distance <= shareRadius)
                {
                    playersInRange++;
                }
            }

            if (playersInRange == 0)
                return;

            // Award XP to all players
            foreach (Player player in Main.player)
            {
                if (!player.active || player.dead)
                    continue;

                float distance = Vector2.Distance(player.Center, position);
                if (distance <= shareRadius)
                {
                    PlayerLevel levelSystem = player.GetModPlayer<PlayerLevel>();
                    levelSystem.GainExperience(baseXP, source, monsterLevel);
                }
            }
        }
        
        /// <summary>
        /// Distribute XP to specific players who dealt damage
        /// Used for trap/lava kill handling - only players who damaged the NPC get XP
        /// </summary>
        public static void DistributeXPToSpecificPlayers(System.Collections.Generic.List<int> playerIndices, long baseXP, XPSource source, int monsterLevel = 0)
        {
            if (playerIndices == null || playerIndices.Count == 0)
                return;
            
            // Award XP to each player who dealt damage
            foreach (int playerIndex in playerIndices)
            {
                if (playerIndex < 0 || playerIndex >= Main.maxPlayers)
                    continue;
                    
                Player player = Main.player[playerIndex];
                if (!player.active || player.dead)
                    continue;
                
                PlayerLevel levelSystem = player.GetModPlayer<PlayerLevel>();
                levelSystem.GainExperience(baseXP, source, monsterLevel);
            }
        }

        private void SendXPGainPacket(long baseXP, XPSource source, int monsterLevel)
        {
            if (Player == null || !Player.active)
                return;

            ModPacket packet = Mod.GetPacket();
            packet.Write((byte)global::Rpg.RpgMessageType.AwardXP);
            packet.Write((byte)Player.whoAmI);
            packet.Write(baseXP);
            packet.Write((byte)source);
            packet.Write(monsterLevel);
            packet.Send(Player.whoAmI);
        }

        #endregion

        #region Utility

        /// <summary>
        /// Get progress to next level as percentage (0-1)
        /// </summary>
        public float GetLevelProgress()
        {
            if (rpgPlayer.RequiredXP <= 0)
                return 1f;

            return (float)rpgPlayer.CurrentXP / rpgPlayer.RequiredXP;
        }

        /// <summary>
        /// Get remaining XP needed for next level
        /// </summary>
        public long GetRemainingXP()
        {
            return rpgPlayer.RequiredXP - rpgPlayer.CurrentXP;
        }

        #endregion
    }
}
