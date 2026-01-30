using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Rpg.Common.Players;
using Rpg.Common.Config;

namespace Rpg.Content.Items
{
    /// <summary>
    /// Stat Reset Orb - Allows player to reset all manually allocated stat points
    /// Per design doc: "특정 아이템으로 스탯 초기화(Reset)를 가능하게"
    /// </summary>
    public class StatResetOrb : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 32;
            Item.height = 32;
            Item.useTime = 45;
            Item.useAnimation = 45;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.consumable = true;
            Item.maxStack = 10;
            Item.rare = ItemRarityID.Orange;
            Item.value = Item.sellPrice(gold: 5);
            Item.UseSound = SoundID.Item4;
        }

        public override bool CanUseItem(Player player)
        {
            var rpgPlayer = player.GetModPlayer<RpgPlayer>();
            // Only allow use if player has allocated stats
            return rpgPlayer.GetTotalAllocatedStats() > 0;
        }

        public override bool? UseItem(Player player)
        {
            var rpgPlayer = player.GetModPlayer<RpgPlayer>();
            
            // Get total allocated for refund
            int totalPoints = rpgPlayer.GetTotalAllocatedStats();
            
            if (totalPoints > 0)
            {
                // Reset all stats
                rpgPlayer.ResetStats();
                
                // Show message
                Main.NewText($"All stat points have been reset! You now have {rpgPlayer.StatPoints} points to allocate.", 
                    Color.Gold);
                
                // Visual effect
                for (int i = 0; i < 30; i++)
                {
                    Dust dust = Dust.NewDustDirect(
                        player.position, 
                        player.width, 
                        player.height, 
                        DustID.GoldCoin, 
                        Main.rand.NextFloat(-3f, 3f), 
                        Main.rand.NextFloat(-5f, -2f),
                        100,
                        default,
                        1.5f
                    );
                    dust.noGravity = true;
                }
                
                return true;
            }
            
            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.FallenStar, 10)
                .AddIngredient(ItemID.GoldBar, 5)
                .AddIngredient(ItemID.Diamond, 3)
                .AddTile(TileID.Anvils)
                .Register();
                
            // Alternative with platinum
            CreateRecipe()
                .AddIngredient(ItemID.FallenStar, 10)
                .AddIngredient(ItemID.PlatinumBar, 5)
                .AddIngredient(ItemID.Diamond, 3)
                .AddTile(TileID.Anvils)
                .Register();
        }
    }
    
    /// <summary>
    /// Skill Reset Scroll - Allows player to reset all skill points
    /// </summary>
    public class SkillResetScroll : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 28;
            Item.height = 36;
            Item.useTime = 45;
            Item.useAnimation = 45;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.consumable = true;
            Item.maxStack = 10;
            Item.rare = ItemRarityID.LightRed;
            Item.value = Item.sellPrice(gold: 10);
            Item.UseSound = SoundID.Item4;
        }

        public override bool CanUseItem(Player player)
        {
            var serverConfig = ModContent.GetInstance<RpgServerConfig>();
            if (serverConfig != null && !serverConfig.EnableSkillSystem)
            {
                Main.NewText("Skill system is disabled on this server.", Color.Red);
                return false;
            }
            
            var rpgPlayer = player.GetModPlayer<RpgPlayer>();
            // Only allow if player has learned skills
            return rpgPlayer.GetTotalAllocatedSkillPoints() > 0;
        }

        public override bool? UseItem(Player player)
        {
            var rpgPlayer = player.GetModPlayer<RpgPlayer>();
            
            int totalSkillPoints = rpgPlayer.GetTotalAllocatedSkillPoints();
            
            if (totalSkillPoints > 0)
            {
                // Reset all skills
                rpgPlayer.ResetSkills();
                
                Main.NewText($"All skills have been reset! You now have {rpgPlayer.SkillPoints} skill points to allocate.", 
                    Color.Cyan);
                
                // Visual effect - magic sparkles
                for (int i = 0; i < 40; i++)
                {
                    Dust dust = Dust.NewDustDirect(
                        player.position, 
                        player.width, 
                        player.height, 
                        DustID.MagicMirror, 
                        Main.rand.NextFloat(-4f, 4f), 
                        Main.rand.NextFloat(-6f, -2f),
                        150,
                        default,
                        1.2f
                    );
                    dust.noGravity = true;
                }
                
                return true;
            }
            
            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.SpellTome, 1)
                .AddIngredient(ItemID.SoulofLight, 5)
                .AddIngredient(ItemID.SoulofNight, 5)
                .AddTile(TileID.Bookcases)
                .Register();
        }
    }
    
    /// <summary>
    /// Complete Reset Crystal - Resets both stats AND skills
    /// Rare endgame item
    /// </summary>
    public class CompleteResetCrystal : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 24;
            Item.height = 28;
            Item.useTime = 60;
            Item.useAnimation = 60;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.consumable = true;
            Item.maxStack = 5;
            Item.rare = ItemRarityID.Lime;
            Item.value = Item.sellPrice(gold: 25);
            Item.UseSound = SoundID.Item119;
        }

        public override bool CanUseItem(Player player)
        {
            var rpgPlayer = player.GetModPlayer<RpgPlayer>();
            return rpgPlayer.GetTotalAllocatedStats() > 0 || rpgPlayer.GetTotalAllocatedSkillPoints() > 0;
        }

        public override bool? UseItem(Player player)
        {
            var rpgPlayer = player.GetModPlayer<RpgPlayer>();
            
            bool hadStats = rpgPlayer.GetTotalAllocatedStats() > 0;
            bool hadSkills = rpgPlayer.GetTotalAllocatedSkillPoints() > 0;
            
            if (hadStats || hadSkills)
            {
                if (hadStats)
                    rpgPlayer.ResetStats();
                if (hadSkills)
                    rpgPlayer.ResetSkills();
                
                Main.NewText("Complete reset! All stats and skills have been refunded.", Color.Magenta);
                Main.NewText($"Stat Points: {rpgPlayer.StatPoints} | Skill Points: {rpgPlayer.SkillPoints}", Color.White);
                
                // Grand visual effect
                for (int i = 0; i < 60; i++)
                {
                    Dust dust = Dust.NewDustDirect(
                        player.position - new Vector2(20, 20), 
                        player.width + 40, 
                        player.height + 40, 
                        DustID.RainbowMk2, 
                        Main.rand.NextFloat(-5f, 5f), 
                        Main.rand.NextFloat(-8f, -3f),
                        100,
                        default,
                        1.8f
                    );
                    dust.noGravity = true;
                }
                
                return true;
            }
            
            return false;
        }

        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.LunarBar, 5)
                .AddIngredient(ItemID.FragmentSolar, 2)
                .AddIngredient(ItemID.FragmentVortex, 2)
                .AddIngredient(ItemID.FragmentNebula, 2)
                .AddIngredient(ItemID.FragmentStardust, 2)
                .AddTile(TileID.LunarCraftingStation)
                .Register();
        }
    }
}
