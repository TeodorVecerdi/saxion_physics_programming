using System;
using System.Collections.Generic;
using GXPEngine;
using physics_programming.final_assignment.Utils;

namespace physics_programming.final_assignment {
    public class Player : Sprite {
        public float Radius = 30;
        public Vec2 Position;
        public Vec2 Velocity;

        private readonly Barrel barrel;
        private readonly float maxVelocity;
        private Vec2 acceleration;
        private Vec2 oldPosition;
        private List<CircleCollider> colliders;

        public Player(float px, float py, float maxVelocity) : base("data/assets/bodies/t34.png") {
            Position.x = px;
            Position.y = py;
            this.maxVelocity = maxVelocity;
            SetOrigin(texture.width / 2f, texture.height / 2f);
            barrel = new Barrel(this);
            barrel.SetOrigin(34, 28);
            AddChild(barrel);
            
            colliders = new List<CircleCollider> {
                new CircleCollider(20, new Vec2(-texture.width / 2f + 30, -texture.height / 2f + 30)),
                new CircleCollider(20, new Vec2(-texture.width / 2f + 30,  texture.height / 2f - 30)),
                new CircleCollider(20, new Vec2( texture.width / 2f - 28, -texture.height / 2f + 30)),
                new CircleCollider(20, new Vec2( texture.width / 2f - 28,  texture.height / 2f - 30)),
                new CircleCollider(20, new Vec2( 0, -texture.height / 2f + 30)),
                new CircleCollider(20, new Vec2( 0,  texture.height / 2f - 30)),
                new CircleCollider(20, new Vec2( texture.width / 3f - 28, -texture.height / 2f + 30)),
                new CircleCollider(20, new Vec2( texture.width / 3f - 28,  texture.height / 2f - 30))
            };
            colliders.ForEach(AddChild);
        }

        private void Controls() {
            var rotationAmount = 0f;
            if (Input.GetKey(Key.D))
                rotationAmount = 1f;

            if (Input.GetKey(Key.A))
                rotationAmount = -1f;

            rotationAmount *= MathUtils.Map(Velocity.sqrMagnitude, 0, maxVelocity * maxVelocity, 0, 1);
            rotation += rotationAmount;
            acceleration = Vec2.Right * 0.5f;
            acceleration.SetAngleDegrees(rotation);

            var dir = 0f;
            if (Input.GetKey(Key.W)) dir += 1;
            if (Input.GetKey(Key.S)) dir -= 1;
            acceleration *= dir;
        }

        private void Shoot() {
            if (Input.GetMouseButtonDown(0)) {
                var g = (MyGame) game;
                var bullet = new Bullet(Position, Vec2.GetUnitVectorDeg(barrel.rotation + rotation) * 5f) {rotation = barrel.rotation + rotation};
                Console.WriteLine(barrel.rotation + rotation);
                g.AddBullet(bullet);
            }
        }

        private void UpdateScreenPosition() {
            x = Position.x;
            y = Position.y;
        }

        public void Update() {
            Controls();

            // Basic Euler integration:
            if (acceleration != Vec2.Zero) {
                Velocity += acceleration;
                if (Velocity.sqrMagnitude >= maxVelocity * maxVelocity)
                    Velocity = Velocity.normalized * maxVelocity;
            } else {
                Velocity.x = Mathf.Lerp(Velocity.x, 0f, 0.05f);
                Velocity.y = Mathf.Lerp(Velocity.y, 0f, 0.05f);
            }

            oldPosition = Position;
            Position += Velocity;
            Shoot();
            UpdateScreenPosition();
            
            colliders.ForEach(collider => {
                collider.Step();
            });
        }

        private CollisionInfo FindEarliestLineCollision() {
            var myGame = (MyGame) game;
            var collisionInfo = new CollisionInfo(Vec2.Zero, null, Mathf.Infinity);

            for (var i = 0; i < myGame.GetNumberOfLines(); i++) {
                var line = myGame.GetLine(i);
                var currentCollisionInfo = CollisionUtils.CircleLineCollision(Position, oldPosition, Velocity, Radius, line);
                if (currentCollisionInfo != null && currentCollisionInfo.TimeOfImpact < collisionInfo.TimeOfImpact)
                    collisionInfo = new CollisionInfo(currentCollisionInfo.Normal, null, currentCollisionInfo.TimeOfImpact);
            }

            return float.IsPositiveInfinity(collisionInfo.TimeOfImpact) ? null : collisionInfo;
        }
    }
}