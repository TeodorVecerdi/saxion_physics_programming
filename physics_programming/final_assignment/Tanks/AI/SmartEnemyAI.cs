using System;
using System.Drawing;
using GXPEngine;

namespace physics_programming.final_assignment {
    public class SmartEnemyAI : TankAIBase {
        private AttackIndicator attackIndicator;
        public SmartEnemyAI(float px, float py, float accuracy = 1f) : base(accuracy) {
            Tank = new Tank(px, py, TankMove, TankShoot, BarrelMove, 0xffffeabc);
            AddChild(Tank);
            attackIndicator = new AttackIndicator(TimeToShoot, radius: 100, color: Color.FromArgb(40, 255, 255, 255 ), initialArcAngle: 180);
            attackIndicator.SetXY(Tank.Barrel.width/3f, 0);
            Tank.Barrel.AddChild(attackIndicator);
        }

        protected override void TankMove(Tank tank) {
            // Enemy tanks don't move
        }

        protected override void TankShoot(Tank tank) {
            if (TimeLeftToShoot > 0f)
                return;

            var g = (MyGame) game;
            var accuracyOffset = Rand.Range(-AccuracyDegreeVariation + Accuracy * AccuracyDegreeVariation, AccuracyDegreeVariation - Accuracy * AccuracyDegreeVariation);
            var bulletRotation = accuracyOffset + tank.Barrel.rotation + tank.rotation;
            var bullet = new Bullet(tank.Position, Vec2.GetUnitVectorDeg(bulletRotation), Tank) {rotation = bulletRotation};
            g.AddBullet(bullet);
            TimeLeftToShoot = TimeToShoot;
        }

        protected override void BarrelMove(Tank tank) {
            // Advanced aiming
            // vars
            var target = ((MyGame) game).Player.Tank;
            var targetVelocityWhenShot = target.Velocity + TimeLeftToShoot * target.Acceleration;
            var positionDelta = target.Position - tank.Position;

            // quadratic equation
            var a = targetVelocityWhenShot.Dot(targetVelocityWhenShot) - Bullet.Speed * Bullet.Speed;
            var b = 2f * targetVelocityWhenShot.Dot(positionDelta);
            var c = positionDelta.Dot(positionDelta);
            var det = b * b - 4f * a * c;
            if (det <= 0f) return;
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
            
            attackIndicator.UpdateIndicator(TimeLeftToShoot, 0);
        }
    }
}