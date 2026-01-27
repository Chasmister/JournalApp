using System;
using System.Collections.Generic;
using System.Text;

using SQLite;

namespace JournalAppNeww.Models
{
    public class AppSettings
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public string Theme { get; set; } = "light"; // light, dark, custom

        public bool PasswordEnabled { get; set; } = false;

        public string PasswordHash { get; set; } = string.Empty;

        // Custom theme colors (if theme is "custom")
        public string PrimaryColor { get; set; } = "#007bff";
        public string BackgroundColor { get; set; } = "#ffffff";
        public string SidebarColor { get; set; } = "#2c3e50";
        public string TextColor { get; set; } = "#333333";
    }
}