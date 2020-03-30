using System;
using System.Collections.Generic;
using System.Drawing;
using GXPEngine;
using physics_programming.assignment5.Components;

namespace physics_programming.assignment5 {
    public class MyGame : Game {
        private bool paused;
        private bool stepped;

        private readonly Canvas lineContainer;
        private int startSceneNumber;
        private int stepIndex;

        private readonly List<Ball> movers;
        private readonly List<LineSegment> lines;

        public MyGame() : base(800, 600, false, false) {
            lineContainer = new Canvas(width, height);
            AddChild(lineContainer);

            targetFps = 60;

            movers = new List<Ball>();
            lines = new List<LineSegment>();

            LoadScene(startSceneNumber);

            PrintInfo();
        }

        public int GetNumberOfLines() {
            return lines.Count;
        }

        public LineSegment GetLine(int index) {
            if (index >= 0 && index < lines.Count)
                return lines[index];
            return null;
        }

        public int GetNumberOfMovers() {
            return movers.Count;
        }

        public Ball GetMover(int index) {
            if (index >= 0 && index < movers.Count)
                return movers[index];
            return null;
        }

        public void DrawLine(Vec2 start, Vec2 end) {
            lineContainer.graphics.DrawLine(Pens.White, start.x, start.y, end.x, end.y);
        }

        private void AddLine(Vec2 start, Vec2 end, bool addReverseLine = false, bool addLineEndings = true) {
            var line = new LineSegment(start, end, 0xff00ff00, 4);
            AddChild(line);
            lines.Add(line);
            if (addReverseLine) {
                var reverseLine = new LineSegment(end, start, 0xff00ff00, 4);
                AddChild(reverseLine);
                lines.Add(reverseLine);
            }

            if (addLineEndings) {
                movers.Add(new Ball(0, start, isKinematic: true));
                movers.Add(new Ball(0, end, isKinematic: true));
            }
        }

        private void LoadScene(int sceneNumber) {
            startSceneNumber = sceneNumber;

            // remove previous scene:
            foreach (var mover in movers)
                mover.Destroy();
            movers.Clear();
            foreach (var line in lines)
                line.Destroy();
            lines.Clear();

            // boundary:
            AddLine(new Vec2(width - 60, height - 60), new Vec2(50, height - 20)); //bottom
            AddLine(new Vec2(50, height - 20), new Vec2(200, 60));
            AddLine(new Vec2(200, 60), new Vec2(width - 20, 50));
            AddLine(new Vec2(width - 20, 50), new Vec2(width - 60, height - 60)); //right

            switch (sceneNumber) {
                // BALL / BALL COLLISION SCENES:
                case 1: // one moving ball (medium speed), one fixed ball.
                    Ball.Acceleration.SetXY(0, 0);
                    movers.Add(new Ball(30, new Vec2(200, 300), new Vec2(38, 0)));
                    movers.Add(new Ball(30, new Vec2(400, 340)));
                    break;
                case 2: // one moving ball (high speed), one fixed ball.
                    Ball.Acceleration.SetXY(0, 0);
                    movers.Add(new Ball(30, new Vec2(200, 300), new Vec2(72, 0)));
                    movers.Add(new Ball(30, new Vec2(400, 340)));
                    break;
                case 3: // many balls:
                    Ball.Acceleration.SetXY(0, 0);

                    movers.Add(new Ball(30, new Vec2(200, 300), new Vec2(3, 4)));
                    movers.Add(new Ball(50, new Vec2(600, 300), new Vec2(5, 4)));
                    movers.Add(new Ball(40, new Vec2(400, 300), new Vec2(-3, 4)));
                    movers.Add(new Ball(15, new Vec2(500, 200), new Vec2(7, 4)));
                    movers.Add(new Ball(20, new Vec2(300, 400), new Vec2(-3, 4)));
                    movers.Add(new Ball(30, new Vec2(200, 200), new Vec2(3, 4)));
                    movers.Add(new Ball(50, new Vec2(600, 200), new Vec2(5, 4)));
                    movers.Add(new Ball(40, new Vec2(300, 200), new Vec2(-3, 4)));
                    movers.Add(new Ball(15, new Vec2(400, 100), new Vec2(7, 4)));
                    movers.Add(new Ball(20, new Vec2(500, 300), new Vec2(-3, 4)));
                    break;
                case 4: // one moving ball bouncing on some fixed balls:
                    Ball.Acceleration.SetXY(0, 1);
                    movers.Add(new Ball(30, new Vec2(200, 470), isKinematic: true));
                    movers.Add(new Ball(30, new Vec2(260, 500), isKinematic: true));
                    movers.Add(new Ball(30, new Vec2(320, 500), isKinematic: true));
                    movers.Add(new Ball(30, new Vec2(380, 470), isKinematic: true));
                    movers.Add(new Ball(30, new Vec2(400, 302), new Vec2(0, 0)));
                    break;

                // LINE SEGMENT SCENES:
                case 5: // line segment:
                    Ball.Acceleration.SetXY(0, 0);
                    movers.Add(new Ball(30, new Vec2(200, 300), new Vec2(5, 0)));
                    AddLine(new Vec2(290, 250), new Vec2(455, 350), true);
                    break;
                case 6: // polygon:
                    Ball.Acceleration.SetXY(0, 1);
                    movers.Add(new Ball(30, new Vec2(400, 180), new Vec2(0, 0)));
                    AddLine(new Vec2(290, 250), new Vec2(455, 350));
                    AddLine(new Vec2(455, 350), new Vec2(600, 250));
                    AddLine(new Vec2(600, 250), new Vec2(450, 300));
                    AddLine(new Vec2(450, 300), new Vec2(290, 250));
                    break;
                default: // one moving ball (low speed), one fixed ball.
                    Ball.Acceleration.SetXY(0, 0);
                    movers.Add(new Ball(30, new Vec2(200, 300), new Vec2(10, 0)));
                    movers.Add(new Ball(30, new Vec2(400, 340)));
                    break;
            }

            stepIndex = -1;
            foreach (var b in movers)
                AddChild(b);
        }

