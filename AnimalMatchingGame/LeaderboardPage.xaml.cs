namespace AnimalMatchingGame;

public partial class LeaderboardPage : ContentPage
{
	public LeaderboardPage(List<LeaderboardEntry> entries)
	{
		InitializeComponent();
		LeaderboardList.ItemsSource = entries;
	}
}