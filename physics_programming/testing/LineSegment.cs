
using GXPEngine;
using GXPEngine.Core;
using GXPEngine.OpenGL;

namespace physics_programming.testing {
    public class LineSegment : GameObject {
        public readonly uint Color;
        public readonly uint LineWidth;
        public Vec2 End;
        public Vec2 Start;

        public LineSegment(Vec2 start, Vec2 end, uint color = 0xffffffff, uint lineWidth = 1) {
            Start = start;
            End = end;
            Color = color;
            LineWidth = lineWidth;
        }

        //------------------------------------------------------------------------------------------------------------------------
        //														RenderSelf()
        //------------------------------------------------------------------------------------------------------------------------
        protected override void RenderSelf(GLContext glContext) {
            if (game != null)
                RenderLine(Start, End, Color, LineWidth);
        }

        public static void RenderLine(Vec2 start, Vec2 end, uint color = 0xffffffff, uint lineWidth = 1, bool useGlobalCoords = false) {
            RenderLine(start.x, start.y, end.x, end.y, color, lineWidth, useGlobalCoords);
        }

        public static void RenderLine(float startX, float startY, float endX, float endY, uint color = 0xffffffff, uint lineWidth = 1, bool useGlobalCoords = false) {
            if (useGlobalCoords) GL.LoadIdentity();
            GL.Disable(GL.TEXTURE_2D);
            GL.LineWidth(lineWidth);
            GL.Color4ub((byte) ((color >> 16) & 0xff), (byte) ((color >> 8) & 0xff), (byte) (color & 0xff), (byte) ((color >> 24) & 0xff));
            float[] vertices = {startX, startY, endX, endY};
            GL.EnableClientState(GL.VERTEX_ARRAY);
            GL.VertexPointer(2, GL.FLOAT, 0, vertices);
            GL.DrawArrays(GL.LINES, 0, 2);
            GL.DisableClientState(GL.VERTEX_ARRAY);
            GL.Enable(GL.TEXTURE_2D);
        }
    }
}