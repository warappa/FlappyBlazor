using System.Drawing;

namespace FlappyBlazor.Client.Domain
{
    public class Pipe
    {
        public int Left { get; set; }
        public int Top { get; set; }

        public Rectangle GetBounds()
        {
            return new Rectangle(Left, Top, 26, 160);
        }
    }
}
