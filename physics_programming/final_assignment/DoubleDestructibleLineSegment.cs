using System;
using GXPEngine;
using GXPEngine.Core;

namespace physics_programming.final_assignment {
    public class DoubleDestructibleLineSegment : GameObject {
        public bool ShouldRemove;
        public DestructibleLineSegment SideA;
        public DestructibleLineSegment SideB;
        public CircleCollider StartCollider;
        public CircleCollider EndCollider;

        public DoubleDestructibleLineSegment(Vec2 pStart, Vec2 pEnd, uint pColor = 0xffffffff, uint pLineWidth = 1) {
            SideA = new DestructibleLineSegment(pStart, pEnd, pColor, pLineWidth);
            SideB = new DestructibleLineSegment(pEnd, pStart, pColor, pLineWidth);
            AddChild(SideA);
            AddChild(SideB);
            StartCollider = new CircleCollider(0, pStart, true);
            EndCollider = new CircleCollider(0, pEnd, true);
            AddChild(StartCollider);
            AddChild(EndCollider);
        }

        //------------------------------------------------------------------------------------------------------------------------
        //														RenderSelf()
        //------------------------------------------------------------------------------------------------------------------------
        protected override void RenderSelf(GLContext glContext) {
        }
        
        public static ValueTuple<DoubleDestructibleLineSegment, DoubleDestructibleLineSegment> Split(DoubleDestructibleLineSegment a, Vec2 point, float size) {
            var split1 = DestructibleLineSegment.Split(a.SideA, point, size);
            var split2 = DestructibleLineSegment.Split(a.SideB, point, size);
            DoubleDestructibleLineSegment lineLeft;
            DoubleDestructibleLineSegment lineRight;
            if (split1.Item1 == null || split2.Item2 == null) {
                lineLeft = null;
            } else {
                lineLeft = new DoubleDestructibleLineSegment(split1.Item1.Start, split1.Item1.End, split1.Item1.Color, split1.Item1.LineWidth);
            }

            if (split1.Item2 == null || split2.Item1 == null) {
                lineRight = null;
            } else {
                lineRight = new DoubleDestructibleLineSegment(split1.Item2.Start, split1.Item2.End, split1.Item2.Color, split1.Item2.LineWidth);
            }
            return ValueTuple.Create(lineLeft, lineRight);
        }
    }
}