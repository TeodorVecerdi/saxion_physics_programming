using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using DelaunayVoronoi;
using GXPEngine;
using Point = DelaunayVoronoi.Point;

namespace physics_programming.testing {
    public class MainAppTesting : Game {
        private readonly DelaunayTriangulator delaunay = new DelaunayTriangulator();
        private readonly EasyDraw draw;
        private readonly float maxX = 800f, maxY = 200f;
        private readonly Quad mappedQuad;
        private readonly Quad originalQuad;
        private bool drawPoints;
        private bool drawProjection;
        private bool drawTriangulation;
        private bool drawVoronoi;
        private IEnumerable<Edge> edges;

        private IEnumerable<Point> points;
        private IEnumerable<Triangle> triangles;
        private int pointAmount = 10;

        private MainAppTesting() : base(Globals.WIDTH, Globals.HEIGHT, Globals.FULLSCREEN, Globals.VSYNC, pPixelArt: Globals.PIXEL_ART, windowTitle: Globals.WINDOW_TITLE) {
            targetFps = 60;

            originalQuad = new Quad(
                new Point(0 + 50, 50 + 0 + 50), //1
                new Point(maxX + 50, 50 + 0 + 50), //2
                new Point(maxX + 50, 50 + maxY + 50), //3
                new Point(0 + 50, 50 + maxY + 50) //4
                );
            mappedQuad = new Quad(
                new Point(0 + 50, 50 + 0 + maxY + 25 + 50), //1
                new Point(maxX + 50, 50 + 0 + maxY + 25 + 50), //2
                new Point(maxX + 50, 50 + maxY + maxY + 25 + 50), //3
                new Point(0 + 50, 50 + maxY + maxY + 25 + 50) //4
                );

            draw = new EasyDraw(Globals.WIDTH, Globals.HEIGHT, false);
            AddChild(draw);
            var point = new Vec2(75, 125);
            var projection = MapPointOnQuad(point, originalQuad, mappedQuad);
            var expected = new Vec2(75, 125 + maxY + 25);
            Debug.Log($"original: {point}, projected: {projection}, expected: {expected}");
            Environment.Exit(-1);
            Reset();
        }

        private void Update() {
            if (Input.GetKeyDown(Key.ONE)) {
                drawPoints = !drawPoints;
                Draw();
            }

            if (Input.GetKeyDown(Key.TWO)) {
                drawVoronoi = !drawVoronoi;
                Draw();
            }

            if (Input.GetKeyDown(Key.THREE)) {
                drawTriangulation = !drawTriangulation;
                Draw();
            }
            
            if (Input.GetKeyDown(Key.FOUR)) {
                drawProjection = !drawProjection;
                Draw();
            }

            if (Input.GetKeyDown(Key.SPACE))
                Reset();

            if (Input.GetKeyDown(Key.UP) || Input.GetKey(Key.UP) && Input.GetKey(Key.LEFT_SHIFT)) {
                pointAmount++;
                Draw();
            }

            if (Input.GetKeyDown(Key.DOWN) || Input.GetKey(Key.DOWN) && Input.GetKey(Key.LEFT_SHIFT)) {
                pointAmount--;
                Draw();
            }
        }

        private void Reset() {
            points = delaunay.GeneratePoints(pointAmount, maxX, maxY);
            triangles = delaunay.BowyerWatson(points);
            edges = Voronoi.GenerateEdgesFromDelaunay(triangles);
            Draw();
        }

        private void Draw() {
            draw.Clear(Color.Transparent);
            DrawQuad(originalQuad, Color.Beige, Color.Aquamarine);
            DrawQuad(mappedQuad, Color.Beige, Color.Fuchsia);
            if (drawTriangulation) DrawTriangulation(triangles, originalQuad);
            if (drawVoronoi) DrawVoronoi(edges, originalQuad);
            if (drawPoints) DrawPoints(points, originalQuad);
            DrawDebug();
        }

        private void DrawPoints(IEnumerable<Point> points, Quad quad = null) {
            var offsetX = quad == null ? 0f : (float) quad.P4.X;
            var offsetY = quad == null ? 0f : (float) quad.P4.Y;
            foreach (var point in points) {
                draw.NoStroke();
                draw.Fill(Color.Chartreuse);
                draw.Ellipse((float) point.X + offsetX, (float) point.Y + offsetY, 8, 8);
                if (drawProjection) {
                    var vec2Point = new Vec2((float) point.X, (float) point.Y);
                    var projectedPoint = MapPointOnQuad(vec2Point, originalQuad, mappedQuad);
                    draw.Fill(Color.Gold);
                    draw.Ellipse(projectedPoint.x + offsetX, projectedPoint.y + offsetY, 8, 8);
                }
            }
        }

        private void DrawVoronoi(IEnumerable<Edge> edges, Quad quad = null) {
            var offsetX = quad == null ? 0f : (float) quad.P4.X;
            var offsetY = quad == null ? 0f : (float) quad.P4.Y;
            foreach (var edge in edges) {
                draw.NoFill();
                draw.Stroke(Color.DodgerBlue);
                draw.Line((float) edge.Point1.X + offsetX, (float) edge.Point1.Y + offsetY, (float) edge.Point2.X + offsetX, (float) edge.Point2.Y + offsetY);
            }
        }

