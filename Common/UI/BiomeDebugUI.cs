using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Terraria;
using Terraria.GameInput;
using Terraria.ModLoader;
using Terraria.UI;
using Rpg.Common;
using Rpg.Common.Jobs;
using Rpg.Common.Players;
using Rpg.Common.Systems;

namespace Rpg.Common.UI
{
    /// <summary>
    /// Debug panel for testing RPG progression and biome levels.
    /// </summary>
    public class BiomeDebugUISystem : ModSystem
    {
        private const bool ENABLED = false;
        public static ModKeybind ToggleKey { get; private set; }

        private UserInterface debugInterface;
        private BiomeDebugUI debugUI;
        private bool isVisible;

        public override void Load()
        {
            if (!ENABLED)
                return;

#pragma warning disable CS0162
            ToggleKey = KeybindLoader.RegisterKeybind(Mod, "Toggle RPG Test Panel", "F9");

            if (!Main.dedServ)
            {
                debugUI = new BiomeDebugUI();
                debugUI.Activate();
                debugInterface = new UserInterface();
            }
#pragma warning restore CS0162
        }

        public override void Unload()
        {
            ToggleKey = null;
            debugInterface = null;
            debugUI = null;
        }

        public override void UpdateUI(GameTime gameTime)
        {
            if (!ENABLED)
                return;

#pragma warning disable CS0162
            if (isVisible)
            {
                debugInterface?.Update(gameTime);
            }
#pragma warning restore CS0162
        }

        public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
        {
            if (!ENABLED)
                return;

#pragma warning disable CS0162
            if (!isVisible)
                return;

            int mouseTextIndex = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Mouse Text"));
            if (mouseTextIndex != -1)
            {
                layers.Insert(mouseTextIndex, new LegacyGameInterfaceLayer(
                    "Rpg: Test Panel",
                    delegate
                    {
                        debugInterface?.Draw(Main.spriteBatch, new GameTime());
                        return true;
                    },
                    InterfaceScaleType.UI
                ));
            }
#pragma warning restore CS0162
        }

        public void ToggleUI()
        {
            if (!ENABLED)
                return;

#pragma warning disable CS0162
            if (isVisible)
                HideUI();
            else
                ShowUI();
#pragma warning restore CS0162
        }

        public void ShowUI()
        {
            if (isVisible)
                return;

            debugInterface?.SetState(debugUI);
            isVisible = true;
        }

        public void HideUI()
        {
            if (!isVisible)
                return;

            debugInterface?.SetState(null);
            isVisible = false;
        }
    }

    public class BiomeDebugUIPlayer : ModPlayer
    {
        public override void ProcessTriggers(TriggersSet triggersSet)
        {
            if (BiomeDebugUISystem.ToggleKey == null)
                return;

            if (Main.drawingPlayerChat || Main.editSign || Main.editChest ||
                Main.gameMenu || Main.ingameOptionsWindow || Main.playerInventory || Main.inFancyUI)
                return;

#pragma warning disable CS0162
            if (BiomeDebugUISystem.ToggleKey != null && BiomeDebugUISystem.ToggleKey.JustPressed)
            {
                ModContent.GetInstance<BiomeDebugUISystem>().ToggleUI();
            }
#pragma warning restore CS0162
        }
    }

    public class BiomeDebugUI : UIState
    {
        private enum DebugTab
        {
            Biome,
            Player,
            Job,
            Stats
        }

        private enum BiomeSource
        {
            Player,
            Mouse,
            Preview
        }

        private readonly struct BiomeEntry
        {
            public readonly BiomeType VanillaBiome;
            public readonly ModBiome ModBiome;

            public BiomeEntry(BiomeType biome)
            {
                VanillaBiome = biome;
                ModBiome = null;
            }

            public BiomeEntry(ModBiome biome)
            {
                VanillaBiome = BiomeType.None;
                ModBiome = biome;
            }

            public bool IsMod => ModBiome != null;
        }

        private const int PANEL_WIDTH = 440;
        private const int PANEL_HEIGHT = 460;
        private const int HEADER_HEIGHT = 28;

        private Rectangle bounds;
        private bool dragging;
        private Vector2 dragOffset;
        private static Rectangle? lastBounds;
        private static DebugTab? lastTab;

        private DebugTab currentTab = DebugTab.Biome;
        private BiomeSource source = BiomeSource.Player;
        private BiomeEntry[] biomeList = Array.Empty<BiomeEntry>();
        private int previewBiomeIndex;
        private JobType[] jobList = Array.Empty<JobType>();
        private JobType previewJob = JobType.Novice;
        private StatType[] statList = Array.Empty<StatType>();
        private bool editAutoStats;

