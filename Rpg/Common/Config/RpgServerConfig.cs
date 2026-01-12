using System.ComponentModel;
using Terraria.ModLoader.Config;

namespace Rpg.Common.Config
{
    /// <summary>
    /// Server-side balance configuration.
    /// These settings are synced from server to clients in multiplayer.
    /// </summary>
    public class RpgServerConfig : ModConfig
    {
        public override ConfigScope Mode => ConfigScope.ServerSide;
        
        #region Experience Settings
        
        [Header("ExperienceSettings")]
        
        [DefaultValue(1.0f)]
        [Range(0.1f, 10.0f)]
        [Slider]
        public float GlobalXPMultiplier { get; set; } = 1.0f;
        
        [DefaultValue(0.5f)]
        [Range(0.0f, 1.0f)]
        [Slider]
        public float EventXPMultiplier { get; set; } = 0.5f;
        
        [DefaultValue(true)]
        public bool StatueSpawnsGiveXP { get; set; } = false;
        
        [DefaultValue(5000f)]
        [Range(500f, 50000f)]
        public float MultiplayerXPShareRange { get; set; } = 5000f;
        
        #endregion
        
        #region Scaling Settings
        
        [Header("ScalingSettings")]
        
        [DefaultValue(0.03f)]
        [Range(0.01f, 0.10f)]
        [Slider]
        public float MonsterHPScalePerLevel { get; set; } = 0.03f;
        
        [DefaultValue(0.008f)]
        [Range(0.001f, 0.02f)]
        [Slider]
        public float MonsterDamageScalePerLevel { get; set; } = 0.008f;
        
        [DefaultValue(false)]
        public bool ScaleMonsterDefense { get; set; } = false;
        
        [DefaultValue(0.001f)]
        [Range(0.0f, 0.01f)]
        [Slider]
        public float MonsterDefenseScalePerLevel { get; set; } = 0.001f;
        
        #endregion
        
        #region Level Cap Settings
        
        [Header("LevelCapSettings")]
        
        [DefaultValue(true)]
        public bool EnableBossLevelCaps { get; set; } = true;
        
        [DefaultValue(200)]
        [Range(50, 1000)]
        public int MaxLevelAfterMoonLord { get; set; } = 200;
        
        [DefaultValue(true)]
        public bool InfiniteLevelingAfterMoonLord { get; set; } = true;
        
        #endregion
        
        #region Stat Settings
        
        [Header("StatSettings")]
        
        [DefaultValue(1.0f)]
        [Range(0.5f, 2.0f)]
        [Slider]
        public float StatPointMultiplier { get; set; } = 1.0f;
        
        [DefaultValue(1.0f)]
        [Range(0.5f, 2.0f)]
        [Slider]
        public float SkillPointMultiplier { get; set; } = 1.0f;
        
        [DefaultValue(true)]
        public bool EnableAutoGrowth { get; set; } = true;
        
        [DefaultValue(0.35f)]
        [Range(0.0f, 0.5f)]
        [Slider]
        public float AutoGrowthPercent { get; set; } = 0.35f;
        
        #endregion
        
        #region Difficulty Adjustments
        
        [Header("DifficultyAdjustments")]
        
        [DefaultValue(true)]
        public bool AdjustForExpertMode { get; set; } = true;
        
        [DefaultValue(true)]
        public bool AdjustForMasterMode { get; set; } = true;
        
        [DefaultValue(0.8f)]
        [Range(0.5f, 1.0f)]
        [Slider]
        public float ExpertModeScaleReduction { get; set; } = 0.8f;
        
        [DefaultValue(0.6f)]
        [Range(0.3f, 1.0f)]
        [Slider]
        public float MasterModeScaleReduction { get; set; } = 0.6f;
        
        #endregion
        
        #region Mod Compatibility
        
        [Header("ModCompatibility")]
        
        [DefaultValue(true)]
        public bool EnableCalamityCompatibility { get; set; } = true;
        
        [DefaultValue(true)]
        public bool EnableThoriumCompatibility { get; set; } = true;
        
        [DefaultValue(true)]
        public bool AutoDetectModBosses { get; set; } = true;
        
        #endregion
    }
}
