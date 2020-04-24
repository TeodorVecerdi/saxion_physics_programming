using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using GXPEngine;
using physics_programming.final_assignment.Components;

namespace physics_programming.final_assignment {
    public class MyGame : Game {
        public readonly List<Ball> Movers;
        public readonly List<Bullet> Bullets;
        public readonly List<DestructibleBlock> DestructibleBlocks;
        public readonly List<DestructibleChunk> DestructibleChunks;
        public readonly List<DoubleDestructibleLineSegment> DestructibleLines;
        public readonly List<LineSegment> Lines;
        public readonly List<TankAIBase> Enemies;
        public Player Player;
        public Scene currentScene = null;
        public Scene newScene = null;
        
        private Scene testScene = new TestScene();

        private bool paused;
        private bool stepped;

        private int stepIndex;

        public MyGame() : base(Globals.WIDTH, Globals.HEIGHT, Globals.FULLSCREEN, Globals.VSYNC, pPixelArt: Globals.PIXEL_ART, windowTitle: Globals.WINDOW_TITLE) {
            targetFps = 60;

            Movers = new List<Ball>();
            Lines = new List<LineSegment>();
            Bullets = new List<Bullet>();
            DestructibleLines = new List<DoubleDestructibleLineSegment>();
            DestructibleBlocks = new List<DestructibleBlock>();
            DestructibleChunks = new List<DestructibleChunk>();
            Enemies = new List<TankAIBase>();

            newScene = testScene;
            Restart();
            PrintInfo();
        }

        public void AddLine(Vec2 start, Vec2 end, bool addReverseLine = false, bool addLineEndings = true, uint color = 0xffffffff) {
            var line = new LineSegment(start, end, color, 4);
            AddChild(line);
            Lines.Add(line);
            if (addReverseLine) {
                var reverseLine = new LineSegment(end, start, color, 4);
                AddChild(reverseLine);
                Lines.Add(reverseLine);
            }

            if (addLineEndings) {
                Movers.Add(new Ball(0, start, isKinematic: true));
                Movers.Add(new Ball(0, end, isKinematic: true));
            }
        }

        public void AddDestructibleLine(Vec2 start, Vec2 end, bool addLineEndings = true, uint color = 0xff00ff00) {
            var line = new DoubleDestructibleLineSegment(start, end, color, 2);
            AddChild(line);
            DestructibleLines.Add(line);

            if (addLineEndings) {
                Movers.Add(new Ball(0, start, isKinematic: true));
                Movers.Add(new Ball(0, end, isKinematic: true));
            }
        }

        public void AddBullet(Bullet bullet) {
            Bullets.Add(bullet);
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

            if (Input.GetKeyDown(Key.T))
                foreach (var line in DestructibleLines)
                    Console.WriteLine($"Line Size: {(line.SideA.End - line.SideA.Start).magnitude}");
        }

        private void Restart() {
            Player?.Destroy();
            foreach (var enemy in Enemies)
                enemy.Destroy();
            Enemies.Clear();
            foreach (var line in Lines)
                line.Destroy();
            Lines.Clear();
            foreach (var mover in Movers)
                mover.Destroy();
            Movers.Clear();
            foreach (var bullet in Bullets)
                bullet.Destroy();
            Bullets.Clear();
            foreach (var destructibleLine in DestructibleLines)
                destructibleLine.Destroy();
            DestructibleLines.Clear();
            foreach (var destructibleBlock in DestructibleBlocks)
                destructibleBlock.Destroy();
            DestructibleBlocks.Clear();
            foreach (var destructibleChunk in DestructibleChunks)
                destructibleChunk.Destroy();
            DestructibleChunks.Clear();
            
            currentScene?.Finalise(this);
            currentScene = newScene;
            currentScene?.Initialise(this);
            // movers.Add(new Ball(30, new Vec2(200, 300), new Vec2(0, 0)));
            foreach (var b in Movers)
                AddChild(b);
            foreach (var enemy in Enemies)
                AddChild(enemy);
        }

        private void StepThroughMovers() {
            if (stepped) {
                stepIndex++;
                if (stepIndex >= Movers.Count)
                    stepIndex = 0;
                if (!Movers[stepIndex].IsKinematic)
                    Movers[stepIndex].Step();
            } else Movers.Where(mover => !mover.IsKinematic).ToList().ForEach(mover => mover.Step());

            UpdateBullets();
            UpdateDestructibleEnvironment();
        }

        private void UpdateBullets() {
            foreach (var bullet in Bullets) bullet.Step();
            Bullets.Where(b => b.Dead).ToList().ForEach(bullet => {
                bullet.Destroy();
                Bullets.Remove(bullet);
            });
        }

        private void UpdateDestructibleEnvironment() {
            var minSizeSqr = Globals.World.DestructibleLineMinLength * 1.5f;
            minSizeSqr *= minSizeSqr;

            //// LINES
            DestructibleLines.Where(l => l.ShouldRemove || (l.SideA.End - l.SideA.Start).sqrMagnitude <= minSizeSqr)
                .ToList().ForEach(l => {
                    l.Destroy();
                    DestructibleLines.Remove(l);
                });

            //// CHUNKS
            DestructibleChunks.Where(chunk => chunk.ShouldRemove)
                .ToList().ForEach(chunk => {
                    chunk.Destroy();
                    DestructibleChunks.Remove(chunk);
                });

            //// BLOCKS
            DestructibleBlocks.Where(block => block.ShouldRemove || (block.Length1.End - block.Length1.Start).sqrMagnitude <= minSizeSqr || (block.Length2.End - block.Length2.Start).sqrMagnitude <= minSizeSqr)
                .ToList().ForEach(block => {
                    block.Destroy();
                    DestructibleBlocks.Remove(block);
                });
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