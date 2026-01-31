using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent;
using Terraria.ModLoader;
using Terraria.UI;
using RpgMod.Common;
using RpgMod.Common.Players;
using RpgMod.Common.Systems;
using RpgMod.Common.NPCs;
using RpgMod.Common.Config;

namespace RpgMod.Common.UI
{
    /// <summary>
    /// Displays monster level above their heads
    /// Per design doc: "몬스터를 조준 시 해당 몬스터의 레벨과 등급 색상을 표시"
    /// Optimized for events with many monsters
    /// </summary>
    public class MonsterLevelUI : UIState
    {
        // Performance: Limit how many monsters show levels during events
        private const int MAX_DISPLAYED_DURING_EVENT = 15;
        private const int MAX_DISPLAYED_NORMAL = 50;
        private const float PRIORITY_RANGE = 400f; // Pixels - closer monsters get priority
        
        public override void Draw(SpriteBatch spriteBatch)
        {
            if (Main.gameMenu || Main.LocalPlayer == null || !Main.LocalPlayer.active)
                return;
            
            // Get client config
            var config = ModContent.GetInstance<RpgClientConfig>();
            if (config != null && !config.ShowMonsterLevel)
                return;
            
            DrawMonsterLevels(spriteBatch, config);
        }
        
        private void DrawMonsterLevels(SpriteBatch spriteBatch, Config.RpgClientConfig config)
        {
            Player player = Main.LocalPlayer;
            var rpgPlayer = player.GetModPlayer<RpgPlayer>();
            int playerLevel = rpgPlayer.Level;
            Vector2 playerCenter = player.Center;
            
            // Check if we're in an event (need to limit displays)
            bool inEvent = IsInMassEvent();
            int maxDisplay = inEvent ? MAX_DISPLAYED_DURING_EVENT : MAX_DISPLAYED_NORMAL;
            
            // Collect and prioritize monsters
            var monstersToDisplay = new List<(NPC npc, float distance, int level)>();
            
            foreach (NPC npc in Main.npc)
            {
                if (!npc.active || npc.friendly || npc.townNPC || npc.lifeMax <= 5)
                    continue;

                if (IsDuplicateSegment(npc))
                    continue;
                
                // Check if on screen
                if (!IsOnScreen(npc))
                    continue;
                
                float distance = Vector2.Distance(playerCenter, npc.Center);
                int monsterLevel = npc.GetGlobalNPC<RpgGlobalNPC>().MonsterLevel;
                if (monsterLevel <= 0)
                {
                    // Fallback for edge cases: calculate once if not set
                    if (npc.boss || RpgFormulas.IsBossHead(npc.type))
                        monsterLevel = RpgFormulas.GetBossLevel(npc.type);
                    else
                        monsterLevel = BiomeLevelSystem.CalculateMonsterLevel(npc);
                    npc.GetGlobalNPC<RpgGlobalNPC>().MonsterLevel = monsterLevel;
                }
                
                monstersToDisplay.Add((npc, distance, monsterLevel));
            }
            
            // Sort by priority: bosses first, then by distance
            monstersToDisplay.Sort((a, b) =>
            {
                // Bosses always first
                if (a.npc.boss && !b.npc.boss) return -1;
                if (!a.npc.boss && b.npc.boss) return 1;
                
                // Then by distance
                return a.distance.CompareTo(b.distance);
            });
            
            // Draw limited number
            int drawn = 0;
            foreach (var (npc, distance, monsterLevel) in monstersToDisplay)
            {
                if (drawn >= maxDisplay)
                    break;
                
                // During events, skip very distant monsters unless boss
                if (inEvent && !npc.boss && distance > PRIORITY_RANGE * 2)
                    continue;
                
                Color levelColor = (config != null && !config.ColorCodeMonsterLevel)
                    ? Color.White
                    : BiomeLevelSystem.GetLevelColor(monsterLevel, playerLevel);
                
                // Fade out distant monsters during events
                if (inEvent && distance > PRIORITY_RANGE)
                {
                    float fade = 1f - ((distance - PRIORITY_RANGE) / PRIORITY_RANGE);
                    levelColor *= MathHelper.Clamp(fade, 0.3f, 1f);
                }
                
                DrawLevelText(spriteBatch, npc, monsterLevel, levelColor);
                drawn++;
            }
        }
        