        /****************************************************************************************/

        private void PrintInfo() {
            Console.WriteLine("Hold spacebar to slow down the frame rate.");
            Console.WriteLine("Use arrow keys and backspace to set the gravity.");
            Console.WriteLine("Press S to toggle stepped mode.");
            Console.WriteLine("Press P to toggle pause.");
            Console.WriteLine("Press D to draw debug lines.");
            Console.WriteLine("Press C to clear all debug lines.");
            Console.WriteLine("Press R to reset scene, and numbers to load different scenes.");
            Console.WriteLine("Press B to toggle high/low bounciness.");
            Console.WriteLine("Press W to toggle extra output text.");
        }

        private void HandleInput() {
            targetFps = Input.GetKey(Key.SPACE) ? 5 : 60;
            if (Input.GetKeyDown(Key.UP))
                Ball.Acceleration.SetXY(0, -1);
            if (Input.GetKeyDown(Key.DOWN))
                Ball.Acceleration.SetXY(0, 1);
            if (Input.GetKeyDown(Key.LEFT))
                Ball.Acceleration.SetXY(-1, 0);
            if (Input.GetKeyDown(Key.RIGHT))
                Ball.Acceleration.SetXY(1, 0);
            if (Input.GetKeyDown(Key.BACKSPACE))
                Ball.Acceleration.SetXY(0, 0);
            if (Input.GetKeyDown(Key.S))
                stepped ^= true;
            if (Input.GetKeyDown(Key.D))
                Ball.DrawDebugLine ^= true;
            if (Input.GetKeyDown(Key.P))
                paused ^= true;
            if (Input.GetKeyDown(Key.B))
                Ball.Bounciness = 1.5f - Ball.Bounciness;
            if (Input.GetKeyDown(Key.W))
                Ball.Wordy ^= true;
            if (Input.GetKeyDown(Key.C))
                lineContainer.graphics.Clear(Color.Black);
            if (Input.GetKeyDown(Key.R))
                LoadScene(startSceneNumber);
            for (var i = 0; i < 10; i++)
                if (Input.GetKeyDown(48 + i))
                    LoadScene(i);
        }

        private void StepThroughMovers() {
            if (stepped) {
                // move everything step-by-step: in one frame, only one mover moves
                stepIndex++;
                if (stepIndex >= movers.Count)
                    stepIndex = 0;
                if (!movers[stepIndex].IsKinematic)
                    movers[stepIndex].Step();
            } else // move all movers every frame
                foreach (var mover in movers)
                    if (!mover.IsKinematic)
                        mover.Step();
        }

        private void Update() {
            HandleInput();
            if (!paused)
                StepThroughMovers();
        }

        private static void Main() {
            new MyGame().Start();
        }
    }
}