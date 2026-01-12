using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Rpg.Common.Players;
using Rpg.Common.Systems;
using Rpg.Common.Compatibility;
using System.Collections.Generic;

namespace Rpg.Common.NPCs
{
    /// <summary>
    /// Global NPC handler - applies monster scaling and XP drops
    /// Handles all special cases: bosses, events, statues, segments
    /// Also tracks player damage for trap/lava kills
    /// </summary>
    public class RpgGlobalNPC : GlobalNPC
    {
        public override bool InstancePerEntity => true;
        
        // Track which players have damaged this NPC and how much
        private Dictionary<int, int> playerDamageDealt = new();
        
        // Minimum damage threshold to qualify for XP (prevents cheese)
        private const int MIN_DAMAGE_FOR_XP = 1;
        
        // Time window for damage to count (in ticks, 60 = 1 second)
        private int lastDamageTime = 0;
        private const int DAMAGE_TIMEOUT = 600; // 10 seconds
        
        // Monster level (calculated on spawn)
        public int MonsterLevel { get; private set; } = 1;

        #region Damage Tracking
        
        public override void OnHitByItem(NPC npc, Player player, Item item, NPC.HitInfo hit, int damageDone)
        {
            TrackPlayerDamage(player.whoAmI, damageDone);
        }
        
        public override void OnHitByProjectile(NPC npc, Projectile projectile, NPC.HitInfo hit, int damageDone)
        {
            // Track damage from player's projectile
            if (projectile.owner >= 0 && projectile.owner < Main.maxPlayers)
            {
                TrackPlayerDamage(projectile.owner, damageDone);
            }
            // Track minion damage
            else if (projectile.minion || projectile.sentry)
            {
                // Find owner of minion
                for (int i = 0; i < Main.maxPlayers; i++)
                {
                    if (Main.player[i].active && projectile.owner == i)
                    {
                        TrackPlayerDamage(i, damageDone);
                        break;
                    }
                }
            }
        }
        
        /// <summary>
        /// Track damage dealt by a player
        /// </summary>
        private void TrackPlayerDamage(int playerIndex, int damage)
        {
            if (playerIndex < 0 || playerIndex >= Main.maxPlayers)
                return;
                
            if (playerDamageDealt.ContainsKey(playerIndex))
                playerDamageDealt[playerIndex] += damage;
            else
                playerDamageDealt[playerIndex] = damage;
                
            lastDamageTime = (int)Main.GameUpdateCount;
        }
        
        /// <summary>
        /// Check if any player has dealt damage to this NPC
        /// </summary>
        private bool HasPlayerDamage()
        {
            // Check if damage was dealt recently
            if ((int)Main.GameUpdateCount - lastDamageTime > DAMAGE_TIMEOUT)
            {
                playerDamageDealt.Clear();
                return false;
            }
            
            foreach (var kvp in playerDamageDealt)
            {
                if (kvp.Value >= MIN_DAMAGE_FOR_XP)
                    return true;
            }
            return false;
        }
        
        /// <summary>
        /// Get list of players who dealt damage (for XP distribution)
        /// </summary>
        private List<int> GetPlayersWhoDamaged()
        {
            var players = new List<int>();
            
            // Check timeout
            if ((int)Main.GameUpdateCount - lastDamageTime > DAMAGE_TIMEOUT)
                return players;
            
            foreach (var kvp in playerDamageDealt)
            {
                if (kvp.Value >= MIN_DAMAGE_FOR_XP && 
                    kvp.Key >= 0 && kvp.Key < Main.maxPlayers &&
                    Main.player[kvp.Key].active)
                {
                    players.Add(kvp.Key);
                }
            }
            return players;
        }
        
        #endregion

        #region Scaling

        public override void SetDefaults(NPC npc)
        {
            // Don't scale town NPCs or friendly NPCs
            if (npc.townNPC || npc.friendly)
                return;

            // Don't scale critters
            if (npc.lifeMax <= 5)
                return;

            // Apply world level scaling with progression-aware system
            ApplyProgressionAwareScaling(npc);
        }

