using System;
using System.Collections.Generic;
using System.Text;

using JournalAppNeww.Models;
using Microsoft.JSInterop;

namespace JournalAppNeww.Services
{
    public class ThemeService
    {
        private readonly IJSRuntime _jsRuntime;
        private readonly DatabaseService _databaseService;
        private AppSettings? _currentSettings;

        public event Action? OnThemeChanged;

        public ThemeService(IJSRuntime jsRuntime, DatabaseService databaseService)
        {
            _jsRuntime = jsRuntime;
            _databaseService = databaseService;
        }

        public async Task InitializeAsync()
        {
            _currentSettings = await _databaseService.GetSettingsAsync();
            await ApplyThemeAsync();
        }

        public async Task SetThemeAsync(string theme)
        {
            if (_currentSettings == null)
                _currentSettings = await _databaseService.GetSettingsAsync();

            _currentSettings.Theme = theme;
            await _databaseService.SaveSettingsAsync(_currentSettings);
            await ApplyThemeAsync();
            OnThemeChanged?.Invoke();
        }

        public async Task SetCustomColorsAsync(string primary, string background, string sidebar, string text)
        {
            if (_currentSettings == null)
                _currentSettings = await _databaseService.GetSettingsAsync();

            _currentSettings.Theme = "custom";
            _currentSettings.PrimaryColor = primary;
            _currentSettings.BackgroundColor = background;
            _currentSettings.SidebarColor = sidebar;
            _currentSettings.TextColor = text;

            await _databaseService.SaveSettingsAsync(_currentSettings);
            await ApplyThemeAsync();
            OnThemeChanged?.Invoke();
        }

        public async Task ApplyThemeAsync()
        {
            if (_currentSettings == null)
                return;

            try
            {
                await _jsRuntime.InvokeVoidAsync("applyTheme",
                    _currentSettings.Theme,
                    _currentSettings.PrimaryColor,
                    _currentSettings.BackgroundColor,
                    _currentSettings.SidebarColor,
                    _currentSettings.TextColor);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error applying theme: {ex.Message}");
            }
        }

        public string GetCurrentTheme()
        {
            return _currentSettings?.Theme ?? "light";
        }

        public AppSettings? GetCurrentSettings()
        {
            return _currentSettings;
        }
    }
}