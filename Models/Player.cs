namespace SporttiporssiPortal.Models
{
    public class Player
    {
        public int Id { get; set; }
        public int PlayerId { get; set; }
        public int TeamId { get; set; }
        public string? TeamName { get; set; }
        public string? TeamShortName { get; set; }
        public string? Role { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Nationality { get; set; }
        public string? Tournament { get; set; }
        public string? PictureUrl { get; set; }
        public bool Injured { get; set; }
        public int Jersey { get; set; }
        public bool Goalkeeper { get; set; }     
        public int PlayedGames { get; set; }
        public bool Suspended { get; set; }
        public int? TimeOnIce { get; set; }
        public int? Goals { get; set; }
        public int? Assists { get; set; }
        public int? Points { get; set; }
        public int? Plus { get; set; }
        public int? Minus { get; set; }
        public int? PlusMinus { get; set; }
        public int? PenaltyMinutes { get; set; }
        public int? Penalty2 { get; set; }
        public int? Penalty10 { get; set; }
        public int? Penalty20 { get; set; }
        public int WinningGoals { get; set; }
        public int? Shots { get; set; }
        public int? Saves { get; set; }
        public int? GoalieShutout { get; set; }
        public int? AllowedGoals { get; set; }
        public int? FaceoffsWon { get; set; }
        public int? FaceoffsLost { get; set; }
        public string? TimeOnIceAvg { get; set; }
        public double? FaceoffWonPercentage { get; set; }
        public double? ShotPercentage { get; set; }
        public int? FaceoffsTotal { get; set; }
        public DateTime LastUpdated { get; set; }
        public int? FTP { get; set; }
        public int? Price { get; set; }
        public int? PlayerOwned { get; set; }
        public int? BlockedShots { get; set; }
        public int? GameWon { get; set; }
        public Guid Serie { get; set; }
    }
}
