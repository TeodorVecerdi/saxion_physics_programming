using System.Diagnostics.CodeAnalysis;
using GXPEngine;

namespace physics_programming.final_assignment {
    public abstract class Enemy : GameObject {
        protected const float AccuracyDegreeVariation = 30f;

        protected const float timeToShoot = 2f;

        protected readonly float Accuracy;
        protected float timeLeftToShoot = timeToShoot;

        protected Enemy(float accuracy) {
            Accuracy = accuracy;
        }

        protected abstract void BarrelMove(Tank tank);
        protected abstract void TankMove(Tank tank);
        protected abstract void TankShoot(Tank tank);

        [SuppressMessage("ReSharper", "UnusedMember.Local")]
        public void Update() {
            timeLeftToShoot -= Time.deltaTime;
        }
    }
}