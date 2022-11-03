using Avalonia.Controls;
using Avalonia.Platform;
using System.Numerics;

namespace UnmistakableAPKInstaller.AvaloniaUI.Windows
{
    public static class AvaloniaWindowUtils
    {
        public static void SetDefaultWindowSize(this Window window, Screen screen, float targetRatio, float targetSizeFactor)
        {
            var screenBounds = screen.Bounds.Size;
            var aspectRatio = screenBounds.AspectRatio;

            var ratioFactor = targetRatio / aspectRatio;

            var windowSize = new Vector2(screenBounds.Width, screenBounds.Height);

            var screenInPortraitMode = aspectRatio < 1;
            var windowInPortraitMode = targetRatio < 1;

            // If current screen is in portrait mode
            if (screenInPortraitMode)
            {
                // Inverse radio factor
                ratioFactor = 1 / ratioFactor;

                if (!windowInPortraitMode)
                {
                    // Reduce micro screen issues in portrait mode
                    targetSizeFactor = 1;
                }
            }

            windowSize *= (float)ratioFactor;
            windowSize *= targetSizeFactor;

            // Inverse window resolution
            // If target window or current screen is in portrait mode
            if (windowInPortraitMode || screenInPortraitMode)
            {
                windowSize.Y = windowSize.X;
                windowSize.X *= targetRatio;
            }

            window.Width = windowSize.X;
            window.Height = windowSize.Y;
        }
    }
}
