
namespace PacMan.Models
{
    public class GameState
    {
        public int Score { get; set; } = 0;
        public int Lives { get; set; } = 3;
        public bool GameRunning { get; set; } = false;
        public bool Muted { get; set; } = false;
    }

    public struct Position
    {
        public double X { get; set; }
        public double Y { get; set; }

        public Position(double x, double y)
        {
            X = x;
            Y = y;
        }
    }

    public enum Direction
    {
        None = -1,
        Right = 0,
        Down = 1,
        Left = 2,
        Up = 3
    }

    public class Pacman
    {
        public Position Position { get; set; }
        public Direction CurrentDirection { get; set; }
        public Direction NextDirection { get; set; }
        public double Speed { get; set; } = 2.0;

        public Pacman()
        {
            Position = new Position(19 * 20, 21 * 20);
            CurrentDirection = Direction.Right;
            NextDirection = Direction.Right;
        }

        public void Reset()
        {
            Position = new Position(19 * 20, 21 * 20);
            CurrentDirection = Direction.Right;
            NextDirection = Direction.Right;
        }
         public string GetImageFileName()
        {
            return "pacman.png";
        }
    }

    public class Ghost
    {
        public Position Position { get; set; }
        public Direction Direction { get; set; }
        public string Color { get; set; }
        public double Speed { get; set; } = 1.0;

        public Ghost(double x, double y, Direction direction, string color)
        {
            Position = new Position(x, y);
            Direction = direction;
            Color = color;
        }
        public string GetImageFileName()
        {
            return Color switch
            {
                "Red" => "gosthred.png",
                "Pink" => "gosth-pink.png",
                "Cyan" => "gosth-blue-up.png",
                _ => "gosthred.png"
            };
        }
    }
}