        public override void OnInitialize()
        {
            if (lastBounds.HasValue)
            {
                bounds = lastBounds.Value;
            }
            else
            {
                bounds = new Rectangle(
                    Main.screenWidth / 2 - PANEL_WIDTH / 2,
                    Main.screenHeight / 2 - PANEL_HEIGHT / 2,
                    PANEL_WIDTH,
                    PANEL_HEIGHT
                );
            }

            currentTab = lastTab ?? DebugTab.Biome;

            var biomeEntries = new List<BiomeEntry>();
            foreach (BiomeType biome in Enum.GetValues(typeof(BiomeType)))
            {
                if (biome != BiomeType.None)
                    biomeEntries.Add(new BiomeEntry(biome));
            }

            foreach (ModBiome modBiome in ModContent.GetContent<ModBiome>())
            {
                biomeEntries.Add(new BiomeEntry(modBiome));
            }

            biomeList = biomeEntries.ToArray();
            previewBiomeIndex = biomeList.Length > 0 ? 0 : -1;

            var jobs = new List<JobType>();
            foreach (JobType job in Enum.GetValues(typeof(JobType)))
            {
                if (job != JobType.None)
                    jobs.Add(job);
            }
            jobList = jobs.ToArray();
            if (jobList.Length > 0)
                previewJob = jobList[0];

            var stats = new List<StatType>();
            foreach (StatType stat in Enum.GetValues(typeof(StatType)))
            {
                stats.Add(stat);
            }
            statList = stats.ToArray();
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            HandleDragging();
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            if (Main.gameMenu || Main.LocalPlayer == null || !Main.LocalPlayer.active)
                return;

            DrawPanel(spriteBatch, bounds, new Color(18, 22, 34, 235), new Color(70, 85, 120));

            Rectangle header = new Rectangle(bounds.X, bounds.Y, bounds.Width, HEADER_HEIGHT);
            DrawPanel(spriteBatch, header, new Color(30, 35, 55, 255), new Color(90, 110, 150));
            DrawText(spriteBatch, "RPG Test Panel", new Vector2(bounds.X + 10, bounds.Y + 6), Color.Gold, 0.85f);

            Rectangle closeBtn = new Rectangle(bounds.Right - 22, bounds.Y + 5, 16, 16);
            if (DrawButton(spriteBatch, closeBtn, "X", false, true, 0.7f))
            {
                ModContent.GetInstance<BiomeDebugUISystem>().HideUI();
                return;
            }

            int x = bounds.X + 10;
            int y = bounds.Y + HEADER_HEIGHT + 6;

            y = DrawTabs(spriteBatch, x, y);

            switch (currentTab)
            {
                case DebugTab.Player:
                    DrawPlayerTab(spriteBatch, x, y);
                    break;
                case DebugTab.Job:
                    DrawJobTab(spriteBatch, x, y);
                    break;
                case DebugTab.Stats:
                    DrawStatsTab(spriteBatch, x, y);
                    break;
                default:
                    DrawBiomeTab(spriteBatch, x, y);
                    break;
            }

            if (bounds.Contains(Main.mouseX, Main.mouseY))
                Main.LocalPlayer.mouseInterface = true;
        }

        private void HandleDragging()
        {
            Rectangle header = new Rectangle(bounds.X, bounds.Y, bounds.Width, HEADER_HEIGHT);
            Rectangle closeBtn = new Rectangle(bounds.Right - 22, bounds.Y + 5, 16, 16);

            if (!dragging && Main.mouseLeft && header.Contains(Main.mouseX, Main.mouseY) && !closeBtn.Contains(Main.mouseX, Main.mouseY))
            {
                dragging = true;
                dragOffset = new Vector2(Main.mouseX - bounds.X, Main.mouseY - bounds.Y);
            }
            else if (dragging && !Main.mouseLeft)
            {
                dragging = false;
                lastBounds = bounds;
            }

            if (dragging)
            {
                bounds.X = (int)(Main.mouseX - dragOffset.X);
                bounds.Y = (int)(Main.mouseY - dragOffset.Y);
                bounds.X = Utils.Clamp(bounds.X, 0, Main.screenWidth - PANEL_WIDTH);
                bounds.Y = Utils.Clamp(bounds.Y, 0, Main.screenHeight - PANEL_HEIGHT);
            }
        }

