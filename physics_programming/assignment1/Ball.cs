using System;
using System.Threading;
using GXPEngine;

namespace physics_programming.assignment1 {
    // Q: What happens if you use different percentages?
    // A: By decreasing the first percentage and increasing the second percentage the ball "loses" inertia faster.
    //     The opposite happens if you increase the first percentage and decrease the second percentage
    // Q: What happens if you use percentages that don’t add up to 100%?
    // A: I couldn't see something special happening. You just decrease one of the ratios.
    // Q: What happens if one of the percentages is negative?
    // A: If the first percentage is negative, the ball increases in speed much slower.
    //     If the second percentage is negative, the ball moves away from the mouse instead of towards

    public class Ball : EasyDraw {
        public Vec2 position => _position;
        public Vec2 velocity;
        public bool IsDone;

        private int _radius;
        private Vec2 _position;
        private float _speed;
        private float lerpPrecentage;
        private float lerpPrecentage2;

        public Ball(int pRadius, Vec2 pPosition, float pSpeed = 0, float t = 0.99f, float t2 = 0.1f) : base(pRadius * 2 + 1, pRadius * 2 + 1) {
            _radius = pRadius;
            _position = pPosition;
            _speed = pSpeed;
            lerpPrecentage = t;
            lerpPrecentage2 = t2;

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
            var newVelocity = direction * _speed;
            var desiredVelocity = newVelocity + lerpPrecentage * (velocity - newVelocity); 
            // var desiredVelocity = lerpPrecentage * velocity + (1 - lerpPrecentage) * newVelocity;
            velocity = desiredVelocity;
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