using System;
using System.Diagnostics.CodeAnalysis;
using GXPEngine;

namespace physics_programming.final_assignment {
    public class Enemy : GameObject {
        private Tank enemyTank;
        private Player player;
        private const float timeToShoot = 2f;
        private float timeLeftToShoot = timeToShoot;
        public Enemy(float px, float py, float maxVelocity, Player player) {
            this.player = player;
            enemyTank = new Tank(px, py, TankMove, TankShoot, BarrelMove);
            AddChild(enemyTank);
        }

        [SuppressMessage("ReSharper", "UnusedMember.Local")]
        private void Update() {
            timeLeftToShoot -= Time.deltaTime;
        }

        private void TankMove(Tank tank) {
            // Enemy tanks don't move
        }

        private void TankShoot(Tank tank) {
            if (timeLeftToShoot > 0f)
                return;
            
            var g = (MyGame) game;
            var bullet = new Bullet(tank.Position, Vec2.GetUnitVectorDeg(tank.Barrel.rotation + tank.rotation) * 5f) {rotation = tank.Barrel.rotation + tank.rotation};
            g.AddBullet(bullet);
            timeLeftToShoot = timeToShoot;
        }

        private void BarrelMove(Tank tank) {
            var targetPosition = player.PlayerTank.Position;
            var desiredRotation = -tank.rotation + Vec2.Rad2Deg((float)Math.Atan2(targetPosition.y - tank.Position.y, targetPosition.x - tank.Position.x));
            var delta = desiredRotation - tank.Barrel.rotation;
            var shortestAngle = Mathf.Clamp(delta - Mathf.Floor(delta / 360f) * 360f, 0.0f, 360f);
            if (shortestAngle > 180)
                shortestAngle -= 360;
            if(Math.Abs(tank.Barrel.rotation - desiredRotation) > 0.5)
                tank.Barrel.rotation += shortestAngle * 0.15f;
        }
    }
}