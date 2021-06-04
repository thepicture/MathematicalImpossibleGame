using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace MathematicalImpossibleGame
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly Random random = new Random();
        public MainWindow()
        {
            InitializeComponent();

            int cellsCount = 3 * random.Next(8, 12);
            for (int i = 0; i < cellsCount; i++)
            {
                Gameboard.Children.Add(new Button
                {
                    Name = "Button" + i,
                    Width = 50,
                    Background = Brushes.White,
                    Height = 50,
                    FontSize = 30,
                    VerticalContentAlignment = VerticalAlignment.Center,
                    HorizontalContentAlignment = HorizontalAlignment.Center
                });
            }
            foreach (Button button in Gameboard.Children)
            {
                button.Click += Button_Click;
            }
        }

        readonly List<Button> usedButtons = new List<Button>();
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            ChangePlayer.IsEnabled = true;
            Button button = sender as Button;
            if (button.Content != null)
            {
                return;
            }
            if (usedButtons.Count >= 2)
            {
                usedButtons.ElementAt(0).Content = null;
                usedButtons.RemoveAt(0);
            }
            button.Content = "X";
            if (Gameboard.Children.Cast<Button>().Where(b => b.Content == null).Count() == 0)
            {
                Table.Text = "YOU WIN";
                RestartButton.Visibility = Visibility.Visible;
                ChangePlayer.Visibility = Visibility.Hidden;
                return;
            }
            usedButtons.Add(button);
        }

        private async void ChangePlayer_Click(object sender, RoutedEventArgs e)
        {
            if (RulesButton.Visibility == Visibility.Visible)
            {
                RulesButton.Visibility = Visibility.Hidden;
            }
            ChangePlayer.IsEnabled = false;
            IEnumerable<Button> buttonList = Gameboard.Children.Cast<Button>().Where(b => b.Content
            == null).OrderBy(a => random.Next()).Take(3 - usedButtons.Count());
            foreach (Button currentButton in Gameboard.Children)
            {
                currentButton.IsEnabled = false;
            }
            Table.Text = "OPPONENT'S MOVE";
            foreach (Button currentButton in buttonList)
            {
                await Task.Delay(500);
                currentButton.Content = "O";
                if (Gameboard.Children.Cast<Button>().Where(b => b.Content == null).Count() == 0)
                {
                    Table.Text = "YOU LOSE";
                    RestartButton.Visibility = Visibility.Visible;
                    ChangePlayer.Visibility = Visibility.Hidden;
                    return;
                }
            }
            await Task.Delay(500);
            foreach (Button currentButton in Gameboard.Children)
            {
                currentButton.IsEnabled = true;
            }
            usedButtons.Clear();
            Table.Text = "YOUR MOVE";
        }

        private void RestartButton_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start(Application.ResourceAssembly.Location);
            Application.Current.Shutdown();
        }

        private void RulesButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Use cells to place one or two X'es.\nIf you do not have any avalaible cells during your move, you lose.");
        }

        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            App.Current.Shutdown();
        }
    }
}
