using System.ComponentModel;
using System.Runtime.CompilerServices;
using PacMan.Services;
using System.Windows.Input;


namespace PacMan.ViewModels
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        private readonly GameEngine _gameEngine;
        private string _currentScreen = "StartScreen";
        private string _scoreText = "0";
        private string _livesText = "3";
        private string _muteButtonText = "🔊";
        private bool _gameOverVisible = false;
        private string _finalScore = "0";

        public string Greeting => "¡Bienvenido a Pac-Man!";


        public MainWindowViewModel()
        {
            _gameEngine = new GameEngine();
            _gameEngine.ScoreChanged += OnScoreChanged;
            _gameEngine.GameOver += OnGameOver;

            StartGameCommand = new RelayCommand(StartGame);
            ShowScoreboardCommand = new RelayCommand(ShowScoreboard);
            BackToMenuCommand = new RelayCommand(BackToMenu);
            RestartGameCommand = new RelayCommand(RestartGame);
            ToggleMuteCommand = new RelayCommand(ToggleMute);
            ExitCommand = new RelayCommand(Exit);
        }

        public GameEngine GameEngine => _gameEngine;

        public ICommand StartGameCommand { get; }
        public ICommand ShowScoreboardCommand { get; }
        public ICommand BackToMenuCommand { get; }
        public ICommand RestartGameCommand { get; }
        public ICommand ToggleMuteCommand { get; }
        public ICommand ExitCommand { get; }

        public string CurrentScreen
        {
            get => _currentScreen;
            set => SetProperty(ref _currentScreen, value);
        }

        public string ScoreText
        {
            get => _scoreText;
            set => SetProperty(ref _scoreText, value);
        }

        public string LivesText
        {
            get => _livesText;
            set => SetProperty(ref _livesText, value);
        }

        public string MuteButtonText
        {
            get => _muteButtonText;
            set => SetProperty(ref _muteButtonText, value);
        }

        public bool GameOverVisible
        {
            get => _gameOverVisible;
            set => SetProperty(ref _gameOverVisible, value);
        }

        public string FinalScore
        {
            get => _finalScore;
            set => SetProperty(ref _finalScore, value);
        }

        public void StartGame()
        {
            CurrentScreen = "GameScreen";
            _gameEngine.StartGame();
            GameOverVisible = false;
        }

        public void ShowScoreboard()
        {
            CurrentScreen = "ScoreScreen";
        }

        public void BackToMenu()
        {
            CurrentScreen = "StartScreen";
            _gameEngine.GameState.GameRunning = false;
        }

        public void RestartGame()
        {
            _gameEngine.StartGame();
            GameOverVisible = false;
        }

        public void ToggleMute()
        {
            _gameEngine.GameState.Muted = !_gameEngine.GameState.Muted;
            MuteButtonText = _gameEngine.GameState.Muted ? "🔇" : "🔊";
        }

        private void OnScoreChanged()
        {
            ScoreText = _gameEngine.GameState.Score.ToString();
            LivesText = _gameEngine.GameState.Lives.ToString();
        }
        public void Exit()
        {
            System.Environment.Exit(0);
        }

        private void OnGameOver()
        {
            FinalScore = _gameEngine.GameState.Score.ToString();
            GameOverVisible = true;
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected bool SetProperty<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
        {
            if (Equals(field, value)) return false;
            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }
        public class RelayCommand : ICommand
        {
            private readonly System.Action _execute;
            private readonly System.Func<bool>? _canExecute;

            public RelayCommand(System.Action execute, System.Func<bool>? canExecute = null)
            {
            _execute = execute ?? throw new System.ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
            }

            public event System.EventHandler? CanExecuteChanged;

            public bool CanExecute(object? parameter)
            {
            return _canExecute?.Invoke() ?? true;
            }

            public void Execute(object? parameter)
            {
            _execute();
            }

            public void RaiseCanExecuteChanged()
            {
            CanExecuteChanged?.Invoke(this, System.EventArgs.Empty);
            }
        }
    }
}