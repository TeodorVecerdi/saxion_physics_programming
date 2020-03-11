using GXPEngine;

namespace physics_programming.assignment2 {
    public class Bullet : Sprite {
        // public fields & properties:
        public Vec2 position => _position;
        public Vec2 velocity;

        // private fields:
        Vec2 _position;

        public Bullet(Vec2 pPosition, Vec2 pVelocity) : base("data/assets/bullet.png") {
            _position = pPosition;
            velocity = pVelocity;
        }

        void UpdateScreenPosition() {
            x = _position.x;
            y = _position.y;
        }

        public void Update() {
            _position += velocity;
            UpdateScreenPosition();
        }
    }
}