using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using GXPEngine;
using physics_programming.final_assignment.Utils;

namespace physics_programming.final_assignment {
    public class Tank : Sprite {
        public readonly Barrel Barrel;

        public Vec2 Acceleration;
        public Vec2 Velocity;
        public Vec2 Position;
        public Vec2 OldPosition;

        private readonly List<CircleCollider> colliders;
        private readonly Action<Tank> tankMove;
        private readonly Action<Tank> tankShoot;
        private readonly Action<Tank> barrelMove;

        public Tank(float px, float py, Action<Tank> tankMove, Action<Tank> tankShoot, Action<Tank> barrelMove) : base("data/assets/bodies/t34.png") {
            Position.x = px;
            Position.y = py;
            this.tankMove = tankMove;
            this.tankShoot = tankShoot;
            this.barrelMove = barrelMove;

            SetOrigin(texture.width / 2f, texture.height / 2f);
            Barrel = new Barrel();
            Barrel.SetOrigin(34, 28);
            AddChild(Barrel);

            colliders = new List<CircleCollider> {
                new CircleCollider(40, new Vec2(-texture.width / 2f + 45, 0)),
                new CircleCollider(40, new Vec2(texture.width / 2f - 45, 0))
            };
            colliders.ForEach(collider => {
                collider.IsVisible = true;
                AddChild(collider);
            });
        }

        private void Collisions() {
            var collisionInfo = new CollisionInfo(Vec2.Zero, null, Mathf.Infinity);
            colliders.Select(collider => collider.FindEarliestLineCollision(Position, Velocity * Time.deltaTime, OldPosition, rotation))
                .Where(earliest => earliest != null && earliest.TimeOfImpact < collisionInfo.TimeOfImpact).ToList()
                .ForEach(earliest => collisionInfo = new CollisionInfo(earliest.Normal, null, earliest.TimeOfImpact));
            colliders.Select(collider => collider.FindEarliestDestructibleLineCollision(Position, Velocity * Time.deltaTime, OldPosition, rotation))
                .Where(earliest => earliest != null && earliest.TimeOfImpact < collisionInfo.TimeOfImpact).ToList()
                .ForEach(earliest => collisionInfo = new CollisionInfo(earliest.Normal, null, earliest.TimeOfImpact));
            if (!float.IsPositiveInfinity(collisionInfo.TimeOfImpact))
                Position = OldPosition + Time.deltaTime * (collisionInfo.TimeOfImpact - 0.00001f) * Velocity;
        }

        private void UpdateScreenPosition() {
            x = Position.x;
            y = Position.y;
        }

        [SuppressMessage("ReSharper", "UnusedMember.Local")]
        private void Update() {
            tankMove?.Invoke(this);
            Collisions();

            barrelMove?.Invoke(this);
            tankShoot?.Invoke(this);

            UpdateScreenPosition();
        }
    }
}