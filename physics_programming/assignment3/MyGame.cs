using System;
using GXPEngine;
using System.Drawing;
using System.Collections.Generic;
using physics_programming;
using physics_programming.assignment4.Components;

namespace physics_programming.assignment3 {
    public class MyGame : Game {
        bool _stepped = false;
        bool _paused = false;
        int _stepIndex = 0;
        int _startSceneNumber = 0;

        float _leftXBoundary = 0;
        float _rightXBoundary = 0;
        float _topYBoundary = 0;
        float _bottomYBoundary = 0;

        Canvas _lineContainer = null;

        List<Block> _movers;

        public float LeftXBoundary {
            get {
                return _leftXBoundary;
            }
        }
        public float RightXBoundary {
            get {
                return _rightXBoundary;
            }
        }
        public float TopYBoundary {
            get {
                return _topYBoundary;
            }
        }
        public float BottomYBoundary {
            get {
                return _bottomYBoundary;
            }
        }

        public int GetNumberOfMovers() {
            return _movers.Count;
        }

        public Block GetMover(int index) {
            if (index >= 0 && index < _movers.Count) {
                return _movers[index];
            }

            return null;
        }

        public void DrawLine(Vec2 start, Vec2 end) {
            _lineContainer.graphics.DrawLine(Pens.White, start.x, start.y, end.x, end.y);
        }

        public MyGame() : base(800, 600, false, false) {
            _lineContainer = new Canvas(width, height);
            AddChild(_lineContainer);

            targetFps = 60;

            float border = 50;

            _leftXBoundary = border;
            _rightXBoundary = width - border;
            _topYBoundary = border;
            _bottomYBoundary = height - border;

            createVisualXBoundary(_leftXBoundary);
            createVisualXBoundary(_rightXBoundary);
            createVisualYBoundary(_topYBoundary);
            createVisualYBoundary(_bottomYBoundary);

            _movers = new List<Block>();

            LoadScene(_startSceneNumber);

            PrintInfo();
        }

        void LoadScene(int sceneNumber) {
            _startSceneNumber = sceneNumber;

            // remove previous scene:
            foreach (Block mover in _movers) {
                mover.Destroy();
            }

            _movers.Clear();

            // create new scene:
            switch (sceneNumber) {
                case 1: // different sizes, gravity
                    Block.Acceleration = new Vec2(0, 1);

                    _movers.Add(new Block(30, new Vec2(200, 300), new Vec2(1, 0)));
                    _movers.Add(new Block(50, new Vec2(600, 300), new Vec2(-1, 0)));

                    // _movers.Add(new Block(40, new Vec2(400, 300), new Vec2(3, -8)));
                    // _movers.Add(new Block(15, new Vec2(400, 200), new Vec2(-3, -8)));
                    // _movers.Add(new Block(20, new Vec2(400, 400), new Vec2(3, 3)));
                    break;
                case 2: // stack
                    Block.Acceleration = new Vec2(0, 0);

                    _movers.Add(new Block(30, new Vec2(100, 520), new Vec2(7, 0)));
                    _movers.Add(new Block(50, new Vec2(200, 500), new Vec2(7, 0)));
                    _movers.Add(new Block(40, new Vec2(300, 510), new Vec2(7, 0)));
                    _movers.Add(new Block(15, new Vec2(400, 535), new Vec2(7, 0)));
                    _movers.Add(new Block(20, new Vec2(500, 530), new Vec2(7, 0)));
                    break;
                case 3: // Newton's cradle
                    Block.Acceleration = new Vec2(0, 0);
                    _movers.Add(new Block(25, new Vec2(50, 500), new Vec2(20, 0)));
                    _movers.Add(new Block(25, new Vec2(400, 500), new Vec2(0, 0)));
                    _movers.Add(new Block(25, new Vec2(450, 500), new Vec2(0, 0)));
                    _movers.Add(new Block(25, new Vec2(500, 500), new Vec2(0, 0)));
                    _movers.Add(new Block(25, new Vec2(550, 500), new Vec2(0, 0)));
                    break;
                case 4: // Tunneling
                    Block.Acceleration = new Vec2(0, 0);
                    _movers.Add(new Block(20, new Vec2(145, 510), new Vec2(85, 0)));
                    _movers.Add(new Block(20, new Vec2(472, 500), new Vec2(-10, 0)));
                    break;
                case 5: // Diagonal impact test
                    Block.Acceleration = new Vec2(0, 0);
                    _movers.Add(new Block(25, new Vec2(199, 71), new Vec2(30, 30)));
                    _movers.Add(new Block(25, new Vec2(400, 300), new Vec2(0, 0)));
                    break;
                case 6: // basic, one block launched into corner, fast
                    Block.Acceleration = new Vec2(0, 0);
                    _movers.Add(new Block(30, new Vec2(400, 300), new Vec2(60, 43)));
                    break;
                default: // basic, one block launched into corner, slow
                    Block.Acceleration = new Vec2(0, 0);
                    _movers.Add(new Block(30, new Vec2(400, 300), new Vec2(6, 4)));
                    break;
            }

            _stepIndex = -1;
            foreach (Block b in _movers) {
                AddChild(b);
            }
        }

