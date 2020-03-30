using System;
using GXPEngine;
using physics_programming.assignment5.Components;

namespace physics_programming.assignment5 {
    public class Ball : EasyDraw {
        // These four public static fields are changed from MyGame, based on key input (see Console):
        public static bool DrawDebugLine = false;
        public static bool Wordy = false;
        public static float Bounciness = 0.98f;

        // For ease of testing / changing, we assume every ball has the same acceleration (gravity):
        public static Vec2 Acceleration = new Vec2(0, 0);
        public const float Gravity = 0.0981f;
        public readonly bool IsKinematic;

        public readonly int Radius;
        public Vec2 Position;

        public Vec2 Velocity;
        private readonly Arrow velocityIndicator;

        private readonly float density = 1;

        private Vec2 oldPosition;

        // Mass = density * volume.
        // In 2D, we assume volume = area (=all objects are assumed to have the same "depth")
        public float Mass => Radius * Radius * density;

        public Ball(int pRadius, Vec2 pPosition, Vec2 pVelocity = new Vec2(), bool isKinematic = false) : base(pRadius * 2 + 1, pRadius * 2 + 1) {
            Radius = pRadius;
            Position = pPosition;
            Velocity = pVelocity;
            IsKinematic = isKinematic;

            Position = pPosition;
            UpdateScreenPosition();
            SetOrigin(Radius, Radius);

            Draw(230, 200, 0);

            velocityIndicator = new Arrow(Position, new Vec2(0, 0), 10);
            AddChild(velocityIndicator);
        }

        private void Draw(byte red, byte green, byte blue) {
            Fill(red, green, blue);
            Stroke(red, green, blue);
            Ellipse(Radius, Radius, 2 * Radius, 2 * Radius);
        }

        private void UpdateScreenPosition() {
            x = Position.x;
            y = Position.y;
        }

        public void Step() {
            Velocity += Acceleration * Gravity;
            oldPosition = Position;
            Position += Velocity;

            // This can be removed after adding line segment collision detection:
            // BoundaryWrapAround();

            // BallLineCollisions();

            var firstBallCollision = FindEarliestBallCollision();
            var firstLineCollision = FindEarliestLineCollision();
            var lineTOI = firstLineCollision?.TimeOfImpact ?? Mathf.Infinity;
            var ballTOI = firstBallCollision?.TimeOfImpact ?? Mathf.Infinity;
            if (lineTOI < ballTOI) {
                if(firstLineCollision != null) ResolveCollision(firstLineCollision);
                else if (firstBallCollision != null) ResolveCollision(firstBallCollision);
            } else  {
                if (firstBallCollision != null) ResolveCollision(firstBallCollision);
                else if(firstLineCollision != null) ResolveCollision(firstLineCollision);
            }

            UpdateScreenPosition();
            ShowDebugInfo();
        }

        private CollisionInfo FindEarliestBallCollision() {
            var myGame = (MyGame) game;
            var collisionInfo = new CollisionInfo(Vec2.Zero, null, Mathf.Infinity);

            // Check other movers:			
            for (var i = 0; i < myGame.GetNumberOfMovers(); i++) {
                var other = myGame.GetMover(i);
                if (other != this) {
                    // calculations
                    var u = Position - other.Position;
                    var dot = u.Dot(Velocity);
                    var radiusSquared = (Radius + other.Radius) * (Radius + other.Radius);
                    var uLengthSquared = u.sqrMagnitude;
                    var vLengthSquared = Velocity.sqrMagnitude;

                    // quadratic
                    var delta = (2 * dot) * (2 * dot) - 4 * vLengthSquared * (uLengthSquared - radiusSquared);
                    if (delta < 0) continue;
                    var upper = -2 * dot - Mathf.Sqrt(delta);
                    var lower = 2 * vLengthSquared;
                    var t = upper / lower;
                    if (t < 0 || t > 1) continue;

                    var pointOfImpact = oldPosition + Velocity * t;
                    var normal = (pointOfImpact - other.Position).normalized;
                    if (t < collisionInfo.TimeOfImpact) {
                        collisionInfo = new CollisionInfo(normal, other, t);
                    }
                }
            }

            if (float.IsPositiveInfinity(collisionInfo.TimeOfImpact)) return null;
            return collisionInfo;
        }

