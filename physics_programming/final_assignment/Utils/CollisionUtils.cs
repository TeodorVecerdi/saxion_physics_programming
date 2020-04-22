using System;
using GXPEngine;
using physics_programming.final_assignment.Components;

namespace physics_programming.final_assignment.Utils {
    public static class CollisionUtils {
        public static CollisionInfo CircleLineCollision(Vec2 circlePosition, Vec2 oldCirclePosition, Vec2 circleVelocity, float circleRadius, LineSegment line) {
            var segmentVec = line.End - line.Start;
            var normalizedSegmentVec = segmentVec.normalized;
            var segmentLengthSqr = segmentVec.sqrMagnitude;
            var normal = segmentVec.Normal();
            var ballDiff = circlePosition - line.Start;

            // Calculate time of impact
            var a = normal.Dot(ballDiff) - circleRadius;
            var b = -normal.Dot(circleVelocity);
            float t;
            if (b < 0) return null;
            if (a >= 0)
                t = a / b;
            else if (a >= -circleRadius)
                t = 0;
            else return null;

            if (t > 1f) return null;

            var pointOfImpact = oldCirclePosition + circleVelocity * t;
            var newDiff = pointOfImpact - line.Start;
            var distanceAlongLine = newDiff.Dot(normalizedSegmentVec);
            if (distanceAlongLine >= 0 && distanceAlongLine * distanceAlongLine <= segmentLengthSqr)
                return new CollisionInfo(normal, null, t);

            return null;
        }

        public static CollisionInfo CircleDestructibleLineCollision(Vec2 circlePosition, Vec2 oldCirclePosition, Vec2 circleVelocity, float circleRadius, DoubleDestructibleLineSegment line, bool doEdges = true) {
            var collisionInfoSideA = CircleLineCollision(circlePosition, oldCirclePosition, circleVelocity, circleRadius, new LineSegment(line.SideA.Start, line.SideA.End));
            var collisionInfoSideB = CircleLineCollision(circlePosition, oldCirclePosition, circleVelocity, circleRadius, new LineSegment(line.SideB.Start, line.SideB.End));
            CollisionInfo collisionInfoEdge = null;
            if (doEdges) {
                var collisionInfoEdgeA = CircleCircleCollision(circlePosition, oldCirclePosition, circleVelocity, circleRadius, line.StartCollider.Position, line.StartCollider.Radius);
                var collisionInfoEdgeB = CircleCircleCollision(circlePosition, oldCirclePosition, circleVelocity, circleRadius, line.EndCollider.Position, line.EndCollider.Radius);
                if (collisionInfoEdgeA != null || collisionInfoEdgeB != null)
                    collisionInfoEdge = collisionInfoEdgeA ?? collisionInfoEdgeB;
            }

            if (collisionInfoSideA != null || collisionInfoSideB != null) {
                var collisionInfo = collisionInfoSideA ?? collisionInfoSideB;
                if (doEdges && collisionInfoEdge != null && collisionInfoEdge.TimeOfImpact < collisionInfo.TimeOfImpact)
                    return collisionInfoEdge;
                return collisionInfo;
            }

            if (doEdges && collisionInfoEdge != null)
                return collisionInfoEdge;
            return null;
        }

        public static (DoubleDestructibleLineSegment, DoubleDestructibleLineSegment) BulletLineCollision(Bullet bullet, DoubleDestructibleLineSegment line) {
            var collisionInfo = CircleDestructibleLineCollision(bullet.Position, bullet.OldPosition, bullet.Velocity * Time.deltaTime, bullet.Radius, line, false);
            if (collisionInfo != null) {
                var pointOfImpact = bullet.OldPosition + bullet.Velocity * Time.deltaTime * collisionInfo.TimeOfImpact;

                var projectedPoint = Vec2.ProjectPointOnLineSegment(pointOfImpact, line.SideA.Start, line.SideA.End);

                var splitSegments = DoubleDestructibleLineSegment.Split(line, projectedPoint, Globals.World.BulletDLSDamage);
                return splitSegments;
            }

            return ValueTuple.Create<DoubleDestructibleLineSegment, DoubleDestructibleLineSegment>(null, null);
        }

        public static (DestructibleBlock, DestructibleBlock, CollisionInfo) BulletBlockCollision(Bullet bullet, DestructibleBlock block) {
            var collisionInfoLengthA = CircleLineCollision(bullet.Position, bullet.OldPosition, bullet.Velocity * Time.deltaTime, bullet.Radius, block.Length1);
            var collisionInfoLengthB = CircleLineCollision(bullet.Position, bullet.OldPosition, bullet.Velocity * Time.deltaTime, bullet.Radius, block.Length2);
            if (collisionInfoLengthA != null) {
                var pointOfImpact = bullet.OldPosition + bullet.Velocity * Time.deltaTime * collisionInfoLengthA.TimeOfImpact;
                var projectedPoint = Vec2.ProjectPointOnLineSegment(pointOfImpact, block.Length1.Start, block.Length1.End);
                var splitSegments = DestructibleBlock.Split(block, projectedPoint, Globals.World.BulletDLSDamage);
                return (splitSegments.Item1, splitSegments.Item2, null);
            }

            if (collisionInfoLengthB != null) {
                var pointOfImpact = bullet.OldPosition + bullet.Velocity * Time.deltaTime * collisionInfoLengthB.TimeOfImpact;
                var projectedPoint = Vec2.ProjectPointOnLineSegment(pointOfImpact, block.Length1.Start, block.Length1.End);
                var splitSegments = DestructibleBlock.Split(block, projectedPoint, Globals.World.BulletDLSDamage);
                return (splitSegments.Item1, splitSegments.Item2, null);
            }

            var collisionInfoSideA = CircleLineCollision(bullet.Position, bullet.OldPosition, bullet.Velocity * Time.deltaTime, bullet.Radius, block.Side1);
            var collisionInfoSideB = CircleLineCollision(bullet.Position, bullet.OldPosition, bullet.Velocity * Time.deltaTime, bullet.Radius, block.Side2);
            if (collisionInfoSideA != null && collisionInfoSideB != null)
                return (null, null, collisionInfoSideA.TimeOfImpact < collisionInfoSideB.TimeOfImpact ? collisionInfoSideA : collisionInfoSideB);
            if (collisionInfoSideA != null)
                return (null, null, collisionInfoSideA);
            if (collisionInfoSideB != null)
                return (null, null, collisionInfoSideB);
            return (null, null, null);
        }

        public static CollisionInfo CircleCircleCollision(Vec2 position, Vec2 oldPosition, Vec2 velocity, float radius, Vec2 otherPosition, float otherRadius) {
            var u = position - otherPosition;
            var dot = u.Dot(velocity);
            var radiusSquared = (radius + otherRadius) * (radius + otherRadius);
            var uLengthSquared = u.sqrMagnitude;
            var vLengthSquared = velocity.sqrMagnitude;

            // quadratic
            var delta = 4 * dot * dot - 4 * vLengthSquared * (uLengthSquared - radiusSquared);
            if (delta < 0) return null;
            var upper = -2 * dot - Mathf.Sqrt(delta);
            var lower = 2 * vLengthSquared;
            var t = upper / lower;
            if (t < 0 || t > 1) return null;

            var pointOfImpact = oldPosition + velocity * t;
            var normal = (pointOfImpact - otherPosition).normalized;
            return new CollisionInfo(normal, null, t);
        }
    }
}