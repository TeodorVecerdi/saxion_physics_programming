using GXPEngine;

namespace physics_programming.final_assignment {
    public class CircleCollider : EasyDraw {
        public readonly bool IsKinematic;
        public readonly int Radius;
        public Vec2 Position;

        private bool isVisible;
        private Vec2 oldPosition;
        
        public bool IsVisible {
            get => isVisible;
            set {
                isVisible = value;
                Draw(0x00, 0xff, 0x00, isVisible ? (byte) 0xff : (byte) 0x00);
            }
        }

        public CircleCollider(int radius, Vec2 offset, bool isKinematic = false, bool isVisible = true) : base(radius * 2 + 1, radius * 2 + 1) {
            Radius = radius;
            Position = offset;
            IsKinematic = isKinematic;

            UpdateScreenPosition();
            SetOrigin(Radius, Radius);

            IsVisible = isVisible;
            Draw(0, 255, 0);
        }

        private void Draw(byte red, byte green, byte blue, byte alpha = 0xff) {
            NoFill();
            Stroke(red, green, blue, alpha);
            Ellipse(Radius, Radius, 2 * Radius, 2 * Radius);
        }

        private void UpdateScreenPosition() {
            x = Position.x;
            y = Position.y;
        }

        public void Step() {
            // Position = parentPosition + offset;
            UpdateScreenPosition();
        }

        public CollisionInfo FindEarliestLineCollision(Vec2 parentVelocity) {
            var myGame = (MyGame) game;
            var collisionInfo = new CollisionInfo(Vec2.Zero, null, Mathf.Infinity);

            // Check other movers:			
            for (var i = 0; i < myGame.GetNumberOfLines(); i++) {
                var line = myGame.GetLine(i);
                var segmentVec = line.End - line.Start;
                var normalizedSegmentVec = segmentVec.normalized;
                var segmentLengthSqr = segmentVec.sqrMagnitude;
                var normal = segmentVec.Normal();
                var positionDifference = Position - line.Start;

                // Calculate time of impact
                var a = normal.Dot(positionDifference) - Radius;
                var b = -normal.Dot(parentVelocity);
                float t;
                if (b < 0) continue;
                if (a >= 0)
                    t = a / b;
                else if (a >= -Radius)
                    t = 0;
                else
                    continue;

                if (t > 1f) continue;

                var pointOfImpact = oldPosition + parentVelocity * t;
                var newDiff = pointOfImpact - line.Start;
                var distanceAlongLine = newDiff.Dot(normalizedSegmentVec);
                if (distanceAlongLine >= 0 && distanceAlongLine * distanceAlongLine <= segmentLengthSqr)
                    if (t < collisionInfo.TimeOfImpact)
                        collisionInfo = new CollisionInfo(normal, null, t);
            }

            return float.IsPositiveInfinity(collisionInfo.TimeOfImpact) ? null : collisionInfo;
        }
    }
}