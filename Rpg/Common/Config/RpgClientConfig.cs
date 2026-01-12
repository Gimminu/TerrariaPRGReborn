using System.ComponentModel;
using Terraria.ModLoader.Config;

namespace Rpg.Common.Config
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
        
        [Header("WorldLevel")]
        [DefaultValue(true)]
        public bool ShowWorldLevelUI { get; set; } = true;
        
        [Header("MonsterInfo")]
        [DefaultValue(true)]
        public bool ShowMonsterLevel { get; set; } = true;
        
        [DefaultValue(true)]
        public bool ColorCodeMonsterLevel { get; set; } = true;
    }
}
