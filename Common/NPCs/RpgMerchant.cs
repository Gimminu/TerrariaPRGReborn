using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Rpg.Content.Items;

namespace Rpg.Common.NPCs
{
    /// <summary>
    /// RPG Shop NPC - Sells job advancement items, reset items, and skill books
    /// </summary>
    [AutoloadHead]
    public class RpgMerchant : ModNPC
    {
        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[Type] = 25;
            NPCID.Sets.ExtraFramesCount[Type] = 9;
            NPCID.Sets.AttackFrameCount[Type] = 4;
            NPCID.Sets.DangerDetectRange[Type] = 700;
            NPCID.Sets.AttackType[Type] = 0; // Throwing
            NPCID.Sets.AttackTime[Type] = 90;
            NPCID.Sets.AttackAverageChance[Type] = 30;
            NPCID.Sets.HatOffsetY[Type] = 4;
            
            // NPC Happiness
            NPCID.Sets.NPCBestiaryDrawModifiers drawModifiers = new NPCID.Sets.NPCBestiaryDrawModifiers()
            {
                Velocity = 1f,
                Direction = 1
            };
            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, drawModifiers);
        }

        public override void SetDefaults()
        {
            NPC.townNPC = true;
            NPC.friendly = true;
            NPC.width = 18;
            NPC.height = 40;
            NPC.aiStyle = NPCAIStyleID.Passive; // Town NPC walk/idle behavior
            AIType = NPCID.Guide; // Use Guide movement/AI (prevents slime hopping)
            NPC.damage = 10;
            NPC.defense = 15;
            NPC.lifeMax = 250;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath1;
            NPC.knockBackResist = 0.5f;
            AnimationType = NPCID.Guide;
        }

        public override bool CanTownNPCSpawn(int numTownNPCs)
        {
            // Spawns after any player has reached level 10
            for (int i = 0; i < Main.maxPlayers; i++)
            {
                Player player = Main.player[i];
                if (player.active)
                {
                    var rpgPlayer = player.GetModPlayer<Players.RpgPlayer>();
                    if (rpgPlayer.Level >= 10)
                        return true;
                }
            }
            return false;
        }

        public override List<string> SetNPCNameList()
        {
            return new List<string>
            {
                "Magnus",
                "Aldric",
                "Cedric",
                "Theron",
                "Varian",
                "Godfrey",
                "Elric",
                "Baelor"
            };
        }

        public override string GetChat()
        {
            var player = Main.LocalPlayer;
            var rpgPlayer = player.GetModPlayer<Players.RpgPlayer>();
            
            List<string> dialogue = new List<string>
            {
                "Greetings, adventurer! I have items to aid your journey.",
                "Looking to advance your abilities? You've come to the right place!",
                "Every hero needs the right tools. Take a look at my wares.",
                $"Ah, a Level {rpgPlayer.Level} {rpgPlayer.CurrentJob}! Impressive progress!"
            };
            
            if (rpgPlayer.Level >= 50)
            {
                dialogue.Add("Your power is formidable! Perhaps you're ready for the next tier?");
            }
            
            if (rpgPlayer.Level >= 100)
            {
                dialogue.Add("A true master walks among us! Your legend will be remembered.");
            }
            
            return dialogue[Main.rand.Next(dialogue.Count)];
        }

        public override void SetChatButtons(ref string button, ref string button2)
        {
            button = Language.GetTextValue("LegacyInterface.28"); // "Shop"
            button2 = "Job Info";
        }

        public override void OnChatButtonClicked(bool firstButton, ref string shop)
        {
            if (firstButton)
            {
                shop = "RpgShop";
            }
            else
            {
                // Show job info
                var player = Main.LocalPlayer;
                var rpgPlayer = player.GetModPlayer<Players.RpgPlayer>();
                
                Main.npcChatText = GetJobInfoText(rpgPlayer);
            }
        }

        public override void AddShops()
        {
            var shop = new NPCShop(Type, "RpgShop");
            
            // Job Advancement Scrolls
            shop.Add<FirstJobScroll>(Condition.PlayerCarriesItem(ItemID.GoldCoin));
            shop.Add<SecondJobScroll>(Condition.Hardmode);
            shop.Add<ThirdJobScroll>(Condition.DownedPlantera);
            shop.Add<JobManual>();
            
            // Reset Items
            shop.Add<StatResetOrb>();
            shop.Add<SkillResetScroll>(Condition.DownedEyeOfCthulhu);
            shop.Add<CompleteResetCrystal>(Condition.DownedMoonLord);
            
            shop.Register();
        }

        private string GetJobInfoText(Players.RpgPlayer rpgPlayer)
        {
            string info = $"Current Job: {rpgPlayer.CurrentJob}\n";
            info += $"Job Tier: {rpgPlayer.CurrentTier}\n";
            info += $"Level: {rpgPlayer.Level}\n\n";
            
            var nextTier = rpgPlayer.CurrentTier + 1;
            if ((int)nextTier <= 3)
            {
                int requiredLevel = nextTier switch
                {
                    Common.JobTier.Tier1 => 10,
                    Common.JobTier.Tier2 => 50,
                    Common.JobTier.Tier3 => 100,
                    _ => 999
                };
                
                if (rpgPlayer.Level >= requiredLevel)
                {
                    info += $"You are eligible for advancement!\nPurchase the appropriate scroll to advance.";
                }
                else
                {
                    info += $"Next advancement at Level {requiredLevel}.\n{requiredLevel - rpgPlayer.Level} levels to go!";
                }
            }
            else
            {
                info += "You have reached the highest tier!";
            }
            
            return info;
        }

        public override void TownNPCAttackStrength(ref int damage, ref float knockback)
        {
            damage = 20;
            knockback = 4f;
        }

        public override void TownNPCAttackCooldown(ref int cooldown, ref int randExtraCooldown)
        {
            cooldown = 30;
            randExtraCooldown = 30;
        }

        public override void TownNPCAttackProj(ref int projType, ref int attackDelay)
        {
            projType = ProjectileID.MagicDagger;
            attackDelay = 1;
        }

        public override void TownNPCAttackProjSpeed(ref float multiplier, ref float gravityCorrection, ref float randomOffset)
        {
            multiplier = 12f;
            randomOffset = 2f;
        }
    }
}
