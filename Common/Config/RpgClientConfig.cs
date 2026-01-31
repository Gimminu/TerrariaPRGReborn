using System.ComponentModel;
using Terraria.ModLoader.Config;

namespace RpgMod.Common.Config
{
    /// <summary>
    /// Client-side visual toggles.
    /// </summary>
    public class RpgClientConfig : ModConfig
    {
        public override ConfigScope Mode => ConfigScope.ClientSide;

        [Header("UI")]
        [DefaultValue(true)]
        public bool HideVanillaResourceBars { get; set; } = true;

        [Header("Experience")]
        [DefaultValue(true)]
        public bool ShowXPGainText { get; set; } = true;

        [DefaultValue(true)]
        public bool ShowCapInfo { get; set; } = true;

        [Header("QuestUI")]
        [DefaultValue(true)]
        public bool ShowQuestUI { get; set; } = true;

        [DefaultValue(true)]
        public bool HideQuestUIWhenEmpty { get; set; } = true;

        [Range(0.5f, 1.5f)]
        [DefaultValue(1f)]
        public float QuestUIScale { get; set; } = 1f;

        [Range(0f, 1f)]
        [DefaultValue(0.85f)]
        public float QuestUIPosX { get; set; } = 0.85f;

        [Range(0f, 1f)]
        [DefaultValue(0.5f)]
        public float QuestUIPosY { get; set; } = 0.5f;

        [Header("LayoutMode")]
        [DefaultValue(false)]
        public bool EnableUILayoutMode { get; set; } = false;

        [Header("RpgMenu")]
        [Range(0f, 1f)]
        [DefaultValue(0.5f)]
        public float RpgMenuPosX { get; set; } = 0.5f;

        [Range(0f, 1f)]
        [DefaultValue(0.5f)]
        public float RpgMenuPosY { get; set; } = 0.5f;
        
        [Header("WorldLevel")]
        [DefaultValue(true)]
        public bool ShowWorldLevelUI { get; set; } = true;
        
        [Header("MonsterInfo")]
        [DefaultValue(true)]
        public bool ShowMonsterLevel { get; set; } = true;
        
        [DefaultValue(true)]
        public bool ColorCodeMonsterLevel { get; set; } = true;
        
        [Header("SkillBar")]
        [DefaultValue(true)]
        public bool ShowSkillBar { get; set; } = true;
        
        [Header("UIVisibility")]
        [DefaultValue(true)]
        public bool ShowJobUI { get; set; } = true;
        
        [DefaultValue(true)]
        public bool ShowSkillUI { get; set; } = true;
    }
}
