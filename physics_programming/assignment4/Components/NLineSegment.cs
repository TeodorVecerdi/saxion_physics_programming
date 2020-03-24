using GXPEngine.Core;

namespace physics_programming.assignment4.Components {
	/// <summary>
	///     Implements a line with normal representation
	/// </summary>
	public class NLineSegment : LineSegment {
        private readonly Arrow normal;

        public NLineSegment(float pStartX, float pStartY, float pEndX, float pEndY, uint pColor = 0xffffffff, uint pLineWidth = 1, bool pGlobalCoords = false)
            : this(new Vec2(pStartX, pStartY), new Vec2(pEndX, pEndY), pColor, pLineWidth) { }

        public NLineSegment(Vec2 pStart, Vec2 pEnd, uint pColor = 0xffffffff, uint pLineWidth = 1, bool pGlobalCoords = false)
            : base(pStart, pEnd, pColor, pLineWidth, pGlobalCoords) {
            normal = new Arrow(new Vec2(0, 0), new Vec2(0, 0), 40, 0xffff0000);
            AddChild(normal);
        }

        //------------------------------------------------------------------------------------------------------------------------
        //														RenderSelf()
        //------------------------------------------------------------------------------------------------------------------------
        protected override void RenderSelf(GLContext glContext) {
            if (game != null) {
                RecalculateArrowPosition();
                RenderLine(Start, End, Color, LineWidth);
            }
        }

        private void RecalculateArrowPosition() {
            normal.StartPoint = (Start + End) * 0.5f;
            normal.Vector = (End - Start).Normal();
        }
    }
}