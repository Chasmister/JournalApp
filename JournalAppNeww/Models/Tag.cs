using System;
using System.Collections.Generic;
using System.Text;

using SQLite;

namespace JournalAppNeww.Models
{
    public class Tag
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string Color { get; set; } = "#007bff"; // Default color

        public DateTime CreatedAt { get; set; }
    }
}
