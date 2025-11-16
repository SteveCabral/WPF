using System;
using System.Windows;
using System.Windows.Controls;

namespace ThanksgivingGames
{
    public partial class Game2Control : UserControl
    {
        private readonly IGameHost _host;

        public Game2Control(IGameHost host)
        {
            InitializeComponent();
            _host = host;
        }

        private void AddPoints_Click(object sender, RoutedEventArgs e)
        {
            _host.AddPoints("Bob", 20);  // Example: award 10 points to Alice
        }
    }
}
