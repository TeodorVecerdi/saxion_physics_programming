using GXPEngine;

namespace physics_programming {
    public class Program : Game{
        public static void Main(string[] args) {
            new Program().Start();
        }

        public Program() : base(Globals.WIDTH, Globals.HEIGHT, Globals.FULLSCREEN, Globals.VSYNC, pPixelArt: Globals.PIXEL_ART, windowTitle: "Physics Programming") {
            ShowMouse(true);
        }
    }
}