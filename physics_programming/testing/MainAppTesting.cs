using GXPEngine;

namespace physics_programming.testing {
    public class MainAppTesting : Game {
        private MainAppTesting() : base(Globals.WIDTH, Globals.HEIGHT, Globals.FULLSCREEN, Globals.VSYNC, pPixelArt: Globals.PIXEL_ART, windowTitle: Globals.WINDOW_TITLE) {
            targetFps = 60;
        }
        public static void MainT() {
            new MainAppTesting().Start();
        }
    }
}