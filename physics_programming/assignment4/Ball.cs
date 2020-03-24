using GXPEngine;
using physics_programming.assignment4.Components;

namespace physics_programming.assignment4 {
    public class Ball : EasyDraw {
        public Vec2 Position;
        public Vec2 Velocity;
        public static Vec2 acceleration = Vec2.Zero;
        public const float gravity = 0.0981f;
        private float speed;

        public int Radius { get; }

        public Ball(int pRadius, Vec2 pPosition, Vec2 velocity, float pSpeed = 5) : base(pRadius * 2 + 1, pRadius * 2 + 1) {
            Radius = pRadius;
            Position = pPosition;
            speed = pSpeed;
            Velocity = velocity;

            UpdateScreenPosition();
            SetOrigin(Radius, Radius);

            Draw(255, 255, 255);
        }

        private void Draw(byte red, byte green, byte blue) {
            Fill(red, green, blue);
            Stroke(red, green, blue);
            Ellipse(Radius, Radius, 2 * Radius, 2 * Radius);
        }

        private void FollowMouse() {
            Position.SetXY(Input.mouseX, Input.mouseY);
        }

        private void UpdateScreenPosition() {
            x = Position.x;
            y = Position.y;
        }

        public void Step() {
            // FollowMouse();
            Velocity += acceleration * gravity;
            Position += Velocity;
            UpdateScreenPosition();
        }
    }
}