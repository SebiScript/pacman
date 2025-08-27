using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using PacMan.Models;
using PacMan.ViewModels;

namespace PacMan.Views;

public partial class MainWindow : Window
{
    private MainWindowViewModel? _viewModel;

    public MainWindow()
    {
        InitializeComponent();
        _viewModel = new MainWindowViewModel();
        DataContext = _viewModel;

       
        KeyDown += OnKeyDown;
        Focusable = true;
    }
    private void OnKeyDown(object? sender, KeyEventArgs e)
    {
        if (_viewModel?.GameEngine.GameState.GameRunning != true) return;

        switch (e.Key)
        {
            case Key.Right:
                _viewModel.GameEngine.SetPacmanDirection(Direction.Right);
                break;
            case Key.Down:
                _viewModel.GameEngine.SetPacmanDirection(Direction.Down);
                break;
            case Key.Left:
                _viewModel.GameEngine.SetPacmanDirection(Direction.Left);
                break;
            case Key.Up:
                _viewModel.GameEngine.SetPacmanDirection(Direction.Up);
                break;
        }
    }
    private void StartGame_Click(object? sender, RoutedEventArgs e)
        {
            _viewModel?.StartGame();
            Focus(); 
        }

    private void ShowScoreboard_Click(object? sender, RoutedEventArgs e)
        {
            _viewModel?.ShowScoreboard();
        }

    private void BackToMenu_Click(object? sender, RoutedEventArgs e)
        {
            _viewModel?.BackToMenu();
        }

    private void RestartGame_Click(object? sender, RoutedEventArgs e)
        {
            _viewModel?.RestartGame();
            Focus();
        }

    private void ToggleMute_Click(object? sender, RoutedEventArgs e)
        {
            _viewModel?.ToggleMute();
        }

    private void Exit_Click(object? sender, RoutedEventArgs e)
        {
            Close();
        }
    
        
}