using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using RpgMod.Common.Base;

namespace RpgMod.Common.Skills.Novice
{
    /// <summary>
    /// Resourceful - Novice utility/economy passive.
    /// </summary>
    public class Resourceful : BaseSkill
    {
        public override string InternalName => "Resourceful";
        public override string DisplayName => "Resourceful";
        public override string Description => "Increase luck and mining efficiency while exploring.";

        public override SkillType SkillType => SkillType.Passive;
        public override JobType RequiredJob => JobType.Novice;
        public override int RequiredLevel => 4;
        public override int SkillPointCost => 1;
        public override int MaxRank => 5;
        public override string IconTexture => $"{RpgConstants.ICON_PATH}Skills/Resourceful";
        public override float CooldownSeconds => 0f;
        public override int ResourceCost => 0;
        public override ResourceType ResourceType => ResourceType.None;

        private static readonly float[] LUCK_BONUS = { 0.05f, 0.10f, 0.15f, 0.20f, 0.30f };
        private static readonly float[] MINING_SPEED = { 0.05f, 0.10f, 0.15f, 0.20f, 0.25f };
        private static readonly float[] RESOURCE_DAMAGE = { 1.2f, 1.4f, 1.6f, 1.8f, 2.0f };
        private static readonly int[] COIN_BONUS_CHANCE = { 5, 8, 12, 16, 20 };

        private int bonusCoinsGained;
        private int resourcesGathered;

        public override void ApplyPassive(Player player)
        {
            if (CurrentRank <= 0)
                return;

            int rank = CurrentRank;

            player.luck += LUCK_BONUS[rank - 1];
            player.pickSpeed -= MINING_SPEED[rank - 1];

            if (Main.GameUpdateCount % 300 == 0 && (bonusCoinsGained > 0 || resourcesGathered > 0))
            {
                ShowSessionStats(player);
            }
        }

        /// <summary>
        /// Called when player breaks a tile (hook in ModPlayer).
        /// </summary>
        public void OnBreakTile(Player player, int x, int y, int type)
        {
            if (CurrentRank <= 0)
                return;

            int rank = CurrentRank;
            bool isResource = IsResourceTile(type);

            if (!isResource)
                return;

            resourcesGathered++;

            int coinChance = COIN_BONUS_CHANCE[rank - 1];
            if (Main.rand.Next(100) < coinChance)
            {
                int coins = Main.rand.Next(1, 4);
                bonusCoinsGained += coins;

                Item.NewItem(
                    player.GetSource_Misc("Resourceful"),
                    new Vector2(x * 16, y * 16),
                    16,
                    16,
                    ItemID.CopperCoin,
                    coins
                );

                Dust.NewDust(
                    new Vector2(x * 16, y * 16),
                    16,
                    16,
                    DustID.GoldCoin,
                    0,
                    -2f,
                    100,
                    Color.Gold,
                    1.0f
                );
            }

            if (resourcesGathered % 10 == 0)
            {
                ShowMessage(player, $"+{resourcesGathered} resources gathered!", Color.LightGreen);
                CreateGatheringEffect(player);
            }
        }

        public float GetResourceDamageMultiplier()
        {
            if (CurrentRank <= 0)
                return 1f;

            return RESOURCE_DAMAGE[CurrentRank - 1];
        }

        private bool IsResourceTile(int tileType)
        {
            return tileType == TileID.Trees ||
                   tileType == TileID.Copper ||
                   tileType == TileID.Iron ||
                   tileType == TileID.Silver ||
                   tileType == TileID.Gold ||
                   tileType == TileID.Tin ||
                   tileType == TileID.Lead ||
                   tileType == TileID.Tungsten ||
                   tileType == TileID.Platinum ||
                   tileType == TileID.Meteorite ||
                   tileType == TileID.Demonite ||
                   tileType == TileID.Crimtane ||
                   tileType == TileID.Obsidian ||
                   tileType == TileID.Hellstone ||
                   tileType == TileID.Cobalt ||
                   tileType == TileID.Mythril ||
                   tileType == TileID.Adamantite ||
                   tileType == TileID.Chlorophyte;
        }

        private void ShowSessionStats(Player player)
        {
            string stats = $"Resources: {resourcesGathered} | Bonus Coins: {bonusCoinsGained}";
            ShowMessage(player, stats, Color.LightYellow);
        }

        private void CreateGatheringEffect(Player player)
        {
            SoundEngine.PlaySound(SoundID.CoinPickup, player.position);

            for (int i = 0; i < 8; i++)
            {
                Dust dust = Dust.NewDustDirect(
                    player.position,
                    player.width,
                    player.height,
                    DustID.GoldFlame,
                    Main.rand.NextFloat(-2f, 2f),
                    Main.rand.NextFloat(-3f, -1f),
                    100,
                    Color.Gold,
                    1.2f
                );
                dust.noGravity = true;
            }
        }

        private void ShowMessage(Player player, string text, Color color)
        {
            if (Main.netMode != NetmodeID.Server)
            {
                CombatText.NewText(player.Hitbox, color, text, false, false);
            }
        }
    }
}
