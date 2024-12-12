using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using Microsoft.VisualBasic;

namespace PE3
{
    public partial class MainWindow : Window
    {
        private DispatcherTimer timer = new DispatcherTimer();
        private string color1, color2, color3, color4;
        private int attempts;
        private DateTime startTime;
        private bool gameEnded = false;
        private bool isDebugMode = false;
        private int currentScore = 100;
        private List<GuessHistoryItem> guessHistory = new List<GuessHistoryItem>();
        private string[] highscores = new string[0];
        private string playerName;
        private int maxAttempts;

        public class GuessHistoryItem
        {
            public List<Brush> Colors { get; }
            public List<Brush> Borders { get; }

            public GuessHistoryItem(List<Brush> colors, List<Brush> borders)
            {
                Colors = colors;
                Borders = borders;
            }
        }

        public MainWindow()
        {
            InitializeComponent();
            var (playerNames, maxAttempts)= StartGame();
            this.maxAttempts = maxAttempts;
            attempts = 0;
            Title = $"Poging {attempts + 1}/{maxAttempts}";

            GenerateNewCode();

            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += Timer_Tick;
            StartCountdown();

            this.KeyDown += MainWindow_KeyDown;
            comboBox1.SelectionChanged += ComboBox_SelectionChanged;
            comboBox2.SelectionChanged += ComboBox_SelectionChanged;
            comboBox3.SelectionChanged += ComboBox_SelectionChanged;
            comboBox4.SelectionChanged += ComboBox_SelectionChanged;
        }
        private (List<string>, int) StartGame()
        {
            List<string> playerNames = new List<string>();
            bool addingPlayers = true;

            while (addingPlayers)
            {
                string playerName = Microsoft.VisualBasic.Interaction.InputBox("Voer je naam in:", "Naam Invoeren", "Speler");
                if (!string.IsNullOrEmpty(playerName))
                {
                    playerNames.Add(playerName); 
                }
                
                string response = Microsoft.VisualBasic.Interaction.InputBox("Wil je nog een speler toevoegen? (Ja/Nee)", "Voeg Speler Toe", "Ja");
                if (response.ToLower() == "nee")
                {
                    addingPlayers = false;
                }
            }

            int maxAttempts = 0;
            while (maxAttempts < 3 || maxAttempts > 20)
            {
                string input = Microsoft.VisualBasic.Interaction.InputBox("Voer het aantal pogingen in (tussen 3 en 20):", "Start Nieuw Spel", "10");
                if (int.TryParse(input, out maxAttempts) && maxAttempts >= 3 && maxAttempts <= 20)
                {
                    break;
                }
                else
                {
                    MessageBox.Show("Voer een geldig getal tussen 3 en 20 in.", "Foutmelding", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            return (playerNames, maxAttempts);
        }

        private void closeMenu_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void MainWindow_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.F12 && (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
            {
                ToggleDebug();
            }
        }
        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox comboBox = sender as ComboBox;
            Label correspondingLabel = null;


            if (comboBox == comboBox1)
                correspondingLabel = colorLabel1;
            else if (comboBox == comboBox2)
                correspondingLabel = colorLabel2;
            else if (comboBox == comboBox3)
                correspondingLabel = colorLabel3;
            else if (comboBox == comboBox4)
                correspondingLabel = colorLabel4;


            if (comboBox?.SelectedItem is ComboBoxItem selectedItem)
            {
                correspondingLabel.Background = new SolidColorBrush(GetColorFromString(selectedItem.Content.ToString()));
            }
        }

        private void GenerateNewCode()
        {
            Random rnd = new Random();
            color1 = RandomColor(rnd);
            color2 = RandomColor(rnd);
            color3 = RandomColor(rnd);
            color4 = RandomColor(rnd);
        }

        private string RandomColor(Random rnd)
        {
            int randomNumber = rnd.Next(1, 7);
            switch (randomNumber)
            {
                case 1: return "Blue";
                case 2: return "Red";
                case 3: return "White";
                case 4: return "Yellow";
                case 5: return "Orange";
                case 6: return "Green";
                default: return "";
            }
        }

        /// <summary> 
        /// Deze methode wordt aangeroepen wanneer het spel begint of wanneer een nieuwe beurt wordt gestart.
        /// De timer geeft de tijd weer tijdens de poging.
        /// </summary>
        private void StartCountdown()
        {
            startTime = DateTime.Now;
            timer.Start();
            timer.Interval = TimeSpan.FromMilliseconds(1);
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            TimeSpan interval = DateTime.Now.Subtract(startTime);
            timerTextBox.Text = interval.ToString(@"ss\:fff");

            if (interval.TotalSeconds >= 10)
            {
                StopCountdown();
                MessageBox.Show("Time's up! You lost your turn.");
                attempts++;
                if (attempts >= maxAttempts)
                Title = $"Poging {attempts + 1}/{maxAttempts}";
                {
                    MessageBox.Show("Game over! You've reached the maximum number of attempts.");
                    gameEnded = true;
                    timer.Stop();
                }
            }
        }

        /// <summary> 
        /// Deze methode stopt het aftellen van de timer
        /// </summary>
        private void StopCountdown()
        {
            timer.Stop();
        }

        private string GetComboBoxColor(ComboBox comboBox)
        {
            ComboBoxItem selectedItem = comboBox.SelectedItem as ComboBoxItem;
            return selectedItem?.Content.ToString();
        }

        private Color GetColorFromString(string color)
        {
            switch (color)
            {
                case "Blue": return Colors.Blue;
                case "Red": return Colors.Red;
                case "White": return Colors.White;
                case "Yellow": return Colors.Yellow;
                case "Orange": return Colors.Orange;
                case "Green": return Colors.Green;
                default: return Colors.Gray;
            }
        }

        private void CompareCodeWithLabels(string input1, string input2, string input3, string input4)
        {
            var label1Info = SameColor(colorLabel1, input1, color1, 1);
            var label2Info = SameColor(colorLabel2, input2, color2, 2);
            var label3Info = SameColor(colorLabel3, input3, color3, 3);
            var label4Info = SameColor(colorLabel4, input4, color4, 4);

            guessHistory.Add(new GuessHistoryItem(
                new List<Brush> { label1Info.Item1, label2Info.Item1, label3Info.Item1, label4Info.Item1 },
                new List<Brush> { label1Info.Item2, label2Info.Item2, label3Info.Item2, label4Info.Item2 }
            ));

            UpdateGuessHistoryUI();
        }

        private int CalculateScore(string comboBox1Color, string comboBox2Color, string comboBox3Color, string comboBox4Color)
        {
            int mistakes = 0;
            mistakes += CompareColor(comboBox1Color, color1);
            mistakes += CompareColor(comboBox2Color, color2);
            mistakes += CompareColor(comboBox3Color, color3);
            mistakes += CompareColor(comboBox4Color, color4);
            return mistakes;
        }


        private int CompareColor(string chosenColor, string correctColor)
        {
            if (chosenColor == correctColor)
            {
                return 0;
            }
            else if (color1 == chosenColor || color2 == chosenColor || color3 == chosenColor || color4 == chosenColor)
            {
                return 1;
            }
            else
            {
                return 2;
            }
        }

        private void UpdateGuessHistoryUI()
        {
            guessHistoryListBox.Items.Clear();

            foreach (var guess in guessHistory)
            {
                var guessStackPanel = new StackPanel
                {
                    Orientation = Orientation.Horizontal,
                    Margin = new Thickness(5)
                };

                for (int i = 0; i < 4; i++)
                {
                    var colorAndBorderBox = new Border
                    {
                        Background = guess.Colors[i],
                        BorderBrush = guess.Borders[i],
                        BorderThickness = new Thickness(2),
                        Width = 30,
                        Height = 30,
                        Margin = new Thickness(2)
                    };
                    guessStackPanel.Children.Add(colorAndBorderBox);
                }
                guessHistoryListBox.Items.Add(guessStackPanel);
            }
        }

        private (Brush, Brush) SameColor(Label label, string chosenColor, string correctColor, int position)
        {
            Brush chosenColorActual = GetBrushFromString(chosenColor);
            Brush correctColorActual = GetBrushFromString(correctColor);

            if (((SolidColorBrush)chosenColorActual).Color == Colors.Gray)
            {
                label.Background = chosenColorActual;
                label.BorderBrush = new SolidColorBrush(Colors.Transparent);
                label.BorderThickness = new Thickness(0);
                return (chosenColorActual, new SolidColorBrush(Colors.Transparent));
            }

            if (((SolidColorBrush)chosenColorActual).Color == ((SolidColorBrush)correctColorActual).Color)
            {
                label.Background = chosenColorActual;
                label.BorderThickness = new Thickness(5);
                label.BorderBrush = new SolidColorBrush(Colors.Red);
                return (chosenColorActual, new SolidColorBrush(Colors.Red));
            }
            else if (IsPartialMatch(((SolidColorBrush)chosenColorActual).Color, position))
            {
                label.Background = chosenColorActual;
                label.BorderBrush = new SolidColorBrush(Colors.Wheat);
                label.BorderThickness = new Thickness(3);
                return (chosenColorActual, new SolidColorBrush(Colors.Wheat));
            }
            else
            {
                label.Background = chosenColorActual;
                label.BorderBrush = new SolidColorBrush(Colors.Transparent);
                label.BorderThickness = new Thickness(0);
                return (chosenColorActual, new SolidColorBrush(Colors.Transparent));
            }
        }

        private bool IsPartialMatch(Color chosenColorActual, int position)
        {
            switch (position)
            {
                case 1:
                    return color2 == GetStringFromColor(chosenColorActual) || color3 == GetStringFromColor(chosenColorActual) || color4 == GetStringFromColor(chosenColorActual);
                case 2:
                    return color1 == GetStringFromColor(chosenColorActual) || color3 == GetStringFromColor(chosenColorActual) || color4 == GetStringFromColor(chosenColorActual);
                case 3:
                    return color1 == GetStringFromColor(chosenColorActual) || color2 == GetStringFromColor(chosenColorActual) || color4 == GetStringFromColor(chosenColorActual);
                case 4:
                    return color1 == GetStringFromColor(chosenColorActual) || color2 == GetStringFromColor(chosenColorActual) || color3 == GetStringFromColor(chosenColorActual);
                default:
                    return false;
            }
        }
        private string GetStringFromColor(Color color)
        {
            if (color == Colors.Blue) return "Blue";
            if (color == Colors.Red) return "Red";
            if (color == Colors.White) return "White";
            if (color == Colors.Yellow) return "Yellow";
            if (color == Colors.Orange) return "Orange";
            if (color == Colors.Green) return "Green";
            return string.Empty;
        }
        private void ValidateButton_Click(object sender, RoutedEventArgs e)
        {
            if (gameEnded) return;

            timer.Stop();
            StartCountdown();

            string comboBox1Color = GetComboBoxColor(comboBox1);
            string comboBox2Color = GetComboBoxColor(comboBox2);
            string comboBox3Color = GetComboBoxColor(comboBox3);
            string comboBox4Color = GetComboBoxColor(comboBox4);

            if (comboBox1Color == null || comboBox2Color == null || comboBox3Color == null || comboBox4Color == null)
            {
                MessageBox.Show("Please select colors from all ComboBoxes.");
                return;
            }

            int mistakes = CalculateScore(comboBox1Color, comboBox2Color, comboBox3Color, comboBox4Color);
            currentScore -= mistakes;

            scoreLabel.Content = $"Score: {currentScore}";

            CompareCodeWithLabels(comboBox1Color, comboBox2Color, comboBox3Color, comboBox4Color);

            attempts++;
            Title = $"Poging {attempts + 1}/{maxAttempts}";

            if (comboBox1Color == color1 && comboBox2Color == color2 && comboBox3Color == color3 && comboBox4Color == color4)
            {
                MessageBox.Show("Congratulations! You guessed the code correctly!");
                UpdateHighscores(playerName, currentScore);

                gameEnded = true;
                timer.Stop();
            }
            else if (attempts >= maxAttempts)
            {
                MessageBox.Show("Game over! You've reached the maximum number of attempts.");
                UpdateHighscores(playerName, currentScore);

                gameEnded = true;
                timer.Stop();
            }
        }

        private void ResetGame()
        {
            attempts = 0;
            currentScore = 100;
            gameEnded = false;
            GenerateNewCode();
            scoreLabel.Content = $"Score: {currentScore}";
            Title = $"Poging {attempts + 1}/ {maxAttempts}";
            timer.Start();
            StartCountdown();
            guessHistory.Clear();
            guessHistoryListBox.Items.Clear();
            timerTextBox.Clear();

            colorLabel1.Background = new SolidColorBrush(Colors.Transparent);
            colorLabel2.Background = new SolidColorBrush(Colors.Transparent);
            colorLabel3.Background = new SolidColorBrush(Colors.Transparent);
            colorLabel4.Background = new SolidColorBrush(Colors.Transparent);

            colorLabel1.BorderBrush = new SolidColorBrush(Colors.Transparent);
            colorLabel1.BorderThickness = new Thickness(0);

            colorLabel2.BorderBrush = new SolidColorBrush(Colors.Transparent);
            colorLabel2.BorderThickness = new Thickness(0);

            colorLabel3.BorderBrush = new SolidColorBrush(Colors.Transparent);
            colorLabel3.BorderThickness = new Thickness(0);

            colorLabel4.BorderBrush = new SolidColorBrush(Colors.Transparent);
            colorLabel4.BorderThickness = new Thickness(0);

            comboBox1.SelectedIndex = -1;
            comboBox2.SelectedIndex = -1;
            comboBox3.SelectedIndex = -1;
            comboBox4.SelectedIndex = -1;
        }

        /// <summary>
        /// Schakelt de debugmodus in of uit. Wanneer deze actief is, wordt de code getoond in de textBox.
        /// </summary>
        private void ToggleDebug()
        {
            isDebugMode = !isDebugMode;
            if (isDebugMode)
            {
                debugCodeTextBox.Visibility = Visibility.Visible;
                debugCodeTextBox.Text = $"{color1}, {color2}, {color3}, {color4}";
            }
            else
            {
                debugCodeTextBox.Visibility = Visibility.Collapsed;
            }
        }
        private Brush GetBrushFromString(string color)
        {
            switch (color)
            {
                case "Blue": return new SolidColorBrush(Colors.Blue);
                case "Red": return new SolidColorBrush(Colors.Red);
                case "White": return new SolidColorBrush(Colors.White);
                case "Yellow": return new SolidColorBrush(Colors.Yellow);
                case "Orange": return new SolidColorBrush(Colors.Orange);
                case "Green": return new SolidColorBrush(Colors.Green);
                default: return new SolidColorBrush(Colors.Gray);
            }
        }
        private void PlayAgain_click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Weet je zeker dat je een nieuw spel wilt starten?", "Nieuw Spel", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                ResetGame();
            }
        }
        private void UpdateHighscores(string playerName, int score)
        {
            string newHighscore = $"{playerName} / {attempts} pogingen / {score}/100";
            List<string> highscoreList = highscores.ToList();
            highscoreList.Add(newHighscore);
            highscoreList = highscoreList.Take(15).ToList();
            highscores = highscoreList.ToArray();
        }

        private void ShowHighscores()
        {
            string highscoreDisplay = "Highscores:\n\n";
            foreach (var score in highscores)
            {
                if (!string.IsNullOrEmpty(score))
                {
                    highscoreDisplay += score + "\n";
                }
            }
            MessageBox.Show(highscoreDisplay, "Highscores");
        }
        private void MenuHighscore_Click(object sender, RoutedEventArgs e)
        {
            ShowHighscores();
        }
    }
}
