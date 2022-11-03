using Avalonia.Controls;
using Avalonia.Platform;
using Serilog;
using System;
using System.Numerics;

namespace UnmistakableAPKInstaller.AvaloniaUI.Utils
{
    public static class AvaloniaWindowUtils
    {
        // TODO: check && update this method to cross-platform

        /// <summary>
        /// This method only works for Windows OS
        /// Possible bugowner: <see cref="Screen"/>
        /// </summary>
        /// <param name="window"></param>
        /// <param name="screen"></param>
        /// <param name="targetRatio"></param>
        /// <param name="targetSizeFactor"></param>
        public static void SetDefaultWindowSize(this Window window, Screen screen, Vector2 defaultSize, float targetSizeFactor)
        {
            try
            {
                var targetRatio = defaultSize.X / defaultSize.Y;

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
            catch(Exception e)
            {
                Log.Error($"Set window size error: {e}");
                window.Width = defaultSize.X;
                window.Height = defaultSize.Y;
            }
        }
    }
}
