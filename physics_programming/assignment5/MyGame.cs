using System;
using GXPEngine;
using System.Drawing;
using System.Collections.Generic;

public class MyGame : Game
{	
	bool _stepped = false;
	bool _paused = false;
	int _stepIndex = 0;
	int _startSceneNumber = 0;

	Canvas _lineContainer = null;

	List<Ball> _movers;
	List<LineSegment> _lines;

	public int GetNumberOfLines() {
		return _lines.Count;
	}

	public LineSegment GetLine(int index) {
		if (index >= 0 && index < _lines.Count) {
			return _lines [index];
		}
		return null;	
	}

	public int GetNumberOfMovers() {
		return _movers.Count;
	}

	public Ball GetMover(int index) {
		if (index >= 0 && index < _movers.Count) {
			return _movers [index];
		}
		return null;
	}
	
	public void DrawLine(Vec2 start, Vec2 end) {
		_lineContainer.graphics.DrawLine(Pens.White, start.x, start.y, end.x, end.y);
	}

	public MyGame () : base(800, 600, false, false)
	{
		_lineContainer = new Canvas(width, height);
		AddChild(_lineContainer);

		targetFps = 60;

		_movers = new List<Ball>();
		_lines = new List<LineSegment>();

		LoadScene(_startSceneNumber);

		PrintInfo();
	}
	
	void AddLine (Vec2 start, Vec2 end) {
		LineSegment line = new LineSegment (start, end, 0xff00ff00, 4);
		AddChild (line);
		_lines.Add (line);
	}

	void LoadScene(int sceneNumber) {
		_startSceneNumber = sceneNumber;
		// remove previous scene:
		foreach (Ball mover in _movers) {
			mover.Destroy();
		}
		_movers.Clear();
		foreach (LineSegment line in _lines) {
			line.Destroy();
		}
		_lines.Clear();
		
		// boundary:
		AddLine (new Vec2 (width-60, height-60), new Vec2 (50, height-20));	//bottom
		AddLine (new Vec2 (50, height-20), new Vec2 (200, 60));
		AddLine (new Vec2 (200, 60), new Vec2 (width-20, 50));
		AddLine (new Vec2 (width-20, 50), new Vec2 (width-60, height-60));  //right

		switch (sceneNumber) {
			// BALL / BALL COLLISION SCENES:
			case 1: // one moving ball (medium speed), one fixed ball.
				Ball.acceleration.SetXY(0, 0);
				_movers.Add(new Ball(30, new Vec2(200, 300), new Vec2(38, 0)));
				_movers.Add(new Ball(30, new Vec2(400, 340)));
				break;				
			case 2: // one moving ball (high speed), one fixed ball.
				Ball.acceleration.SetXY(0, 0);
				_movers.Add(new Ball(30, new Vec2(200, 300), new Vec2(72, 0)));
				_movers.Add(new Ball(30, new Vec2(400, 340)));
				break;
			case 3: // many balls:
				Ball.acceleration.SetXY(0, 0);

				_movers.Add(new Ball(30, new Vec2(200, 300), new Vec2(3,4)));
				_movers.Add(new Ball(50, new Vec2(600, 300), new Vec2(5,4)));
				_movers.Add(new Ball(40, new Vec2(400, 300), new Vec2(-3,4)));
				_movers.Add(new Ball(15, new Vec2(500, 200), new Vec2(7,4)));
				_movers.Add(new Ball(20, new Vec2(300, 400), new Vec2(-3,4)));
				_movers.Add(new Ball(30, new Vec2(200, 200), new Vec2(3,4)));
				_movers.Add(new Ball(50, new Vec2(600, 200), new Vec2(5,4)));
				_movers.Add(new Ball(40, new Vec2(300, 200), new Vec2(-3,4)));
				_movers.Add(new Ball(15, new Vec2(400, 100), new Vec2(7,4)));
				_movers.Add(new Ball(20, new Vec2(500, 300), new Vec2(-3,4)));
				break;
			case 4: // one moving ball bouncing on some fixed balls:
				Ball.acceleration.SetXY(0, 1);
				_movers.Add(new Ball(30, new Vec2(200, 470), moving: false));
				_movers.Add(new Ball(30, new Vec2(260, 500), moving: false));
				_movers.Add(new Ball(30, new Vec2(320, 500), moving: false));
				_movers.Add(new Ball(30, new Vec2(380, 470), moving: false));
				_movers.Add(new Ball(30, new Vec2(400, 302), new Vec2(0, 0)));
				break;
			// LINE SEGMENT SCENES:
			case 5: // line segment:
				Ball.acceleration.SetXY(0, 0);
				_movers.Add(new Ball(30, new Vec2(200, 300), new Vec2(20, 0)));
				AddLine(new Vec2(290, 250), new Vec2(455, 350));
				break;
			case 6: // polygon:
				Ball.acceleration.SetXY(0, 1);
				_movers.Add(new Ball(30, new Vec2(400, 180), new Vec2(0, 0)));
				AddLine(new Vec2(290, 250), new Vec2(455, 350));
				AddLine(new Vec2(455, 350), new Vec2(600, 250));
				AddLine(new Vec2(600, 250), new Vec2(450, 300));
				AddLine(new Vec2(450, 300), new Vec2(290, 250));
				break;				
			default: // one moving ball (low speed), one fixed ball.
				Ball.acceleration.SetXY(0, 0);
				_movers.Add(new Ball(30, new Vec2(200, 300), new Vec2(10, 0)));
				_movers.Add(new Ball(30, new Vec2(400, 340)));
				break;
		}		
		_stepIndex = -1;
		foreach (Ball b in _movers) {
			AddChild(b);
		}
	}

