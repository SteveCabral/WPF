using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace ThanksgivingGames.Models
{
    // Config classes (deserialize from JSON)
    public class GameConfigRoot
    {
        public List<GameConfig> Games { get; set; }
    }

    public class GameConfig
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public List<CardConfig> Cards { get; set; }
    }

    public class CardConfig
    {
        public int CardNumber { get; set; }
        public string Title { get; set; }
        public int Points { get; set; }
        public string Image { get; set; }
    }

    // Runtime models & viewmodels
    public class Card : INotifyPropertyChanged
    {
        public int CardNumber { get; set; }
        public string Title { get; set; }
        public int Points { get; set; }
        public string Image { get; set; }

        private bool _isRevealed;
        public bool IsRevealed { get => _isRevealed; set { _isRevealed = value; OnPropertyChanged(nameof(IsRevealed)); } }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string n) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(n));
    }

    public class PlayerGameState : INotifyPropertyChanged
    {
        public string Name { get; set; }
        private int _score;
        public int Score { get => _score; set { _score = value; OnPropertyChanged(nameof(Score)); } }

        private PlayerState _state = PlayerState.NotPlayed;
        public PlayerState State { get => _state; set { _state = value; OnPropertyChanged(nameof(State)); } }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string n) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(n));
    }

    public enum PlayerState
    {
        NotPlayed,
        Current,
        Played,
        Negative
    }
}