        /// <summary>
        /// Apply HP and damage scaling based on world level with progression separation
        /// Per design doc: Pre-hardmode monsters cap their scaling when hardmode begins
        /// </summary>
        private void ApplyProgressionAwareScaling(NPC npc)
        {
            int worldLevel = RpgWorld.GetWorldLevel();

            // Skip scaling if world level is 1 (no bosses killed yet)
            if (worldLevel <= 1)
                return;

            // Determine if this is a pre-hardmode or hardmode monster
            bool isHardmodeMonster = IsHardmodeMonster(npc);
            bool worldInHardmode = Main.hardMode;
            
            // Get effective world level for this monster
            int effectiveWorldLevel = GetEffectiveWorldLevel(npc, worldLevel, isHardmodeMonster, worldInHardmode);
            
            // Apply biome-based scaling
            ApplyBiomeAwareScaling(npc, effectiveWorldLevel);
        }
        
        /// <summary>
        /// Check if NPC is a hardmode monster
        /// </summary>
        private bool IsHardmodeMonster(NPC npc)
        {
            // Check known hardmode NPCs
            return npc.type >= Terraria.ID.NPCID.Pixie && npc.type <= Terraria.ID.NPCID.DungeonSpirit ||
                   npc.type >= Terraria.ID.NPCID.Wraith && npc.type <= Terraria.ID.NPCID.Clown ||
                   npc.type >= Terraria.ID.NPCID.Mummy && npc.type <= Terraria.ID.NPCID.DarkMummy ||
                   npc.type >= Terraria.ID.NPCID.Corruptor && npc.type <= Terraria.ID.NPCID.CorruptSlime ||
                   npc.type >= Terraria.ID.NPCID.Gastropod && npc.type <= Terraria.ID.NPCID.IlluminantSlime ||
                   npc.type >= Terraria.ID.NPCID.GiantBat && npc.type <= Terraria.ID.NPCID.IceGolem ||
                   // Mechanical bosses and beyond
                   npc.type == Terraria.ID.NPCID.TheDestroyer ||
                   npc.type == Terraria.ID.NPCID.Retinazer ||
                   npc.type == Terraria.ID.NPCID.Spazmatism ||
                   npc.type == Terraria.ID.NPCID.SkeletronPrime ||
                   npc.type == Terraria.ID.NPCID.Plantera ||
                   npc.type == Terraria.ID.NPCID.Golem ||
                   npc.type == Terraria.ID.NPCID.DukeFishron ||
                   npc.type == Terraria.ID.NPCID.CultistBoss ||
                   npc.type == Terraria.ID.NPCID.MoonLordCore ||
                   // Use the hardmode flag as fallback for modded NPCs
                   (npc.HitSound == Terraria.ID.SoundID.NPCHit1 && npc.lifeMax > 100 && Main.hardMode);
        }
        
        /// <summary>
        /// Get effective world level for scaling, considering progression
        /// </summary>
        private int GetEffectiveWorldLevel(NPC npc, int worldLevel, bool isHardmodeMonster, bool worldInHardmode)
        {
            // Pre-hardmode monsters cap at world level when WoF was killed
            if (!isHardmodeMonster && worldInHardmode)
            {
                // Cap pre-hardmode monsters at a reasonable level
                // This is approximately world level 5-6 (after major pre-hm bosses)
                int preHardmodeCap = RpgConstants.PREHARDMODE_MONSTER_LEVEL_CAP;
                return System.Math.Min(worldLevel, preHardmodeCap);
            }
            
            // Hardmode monsters use full world level, but start from a baseline
            if (isHardmodeMonster)
            {
                // Hardmode monsters start scaling from hardmode world level
                return worldLevel;
            }
            
            // Pre-hardmode in pre-hardmode world: normal scaling
            return worldLevel;
        }
        
        /// <summary>
        /// Apply scaling using biome-aware system
        /// </summary>
        private void ApplyBiomeAwareScaling(NPC npc, int effectiveWorldLevel)
        {
            // Calculate monster level from biome system
            MonsterLevel = BiomeLevelSystem.CalculateMonsterLevel(npc);
            
            if (effectiveWorldLevel <= 1)
                return;
            
            // Get multipliers from formulas using effective world level
            float hpMultiplier = RpgFormulas.GetMonsterHPMultiplier(effectiveWorldLevel);
            float damageMultiplier = RpgFormulas.GetMonsterDamageMultiplier(effectiveWorldLevel);

            // Apply HP scaling
            npc.lifeMax = (int)(npc.lifeMax * hpMultiplier);
            npc.life = npc.lifeMax;

            // Apply damage scaling
            npc.damage = (int)(npc.damage * damageMultiplier);
        }

