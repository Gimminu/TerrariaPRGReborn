using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using RpgMod.Content.Buffs;

namespace RpgMod.Content.Items.Tools
{
    public class MonsterMagnet : ModItem
    {
        // 텍스처 파일이 없으므로 바닐라 '천상의 자석' 텍스처를 임시로 사용
        public override string Texture => "Terraria/Images/Item_" + ItemID.CelestialMagnet;

        public override void SetDefaults()
        {
            Item.width = 32;
            Item.height = 32;
            Item.useTime = 20;
            Item.useAnimation = 20;
            Item.useStyle = ItemUseStyleID.HoldUp;
            Item.value = Item.sellPrice(gold: 1);
            Item.rare = ItemRarityID.Pink;
            Item.UseSound = SoundID.Item4; // Crystal sound
            Item.autoReuse = false;
        }

        public override bool? UseItem(Player player)
        {
            if (player.whoAmI == Main.myPlayer)
            {
                // Toggle buff
                int buffType = ModContent.BuffType<MonsterMagnetBuff>();
                if (player.HasBuff(buffType))
                {
                    player.ClearBuff(buffType);
                    Terraria.Audio.SoundEngine.PlaySound(SoundID.MenuClose);
                    Main.NewText("Monster Magnet Deactivated", Color.Gray);
                }
                else
                {
                    player.AddBuff(buffType, 2); // 2 ticks, but buff logic will keep it infinite
                    Terraria.Audio.SoundEngine.PlaySound(SoundID.MenuOpen);
                    Main.NewText("Monster Magnet Activated! Spawn rate drastically increased.", Color.Red);
                    
                    // Visual Burst
                    for (int i = 0; i < 30; i++)
                    {
                        Vector2 velocity = Main.rand.NextVector2Circular(5f, 5f);
                        Dust dust = Dust.NewDustDirect(player.Center, 0, 0, DustID.LifeDrain, velocity.X, velocity.Y, 100, default, 1.5f);
                        dust.noGravity = true;
                    }
                }
            }
            return true;
        }
        
        public override void AddRecipes()
        {
            CreateRecipe()
                .AddIngredient(ItemID.WaterCandle, 1)
                .AddIngredient(ItemID.BattlePotion, 5)
                .AddIngredient(ItemID.SoulofNight, 5)
                .AddTile(TileID.DemonAltar)
                .Register();
        }
    }
}
