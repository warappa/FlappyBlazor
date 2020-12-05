using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FlappyBlazor.Client.Domain
{
    public class GameManager
    {
        public Bird Bird { get; set; } = new Bird();
        public PipesCollection Pipes { get; set; } = new PipesCollection();

        public int Score { get; set; }
        public int BestScore { get; set; }

        public bool IsRunning { get; private set; }
        public bool IsGameOver { get; private set; }
        public bool IsPaused { get; private set; }

        private CancellationTokenSource loopCancellationToken;

        public event Action Rendered;

        public void StartGame()
        {
            //Console.WriteLine("Start Game");

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
            //Console.WriteLine("Start Game Loop");

            var lastTicks = DateTime.UtcNow.Ticks;
            while (IsRunning)
            {
                var deltaMs = (DateTime.UtcNow.Ticks - lastTicks) / 10000;
                var delta = 2 * deltaMs / 16;
                if (!IsPaused)
                {

                    //Console.WriteLine($"Loop {delta} {deltaMs} {stopwatch.ElapsedMilliseconds}");

                    Bird.Move(delta);

                    Pipes.Move(delta);

                    CheckCollisions();

                    CheckPassedPipes();


                    Rendered?.Invoke();
                }

                lastTicks = DateTime.UtcNow.Ticks;

                await Task.Delay(16);

                if (cancellationToken.IsCancellationRequested)
                {
                    break;
                }
            }
        }

        private void CheckPassedPipes()
        {
            foreach (var pipes in Pipes.ToList())
            {
                if (pipes.IsPassed)
                {
                    return;
                }

                var pipeBounds = pipes.PipeTop.GetBounds();
                if (pipeBounds.X + pipeBounds.Width < Bird.Left)
                {
                    Score++;
                    Pipes.AddPassedPipes(pipes);
                }
            }
        }

        private void CheckCollisions()
        {
            foreach (var pipes in Pipes)
            {
                var pipeBounds = new[] { pipes.PipeTop.GetBounds(), pipes.PipeBottom.GetBounds() };

                var birdBounds = Bird.GetBounds();
                foreach (var bounds in pipeBounds)
                {
                    if (birdBounds.IntersectsWith(bounds))
                    {
                        GameOver();
                    }
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

            IsGameOver = false;
        }

        public void GameOver()
        {
            StopGame();

            IsGameOver = true;

            BestScore = Math.Max(Score, BestScore);
        }
    }
}
