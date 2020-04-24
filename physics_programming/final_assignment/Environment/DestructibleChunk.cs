using System.Drawing;
using DelaunayVoronoi;
using GXPEngine;

namespace physics_programming.final_assignment {
    public class DestructibleChunk : EasyDraw {
        public Components.Triangle Triangle;

        public DestructibleChunk(Components.Triangle triangle) : base(Globals.WIDTH, Globals.HEIGHT) {
            Triangle = triangle;
        }

        public void Draw() {
            Clear(Color.Transparent);
            Stroke(Color.Crimson);
            Fill(Color.Crimson, 127);
            base.Triangle(Triangle.P1.x, Triangle.P1.y,
                Triangle.P2.x, Triangle.P2.y,
                Triangle.P3.x, Triangle.P3.y);
        }
    }
}