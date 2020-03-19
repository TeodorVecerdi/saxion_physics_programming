using System;
using System.Collections.Generic;
using GXPEngine;
using physics_programming;

public class Block : EasyDraw {
    /******* PUBLIC FIELDS AND PROPERTIES *********************************************************/

    // These four public static fields are changed from MyGame, based on key input (see Console):
    public static bool drawDebugLine = false;
    public static bool wordy = false;
    public static float bounciness = .98f;
    // public static float bounciness = 1f;

    // For ease of testing / changing, we assume every block has the same acceleration (gravity):
    public static Vec2 acceleration = new Vec2(0, 0);
    public const float gravity = 0.0981f;
    public readonly int radius;

    // Mass = density * volume.
    // In 2D, we assume volume = area (=all objects are assumed to have the same "depth")
    public float Mass {
        get {
            return 4 * radius * radius * _density;
        }
    }

    public Vec2 position {
        get {
            return _position;
        }
    }

    public Vec2 velocity;

    /******* PRIVATE FIELDS *******************************************************************/

    private Vec2 _position;
    private Vec2 _oldPosition;

    private Arrow _velocityIndicator;

    private float _red = 1;
    private float _green = 1;
    private float _blue = 1;

    private float _density = 1;

    private const float _colorFadeSpeed = 0.125f;

    /******* PUBLIC METHODS *******************************************************************/

    public Block(int pRadius, Vec2 pPosition, Vec2 pVelocity) : base(pRadius * 2, pRadius * 2) {
        radius = pRadius;
        _position = pPosition;
        velocity = pVelocity;

        SetOrigin(radius, radius);
        draw();
        UpdateScreenPosition();
        _oldPosition = new Vec2(0, 0);

        _velocityIndicator = new Arrow(_position, velocity, 10);
        AddChild(_velocityIndicator);
    }

    public void SetFadeColor(float pRed, float pGreen, float pBlue) {
        _red = pRed;
        _green = pGreen;
        _blue = pBlue;
    }

    public void Update() {
        // For extra testing flexibility, we call the Step method from MyGame instead:
        //Step();
    }

    public void Step() {
        _oldPosition = _position;

        // No need to make changes in this Step method (most of it is related to drawing, color and debug info). 
        // Work in Move instead.
        Move();

        UpdateColor();
        UpdateScreenPosition();
        ShowDebugInfo();
    }

    /******* PRIVATE METHODS *******************************************************************/

    /******* THIS IS WHERE YOU SHOULD WORK: ***************************************************/

    void Move() {
        // TODO: implement Assignment 3 here (and in methods called from here).
        velocity += acceleration * gravity;
        _position += velocity;
        // var collisions = CheckCollisions();
        // ResolveCollisions(collisions);
        // _position += velocity;

        // Example methods (replace/extend):
        CheckBoundaryCollisions();
        CheckBlockOverlaps();
        
        

        // TIP: You can use the CollisionInfo class to pass information between methods, e.g.:
        //
        //Collision firstCollision=FindEarliestCollision();
        //if (firstCollision!=null)
        //	ResolveCollision(firstCollision);
    }

    // This method is just an example of how to check boundaries, and change color.
    void CheckBoundaryCollisions() {
        var horizontalCollisionTOI = 0f;
        var verticalCollisionTOI = 0f;
        MyGame myGame = (MyGame) game;
        if (_position.x - radius < myGame.LeftXBoundary) {
            var b = _position.x - radius;
            var a = myGame.LeftXBoundary + radius;
            horizontalCollisionTOI = a / b;

            // _position.x = myGame.LeftXBoundary + radius;
            // velocity.x *= -bounciness;
            SetFadeColor(1, 0.2f, 0.2f);
            if (wordy) {
                Console.WriteLine("Left boundary collision");
            }
        } else if (_position.x + radius > myGame.RightXBoundary) {
            var b = _position.x + radius;
            var a = myGame.RightXBoundary - radius;
            horizontalCollisionTOI = a / b;

            // _position.x = myGame.RightXBoundary - radius;
            // velocity.x *= -bounciness;
            SetFadeColor(1, 0.2f, 0.2f);
            if (wordy) {
                Console.WriteLine("Right boundary collision");
            }
        }

        if (_position.y - radius < myGame.TopYBoundary) {
            var b = _position.y - radius;
            var a = myGame.TopYBoundary + radius;
            verticalCollisionTOI = a / b;

            // _position.y = myGame.TopYBoundary + radius;
            // velocity.y *= -bounciness;
            SetFadeColor(0.2f, 1, 0.2f);
            if (wordy) {
                Console.WriteLine("Top boundary collision");
            }
        } else if (_position.y + radius > myGame.BottomYBoundary) {
            var b = _position.y + radius;
            var a = myGame.BottomYBoundary - radius;
            verticalCollisionTOI = a / b;

            // _position.y = myGame.BottomYBoundary - radius;
            // velocity.y *= -bounciness;
            SetFadeColor(0.2f, 1, 0.2f);
            if (wordy) {
                Console.WriteLine("Bottom boundary collision");
            }
        }

        Vec2 moveAmount = new Vec2(horizontalCollisionTOI * velocity.x, verticalCollisionTOI * velocity.y);
        _position -= moveAmount;
        if (horizontalCollisionTOI != 0f) {
            velocity.x *= -bounciness;
        }

        if (verticalCollisionTOI != 0) {
            velocity.y *= -bounciness;
        }
    }
    
