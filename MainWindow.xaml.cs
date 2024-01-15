using Microsoft.Data.SqlClient;
using System.Windows;
using Task_1.HW;
using Task_2.HW;
namespace Module_6._1_HW
{
    public partial class MainWindow : Window
    {
        private MyDbContext dbContext;
        public MainWindow()
        {
            InitializeComponent();
            dbContext = new MyDbContext();
            Loaded += MainWindow_Loaded;
            datePicker.SelectedDateChanged += DatePicker_SelectedDateChanged;
            teamComboBox.SelectionChanged += TeamComboBox_SelectionChanged;
        }
        private void ShowTopTeamsByPoints()
        {
            var topTeams = dbContext.Teams
                .OrderByDescending(t => (t.Wins * 3) + t.Draws)
                .Take(3)
                .ToList();
            MessageBox.Show($"Top 3 teams by points:\n" +
                $"{string.Join("\n", topTeams.Select((team, index) => $"{index + 1}. {team.Name} - {(team.Wins * 3) + team.Draws} points"))}");
        }
        private void ShowTopTeamByPoints()
        {
            var topTeam = dbContext.Teams
                .OrderByDescending(t => (t.Wins * 3) + t.Draws)
                .FirstOrDefault();
            MessageBox.Show($"Top team by points:\n" +
                $"{topTeam?.Name} - {(topTeam?.Wins * 3) + topTeam?.Draws} points");
        }
        private void ShowBottomTeamsByPoints()
        {
            var bottomTeams = dbContext.Teams
                .OrderBy(t => (t.Wins * 3) + t.Draws)
                .Take(3)
                .ToList();
            MessageBox.Show($"Bottom 3 teams by points:\n" +
                $"{string.Join("\n", bottomTeams.Select((team, index) => $"{index + 1}. {team.Name} - {(team.Wins * 3) + team.Draws} points"))}");
        }
        private void ShowBottomTeamByPoints()
        {
            var bottomTeam = dbContext.Teams
                .OrderBy(t => (t.Wins * 3) + t.Draws)
                .FirstOrDefault();
            MessageBox.Show($"Bottom team by points:\n" +
                $"{bottomTeam?.Name} - {(bottomTeam?.Wins * 3) + bottomTeam?.Draws} points");
        }
        private void ShowTopScorersByTeam(int teamId)
        {
            var topScorers = dbContext.Players
                .Where(p => (p.Match.Team1Id == teamId || p.Match.Team2Id == teamId) && p.Position == "Forward")
                .GroupBy(p => p.ScorerId)
                .Select(g => new
                {
                    ScorerId = g.Key,
                    Goals = g.Sum(p => p.Goals)
                })
                .OrderByDescending(g => g.Goals)
                .Take(3)
                .ToList();
            MessageBox.Show($"Top 3 scorers for Team {teamId}:\n" +
                $"{string.Join("\n", topScorers.Select((s, index) => $"{index + 1}. Scorer ID: {s.ScorerId}, Goals: {s.Goals}"))}");
        }
        private void ShowTopScorerByTeam(int teamId)
        {
            var topScorer = dbContext.Players
                .Where(p => (p.Match.Team1Id == teamId || p.Match.Team2Id == teamId) && p.Position == "Forward")
                .GroupBy(p => p.ScorerId)
                .Select(g => new
                {
                    ScorerId = g.Key,
                    Goals = g.Sum(p => p.Goals)
                })
                .OrderByDescending(g => g.Goals)
                .FirstOrDefault();
            MessageBox.Show($"Top scorer for Team {teamId}:\n" +
                $"Scorer ID: {topScorer?.ScorerId}, Goals: {topScorer?.Goals}");
        }
        private void ShowTopScorersOverall()
        {
            var topScorers = dbContext.Players
                .Where(p => p.Position == "Forward")
                .GroupBy(p => p.ScorerId)
                .Select(g => new
                {
                    ScorerId = g.Key,
                    Goals = g.Sum(p => p.Goals)
                })
                .OrderByDescending(g => g.Goals)
                .Take(3)
                .ToList();
            MessageBox.Show($"Top 3 overall scorers:\n" +
                $"{string.Join("\n", topScorers.Select((s, index) => $"{index + 1}. Scorer ID: {s.ScorerId}, Goals: {s.Goals}"))}");
        }
        private void ShowTopScorerOverall()
        {
            var topScorer = dbContext.Players
                .Where(p => p.Position == "Forward")
                .GroupBy(p => p.ScorerId)
                .Select(g => new
                {
                    ScorerId = g.Key,
                    Goals = g.Sum(p => p.Goals)
                })
                .OrderByDescending(g => g.Goals)
                .FirstOrDefault();
            MessageBox.Show($"Top overall scorer:\n" +
                $"Scorer ID: {topScorer?.ScorerId}, Goals: {topScorer?.Goals}");
        }
        private void GenerateRandomMatches(int numberOfMatches)
        {
            Random random = new Random();
            for (int i = 0; i < numberOfMatches; i++)
            {
                int team1Id = random.Next(1, 10);
                int team2Id;
                do
                {
                    team2Id = random.Next(1, 10);
                } while (team2Id == team1Id);
                int? goalsTeam1 = random.Next(0, 5);
                int? goalsTeam2 = random.Next(0, 5);
                int? scorerId = random.Next(1, 50);
                DateTime matchDate = DateTime.Now.AddDays(random.Next(-30, 30));
                string connectionString = "Data Source=LAPTOP-2;Initial Catalog=FootballTeamsDB;Integrated Security=True;Connect Timeout=30;Encrypt=False;Trust Server Certificate=False";
                string insertQuery = "INSERT INTO Matches (Team1Id, Team2Id, GoalsTeam1, GoalsTeam2, ScorerId, MatchDate) " +
                                     "VALUES (@Team1Id, @Team2Id, @GoalsTeam1, @GoalsTeam2, @ScorerId, @MatchDate)";
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    using (SqlCommand command = new SqlCommand(insertQuery, connection))
                    {
                        command.Parameters.AddWithValue("@Team1Id", team1Id);
                        command.Parameters.AddWithValue("@Team2Id", team2Id);
                        command.Parameters.AddWithValue("@GoalsTeam1", goalsTeam1 ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@GoalsTeam2", goalsTeam2 ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@ScorerId", scorerId ?? (object)DBNull.Value);
                        command.Parameters.AddWithValue("@MatchDate", matchDate);
                        connection.Open();
                        command.ExecuteNonQuery();
                    }
                }
            }
            LoadAllMatches();
        }
        private void ShowTopTeamsByGoalsScored()
        {
            var topTeams = dbContext.Teams
                .OrderByDescending(t => (t.Wins * 3) + t.Draws)
                .Take(3)
                .ToList();
            MessageBox.Show($"Top 3 teams by goals scored:\n" +
                $"{string.Join("\n", topTeams.Select((team, index) => $"{index + 1}. {team.Name} - {team.Wins * 3 + team.Draws} goals"))}");
        }
        private void ShowTopTeamByGoalsScored()
        {
            var topTeam = dbContext.Teams
                .OrderByDescending(t => (t.Wins * 3) + t.Draws)
                .FirstOrDefault();
            MessageBox.Show($"Top team by goals scored:\n" +
                $"{topTeam?.Name} - {topTeam?.Wins * 3 + topTeam?.Draws} goals");
        }
        private void ShowTopTeamsByGoalsConceded()
        {
            var topTeams = dbContext.Teams
                .OrderBy(t => t.Losses)
                .Take(3)
                .ToList();
            MessageBox.Show($"Top 3 teams by goals conceded:\n" +
                $"{string.Join("\n", topTeams.Select((team, index) => $"{index + 1}. {team.Name} - {team.Losses} goals conceded"))}");
        }
        private void ShowTopTeamByGoalsConceded()
        {
            var topTeam = dbContext.Teams
                .OrderBy(t => t.Losses)
                .FirstOrDefault();
            MessageBox.Show($"Top team by goals conceded:\n" +
                $"{topTeam?.Name} - {topTeam?.Losses} goals conceded");
        }
        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            LoadTeams();
            LoadAllMatches();
            ShowTopTeamsByGoalsScored();
            ShowTopTeamByGoalsScored();
            ShowTopTeamsByGoalsConceded();
            ShowTopTeamByGoalsConceded();
            ShowTopTeamsByPoints();
            ShowTopTeamByPoints();
            ShowBottomTeamsByPoints();
            ShowBottomTeamByPoints();
        }
        private void LoadTeams()
        {
            var teams = dbContext.Teams;
            teamComboBox.ItemsSource = teams.ToList();
        }
        private void LoadAllMatches()
        {
            var matches = dbContext.Matches;
            dataGrid.ItemsSource = matches.ToList();
        }
        private void DatePicker_SelectedDateChanged(object sender, EventArgs e)
        {
            DateTime selectedDate = datePicker.SelectedDate ?? DateTime.Now;
            var matchesOnSelectedDate = dbContext.Matches.Where(m => m.MatchDate.Date == selectedDate.Date).ToList();
            dataGrid.ItemsSource = matchesOnSelectedDate;
        }
        private void TeamComboBox_SelectionChanged(object sender, EventArgs e)
        {
            if (teamComboBox.SelectedItem != null)
            {
                var selectedTeam = (Team)teamComboBox.SelectedItem;
                var teamMatches = dbContext.Matches.Where(m => m.Team1Id == selectedTeam.TeamId || m.Team2Id == selectedTeam.TeamId).ToList();
                teamMatchesDataGrid.ItemsSource = teamMatches;
            }
        }
        private void FilterPlayersByDate(object sender, RoutedEventArgs e)
        {
            DateTime selectedDate = playersDatePicker.SelectedDate ?? DateTime.Now;
            playersDataGrid.ItemsSource = dbContext.Players.Where(p => p.Match.MatchDate.Date == selectedDate.Date).ToList();
        }
        private void AddEditMatch(object sender, RoutedEventArgs e)
        {
            int team1Id = int.Parse(team1IdTextBox.Text);
            int team2Id = int.Parse(team2IdTextBox.Text);
            int? goalsTeam1 = string.IsNullOrEmpty(goalsTeam1TextBox.Text) ? (int?)null : int.Parse(goalsTeam1TextBox.Text);
            int? goalsTeam2 = string.IsNullOrEmpty(goalsTeam2TextBox.Text) ? (int?)null : int.Parse(goalsTeam2TextBox.Text);
            int? scorerId = string.IsNullOrEmpty(scorerIdTextBox.Text) ? (int?)null : int.Parse(scorerIdTextBox.Text);
            DateTime matchDate = matchDatePicker.SelectedDate ?? DateTime.Now;
            string connectionString = "Data Source=LAPTOP-2;Initial Catalog=FootballTeamsDB;Integrated Security=True;Connect Timeout=30;Encrypt=False;Trust Server Certificate=False";
            string insertQuery = "INSERT INTO Matches (Team1Id, Team2Id, GoalsTeam1, GoalsTeam2, ScorerId, MatchDate) " +
                                 "VALUES (@Team1Id, @Team2Id, @GoalsTeam1, @GoalsTeam2, @ScorerId, @MatchDate)";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                using (SqlCommand command = new SqlCommand(insertQuery, connection))
                {
                    command.Parameters.AddWithValue("@Team1Id", team1Id);
                    command.Parameters.AddWithValue("@Team2Id", team2Id);
                    command.Parameters.AddWithValue("@GoalsTeam1", goalsTeam1 ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@GoalsTeam2", goalsTeam2 ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@ScorerId", scorerId ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@MatchDate", matchDate);
                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (dataGrid.SelectedItem != null)
            {
                var selectedMatch = (Match)dataGrid.SelectedItem;

                var matchToDelete = dbContext.Matches.FirstOrDefault(m => m.MatchId == selectedMatch.MatchId);

                if (matchToDelete != null)
                {
                    dbContext.Matches.Remove(matchToDelete);
                    dbContext.SaveChanges();
                    LoadAllMatches();
                }
            }
        }
    }
}