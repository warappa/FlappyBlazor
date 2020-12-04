using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
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

        private double deltaSum = 0;

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
        }

        public void Jump()
        {
            Jumped = true;
        }
    }

    public class GameManager
    {
        public Bird Bird { get; set; }
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
            Bird.Top = 0;

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
