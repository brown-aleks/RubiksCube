using System;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using Window;

namespace Rubiks
{
    class Program
    {
        static void Main(string[] args)
        {
            var nativeWindowSettings = new NativeWindowSettings()
            {
                Size = new Vector2i(800, 600),
                Title = "RubiksCube",
                WindowState = WindowState.Normal,
                Flags = OpenTK.Windowing.Common.ContextFlags.Default,
                Profile = ContextProfile.Core
            };

            using (var window = new Window.Window(GameWindowSettings.Default, nativeWindowSettings))
            {
                window.Run();
            }
        }
    }
}
