using System.Collections.Generic;
using Microsoft.Xna.Framework;
using RpgMod.Common;

namespace RpgMod.Common.Stats
{
    public readonly struct StatDefinition
    {
        public StatDefinition(string title, string abbrev, Color color, IReadOnlyList<string> effectLines)
        {
            Title = title;
            Abbrev = abbrev;
            Color = color;
            EffectLines = effectLines;
        }

        public string Title { get; }
        public string Abbrev { get; }
        public Color Color { get; }
        public IReadOnlyList<string> EffectLines { get; }
    }

    public static class StatDefinitions
    {
        private static readonly IReadOnlyDictionary<StatType, StatDefinition> Definitions =
            new Dictionary<StatType, StatDefinition>
            {
                {
                    StatType.Strength,
                    new StatDefinition(
                        "Strength - Physical Power",
                        "STR",
                        new Color(255, 100, 100),
                        new[]
                        {
                            "+1% Melee Damage per point"
                        })
                },
                {
                    StatType.Dexterity,
                    new StatDefinition(
                        "Dexterity - Precision",
                        "DEX",
                        new Color(100, 255, 100),
                        new[]
                        {
                            "+1% Ranged Damage per point",
                            "+0.3% Attack Speed per point",
                            "+0.3% Ranged Crit per point"
                        })
                },
                {
                    StatType.Rogue,
                    new StatDefinition(
                        "Rogue - Finesse",
                        "ROG",
                        new Color(180, 100, 255),
                        new[]
                        {
                            "+0.8% Melee/Ranged Damage per point",
                            "+0.3% Critical Chance per point"
                        })
                },
                {
                    StatType.Intelligence,
                    new StatDefinition(
                        "Intelligence - Arcane Power",
                        "INT",
                        new Color(100, 150, 255),
                        new[]
                        {
                            "+1.5% Magic Damage per point",
                            "+0.7% Magic Critical per point",
                            "+0.5% Spell Power per point",
                            "+0.2% Mana Cost Reduction per point"
                        })
                },
                {
                    StatType.Focus,
                    new StatDefinition(
                        "Focus - Summoning",
                        "FOC",
                        new Color(150, 200, 255),
                        new[]
                        {
                            "+1.2% Summon Damage per point",
                            "+1 Minion Slot at 10/30/60/100/150+ FOC"
                        })
                },
                {
                    StatType.Vitality,
                    new StatDefinition(
                        "Vitality - Life Force",
                        "VIT",
                        new Color(255, 80, 80),
                        new[]
                        {
                            "+10 Max HP per point",
                            "+0.02 HP Regen per point"
                        })
                },
                {
                    StatType.Stamina,
                    new StatDefinition(
                        "Stamina - Endurance",
                        "STA",
                        new Color(255, 200, 80),
                        new[]
                        {
                            "+2 Max Stamina per point",
                            "+0.05 Stamina Regen per point"
                        })
                },
                {
                    StatType.Defense,
                    new StatDefinition(
                        "Defense - Protection",
                        "DEF",
                        new Color(200, 200, 200),
                        new[]
                        {
                            "+0.3% Damage Reduction per point",
                            "+1 Armor per 5 points"
                        })
                },
                {
                    StatType.Agility,
                    new StatDefinition(
                        "Agility - Mobility",
                        "AGI",
                        new Color(100, 255, 200),
                        new[]
                        {
                            "+0.3% Move Speed per point",
                            "+0.2% Dodge Chance per point"
                        })
                },
                {
                    StatType.Wisdom,
                    new StatDefinition(
                        "Wisdom - Mystic Knowledge",
                        "WIS",
                        new Color(200, 150, 255),
                        new[]
                        {
                            "+5 Max Mana per point",
                            "+0.03 Mana Regen per point"
                        })
                },
                {
                    StatType.Fortitude,
                    new StatDefinition(
                        "Fortitude - Resilience",
                        "FOR",
                        new Color(160, 120, 80),
                        new[]
                        {
                            "+0.5% Debuff Duration Reduction per point",
                            "+0.3% Knockback Resist per point",
                            "+0.2% Damage Reduction per point"
                        })
                },
                {
                    StatType.Luck,
                    new StatDefinition(
                        "Luck - Fortune",
                        "LUK",
                        new Color(255, 215, 0),
                        new[]
                        {
                            "+0.5% Critical Chance per point",
                            "+0.2% Luck (drops/variance) per point",
                            "+0.2% All Damage per point"
                        })
                }
            };

        public static StatDefinition GetDefinition(StatType stat)
        {
            if (Definitions.TryGetValue(stat, out var def))
                return def;

            return new StatDefinition(stat.ToString(), "???", Color.White, new[] { "No data" });
        }
    }
}