	/****************************************************************************************/

	void PrintInfo() {
		Console.WriteLine("Hold spacebar to slow down the frame rate.");
		Console.WriteLine("Use arrow keys and backspace to set the gravity.");
		Console.WriteLine("Press S to toggle stepped mode.");
		Console.WriteLine("Press P to toggle pause.");
		Console.WriteLine("Press D to draw debug lines.");
		Console.WriteLine("Press C to clear all debug lines.");
		Console.WriteLine("Press R to reset scene, and numbers to load different scenes.");
		Console.WriteLine("Press B to toggle high/low bounciness.");
		Console.WriteLine("Press W to toggle extra output text.");
	}

	void HandleInput() {
		targetFps = Input.GetKey(Key.SPACE) ? 5 : 60;
		if (Input.GetKeyDown (Key.UP)) {
			Ball.acceleration.SetXY (0, -1);
		}
		if (Input.GetKeyDown (Key.DOWN)) {
			Ball.acceleration.SetXY (0, 1);
		}
		if (Input.GetKeyDown (Key.LEFT)) {
			Ball.acceleration.SetXY (-1, 0);
		}
		if (Input.GetKeyDown (Key.RIGHT)) {
			Ball.acceleration.SetXY (1, 0);
		}
		if (Input.GetKeyDown (Key.BACKSPACE)) {
			Ball.acceleration.SetXY (0, 0);
		}
		if (Input.GetKeyDown (Key.S)) {
			_stepped ^= true;
		}
		if (Input.GetKeyDown (Key.D)) {
			Ball.drawDebugLine ^= true;
		}
		if (Input.GetKeyDown (Key.P)) {
			_paused ^= true;
		}
		if (Input.GetKeyDown (Key.B)) {
			Ball.bounciness = 1.5f - Ball.bounciness;
		}
		if (Input.GetKeyDown(Key.W)) {
			Ball.wordy ^= true;
		}
		if (Input.GetKeyDown (Key.C)) {
			_lineContainer.graphics.Clear (Color.Black);
		}
		if (Input.GetKeyDown (Key.R)) {
			LoadScene (_startSceneNumber);
		}
		for (int i = 0; i< 10; i++) {
			if (Input.GetKeyDown (48 + i)) {
				LoadScene (i);
			}
		}
	}

	void StepThroughMovers() {
		if (_stepped) { // move everything step-by-step: in one frame, only one mover moves
			_stepIndex++;
			if (_stepIndex >= _movers.Count) {
				_stepIndex = 0;
			}
			if (_movers [_stepIndex].moving) {
				_movers [_stepIndex].Step ();
			}
		} else { // move all movers every frame
			foreach (Ball mover in _movers) {
				if (mover.moving) {
					mover.Step ();
				}
			}
		}
	}

	void Update () {
		HandleInput();
		if (!_paused) {
			StepThroughMovers ();
		}
	}

	static void Main() {
		new MyGame().Start();
	}
}