        private int DrawTabs(SpriteBatch spriteBatch, int x, int y)
        {
            int tabWidth = 80;
            int tabHeight = 18;
            int spacing = 6;
            int tabX = x;

            if (DrawButton(spriteBatch, new Rectangle(tabX, y, tabWidth, tabHeight), "Biome", currentTab == DebugTab.Biome, true, 0.7f))
                SetTab(DebugTab.Biome);
            tabX += tabWidth + spacing;

            if (DrawButton(spriteBatch, new Rectangle(tabX, y, tabWidth, tabHeight), "Player", currentTab == DebugTab.Player, true, 0.7f))
                SetTab(DebugTab.Player);
            tabX += tabWidth + spacing;

            if (DrawButton(spriteBatch, new Rectangle(tabX, y, tabWidth, tabHeight), "Job", currentTab == DebugTab.Job, true, 0.7f))
                SetTab(DebugTab.Job);
            tabX += tabWidth + spacing;

            if (DrawButton(spriteBatch, new Rectangle(tabX, y, tabWidth, tabHeight), "Stats", currentTab == DebugTab.Stats, true, 0.7f))
                SetTab(DebugTab.Stats);

            return y + tabHeight + 10;
        }

        private void SetTab(DebugTab tab)
        {
            currentTab = tab;
            lastTab = tab;
        }

        private void DrawBiomeTab(SpriteBatch spriteBatch, int x, int y)
        {
            int worldLevel = RpgWorld.GetWorldLevel();
            int effectiveWorldLevel = RpgWorld.GetEffectiveWorldLevel();
            string stage = RpgWorld.GetProgressionStageName();

            DrawText(spriteBatch, $"World Level: {worldLevel} (Eff {effectiveWorldLevel})", new Vector2(x, y), Color.LightGreen, 0.8f);
            y += 18;
            DrawText(spriteBatch, $"Stage: {stage}  Hardmode: {(Main.hardMode ? "Yes" : "No")}", new Vector2(x, y), Color.LightGray, 0.75f);
            y += 20;

            int step = GetStep();

            Rectangle minusBtn = new Rectangle(x, y, 20, 18);
            Rectangle plusBtn = new Rectangle(x + 24, y, 20, 18);
            Rectangle resetBtn = new Rectangle(x + 48, y, 54, 18);

            if (DrawButton(spriteBatch, minusBtn, "-", false, true, 0.75f))
                RpgWorld.SetWorldLevel(worldLevel - step);
            if (DrawButton(spriteBatch, plusBtn, "+", false, true, 0.75f))
                RpgWorld.SetWorldLevel(worldLevel + step);
            if (DrawButton(spriteBatch, resetBtn, "Reset", false, true, 0.7f))
                RpgWorld.SetWorldLevel(1);

            DrawText(spriteBatch, "Step: Shift=5  Ctrl=10", new Vector2(x + 110, y + 2), Color.Gray, 0.7f);
            y += 26;

            DrawText(spriteBatch, "Source:", new Vector2(x, y), Color.White, 0.75f);
            int btnWidth = 64;
            int btnHeight = 18;
            int btnSpacing = 6;
            int btnX = x + 58;

            if (DrawButton(spriteBatch, new Rectangle(btnX, y, btnWidth, btnHeight), "Player", source == BiomeSource.Player, true, 0.7f))
                source = BiomeSource.Player;
            if (DrawButton(spriteBatch, new Rectangle(btnX + (btnWidth + btnSpacing), y, btnWidth, btnHeight), "Mouse", source == BiomeSource.Mouse, true, 0.7f))
                source = BiomeSource.Mouse;
            if (DrawButton(spriteBatch, new Rectangle(btnX + (btnWidth + btnSpacing) * 2, y, btnWidth, btnHeight), "Preview", source == BiomeSource.Preview, true, 0.7f))
                source = BiomeSource.Preview;
            y += 24;

            BiomeEntry activeBiome = GetActiveBiome(out string sourceInfo);
            string biomeLabel = GetBiomeLabel(activeBiome);

            if (source == BiomeSource.Preview)
            {
                Rectangle prevBtn = new Rectangle(x, y, 18, 18);
                Rectangle nextBtn = new Rectangle(x + 220, y, 18, 18);
                if (DrawButton(spriteBatch, prevBtn, "<", false, true, 0.75f))
                    MovePreviewBiome(-1);
                if (DrawButton(spriteBatch, nextBtn, ">", false, true, 0.75f))
                    MovePreviewBiome(1);

                DrawText(spriteBatch, $"Preview: {biomeLabel}", new Vector2(x + 24, y + 2), Color.White, 0.8f);
                y += 20;
            }
            else
            {
                DrawText(spriteBatch, $"Biome: {biomeLabel} ({sourceInfo})", new Vector2(x, y), Color.White, 0.8f);
                y += 18;
            }

            BiomeLevelSystem.BiomeLevelData data = activeBiome.IsMod
                ? BiomeLevelSystem.GetModBiomeData(activeBiome.ModBiome)
                : BiomeLevelSystem.GetBiomeData(activeBiome.VanillaBiome);

            int worldBonus = (int)((worldLevel - 1) * data.GrowthRate);
            int rawLevel = data.BaseLevel + worldBonus;
            int currentCap = data.GetCurrentCap();
            int cappedLevel = Math.Min(rawLevel, currentCap);

            if (activeBiome.IsMod && activeBiome.ModBiome != null)
            {
                string modName = activeBiome.ModBiome.Mod?.Name ?? "Mod";
                DrawText(spriteBatch, $"Mod: {modName}  Priority: {activeBiome.ModBiome.Priority}", new Vector2(x, y), Color.LightGray, 0.7f);
                y += 16;
            }

            DrawText(spriteBatch, $"Base {data.BaseLevel}  Growth {data.GrowthRate:0.00}  HardmodeOnly {(data.IsHardmodeOnly ? "Yes" : "No")}", new Vector2(x, y), Color.LightGray, 0.7f);
            y += 16;
            DrawText(spriteBatch, $"Bonus {worldBonus}  Raw {rawLevel}  Cap {currentCap}  Final {cappedLevel}", new Vector2(x, y), Color.LightGray, 0.7f);
            y += 16;
            DrawText(spriteBatch, $"Caps: Pre {data.PreHardmodeCap} / Early {data.EarlyHardmodeCap} / Mid {data.MidHardmodeCap}", new Vector2(x, y), Color.LightGray, 0.7f);
            y += 16;
            DrawText(spriteBatch, $"Caps: Late {data.LateHardmodeCap} / Post {data.PostMoonLordCap}", new Vector2(x, y), Color.LightGray, 0.7f);
            y += 16;

            int calcLevel = activeBiome.IsMod
                ? BiomeLevelSystem.CalculateMonsterLevelForModBiome(activeBiome.ModBiome)
                : BiomeLevelSystem.CalculateMonsterLevelForBiome(activeBiome.VanillaBiome);

            DrawText(spriteBatch, $"Calc (Biome): {calcLevel}", new Vector2(x, y), Color.Khaki, 0.7f);
            y += 16;
            DrawText(
                spriteBatch,
                $"Floors: Pre {RpgConstants.PREHARDMODE_MONSTER_MIN_LEVEL} / PreHM-HM {RpgConstants.PREHARDMODE_MONSTER_MIN_LEVEL_HARDMODE} / HM {RpgConstants.HARDMODE_MONSTER_MIN_LEVEL}",
                new Vector2(x, y),
                Color.Gray,
                0.65f);
        }

