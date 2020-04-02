using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using GXPEngine;
using physics_programming.final_assignment.Utils;

namespace physics_programming.final_assignment {
    public class Player : Sprite {
        public Vec2 Position;
        public Vec2 Velocity;
        private readonly Barrel barrel;

        private readonly float maxVelocity;
        private readonly List<CircleCollider> colliders;

        private Vec2 acceleration;
        private Vec2 oldPosition;

        public Player(float px, float py, float maxVelocity) : base("data/assets/bodies/t34.png") {
            Position.x = px;
            Position.y = py;
            this.maxVelocity = maxVelocity;
            SetOrigin(texture.width / 2f, texture.height / 2f);
            barrel = new Barrel(this);
            barrel.SetOrigin(34, 28);
            AddChild(barrel);

            colliders = new List<CircleCollider> {
                new CircleCollider(40, new Vec2(-texture.width / 2f + 45, 0)),
                new CircleCollider(40, new Vec2(texture.width / 2f - 45, 0))
            };
            colliders.ForEach(collider => {
                collider.IsVisible = true;
                AddChild(collider);
            });
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
                g.AddBullet(bullet);
            }
        }

        private void UpdateScreenPosition() {
            x = Position.x;
            y = Position.y;
        }

        [SuppressMessage("ReSharper", "UnusedMember.Global")]
        public void Update() {
            Controls();

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

            var collisionInfo = new CollisionInfo(Vec2.Zero, null, Mathf.Infinity);
            colliders.Select(collider => collider.FindEarliestLineCollision(Position, Velocity, oldPosition, rotation))
                .Where(earliest => earliest != null && earliest.TimeOfImpact < collisionInfo.TimeOfImpact).ToList()
                .ForEach(earliest => collisionInfo = new CollisionInfo(earliest.Normal, null, earliest.TimeOfImpact));
            colliders.Select(collider => collider.FindEarliestDestructibleLineCollision(Position, Velocity, oldPosition, rotation))
                .Where(earliest => earliest != null && earliest.TimeOfImpact < collisionInfo.TimeOfImpact).ToList()
                .ForEach(earliest => collisionInfo = new CollisionInfo(earliest.Normal, null, earliest.TimeOfImpact));
            if (!float.IsPositiveInfinity(collisionInfo.TimeOfImpact))
                Position = oldPosition + Velocity * (collisionInfo.TimeOfImpact - 0.00001f);
            Shoot();
            UpdateScreenPosition();
        }
    }
}