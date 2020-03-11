﻿using Bibles.DataResources.Models;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using GeneralExtensions;
using System;
using System.Windows.Threading;

namespace Bibles.DataResources
{
    public class InitializeData
    {
        private string[] bibleNames = new string[] { "Afrikaans 1933-56 Hersien", "English King James Version", "German Luther 1545", "Xhosa" };

        public delegate void InitialDataLoadCompletedEvent(object sender, string message, bool completed, Exception error);

        public event InitialDataLoadCompletedEvent InitialDataLoadCompleted;

        public async void LoadEmbeddedBibles(Dispatcher dispatcher)
        {
            Task<List<BibleModel>> loadedBiles = BiblesData.Database.GetBibles();

            if (loadedBiles.Result.Count >= bibleNames.Length)
            {
                dispatcher.Invoke(() => 
                { 
                    this.InitialDataLoadCompleted?.Invoke(this, string.Empty, true, null);
                });

                return;
            }

            await Task.Run(() => 
            {
                try
                {
                    foreach (string bible in bibleNames)
                    {
                        BibleModel bibleModel = loadedBiles.Result.FirstOrDefault(l => l.BibleName == bible);

                        if (bibleModel == null)
                        {
                            bibleModel = new BibleModel
                            {
                                BiblesId = 0,
                                BibleName = bible
                            };

                            Task<int> result = BiblesData.Database.InsertBible(bibleModel);

                            bibleModel.BiblesId = result.Result;
                        }

                        dispatcher.Invoke(() => 
                        {
                            this.InitialDataLoadCompleted?.Invoke(this, $"Loading...{bible}", false, null);
                        });

                        this.LoadBibleVerses(bibleModel);
                    }
                }
                catch (Exception err)
                {
                    dispatcher.Invoke(() =>
                    {
                        this.InitialDataLoadCompleted?.Invoke(this, string.Empty, false, err);
                    });
                }

                dispatcher.Invoke(() =>
                {
                    this.InitialDataLoadCompleted?.Invoke(this, string.Empty, true, null);
                });
            });
        }

        private void LoadBibleVerses(BibleModel bibleModel)
        {
            string bibleFormatName = bibleModel.BibleName
                .Replace(' ', '_')
                .Replace('-', '_');

            var bible = typeof(Properties.Resources)
              .GetProperties(BindingFlags.Static | BindingFlags.NonPublic |
                             BindingFlags.Public)
              .Where(p => p.PropertyType == typeof(string) && p.Name == bibleFormatName)
              .Select(x => new { Bible = x.GetValue(null, null) })
              .FirstOrDefault();

            List<string> verses = bible.Bible
                .ParseToString()
                .Split(new char[] { '\n', '\r' }, System.StringSplitOptions.RemoveEmptyEntries)
                .ToList();


            List<BibleVerseModel> bulkList = new List<BibleVerseModel>();

            foreach (string verseLine in verses)
            {
                int breakIndex = verseLine.LastIndexOf("||") + 2;

                string verseKey = verseLine.Substring(0, breakIndex);

                string verseText = verseLine.Substring(breakIndex, verseLine.Length - breakIndex);

                BibleVerseModel verseModel = new BibleVerseModel
                {
                    BibleVerseKey = $"{bibleModel.BiblesId}||{verseKey}",
                    VerseText = verseText
                };

                bulkList.Add(verseModel);
            }
                
            BiblesData.Database.InsertBibleVerseBulk(bulkList);
        }
    }
}