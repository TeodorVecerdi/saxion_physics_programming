using System.Collections.Generic;
using System.Linq;
using GXPEngine;
using physics_programming.final_assignment.Utils;

namespace physics_programming.final_assignment {
    public class Bullet : Sprite {
        public const float Bounciness = 0.95f;
        public const float Speed = 500f;

        public readonly float Radius;
        public bool Dead;
        public Vec2 OldPosition;

        public Vec2 Position;
        public Vec2 Velocity;
        private int bouncesLeft;
        private readonly Tank parentTank;

        public Bullet(Vec2 position, Vec2 velocity, Tank parentTank, int maxBounces = 0, int radius = 2) : base("data/assets/bullet.png") {
            Position = position;
            Velocity = velocity * Speed;
            this.parentTank = parentTank;
            bouncesLeft = maxBounces;
            Radius = radius;

            UpdateScreenPosition();
            SetOrigin(Radius, Radius);
        }

        private void UpdateScreenPosition() {
            x = Position.x;
            y = Position.y;
        }

        public void Step() {
            var g = (MyGame) game;
            OldPosition = Position;
            Position += Velocity * Time.deltaTime;

            var lineCollision = FindEarliestLineCollision();
            if (lineCollision != null) {
                if (bouncesLeft <= 0) Dead = true;
                else {
                    bouncesLeft--;
                    ResolveCollision(lineCollision);
                }
            }

            // Destructible collisions
            var linesToAdd = new List<DoubleDestructibleLineSegment>();
            foreach (var destructibleLine in g.DestructibleLines) {
                var (line1, line2) = CollisionUtils.BulletLineCollision(this, destructibleLine);
                if (line1 == null && line2 == null) continue;
                destructibleLine.ShouldRemove = true;
                if (line1 != null) linesToAdd.Add(line1);
                if (line2 != null) linesToAdd.Add(line2);

                if (bouncesLeft <= 0) Dead = true;
                else bouncesLeft--;
            }

            linesToAdd.ForEach(line => {
                g.DestructibleLines.Add(line);
                g.AddChild(line);
            });

            // Dead = g.Enemies.SelectMany(enemy => enemy.Tank.Colliders).Any(circleCollider => CollisionUtils.CircleCircleCollision(Position, OldPosition, Velocity, Radius, circleCollider.Position, circleCollider.Radius) != null);
            var availableTanks = new List<Tank>();
            availableTanks.AddRange(g.Enemies.Select(enemy => enemy.Tank));
            availableTanks.Add(g.Player.Tank);
            availableTanks.Remove(parentTank);
            var validColliders = availableTanks.SelectMany(tank => tank.Colliders);

            foreach (var circleCollider in validColliders) {
                var colliderParent = circleCollider.parent as Tank;
                var (worldPosition, worldOldPosition) = CircleCollider.LocalToWorldCoords(circleCollider.Position, circleCollider.OldPosition, colliderParent.Position, colliderParent.OldPosition);
                var (rotatedPosition, rotatedOldPosition) = CircleCollider.ApplyRotation(worldPosition, colliderParent.Position, worldOldPosition, colliderParent.OldPosition, colliderParent.rotation);

                var collisionInfo = CollisionUtils.CircleCircleCollision(Position, OldPosition, Velocity * Time.deltaTime, Radius, rotatedPosition, circleCollider.Radius);
                if (collisionInfo == null) continue;

                Dead = true;
                break;
            }

            UpdateScreenPosition();
        }

        private CollisionInfo FindEarliestLineCollision() {
            var myGame = (MyGame) game;
            var collisionInfo = new CollisionInfo(Vec2.Zero, null, Mathf.Infinity);

            for (var i = 0; i < myGame.GetNumberOfLines(); i++) {
                var line = myGame.GetLine(i);
                var currentCollisionInfo = CollisionUtils.CircleLineCollision(Position, OldPosition, Velocity * Time.deltaTime, Radius, line);
                if (currentCollisionInfo != null && currentCollisionInfo.TimeOfImpact < collisionInfo.TimeOfImpact)
                    collisionInfo = new CollisionInfo(currentCollisionInfo.Normal, null, currentCollisionInfo.TimeOfImpact);
            }

            return float.IsPositiveInfinity(collisionInfo.TimeOfImpact) ? null : collisionInfo;
        }

        private void ResolveCollision(CollisionInfo collisionInfo) {
            if (collisionInfo.Other == null) {
                // Line collision
                Position = OldPosition + Velocity * Time.deltaTime * collisionInfo.TimeOfImpact;
                Velocity.Reflect(collisionInfo.Normal, Bounciness);
                rotation = Velocity.GetAngleDegrees();
            }
        }
    }
}