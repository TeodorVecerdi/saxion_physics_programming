using System;
using GXPEngine;
using physics_programming.assignment3.Components;

namespace physics_programming.assignment3 {
    public class Block : EasyDraw {
        public const float Gravity = 0.0981f;
        /******* PUBLIC FIELDS AND PROPERTIES *********************************************************/

        // These four public static fields are changed from MyGame, based on key input (see Console):
        public static bool DrawDebugLine = false;
        public static bool Wordy = false;
        public static float Bounciness = 0.98f;

        // For ease of testing / changing, we assume every block has the same acceleration (gravity):
        public static Vec2 Acceleration = new Vec2(0, 0);

        public readonly int Radius;

        public Vec2 Velocity;

        private const float colorFadeSpeed = 0.025f;

        private readonly Arrow velocityIndicator;
        private float blue = 1;

        private readonly float density = 1;
        private float green = 1;

        private float red = 1;

        private float smallestToi;
        private Vec2 oldPosition;

        /******* PRIVATE FIELDS *******************************************************************/

        private Vec2 _position;

        // Mass = density * volume.
        // In 2D, we assume volume = area (=all objects are assumed to have the same "depth")
        public float Mass => 4 * Radius * Radius * density;

        public Vec2 Position => _position;

        /******* PUBLIC METHODS *******************************************************************/

        public Block(int pRadius, Vec2 pPosition, Vec2 pVelocity) : base(pRadius * 2, pRadius * 2) {
            Radius = pRadius;
            _position = pPosition;
            Velocity = pVelocity;

            SetOrigin(Radius, Radius);
            Draw();
            UpdateScreenPosition();
            oldPosition = new Vec2(0, 0);

            velocityIndicator = new Arrow(_position, Velocity, 10);
            AddChild(velocityIndicator);
        }

        public void SetFadeColor(float pRed, float pGreen, float pBlue) {
            red = pRed;
            green = pGreen;
            blue = pBlue;
        }

        public void Update() {
            // For extra testing flexibility, we call the Step method from MyGame instead:
            //Step();
        }

        public void Step() {
            oldPosition = _position;

            // No need to make changes in this Step method (most of it is related to drawing, color and debug info). 
            // Work in Move instead.
            Move();

            UpdateColor();
            UpdateScreenPosition();
            ShowDebugInfo();
        }

        /******* PRIVATE METHODS *******************************************************************/

        /******* THIS IS WHERE YOU SHOULD WORK: ***************************************************/

        private void Move() {
            // TODO: implement Assignment 3 here (and in methods called from here).
            Velocity += Acceleration * Gravity;
            _position += Velocity;

            // Example methods (replace/extend):
            CheckBoundaryCollisions();
            CheckBlockOverlaps();

            // TIP: You can use the CollisionInfo class to pass information between methods, e.g.:
            //
            //Collision firstCollision=FindEarliestCollision();
            //if (firstCollision!=null)
            //	ResolveCollision(firstCollision);
        }

        // This method is just an example of how to check boundaries, and change color.
        private void CheckBoundaryCollisions() {
            var myGame = (MyGame) game;
            if (_position.x - Radius < myGame.LeftXBoundary) {
                // move block from left to right boundary:
                //_position.x += myGame.RightXBoundary - myGame.LeftXBoundary - 2 * radius;
                //_position.x = myGame.LeftXBoundary + 2 * radius;		//assignment 1
                PointOfImpactX(myGame.LeftXBoundary + Radius);
                Velocity = -Bounciness * Velocity;
                SetFadeColor(1, 0.2f, 0.2f);
                if (Wordy)
                    Console.WriteLine("Left boundary collision");
            } else if (_position.x + Radius > myGame.RightXBoundary) {
                // move block from right to left boundary:

                //_position.x -= myGame.RightXBoundary - myGame.LeftXBoundary - 2 * radius;
                //_position.x = myGame.RightXBoundary - 2*radius;		//assignment 1
                PointOfImpactX(myGame.RightXBoundary - Radius);
                Velocity = -Bounciness * Velocity;
                SetFadeColor(1, 0.2f, 0.2f);
                if (Wordy)
                    Console.WriteLine("Right boundary collision");
            }

            if (_position.y - Radius < myGame.TopYBoundary) {
                // move block from top to bottom boundary:
                //_position.y += myGame.BottomYBoundary - myGame.TopYBoundary - 2 * radius;
                //_position.y = myGame.TopYBoundary + 2 * radius;		//assignment 1
                PointOfImpactY(myGame.TopYBoundary + Radius);
                Velocity.y = -Bounciness * Velocity.y;
                SetFadeColor(0.2f, 1, 0.2f);
                if (Wordy)
                    Console.WriteLine("Top boundary collision");
            } else if (_position.y + Radius > myGame.BottomYBoundary) {
                // move block from bottom to top boundary:
                //_position.y -= myGame.BottomYBoundary - myGame.TopYBoundary - 2 * radius;
                //_position.y = myGame.BottomYBoundary - 2 * radius;		//assignment 1
                PointOfImpactY(myGame.BottomYBoundary - Radius);
                Velocity.y = -Bounciness * Velocity.y;
                SetFadeColor(0.2f, 1, 0.2f);
                if (Wordy)
                    Console.WriteLine("Bottom boundary collision");
            }
        }

