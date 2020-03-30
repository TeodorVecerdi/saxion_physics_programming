using GXPEngine; // For GameObject

public class CollisionInfo {
	public readonly Vec2 normal;
	public readonly GameObject other;
	public readonly float timeOfImpact;

	public CollisionInfo(Vec2 pNormal, GameObject pOther, float pTimeOfImpact) {
		normal = pNormal;
		other = pOther;
		timeOfImpact = pTimeOfImpact;
	}
}
