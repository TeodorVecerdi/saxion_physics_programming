using System;
using System.Diagnostics.CodeAnalysis;
using GXPEngine;

namespace physics_programming.final_assignment {
    public class SmartEnemy : Enemy {
        public SmartEnemy(float px, float py, float accuracy = 1f) : base(accuracy) {
            var enemyTank = new Tank(px, py, TankMove, TankShoot, BarrelMove, 0xffffeabc);
            AddChild(enemyTank);
        }
        protected override void TankMove(Tank tank) {
            // Enemy tanks don't move
        }
        protected override void TankShoot(Tank tank) {
            if (timeLeftToShoot > 0f)
                return;

            var g = (MyGame) game;
            var accuracyOffset = Rand.Range(-AccuracyDegreeVariation + Accuracy * AccuracyDegreeVariation, AccuracyDegreeVariation - Accuracy * AccuracyDegreeVariation);
            var bulletRotation = accuracyOffset + tank.Barrel.rotation + tank.rotation;
            var bullet = new Bullet(tank.Position, Vec2.GetUnitVectorDeg(bulletRotation)) {rotation = bulletRotation};
            g.AddBullet(bullet);
            timeLeftToShoot = timeToShoot;
        }

        protected override void BarrelMove(Tank tank) {
            // Advanced aiming
            // vars
            var target = ((MyGame) game).Player.Tank;
            var targetVelocityWhenShot = target.Velocity + timeLeftToShoot * target.Acceleration;
            var positionDelta = target.Position - tank.Position;
            
            // quadratic equation
            var a = targetVelocityWhenShot.Dot(targetVelocityWhenShot) - Bullet.Speed * Bullet.Speed;
            var b = 2f * targetVelocityWhenShot.Dot(positionDelta);
            var c = positionDelta.Dot(positionDelta);
            var det = b * b - 4f * a * c;
            if(det <= 0f) return;
            var t = 2f * c / (Mathf.Sqrt(det) - b);
            if (t < 0f) return;
            
            // rotation
            var aimTarget = target.Position + t * targetVelocityWhenShot;
            var desiredRotation = -tank.rotation + Vec2.Rad2Deg((float) Math.Atan2(aimTarget.y - tank.Position.y, aimTarget.x - tank.Position.x));
            var delta = desiredRotation - tank.Barrel.rotation;
            var shortestAngle = Mathf.Clamp(delta - Mathf.Floor(delta / 360f) * 360f, 0.0f, 360f);
            if (shortestAngle > 180)
                shortestAngle -= 360;
            if (Math.Abs(tank.Barrel.rotation - desiredRotation) > 0.5)
                tank.Barrel.rotation += shortestAngle * 0.10f;
        }
    }
}