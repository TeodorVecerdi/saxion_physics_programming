using System;
using System.Threading;
using GXPEngine;

namespace physics_programming.assignment1 {
    public class BallA13 : EasyDraw {
        public Vec2 position => _position;
        public Vec2 velocity;
        public bool IsDone;

        private int _radius;
        private Vec2 _position;
        private float _speed;

        public BallA13(int pRadius, Vec2 pPosition, float pSpeed = 0) : base(pRadius * 2 + 1, pRadius * 2 + 1) {
            _radius = pRadius;
            _position = pPosition;
            _speed = pSpeed;

            UpdateScreenPosition();
            SetOrigin(_radius, _radius);

            Draw(150, 0, 255);
        }

        private void Draw(byte red, byte green, byte blue) {
            Fill(red, green, blue);
            Stroke(red, green, blue);
            Ellipse(_radius, _radius, 2 * _radius, 2 * _radius);
        }

        private void UpdateScreenPosition() {
            x = _position.x;
            y = _position.y;
        }

        private void CalculateVelocity() {
            var mousePosition = new Vec2(Input.mouseX, Input.mouseY);
            var direction = (mousePosition - _position).normalized;
            velocity = direction * _speed;
        }

        public void Step() {
            if(IsDone) return;
            CalculateVelocity();
            _position += velocity;

            if (OverlapsMouse(new Vec2(Input.mouseX, Input.mouseY))) {
                IsDone = true;
                return;
            }
            
            _speed += Time.deltaTime;

            UpdateScreenPosition();
        }

        private bool OverlapsMouse(Vec2 mousePosition) {
            var distSqr = (mousePosition - position).sqrMagnitude;
            return distSqr <= _radius * _radius;
        }
    }
}