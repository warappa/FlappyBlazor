using System;
using System.Collections.Generic;
using System.Linq;

namespace FlappyBlazor.Client.Domain
{
    public class PipesCollection : List<Pipes>
    {
        public int PassedPipes { get; set; }

        public void Reset()
        {
            PassedPipes = 0;

            Clear();

            AddPipes();
        }

        private void AddPipes()
        {
            var p = new Pipes();
            p.Reset();

            p.PipeTop.Left += 70;
            p.PipeBottom.Left += 70;
            CheckOverlaps(p);

            Add(p);
        }

        private void CheckOverlaps(Pipes p)
        {
            var previous = this
                .Where(x => x != p)
                .MaxBy(x => x.PipeTop.Left);

            p.SetLeft(144);

            var tooNear = true;
            var overlap = true;
            while (overlap || tooNear)
            {
                if (overlap)
                {
                    p.Move(10);
                }
                if (tooNear)
                {
                    p.SetRandomVerticalOffset();
                }

                var previousLeft = previous?.PipeTop?.Left ?? -1000;
                overlap = previousLeft + 70 >= p.PipeTop.Left;
                   
                var previousTop = previous?.PipeTop.Top ?? -1000;
                tooNear = Math.Abs(previousTop - p.PipeTop.Top) < 30;
            }
        }

        public void AddPassedPipes(Pipes pipes)
        {
            PassedPipes++;
            pipes.IsPassed = true;

            CheckLevel();
        }

        public void CheckLevel()
        {
            if (PassedPipes == 2)
            {
                AddPipes();
            }

            if (PassedPipes == 3)
            {
                AddPipes();
            }
        }

        public void Move(long delta)
        {
            foreach (var pipes in this)
            {
                pipes.Move(delta);
            }

            foreach (var pipes in this)
            {
                if (pipes.PipeTop.Left < -30)
                {
                    pipes.Reset();

                    CheckOverlaps(pipes);
                }
            }
        }
    }
}
