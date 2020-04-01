using GXPEngine;
using GXPEngine.Core;
using GXPEngine.OpenGL;

namespace physics_programming.final_assignment.Components {
    /// <summary>
    ///     Implements an OpenGL line
    /// </summary>
    public class LineSegment : GameObject {
        //stay in this object's coordinate space or interpret vectors as screen coordinates?
        public uint Color = 0xffffffff;
        public uint LineWidth = 1;
        public Vec2 End;
        public Vec2 Start;

        public LineSegment(float pStartX, float pStartY, float pEndX, float pEndY, uint pColor = 0xffffffff, uint pLineWidth = 1)
            : this(new Vec2(pStartX, pStartY), new Vec2(pEndX, pEndY), pColor, pLineWidth) { }

        public LineSegment(Vec2 pStart, Vec2 pEnd, uint pColor = 0xffffffff, uint pLineWidth = 1) {
            Start = pStart;
            End = pEnd;
            Color = pColor;
            LineWidth = pLineWidth;
        }

        //------------------------------------------------------------------------------------------------------------------------
        //														RenderSelf()
        //------------------------------------------------------------------------------------------------------------------------
        protected override void RenderSelf(GLContext glContext) {
            if (game != null) {
                RenderLine(Start, End, Color, LineWidth);
            }
        }

        public static void RenderLine(Vec2 pStart, Vec2 pEnd, uint pColor = 0xffffffff, uint pLineWidth = 1, bool pUseGlobalCoords = false) {
            RenderLine(pStart.x, pStart.y, pEnd.x, pEnd.y, pColor, pLineWidth, pUseGlobalCoords);
        }

        public static void RenderLine(float pStartX, float pStartY, float pEndX, float pEndY, uint pColor = 0xffffffff, uint pLineWidth = 1, bool pUseGlobalCoords = false) {
            if(pUseGlobalCoords) GL.LoadIdentity();
            GL.Disable(GL.TEXTURE_2D);
            GL.LineWidth(pLineWidth);
            GL.Color4ub((byte) ((pColor >> 16) & 0xff), (byte) ((pColor >> 8) & 0xff), (byte) (pColor & 0xff), (byte) ((pColor >> 24) & 0xff));
            float[] vertices = {pStartX, pStartY, pEndX, pEndY};
            GL.EnableClientState(GL.VERTEX_ARRAY);
            GL.VertexPointer(2, GL.FLOAT, 0, vertices);
            GL.DrawArrays(GL.LINES, 0, 2);
            GL.DisableClientState(GL.VERTEX_ARRAY);
            GL.Enable(GL.TEXTURE_2D);
        }
    }
}