using System;
using System.Drawing;
using GXPEngine;

namespace physics_programming.testing {
    public class MainAppTesting : Game {
        private AttackIndicator indicator;
        private const float timeToShoot = 3f;
        private float timeLeftToShoot;
        
        public MainAppTesting() : base(Globals.WIDTH, Globals.HEIGHT, Globals.FULLSCREEN, Globals.VSYNC, pPixelArt: Globals.PIXEL_ART, windowTitle: Globals.WINDOW_TITLE) {
            targetFps = 60;
            timeLeftToShoot = timeToShoot;
            indicator = new AttackIndicator(timeToShoot, color: Color.Aqua, radius: 100, initialArcAngle: 180);
            indicator.SetXY(200,200);
            AddChild(indicator);
        }

        private void Update() {
            timeLeftToShoot -= Time.deltaTime;
            if (timeLeftToShoot < 0) 
                timeLeftToShoot = timeToShoot;
            indicator.UpdateIndicator(timeLeftToShoot, 0);
        }

        public static void MainT() {
            new MainAppTesting().Start();
        }
    }
}