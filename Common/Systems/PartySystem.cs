using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using RpgMod.Common.Players;

namespace RpgMod.Common.Systems
{
    /// <summary>
    /// Party System for multiplayer XP sharing
    /// Players within range share XP when any party member kills an enemy
    /// </summary>
    public class PartySystem : ModSystem
    {
        /// <summary>
        /// Maximum distance for XP sharing (in pixels)
        /// 5000 tiles = 80000 pixels
        /// </summary>
        public const float XP_SHARE_RANGE = 80000f;
        
        /// <summary>
        /// XP multiplier per additional party member (to encourage grouping)
        /// </summary>
        public const float PARTY_XP_BONUS_PER_MEMBER = 0.1f;
        
        /// <summary>
        /// Maximum party XP bonus
        /// </summary>
        public const float MAX_PARTY_XP_BONUS = 0.3f;
        
        /// <summary>
        /// Distribute XP to all nearby players
        /// </summary>
        public static void DistributeXP(Player killer, NPC npc, int baseXP, int monsterLevel = 0)
        {
            if (Main.netMode == NetmodeID.SinglePlayer)
            {
                // Single player - just give XP to the killer
                var playerLevel = killer.GetModPlayer<PlayerLevel>();
                playerLevel.GainExperience(baseXP, XPSource.Monster, monsterLevel);
                return;
            }
            
            // Multiplayer - find all nearby players
            List<Player> nearbyPlayers = GetNearbyPlayers(killer.Center, XP_SHARE_RANGE);
            
            if (nearbyPlayers.Count <= 1)
            {
                // Only the killer is nearby
                var playerLevel = killer.GetModPlayer<PlayerLevel>();
                playerLevel.GainExperience(baseXP, XPSource.Monster, monsterLevel);
                return;
            }
            
            // Calculate party bonus
            float partyBonus = Math.Min(
                (nearbyPlayers.Count - 1) * PARTY_XP_BONUS_PER_MEMBER,
                MAX_PARTY_XP_BONUS
            );
            
            int sharedXP = (int)(baseXP * (1f + partyBonus));
            
            // Distribute to all nearby players
            foreach (Player player in nearbyPlayers)
            {
                var playerLevel = player.GetModPlayer<PlayerLevel>();
                playerLevel.GainExperience(sharedXP, XPSource.Monster, monsterLevel);
                
                // Record achievement for party XP
                var achievements = player.GetModPlayer<AchievementSystem>();
                achievements?.RecordPartyXP();
            }
            
            // Show party bonus message to killer
            if (partyBonus > 0)
            {
                string bonusText = $"+{(int)(partyBonus * 100)}% Party Bonus!";
                CombatText.NewText(killer.Hitbox, Color.Cyan, bonusText, false, true);
            }
        }
        
        /// <summary>
        /// Get all active players within range of a position
        /// </summary>
        public static List<Player> GetNearbyPlayers(Vector2 position, float range)
        {
            List<Player> nearby = new List<Player>();
            float rangeSq = range * range;
            
            for (int i = 0; i < Main.maxPlayers; i++)
            {
                Player player = Main.player[i];
                if (player.active && !player.dead)
                {
                    float distSq = Vector2.DistanceSquared(position, player.Center);
                    if (distSq <= rangeSq)
                    {
                        nearby.Add(player);
                    }
                }
            }
            
            return nearby;
        }
        
        /// <summary>
        /// Check if two players are in party range
        /// </summary>
        public static bool ArePlayersInRange(Player a, Player b)
        {
            if (a == null || b == null || !a.active || !b.active)
                return false;
            
            return Vector2.DistanceSquared(a.Center, b.Center) <= XP_SHARE_RANGE * XP_SHARE_RANGE;
        }
        
        /// <summary>
        /// Get party member count for a player
        /// </summary>
        public static int GetPartyMemberCount(Player player)
        {
            return GetNearbyPlayers(player.Center, XP_SHARE_RANGE).Count;
        }
    }
    
    /// <summary>
    /// Extension for PlayerLevel to support shared XP
    /// </summary>
    public static class PlayerLevelPartyExtensions
    {
        // XPSource enum is already available in Players namespace
    }
}