        private void DrawPlayerTab(SpriteBatch spriteBatch, int x, int y)
        {
            Player player = Main.LocalPlayer;
            RpgPlayer rpgPlayer = player.GetModPlayer<RpgPlayer>();
            if (rpgPlayer == null)
                return;

            int step = GetStep();
            int maxLevel = RpgFormulas.GetMaxLevel();

            DrawText(spriteBatch, $"Level: {rpgPlayer.Level} / {maxLevel}", new Vector2(x, y), Color.White, 0.8f);
            Rectangle levelMinus = new Rectangle(x, y + 18, 20, 18);
            Rectangle levelPlus = new Rectangle(x + 24, y + 18, 20, 18);
            Rectangle levelMax = new Rectangle(x + 48, y + 18, 54, 18);

            if (DrawButton(spriteBatch, levelMinus, "-", false, true, 0.75f))
                SetLevel(rpgPlayer, rpgPlayer.Level - step);
            if (DrawButton(spriteBatch, levelPlus, "+", false, true, 0.75f))
                SetLevel(rpgPlayer, rpgPlayer.Level + step);
            if (DrawButton(spriteBatch, levelMax, "Max", false, true, 0.7f))
                SetLevel(rpgPlayer, maxLevel);

            DrawText(spriteBatch, "Step: Shift=5  Ctrl=10", new Vector2(x + 110, y + 20), Color.Gray, 0.7f);
            y += 42;

            long xpStep = GetXpStep(rpgPlayer, step);
            DrawText(spriteBatch, $"XP: {rpgPlayer.CurrentXP} / {rpgPlayer.RequiredXP}", new Vector2(x, y), Color.White, 0.8f);
            Rectangle xpMinus = new Rectangle(x, y + 18, 20, 18);
            Rectangle xpPlus = new Rectangle(x + 24, y + 18, 20, 18);
            Rectangle xpFill = new Rectangle(x + 48, y + 18, 54, 18);

            if (DrawButton(spriteBatch, xpMinus, "-", false, true, 0.75f))
                AdjustXP(rpgPlayer, -xpStep);
            if (DrawButton(spriteBatch, xpPlus, "+", false, true, 0.75f))
                AdjustXP(rpgPlayer, xpStep);
            if (DrawButton(spriteBatch, xpFill, "Fill", false, true, 0.7f))
                AdjustXP(rpgPlayer, rpgPlayer.RequiredXP);

            DrawText(spriteBatch, $"XP Step: {xpStep}", new Vector2(x + 110, y + 20), Color.Gray, 0.7f);
            y += 42;

            DrawText(spriteBatch, $"Stat Points: {rpgPlayer.StatPoints} (Pending {rpgPlayer.PendingStatPoints})", new Vector2(x, y), Color.White, 0.8f);
            Rectangle spMinus = new Rectangle(x, y + 18, 20, 18);
            Rectangle spPlus = new Rectangle(x + 24, y + 18, 20, 18);
            Rectangle spRelease = new Rectangle(x + 48, y + 18, 68, 18);

            if (DrawButton(spriteBatch, spMinus, "-", false, true, 0.75f))
                rpgPlayer.StatPoints = Math.Max(0, rpgPlayer.StatPoints - step);
            if (DrawButton(spriteBatch, spPlus, "+", false, true, 0.75f))
                rpgPlayer.StatPoints = Math.Max(0, rpgPlayer.StatPoints + step);
            if (DrawButton(spriteBatch, spRelease, "Release", false, true, 0.65f))
                rpgPlayer.ReleasePendingPoints();

            y += 42;

            DrawText(spriteBatch, $"Skill Points: {rpgPlayer.SkillPoints} (Pending {rpgPlayer.PendingSkillPoints})", new Vector2(x, y), Color.White, 0.8f);
            Rectangle skMinus = new Rectangle(x, y + 18, 20, 18);
            Rectangle skPlus = new Rectangle(x + 24, y + 18, 20, 18);
            Rectangle skRelease = new Rectangle(x + 48, y + 18, 68, 18);

            if (DrawButton(spriteBatch, skMinus, "-", false, true, 0.75f))
                rpgPlayer.SkillPoints = Math.Max(0, rpgPlayer.SkillPoints - step);
            if (DrawButton(spriteBatch, skPlus, "+", false, true, 0.75f))
                rpgPlayer.SkillPoints = Math.Max(0, rpgPlayer.SkillPoints + step);
            if (DrawButton(spriteBatch, skRelease, "Release", false, true, 0.65f))
                rpgPlayer.ReleasePendingPoints();
        }

