using JournalApp.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace JournalApp.Shared
{
    public static class Storage
    {
        public static List<JournalEntry> Entries { get; } = new();
    }
}
