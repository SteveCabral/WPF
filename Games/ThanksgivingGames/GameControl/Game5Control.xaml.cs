using System;
using System.Windows;
using System.Windows.Controls;

namespace ThanksgivingGames
{
    public partial class Game5Control : UserControl
    {
        private readonly IGameHost _host;

        public Game5Control(IGameHost host)
        {
            InitializeComponent();
            _host = host;
        }

        private void AddPoints_Click(object sender, RoutedEventArgs e)
        {
            _host.AddPoints("Alice", -50);  // Example: award 10 points to Alice
        }
    }
}
