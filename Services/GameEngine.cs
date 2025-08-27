using System;

using System.Collections.Generic;
using PacMan.Models;
using System.Linq;
using static PacMan.Services.GameState;


namespace PacMan.Services
{
    public class GameEngine
    {
        //propiedades pub
        public GameState GameState { get; private set; }
        public Pacman Pacman { get; private set; }
        public List<Ghost> Ghosts { get; private set; }
        public int[,] Board { get; private set; }

        private Direction _pendingDirection = Direction.None;
        public const int SMALL_DOT_SCORE = 10;
        public const int LARGE_DOT_SCORE = 50;

        private const int TileSize = 20;
        private const int BoardWidth = 38;
        private const int BoardHeight = 28;
        private readonly Random _random = new();

        //eventos
        public event Action? GameOver;
        public event Action? ScoreChanged;

        public GameEngine()
        {
            GameState = new GameState();
            InitializeBoard();
            InitializePacman();
            InitializeGhosts();
        }

        private void InitializeBoard()
        {
            Board = new int[BoardHeight, BoardWidth]
            {
                {1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1},
                {1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
                {1,2,1,1,1,1,0,1,1,1,1,1,0,1,1,1,1,0,1,1,0,1,1,1,1,0,1,1,1,1,1,0,1,1,1,1,2,1},
                {1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
                {1,0,1,1,1,1,0,1,1,0,1,1,1,1,1,1,1,0,1,1,0,1,1,1,1,1,1,1,0,1,1,0,1,1,1,1,0,1},
                {1,0,0,0,0,0,0,1,1,0,0,0,0,1,1,1,1,0,1,1,0,1,1,1,1,0,0,0,0,1,1,0,0,0,0,0,0,1},
                {1,1,1,1,1,1,0,1,1,1,1,1,0,1,1,1,1,0,1,1,0,1,1,1,1,0,1,1,1,1,1,0,1,1,1,1,1,1},
                {3,3,3,3,3,1,0,1,1,1,1,1,0,1,1,1,1,0,1,1,0,1,1,1,1,0,1,1,1,1,1,0,1,3,3,3,3,3},
                {1,1,1,1,1,1,0,1,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,1,0,1,1,1,1,1,1},
                {1,1,1,1,1,1,0,1,1,0,1,1,1,3,3,3,3,3,3,3,3,3,3,3,1,1,1,0,1,1,0,1,1,1,1,1,1,1},
                {3,3,3,3,3,3,0,0,0,0,1,3,3,3,3,3,3,3,3,3,3,3,3,3,3,1,0,0,0,0,0,3,3,3,3,3,3,3},
                {1,1,1,1,1,1,0,1,1,0,1,3,3,3,3,3,3,3,3,3,3,3,3,3,3,1,0,1,1,0,1,1,1,1,1,1,1,1},
                {1,1,1,1,1,1,0,1,1,0,1,3,3,3,3,3,3,3,3,3,3,3,3,3,3,1,0,1,1,0,1,1,1,1,1,1,1,1},
                {1,1,1,1,1,1,0,1,1,0,1,3,3,3,3,3,3,3,3,3,3,3,3,3,3,1,0,1,1,0,1,1,1,1,1,1,1,1},
                {1,1,1,1,1,1,0,1,1,0,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,0,1,1,0,1,1,1,1,1,1,1,1},
                {3,3,3,3,3,3,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,3,3,3,3,3,3,3},
                {1,1,1,1,1,1,0,1,1,0,1,1,1,1,1,1,1,0,1,1,0,1,1,1,1,1,1,1,0,1,1,0,1,1,1,1,1,1},
                {1,1,1,1,1,1,0,1,1,0,1,1,1,1,1,1,1,0,1,1,0,1,1,1,1,1,1,1,0,1,1,0,1,1,1,1,1,1},
                {3,3,3,3,3,1,0,1,1,0,0,0,0,0,0,0,0,0,1,1,0,0,0,0,0,0,0,0,0,1,1,0,1,3,3,3,3,3},
                {1,1,1,1,1,1,0,1,1,1,1,1,0,1,1,1,1,0,1,1,0,1,1,1,1,0,1,1,1,1,1,0,1,1,1,1,1,1},
                {1,0,0,0,0,0,0,0,0,0,0,0,0,1,1,1,1,0,1,1,0,1,1,1,1,0,0,0,0,0,0,0,0,0,0,0,0,1},
                {1,0,1,1,1,1,0,1,1,1,1,1,0,1,1,1,1,0,1,1,0,1,1,1,1,0,1,1,1,1,1,0,1,1,1,1,0,1},
                {1,2,0,0,1,1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,1,0,0,2,1},
                {1,1,1,0,1,1,0,1,1,0,1,1,1,1,1,1,1,0,1,1,0,1,1,1,1,1,1,1,0,1,1,0,1,1,0,1,1,1},
                {1,0,0,0,0,0,0,1,1,0,0,0,0,1,1,1,1,0,1,1,0,1,1,1,1,0,0,0,0,1,1,0,0,0,0,0,0,1},
                {1,0,1,1,1,1,1,1,1,1,1,1,0,1,1,1,1,0,1,1,0,1,1,1,1,0,1,1,1,1,1,1,1,1,1,1,0,1},
                {1,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1},
                {1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1,1}
            };
        }

        private void InitializePacman()
        {

            Pacman = new Pacman
            {
                Position = new Position { X = 19 * 20, Y = 15 * 20 },
                Direction = Direction.None
            };
        }

        private void InitializeGhosts()
        {
           Ghosts = new List<Ghost>
            {
                new Ghost 
                { 
                    Position = new Position { X = 17 * 20, Y = 9 * 20 }, 
                    Color = "Red",
                    Direction = Direction.Left
                },
                new Ghost 
                { 
                    Position = new Position { X = 18 * 20, Y = 9 * 20 }, 
                    Color = "Pink",
                    Direction = Direction.Right
                },
                new Ghost 
                { 
                    Position = new Position { X = 19 * 20, Y = 9 * 20 }, 
                    Color = "Cyan",
                    Direction = Direction.Up
                }
            };
        }

        public void StartGame()
        {
            GameState.Score = 0;
            GameState.Lives = 3;
            GameState.GameRunning = true;
            InitializePacman();
            InitializeGhosts();

            
            InitializeBoard();

            OnScoreChanged();
        }
        public void SetPacmanDirection(Direction direction)
        {
            _pendingDirection = direction;
        }

        private bool CanMoveTo(int x, int y)
        {

            if (x < 0 || x >= Board.GetLength(1) || y < 0 || y >= Board.GetLength(0))
                return false;

            return Board[y, x] != 1;
        }

        public void Update()
        {
            if (!GameState.GameRunning) return;

            MovePacman();
            MoveGhosts();
            CheckGhostCollisions();
            CheckWinCondition();
        }

        private void MovePacman()
        {
            var currentGridX = Pacman.Position.X / 20; // TileSize = 20
            var currentGridY = Pacman.Position.Y / 20;

            // Intentar cambiar dirección si hay una pendiente
            if (_pendingDirection != Direction.None)
            {
                var newX = currentGridX;
                var newY = currentGridY;

                switch (_pendingDirection)
                {
                    case Direction.Up: newY--; break;
                    case Direction.Down: newY++; break;
                    case Direction.Left: newX--; break;
                    case Direction.Right: newX++; break;
                }

                if (CanMoveTo(newX, newY))
                {
                    Pacman.Direction = _pendingDirection;
                    _pendingDirection = Direction.None;
                }
            }

            // Mover en la dirección actual
            var nextX = currentGridX;
            var nextY = currentGridY;

            switch (Pacman.Direction)
            {
                case Direction.Up: nextY--; break;
                case Direction.Down: nextY++; break;
                case Direction.Left: nextX--; break;
                case Direction.Right: nextX++; break;
                case Direction.None: return; 
            }

            if (CanMoveTo(nextX, nextY))
            {
                Pacman.Position = new Position { X = nextX * 20, Y = nextY * 20 };
                CheckDotCollision(nextX, nextY);
            }
            else
            {
                // Si no puede moverse, detener
                Pacman.Direction = Direction.None;
            }
        }
        private void CheckDotCollision(int gridX, int gridY)
        {
            var tileValue = Board[gridY, gridX];

            if (tileValue == 0) 
            {
                Board[gridY, gridX] = -1; 
                GameState.Score += SMALL_DOT_SCORE;
                OnScoreChanged();
            }
            else if (tileValue == 2) 
            {
                Board[gridY, gridX] = -1; 
                GameState.Score += LARGE_DOT_SCORE;
                OnScoreChanged();
            }
        }


        private void MoveGhosts()
        {
            foreach (var ghost in Ghosts)
            {
                var currentGridX = ghost.Position.X / 20;
                var currentGridY = ghost.Position.Y / 20;

                
                var possibleDirections = new List<Direction>();

                
                if (CanMoveTo(currentGridX, currentGridY - 1)) possibleDirections.Add(Direction.Up);
                if (CanMoveTo(currentGridX, currentGridY + 1)) possibleDirections.Add(Direction.Down);
                if (CanMoveTo(currentGridX - 1, currentGridY)) possibleDirections.Add(Direction.Left);
                if (CanMoveTo(currentGridX + 1, currentGridY)) possibleDirections.Add(Direction.Right);

                if (possibleDirections.Count > 0)
                {
                    
                    var random = new Random();
                    var randomDirection = possibleDirections[random.Next(possibleDirections.Count)];

                    
                    if (possibleDirections.Count > 1)
                    {
                        var oppositeDirection = GetOppositeDirection(ghost.Direction);
                        if (possibleDirections.Contains(oppositeDirection) && randomDirection == oppositeDirection)
                        {
                            
                            var otherDirections = possibleDirections.Where(d => d != oppositeDirection).ToList();
                            if (otherDirections.Count > 0)
                            {
                                randomDirection = otherDirections[random.Next(otherDirections.Count)];
                            }
                        }
                    }

                    ghost.Direction = randomDirection;

                    
                    var nextX = currentGridX;
                    var nextY = currentGridY;

                    switch (ghost.Direction)
                    {
                        case Direction.Up: nextY--; break;
                        case Direction.Down: nextY++; break;
                        case Direction.Left: nextX--; break;
                        case Direction.Right: nextX++; break;
                    }

                    ghost.Position = new Position { X = nextX * 20, Y = nextY * 20 };
                }
            }
        }


        private Direction GetOppositeDirection(Direction direction)
        {
            return direction switch
            {
                Direction.Up => Direction.Down,
                Direction.Down => Direction.Up,
                Direction.Left => Direction.Right,
                Direction.Right => Direction.Left,
                _ => Direction.None
            };
        }

        private void CheckGhostCollisions()
        {
            foreach (var ghost in Ghosts)
            {
                // Verificar si Pacman y el fantasma están en la misma posición
                if (Math.Abs(Pacman.Position.X - ghost.Position.X) < 10 &&
                    Math.Abs(Pacman.Position.Y - ghost.Position.Y) < 10)
                {
                    
                    GameState.Lives--;
                    OnScoreChanged();

                    if (GameState.Lives <= 0)
                    {
                        GameState.GameRunning = false;
                        OnGameOver();
                    }
                    else
                    {
                        
                        InitializePacman();
                        InitializeGhosts();
                    }
                    break;
                }
            }
        }

        
        private void CheckWinCondition()
        {
            bool allDotsEaten = true;

            for (int y = 0; y < Board.GetLength(0); y++)
            {
                for (int x = 0; x < Board.GetLength(1); x++)
                {
                    if (Board[y, x] == 0 || Board[y, x] == 2) 
                    {
                        allDotsEaten = false;
                        break;
                    }
                }
                if (!allDotsEaten) break;
            }

            if (allDotsEaten)
            {
                
                GameState.GameRunning = false;
                OnGameOver();
            }
        }

        protected virtual void OnScoreChanged()
        {
            ScoreChanged?.Invoke();
        }

        protected virtual void OnGameOver()
        {
            GameOver?.Invoke();
        }

    }
    
    public class GameState
    {
        public bool GameRunning { get; set; } = false;
        public int Score { get; set; } = 0;
        public int Lives { get; set; } = 3;
        public bool Muted { get; set; } = false;
    }

     

    public class Position
    {
        public int X { get; set; }
        public int Y { get; set; }
    }

    public class Pacman
    {
        public Position Position { get; set; } = new Position();
        public Direction Direction { get; set; } = Direction.None;
    }

    public class Ghost
    {
        public Position Position { get; set; } = new Position();
        public string Color { get; set; } = "Red";
        public Direction Direction { get; set; } = Direction.None;
    }
}