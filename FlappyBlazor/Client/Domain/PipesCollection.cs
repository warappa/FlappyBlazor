using System;
using System.Collections.Generic;

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
            for (var i = 0; i < Count; i++)
            {
                var pp = this[i];

                if (pp == p)
                {
                    continue;
                }

                //Console.WriteLine($"check {pp.PipeTop.GetBounds()} vs {p.PipeTop.GetBounds()}");
                if (pp.PipeTop.GetBounds()
                    .IntersectsWith(p.PipeTop.GetBounds()))
                {
                    Console.WriteLine("Next try");
                    p.PipeTop.Left += 70;
                    p.PipeBottom.Left += 70;
                    i = 0;
                }
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
