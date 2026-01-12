using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Rpg.Common;
using Rpg.Common.Players;
using Rpg.Common.UI;
using Rpg.Common.Jobs;

namespace Rpg.Content.Items
{
    /// <summary>
    /// Base class for job advancement scrolls
    /// </summary>
    public abstract class JobAdvancementScroll : ModItem
    {
        public abstract JobTier RequiredTier { get; }
        public abstract string ScrollName { get; }
        public abstract string ScrollDescription { get; }
        
        public override void SetDefaults()
        {
            Item.width = 28;
            Item.height = 32;
            Item.maxStack = 1;
            Item.value = Item.buyPrice(gold: GetTierValue());
            Item.rare = GetRarity();
            Item.useAnimation = 30;
            Item.useTime = 30;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.consumable = false; // Not consumed, just opens UI
            Item.noUseGraphic = false;
        }
        
        private int GetTierValue()
        {
            return RequiredTier switch
            {
                JobTier.Novice => 5,
                JobTier.Tier1 => 10,
                JobTier.Tier2 => 15,
                JobTier.Tier3 => 20,
                _ => 5
            };
        }
        
        private int GetRarity()
        {
            return RequiredTier switch
            {
                JobTier.Novice => ItemRarityID.White,
                JobTier.Tier1 => ItemRarityID.Green,
                JobTier.Tier2 => ItemRarityID.Orange,
                JobTier.Tier3 => ItemRarityID.LightPurple,
                _ => ItemRarityID.White
            };
        }
        
        public override bool CanUseItem(Player player)
        {
            var rpgPlayer = player.GetModPlayer<RpgPlayer>();
            
            // Check if player is at correct tier
            if (rpgPlayer.CurrentTier != RequiredTier)
            {
                Main.NewText($"You need to be Tier {RequiredTier} to use this scroll!", Color.Red);
                return false;
            }
            
            // Check if there are available jobs to advance to
            var availableJobs = JobDatabase.GetAvailableJobs(rpgPlayer);
            if (availableJobs == null || availableJobs.Count == 0)
            {
                Main.NewText("No job advancement available at this time.", Color.Yellow);
                return false;
            }
            
            return true;
        }
        
        public override bool? UseItem(Player player)
        {
            if (Main.netMode == NetmodeID.Server)
                return false;
            
            // Open job advancement UI
            var uiSystem = ModContent.GetInstance<JobUISystem>();
            uiSystem?.ToggleUI();
            
            return true;
        }
        
        public override void ModifyTooltips(System.Collections.Generic.List<TooltipLine> tooltips)
        {
            var rpgPlayer = Main.LocalPlayer?.GetModPlayer<RpgPlayer>();
            if (rpgPlayer == null)
                return;
            
            // Show requirements
            Color reqColor = rpgPlayer.CurrentTier == RequiredTier ? Color.Green : Color.Red;
            string status = rpgPlayer.CurrentTier == RequiredTier ? "✓" : "✗";
            
            tooltips.Add(new TooltipLine(Mod, "Requirement", 
                $"Requires: Tier {RequiredTier} {status}") { OverrideColor = reqColor });
            
            // Show available jobs preview
            var availableJobs = JobDatabase.GetAvailableJobs(rpgPlayer);
            if (availableJobs != null && availableJobs.Count > 0)
            {
                tooltips.Add(new TooltipLine(Mod, "AvailableJobs", 
                    $"Available Jobs: {availableJobs.Count}") { OverrideColor = Color.Cyan });
            }
        }
    }
    
    /// <summary>
    /// Tier 0 -> Tier 1 advancement (Novice -> Warrior/Ranger/Mage/Summoner)
    /// </summary>
    public class FirstJobScroll : JobAdvancementScroll
    {
        public override JobTier RequiredTier => JobTier.Novice;
        public override string ScrollName => "First Job Scroll";
        public override string ScrollDescription => "Choose your first job class";
        
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("First Job Advancement Scroll");
            // Tooltip.SetDefault("Use to choose your first job class\nRequires Level 10+");
        }
        
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.FallenStar, 5)
                .AddIngredient(ItemID.Silk, 10)
                .AddTile(TileID.WorkBenches)
                .Register();
        }
    }
    
    /// <summary>
    /// Tier 1 -> Tier 2 advancement (Warrior -> Knight/Berserker, etc.)
    /// </summary>
    public class SecondJobScroll : JobAdvancementScroll
    {
        public override JobTier RequiredTier => JobTier.Tier1;
        public override string ScrollName => "Second Job Scroll";
        public override string ScrollDescription => "Advance to a specialized job class";
        
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Second Job Advancement Scroll");
            // Tooltip.SetDefault("Use to advance to your second job\nRequires defeating Wall of Flesh");
        }
        
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.SoulofNight, 10)
                .AddIngredient(ItemID.SoulofLight, 10)
                .AddIngredient(ItemID.Silk, 20)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }
    
    /// <summary>
    /// Tier 2 -> Tier 3 advancement (Knight -> Guardian, etc.)
    /// </summary>
    public class ThirdJobScroll : JobAdvancementScroll
    {
        public override JobTier RequiredTier => JobTier.Tier2;
        public override string ScrollName => "Third Job Scroll";
        public override string ScrollDescription => "Master your ultimate job class";
        
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Third Job Advancement Scroll");
            // Tooltip.SetDefault("Use to advance to your final job\nRequires defeating Plantera");
        }
        
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.ChlorophyteBar, 15)
                .AddIngredient(ItemID.SoulofMight, 5)
                .AddIngredient(ItemID.SoulofSight, 5)
                .AddIngredient(ItemID.SoulofFright, 5)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }
    
    /// <summary>
    /// Special item that opens job UI at any time (for viewing info)
    /// </summary>
    public class JobManual : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Job Manual");
            // Tooltip.SetDefault("Reference guide for all job classes\nRight-click to view job tree");
        }
        
        public override void SetDefaults()
        {
            Item.width = 28;
            Item.height = 32;
            Item.maxStack = 1;
            Item.value = Item.buyPrice(gold: 1);
            Item.rare = ItemRarityID.Blue;
            Item.useAnimation = 20;
            Item.useTime = 20;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.consumable = false;
        }
        
        public override bool? UseItem(Player player)
        {
            if (Main.netMode == NetmodeID.Server)
                return false;
            
            var uiSystem = ModContent.GetInstance<JobUISystem>();
            uiSystem?.ToggleUI();
            
            return true;
        }
        
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.Book, 1)
                .AddIngredient(ItemID.Silk, 5)
                .AddTile(TileID.WorkBenches)
                .Register();
        }
    }
}
