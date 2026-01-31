using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.ID;

namespace RpgMod.Common.Systems
{
    /// <summary>
    /// Equipment Set Bonus System - Provides RPG stat bonuses for wearing equipment sets
    /// </summary>
    public class EquipmentSetBonusSystem : ModPlayer
    {
        // Active set bonuses
        public string ActiveSetBonus { get; private set; } = "";
        public float BonusStrength { get; private set; }
        public float BonusVitality { get; private set; }
        public float BonusIntelligence { get; private set; }
        public float BonusDexterity { get; private set; }
        public float BonusLuck { get; private set; }
        public float BonusXPMultiplier { get; private set; }
        public float BonusCritChance { get; private set; }
        public float BonusCritDamage { get; private set; }
        public float BonusDamageReduction { get; private set; }
        
        // Set definitions
        public static readonly Dictionary<string, SetBonusData> SetBonuses = new()
        {
            // Pre-Hardmode Sets
            ["copper"] = new SetBonusData("Copper Set", 2, 1, 0, 0, 0, 0, 0, 0, 0),
            ["iron"] = new SetBonusData("Iron Set", 3, 2, 0, 0, 0, 0, 0, 0, 0),
            ["silver"] = new SetBonusData("Silver Set", 3, 2, 1, 0, 0, 0, 0.01f, 0, 0),
            ["gold"] = new SetBonusData("Gold Set", 4, 2, 1, 1, 1, 0.02f, 0.02f, 0, 0),
            ["platinum"] = new SetBonusData("Platinum Set", 5, 3, 1, 1, 1, 0.02f, 0.02f, 0.01f, 0),
            ["shadow"] = new SetBonusData("Shadow Scale Set", 6, 3, 0, 3, 0, 0, 0.03f, 0.05f, 0.02f),
            ["crimson"] = new SetBonusData("Crimtane Set", 5, 5, 0, 2, 0, 0, 0.02f, 0.03f, 0.03f),
            ["jungle"] = new SetBonusData("Jungle Set", 2, 2, 5, 3, 2, 0.03f, 0.02f, 0, 0),
            ["meteor"] = new SetBonusData("Meteor Set", 3, 2, 6, 0, 0, 0.03f, 0.03f, 0.05f, 0),
            ["necro"] = new SetBonusData("Necro Set", 4, 2, 0, 6, 0, 0, 0.05f, 0.05f, 0),
            ["bee"] = new SetBonusData("Bee Set", 3, 3, 3, 3, 3, 0.03f, 0.03f, 0.03f, 0.02f),
            ["obsidian"] = new SetBonusData("Obsidian Set", 4, 4, 0, 4, 0, 0, 0.02f, 0, 0.03f),
            ["molten"] = new SetBonusData("Molten Set", 8, 5, 0, 2, 0, 0.02f, 0.03f, 0.03f, 0.03f),
            
            // Hardmode Sets
            ["cobalt"] = new SetBonusData("Cobalt Set", 6, 4, 4, 4, 0, 0.03f, 0.03f, 0.02f, 0.02f),
            ["palladium"] = new SetBonusData("Palladium Set", 5, 6, 3, 4, 0, 0.03f, 0.02f, 0.02f, 0.04f),
            ["mythril"] = new SetBonusData("Mythril Set", 7, 5, 5, 5, 0, 0.04f, 0.04f, 0.03f, 0.02f),
            ["orichalcum"] = new SetBonusData("Orichalcum Set", 6, 5, 4, 6, 1, 0.04f, 0.05f, 0.04f, 0.03f),
            ["adamantite"] = new SetBonusData("Adamantite Set", 10, 7, 5, 5, 0, 0.04f, 0.04f, 0.04f, 0.03f),
            ["titanium"] = new SetBonusData("Titanium Set", 8, 8, 5, 6, 2, 0.05f, 0.05f, 0.04f, 0.05f),
            ["frost"] = new SetBonusData("Frost Set", 8, 6, 4, 8, 0, 0.04f, 0.06f, 0.05f, 0.02f),
            ["forbidden"] = new SetBonusData("Forbidden Set", 5, 5, 10, 5, 3, 0.05f, 0.04f, 0.04f, 0.02f),
            ["hallowed"] = new SetBonusData("Hallowed Set", 10, 8, 8, 8, 5, 0.06f, 0.06f, 0.05f, 0.04f),
            ["chlorophyte"] = new SetBonusData("Chlorophyte Set", 12, 10, 8, 8, 3, 0.06f, 0.05f, 0.05f, 0.05f),
            ["turtle"] = new SetBonusData("Turtle Set", 15, 15, 0, 5, 0, 0.04f, 0.03f, 0.02f, 0.08f),
            ["beetle"] = new SetBonusData("Beetle Set", 18, 18, 0, 5, 0, 0.05f, 0.04f, 0.03f, 0.10f),
            ["shroomite"] = new SetBonusData("Shroomite Set", 8, 6, 0, 15, 5, 0.08f, 0.08f, 0.10f, 0.02f),
            ["spectre"] = new SetBonusData("Spectre Set", 5, 8, 18, 5, 3, 0.07f, 0.05f, 0.08f, 0.03f),
            ["spooky"] = new SetBonusData("Spooky Set", 6, 6, 15, 6, 10, 0.08f, 0.06f, 0.06f, 0.02f),
            ["tiki"] = new SetBonusData("Tiki Set", 5, 5, 12, 5, 8, 0.07f, 0.05f, 0.05f, 0.02f),
            
            // Endgame Sets
            ["solar"] = new SetBonusData("Solar Flare Set", 25, 20, 5, 10, 5, 0.10f, 0.08f, 0.10f, 0.15f),
            ["vortex"] = new SetBonusData("Vortex Set", 12, 10, 5, 25, 10, 0.12f, 0.12f, 0.15f, 0.05f),
            ["nebula"] = new SetBonusData("Nebula Set", 8, 12, 25, 8, 10, 0.12f, 0.08f, 0.12f, 0.05f),
            ["stardust"] = new SetBonusData("Stardust Set", 10, 15, 20, 10, 15, 0.10f, 0.06f, 0.08f, 0.08f),
        };
        