        private void DrawJobTab(SpriteBatch spriteBatch, int x, int y)
        {
            Player player = Main.LocalPlayer;
            RpgPlayer rpgPlayer = player.GetModPlayer<RpgPlayer>();
            if (rpgPlayer == null)
                return;

            if (jobList.Length == 0)
            {
                DrawText(spriteBatch, "No jobs loaded.", new Vector2(x, y), Color.Orange, 0.8f);
                return;
            }

            if (Array.IndexOf(jobList, previewJob) < 0)
                previewJob = jobList[0];

            DrawText(spriteBatch, $"Current: {rpgPlayer.CurrentJob} ({rpgPlayer.CurrentTier})", new Vector2(x, y), Color.White, 0.8f);
            y += 18;
            DrawText(spriteBatch, $"Select: {previewJob} ({RpgFormulas.GetJobTier(previewJob)})", new Vector2(x, y), Color.LightGray, 0.75f);
            y += 20;

            Rectangle prevBtn = new Rectangle(x, y, 22, 18);
            Rectangle nextBtn = new Rectangle(x + 26, y, 22, 18);
            Rectangle setBtn = new Rectangle(x + 52, y, 54, 18);
            Rectangle advBtn = new Rectangle(x + 110, y, 70, 18);

            if (DrawButton(spriteBatch, prevBtn, "<", false, true, 0.75f))
                previewJob = GetNextJob(previewJob, -1);
            if (DrawButton(spriteBatch, nextBtn, ">", false, true, 0.75f))
                previewJob = GetNextJob(previewJob, 1);
            if (DrawButton(spriteBatch, setBtn, "Set", false, true, 0.7f))
                SetJob(rpgPlayer, previewJob);
            if (DrawButton(spriteBatch, advBtn, "Advance", false, true, 0.6f))
                player.GetModPlayer<PlayerLevel>().AdvanceJob(previewJob);

            y += 24;

            List<string> missing = JobDatabase.GetMissingRequirementDescriptions(rpgPlayer, previewJob);
            if (missing.Count > 0)
                DrawText(spriteBatch, "Missing: " + string.Join(", ", missing), new Vector2(x, y), Color.Orange, 0.7f);
            else
                DrawText(spriteBatch, "Requirements: OK", new Vector2(x, y), Color.LightGreen, 0.7f);
        }

