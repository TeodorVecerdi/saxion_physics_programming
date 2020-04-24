using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using DelaunayVoronoi;
using GXPEngine;
using physics_programming.final_assignment;
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
        private int pointAmount = 4;

        private MainAppTesting() : base(Globals.WIDTH, Globals.HEIGHT, Globals.FULLSCREEN, Globals.VSYNC, pPixelArt: Globals.PIXEL_ART, windowTitle: Globals.WINDOW_TITLE) {
            targetFps = 60;

            originalQuad = new Quad(
                new Point(0 + 50, 50 + 0 + 50), //1
                new Point(maxX + 50, 50 + 0 + 50), //2
                new Point(maxX + 50, 50 + maxY + 50), //3
                new Point(0 + 50, 50 + maxY + 50) //4
            );
            var vec = new Vec2(50 + maxX, 300 + maxY + maxY) - new Vec2(50, 100 + maxY);

            var block = new DestructibleBlock(maxY, new Vec2(50, 100 + maxY), vec.normalized * maxX + new Vec2(50, 100 + maxY));
            // var block = new DestructibleBlock(maxY, new Point(maxX + 50, 50 + 0 + 50), new Point(0 + 50, 50 + 0 + 50));

            // mappedQuad = new Quad(
            //     (Vec2) (50f, 100f),
            //     (Vec2) (850f, 300f),
            //     (Vec2) (801.49287f, 494.0285f),
            //     (Vec2) (1.49287f, 294.0285f)
            // );
            mappedQuad = new Quad(
                block.BlockVertices[1],
                block.BlockVertices[2],
                block.BlockVertices[3],
                block.BlockVertices[0]
            );

            // Debug.Log(((Vec2)mappedQuad.P2 - (Vec2)mappedQuad.P1).GetAngleDegrees());
            draw = new EasyDraw(Globals.WIDTH, Globals.HEIGHT, false);
            AddChild(draw);

            // var point = new Vec2(75, 125);
            // var projection = MapPointOnQuad(point, mappedQuad);
            // var expected = new Vec2(75, 125 + maxY + 25);
            // Debug.Log($"original: {point}, projected: {projection}, expected: {expected}");
            // Environment.Exit(-1);
            Reset();
            // Debug.Log((Vec2)points.ToList()[0]);
            // Debug.Log((Vec2)triangles.ToList()[0].Vertices[0]);
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
            var offsetX = quad == null ? 0f : (float) quad.P1.x;
            var offsetY = quad == null ? 0f : (float) quad.P1.y;
            var i = 0;
            foreach (var point in points) {
                var p = new Vec2((float) point.X + offsetX, (float) point.Y + offsetY);
                draw.NoStroke();
                draw.Fill(Color.Chartreuse);
                draw.Ellipse(p.x, p.y, 8, 8);
                draw.Text($"{i}", p.x, p.y);
                if (drawProjection) {
                    var projectedPoint = MapPointOnQuad2(p, originalQuad, mappedQuad);
                    draw.Fill(Color.Gold);
                    draw.Ellipse(projectedPoint.x, projectedPoint.y, 8, 8);
                    draw.Text($"{i}", projectedPoint.x, projectedPoint.y);
                }

                i++;
            }
        }

        private void DrawVoronoi(IEnumerable<Edge> edges, Quad quad = null) {
            var offsetX = quad == null ? 0f : (float) quad.P1.x;
            var offsetY = quad == null ? 0f : (float) quad.P1.y;
            foreach (var edge in edges) {
                draw.NoFill();
                draw.Stroke(Color.DodgerBlue);
                draw.Line((float) edge.Point1.X + offsetX, (float) edge.Point1.Y + offsetY, (float) edge.Point2.X + offsetX, (float) edge.Point2.Y + offsetY);
            }
        }

        private void DrawTriangulation(IEnumerable<Triangle> triangles, Quad quad = null) {
            var offsetX = quad == null ? 0f : (float) quad.P1.x;
            var offsetY = quad == null ? 0f : (float) quad.P1.y;
            foreach (var triangle in triangles) {
                var v0 = (Vec2) triangle.Vertices[0] + new Vec2(offsetX, offsetY);
                var v1 = (Vec2) triangle.Vertices[1] + new Vec2(offsetX, offsetY);
                var v2 = (Vec2) triangle.Vertices[2] + new Vec2(offsetX, offsetY);

                draw.Stroke(Color.Crimson);
                draw.NoFill();

                // draw.Fill(Color.Crimson, 127);
                draw.Triangle(v0.x, v0.y,
                    v1.x, v1.y,
                    v2.x, v2.y);

                if (drawProjection) {
                    draw.Stroke(Color.DarkOrange);
                    var projectedTriangle = MapTriangleOnQuad(v0,v1,v2, originalQuad, mappedQuad);
                    draw.Triangle((float) projectedTriangle.Vertices[0].X, (float) projectedTriangle.Vertices[0].Y,
                        (float) projectedTriangle.Vertices[1].X, (float) projectedTriangle.Vertices[1].Y ,
                        (float) projectedTriangle.Vertices[2].X, (float) projectedTriangle.Vertices[2].Y);
                }
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
            draw.Quad((float) quad.P1.x, (float) quad.P1.y, (float) quad.P2.x, (float) quad.P2.y, (float) quad.P3.x, (float) quad.P3.y, (float) quad.P4.x, (float) quad.P4.y);
            draw.Fill(stroke);
            draw.Ellipse((float) quad.P1.x, (float) quad.P1.y, 8, 8);
            draw.Ellipse((float) quad.P2.x, (float) quad.P2.y, 8, 8);
            draw.Ellipse((float) quad.P3.x, (float) quad.P3.y, 8, 8);
            draw.Ellipse((float) quad.P4.x, (float) quad.P4.y, 8, 8);

            // draw.Text("1", (float) quad.P1.x, (float) quad.P1.y);
            // draw.Text("2", (float) quad.P2.x, (float) quad.P2.y);
            // draw.Text("3", (float) quad.P3.x, (float) quad.P3.y);
            // draw.Text("4", (float) quad.P4.x, (float) quad.P4.y);
        }
        private Vec2 MapPointOnQuad(Vec2 point, Quad original, Quad target) {
            var p01 = new Vec2((float) original.P1.x, (float) original.P1.y);
            var p02 = new Vec2((float) original.P2.x, (float) original.P2.y);
            var p03 = new Vec2((float) original.P3.x, (float) original.P3.y);
            var p04 = new Vec2((float) original.P4.x, (float) original.P4.y);
            var a0 = p01;
            var b0 = p02 - p01;
            var c0 = p03 - p01;
            var d0 = p04 - p02 - p03 + p01;

            var p11 = new Vec2((float) target.P1.x, (float) target.P1.y);
            var p12 = new Vec2((float) target.P2.x, (float) target.P2.y);
            var p13 = new Vec2((float) target.P3.x, (float) target.P3.y);
            var p14 = new Vec2((float) target.P4.x, (float) target.P4.y);
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
            var delta = B * B - 4 * A * C;
            var sqrt_delta = Mathf.Sqrt(delta);
            var ee1 = (-B + sqrt_delta) / 2 * A;
            var ee2 = (-B - sqrt_delta) / 2 * A;

            float ee;
            if (ee1 >= 0f && ee1 <= 1f) ee = ee1;
            else ee = ee2;
            var nn = (a - b * ee) / c;

            var p1 = a1 + b1 * ee + c1 * nn + d1 * ee * nn;
            return p1;
        }
        private Vec2 MapPointOnQuad2(Vec2 point, Quad original, Quad target) {
            var v1 = (Vec2) original.P2 - (Vec2) original.P1;
            var v2 = (Vec2) target.P2 - (Vec2) target.P1;

            var dot = v1.Dot(v2);
            var det = v1.Det(v2);
            var angle = Mathf.Atan2(det, dot);
            var translatedPoint = point - original.P1;
            var rotatedPoint = translatedPoint;
            rotatedPoint.RotateRadians(angle);
            var translatedRotatedPoint = rotatedPoint + target.P1;
            return translatedRotatedPoint;
        }

        private Triangle MapTriangleOnQuad(Vec2 v0, Vec2 v1, Vec2 v2, Quad original, Quad target) {
            var p0 = MapPointOnQuad2(v0, original, target);
            var p1 = MapPointOnQuad2(v1, original, target);
            var p2 = MapPointOnQuad2(v2, original, target);
            
            return new Triangle(p0, p1, p2);
        }

        public static void Main() {
            new MainAppTesting().Start();
        }
    }
}