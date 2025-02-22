using System;

namespace PopulationGame.Models
{
    public class UserGameLog
    {
        public int LogId { get; set; }
        public int UserId { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public int Streak { get; set; }
        public int Guesses { get; set; }
        public string Difficulty { get; set; }
    }
}

