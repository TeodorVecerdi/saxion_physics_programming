using GXPEngine;

// For GameObject

namespace physics_programming.final_assignment {
    public class CollisionInfo {
        public readonly float TimeOfImpact;
        public readonly GameObject Other;
        public readonly Vec2 Normal;

        public CollisionInfo(Vec2 pNormal, GameObject pOther, float pTimeOfImpact) {
            Normal = pNormal;
            Other = pOther;
            TimeOfImpact = pTimeOfImpact;
        }
    }
}