using GXPEngine;
using GXPEngine.Core;

namespace physics_programming.assignment4.Components {
    public class Arrow : GameObject {
        public readonly float ScaleFactor;
        public readonly uint Color;
        public readonly uint LineWidth;
        public Vec2 StartPoint;
        public Vec2 Vector;

        public Arrow(Vec2 pStartPoint, Vec2 pVector, float pScale, uint pColor = 0xffffffff, uint pLineWidth = 1) {
            StartPoint = pStartPoint;
            Vector = pVector;
            ScaleFactor = pScale;

            Color = pColor;
            LineWidth = pLineWidth;
        }

        protected override void RenderSelf(GLContext glContext) {
            var endPoint = StartPoint + Vector * ScaleFactor;
            LineSegment.RenderLine(StartPoint, endPoint, Color, LineWidth, true);

            var smallVec = Vector.normalized * -10; // constant length 10, opposite direction of vector
            var left = new Vec2(-smallVec.y, smallVec.x) + smallVec + endPoint;
            var right = new Vec2(smallVec.y, -smallVec.x) + smallVec + endPoint;

            LineSegment.RenderLine(endPoint, left, Color, LineWidth, true);
            LineSegment.RenderLine(endPoint, right, Color, LineWidth, true);
        }
    }
}