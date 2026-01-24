using JournalAppNeww.Models;
using SQLite;

namespace JournalAppNeww.Services
{
    public class DatabaseService
    {
        private SQLiteAsyncConnection _database;

        public DatabaseService()
        {
        }

        private async Task Init()
        {
            if (_database != null)
                return;

            var dbPath = Path.Combine(FileSystem.AppDataDirectory, "journal.db");
            _database = new SQLiteAsyncConnection(dbPath);
            await _database.CreateTableAsync<JournalEntry>();
        }

        public async Task<List<JournalEntry>> GetAllEntriesAsync()
        {
            await Init();
            return await _database.Table<JournalEntry>()
                .OrderByDescending(e => e.Date)
                .ToListAsync();
        }

        public async Task<JournalEntry?> GetEntryByDateAsync(DateTime date)
        {
            await Init();
            var dateOnly = date.Date;
            return await _database.Table<JournalEntry>()
                .Where(e => e.Date == dateOnly)
                .FirstOrDefaultAsync();
        }

        public async Task<int> SaveEntryAsync(JournalEntry entry)
        {
            await Init();
            entry.UpdatedAt = DateTime.Now;

            if (entry.Id != 0)
            {
                return await _database.UpdateAsync(entry);
            }
            else
            {
                entry.CreatedAt = DateTime.Now;
                entry.Date = entry.Date.Date; // Ensure only date part
                return await _database.InsertAsync(entry);
            }
        }

        public async Task<int> DeleteEntryAsync(JournalEntry entry)
        {
            await Init();
            return await _database.DeleteAsync(entry);
        }
    }
}