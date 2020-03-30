using System;

namespace GXPEngine
{
	public class Arrow:GameObject
	{
		public Vec2 startPoint;
		public Vec2 vector;

		public float scaleFactor;

		public uint color = 0xffffffff;
		public uint lineWidth = 1;

		public Arrow (Vec2 pStartPoint, Vec2 pVector, float pScale, uint pColor = 0xffffffff, uint pLineWidth = 1)
		{
			startPoint = pStartPoint;
			vector = pVector;
			scaleFactor = pScale;

			color = pColor;
			lineWidth = pLineWidth;
		}

		protected override void RenderSelf (GXPEngine.Core.GLContext glContext)
		{
			Vec2 endPoint = startPoint + vector * scaleFactor;
			LineSegment.RenderLine (startPoint, endPoint, color, lineWidth, true);

			Vec2 smallVec = vector.Normalized() * -10; // constant length 10, opposite direction of vector
			Vec2 left = new Vec2 (-smallVec.y, smallVec.x) + smallVec + endPoint;
			Vec2 right = new Vec2 (smallVec.y, -smallVec.x) + smallVec + endPoint;

			LineSegment.RenderLine (endPoint, left, color, lineWidth, true);
			LineSegment.RenderLine (endPoint, right, color, lineWidth, true);			
		}
	}
}

