using SQLite;

namespace JournalAppNeww.Models
{
    public class JournalEntry
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public DateTime Date { get; set; }

        public string Title { get; set; } = string.Empty;

        public string Content { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }

        public int WordCount { get; set; }

        // Mood tracking - Primary mood is required
        public string PrimaryMood { get; set; } = string.Empty;

        // Secondary moods - stored as comma-separated string (max 2)
        public string SecondaryMoods { get; set; } = string.Empty;
    }
}