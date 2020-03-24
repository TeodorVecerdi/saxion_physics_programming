using GXPEngine;

namespace physics_programming.assignment3.Components
{
	public class Arrow:GameObject
	{
		public Vec2 startPoint;
		public Vec2 vector;

		public float drawScale;

		public uint color = 0xffffffff;
		public uint lineWidth = 1;

		public Arrow (Vec2 pStartPoint, Vec2 pVector, float pScale, uint pColor = 0xffffffff, uint pLineWidth = 1)
		{
			startPoint = pStartPoint;
			vector = pVector;
			drawScale = pScale;

			color = pColor;
			lineWidth = pLineWidth;
		}



		protected override void RenderSelf (GXPEngine.Core.GLContext glContext)
		{
			// NB: After you insert your own extended Vec2 struct here, 
			// it's a lot easier and clearer to implement the next lines using
			// methods such as Add, Scale, Length, Normalize, Rotate, etc.!

			Vec2 endPoint = new Vec2 (
				                startPoint.x + vector.x * drawScale,
				                startPoint.y + vector.y * drawScale
			                );
			LineSegment.RenderLine (startPoint, endPoint, color, lineWidth, true);

			float length = Mathf.Sqrt (vector.x * vector.x + vector.y * vector.y); // length of vector
			Vec2 smallVec = new Vec2 (-10 * vector.x / length, -10 * vector.y / length); // constant length 10, opposite direction of vector
			Vec2 left = new Vec2 (-smallVec.y, smallVec.x); // rotate 90 degrees
			Vec2 right = new Vec2 (smallVec.y, -smallVec.x); // rotate -90 degrees
			left += smallVec;
			left += endPoint;
			right += smallVec;
			right += endPoint;

			LineSegment.RenderLine (endPoint, left, color, lineWidth, true);
			LineSegment.RenderLine (endPoint, right, color, lineWidth, true);			
		}
	}
}

