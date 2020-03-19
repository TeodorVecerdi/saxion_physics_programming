using GXPEngine;

namespace physics_programming.assignment2 {
    public class MyGame : Game {
        static void Main2() {
            new MyGame().Start();
        }

        public MyGame() : base(800, 600, false, false) {
            // background:
            AddChild(new Sprite("data/assets/desert.png"));

            // tank:
            AddChild(new Tank(width / 2f, height / 2f, 3));
        }
    }
}