using System;
using DelaunayVoronoi;
using GXPEngine;

namespace physics_programming.testing {
    public struct Vec3 : IEquatable<Vec3> {
        // ReSharper disable once InconsistentNaming
        public float x;

        // ReSharper disable once InconsistentNaming
        public float y;

        // ReSharper disable once InconsistentNaming
        public float z;

        public Vec3(float x, float y, float z) {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        // ReSharper disable once InconsistentNaming
        /// <summary>
        ///     Sets the x and y components of this vector
        /// </summary>
        /// <param name="x">The new x component</param>
        /// <param name="y">The new y component</param>
        public void SetXYZ(float x, float y, float z) {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        /// <summary>
        ///     Returns the magnitude of this vector
        /// </summary>
        public float magnitude => (float) Math.Sqrt(x * x + y * y + z * z);
        /// <summary>
        ///     Returns the magnitude of this vector squared
        /// </summary>
        public float sqrMagnitude => x * x + y * y + z * z;

        /// <summary>
        ///     Returns this vector normalized
        /// </summary>
        public Vec3 normalized => magnitude > 0 ? this / magnitude : this;

        /// <summary>
        ///     Returns the dot product between this vector and <paramref name="other" />
        /// </summary>
        /// <param name="other">Other vector</param>
        /// <returns>The dot product between this vector and <paramref name="other" /></returns>
        public float Dot(Vec3 other) {
            return x * other.x + y * other.y + z * other.z;
        }

        /// <summary>
        /// Returns the cross product between this vector and <paramref name="other"/>
        /// </summary>
        /// <param name="other">Other vector</param>
        /// <returns>The cross product between this vector and <paramref name="other"/></returns>
        public Vec3 Cross(Vec3 other) {
            var cx = y * other.z - z * other.y;
            var cy = z * other.x - x * other.z;
            var cz = x * other.y - y * other.x;
            return new Vec3(cx, cy, cz);
        }

        /// <summary>
        ///     Calculates and returns the normal vector of the current vector
        /// </summary>
        /// <returns>Normal of the vector</returns>
        public Vec3 Normal() {
            var a = new Vec3(z, z, -x - y);
            var b = new Vec3(-y - z, x, x);
            if (Math.Abs(a.x) < 0.0000000001f && Math.Abs(a.y) < 0.0000000001f && Math.Abs(a.z) < 0.0000000001f) return b;
            return a;
        }

        /// <summary>
        ///     Normalizes the current vector
        /// </summary>
        public void Normalize() {
            var mag = magnitude;
            if (mag < 0.000001f) // Console.Error.WriteLine($"Division by magnitude of zero while normalizing Vec2: {this}");
                return;

            x /= mag;
            y /= mag;
            z /= mag;
        }
        
        /// <summary>
        ///     Projects a point on a line segment
        /// </summary>
        /// <param name="q">Point to project</param>
        /// <param name="p0">Start of line segment</param>
        /// <param name="p1">End of line segment</param>
        /// <returns>
        ///     <paramref name="q" /> projected on the line segment from <paramref name="p0" /> to <paramref name="p1" />
        /// </returns>
        public static Vec2 ProjectPointOnLineSegment(Vec2 q, Vec2 p0, Vec2 p1) {
            var a = p1.x - p0.x;
            var b = p1.y - p0.y;
            var c = p0.y - p1.y;

            var pYN = p0.y * a - p0.x * b - q.x * c - q.y * (c * b / a);
            var pYD = a - c * b / a;
            var pY = pYN / pYD;

            var pXN = q.x * a + q.y * b - pY * b;
            var pXD = a;
            var pX = pXN / pXD;

            return new Vec2(pX, pY);
        }

        public static Vec3 operator +(Vec3 a, Vec3 b) {
            return new Vec3(a.x + b.x, a.y + b.y, a.z + b.z);
        }

        public static Vec3 operator -(Vec3 a, Vec3 b) {
            return new Vec3(a.x - b.x, a.y - b.y, a.z - b.z);
        }

        public static Vec3 operator *(Vec3 a, float d) {
            return new Vec3(a.x * d, a.y * d, a.z * d);
        }

        public static Vec3 operator *(float d, Vec3 a) {
            return new Vec3(a.x * d, a.y * d, a.z * d);
        }

        public static Vec3 operator /(Vec3 a, float d) {
            if (d < 0.000001f) {
                Console.Error.WriteLine($"Division by zero {a}/{d}. Returning same vector without dividing.");
                return a;
            }

            return new Vec3(a.x / d, a.y / d, a.z / d);
        }

        public static bool operator ==(Vec3 a, Vec3 b) {
            return a.Equals(b);
        }

        public static bool operator !=(Vec3 a, Vec3 b) {
            return !a.Equals(b);
        }

        public static Vec3 operator -(Vec3 a) {
            return new Vec3(-a.x, -a.y, -a.z);
        }

        public bool Equals(Vec3 other) {
            return x.Equals(other.x) && y.Equals(other.y) && z.Equals(other.z);
        }

        public override bool Equals(object obj) {
            return obj is Vec3 other && Equals(other);
        }

        public override string ToString() {
            return $"({x},{y})";
        }

        public static implicit operator Vec3(Vec2 vec2) {
            return new Vec3(vec2.x, vec2.y, 0f);
        }

        public static implicit operator Vec3(Vector2 vector2) {
            return new Vec3(vector2.x, vector2.y, 0f);
        }

        public static implicit operator Vec3(Vector2Int vector2Int) {
            return new Vec3(vector2Int.x, vector2Int.y, 0f);
        }

        public static implicit operator Vec3(Vector3 vector3) {
            return new Vec3(vector3.x, vector3.y, vector3.z);
        }

        public static implicit operator Vec3((float, float, float) valueTuple) {
            return new Vec3(valueTuple.Item1, valueTuple.Item2, valueTuple.Item3);
        }

        public static implicit operator Vec3(Point point) {
            return new Vec3((float)point.X, (float)point.Y, 0f);
        }

        public static readonly Vec3 Zero = new Vec3(0f, 0f, 0f);
        public static readonly Vec3 One = new Vec3(1f, 1f, 1f);
        public static readonly Vec3 PositiveInfinity = new Vec3(float.PositiveInfinity, float.PositiveInfinity, float.PositiveInfinity);
        public static readonly Vec3 NegativeInfinity = new Vec3(float.NegativeInfinity, float.NegativeInfinity, float.NegativeInfinity);
    }
}