using QRCoder;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using ThanksgivingGames.WebServer;
using ThanksgivingGames.Models;

namespace ThanksgivingGames
{
    public partial class MainWindow : Window, INotifyPropertyChanged, IGameHost
    {
        public ObservableCollection<Player> Players { get; set; }
        private bool _isDarkMode = true;
        private WebHostServer _server;


        private string _currentGameTitle;
        public string CurrentGameTitle
        {
            get => _currentGameTitle;
            set { _currentGameTitle = value; OnPropertyChanged(nameof(CurrentGameTitle)); }
        }

        private object _currentGameContent;
        public object CurrentGameContent
        {
            get => _currentGameContent;
            set { _currentGameContent = value; OnPropertyChanged(nameof(CurrentGameContent)); }
        }

        public ICommand SelectGameCommand { get; }

        public MainWindow()
        {
            InitializeComponent();

            var cfg = ConfigLoader.Load("game_config.json");

            Players = new ObservableCollection<Player>
            {
                new Player { Name = "Alice", Points = 120 },
                new Player { Name = "Bob", Points = 90 },
                new Player { Name = "Charlie", Points = 130 },
                new Player { Name = "Diana", Points = 75 }
            };

            SelectGameCommand = new RelayCommand(SelectGame);

            DataContext = this;

            foreach (var p in Players)
                p.PropertyChanged += Player_PropertyChanged;

            UpdateRankings();

            CurrentGameTitle = "Select a Game";
            CurrentGameContent = new TextBlock
            {
                Text = "No game selected.",
                Foreground = System.Windows.Media.Brushes.Gray,
                FontSize = 22,
                HorizontalAlignment = HorizontalAlignment.Center,
                Margin = new Thickness(0, 50, 0, 0)
            };

        }

        private void SelectGame(object parameter)
        {
            if (parameter is string gameTitle)
            {
                CurrentGameTitle = gameTitle;

                // Load the corresponding UserControl
                switch (gameTitle)
                {
                    case "Game 1": CurrentGameContent = new Game1Control(this); break;
                    case "Game 2": CurrentGameContent = new Game2Control(this); break;
                    case "Game 3": CurrentGameContent = new Game3Control(this); break;
                    case "Game 4": CurrentGameContent = new Game4Control(this); break;
                    case "Game 5": CurrentGameContent = new Game5Control(this); break;
                }
            }
        }

