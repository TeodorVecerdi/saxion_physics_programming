using System;
using System.Collections.Generic;
using System.Drawing;
using GXPEngine;
using GXPEngine.Core;
using GXPEngine.OpenGL;

namespace physics_programming.testing {
    public class AttackIndicator : EasyDraw {
        private readonly float initialArcAngle;
        private readonly float initialTimeLeft;
        private readonly float radius;
        private readonly Color color;

        private float arcAngle;
        private float startAngle;
        private float timeLeft;

        public AttackIndicator(float initialTimeLeft, Color? color = null, float initialArcAngle = 90f, float radius = 2f) : base((int) (2 * radius), (int) (2 * radius)) {
            this.initialTimeLeft = initialTimeLeft;
            this.color = color ?? Color.White;
            this.initialArcAngle = initialArcAngle;
            this.radius = radius;
            timeLeft = initialTimeLeft;
            Draw();
        }

        public void Draw() {
            Clear(Color.Transparent);
            Fill(color, color.A);
            NoStroke();
            Arc(radius, radius, 2*radius, 2*radius, startAngle, arcAngle);
        }

        private void RecalculateAngles(float parentRotation) {
            arcAngle = (timeLeft * initialArcAngle) / initialTimeLeft;
            startAngle = -arcAngle / 2f + parentRotation;
        // startAngle += parentRotation;

            Debug.LogWarning($"arcAngle: {arcAngle} - startAngle {startAngle}");
        }

        public void UpdateIndicator(float newTimeLeft, float parentRotation) {
            timeLeft = newTimeLeft;
            RecalculateAngles(parentRotation);
            Draw();
        }
    }
}