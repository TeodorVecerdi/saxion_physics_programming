using System;
using GXPEngine;

public class Ball : EasyDraw
{
	// These four public static fields are changed from MyGame, based on key input (see Console):
	public static bool drawDebugLine = false;
	public static bool wordy = false;
	public static float bounciness = 0.98f;
	// For ease of testing / changing, we assume every ball has the same acceleration (gravity):
	public static Vec2 acceleration = new Vec2 (0, 0);


	public Vec2 velocity;
	public Vec2 position;

	public readonly int radius;
	public readonly bool moving;

	// Mass = density * volume.
	// In 2D, we assume volume = area (=all objects are assumed to have the same "depth")
	public float Mass {
		get {
			return radius * radius * _density;
		}
	}

	Vec2 _oldPosition;
	Arrow _velocityIndicator;

	float _density = 1;

	public Ball (int pRadius, Vec2 pPosition, Vec2 pVelocity=new Vec2(), bool moving=true) : base (pRadius*2 + 1, pRadius*2 + 1)
	{
		radius = pRadius;
		position = pPosition;
		velocity = pVelocity;
		this.moving = moving;

		position = pPosition;
		UpdateScreenPosition ();
		SetOrigin (radius, radius);

		Draw (230, 200, 0);

		_velocityIndicator = new Arrow(position, new Vec2(0,0), 10);
		AddChild(_velocityIndicator);
	}

	void Draw(byte red, byte green, byte blue) {
		Fill (red, green, blue);
		Stroke (red, green, blue);
		Ellipse (radius, radius, 2*radius, 2*radius);
	}

	void UpdateScreenPosition() {
		x = position.x;
		y = position.y;
	}

	public void Step () {
		velocity += acceleration;
		_oldPosition = position;
		position += velocity;
		// This can be removed after adding line segment collision detection:
		BoundaryWrapAround();			

		CollisionInfo firstCollision = FindEarliestCollision();
		if (firstCollision != null) {
			ResolveCollision(firstCollision);
		}

		UpdateScreenPosition();

		ShowDebugInfo();
	}

	CollisionInfo FindEarliestCollision() {
		MyGame myGame = (MyGame)game;
		// Check other movers:			
		for (int i = 0; i < myGame.GetNumberOfMovers(); i++) {
			Ball mover = myGame.GetMover(i);
			if (mover != this) {
				Vec2 relativePosition = position - mover.position;
				if (relativePosition.Length () < radius + mover.radius) {
					// TODO: compute correct normal and time of impact, and 
					// 		 return *earliest* collision instead of *first detected collision*:
					return new CollisionInfo (new Vec2 (1, 0), mover, 0);
				}
			}
		}
		// TODO: Check Line segments using myGame.GetLine();
		return null;
	}

	void ResolveCollision(CollisionInfo col) {
		// TODO: resolve the collision correctly: position reset & velocity reflection.
		// ...this is not an ideal collision resolve:
		velocity *= -1;
		if (col.other is Ball) {
			Ball otherBall = (Ball)col.other;
			otherBall.velocity *= -1;
		}
	}

	void BoundaryWrapAround() {
		if (position.x < 0) {
			position.x += game.width;
		}
		if (position.x > game.width) {			
			position.x -= game.width;
		}
		if (position.y < 0) {
			position.y += game.height;
		}
		if (position.y > game.height) {
			position.y -= game.height;
		}
	}

	void ShowDebugInfo() {
		if (drawDebugLine) {
			((MyGame)game).DrawLine (_oldPosition, position);
		}
		_velocityIndicator.startPoint = position;
		_velocityIndicator.vector = velocity;
	}
}

