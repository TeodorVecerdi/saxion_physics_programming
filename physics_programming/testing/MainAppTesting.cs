using System.Collections.Generic;
using GXPEngine;

namespace physics_programming.testing {
    public class MainAppTesting : Game {
        private MainAppTesting() : base(Globals.WIDTH, Globals.HEIGHT, Globals.FULLSCREEN, Globals.VSYNC, pPixelArt: Globals.PIXEL_ART, windowTitle: Globals.WINDOW_TITLE) {
            targetFps = 60;

            AddChild(new DestructibleBlock(20, new Vec2(500, 500), new Vec2(800, 200)));
            // AddChild(new Block(new List<Vec2> {(500f, 500f), (507.4278f, 481.4305f), (1007.428f, 681.4305f), (1000f, 700f)}));
        }

        public static void MainT() {
            new MainAppTesting().Start();
        }
    }
}