using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace PasswordGameApp
{
    public partial class GameWindow : Window
    {
        public GameWindow()
        {
            InitializeComponent();
        }

        public void DisplayClue(string clue)
        {
            ClueDisplay.Text = $"Clue: {clue}";
            ResultDisplay.Text = ""; // Clear previous result
        }

        private void SubmitGuessButton_Click(object sender, RoutedEventArgs e)
        {
            string playerGuess = GuessTextBox.Text.Trim();
            if (!string.IsNullOrEmpty(playerGuess))
            {
                if (string.Equals(playerGuess, MainWindow.SecretWord, StringComparison.OrdinalIgnoreCase))
                {
                    ResultDisplay.Text = "Correct! You guessed the word!";
                }
                else
                {
                    ResultDisplay.Text = "Incorrect. Try again!";
                }
                GuessTextBox.Clear();
            }
        }
    }
}
