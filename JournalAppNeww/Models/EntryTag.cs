using System;
using System.Collections.Generic;
using System.Text;

using SQLite;

namespace JournalAppNeww.Models
{
    public class EntryTag
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public int JournalEntryId { get; set; }

        public int TagId { get; set; }
    }
}