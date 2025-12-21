using System;
using System.Collections.Generic;
using System.Text;

namespace JournalApp.Shared
{
    public class ThemeState
    {
        public static bool IsDarkMode { get; set; } = false;

        public static string CurrentTheme =>
            IsDarkMode ? "dark-theme" : "light-theme";
    }
}
