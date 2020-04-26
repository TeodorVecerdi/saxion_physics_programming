using System;
using GXPEngine;
using NUnit.Framework;

namespace physics_programming.tests {
    [TestFixture(TestOf = typeof(Vec2))]
    public class Vec2Tests {
        private const float tolerance = 0.0001f;

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

        [Test]
        public void Dot() {
            var a = new Vec2(4, 3);
            var b = new Vec2(3, -2);
            var dot = a.Dot(b);
            var expected = 4 * 3 + 3 * -2;
            var diff = Math.Abs(dot - expected);
            Assert.IsTrue(diff < tolerance, $"a{a} dot b{b} = {dot}, expected {expected}");
        }

        [Test]
        public void Normal() {
            var a = new Vec2(3, -4);
            var normal = a.Normal();
            var expected = new Vec2(4, 3).normalized;
            var diff = expected - normal;
            diff.x = Math.Abs(diff.x);
            diff.y = Math.Abs(diff.y);
            Assert.IsTrue(diff.x < tolerance && diff.y < tolerance, $"normal({a}) = {normal}, expected({expected})");
        }

        [Test]
        public void Reflect() {
            var velocity = new Vec2(-4.908581f, -8.712396f);
            var normal = (new Vec2(200, 300) - new Vec2(100, 150)).Normal();
            var reflect1 = velocity;
            var reflect2 = velocity;
            reflect1.Reflect(normal);
            reflect2.Reflect(normal, 0.85f);
            var expected1 = new Vec2(-6.154295f, -7.881919f);
            var expected2 = new Vec2(-6.060867f, -7.944205f);
            var diff1 = expected1 - reflect1;
            diff1.x = Math.Abs(diff1.x);
            diff1.y = Math.Abs(diff1.y);
            var diff2 = expected2 - reflect2;
            diff2.x = Math.Abs(diff2.x);
            diff2.y = Math.Abs(diff2.y);
            Assert.IsTrue(diff1.x < tolerance && diff1.y < tolerance, $"{velocity}.Reflect({normal}, 1f) = {reflect1}, expected({expected1})");
            Assert.IsTrue(diff2.x < tolerance && diff2.y < tolerance, $"{velocity}.Reflect({normal}, 0.85f) = {reflect2}, expected({expected2})");
        }

        [Test]
        public void Op_UnaryNegation() {
            var vector = Vec2.RandomUnitVector();
            var expected = new Vec2(-vector.x, -vector.y);
            var actual = -vector;
            var diff = expected - actual;
            diff.x = Math.Abs(diff.x);
            diff.y = Math.Abs(diff.y);
            Assert.IsTrue(diff.x < tolerance && diff.y < tolerance, $"-{vector} = {actual}, expected({expected})");
        }

        [Test]
        public void Det() {
            var a = new Vec2(-8.493346f, 5.278548f);
            var b = new Vec2(-2.075286f, 9.78229f);
            var actual = a.Det(b);
            var expected = -72.12988f;
            var diff = Math.Abs(expected - actual);
            Assert.IsTrue(diff < tolerance, $"{a}.Det({b}) = {actual}, expected {expected}");
        }

        [Test]
        public void ProjectPointOnLineSegment() {
            var segmentStart = new Vec2(147f, 210f);
            var segmentEnd = new Vec2(605f, 373f);
            var point = new Vec2(115.677f,192.64f);
            var projected = Vec2.ProjectPointOnLineSegment(point, segmentStart, segmentEnd);
            var expected = new Vec2(113.7146f, 198.1539f);
            var diff = expected - projected;
            diff.x = Math.Abs(diff.x);
            diff.y = Math.Abs(diff.y);
            Assert.IsTrue(diff.x < tolerance && diff.y < tolerance, $"Vec2.Project({point}, start, end) = {projected}, expected {expected}, diff {diff}");
        }
    }
}