        /// <summary>
        /// Check if we're in a mass-spawn event
        /// </summary>
        private bool IsInMassEvent()
        {
            return Main.bloodMoon || Main.eclipse || 
                   Main.pumpkinMoon || Main.snowMoon ||
                   Main.invasionType > 0;
        }
        
        private bool IsOnScreen(NPC npc)
        {
            Rectangle screenRect = new Rectangle(
                (int)Main.screenPosition.X - 100,
                (int)Main.screenPosition.Y - 100,
                Main.screenWidth + 200,
                Main.screenHeight + 200
            );
            
            return screenRect.Contains(npc.Center.ToPoint());
        }

        private static bool IsDuplicateSegment(NPC npc)
        {
            if (npc.realLife >= 0 && npc.realLife != npc.whoAmI)
                return true;

            return RpgFormulas.IsBodySegment(npc.type);
        }
        
        private void DrawLevelText(SpriteBatch spriteBatch, NPC npc, int level, Color color)
        {
            Player player = Main.LocalPlayer;
            var rpgPlayer = player.GetModPlayer<RpgPlayer>();
            int playerLevel = rpgPlayer.Level;
            int levelDiff = level - playerLevel;
            
            // Position above NPC
            Vector2 screenPos = npc.Top - Main.screenPosition;
            screenPos.Y -= 24; // Offset above NPC
            
            // Draw Name (if not boss, as bosses usually have big health bars)
            if (!npc.boss)
            {
                string nameText = npc.FullName;
                Vector2 nameSize = FontAssets.MouseText.Value.MeasureString(nameText) * 0.7f;
                Vector2 namePos = new Vector2(screenPos.X - nameSize.X / 2, screenPos.Y - 18);
                DrawTextWithOutline(spriteBatch, nameText, namePos, Color.LightGray, 0.7f);
            }
            
            // Format text with difficulty indicator (난이도 표시)
            string levelText = $"Lv.{level}";
            
            // Add difficulty symbol based on level difference
            string difficultySymbol = GetDifficultySymbol(levelDiff);
            if (!string.IsNullOrEmpty(difficultySymbol))
            {
                levelText = $"{difficultySymbol} {levelText}";
            }
            
            // Get text size for centering
            Vector2 textSize = FontAssets.MouseText.Value.MeasureString(levelText) * 0.8f;
            screenPos.X -= textSize.X / 2;
            
            // Draw with outline for visibility
            DrawTextWithOutline(spriteBatch, levelText, screenPos, color, 0.8f);
            
            // Draw XP efficiency indicator below level (경험치 효율 표시)
            if (!npc.boss)
            {
                DrawXPEfficiencyIndicator(spriteBatch, npc, playerLevel, level, screenPos);
            }
            
            // Optional: Draw HP bar if boss
            if (npc.boss)
            {
                DrawBossLevelIndicator(spriteBatch, npc, level, color);
            }
        }
        
        /// <summary>
        /// Get difficulty symbol based on level difference (난이도 심볼)
        /// </summary>
        private string GetDifficultySymbol(int levelDiff)
        {
            if (levelDiff >= 15) return "☠☠";      // Extreme danger
            if (levelDiff >= 10) return "☠";       // Very dangerous
            if (levelDiff >= 7) return "▲▲";      // Dangerous
            if (levelDiff >= 5) return "▲";       // Hard
            if (levelDiff >= 2) return "●";       // Challenging (recommended)
            if (levelDiff >= -2) return "";       // Equal (no symbol)
            if (levelDiff >= -5) return "▼";      // Easy
            if (levelDiff >= -10) return "▼▼";    // Very easy
            return "…";                            // Trivial
        }
        
