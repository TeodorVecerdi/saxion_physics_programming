using System;
using System.Collections.Generic;
using GXPEngine;
using physics_programming.final_assignment.Utils;
using physics_programming.final_assignment.Components;

namespace physics_programming.testing {
    public class DestructibleBlock : EasyDraw {
        public readonly LineSegment Length1;
        public readonly LineSegment Length2;

        private readonly LineSegment side1;
        private readonly LineSegment side2;
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
            Length1.AddChild(new Arrow((Length1.Start + Length1.End) / 2f, (Length1.End - Length1.Start).Normal() * 5, 10));

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
            Length2 = new LineSegment(length2Start, length2End);
            Length2.AddChild(new Arrow((Length2.Start + Length2.End) / 2f, (Length2.End - Length2.Start).Normal() * 5, 10));

            AddChild(Length1);
            AddChild(Length2);
            AddChild(side1);
            AddChild(side2);

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
            side1 = new LineSegment(vertices[1], vertices[0]);
            side2 = new LineSegment(vertices[3], vertices[2]);

            Length1.AddChild(new Arrow((Length1.Start + Length1.End) / 2f, (Length1.End - Length1.Start).Normal() * 5, 10));
            Length2.AddChild(new Arrow((Length2.Start + Length2.End) / 2f, (Length2.End - Length2.Start).Normal() * 5, 10));
            side1.AddChild(new Arrow((side1.Start + side1.End) / 2f, (side1.End - side1.Start).Normal() * 2.5f, 10));
            side2.AddChild(new Arrow((side2.Start + side2.End) / 2f, (side2.End - side2.Start).Normal() * 2.5f, 10));

            AddChild(Length1);
            AddChild(Length2);
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

        public static ValueTuple<DestructibleBlock, DestructibleBlock> Split(DestructibleBlock block, Vec2 point, float size) {
            var distanceLength1 = point.Dot(block.Length1.End - block.Length1.Start);
            var distanceLength2 = point.Dot(block.Length2.End - block.Length2.Start);
            bool flipped = false;
            LineSegment firstSide;
            LineSegment secondSide;
            Vec2 secondProjection;
            if (distanceLength1 < distanceLength2) {
                secondProjection = Vec2.ProjectPointOnLineSegment(point, block.Length2.Start, block.Length2.End);
                firstSide = block.Length1;
                secondSide = block.Length2;
            } else {
                secondProjection = Vec2.ProjectPointOnLineSegment(point, block.Length1.Start, block.Length1.End);
                firstSide = block.Length2;
                secondSide = block.Length1;
                flipped = true;
            }

            var (firstSideSplitLeft, firstSideSplitRight) = LineSegment.Split(firstSide, point, size);
            var (secondSideSplitLeft, secondSideSplitRight) = LineSegment.Split(secondSide, secondProjection, size);
            DestructibleBlock left;
            DestructibleBlock right;
            
            if (flipped) {
                left = new DestructibleBlock(new List<Vec2> {secondSideSplitLeft.Start, firstSideSplitLeft.End, firstSideSplitLeft.Start, secondSideSplitLeft.End} );
                right = new DestructibleBlock(new List<Vec2> {secondSideSplitRight.Start, firstSideSplitRight.End, firstSideSplitRight.Start, secondSideSplitRight.End} );
            } else {
                left = new DestructibleBlock(new List<Vec2> {firstSideSplitLeft.Start, secondSideSplitLeft.End, secondSideSplitLeft.Start, firstSideSplitLeft.End} );
                right = new DestructibleBlock(new List<Vec2> {firstSideSplitRight.Start, secondSideSplitRight.End, secondSideSplitRight.Start, firstSideSplitRight.End} );
            }
            return ValueTuple.Create(left, right);
        }
    }
}