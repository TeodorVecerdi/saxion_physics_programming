using System;
using GXPEngine;

namespace physics_programming.assignment2 {
    public class Barrel : Sprite {

        private Tank parent;
        public Barrel(Tank parent) : base("data/assets/barrels/t34.png") {
            this.parent = parent;
        }

        public void Update() {
            var mousePos =  new Vec2(Input.mouseX, Input.mouseY);
            var desiredRotation = -parent.rotation + Angle(mousePos, parent.Position);
            float delta = desiredRotation - rotation;
            float shortestAngle = Mathf.Clamp(delta - Mathf.Floor(delta / 360f) * 360f, 0.0f, 360f);
            if (shortestAngle > 180)
                shortestAngle -= 360;
            if(Math.Abs(rotation - desiredRotation) > 0.5)
                rotation += shortestAngle * 0.1f;
            // rotation = Mathf.Lerp(desiredRotation, rotation, 0.95f);
        }
        private float Angle(Vec2 a, Vec2 b) {
            return Vec2.Rad2Deg((float)Math.Atan2(a.y - b.y, a.x - b.x));
        }
    }
}