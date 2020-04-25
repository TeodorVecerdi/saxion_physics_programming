using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using DelaunayVoronoi;
using GXPEngine;
using physics_programming.final_assignment;
using Point = DelaunayVoronoi.Point;

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