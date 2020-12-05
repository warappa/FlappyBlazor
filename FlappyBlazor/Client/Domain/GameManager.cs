using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace FlappyBlazor.Client.Domain
{
    public class Bird
    {
        public double Gravity = 0.15;
        public int Top { get; internal set; } = 0;
        public double Velocity { get; set; } = 0.0;

        public bool Jumped { get; private set; }

        public void Move(double delta)
        {
            if (Jumped)
            {
                Jumped = false;
                Velocity = -3;
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

    public class Pipe
    {
        public int Left { get; set; }
        public int Top { get; set; }
    }

    public class Pipes
    {
        public Pipes()
        {
            PipeBottom = new Pipe();
            PipeTop = new Pipe();
        }

        public Pipe PipeTop { get; set; }
        public Pipe PipeBottom { get; set; }

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
            PipeTop.Top = random - 25;
            PipeBottom.Top = random + 25;
        }
    }

    public class GameManager
    {
        public Bird Bird { get; set; }
        public Pipes Pipes { get; set; }

        public bool IsRunning { get; private set; }

        private CancellationTokenSource loopCancellationToken;

        public event Action Rendered;

        public void StartGame()
        {
            if (IsRunning)
            {
                loopCancellationToken?.Cancel();
                IsRunning = false;
                return;
            }

            Console.WriteLine("Start Game");

            Bird = new Bird();
            Bird.Reset();

            Pipes = new Pipes();
            Pipes.Reset();

            IsRunning = true;

            loopCancellationToken = new CancellationTokenSource();
            _ = GameLoop(loopCancellationToken.Token);
        }

        public async Task GameLoop(CancellationToken cancellationToken)
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            Console.WriteLine("Start Game Loop");

            var lastTicks = 0L;
            while (IsRunning)
            {
                var deltaMs = stopwatch.ElapsedMilliseconds - lastTicks;
                var delta = deltaMs / 8;

                //Console.WriteLine($"Loop {delta} {deltaMs} {stopwatch.ElapsedMilliseconds}");

                Bird.Move(delta);
                Pipes.Move(delta);

                lastTicks = stopwatch.ElapsedMilliseconds;

                Rendered?.Invoke();

                await Task.Delay(8);

                if (cancellationToken.IsCancellationRequested)
                {
                    break;
                }
            }

            stopwatch.Stop();
        }
    }
}
