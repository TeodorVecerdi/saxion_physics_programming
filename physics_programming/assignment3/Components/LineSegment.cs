using System;
using GXPEngine.Core;
using GXPEngine.OpenGL;
using physics_programming;

namespace GXPEngine
{
	/// <summary>
	/// Implements an OpenGL line
	/// </summary>
	public class LineSegment : GameObject
	{
		public Vec2 start;
		public Vec2 end;

		public uint color = 0xffffffff;
		public uint lineWidth = 1;

		//stay in this object's coordinate space or interpret vectors as screen coordinates?
		public bool useGlobalCoords = false;

		public LineSegment (float pStartX, float pStartY, float pEndX, float pEndY, uint pColor = 0xffffffff, uint pLineWidth = 1, bool pGlobalCoords = false)
			: this (new Vec2 (pStartX, pStartY), new Vec2 (pEndX, pEndY), pColor, pLineWidth)
		{
		}

		public LineSegment (Vec2 pStart, Vec2 pEnd, uint pColor = 0xffffffff, uint pLineWidth = 1, bool pGlobalCoords = false)
		{
			start = pStart;
			end = pEnd;
			color = pColor;
			lineWidth = pLineWidth;
			useGlobalCoords = pGlobalCoords;
		}
	
		//------------------------------------------------------------------------------------------------------------------------
		//														RenderSelf()
		//------------------------------------------------------------------------------------------------------------------------
		override protected void RenderSelf(GLContext glContext) {
			if (game != null) {
				RenderLine (start, end, color, lineWidth);
			}
		}

		static public void RenderLine (Vec2 pStart, Vec2 pEnd, uint pColor = 0xffffffff, uint pLineWidth = 1, bool pGlobalCoords = false) {
			RenderLine (pStart.x, pStart.y, pEnd.x, pEnd.y, pColor, pLineWidth, pGlobalCoords);
		}

		static public void RenderLine (float pStartX, float pStartY, float pEndX, float pEndY, uint pColor = 0xffffffff, uint pLineWidth = 1, bool pGlobalCoords = false) {
			if (pGlobalCoords) GL.LoadIdentity ();
			GL.Disable (GL.TEXTURE_2D);
			GL.LineWidth (pLineWidth);
			GL.Color4ub ((byte)((pColor >> 16) & 0xff), (byte)((pColor >> 8) & 0xff), (byte)((pColor) & 0xff),(byte)((pColor >> 24) & 0xff));
			float[] vertices = new float[] { pStartX, pStartY, pEndX, pEndY };
			GL.EnableClientState( GL.VERTEX_ARRAY );
			GL.VertexPointer( 2, GL.FLOAT, 0, vertices);
			GL.DrawArrays(GL.LINES, 0, 2);
			GL.DisableClientState(GL.VERTEX_ARRAY);
			GL.Enable(GL.TEXTURE_2D);
		}
	
		
	}
}

