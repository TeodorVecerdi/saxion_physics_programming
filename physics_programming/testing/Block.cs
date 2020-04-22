using System.Collections.Generic;
using GXPEngine;
using physics_programming.assignment5.Components;

namespace physics_programming.testing {
    public class Block : EasyDraw {
        private readonly LineSegment length1;
        private readonly LineSegment length2;
        private readonly LineSegment side1;
        private readonly LineSegment side2;
        private readonly List<Vec2> blockVertices;

        private float blockWidth;
        private Vec2 end;
        private Vec2 start;

        public Block(float blockWidth, Vec2 start, Vec2 end) : base(Globals.WIDTH, Globals.HEIGHT) {
            this.blockWidth = blockWidth;
            this.start = start;
            this.end = end;

            var length1Start = start;
            var length1End = end;
            length1 = new LineSegment(length1Start, length1End);
            var length1Vector = end - start;
            var length1Normal = length1Vector.Normal();
            length1.AddChild(new Arrow((length1.Start + length1.End) / 2f, (length1.End - length1.Start).Normal() * 5, 10));

            var side1Start = start - length1Normal * blockWidth;
            var side1End = start;
            side1 = new LineSegment(side1Start, side1End);
            side1.AddChild(new Arrow((side1.Start + side1.End) / 2f, (side1.End - side1.Start).Normal() * 2.5f, 10));

            var side2Start = end;
            var side2End = end - length1Normal * blockWidth;
            side2 = new LineSegment(side2Start, side2End);
            side2.AddChild(new Arrow((side2.Start + side2.End) / 2f, (side2.End - side2.Start).Normal() * 2.5f, 10));

            var length2Start = side2.End;
            var length2End = side1.Start;
            length2 = new LineSegment(length2Start, length2End);
            length2.AddChild(new Arrow((length2.Start + length2.End) / 2f, (length2.End - length2.Start).Normal() * 5, 10));

            AddChild(length1);
            AddChild(length2);
            AddChild(side1);
            AddChild(side2);

            //Setup vertices and draw
            blockVertices = new List<Vec2>();
            blockVertices.AddRange(new[] {length1.Start, length2.End, length2.Start, length1.End});
            Draw();
        }

        public Block(List<Vec2> vertices) : base(Globals.WIDTH, Globals.HEIGHT) {
            blockWidth = (vertices[1] - vertices[0]).magnitude;
            start = vertices[0];
            end = vertices[3];
            blockVertices = vertices;
            length1 = new LineSegment(vertices[0], vertices[3]);
            length2 = new LineSegment(vertices[2], vertices[1]);
            side1 = new LineSegment(vertices[1], vertices[0]);
            side2 = new LineSegment(vertices[3], vertices[2]);

            length1.AddChild(new Arrow((length1.Start + length1.End) / 2f, (length1.End - length1.Start).Normal() * 5, 10));
            length2.AddChild(new Arrow((length2.Start + length2.End) / 2f, (length2.End - length2.Start).Normal() * 5, 10));
            side1.AddChild(new Arrow((side1.Start + side1.End) / 2f, (side1.End - side1.Start).Normal() * 2.5f, 10));
            side2.AddChild(new Arrow((side2.Start + side2.End) / 2f, (side2.End - side2.Start).Normal() * 2.5f, 10));

            AddChild(length1);
            AddChild(length2);
            AddChild(side1);
            AddChild(side2);

            Draw();
        }

        private void Draw() {
            Fill(255, 255, 255, 100);
            Quad((int) blockVertices[0].x, (int) blockVertices[0].y,
                (int) blockVertices[1].x, (int) blockVertices[1].y,
                (int) blockVertices[2].x, (int) blockVertices[2].y,
                (int) blockVertices[3].x, (int) blockVertices[3].y
            );
        }
    }
}