        private void DrawStatsTab(SpriteBatch spriteBatch, int x, int y)
        {
            Player player = Main.LocalPlayer;
            RpgPlayer rpgPlayer = player.GetModPlayer<RpgPlayer>();
            if (rpgPlayer == null)
                return;

            int step = GetStep();

            string editLabel = editAutoStats ? "Edit: Auto" : "Edit: Base";
            if (DrawButton(spriteBatch, new Rectangle(x, y, 80, 18), editLabel, true, true, 0.6f))
                editAutoStats = !editAutoStats;

            Rectangle resetBase = new Rectangle(x + 86, y, 76, 18);
            Rectangle resetAuto = new Rectangle(x + 168, y, 76, 18);

            if (DrawButton(spriteBatch, resetBase, "Reset B", false, true, 0.6f))
                rpgPlayer.ResetStats();
            if (DrawButton(spriteBatch, resetAuto, "Reset A", false, true, 0.6f))
                ResetAutoStats(rpgPlayer);

            DrawText(spriteBatch, "Value: Base/Auto", new Vector2(x, y + 22), Color.Gray, 0.7f);
            y += 40;

            int colWidth = (bounds.Width - 20 - 12) / 2;
            int rowHeight = 18;

            for (int i = 0; i < statList.Length; i++)
            {
                int col = i / 6;
                int row = i % 6;
                int rowY = y + row * rowHeight;
                int colX = x + col * (colWidth + 12);

                StatType stat = statList[i];
                int baseValue = rpgPlayer.GetBaseStatValue(stat);
                int autoValue = rpgPlayer.GetAutoStatValue(stat);

                DrawText(spriteBatch, $"{GetStatShortName(stat)} {baseValue}/{autoValue}", new Vector2(colX, rowY), Color.White, 0.7f);

                Rectangle minusBtn = new Rectangle(colX + colWidth - 40, rowY, 18, 16);
                Rectangle plusBtn = new Rectangle(colX + colWidth - 20, rowY, 18, 16);

                if (DrawButton(spriteBatch, minusBtn, "-", false, true, 0.7f))
                    AdjustStatValue(rpgPlayer, stat, -step, editAutoStats);
                if (DrawButton(spriteBatch, plusBtn, "+", false, true, 0.7f))
                    AdjustStatValue(rpgPlayer, stat, step, editAutoStats);
            }
        }

        private BiomeEntry GetActiveBiome(out string sourceInfo)
        {
            sourceInfo = source.ToString();

            if (source == BiomeSource.Preview)
                return GetPreviewBiomeEntry();

            if (source == BiomeSource.Mouse)
            {
                int tileX = (int)(Main.MouseWorld.X / 16f);
                int tileY = (int)(Main.MouseWorld.Y / 16f);
                sourceInfo = $"Mouse {tileX},{tileY}";
                return GetBiomeEntryAt(tileX, tileY);
            }

            var player = Main.LocalPlayer;
            int px = (int)(player.Center.X / 16f);
            int py = (int)(player.Center.Y / 16f);
            sourceInfo = $"Player {px},{py}";
            return GetBiomeEntryAt(px, py);
        }

        private BiomeEntry GetBiomeEntryAt(int tileX, int tileY)
        {
            BiomeType vanillaBiome = BiomeLevelSystem.GetBiomeAt(tileX, tileY);
            if (BiomeLevelSystem.IsEventBiome(vanillaBiome))
                return new BiomeEntry(vanillaBiome);

            ModBiome modBiome = BiomeLevelSystem.GetActiveModBiomeAt(tileX, tileY);
            if (modBiome != null)
                return new BiomeEntry(modBiome);

            return new BiomeEntry(vanillaBiome);
        }

        private BiomeEntry GetPreviewBiomeEntry()
        {
            if (biomeList.Length == 0 || previewBiomeIndex < 0)
                return new BiomeEntry(BiomeType.Forest);

            if (previewBiomeIndex >= biomeList.Length)
                previewBiomeIndex = biomeList.Length - 1;

            return biomeList[previewBiomeIndex];
        }

