using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using GXPEngine;
using physics_programming.final_assignment.Components;

namespace physics_programming.final_assignment {
    public class MyGame : Game {
        private readonly List<Ball> movers;
        private readonly List<Bullet> bullets;
        private readonly List<LineSegment> lines;
        private bool paused;
        private bool stepped;

        private int stepIndex;
        private Player player;

        public MyGame() : base(Globals.WIDTH, Globals.HEIGHT, Globals.FULLSCREEN, Globals.VSYNC, pPixelArt: Globals.PIXEL_ART, windowTitle: Globals.WINDOW_TITLE) {
            targetFps = 60;

            movers = new List<Ball>();
            lines = new List<LineSegment>();
            bullets = new List<Bullet>();

            Restart();
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

        public void AddBullet(Bullet bullet) {
            bullets.Add(bullet);
            AddChild(bullet);
        }

        private void PrintInfo() {
            Console.WriteLine("Hold SPACE to slow down the frame rate.");
            Console.WriteLine("Use arrow keys and backspace to set the gravity.");
            Console.WriteLine("Press X to toggle stepped mode.");
            Console.WriteLine("Press P to toggle pause.");
            Console.WriteLine("Press R to reset scene");
        }

        private void HandleInput() {
            targetFps = Input.GetKey(Key.SPACE) ? 1 : 60;
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
            if (Input.GetKeyDown(Key.X))
                stepped ^= true;
            if (Input.GetKeyDown(Key.P))
                paused ^= true;
            if (Input.GetKeyDown(Key.R))
                Restart();
        }

        private void Restart() {
            player?.Destroy();
            foreach (var line in lines)
                line.Destroy();
            lines.Clear();
            foreach (var mover in movers)
                mover.Destroy();
            movers.Clear();

            foreach (var bullet in bullets)
                bullet.Destroy();
            bullets.Clear();

            player = new Player(500, 400, 3);
            AddChild(player);

            AddLine(new Vec2(0, height), new Vec2(0, 0));
            AddLine(new Vec2(width, 0), new Vec2(width, height));
            AddLine(new Vec2(0, 0), new Vec2(width, 0));
            AddLine(new Vec2(width, height), new Vec2(0, height));

            Ball.Acceleration.SetXY(0, 0);

            // movers.Add(new Ball(30, new Vec2(200, 300), new Vec2(0, 0)));
            foreach (var b in movers)
                AddChild(b);
        }

        private void StepThroughMovers() {
            if (stepped) {
                stepIndex++;
                if (stepIndex >= movers.Count)
                    stepIndex = 0;
                if (!movers[stepIndex].IsKinematic)
                    movers[stepIndex].Step();
            } else movers.Where(mover => !mover.IsKinematic).ToList().ForEach(mover => mover.Step());

            foreach (var bullet in bullets) bullet.Step();

            bullets.Where(b => b.Dead).ToList().ForEach(RemoveChild);
            bullets.RemoveAll(bullet => bullet.Dead);
        }

        [SuppressMessage("ReSharper", "UnusedMember.Local")]
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