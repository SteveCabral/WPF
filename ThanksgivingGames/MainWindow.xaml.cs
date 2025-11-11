using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace ThanksgivingGames
{
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        public ObservableCollection<Player> Players { get; set; }

        private string _currentGameTitle;
        public string CurrentGameTitle
        {
            get => _currentGameTitle;
            set
            {
                _currentGameTitle = value;
                OnPropertyChanged(nameof(CurrentGameTitle));
            }
        }

        public ICommand SelectGameCommand { get; }

        public MainWindow()
        {
            InitializeComponent();

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

            // Default game title
            CurrentGameTitle = "Select a Game";

            // Example timer (optional)
            var timer = new System.Windows.Threading.DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(3);
            timer.Tick += (s, e) =>
            {
                Players[1].Points += 50;
                timer.Stop();
            };
            timer.Start();
        }

        private void SelectGame(object parameter)
        {
            if (parameter is string gameTitle)
                CurrentGameTitle = gameTitle;
        }

        private void Player_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(Player.Points))
                UpdateRankings();
        }

        private void UpdateRankings()
        {
            var sorted = Players
                .OrderByDescending(p => p.Points)
                .ThenBy(p => p.Name)
                .ToList();

            for (int i = 0; i < sorted.Count; i++)
                sorted[i].Place = i + 1;

            for (int i = 0; i < sorted.Count; i++)
            {
                var existing = Players.IndexOf(sorted[i]);
                if (existing != i)
                    Players.Move(existing, i);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
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
        protected void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    // Simple command class
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
}