        private void Player_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(Player.Points))
                UpdateRankings();
        }

        private void UpdateRankings()
        {
            if (Players == null || Players.Count == 0)
                return;

            var ranked = Players
                .OrderByDescending(p => p.Points)
                .Select((p, index) => { p.Place = index + 1; return p; })
                .ToList();

            // Rebuild the Players collection
            Players.Clear();
            foreach (var p in ranked)
                Players.Add(p);

            // Notify the UI that data changed
            PlayerListView.Items.Refresh();
        }


        public void AddPoints(string playerName, int points)
        {
            var player = Players.FirstOrDefault(p => p.Name.Equals(playerName, StringComparison.OrdinalIgnoreCase));
            if (player != null)
            {
                player.Points += points;
                UpdateRankings();
            }
        }

        public void DeductPoints(string playerName, int points)
        {
            var player = Players.FirstOrDefault(p => p.Name.Equals(playerName, StringComparison.OrdinalIgnoreCase));
            if (player != null)
            {
                player.Points -= points;
                UpdateRankings();
            }
        }

        private void IncrementPoints_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is Player player)
            {
                player.Points++;
                UpdateRankings();
            }
        }

        private void DecrementPoints_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is Player player)
            {
                if (player.Points > 0)
                    player.Points--;

                UpdateRankings();
            }
        }


        private void AddPlayer_Click(object sender, RoutedEventArgs e)
        {
            string name = NewPlayerNameBox.Text.Trim();

            if (string.IsNullOrEmpty(name))
                return;

            // Prevent duplicate player names
            if (Players.Any(p => p.Name.Equals(name, StringComparison.OrdinalIgnoreCase)))
            {
                MessageBox.Show("Player already exists.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var newPlayer = new Player { Name = name, Points = 0 };
            newPlayer.PropertyChanged += Player_PropertyChanged;
            Players.Add(newPlayer);

            NewPlayerNameBox.Text = ""; // Clear input
            UpdateRankings();           // Resort and re-rank
        }

        private void RemovePlayer_Click(object sender, RoutedEventArgs e)
        {
            if (PlayerListView.SelectedItem is Player selectedPlayer)
            {
                if (MessageBox.Show($"Remove {selectedPlayer.Name}?",
                                    "Confirm Remove",
                                    MessageBoxButton.YesNo,
                                    MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    Players.Remove(selectedPlayer);
                    UpdateRankings();
                }
            }
            else
            {
                MessageBox.Show("Select a player to remove.", "Info",
                                MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void ResetScores_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (Players == null || Players.Count == 0)
                {
                    MessageBox.Show("No players to reset.", "Info",
                                    MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }

                var confirm = MessageBox.Show("Reset all scores to 0?",
                                              "Confirm Reset",
                                              MessageBoxButton.YesNo,
                                              MessageBoxImage.Question);

                if (confirm == MessageBoxResult.Yes)
                {
                    // Reset scores in a stable loop
                    foreach (var player in Players.ToList())
                    {
                        if (player != null)
                            player.Points = 0;
                    }

                    // Refresh the leaderboard safely
                    UpdateRankings();

                    MessageBox.Show("All scores reset.", "Info",
                                    MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error resetting scores:\n{ex.Message}",
                                "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string name)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        private void ToggleTheme_Click(object sender, RoutedEventArgs e)
        {
            _isDarkMode = !_isDarkMode;

            var newTheme = new ResourceDictionary();
            string themePath = _isDarkMode
                ? "Themes/DarkTheme.xaml"
                : "Themes/LightTheme.xaml";

            newTheme.Source = new Uri(themePath, UriKind.Relative);

            // Replace the first merged dictionary (the current theme)
            Application.Current.Resources.MergedDictionaries.Clear();
            Application.Current.Resources.MergedDictionaries.Add(newTheme);
        }


        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            _server = new WebHostServer(5000);

            try
            {
                await _server.StartAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error starting server: {ex.Message}");
                return;
            }

            // Build the URL
            string url = $"http://{_server.LocalAddress}:{_server.Port}/";

            // Show QR Code
            GenerateQRCode(url);
        }


        private void GenerateQRCode(string url)
        {
            QRCodeGenerator qrGenerator = new QRCodeGenerator();
            QRCodeData qrCodeData = qrGenerator.CreateQrCode(url, QRCodeGenerator.ECCLevel.Q);
            QRCode qrCode = new QRCode(qrCodeData);

            using (Bitmap qrBitmap = qrCode.GetGraphic(20))
            {
                using (MemoryStream memory = new MemoryStream())
                {
                    qrBitmap.Save(memory, System.Drawing.Imaging.ImageFormat.Png);
                    memory.Position = 0;

                    BitmapImage bitmapImage = new BitmapImage();
                    bitmapImage.BeginInit();
                    bitmapImage.StreamSource = memory;
                    bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                    bitmapImage.EndInit();

                    QrCodeImage.Source = bitmapImage;
                }
            }
        }

    }

    public class Player : INotifyPropertyChanged
    {
        private string _name;
        private int _points;
        private int _place;

        public string Name { get => _name; set { _name = value; OnPropertyChanged(nameof(Name)); } }
        public int Points { get => _points; set { _points = value; OnPropertyChanged(nameof(Points)); } }
        public int Place { get => _place; set { _place = value; OnPropertyChanged(nameof(Place)); } }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }

    public class RelayCommand : ICommand
    {
        private readonly Action<object> _execute;
        private readonly Func<object, bool> _canExecute;

        public RelayCommand(Action<object> execute, Func<object, bool> canExecute = null)
        {
            _execute = execute;
            _canExecute = canExecute;
        }

        public bool CanExecute(object parameter) => _canExecute == null || _canExecute(parameter);
        public void Execute(object parameter) => _execute(parameter);
        public event EventHandler CanExecuteChanged { add { } remove { } }
    }

    public interface IGameHost
    {
        ObservableCollection<Player> Players { get; }
        void AddPoints(string playerName, int points);
        void DeductPoints(string playerName, int points);
    }

    public static class ConfigLoader
    {
        public static GameConfigRoot Load(string path)
        {
            var json = File.ReadAllText(path);
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            return JsonSerializer.Deserialize<GameConfigRoot>(json, options);
        }
    }
}