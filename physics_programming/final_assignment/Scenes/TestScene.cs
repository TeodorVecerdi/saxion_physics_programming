namespace physics_programming.final_assignment {
    public class TestScene : Scene {
        public override void Initialise(MyGame myGame) {
            myGame.Player = new Player(500, 400, 300);
            myGame.AddChild(myGame.Player);

            myGame.AddLine(new Vec2(0, Globals.HEIGHT), new Vec2(0, 0));
            myGame.AddLine(new Vec2(Globals.WIDTH, 0), new Vec2(Globals.WIDTH, Globals.HEIGHT));
            myGame.AddLine(new Vec2(0, 0), new Vec2(Globals.WIDTH, 0));
            myGame.AddLine(new Vec2(Globals.WIDTH, Globals.HEIGHT), new Vec2(0, Globals.HEIGHT));

            myGame.AddDestructibleLine(new Vec2(100, 300 + 200), new Vec2(50, 300 + 500));
            myGame.AddDestructibleLine(new Vec2(50, 300 + 500), new Vec2(49, 300 + 600));
            myGame.AddDestructibleLine(new Vec2(100, 300 + 200), new Vec2(500, 300 + 250));
            myGame.AddDestructibleLine(new Vec2(100, 300 + 175), new Vec2(500, 300 + 225));
            myGame.AddDestructibleLine(new Vec2(500, 300 + 250), new Vec2(550, 300 + 300));
            myGame.AddDestructibleLine(new Vec2(550, 300 + 300), new Vec2(600, 300 + 375));
            myGame.AddDestructibleLine(new Vec2(600, 300 + 375), new Vec2(625, 300 + 475));
            myGame.AddDestructibleLine(new Vec2(625, 300 + 475), new Vec2(626, 300 + 600));

            var block = new DestructibleBlock(30, new Vec2(200, 200), new Vec2(600, 200));
            myGame.DestructibleBlocks.Add(block);
            myGame.AddChild(block);

            var block2 = new DestructibleBlock(30, new Vec2(750, 200), new Vec2(1050, 200));
            myGame.DestructibleBlocks.Add(block2);
            myGame.AddChild(block2);
        }

        public override void Finalise(MyGame myGame) {
        }
    }
}