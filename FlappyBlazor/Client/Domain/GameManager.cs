using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace FlappyBlazor.Client.Domain
{
    public class GameManager
    {
        public Bird Bird { get; set; } = new Bird();
        public Pipes Pipes { get; set; } = new Pipes();

        public int Score { get; set; }
        public int BestScore { get; set; }

        public bool IsRunning { get; private set; }
        public bool IsGameOver { get; private set; }
        public bool IsPaused { get; private set; }

        private CancellationTokenSource loopCancellationToken;

        public event Action Rendered;

        public void StartGame()
        {
            Console.WriteLine("Start Game");

            Score = 0;

            Bird.Reset();
            Pipes.Reset();

            IsGameOver = false;
            IsRunning = true;
            IsPaused = false;

            loopCancellationToken = new CancellationTokenSource();
            _ = GameLoop(loopCancellationToken.Token);
        }

        public void PauseGame()
        {
            if (!IsPaused)
            {
                IsPaused = true;
            }
        }

        public void ResumeGame()
        {
            if (IsPaused)
            {
                IsPaused = false;
            }
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
                if (!IsPaused)
                {

                    //Console.WriteLine($"Loop {delta} {deltaMs} {stopwatch.ElapsedMilliseconds}");

                    Bird.Move(delta);
                    Pipes.Move(delta);

                    CheckCollisions();

                    CheckPassedPipes();


                    Rendered?.Invoke();
                }
                
                lastTicks = stopwatch.ElapsedMilliseconds;
                
                await Task.Delay(8);

                if (cancellationToken.IsCancellationRequested)
                {
                    break;
                }
            }

            stopwatch.Stop();
        }

        private void CheckPassedPipes()
        {
            if (Pipes.IsPassed)
            {
                return;
            }

            var pipeBounds = Pipes.PipeTop.GetBounds();
            if (pipeBounds.X + pipeBounds.Width < Bird.Left)
            {
                Score++;
                Pipes.IsPassed = true;
            }
        }

        private void CheckCollisions()
        {
            var pipeBounds = new[] { Pipes.PipeTop.GetBounds(), Pipes.PipeBottom.GetBounds() };

            var birdBounds = Bird.GetBounds();
            foreach (var bounds in pipeBounds)
            {
                if (birdBounds.IntersectsWith(bounds))
                {
                    GameOver();
                }
            }
        }

        public void StopGame()
        {
            if (IsRunning)
            {
                loopCancellationToken?.Cancel();
                IsRunning = false;
            }
        }

        public void GameOver()
        {
            StopGame();

            IsGameOver = true;

            BestScore = Math.Max(Score, BestScore);
        }
    }
}
