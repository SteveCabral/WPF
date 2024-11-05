using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PasswordGameApp
{
    public partial class MainWindow : Window
    {
        public static string SecretWord { get; private set; }
        private GameWindow gameWindow;

        public MainWindow()
        {
            InitializeComponent();
            SecretWord = "SUNSHINE"; // Set a sample secret word
        }

        private void SubmitClueButton_Click(object sender, RoutedEventArgs e)
        {
            if (gameWindow == null)
            {
                gameWindow = new GameWindow();
                gameWindow.Show();
            }

            string clue = ClueTextBox.Text.Trim();
            if (!string.IsNullOrEmpty(clue))
            {
                gameWindow.DisplayClue(clue);
                ClueTextBox.Clear();
            }
        }

        private void RevealSecretButton_Click(object sender, RoutedEventArgs e)
        {
            SecretWordDisplay.Text = $"Secret Word: {SecretWord}";
            SecretWordDisplay.Visibility = Visibility.Visible;
        }
    }
}