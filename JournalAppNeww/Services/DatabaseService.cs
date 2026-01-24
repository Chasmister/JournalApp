using SQLite;
using JournalAppNeww.Models;

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
            await _database.CreateTableAsync<Tag>();
            await _database.CreateTableAsync<EntryTag>();
        }

        // ===== JOURNAL ENTRIES =====
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
                entry.Date = entry.Date.Date;
                return await _database.InsertAsync(entry);
            }
        }

        public async Task<int> DeleteEntryAsync(JournalEntry entry)
        {
            await Init();
            // Delete associated entry tags first
            await _database.Table<EntryTag>()
                .DeleteAsync(et => et.JournalEntryId == entry.Id);
            return await _database.DeleteAsync(entry);
        }

        // ===== TAGS =====
        public async Task<List<Tag>> GetAllTagsAsync()
        {
            await Init();
            return await _database.Table<Tag>()
                .OrderBy(t => t.Name)
                .ToListAsync();
        }

        public async Task<Tag?> GetTagByNameAsync(string name)
        {
            await Init();
            return await _database.Table<Tag>()
                .Where(t => t.Name == name)
                .FirstOrDefaultAsync();
        }

        public async Task<int> SaveTagAsync(Tag tag)
        {
            await Init();

            if (tag.Id != 0)
            {
                return await _database.UpdateAsync(tag);
            }
            else
            {
                tag.CreatedAt = DateTime.Now;
                return await _database.InsertAsync(tag);
            }
        }

        public async Task<int> DeleteTagAsync(Tag tag)
        {
            await Init();
            // Delete associated entry tags first
            await _database.Table<EntryTag>()
                .DeleteAsync(et => et.TagId == tag.Id);
            return await _database.DeleteAsync(tag);
        }

        // ===== ENTRY TAGS (Many-to-Many) =====
        public async Task<List<Tag>> GetTagsForEntryAsync(int entryId)
        {
            await Init();
            var entryTags = await _database.Table<EntryTag>()
                .Where(et => et.JournalEntryId == entryId)
                .ToListAsync();

            var tags = new List<Tag>();
            foreach (var et in entryTags)
            {
                var tag = await _database.Table<Tag>()
                    .Where(t => t.Id == et.TagId)
                    .FirstOrDefaultAsync();
                if (tag != null)
                    tags.Add(tag);
            }

            return tags;
        }

        public async Task AddTagToEntryAsync(int entryId, int tagId)
        {
            await Init();
            // Check if already exists
            var existing = await _database.Table<EntryTag>()
                .Where(et => et.JournalEntryId == entryId && et.TagId == tagId)
                .FirstOrDefaultAsync();

            if (existing == null)
            {
                await _database.InsertAsync(new EntryTag
                {
                    JournalEntryId = entryId,
                    TagId = tagId
                });
            }
        }

        public async Task RemoveTagFromEntryAsync(int entryId, int tagId)
        {
            await Init();
            var entryTag = await _database.Table<EntryTag>()
                .Where(et => et.JournalEntryId == entryId && et.TagId == tagId)
                .FirstOrDefaultAsync();

            if (entryTag != null)
            {
                await _database.DeleteAsync(entryTag);
            }
        }

        public async Task ClearTagsForEntryAsync(int entryId)
        {
            await Init();
            await _database.Table<EntryTag>()
                .DeleteAsync(et => et.JournalEntryId == entryId);
        }
    }
}