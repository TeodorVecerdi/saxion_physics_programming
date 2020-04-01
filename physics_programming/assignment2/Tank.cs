using System;
using GXPEngine;
using physics_programming.final_assignment.Utils;

// TODO: Fix this mess! - see Assignment 2.2
namespace physics_programming.assignment2 {
    public class Tank : Sprite {
        public Vec2 Position => position;
        public Vec2 Velocity;

        private Vec2 acceleration;
        private float maxVelocity;
        private Vec2 position;
        private Barrel barrel;

        public Tank(float px, float py, float maxVelocity) : base("data/assets/bodies/t34.png") {
            position.x = px;
            position.y = py;
            this.maxVelocity = maxVelocity;
            SetOrigin(texture.width / 2f, texture.height / 2f);
            barrel = new Barrel(this);

            // barrel.SetXY(-texture.width / 4f, -texture.height / 3.2f);
            barrel.SetOrigin(34, 28);

            // barrel.rotation = 45;
            AddChild(barrel);
        }

        private void Controls() {
            var rotationAmount = 0f;
            if (Input.GetKey(Key.LEFT)) {
                rotationAmount = 1f;
            }

            if (Input.GetKey(Key.RIGHT)) {
                rotationAmount = -1f;
            }

            rotationAmount *= MathUtils.Map(Velocity.sqrMagnitude, 0, maxVelocity * maxVelocity, 0, 1);
            rotation += rotationAmount;

            acceleration = Vec2.Right * 0.5f;
            acceleration.SetAngleDegrees(rotation);
            var dir = 0f;
            if (Input.GetKey(Key.UP)) {
                dir += 1;

                // acceleration += new Vec2(0.1f, 0);
            }

            if (Input.GetKey(Key.DOWN)) {
                dir -= 1;
            }

            acceleration *= dir;
        }

        private void Shoot() {
            if (Input.GetMouseButtonDown(0)) {
                var bullet = new Bullet(Vec2.Zero, Vec2.GetUnitVectorDeg(barrel.rotation) * 5f) {rotation = barrel.rotation};
                AddChild(bullet);
            }
        }

        private void UpdateScreenPosition() {
            x = position.x;
            y = position.y;
        }

        public void Update() {
            Console.WriteLine($"FPS: {game.currentFps}");
            Controls();

            // Basic Euler integration:
            if (acceleration != Vec2.Zero) {
                Velocity += acceleration;
                if (Velocity.sqrMagnitude >= maxVelocity * maxVelocity) {
                    Velocity = Velocity.normalized * maxVelocity;
                }
            } else {
                Velocity.x = Mathf.Lerp(Velocity.x, 0f, 0.05f);
                Velocity.y = Mathf.Lerp(Velocity.y, 0f, 0.05f);
            }

            position += Velocity;
            Shoot();
            UpdateScreenPosition();
        }
    }
}