    private List<CollisionInfo> CheckCollisions() {
        var myGame = (MyGame) game;
        var collisions = new List<CollisionInfo>();
        for (var i = 0; i < myGame.GetNumberOfMovers(); i++) {
            var other = myGame.GetMover(i);
            if (other == this)
                continue;
            if (HitTest(other, out var collisionInfo)) {
                collisions.Add(collisionInfo);
            }
        }

        return collisions;
    }

    private void ResolveCollisions(List<CollisionInfo> collisions) {
        collisions.ForEach(ResolveCollision);
    }

    // This method is just an example of how to get information about other blocks in the scene.
    void CheckBlockOverlaps() {
        MyGame myGame = (MyGame) game;
        for (int i = 0; i < myGame.GetNumberOfMovers(); i++) {
            Block other = myGame.GetMover(i);
            if (other != this) {
                if (HitTest(other, out var collisionInfo)) {
                    ResolveCollision(collisionInfo);
                    SetFadeColor(0.2f, 0.2f, 1);
                    other.SetFadeColor(0.2f, 0.2f, 1);
                    if (wordy) {
                        Console.WriteLine("Block-block overlap detected.");
                    }
                }
            }
        }
    }

    public bool HitTest(Block other, out CollisionInfo collisionInfo) {
        var collides = _position.x < other._position.x + other.width &&
                       _position.x + width > other._position.x &&
                       _position.y < other._position.y + other.height &&
                       _position.y + height > other._position.y;

        var collidesOrigin = _position.x - width / 2f < other._position.x + other.width / 2f &&
                             _position.x + width / 2f > other._position.x - other.width / 2f &&
                             _position.y - height / 2f < other._position.y + other.height / 2f &&
                             _position.y + height / 2f > other._position.y - other.height / 2f;

        // var collides = x + width > other.x &&
        //                x < other.x + other.width &&
        //                y < other.y + other.height &&
        //                y + height > other.y;
        if (!collidesOrigin) {
            collisionInfo = null;
            return false;
        }

        var overlapX = 0f;
        var overlapY = 0f;
        /*
         // ORIGIN OVERLAP
         if (_position.x < other._position.x) overlapX = -(_position.x + width/2f - other._position.x + other.width/2f);
        else overlapX = other._position.x + other.width/2f - _position.x + width/2f;
        if (_position.y < other._position.y) overlapY = -(_position.y + height/2f - other._position.y + other.height/2f);
        else overlapY = other._position.y + other.height/2f - _position.y + height/2f;*/
        
        
         // NORMAL OVERLAP
        if (_position.x < other._position.x) overlapX = -(_position.x + width - other._position.x);
        else overlapX = other._position.x + other.width - _position.x;
        if (_position.y < other._position.y) overlapY = -(_position.y + height - other._position.y);
        else overlapY = other._position.y + other.height - _position.y;
         

        var TOI = 0f;
        if (Math.Abs(overlapX) < Math.Abs(overlapY)) {
            overlapY = 0f;
            var b = _position.x;
            var a = _position.x - overlapX;
            TOI = b / a;
        } else {
            overlapX = 0f;
            var b = _position.y;
            var a = _position.y - overlapY;
            TOI = b / a;
        }

        collisionInfo = new CollisionInfo(new Vec2(overlapX, overlapY).normalized, other, TOI);
        return true;
    }

    public void ResolveCollision(CollisionInfo collisionInfo) {
        // Console.WriteLine(collisionInfo.timeOfImpact);
        // Reset position
        _position = _oldPosition + collisionInfo.timeOfImpact * velocity;
        
        // Bounce
        var other = collisionInfo.other as Block;
        // No Conservation
        /*if (Math.Abs(collisionInfo.normal.x) > 0.000001f) {
            velocity.x *= -1;
            (collisionInfo.other as Block).velocity.x *= -1;
        }

        if (Math.Abs(collisionInfo.normal.y) > 0.000001f) {
            velocity.y *= -1;
            (collisionInfo.other as Block).velocity.y *= -1;
        }
        */
        


        // Conservation of Momentum
        var u = (Mass * velocity + other.Mass * other.velocity) / (Mass + other.Mass);
        var newVel1 = u - bounciness * (velocity - u);
        var newVel2 = u - bounciness * (other.velocity - u);
        if (Math.Abs(collisionInfo.normal.x) > 0.000001f) {
            velocity.x = newVel1.x;
            other.velocity.x = newVel2.x;
        }

        if (Math.Abs(collisionInfo.normal.y) > 0.000001f) {
            velocity.y = newVel1.y;
            other.velocity.y = newVel2.y;
        }
    }

    /******* NO NEED TO CHANGE ANY OF THE CODE BELOW: **********************************************/

    void UpdateColor() {
        if (_red < 1) {
            _red = Mathf.Min(1, _red + _colorFadeSpeed);
        }

        if (_green < 1) {
            _green = Mathf.Min(1, _green + _colorFadeSpeed);
        }

        if (_blue < 1) {
            _blue = Mathf.Min(1, _blue + _colorFadeSpeed);
        }

        SetColor(_red, _green, _blue);
    }

    void ShowDebugInfo() {
        if (drawDebugLine) {
            ((MyGame) game).DrawLine(_oldPosition, _position);
        }

        _velocityIndicator.startPoint = _position;
        _velocityIndicator.vector = velocity;
    }

    void UpdateScreenPosition() {
        x = _position.x;
        y = _position.y;
    }

    void draw() {
        Fill(200);
        NoStroke();
        ShapeAlign(CenterMode.Min, CenterMode.Min);
        Rect(0, 0, width, height);
    }
}