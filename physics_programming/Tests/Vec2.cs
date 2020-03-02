using System;
using GXPEngine;
using NUnit.Framework;

namespace physics_programming.tests {
    [TestFixture(TestOf = typeof(Vec2))]
    public class Vec2Tests {
        private const float tolerance = 0.000001f;

        [Test]
        public void Create() {
            var v = new Vec2(2, 3);
            Assert.IsTrue(Math.Abs(v.x - 2) < tolerance && Math.Abs(v.y - 3) < tolerance, $"new Vec2(2, 3) should be (2,3), instead got {v}");
        }

        [Test]
        public void Set() {
            var v = new Vec2(0, 0);
            v.SetXY(2, 3);
            Assert.IsTrue(Math.Abs(v.x - 2) < tolerance && Math.Abs(v.y - 3) < tolerance, $"v.SetXY(2, 3) should be (2,3), instead got {v}");
        }

        [Test]
        public void Magnitude() {
            var v = new Vec2(3, 4);
            var mag = v.magnitude;
            Assert.AreEqual(mag, 5f, tolerance, $"{v}.magnitude should be 5f, instead got {mag}");
        }

        [Test]
        public void SqrMagnitude() {
            var v = new Vec2(3, 4);
            var sqMag = v.sqrMagnitude;
            Assert.AreEqual(sqMag, 25f, tolerance, $"{v}.magnitude should be 25f, instead got {sqMag}");
        }

        [Test]
        public void Normalized() {
            var v = new Vec2(0, 3);
            var n = v.normalized;
            Assert.IsTrue(Math.Abs(n.x) < tolerance && Math.Abs(n.y - 1) < tolerance, $"(0,3).normalized should be (0,1), instead got {n}");
        }

        [Test]
        public void Normalize() {
            var v = new Vec2(0, 3);
            v.Normalize();
            Assert.IsTrue(Math.Abs(v.x) < tolerance && Math.Abs(v.y - 1) < tolerance, $"(0,3).normalized should be (0,1), instead got {v}");
        }

        [Test]
        public void Add() {
            var a = new Vec2(2, 3);
            var b = new Vec2(3, 4);
            var c = a + b;
            Assert.IsTrue(Math.Abs(c.x - 5) < tolerance && Math.Abs(c.y - 7) < tolerance, $"{a} + {b} should be (5, 7), instead got {c}");
        }

        [Test]
        public void Subtract() {
            var a = new Vec2(2, 3);
            var b = new Vec2(3, 4);
            var c = a - b;
            Assert.IsTrue(Math.Abs(c.x - -1) < tolerance && Math.Abs(c.y - -1) < tolerance, $"{a} - {b} should be (-1, -1), instead got {c}");
        }

        [Test]
        public void MultiplyScalarLeft() {
            var a = new Vec2(2, 3);
            var d = 5f;
            var c = a * d;
            Assert.IsTrue(Math.Abs(c.x - 10) < tolerance && Math.Abs(c.y - 15) < tolerance, $"{a} * {d} should be (10, 15), instead got {c}");
        }

        [Test]
        public void MultiplyScalarRight() {
            var a = new Vec2(2, 3);
            var d = 5f;
            var c = d * a;
            Assert.IsTrue(Math.Abs(c.x - 10) < tolerance && Math.Abs(c.y - 15) < tolerance, $"{d} * {a} should be (10, 15), instead got {c}");
        }

        [Test]
        public void DivideScalar() {
            var a = new Vec2(6, 3);
            var d = 3f;
            var c = a / d;
            Assert.IsTrue(Math.Abs(c.x - 2) < tolerance && Math.Abs(c.y - 1) < tolerance, $"{a} / {d} should be (2, 1), instead got {c}");
        }

        [Test]
        public void EqualsTrue() {
            var a = new Vec2(2, 3);
            var b = new Vec2(2, 3);
            var c = a == b;
            Assert.IsTrue(c, $"{a} == {b} should be {true}, instead got {c}");
        }

        [Test]
        public void EqualsFalse() {
            var a = new Vec2(2, 3);
            var b = new Vec2(3, 2);
            var c = a == b;
            Assert.IsFalse(c, $"{a} == {b} should be {false}, instead got {c}");
        }

        [Test]
        public void NotEqualsTrue() {
            var a = new Vec2(2, 3);
            var b = new Vec2(3, 2);
            var c = a != b;
            Assert.IsTrue(c, $"{a} != {b} should be {true}, instead got {c}");
        }

        [Test]
        public void NotEqualsFalse() {
            var a = new Vec2(2, 3);
            var b = new Vec2(2, 3);
            var c = a != b;
            Assert.IsFalse(c, $"{a} != {b} should be {false}, instead got {c}");
        }
    }
}