        private void MovePreviewBiome(int direction)
        {
            if (biomeList.Length == 0)
                return;

            int next = previewBiomeIndex + direction;
            if (next < 0)
                next = biomeList.Length - 1;
            if (next >= biomeList.Length)
                next = 0;

            previewBiomeIndex = next;
        }

        private int GetStep()
        {
            if (Main.keyState.IsKeyDown(Keys.LeftControl) || Main.keyState.IsKeyDown(Keys.RightControl))
                return 10;
            if (Main.keyState.IsKeyDown(Keys.LeftShift) || Main.keyState.IsKeyDown(Keys.RightShift))
                return 5;
            return 1;
        }

        private long GetXpStep(RpgPlayer rpgPlayer, int step)
        {
            long baseStep = Math.Max(1, rpgPlayer.RequiredXP / 10);
            return baseStep * step;
        }

        private void SetLevel(RpgPlayer rpgPlayer, int level)
        {
            int maxLevel = RpgFormulas.GetMaxLevel();
            rpgPlayer.Level = Utils.Clamp(level, 1, maxLevel);
            long maxXp = Math.Max(0, rpgPlayer.RequiredXP - 1);
            if (rpgPlayer.CurrentXP > maxXp)
                rpgPlayer.CurrentXP = maxXp;
        }

        private void AdjustXP(RpgPlayer rpgPlayer, long delta)
        {
            long maxXp = Math.Max(0, rpgPlayer.RequiredXP - 1);
            long newXp = rpgPlayer.CurrentXP + delta;
            if (newXp < 0)
                newXp = 0;
            if (newXp > maxXp)
                newXp = maxXp;
            rpgPlayer.CurrentXP = newXp;
        }

        private string GetBiomeLabel(BiomeEntry entry)
        {
            if (entry.IsMod && entry.ModBiome != null)
            {
                string modName = entry.ModBiome.Mod?.Name ?? "Mod";
                string biomeName = entry.ModBiome.DisplayName?.Value ?? entry.ModBiome.Name;
                return $"{modName}::{biomeName}";
            }

            return entry.VanillaBiome.ToString();
        }

        private JobType GetNextJob(JobType current, int direction)
        {
            if (jobList.Length == 0)
                return current;

            int index = Array.IndexOf(jobList, current);
            if (index < 0)
                index = 0;

            int next = index + direction;
            if (next < 0)
                next = jobList.Length - 1;
            if (next >= jobList.Length)
                next = 0;

            return jobList[next];
        }

        private void SetJob(RpgPlayer rpgPlayer, JobType job)
        {
            JobTier oldTier = rpgPlayer.CurrentTier;
            rpgPlayer.CurrentJob = job;
            if (RpgFormulas.GetJobTier(job) > oldTier)
                rpgPlayer.ReleasePendingPoints();
        }

        private void AdjustStatValue(RpgPlayer rpgPlayer, StatType stat, int delta, bool auto)
        {
            if (auto)
            {
                int value = rpgPlayer.GetAutoStatValue(stat) + delta;
                SetAutoStatValue(rpgPlayer, stat, value);
            }
            else
            {
                int value = rpgPlayer.GetBaseStatValue(stat) + delta;
                SetBaseStatValue(rpgPlayer, stat, value);
            }
        }

        private void SetBaseStatValue(RpgPlayer rpgPlayer, StatType stat, int value)
        {
            value = Math.Max(0, value);

            switch (stat)
            {
                case StatType.Strength: rpgPlayer.Strength = value; break;
                case StatType.Dexterity: rpgPlayer.Dexterity = value; break;
                case StatType.Rogue: rpgPlayer.Rogue = value; break;
                case StatType.Intelligence: rpgPlayer.Intelligence = value; break;
                case StatType.Focus: rpgPlayer.Focus = value; break;
                case StatType.Vitality: rpgPlayer.Vitality = value; break;
                case StatType.Stamina: rpgPlayer.StaminaStat = value; break;
                case StatType.Defense: rpgPlayer.Defense = value; break;
                case StatType.Agility: rpgPlayer.Agility = value; break;
                case StatType.Wisdom: rpgPlayer.Wisdom = value; break;
                case StatType.Fortitude: rpgPlayer.Fortitude = value; break;
                case StatType.Luck: rpgPlayer.Luck = value; break;
            }
        }

