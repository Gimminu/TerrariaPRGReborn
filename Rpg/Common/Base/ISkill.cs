using Terraria;
using Terraria.ModLoader;

namespace Rpg.Common.Base
{
    /// <summary>
    /// Interface for all skills - ensures consistent skill implementation
    /// </summary>
    public interface ISkill
    {
        /// <summary>Unique internal name for the skill</summary>
        string InternalName { get; }

        /// <summary>Display name shown to player</summary>
        string DisplayName { get; }

        /// <summary>Skill description</summary>
        string Description { get; }

        /// <summary>Skill type (Active/Passive/etc)</summary>
        SkillType SkillType { get; }

        /// <summary>Required job to learn this skill</summary>
        JobType RequiredJob { get; }

        /// <summary>Required player level</summary>
        int RequiredLevel { get; }

        /// <summary>Skill points needed to unlock</summary>
        int SkillPointCost { get; }

        /// <summary>Maximum skill level/rank</summary>
        int MaxRank { get; }

        /// <summary>Current skill rank</summary>
        int CurrentRank { get; set; }

        /// <summary>Icon texture path</summary>
        string IconTexture { get; }

        /// <summary>Can this skill be learned by the player?</summary>
        bool CanLearn(Player player);

        /// <summary>Can this skill be used right now?</summary>
        bool CanUse(Player player);

        /// <summary>Skill cooldown in seconds (0 for passive)</summary>
        float CooldownSeconds { get; }

        /// <summary>Current cooldown remaining</summary>
        float CurrentCooldown { get; set; }

        /// <summary>Resource cost (mana/stamina/etc)</summary>
        int ResourceCost { get; }

        /// <summary>Resource type (mana/life/etc)</summary>
        ResourceType ResourceType { get; }
    }

    /// <summary>
    /// Resource types for skill costs
    /// </summary>
    public enum ResourceType
    {
        None,
        Mana,
        Life,
        Stamina,
        Rage,
        Energy
    }
}
