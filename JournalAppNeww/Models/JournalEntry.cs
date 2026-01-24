using System;
using System.Collections.Generic;
using System.Text;

using SQLite;

namespace JournalAppNeww.Models
{
    public class JournalEntry
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public DateTime Date { get; set; } // Only date part, one entry per day

        public string Title { get; set; } = string.Empty;

        public string Content { get; set; } = string.Empty; // Rich text HTML from Quill

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }

        public int WordCount { get; set; }
    }
}