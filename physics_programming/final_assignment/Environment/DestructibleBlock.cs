using System;
using System.Collections.Generic;
using GXPEngine;
using physics_programming.final_assignment.Components;

namespace physics_programming.final_assignment {
    public class DestructibleBlock : EasyDraw {
        public readonly LineSegment Length1;
        public readonly LineSegment Length2;

        public readonly LineSegment Side1;
        public readonly LineSegment Side2;

        public bool ShouldRemove;
        private readonly List<Vec2> blockVertices;

        private float blockWidth;
        private Vec2 end;
        private Vec2 start;

        public DestructibleBlock(float blockWidth, Vec2 start, Vec2 end) : base(Globals.WIDTH, Globals.HEIGHT) {
            this.blockWidth = blockWidth;
            this.start = start;
            this.end = end;

            var length1Start = start;
            var length1End = end;
            Length1 = new LineSegment(length1Start, length1End);
            var length1Vector = end - start;
            var length1Normal = length1Vector.Normal();
            // Length1.AddChild(new Arrow((Length1.Start + Length1.End) / 2f, (Length1.End - Length1.Start).Normal() * 5, 10));

            var side1Start = start - length1Normal * blockWidth;
            var side1End = start;
            Side1 = new LineSegment(side1Start, side1End);
            // Side1.AddChild(new Arrow((Side1.Start + Side1.End) / 2f, (Side1.End - Side1.Start).Normal() * 2.5f, 10));

            var side2Start = end;
            var side2End = end - length1Normal * blockWidth;
            Side2 = new LineSegment(side2Start, side2End);
            // Side2.AddChild(new Arrow((Side2.Start + Side2.End) / 2f, (Side2.End - Side2.Start).Normal() * 2.5f, 10));

            var length2Start = Side2.End;
            var length2End = Side1.Start;
            Length2 = new LineSegment(length2Start, length2End);
            // Length2.AddChild(new Arrow((Length2.Start + Length2.End) / 2f, (Length2.End - Length2.Start).Normal() * 5, 10));

            AddChild(Length1);
            AddChild(Length2);
            AddChild(Side1);
            AddChild(Side2);

            //Setup vertices and draw
            blockVertices = new List<Vec2>();
            blockVertices.AddRange(new[] {Length1.Start, Length2.End, Length2.Start, Length1.End});
            Draw();
        }

        public DestructibleBlock(List<Vec2> vertices) : base(Globals.WIDTH, Globals.HEIGHT) {
            blockWidth = (vertices[1] - vertices[0]).magnitude;
            start = vertices[0];
            end = vertices[3];
            blockVertices = vertices;
            Length1 = new LineSegment(vertices[0], vertices[3]);
            Length2 = new LineSegment(vertices[2], vertices[1]);
            Side1 = new LineSegment(vertices[1], vertices[0]);
            Side2 = new LineSegment(vertices[3], vertices[2]);

            // Length1.AddChild(new Arrow((Length1.Start + Length1.End) / 2f, (Length1.End - Length1.Start).Normal() * 5, 10));
            // Length2.AddChild(new Arrow((Length2.Start + Length2.End) / 2f, (Length2.End - Length2.Start).Normal() * 5, 10));
            // Side1.AddChild(new Arrow((Side1.Start + Side1.End) / 2f, (Side1.End - Side1.Start).Normal() * 2.5f, 10));
            // Side2.AddChild(new Arrow((Side2.Start + Side2.End) / 2f, (Side2.End - Side2.Start).Normal() * 2.5f, 10));

            AddChild(Length1);
            AddChild(Length2);
            AddChild(Side1);
            AddChild(Side2);
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

        public static ValueTuple<DestructibleBlock, DestructibleBlock> Split(DestructibleBlock block, Vec2 point, float size) {
            var distanceLength1 = point.Dot(block.Length1.Start);
            var distanceLength2 = point.Dot(block.Length2.Start);
            var flipped = false;
            LineSegment firstSide;
            LineSegment secondSide;
            Vec2 secondProjection;
            if (distanceLength1 < distanceLength2) {
                var normal = (block.Length2.End - block.Length2.Start).Normal();
                secondProjection = point + normal * block.blockWidth;
                // secondProjection = Vec2.ProjectPointOnLineSegment(point, block.Length2.Start, block.Length2.End);
                firstSide = block.Length1;
                secondSide = block.Length2;
            } else {
                var normal = (block.Length1.End - block.Length1.Start).Normal();
                secondProjection = point - normal * block.blockWidth;
                firstSide = block.Length2;
                secondSide = block.Length1;
                flipped = true;
            }

            var (firstSideSplitLeft, firstSideSplitRight) = LineSegment.Split(firstSide, point, size);
            var (secondSideSplitRight, secondSideSplitLeft) = LineSegment.Split(secondSide, secondProjection, size);
            DestructibleBlock left = null;
            DestructibleBlock right = null;

            if (flipped) {
                if (firstSideSplitLeft != null) left = new DestructibleBlock(new List<Vec2> {secondSideSplitLeft.Start, firstSideSplitLeft.End, firstSideSplitLeft.Start, secondSideSplitLeft.End});
                if (firstSideSplitRight != null) right = new DestructibleBlock(new List<Vec2> {secondSideSplitRight.Start, firstSideSplitRight.End, firstSideSplitRight.Start, secondSideSplitRight.End});
            } else {
                if (firstSideSplitLeft != null) left = new DestructibleBlock(new List<Vec2> {firstSideSplitLeft.Start, secondSideSplitLeft.End, secondSideSplitLeft.Start, firstSideSplitLeft.End});
                if (firstSideSplitRight != null) right = new DestructibleBlock(new List<Vec2> {firstSideSplitRight.Start, secondSideSplitRight.End, secondSideSplitRight.Start, firstSideSplitRight.End});
            }
            return ValueTuple.Create(left, right);
        }
    }
}