        /// <summary>
        /// Draw XP efficiency indicator below monster level (경험치 효율 표시)
        /// </summary>
        private void DrawXPEfficiencyIndicator(SpriteBatch spriteBatch, NPC npc, int playerLevel, int monsterLevel, Vector2 levelTextPos)
        {
            int levelDiff = monsterLevel - playerLevel;
            
            // Only show if significant difference
            if (levelDiff < -5 || levelDiff > 5)
            {
                Vector2 efficiencyPos = levelTextPos + new Vector2(0, 18); // Below level text
                
                string efficiencyText;
                Color efficiencyColor;
                
                // Sweet spot (권장 난이도)
                if (levelDiff >= RpgConstants.LEVEL_DIFF_SWEETSPOT_START && 
                    levelDiff <= RpgConstants.LEVEL_DIFF_SWEETSPOT_END)
                {
                    efficiencyText = "★ Sweet Spot ★";
                    efficiencyColor = new Color(255, 223, 0); // Bright gold
                }
                // High efficiency
                else if (levelDiff >= -2 && levelDiff <= 5)
                {
                    return; // Normal range, no indicator needed
                }
                // Low efficiency (too high)
                else if (levelDiff >= 7)
                {
                    float xpMult = BiomeLevelSystem.GetLevelDifferenceXPMultiplier(playerLevel, monsterLevel);
                    int xpPercent = (int)(xpMult * 100);
                    efficiencyText = $"{xpPercent}% XP";
                    efficiencyColor = new Color(200, 100, 100); // Reddish
                }
                // Low efficiency (too low)
                else if (levelDiff <= -5)
                {
                    float xpMult = BiomeLevelSystem.GetLevelDifferenceXPMultiplier(playerLevel, monsterLevel);
                    int xpPercent = (int)(xpMult * 100);
                    efficiencyText = $"{xpPercent}% XP";
                    efficiencyColor = new Color(150, 150, 150); // Gray
                }
                else
                {
                    return;
                }
                
                // Center the efficiency text
                Vector2 effTextSize = FontAssets.MouseText.Value.MeasureString(efficiencyText) * 0.5f;
                efficiencyPos.X += (FontAssets.MouseText.Value.MeasureString($"Lv.{monsterLevel}").X * 0.7f - effTextSize.X) / 2;
                
                // Draw efficiency text (smaller scale)
                DrawTextWithOutline(spriteBatch, efficiencyText, efficiencyPos, efficiencyColor, 0.5f);
            }
        }
        
        private void DrawTextWithOutline(SpriteBatch spriteBatch, string text, Vector2 position, Color color, float scale)
        {
            // Draw outline (black border)
            Color outlineColor = Color.Black;
            float outlineOffset = 1f;
            
            for (int x = -1; x <= 1; x++)
            {
                for (int y = -1; y <= 1; y++)
                {
                    if (x == 0 && y == 0) continue;
                    
                    Vector2 offsetPos = position + new Vector2(x * outlineOffset, y * outlineOffset);
                    Utils.DrawBorderStringFourWay(
                        spriteBatch,
                        FontAssets.MouseText.Value, 
                        text, 
                        offsetPos.X,
                        offsetPos.Y,
                        outlineColor,
                        Color.Black,
                        Vector2.Zero,
                        scale
                    );
                }
            }
            
            // Draw main text using Utils.DrawBorderString
            Utils.DrawBorderString(spriteBatch, text, position, color, scale);
        }
        
        private void DrawBossLevelIndicator(SpriteBatch spriteBatch, NPC npc, int level, Color color)
        {
            // For bosses, draw a special indicator
            Vector2 screenPos = npc.Top - Main.screenPosition;
            screenPos.Y -= 35;
            
            string bossText = $"★ Boss Lv.{level} ★";
            Vector2 textSize = FontAssets.MouseText.Value.MeasureString(bossText) * 0.8f;
            screenPos.X -= textSize.X / 2;
            
            // Draw with gold-ish color for bosses
            DrawTextWithOutline(spriteBatch, bossText, screenPos, Color.Gold, 0.8f);
        }
    }
    
    /// <summary>
    /// System to manage Monster Level UI
    /// </summary>
    public class MonsterLevelUISystem : ModSystem
    {
        internal UserInterface monsterLevelInterface;
        internal MonsterLevelUI monsterLevelUI;
        
        public override void Load()
        {
            if (!Main.dedServ)
            {
                monsterLevelUI = new MonsterLevelUI();
                monsterLevelInterface = new UserInterface();
                monsterLevelInterface.SetState(monsterLevelUI);
            }
        }
        
        public override void UpdateUI(GameTime gameTime)
        {
            monsterLevelInterface?.Update(gameTime);
        }
        
        public override void ModifyInterfaceLayers(System.Collections.Generic.List<GameInterfaceLayer> layers)
        {
            // Draw before NPC names
            int npcNameIndex = layers.FindIndex(layer => layer.Name.Equals("Vanilla: NPC / Sign Dialog"));
            if (npcNameIndex != -1)
            {
                layers.Insert(npcNameIndex, new LegacyGameInterfaceLayer(
                    "Rpg: Monster Levels",
                    () =>
                    {
                        monsterLevelInterface?.Draw(Main.spriteBatch, new GameTime());
                        return true;
                    },
                    InterfaceScaleType.Game
                ));
            }
        }
    }
}
