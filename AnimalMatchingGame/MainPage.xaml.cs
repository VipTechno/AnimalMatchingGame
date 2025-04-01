
using Microsoft.Maui.Controls;


namespace AnimalMatchingGame
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
            LoadLeaderboard();
        }

        private async void GoToLeaderboard_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new LeaderboardPage(leaderboardEntries));
        }

        private void StartGameButton_Clicked(object sender, EventArgs e)
        {
            StartGameButton.IsVisible = false;
            PlayAgainButton.IsVisible = false;
            GameArea.IsVisible = true;
        }

        private async void SubmitName_Clicked(object sender, EventArgs e)
        {
            string playerName = PlayerNameEntry.Text?.Trim();

            if (!string.IsNullOrEmpty(playerName))
            {
                 await DisplayAlert("Welcome!", $"Hello, {playerName}! Let's play!", "OK");

                LeaderboardList.ItemsSource = leaderboardEntries.Select(entry => $"{entry.PlayerName}: {entry.Time.ToString("0.0s")}").ToList();


                SubmitNameButton.IsVisible = false;
                PlayerNameEntry.IsVisible = false;
                LeaderboardList.IsVisible = false;
                LeaderboardListMain.IsVisible = false;
                EnterYourNameLabel.IsVisible = false;
                VehicleEmojis.IsVisible = false;


                PlayAgainButton_Clicked(sender, e);

                Dispatcher.StartTimer(TimeSpan.FromSeconds(.1), TimerTick);
            }
            else
            {
                DisplayAlert("Error", "Please enter a valid name.", "OK");
            }
        }
        



                private void PlayAgainButton_Clicked(object sender, EventArgs e)
                {
                    tenthsOfSecondsElapsed = 0;
                    TimeElapsed.Text = "Time elapsed: 0.0s";

                    isGameRunning = true;

                    AnimalButtons.IsVisible = true;
                    PlayAgainButton.IsVisible = false;
                    LeaderboardList.IsVisible = false;
                    LeaderboardListMain.IsVisible = false;

            List<string> animalEmoji = new List<string>
                    {
                        "🚌", "🚌", "🏎️", "🏎️", "🛥️", "🛥️", "🏍️", "🏍️", "🚚", "🚚", "🚑", "🚑", "🚒", "🚒", "🚗", "🚗", "🚁", "🚁"
                    };
                    foreach (var button in AnimalButtons.Children.OfType<Button>())
                    {
                        int index = Random.Shared.Next(animalEmoji.Count);
                        string nextEmoji = animalEmoji[index];
                        button.Text = nextEmoji;             
                        animalEmoji.RemoveAt(index);
                    }
                    Dispatcher.StartTimer(TimeSpan.FromSeconds(.1), TimerTick);
        }

        int tenthsOfSecondsElapsed = 0;
        int bestTime = int.MaxValue; // Initialize to a high value, so any time beats it initially.
        bool isGameRunning = true;

        List<LeaderboardEntry> leaderboardEntries = new List<LeaderboardEntry>();

        private void SaveLeaderboard()
        {
            var leaderboardData = string.Join(";", leaderboardEntries
                .Select(entry => $"{entry.PlayerName}|{entry.Time}"));
            Preferences.Set("Leaderboard", leaderboardData);
        }

        private void LoadLeaderboard()
        {
            var leaderboardData = Preferences.Get("Leaderboard", string.Empty);
            if (!string.IsNullOrEmpty(leaderboardData))
            {
                leaderboardEntries = leaderboardData.Split(';')
                    .Select(data =>
                    {
                        var parts = data.Split('|');
                        return new LeaderboardEntry
                        {
                            PlayerName = parts[0],
                            Time = double.Parse(parts[1])
                        };
                    })
                    .ToList();
            }
        }


        private bool TimerTick()
        {
          if (!isGameRunning) return false;

            tenthsOfSecondsElapsed++;
            TimeElapsed.Text = "Time elapsed: " + (tenthsOfSecondsElapsed / 10F).ToString("0.0s");

            return true;
        }

        Button lastClicked;
        bool findingMatch = false;
        int matchesFound;
        //This line of code keepers track of the number of matches found


        private void Button_Clicked(object sender, EventArgs e)
        {
            
            if (sender is Button buttonClicked)
            {
                if (!string.IsNullOrWhiteSpace(buttonClicked.Text) && (findingMatch == false))
                {
                    buttonClicked.BackgroundColor = Colors.Red;
                    lastClicked = buttonClicked;
                    findingMatch = true;
                    // These 3 lines of code keep track of the first button clicked and changes the color to red and keeps track of it
                }
                else
                {
                    if ((buttonClicked != lastClicked) && (buttonClicked.Text == lastClicked.Text) && (!String.IsNullOrWhiteSpace(buttonClicked.Text)))
                    {
                        matchesFound++;
                        lastClicked.Text = " ";
                        buttonClicked.Text = " ";
                    }
                    lastClicked.BackgroundColor = Colors.LightBlue;
                    buttonClicked.BackgroundColor = Colors.LightBlue;
                    findingMatch = false;
                    // these lines of code check if the second button clicked is the same as the first button clicked and if it is a match  it adds one to the matches found and changes the color of the buttons back to light blue
                }

            }
            if (matchesFound == 9)
            {
                if (tenthsOfSecondsElapsed < bestTime)
                {
                    bestTime = tenthsOfSecondsElapsed;
                    BestTime.Text = "Best Time: " + (bestTime / 10F).ToString("0.0s");
                }
                leaderboardEntries.Add(new LeaderboardEntry
                {
                    PlayerName = PlayerNameEntry.Text?.Trim(),
                    Time = tenthsOfSecondsElapsed / 10F
                });

                leaderboardEntries = leaderboardEntries.OrderBy(entry => entry.Time).Take(10).ToList();

                LeaderboardList.ItemsSource = leaderboardEntries
                    .Select(entry => $"{entry.PlayerName}: {entry.Time.ToString("0.0s")}")
                    .ToList();
                SaveLeaderboard();

                LeaderboardListMain.IsVisible = true;
                LeaderboardList.IsVisible = true;

                matchesFound = 0;
                AnimalButtons.IsVisible = false;
                PlayAgainButton.IsVisible = true;
                LeaderboardListMain.IsVisible = true;
                LeaderboardList.IsVisible = true;
                VehicleEmojis.IsVisible = true;

                isGameRunning = false;


            }
        }

    }
}
