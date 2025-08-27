using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Threading;
using System;
using PacMan.Models;
using PacMan.Services;
using static PacMan.Services.GameState;

namespace PacMan.Controls
{
    public class GameCanvas : Control
    {
        private GameEngine? _gameEngine;
        private DispatcherTimer? _gameTimer;
        private const int TileSize = 20;

        private Bitmap? _pacmanImage;
        private Bitmap? _redGhostImage;
        private Bitmap? _pinkGhostImage;
        private Bitmap? _blueGhostImage;

        public static readonly StyledProperty<GameEngine?> GameEngineProperty =
            AvaloniaProperty.Register<GameCanvas, GameEngine?>(nameof(GameEngine));

        public GameEngine? GameEngine
        {
            get => GetValue(GameEngineProperty);
            set => SetValue(GameEngineProperty, value);
        }
        public GameCanvas()
        {
            LoadImages();
            Focusable = true;
        }

        protected override void OnLoaded(Avalonia.Interactivity.RoutedEventArgs e)
        {
            base.OnLoaded(e);
            Focus();
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);

            if (_gameEngine?.GameState.GameRunning == true)
            {
                Direction newDirection = Direction.None;

                switch (e.Key)
                {
                    case Key.Up:
                    case Key.W:
                        newDirection = Direction.Up;
                        break;
                    case Key.Down:
                    case Key.S:
                        newDirection = Direction.Down;
                        break;
                    case Key.Left:
                    case Key.A:
                        newDirection = Direction.Left;
                        break;
                    case Key.Right:
                    case Key.D:
                        newDirection = Direction.Right;
                        break;
                }

                if (newDirection != Direction.None)
                {
                    _gameEngine.SetPacmanDirection(newDirection);
                }
            }
        }

        private void LoadImages()
        {
            try
            {
                _pacmanImage = ImageLoader.LoadImage("pacman.png");
                _redGhostImage = ImageLoader.LoadImage("gosth-red.png");
                _pinkGhostImage = ImageLoader.LoadImage("gosth-pink.png");
                _blueGhostImage = ImageLoader.LoadImage("gosth-blue-up.png");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading images: {ex.Message}");
            }
        }

        protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
        {
            base.OnPropertyChanged(change);

            if (change.Property == GameEngineProperty)
            {
                if (_gameEngine != null)
                {
                    StopGameLoop();
                }

                _gameEngine = change.NewValue as GameEngine;

                if (_gameEngine != null)
                {
                    StartGameLoop();
                }
            }
        }

        private void StartGameLoop()
        {
            _gameTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(100)
            };
            _gameTimer.Tick += GameLoop;
            _gameTimer.Start();
        }

        private void StopGameLoop()
        {
            _gameTimer?.Stop();
            _gameTimer = null;
        }

        private void GameLoop(object? sender, EventArgs e)
        {
            if (_gameEngine?.GameState.GameRunning == true)
            {
                _gameEngine.Update();
                InvalidateVisual();
            }
        }

        public override void Render(DrawingContext context)
        {
            base.Render(context);
            context.FillRectangle(Brushes.Black, new Rect(Bounds.Size));

            if (_gameEngine == null) return;

           
            context.FillRectangle(Brushes.Black, new Rect(0, 0, Bounds.Width, Bounds.Height));


            DrawBoard(context);
            DrawPacman(context);
            DrawGhosts(context);
        }

        private void DrawBoard(DrawingContext context)
        {
            if (_gameEngine == null) return;

            var board = _gameEngine.Board;
            var boardHeight = board.GetLength(0);
            var boardWidth = board.GetLength(1);

            for (int y = 0; y < boardHeight; y++)
            {
                for (int x = 0; x < boardWidth; x++)
                {
                    var tile = board[y, x];
                    var rect = new Rect(x * TileSize, y * TileSize, TileSize, TileSize);

                    switch (tile)
                    {
                        case 1: 
                            context.FillRectangle(Brushes.Blue, rect);
                            break;
                        case 0: 
                            var smallDotCenter = new Point(
                                 x * TileSize + TileSize / 2,
                                 y * TileSize + TileSize / 2);
                            context.FillRectangle(Brushes.Yellow,
                                new Rect(smallDotCenter.X - 2, smallDotCenter.Y - 2, 4, 4));
                            break;
                        case 2: 
                            var largeDotCenter = new Point(
                                x * TileSize + TileSize / 2,
                                y * TileSize + TileSize / 2);
                            context.FillRectangle(Brushes.Yellow,
                                new Rect(largeDotCenter.X - 6, largeDotCenter.Y - 6, 12, 12));

                            break;
                    }
                }
            }
        }

        private void DrawPacman(DrawingContext context)
        {
            if (_gameEngine == null) return;

            var pacman = _gameEngine.Pacman;
            var rect = new Rect(
                pacman.Position.X,
                pacman.Position.Y,
                TileSize,
                TileSize);

            if (_pacmanImage != null)
            {

                context.DrawImage(_pacmanImage, rect);
            }
            else
            {

                var geometry = new EllipseGeometry(new Rect(
                    pacman.Position.X + 2,
                    pacman.Position.Y + 2,
                    TileSize - 4,
                    TileSize - 4));
                context.DrawGeometry(Brushes.Yellow, null, geometry);
            }
        }

        private void DrawGhosts(DrawingContext context)
        {
            if (_gameEngine == null) return;

            foreach (var ghost in _gameEngine.Ghosts)
            {
                var rect = new Rect(
                    ghost.Position.X,
                    ghost.Position.Y,
                    TileSize,
                    TileSize);

                Bitmap? ghostImage = ghost.Color switch
                {
                    "Red" => _redGhostImage,
                    "Pink" => _pinkGhostImage,
                    "Cyan" => _blueGhostImage,
                    _ => _redGhostImage
                };

                if (ghostImage != null)
                {

                    context.DrawImage(ghostImage, rect);
                }
                else
                {

                    var brush = ghost.Color switch
                    {
                        "Red" => Brushes.Red,
                        "Pink" => Brushes.Pink,
                        "Cyan" => Brushes.Cyan,
                        _ => Brushes.Orange
                    };

                    var geometry = new EllipseGeometry(new Rect(
                        ghost.Position.X + 2,
                        ghost.Position.Y + 2,
                        TileSize - 4,
                        TileSize - 4));
                    context.DrawGeometry(brush, null, geometry);
                }
            }
        }
        protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
        {
            base.OnDetachedFromVisualTree(e);

            StopGameLoop();
            _pacmanImage?.Dispose();
            _redGhostImage?.Dispose();
            _pinkGhostImage?.Dispose();
            _blueGhostImage?.Dispose();
        }
        
        
    }
}