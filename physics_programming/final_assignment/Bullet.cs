using System;
using GXPEngine;
using physics_programming.final_assignment.Utils;

namespace physics_programming.final_assignment {
    public class Bullet : Sprite {
        private Vec2 oldPosition;
        private int bouncesLeft;

        public Vec2 Position;
        public Vec2 Velocity;
        public const float Bounciness = 0.95f;
        
        public float Radius;
        public float Mass => Radius * Radius;
        public bool Dead;

        public Bullet(Vec2 position, Vec2 velocity, int maxBounces = 20, int radius = 2) : base("data/assets/bullet.png") {
            Position = position;
            Velocity = velocity;
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
            oldPosition = Position;
            Position += Velocity;

            var lineCollision = FindEarliestLineCollision();
            if (lineCollision != null) {
                
                if (bouncesLeft <= 0) Dead = true;
                else {
                    bouncesLeft--;
                    ResolveCollision(lineCollision);
                }
            }

            UpdateScreenPosition();
        }

        private CollisionInfo FindEarliestLineCollision() {
            var myGame = (MyGame) game;
            var collisionInfo = new CollisionInfo(Vec2.Zero, null, Mathf.Infinity);

            for (var i = 0; i < myGame.GetNumberOfLines(); i++) {
                var line = myGame.GetLine(i);
                var currentCollisionInfo = CollisionUtils.CircleLineCollision(Position, oldPosition, Velocity, Radius, line);
                if (currentCollisionInfo != null && currentCollisionInfo.TimeOfImpact < collisionInfo.TimeOfImpact) {
                    collisionInfo = new CollisionInfo(currentCollisionInfo.Normal, null, currentCollisionInfo.TimeOfImpact);
                }
            }
            return float.IsPositiveInfinity(collisionInfo.TimeOfImpact) ? null : collisionInfo;
        }

        private void ResolveCollision(CollisionInfo col) {
            if (col.Other == null) {
                // Line collision
                Position = oldPosition + Velocity * col.TimeOfImpact;
                Velocity.Reflect(col.Normal, Bounciness);
                rotation = Velocity.GetAngleDegrees();
            } else {
                // TODO: Implement Bullet-Enemy Collision later
            }
        }
    }
}