        private void SetAutoStatValue(RpgPlayer rpgPlayer, StatType stat, int value)
        {
            value = Math.Max(0, value);

            switch (stat)
            {
                case StatType.Strength: rpgPlayer.AutoStrength = value; break;
                case StatType.Dexterity: rpgPlayer.AutoDexterity = value; break;
                case StatType.Rogue: rpgPlayer.AutoRogue = value; break;
                case StatType.Intelligence: rpgPlayer.AutoIntelligence = value; break;
                case StatType.Focus: rpgPlayer.AutoFocus = value; break;
                case StatType.Vitality: rpgPlayer.AutoVitality = value; break;
                case StatType.Stamina: rpgPlayer.AutoStamina = value; break;
                case StatType.Defense: rpgPlayer.AutoDefense = value; break;
                case StatType.Agility: rpgPlayer.AutoAgility = value; break;
                case StatType.Wisdom: rpgPlayer.AutoWisdom = value; break;
                case StatType.Fortitude: rpgPlayer.AutoFortitude = value; break;
                case StatType.Luck: rpgPlayer.AutoLuck = value; break;
            }
        }

        private void ResetAutoStats(RpgPlayer rpgPlayer)
        {
            rpgPlayer.AutoStrength = 0;
            rpgPlayer.AutoDexterity = 0;
            rpgPlayer.AutoRogue = 0;
            rpgPlayer.AutoIntelligence = 0;
            rpgPlayer.AutoFocus = 0;
            rpgPlayer.AutoVitality = 0;
            rpgPlayer.AutoStamina = 0;
            rpgPlayer.AutoDefense = 0;
            rpgPlayer.AutoAgility = 0;
            rpgPlayer.AutoWisdom = 0;
            rpgPlayer.AutoFortitude = 0;
            rpgPlayer.AutoLuck = 0;
        }

        private string GetStatShortName(StatType stat)
        {
            return stat switch
            {
                StatType.Strength => "STR",
                StatType.Dexterity => "DEX",
                StatType.Rogue => "ROG",
                StatType.Intelligence => "INT",
                StatType.Focus => "FOC",
                StatType.Vitality => "VIT",
                StatType.Stamina => "STA",
                StatType.Defense => "DEF",
                StatType.Agility => "AGI",
                StatType.Wisdom => "WIS",
                StatType.Fortitude => "FOR",
                StatType.Luck => "LUK",
                _ => "?"
            };
        }

        private bool DrawButton(SpriteBatch spriteBatch, Rectangle rect, string text, bool selected, bool enabled, float scale)
        {
            bool hover = rect.Contains(Main.mouseX, Main.mouseY);

            Color bg;
            if (!enabled)
                bg = new Color(40, 40, 50);
            else if (selected)
                bg = new Color(90, 120, 170);
            else if (hover)
                bg = new Color(70, 90, 130);
            else
                bg = new Color(50, 70, 100);

            Color border = selected ? new Color(140, 170, 220) : new Color(90, 110, 150);
            DrawPanel(spriteBatch, rect, bg, border);

            Vector2 textSize = Terraria.GameContent.FontAssets.MouseText.Value.MeasureString(text) * scale;
            Vector2 textPos = new Vector2(rect.X + (rect.Width - textSize.X) / 2f, rect.Y + (rect.Height - textSize.Y) / 2f);
            DrawText(spriteBatch, text, textPos, Color.White, scale);

            return enabled && hover && Main.mouseLeft && Main.mouseLeftRelease;
        }

        private void DrawPanel(SpriteBatch spriteBatch, Rectangle rect, Color bgColor, Color borderColor)
        {
            Texture2D pixel = Terraria.GameContent.TextureAssets.MagicPixel.Value;
            spriteBatch.Draw(pixel, rect, bgColor);

            spriteBatch.Draw(pixel, new Rectangle(rect.X, rect.Y, rect.Width, 2), borderColor);
            spriteBatch.Draw(pixel, new Rectangle(rect.X, rect.Bottom - 2, rect.Width, 2), borderColor);
            spriteBatch.Draw(pixel, new Rectangle(rect.X, rect.Y, 2, rect.Height), borderColor);
            spriteBatch.Draw(pixel, new Rectangle(rect.Right - 2, rect.Y, 2, rect.Height), borderColor);
        }

        private void DrawText(SpriteBatch spriteBatch, string text, Vector2 position, Color color, float scale)
        {
            Utils.DrawBorderStringFourWay(
                spriteBatch,
                Terraria.GameContent.FontAssets.MouseText.Value,
                text,
                position.X,
                position.Y,
                color,
                Color.Black,
                Vector2.Zero,
                scale);
        }
    }
}
