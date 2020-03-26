using System;
using System.Collections.Generic;
using System.Drawing;
using GXPEngine;
using physics_programming.assignment4.Components;

namespace physics_programming.assignment4 {
    public class MyGame : Game {
        private readonly Ball ball;
        private readonly EasyDraw text;
        private readonly NLineSegment lineSegment, lineSegment2;
        private readonly NLineSegment wallLeft, wallRight, wallTop, wallBottom;

        public List<NLineSegment> Lines;

        public MyGame() : base(800, 600, false, false) {
            ball = new Ball(30, new Vec2(width / 2, height / 2), new Vec2(-2, 1));
            AddChild(ball);

            text = new EasyDraw(250, 25);
            text.TextAlign(CenterMode.Min, CenterMode.Min);
            AddChild(text);

            lineSegment = new NLineSegment(new Vec2(500, 500), new Vec2(100, 200), 0xff00ff00, 3);
            lineSegment2 = new NLineSegment(new Vec2(100, 200), new Vec2(500, 500), 0xff00ff00, 3);
            wallLeft = new NLineSegment(new Vec2(0, height), new Vec2(0, 0), pLineWidth: 2);
            wallRight = new NLineSegment(new Vec2(width, 0), new Vec2(width, height), pLineWidth: 2);
            wallTop = new NLineSegment(new Vec2(0, 0), new Vec2(width, 0), pLineWidth: 2);
            wallBottom = new NLineSegment(new Vec2(width, height), new Vec2(0, height), pLineWidth: 2);

            AddChild(lineSegment);
            AddChild(lineSegment2);
            AddChild(wallLeft);
            AddChild(wallRight);
            AddChild(wallTop);
            AddChild(wallBottom);

            Lines = new List<NLineSegment> {lineSegment,lineSegment2, wallLeft, wallRight, wallTop, wallBottom};
            Restart();
            PrintInfo();
        }

        #region Debug
        private bool _paused;

        private bool _stepped;

        void PrintInfo() {
            Console.WriteLine("Hold spacebar to slow down the frame rate.");
            Console.WriteLine("Use arrow keys and backspace to set the gravity.");
            Console.WriteLine("Press S to toggle stepped mode.");
            Console.WriteLine("Press P to toggle pause.");
            Console.WriteLine("Press R to restart.");
        }

        private void HandleInput() {
            targetFps = Input.GetKey(Key.SPACE) ? 5 : 60;
            if (Input.GetKeyDown(Key.S)) {
                _stepped ^= true;
            }

            if (Input.GetKeyDown(Key.P)) {
                _paused ^= true;
            }

            // gravity:
            if (Input.GetKeyDown(Key.UP)) {
                Ball.acceleration = new Vec2(0, -1);
            }

            if (Input.GetKeyDown(Key.DOWN)) {
                Ball.acceleration = new Vec2(0, 1);
            }

            if (Input.GetKeyDown(Key.LEFT)) {
                Ball.acceleration = new Vec2(-1, 0);
            }

            if (Input.GetKeyDown(Key.RIGHT)) {
                Ball.acceleration = new Vec2(1, 0);
            }

            if (Input.GetKeyDown(Key.BACKSPACE)) {
                Ball.acceleration = new Vec2(0, 0);
            }

            if (Input.GetKeyDown(Key.R)) {
                Restart();
            }
        }

        private void Restart() {
            ball.Position = new Vec2(width / 2, height / 2);
            ball.Velocity = new Vec2(-2, 1);
            Ball.acceleration = Vec2.Zero;
        }
        #endregion

        private void Update() {
            HandleInput();
            // For now: this just puts the ball at the mouse position:
            ball.Step();
            foreach (var line in Lines) {
                var segmentVec = line.End - line.Start;
                var normalizedSegmentVec = segmentVec.normalized;
                var segmentLengthSqr = segmentVec.sqrMagnitude;
                var normal = segmentVec.Normal();
                
                // Start of line
                var ballDiff = ball.Position - line.Start;
                var ballDistance = Math.Abs(ballDiff.Dot(normal));
                var projectionLength = ballDiff.Dot(normalizedSegmentVec);
                var projectionVector = projectionLength * normalizedSegmentVec;
                var projectionVectorDot = projectionVector.Dot(normalizedSegmentVec);
                var projectionLengthSqr = projectionLength*projectionLength;
                
                // Checks
                var ballDistanceCheck = ballDistance < ball.Radius;
                var projectionLengthCheck = projectionLengthSqr < segmentLengthSqr;
                var projectionDirectionCheck = projectionVectorDot > 0;
                
                if (ballDistanceCheck &&  projectionLengthCheck && projectionDirectionCheck) {
                    ball.SetColor(1, 0, 0);
                    // Reset position
                    ball.Position += (-ballDistance + ball.Radius) * normal;
                    // Reflect
                    ball.Velocity.Reflect(normal, 0.5f);
                } else {
                    ball.SetColor(0, 1, 0);
                }
            }
        }
        private static void Main() {
            new MyGame().Start();
        }
    }
}