        private void PointOfImpactX(float newCoordonate) {
            var impactX = newCoordonate;
            var a = Math.Abs(impactX - oldPosition.x);
            var b = Math.Abs(_position.x - oldPosition.x);
            var t = a / b;

            var point = oldPosition + t * Velocity;
            _position = point;
        }

        private void PointOfImpactY(float newCoordonate) {
            var impactY = newCoordonate;
            var a = Math.Abs(impactY - oldPosition.y);
            var b = Math.Abs(_position.y - oldPosition.y);
            var t = a / b;

            var point = oldPosition + t * Velocity;
            _position = point;
        }

        private float TimeOfImpactX(float newCoordonate) {
            var impactX = newCoordonate;
            var a = impactX - oldPosition.x;
            var b = _position.x - oldPosition.x;
            var t = a / b;

            return t;
        }

        private float TimeOfImpactY(float newCoordonate) {
            var impactY = newCoordonate;
            var a = impactY - oldPosition.y;
            var b = _position.y - oldPosition.y;
            var t = a / b;

            return t;
        }

        // This method is just an example of how to get information about other blocks in the scene.
        private void CheckBlockOverlaps() {
            var currentBlock = GetEarliestCollision();
            if (currentBlock != null) ResetPosition(currentBlock);
        }

        private Block GetEarliestCollision() {
            var myGame = (MyGame) game;
            smallestToi = 2f;

            Block currentBlock = null;
            for (var i = 0; i < myGame.GetNumberOfMovers(); i++) {
                var other = myGame.GetMover(i);
                if (other != this) // TODO: improve hit test, move to method:
                    if (IsOverlapping(other)) {
                        var toi = ClaculateToi(other);
                        currentBlock = SetSmallestToi(currentBlock, other, toi);
                    }
            }

            return currentBlock;
        }

        private Block SetSmallestToi(Block currentBlock, Block other, float toi) {
            if (toi < smallestToi) {
                smallestToi = toi;
                currentBlock = other;
            }

            return currentBlock;
        }

        private float ClaculateToi(Block other) {
            var toiX = 2f;
            var toiY = 2f;
            if (Velocity.x > 0)
                toiX = TimeOfImpactX(other.Position.x - other.width / 2 - width / 2);
            else
                toiX = TimeOfImpactX(other.Position.x + other.width / 2 + width / 2);

            if (Velocity.y > 0)
                toiY = TimeOfImpactY(other.Position.y - other.height / 2 - height / 2);
            else
                toiY = TimeOfImpactY(other.Position.y + other.height / 2 + height / 2);

            if (Velocity.x == 0f)
                toiX = toiY;

            if (Velocity.y == 0f)
                toiY = toiX;

            return Math.Max(toiX, toiY);
        }

        private bool IsOverlapping(Block other) {
            return _position.x + width / 2 >= other.Position.x - other.width / 2 &&
                   _position.x - width / 2 <= other.Position.x + other.width / 2 &&
                   _position.y - height / 2 <= other.Position.y + other.height / 2 &&
                   _position.y + height / 2 >= other.Position.y - other.height / 2;
        }

        private void ResetPosition(Block other) {
            if (IsNotMovingInTheSameDirection(other))
                ChengeVelocityAndSetPosition(other);

            SetFadeColor(0.2f, 0.2f, 1);
            other.SetFadeColor(0.2f, 0.2f, 1);
            if (Wordy)
                Console.WriteLine("Block-block overlap detected.");
        }

        private void ChengeVelocityAndSetPosition(Block other) {
            var u = (Mass * Velocity + other.Mass * other.Velocity) * (1 / (Mass + other.Mass));
            Velocity = u - Bounciness * (Velocity - u);
            other.Velocity = u - Bounciness * (other.Velocity - u);
            _position = oldPosition + smallestToi * Velocity;
        }

        private bool IsNotMovingInTheSameDirection(Block other) {
            return !(Velocity.x > 0 && other.Velocity.x > 0 && Velocity.x < other.Velocity.x ||
                     Velocity.x < 0 && other.Velocity.x < 0 && Velocity.x > other.Velocity.x) &&
                   !(Velocity.y > 0 && other.Velocity.y > 0 && Velocity.y < other.Velocity.y ||
                     Velocity.y < 0 && other.Velocity.y < 0 && Velocity.y > other.Velocity.y);
        }

        /******* NO NEED TO CHANGE ANY OF THE CODE BELOW: **********************************************/

        private void UpdateColor() {
            if (red < 1)
                red = Mathf.Min(1, red + colorFadeSpeed);

            if (green < 1)
                green = Mathf.Min(1, green + colorFadeSpeed);

            if (blue < 1)
                blue = Mathf.Min(1, blue + colorFadeSpeed);

            SetColor(red, green, blue);
        }

        private void ShowDebugInfo() {
            if (DrawDebugLine)
                ((MyGame) game).DrawLine(oldPosition, _position);

            velocityIndicator.startPoint = _position;
            velocityIndicator.vector = Velocity;
        }

        private void UpdateScreenPosition() {
            x = _position.x;
            y = _position.y;
        }

        private void Draw() {
            Fill(200);
            NoStroke();
            ShapeAlign(CenterMode.Min, CenterMode.Min);
            Rect(0, 0, width, height);
        }
    }
}