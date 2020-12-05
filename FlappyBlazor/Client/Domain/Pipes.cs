using System;

namespace FlappyBlazor.Client.Domain
{
    public class Pipes
    {
        public Pipes()
        {
            PipeBottom = new Pipe();
            PipeTop = new Pipe();
        }

        public Pipe PipeTop { get; set; }
        public Pipe PipeBottom { get; set; }
        public bool IsPassed { get; set; }

        public double Velocity { get; set; } = 1.0;

        public void Move(double delta)
        {
            //Console.WriteLine($"Velocity {Gravity} {Velocity} {delta}");
            PipeTop.Left -= (int)Math.Floor(Velocity * delta);
            PipeBottom.Left -= (int)Math.Floor(Velocity * delta);

            if (PipeTop.Left < -30)
            {
                Reset();
            }
        }

        public void Reset()
        {
            PipeTop.Left = 150;
            PipeBottom.Left = 150;

            var random = 128 - 40 + new Random().Next(-44, 44);
            PipeTop.Top = random - 35 - 160;
            PipeBottom.Top = random + 35;
            
            IsPassed = false;
        }
    }
}
