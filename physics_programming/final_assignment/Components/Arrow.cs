using GXPEngine;
using GXPEngine.Core;

namespace physics_programming.final_assignment.Components {
    public class Arrow : GameObject {
        public float ScaleFactor;

        public uint Color = 0xffffffff;
        public uint LineWidth = 1;
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
            LineSegment.RenderLine(StartPoint, endPoint, pColor:Color, pLineWidth:LineWidth, pUseGlobalCoords: true);

            var smallVec = Vector.normalized * -10; // constant length 10, opposite direction of vector
            var left = new Vec2(-smallVec.y, smallVec.x) + smallVec + endPoint;
            var right = new Vec2(smallVec.y, -smallVec.x) + smallVec + endPoint;

            LineSegment.RenderLine(endPoint, left, pColor:Color, pLineWidth:LineWidth, pUseGlobalCoords: true);
            LineSegment.RenderLine(endPoint, right, pColor:Color, pLineWidth:LineWidth, pUseGlobalCoords: true);
        }
    }
}