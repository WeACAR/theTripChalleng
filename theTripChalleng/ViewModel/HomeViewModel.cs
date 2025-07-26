// ViewModels for Home
using theTripChalleng.Models;

namespace theTripChalleng.ViewModels
{
    // ViewModel for index page showing users leaderboard and criterias
    public class HomeViewModel
    {
        public List<User>? Leaderboard { get; set; }
        public List<Criterion>? Criterias { get; set; }
    }

    // ViewModel for assigned criteria containing user and criteria information
    public class AssignedCriteriaViewModel
    {
        public AssignedCritera? AssignedCriterias { get; set; }
        public User? User { get; set; }
        public Criterion? Criteria { get; set; }
    }

    //LeaderboardViewModel that contains a list of users for each category depending on their points history for that category using criteria
    public class LeaderboardViewModel
    {
        public List<User> Users { get; set; } = new List<User>();
        public string CategoryName { get; set; } = string.Empty;
    }
}