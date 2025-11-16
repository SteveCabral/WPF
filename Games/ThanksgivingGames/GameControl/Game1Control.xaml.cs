using System;
using System.Windows;
using System.Windows.Controls;

namespace ThanksgivingGames
{
    public partial class Game1Control : UserControl
    {
        private readonly IGameHost _host;

        public Game1Control(IGameHost host)
        {
            InitializeComponent();
            _host = host;
        }

        private void AddPoints_Click(object sender, RoutedEventArgs e)
        {
            _host.AddPoints("Alice", 10);  // Example: award 10 points to Alice
        }
    }
}