        #endregion

        #region XP Drop System

        public override void OnKill(NPC npc)
        {
            // Calculate and award XP
            CalculateAndAwardXP(npc);

            // Track boss kills for world progression
            if (npc.boss)
            {
                WorldProgression.OnNPCKilled(npc);
            }
            
            // Track kill achievements for nearby players
            TrackKillAchievements(npc);
        }
        
        /// <summary>
        /// Track kill-related achievements for players who damaged this NPC
        /// </summary>
        private void TrackKillAchievements(NPC npc)
        {
            // Only track for non-friendly NPCs with meaningful HP
            if (npc.friendly || npc.townNPC || npc.lifeMax <= 5)
                return;
            
            var damagingPlayers = GetPlayersWhoDamaged();
            
            foreach (int playerIndex in damagingPlayers)
            {
                if (playerIndex < 0 || playerIndex >= Main.maxPlayers)
                    continue;
                
                Player player = Main.player[playerIndex];
                if (player == null || !player.active)
                    continue;
                
                var achievements = player.GetModPlayer<Systems.AchievementSystem>();
                if (achievements == null)
                    continue;
                
                // Record kill for achievements
                achievements.RecordKill();
                
                // Check boss achievements
                if (npc.boss)
                {
                    achievements.CheckBossAchievements();
                }
            }
        }

        /// <summary>
        /// Calculate XP and distribute to nearby players
        /// </summary>
        private void CalculateAndAwardXP(NPC npc)
        {
            // Skip if no XP should be awarded
            if (!ShouldAwardXP(npc))
                return;

            long baseXP;
            XPSource source;
            int monsterLevel = MonsterLevel;

            // Boss XP
            if (npc.boss)
            {
                int bossLevel = RpgFormulas.GetBossLevel(npc.type);
                int averageLevel = PlayerLevel.GetAveragePlayerLevel(
                    npc.Center,
                    RpgConstants.MULTIPLAYER_XP_SHARE_RADIUS
                );
                baseXP = RpgFormulas.CalculateBossXP(bossLevel, averageLevel);
                source = XPSource.Boss;
                monsterLevel = bossLevel; // Use boss level for XP calculation
            }
            // Normal monster XP
            else
            {
                baseXP = RpgFormulas.CalculateBaseXP(npc);
                source = XPSource.Monster;
            }

            // Apply world level multiplier (progression-capped)
            float worldLevelMultiplier = RpgFormulas.GetWorldLevelXPMultiplier(RpgWorld.GetEffectiveWorldLevel());
            baseXP = (long)(baseXP * worldLevelMultiplier);

            // Apply monster level multiplier for regular mobs (biome/world level driven)
            if (!npc.boss)
            {
                float monsterLevelMultiplier = RpgFormulas.GetMonsterLevelXPMultiplier(monsterLevel);
                baseXP = (long)(baseXP * monsterLevelMultiplier);
            }

            // Apply event multiplier (if in event)
            float eventMultiplier = RpgFormulas.GetEventXPMultiplier();
            baseXP = (long)(baseXP * eventMultiplier);
            
            // Apply mod compatibility multiplier (Calamity/Thorium enemies give more XP)
            float modXPMultiplier = ModCompatibilitySystem.GetModXPMultiplier(npc);
            baseXP = (long)(baseXP * modXPMultiplier);
            
            // Apply mod difficulty scaling (reduce XP gain in Calamity's harder modes)
            float modDifficultyScale = ModCompatibilitySystem.GetModDifficultyScaling();
            baseXP = (long)(baseXP * modDifficultyScale);

            // Minimum XP
            if (baseXP < 1)
                baseXP = 1;

            // Distribute to players who damaged this NPC
            var damagingPlayers = GetPlayersWhoDamaged();
            
            if (damagingPlayers.Count > 0)
            {
                // Give XP to players who actually dealt damage
                PlayerLevel.DistributeXPToSpecificPlayers(damagingPlayers, baseXP, source, monsterLevel);
            }
            else
            {
                if (HasRecentPlayerInteraction(npc))
                {
                    PlayerLevel.DistributeXPToSpecificPlayers(
                        new List<int> { npc.lastInteraction },
                        baseXP,
                        source,
                        monsterLevel
                    );
                }
                else
                {
                    // Fallback: distribute to nearby players (shouldn't happen due to ShouldAwardXP check)
                    PlayerLevel.DistributeXPToNearbyPlayers(npc.Center, baseXP, source, monsterLevel);
                }
            }
        }

