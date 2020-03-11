using System;
using System.Runtime.CompilerServices;
using GXPEngine;
using NUnit.Framework.Constraints;

namespace physics_programming {
    public struct Vec2 : IEquatable<Vec2> {
        // ReSharper disable once InconsistentNaming
        public float x;

        // ReSharper disable once InconsistentNaming
        public float y;

        public Vec2(float x, float y) {
            this.x = x;
            this.y = y;
        }

        // ReSharper disable once InconsistentNaming
        public void SetXY(float x, float y) {
            this.x = x;
            this.y = y;
        }

        public float magnitude => (float) Math.Sqrt(x * x + y * y);
        public float sqrMagnitude => x * x + y * y;
        public Vec2 normalized => this / magnitude;

        public void Normalize() {
            var mag = magnitude;
            if (mag < 0.000001f) {
                // Console.Error.WriteLine($"Division by magnitude of zero while normalizing Vec2: {this}");
                return;
            }

            x /= mag;
            y /= mag;
        }

        public void SetAngleDegrees(float degrees) {
            SetAngleRadians(Deg2Rad(degrees));
        }

        public void SetAngleRadians(float radians) {
            var m = magnitude;
            var unit = GetUnitVectorRad(radians);
            SetXY(unit.x * m, unit.y * m);
        }

        public float GetAngleDegrees() {
            return Rad2Deg(GetAngleRadians());
        }

        public float GetAngleRadians() {
            var n = normalized;
            var angle = Mathf.Acos(n.x);
            return angle;
        }

        public void RotateDegrees(float degrees) {
            RotateRadians(Deg2Rad(degrees));
        }

        public void RotateRadians(float radians) {
            var c = Mathf.Cos(radians);
            var s = Mathf.Sin(radians);
            var newX = x * c - y * s;
            var newY = x * s + y * c;
            SetXY(newX, newY);
        }

        public void RotateAroundDegrees(Vec2 point, float degrees) {
            RotateAroundRadians(point, Deg2Rad(degrees));
        }

        public void RotateAroundRadians(Vec2 point, float radians) {
            var copy = this;
            copy -= point;
            copy.RotateRadians(radians);
            copy += point;
            SetXY(copy.x, copy.y);
        }

        public static float Deg2Rad(float degrees) {
            return degrees / 180.0f * Mathf.PI;
        }
        
        public static float Rad2Deg(float radians) {
            return radians * 180.0f / Mathf.PI;
        }

        public static Vec2 GetUnitVectorDeg(float degrees) {
            return GetUnitVectorRad(Deg2Rad(degrees));
        }

        public static Vec2 GetUnitVectorRad(float radians) {
            return new Vec2(Mathf.Cos(radians), Mathf.Sin(radians));
        }

        public static Vec2 RandomUnitVector() {
            var random = new Random(Time.now);
            var rad = Mathf.PI * 2f * (float) random.NextDouble();
            return GetUnitVectorRad(rad);
        }

        public static Vec2 operator +(Vec2 a, Vec2 b) {
            return new Vec2(a.x + b.x, a.y + b.y);
        }

        public static Vec2 operator -(Vec2 a, Vec2 b) {
            return new Vec2(a.x - b.x, a.y - b.y);
        }

        public static Vec2 operator *(Vec2 a, float d) {
            return new Vec2(a.x * d, a.y * d);
        }

        public static Vec2 operator *(float d, Vec2 a) {
            return new Vec2(a.x * d, a.y * d);
        }

        public static Vec2 operator /(Vec2 a, float d) {
            if (d < 0.000001f) {
                Console.Error.WriteLine($"Division by zero {a}/{d}. Returning same vector without dividing.");
                return a;
            }

            return new Vec2(a.x / d, a.y / d);
        }

        public static bool operator ==(Vec2 a, Vec2 b) {
            return a.Equals(b);
        }

        public static bool operator !=(Vec2 a, Vec2 b) {
            return !a.Equals(b);
        }

        public bool Equals(Vec2 other) {
            return x.Equals(other.x) && y.Equals(other.y);
        }

        public override bool Equals(object obj) {
            return obj is Vec2 other && Equals(other);
        }

        public override int GetHashCode() {
            unchecked {
                return (x.GetHashCode() * 397) ^ y.GetHashCode();
            }
        }

        public override string ToString() {
            return $"({x},{y})";
        }

        public static readonly Vec2 Zero = new Vec2(0f, 0f); 
        public static readonly Vec2 One = new Vec2(1f, 1f); 
        public static readonly Vec2 Left = new Vec2(-1f, 0f); 
        public static readonly Vec2 Right = new Vec2(1f, 0f);
        public static readonly Vec2 Up = new Vec2(0f, -1f); 
        public static readonly Vec2 Down = new Vec2(0f, 1f); 
        public static readonly Vec2 PositiveInfinity = new Vec2(float.PositiveInfinity, float.PositiveInfinity); 
        public static readonly Vec2 NegativeInfinity = new Vec2(float.NegativeInfinity, float.NegativeInfinity); 
    }
}