        private CollisionInfo FindEarliestLineCollision() {
            var myGame = (MyGame) game;
            var collisionInfo = new CollisionInfo(Vec2.Zero, null, Mathf.Infinity);

            // Check other movers:			
            for (var i = 0; i < myGame.GetNumberOfLines(); i++) {
                var line = myGame.GetLine(i);

                var segmentVec = line.End - line.Start;
                var normalizedSegmentVec = segmentVec.normalized;
                var segmentLengthSqr = segmentVec.sqrMagnitude;
                var normal = segmentVec.Normal();
                // Start of line
                var ballDiff = Position - line.Start;

                // Calculate time of impact
                var a = normal.Dot(ballDiff) - Radius;
                var b = -normal.Dot(Velocity);
                float t;
                if (b < 0) continue;
                if (a >= 0) {
                    t = a / b;
                } else if (a >= -Radius) {
                    t = 0;
                } else {
                    continue;
                }

                if (t > 1f) continue;

                var pointOfImpact = oldPosition + Velocity * t;
                var newDiff = pointOfImpact - line.Start;
                var distanceAlongLine = newDiff.Dot(normalizedSegmentVec);
                if (distanceAlongLine >= 0 && distanceAlongLine * distanceAlongLine <= segmentLengthSqr) {
                    if (t < collisionInfo.TimeOfImpact) {
                        collisionInfo = new CollisionInfo(normal, null, t);
                    }
                }
            }

            return float.IsPositiveInfinity(collisionInfo.TimeOfImpact) ? null : collisionInfo;
        }

        private void ResolveCollision(CollisionInfo col) {
            if (col.Other == null) {
                if(Wordy) Console.WriteLine($"Line|CollisionInfo{{TOI: {col.TimeOfImpact}, NORMAL: {col.Normal}}}");
                // Line collision
                Position = oldPosition + Velocity * col.TimeOfImpact;
                Velocity.Reflect(col.Normal, Bounciness);
            } else {
                if (!(col.Other is Ball other)) return;
                if(Wordy) Console.WriteLine($"Ball|CollisionInfo{{TOI: {col.TimeOfImpact}, NORMAL: {col.Normal}}}");

                Position = oldPosition + Velocity * col.TimeOfImpact;
                Velocity.Reflect(col.Normal, Bounciness);
                
                if(other.IsKinematic) return;
                
                other.Velocity.Reflect(col.Normal, Bounciness);
                
                // Attempt 1
                /*var u = (Mass * Velocity + other.Mass * other.Velocity) * (1 / (Mass + other.Mass));
                Velocity = u - Bounciness * (Velocity - u);
                other.Velocity = u - Bounciness * (other.Velocity - u);*/
                
                // Attempt 2
                /*var diff = Position - other.Position;
                diff.Normalize();
                Velocity += diff * (other.Mass / (Mass + other.Mass));
                other.Velocity -= diff * (Mass / (Mass + other.Mass));*/
                
                
            } /*

            // TODO: resolve the collision correctly: position reset & velocity reflection.
            // ...this is not an ideal collision resolve:
            Velocity *= -1;
            if (col.Other is Ball) {
                var otherBall = (Ball) col.Other;
                otherBall.Velocity *= -1;
            }*/
        }

        /// <summary>
        /// Discrete collision detection and resolve between ball and line segment
        /// </summary>
        private void BallLineCollisions() {
            var myGame = (MyGame) game;
            for (int i = 0; i < myGame.GetNumberOfLines(); i++) {
                var line = myGame.GetLine(i);

                var segmentVec = line.End - line.Start;
                var normalizedSegmentVec = segmentVec.normalized;
                var segmentLengthSqr = segmentVec.sqrMagnitude;
                var normal = segmentVec.Normal();

                // Start of line
                var ballDiff = Position - line.Start;
                var ballDistance = Math.Abs(ballDiff.Dot(normal));
                var projectionLength = ballDiff.Dot(normalizedSegmentVec);
                var projectionVector = projectionLength * normalizedSegmentVec;
                var projectionVectorDot = projectionVector.Dot(normalizedSegmentVec);
                var projectionLengthSqr = projectionLength * projectionLength;

                // Checks
                var ballDistanceCheck = ballDistance < Radius;
                var projectionLengthCheck = projectionLengthSqr < segmentLengthSqr;
                var projectionDirectionCheck = projectionVectorDot > 0;

                if (ballDistanceCheck && projectionLengthCheck && projectionDirectionCheck) {
                    SetColor(1, 0, 0);

                    // Reset position
                    Position += (-ballDistance + Radius) * normal;

                    // Reflect
                    Velocity.Reflect(normal, Bounciness);
                } else {
                    SetColor(0, 1, 0);
                }
            }
        }

        private void BoundaryWrapAround() {
            if (Position.x < 0)
                Position.x += game.width;

            if (Position.x > game.width)
                Position.x -= game.width;

            if (Position.y < 0)
                Position.y += game.height;

            if (Position.y > game.height)
                Position.y -= game.height;
        }

        private void ShowDebugInfo() {
            if (DrawDebugLine)
                ((MyGame) game).DrawLine(oldPosition, Position);

            velocityIndicator.StartPoint = Position;
            velocityIndicator.Vector = Velocity;
        }
    }
}