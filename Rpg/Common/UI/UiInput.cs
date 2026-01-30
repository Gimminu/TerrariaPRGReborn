using Microsoft.Xna.Framework;
using Terraria;

namespace Rpg.Common.UI
{
    internal static class UiInput
    {
        public static float UiScale
        {
            get
            {
                float scale = Main.UIScale;
                return scale <= 0f ? 1f : scale;
            }
        }

        public static Vector2 ScreenTopLeftUi => Vector2.Zero;

        public static Vector2 ScreenBottomRightUi => new Vector2(
            Main.screenWidth,
            Main.screenHeight
        );

        public static int ScreenWidthUi => (int)ScreenBottomRightUi.X;

        public static int ScreenHeightUi => (int)ScreenBottomRightUi.Y;

        public static Vector2 GetUiMouse()
        {
            return Main.MouseScreen;
        }

        public static Point GetUiMousePoint()
        {
            return Main.MouseScreen.ToPoint();
        }
    }
}
