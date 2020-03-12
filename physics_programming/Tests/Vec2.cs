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

        [Test]
        public void Deg2Rad() {
            var degrees = 288.477720936f;
            var radians = Vec2.Deg2Rad(degrees);
            var expected = 5.034886048982f;
            Assert.IsTrue(Math.Abs(radians - expected) < tolerance);
        }
        
        [Test]
        public void Rad2Deg() {
            var radians = 3.494212610f;
            var degrees = Vec2.Rad2Deg(radians);
            var expected = 200.2036353f;
            Assert.IsTrue(Math.Abs(degrees - expected) < tolerance);
        }

        [Test]
        public void RandomUnitVector() {
            var unit = Vec2.RandomUnitVector();
            var length = unit.sqrMagnitude;
            var expected = 1f;
            Assert.IsTrue(Math.Abs(length - expected) < tolerance);
        }

        [Test]
        public void GetUnitVector() {
            var unit = Vec2.GetUnitVectorDeg(90);
            var expected = Vec2.Down;
            var diff = expected - unit;
            diff.x = Math.Abs(diff.x);
            diff.y = Math.Abs(diff.y);
            Assert.IsTrue(diff.x < tolerance && diff.y < tolerance, $"unit({unit}) == expected({expected})");
        }

        [Test]
        public void SetAngle() {
            var a = new Vec2(10, 0);
            var expected = new Vec2(0, 10);
            a.SetAngleDegrees(90);
            var diff = expected - a;
            diff.x = Math.Abs(diff.x);
            diff.y = Math.Abs(diff.y);
            Assert.IsTrue(diff.x < tolerance && diff.y < tolerance, $"a({a}) == expected({expected})");
        }

        [Test]
        public void GetAngle() {
            var a = new Vec2(0, 10);
            var angle = a.GetAngleDegrees();
            Assert.IsTrue(Math.Abs(90 - angle) < tolerance, $"angle({angle}) == expected(90)");
        }

        [Test]
        public void Rotate() {
            var a = new Vec2(3, 8);
            var expected = new Vec2(-5.4282032302755091741f, 6.598076211353315f);
            a.RotateDegrees(60);
            var diff = expected - a;
            diff.x = Math.Abs(diff.x);
            diff.y = Math.Abs(diff.y);
            Assert.IsTrue(diff.x < tolerance && diff.y < tolerance, $"a({a}) == expected({expected})");
        }

        [Test]
        public void RotateAround() {
            var a = new Vec2(4, 6);
            var p = new Vec2(2, 1);
            var expected = new Vec2(-3, 3);
            a.RotateAroundDegrees(p, 90.0f);
            var diff = expected - a;
            diff.x = Math.Abs(diff.x);
            diff.y = Math.Abs(diff.y);
            Assert.IsTrue(diff.x < tolerance && diff.y < tolerance, $"a({a}) == expected({expected})");
        }
    }
}