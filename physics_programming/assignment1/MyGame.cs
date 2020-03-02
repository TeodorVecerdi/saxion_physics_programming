namespace physics_programming.assignment1 {
    using GXPEngine;
    using System.Drawing;

    public class MyGame : Game {
        public static void Main() {
            new MyGame().Start();
        }

        private Ball _ball;
        private EasyDraw _text;
        private float score;

        public MyGame() : base(800, 600, false, false) {
            _ball = new Ball(30, new Vec2(width / 2, height / 2));
            AddChild(_ball);

            _text = new EasyDraw(400, 50);
            _text.TextAlign(CenterMode.Min, CenterMode.Min);
            score = 0;
            AddChild(_text);
        }

        void Update() {
            if(_ball.IsDone) return;
            
            _ball.Step();
            score += Time.deltaTime;
            
            _text.Clear(Color.Transparent);
            _text.Text($"Score: {(int)score}", 0, 0);
        }
    }
}