using Microsoft.Identity.Client;
using System.ComponentModel.DataAnnotations.Schema;

namespace SporttiporssiPortal.Models
{
    public class UpdatePlayer
    {
        public int Id { get; set; }
        public int PlayerId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Role {  get; set; }
        public int Penalty2 { get; set; }
        public int Penalty10 { get; set; }
        public int Penalty20 { get; set; }
        public int Points { get; set; }
        public int Goals { get; set; }
        public int Assists { get; set; }
        public int Plus { get; set; }
        public int Minus { get; set; }
        public int PlusMinus { get; set; }
        public int PenaltyMinutes { get; set; }
        public int Shots { get; set; }
        public int FaceoffsTotal { get; set; }
        public int Saves { get; set; }
        public int GameWon { get; set; }
        public int GoalieShutout { get; set; }
        public int AllowedGoals { get; set; }
        public int TimeOnIce { get; set; }
        public int FaceoffsWon { get; set; }
        public int FaceoffsLost { get; set; }
        public double FaceoffWonPercentage { get; set; }
        public int BlockedShots { get; set; }
        public int AddGamePlayed { get; set; } = 1;
        public bool Win
        {
            get { return GameWon == 1; }
            set { GameWon = value ? 1 : 0; }
        }
        public bool Shutout
        {
            get { return GoalieShutout == 1; }
            set { GoalieShutout = value ? 1 : 0; }
        }
        public bool AddGame
        {
            get { return AddGamePlayed == 1; }
            set { AddGamePlayed = value ? 1 : 0; }
        }
        public string Name
        {
            get { return $"{FirstName} {LastName}"; }
        }

        // Property for user input in "minutes.seconds" format      
        public string TimeOnIceFormatted
        {
            get
            {
                int minutes = TimeOnIce / 60;
                int seconds = TimeOnIce % 60;
                return $"{minutes}.{seconds:D2}"; // Return in "minutes.seconds" format
            }
            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    var timeParts = value.Split('.');
                    if (timeParts.Length == 2 && int.TryParse(timeParts[0], out int minutes) && int.TryParse(timeParts[1], out int seconds))
                    {
                        TimeOnIce = (minutes * 60) + seconds;
                    }
                }
            }
        }
    }
}
