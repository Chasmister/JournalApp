using System;
using System.Collections.Generic;
using System.Text;

namespace JournalApp.Models
{
    public class JournalEntry
    {
       
        public DateTime EntryDate { get; set; } = DateTime.Today;
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        
    }
}
