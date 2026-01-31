using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI;
using RpgMod.Common.Config;

namespace RpgMod.Common.Systems
{
    /// <summary>
    /// System to hide default HP/MP bars while keeping buff icons visible
    /// </summary>
    public class UIHiderSystem : ModSystem
    {
        public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
        {
            var clientConfig = ModContent.GetInstance<RpgClientConfig>();
            if (clientConfig == null || !clientConfig.HideVanillaResourceBars)
                return;

            // Keep resource bars when breath/lava meters should be visible.
            if (Main.LocalPlayer != null && Main.LocalPlayer.active)
            {
                if (Main.LocalPlayer.breath < Main.LocalPlayer.breathMax ||
                    Main.LocalPlayer.lavaTime < Main.LocalPlayer.lavaMax)
                {
                    return;
                }
            }

            // If the buffs layer is missing (tML layer changes), don't remove bars to avoid hiding buff icons.
            bool hasBuffLayer = layers.Exists(layer => layer.Name.Equals("Vanilla: Buffs"));
            if (!hasBuffLayer)
                return;

            // Find and remove the resource bars layer (HP/MP)
            int resourceBarsIndex = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Resource Bars"));
            if (resourceBarsIndex != -1)
            {
                layers.RemoveAt(resourceBarsIndex);
            }

            // Keep buff icons by not removing "Vanilla: Buffs" layer
            // The buffs layer will remain visible
        }
    }
}