        /// <summary>
        /// Check if this NPC should award XP
        /// </summary>
        private bool ShouldAwardXP(NPC npc)
        {
            // No XP from friendly NPCs
            if (npc.friendly || npc.townNPC)
                return false;

            // No XP from statue-spawned enemies
            if (npc.SpawnedFromStatue)
                return false;

            // No XP from critters
            if (npc.lifeMax <= 5)
                return false;
            
            // TRAP/LAVA KILL CHECK: No XP if no player damaged this NPC
            // This prevents pure trap/lava farms from giving free XP
            if (!HasPlayerDamage() && !HasRecentPlayerInteraction(npc))
                return false;

            // Special case: Segmented bosses (only head gives XP)
            if (RpgFormulas.IsBodySegment(npc.type))
                return false;

            // For segmented bosses with heads, only the head awards XP
            if (npc.boss && RpgFormulas.IsBossHead(npc.type))
            {
                // This is a head, award XP
                return true;
            }
            else if (npc.boss)
            {
                // This is a boss but not specifically a head
                // Check if it's part of a segmented boss
                int headType = GetBossHeadType(npc.type);
                if (headType > 0)
                {
                    // It's part of a segmented boss, don't award XP
                    return false;
                }
                // Not a segmented boss, award XP
                return true;
            }

            // Default: award XP
            return true;
        }

        private bool HasRecentPlayerInteraction(NPC npc)
        {
            int lastPlayer = npc.lastInteraction;
            if (lastPlayer < 0 || lastPlayer >= Main.maxPlayers)
                return false;

            Player player = Main.player[lastPlayer];
            return player != null && player.active;
        }

        /// <summary>
        /// Get the head type for segmented bosses
        /// </summary>
        private int GetBossHeadType(int npcType)
        {
            // Eater of Worlds
            if (npcType == NPCID.EaterofWorldsBody || npcType == NPCID.EaterofWorldsTail)
                return NPCID.EaterofWorldsHead;

            // Destroyer
            if (npcType == NPCID.TheDestroyerBody || npcType == NPCID.TheDestroyerTail)
                return NPCID.TheDestroyer;

            return 0; // Not a segmented boss part
        }

        #endregion

        #region Boss Level Display

        public override void ModifyHoverBoundingBox(NPC npc, ref Rectangle boundingBox)
        {
            // Could be used to show boss level above health bar
            // For now, just override to prepare for UI integration
        }

        #endregion

        #region Special Cases

        /// <summary>
        /// Handle special XP cases for modded NPCs
        /// Override this in derived classes for mod support
        /// </summary>
        public virtual long GetModdedNPCBaseXP(NPC npc)
        {
            // Default: use standard formula
            return RpgFormulas.CalculateBaseXP(npc);
        }

        /// <summary>
        /// Check if modded NPC should award XP
        /// Override for mod-specific logic
        /// </summary>
        public virtual bool ShouldModdedNPCAwardXP(NPC npc)
        {
            return true;
        }

        #endregion

        #region Debugging

#if DEBUG
        /// <summary>
        /// Show NPC stats in debug mode (hover tooltip)
        /// </summary>
        public override void ModifyHitByProjectile(NPC npc, Projectile projectile, ref NPC.HitModifiers modifiers)
        {
            if (RpgConstants.DEBUG_SHOW_XP_VALUES)
            {
                long xp = npc.boss ? 
                    RpgFormulas.CalculateBossXP(
                        RpgFormulas.GetBossLevel(npc.type),
                        Main.LocalPlayer.GetModPlayer<RpgPlayer>().Level
                    ) : 
                    RpgFormulas.CalculateBaseXP(npc);
                
                Main.NewText($"[DEBUG] {npc.FullName}: {xp} XP", Color.Yellow);
            }
        }
#endif

        #endregion
    }
}
