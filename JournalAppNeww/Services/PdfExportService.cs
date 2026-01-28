using System;
using System.Collections.Generic;
using System.Text;
using JournalAppNeww.Models;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using Colors = QuestPDF.Helpers.Colors;
using IContainer = QuestPDF.Infrastructure.IContainer;

namespace JournalAppNeww.Services
{
    public class PdfExportService
    {
        private readonly DatabaseService _databaseService;

        public PdfExportService(DatabaseService databaseService)
        {
            _databaseService = databaseService;
            QuestPDF.Settings.License = LicenseType.Community;
        }

        public async Task<byte[]> ExportEntriesToPdfAsync(DateTime startDate, DateTime endDate, bool includeMoods, bool includeTags)
        {
            var allEntries = await _databaseService.GetAllEntriesAsync();
            var entries = allEntries
                .Where(e => e.Date >= startDate && e.Date <= endDate)
                .OrderBy(e => e.Date)
                .ToList();

            // Load tags if needed
            Dictionary<int, List<Tag>> entryTags = new Dictionary<int, List<Tag>>();
            if (includeTags)
            {
                foreach (var entry in entries)
                {
                    var tags = await _databaseService.GetTagsForEntryAsync(entry.Id);
                    entryTags[entry.Id] = tags;
                }
            }

            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(2, Unit.Centimetre);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(11).FontFamily("Arial"));

                    page.Header()
                        .Text($"Journal Export - {startDate:MMM dd, yyyy} to {endDate:MMM dd, yyyy}")
                        .SemiBold().FontSize(20).FontColor(Colors.Blue.Darken2);

                    page.Content()
                        .PaddingVertical(1, Unit.Centimetre)
                        .Column(column =>
                        {
                            column.Spacing(15);

                            if (!entries.Any())
                            {
                                column.Item().Text("No entries found in this date range.").FontSize(14);
                            }
                            else
                            {
                                foreach (var entry in entries)
                                {
                                    column.Item().Element(c => CreateEntryBlock(c, entry, includeMoods, includeTags, entryTags));
                                }
                            }
                        });

                    page.Footer()
                        .AlignCenter()
                        .Text(x =>
                        {
                            x.Span("Page ");
                            x.CurrentPageNumber();
                            x.Span(" of ");
                            x.TotalPages();
                        });
                });
            });

            return document.GeneratePdf();
        }

        private void CreateEntryBlock(IContainer container, JournalEntry entry, bool includeMoods, bool includeTags, Dictionary<int, List<Tag>> entryTags)
        {
            container.Border(1).BorderColor(Colors.Grey.Lighten2).Padding(15).Column(column =>
            {
                column.Spacing(8);

                // Date and Title
                column.Item().Row(row =>
                {
                    row.RelativeItem().Column(col =>
                    {
                        col.Item().Text(entry.Date.ToString("dddd, MMMM dd, yyyy"))
                            .FontSize(14).SemiBold().FontColor(Colors.Blue.Darken1);

                        if (!string.IsNullOrEmpty(entry.Title))
                        {
                            col.Item().Text(entry.Title)
                                .FontSize(16).Bold().FontColor(Colors.Black);
                        }
                    });

                    row.ConstantItem(100).AlignRight().Text($"{entry.WordCount} words")
                        .FontSize(10).FontColor(Colors.Grey.Darken1);
                });

                // Mood
                if (includeMoods && !string.IsNullOrEmpty(entry.PrimaryMood))
                {
                    column.Item().Row(row =>
                    {
                        row.AutoItem().Text("Mood: ").FontSize(10).SemiBold();
                        row.AutoItem().Text(entry.PrimaryMood).FontSize(10).FontColor(Colors.Blue.Medium);

                        if (!string.IsNullOrEmpty(entry.SecondaryMoods))
                        {
                            var secondaryMoods = string.Join(", ", entry.SecondaryMoods.Split(','));
                            row.AutoItem().Text($" (also: {secondaryMoods})").FontSize(9).FontColor(Colors.Grey.Darken1);
                        }
                    });
                }

                // Tags
                if (includeTags && entryTags.ContainsKey(entry.Id) && entryTags[entry.Id].Any())
                {
                    column.Item().Row(row =>
                    {
                        row.AutoItem().Text("Tags: ").FontSize(10).SemiBold();
                        row.AutoItem().Text(string.Join(", ", entryTags[entry.Id].Select(t => t.Name)))
                            .FontSize(10).FontColor(Colors.Green.Darken1);
                    });
                }

                // Content
                column.Item().PaddingTop(5).Text(StripHtml(entry.Content))
                    .FontSize(11).LineHeight(1.4f);

                // Metadata
                column.Item().PaddingTop(8).BorderTop(1).BorderColor(Colors.Grey.Lighten3)
                    .PaddingTop(5).Text($"Created: {entry.CreatedAt:g} | Updated: {entry.UpdatedAt:g}")
                    .FontSize(8).FontColor(Colors.Grey.Medium);
            });
        }

        private string StripHtml(string html)
        {
            if (string.IsNullOrEmpty(html))
                return string.Empty;

            return System.Text.RegularExpressions.Regex.Replace(html, "<.*?>", " ")
                .Replace("&nbsp;", " ")
                .Trim();
        }
    }
}