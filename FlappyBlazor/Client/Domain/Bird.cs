using System;
using System.Drawing;

namespace FlappyBlazor.Client.Domain
{
    public class Bird
    {
        public double Gravity = 0.10;
        public int Top { get; internal set; } = 0;
        public double Velocity { get; set; } = 0.0;

        public bool Jumped { get; private set; }
        public int Left { get; internal set; } = 20;

        public Rectangle GetBounds()
        {
            return new Rectangle(Left, Top, 17, 12);
        }

        public void Move(double delta)
        {
            if (Jumped)
            {
                Jumped = false;
                Velocity = -2.8;
            }
            Velocity = Velocity + Gravity * delta;
            //Console.WriteLine($"Velocity {Gravity} {Velocity} {delta}");
            Top += (int)Math.Floor(Velocity);

            Top = Math.Min(Math.Max(Top, 0), 216 - 12);
            if (Top == 216 - 12)
            {
                Velocity = 0;
            }
        }

        public void Jump()
        {
            Jumped = true;
        }

        public void Reset()
        {
            Top = 0;
        }
    }
}
