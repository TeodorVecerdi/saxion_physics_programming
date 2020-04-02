using System;
using GXPEngine;

namespace physics_programming.final_assignment {
    public class Barrel : Sprite {

        private Player parent;
        public Barrel(Player parent) : base("data/assets/barrels/t34.png") {
            this.parent = parent;
        }

        public void Update() {
            var mousePos =  new Vec2(Input.mouseX, Input.mouseY);
            var desiredRotation = -parent.rotation + Vec2.Rad2Deg((float)Math.Atan2(mousePos.y - parent.Position.y, mousePos.x - parent.Position.x));
            var delta = desiredRotation - rotation;
            var shortestAngle = Mathf.Clamp(delta - Mathf.Floor(delta / 360f) * 360f, 0.0f, 360f);
            if (shortestAngle > 180)
                shortestAngle -= 360;
            if(Math.Abs(rotation - desiredRotation) > 0.5)
                rotation += shortestAngle * 0.15f;
        }
    }
}