        public override void ResetEffects()
        {
            ActiveSetBonus = "";
            BonusStrength = 0;
            BonusVitality = 0;
            BonusIntelligence = 0;
            BonusDexterity = 0;
            BonusLuck = 0;
            BonusXPMultiplier = 0;
            BonusCritChance = 0;
            BonusCritDamage = 0;
            BonusDamageReduction = 0;
        }
        
        public override void PostUpdate()
        {
            DetectAndApplySetBonus();
        }
        
        private void DetectAndApplySetBonus()
        {
            string setBonus = Player.setBonus?.ToLower() ?? "";
            
            foreach (var (setKey, data) in SetBonuses)
            {
                if (setBonus.Contains(setKey) || IsWearingSet(setKey))
                {
                    ApplySetBonus(setKey, data);
                    return;
                }
            }
        }
        
        private bool IsWearingSet(string setKey)
        {
            // Check if player is wearing a specific armor set based on item names
            var head = Player.armor[0];
            var body = Player.armor[1];
            var legs = Player.armor[2];
            
            if (head == null || body == null || legs == null)
                return false;
            
            string headName = head.Name?.ToLower() ?? "";
            string bodyName = body.Name?.ToLower() ?? "";
            string legsName = legs.Name?.ToLower() ?? "";
            
            // Check if all armor pieces contain the set key
            return headName.Contains(setKey) && bodyName.Contains(setKey) && legsName.Contains(setKey);
        }
        
        private void ApplySetBonus(string setKey, SetBonusData data)
        {
            ActiveSetBonus = data.Name;
            BonusStrength = data.Strength;
            BonusVitality = data.Vitality;
            BonusIntelligence = data.Intelligence;
            BonusDexterity = data.Dexterity;
            BonusLuck = data.Luck;
            BonusXPMultiplier = data.XPMultiplier;
            BonusCritChance = data.CritChance;
            BonusCritDamage = data.CritDamage;
            BonusDamageReduction = data.DamageReduction;
        }
        
        /// <summary>
        /// Get the stat bonuses as a dictionary for display
        /// </summary>
        public Dictionary<string, float> GetBonusStats()
        {
            return new Dictionary<string, float>
            {
                ["STR"] = BonusStrength,
                ["VIT"] = BonusVitality,
                ["INT"] = BonusIntelligence,
                ["DEX"] = BonusDexterity,
                ["LUK"] = BonusLuck,
                ["XP%"] = BonusXPMultiplier * 100f,
                ["Crit%"] = BonusCritChance * 100f,
                ["CritDmg%"] = BonusCritDamage * 100f,
                ["DR%"] = BonusDamageReduction * 100f
            };
        }
    }
    
    public class SetBonusData
    {
        public string Name { get; }
        public float Strength { get; }
        public float Vitality { get; }
        public float Intelligence { get; }
        public float Dexterity { get; }
        public float Luck { get; }
        public float XPMultiplier { get; }
        public float CritChance { get; }
        public float CritDamage { get; }
        public float DamageReduction { get; }
        
        public SetBonusData(string name, float str, float vit, float intel, float dex, float luck, 
                           float xpMult, float crit, float critDmg, float dr)
        {
            Name = name;
            Strength = str;
            Vitality = vit;
            Intelligence = intel;
            Dexterity = dex;
            Luck = luck;
            XPMultiplier = xpMult;
            CritChance = crit;
            CritDamage = critDmg;
            DamageReduction = dr;
        }
    }
}
