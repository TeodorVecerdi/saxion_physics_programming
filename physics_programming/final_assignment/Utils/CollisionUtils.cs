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
            if (a >= 0) {
                t = a / b;
            } else if (a >= -circleRadius) {
                t = 0;
            } else return null;

            if (t > 1f) return null;

            var pointOfImpact = oldCirclePosition + circleVelocity * t;
            var newDiff = pointOfImpact - line.Start;
            var distanceAlongLine = newDiff.Dot(normalizedSegmentVec);
            if (distanceAlongLine >= 0 && distanceAlongLine * distanceAlongLine <= segmentLengthSqr) {
                return new CollisionInfo(normal, null, t);
            }

            return null;
        }
    }
}