        private void DrawTriangulation(IEnumerable<Triangle> triangles, Quad quad = null) {
            var offsetX = quad == null ? 0f : (float) quad.P4.X;
            var offsetY = quad == null ? 0f : (float) quad.P4.Y;
            foreach (var triangle in triangles) {
                draw.Stroke(Color.Crimson);
                draw.NoFill();

                // draw.Fill(Color.Crimson, 127);
                draw.Triangle((float) triangle.Vertices[0].X + offsetX, (float) triangle.Vertices[0].Y + offsetY,
                    (float) triangle.Vertices[1].X + offsetX, (float) triangle.Vertices[1].Y + offsetY,
                    (float) triangle.Vertices[2].X + offsetX, (float) triangle.Vertices[2].Y + offsetY);
            }
        }

        private void DrawDebug() {
            draw.NoStroke();
            draw.Fill(Color.White);
            draw.TextSize(14);
            draw.Text($"Points: {pointAmount}", 0, 25);
            draw.Fill(Color.GreenYellow);
            if (!drawPoints) draw.Fill(Color.OrangeRed);
            draw.Text("[POINTS]", 0, 50);
            draw.Fill(Color.GreenYellow);
            if (!drawVoronoi) draw.Fill(Color.OrangeRed);
            draw.Text("[VORONOI]", 90, 50);
            draw.Fill(Color.GreenYellow);
            if (!drawTriangulation) draw.Fill(Color.OrangeRed);
            draw.Text("[TRIANGULATION]", 200, 50);
            draw.Fill(Color.GreenYellow);
            if (!drawProjection) draw.Fill(Color.OrangeRed);
            draw.Text("[PROJECTION]", 370, 50);
        }

        private void DrawQuad(Quad quad, Color fill, Color stroke) {
            draw.Fill(fill, 127);
            draw.Stroke(stroke);
            draw.Quad((float) quad.P1.X, (float) quad.P1.Y, (float) quad.P2.X, (float) quad.P2.Y, (float) quad.P3.X, (float) quad.P3.Y, (float) quad.P4.X, (float) quad.P4.Y);
            draw.Fill(stroke);
            draw.Ellipse((float) quad.P1.X, (float) quad.P1.Y, 8, 8);
            draw.Ellipse((float) quad.P2.X, (float) quad.P2.Y, 8, 8);
            draw.Ellipse((float) quad.P3.X, (float) quad.P3.Y, 8, 8);
            draw.Ellipse((float) quad.P4.X, (float) quad.P4.Y, 8, 8);
        }

        private Vec2 MapPointOnQuad(Vec2 point, Quad original, Quad target) {
            var p01 = new Vec2((float) original.P1.X, (float) original.P1.Y);
            var p02 = new Vec2((float) original.P2.X, (float) original.P2.Y);
            var p03 = new Vec2((float) original.P3.X, (float) original.P3.Y);
            var p04 = new Vec2((float) original.P4.X, (float) original.P4.Y);
            var a0 = p01;
            var b0 = p02 - p01;
            var c0 = p03 - p01;
            var d0 = p04 - p02 - p03 + p01;

            var p11 = new Vec2((float) target.P1.X, (float) target.P1.Y);
            var p12 = new Vec2((float) target.P2.X, (float) target.P2.Y);
            var p13 = new Vec2((float) target.P3.X, (float) target.P3.Y);
            var p14 = new Vec2((float) target.P4.X, (float) target.P4.Y);
            var a1 = p11;
            var b1 = p12 - p11;
            var c1 = p13 - p11;
            var d1 = p14 - p12 - p13 + p11;

            var u = b0;
            var v = c0;

            var p0 = point;
            var a = (p0 - a0).Dot(u);
            var b = b0.Dot(u);
            var c = c0.Dot(u);
            var f = (p0 - a0).Dot(v);
            var g = b0.Dot(v);
            var h = d0.Dot(v);
            var A = -h * b / c;
            var B = h * a / c;
            var C = -f;
            var delta = B*B - 4 * A * C;
            var sqrt_delta = Mathf.Sqrt(delta);
            var ee1 = (-B + sqrt_delta) / 2 * A;
            var ee2 = (-B - sqrt_delta) / 2 * A;
            
            // var ee1 = (-(g + h * a / c) + Mathf.Sqrt(delta)) / -2f * h * b / c;
            // var ee2 = (-(g + h * a / c) - Mathf.Sqrt(delta)) / -2f * h * b / c;
            // var ee1 = (-(g + h * a / c) + Mathf.Sqrt(delta)) / 2f * f;
            // var ee2 = (-(g + h * a / c) - Mathf.Sqrt(delta)) / 2f * f;
            Debug.LogWarning($"ee1: {ee1}, ee2: {ee2}");
            float ee;
            if (ee1 >= 0f && ee1 <= 1f) ee = ee1;
            else ee = ee2;
            var nn = (a - b * ee) / c;

            var p1 = a1 + b1 * ee + c1 * nn + d1 * ee * nn;
            return p1;
        }

        public static void Main() {
            new MainAppTesting().Start();
        }
    }
}