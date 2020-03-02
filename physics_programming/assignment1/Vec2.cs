using System;
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
    }
}