        /****************************************************************************************/

        void createVisualXBoundary(float xBoundary) {
            AddChild(new LineSegment(xBoundary, 0, xBoundary, height, 0xffffffff, 1));
        }

        void createVisualYBoundary(float yBoundary) {
            AddChild(new LineSegment(0, yBoundary, width, yBoundary, 0xffffffff, 1));
        }

        void PrintInfo() {
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

        void PrintSettings() {
            Console.WriteLine("Extra text output: " + Block.Wordy);
            Console.WriteLine("Bounciness: " + Block.Bounciness);
        }

        void HandleInput() {
            // step mode and speed:
            targetFps = Input.GetKey(Key.SPACE) ? 5 : 60;
            if (Input.GetKeyDown(Key.S)) {
                _stepped ^= true;
            }

            if (Input.GetKeyDown(Key.P)) {
                _paused ^= true;
            }

            // gravity:
            if (Input.GetKeyDown(Key.UP)) {
                Block.Acceleration = new Vec2(0, -1);
            }

            if (Input.GetKeyDown(Key.DOWN)) {
                Block.Acceleration = new Vec2(0, 1);
            }

            if (Input.GetKeyDown(Key.LEFT)) {
                Block.Acceleration = new Vec2(-1, 0);
            }

            if (Input.GetKeyDown(Key.RIGHT)) {
                Block.Acceleration = new Vec2(1, 0);
            }

            if (Input.GetKeyDown(Key.BACKSPACE)) {
                Block.Acceleration = new Vec2(0, 0);
            }

            // Debug lines:
            if (Input.GetKeyDown(Key.D)) {
                Block.DrawDebugLine ^= true;
            }

            if (Input.GetKeyDown(Key.C)) {
                _lineContainer.graphics.Clear(Color.Black);
            }

            // other options:
            if (Input.GetKeyDown(Key.B)) {
                Block.Bounciness = 1.5f - Block.Bounciness;
                PrintSettings();
            }

            if (Input.GetKeyDown(Key.W)) {
                Block.Wordy ^= true;
                PrintSettings();
            }

            // Load/reset scenes:
            if (Input.GetKeyDown(Key.R)) {
                LoadScene(_startSceneNumber);
            }

            for (int i = 0; i < 10; i++) {
                if (Input.GetKeyDown(48 + i)) {
                    LoadScene(i);
                }
            }
        }

        void StepThroughMovers() {
            if (_stepped) {
                // move everything step-by-step: in one frame, only one mover moves
                _stepIndex++;
                if (_stepIndex >= _movers.Count) {
                    _stepIndex = 0;
                }

                _movers[_stepIndex].Step();
            } else {
                // move all movers every frame
                foreach (Block mover in _movers) {
                    mover.Step();
                }
            }
        }

        void Update() {
            HandleInput();
            if (!_paused) {
                StepThroughMovers();
            }
        }

        static void Main3() {
            new MyGame().Start();
        }
    }
}