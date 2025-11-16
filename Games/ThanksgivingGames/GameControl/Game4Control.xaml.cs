using System;
using System.Windows;
using System.Windows.Controls;

namespace ThanksgivingGames
{
    public partial class Game4Control : UserControl
    {
        private readonly IGameHost _host;

        public Game4Control(IGameHost host)
        {
            InitializeComponent();
            _host = host;
        }

        private void AddPoints_Click(object sender, RoutedEventArgs e)
        {
            _host.AddPoints("Diana", 70);  // Example: award 10 points to Alice
        }
    }
}
