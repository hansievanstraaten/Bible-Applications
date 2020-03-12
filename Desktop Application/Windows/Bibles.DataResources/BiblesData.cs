using SQLite;
using System;
using System.Linq;
using System.Threading.Tasks;
using GeneralExtensions;
using Bibles.DataResources.Models;
using System.Collections.Generic;
using Bibles.Common;

namespace Bibles.DataResources
{
    public class BiblesData
    {
        private static readonly Lazy<SQLiteAsyncConnection> LazyInitializer = new Lazy<SQLiteAsyncConnection>(() =>
        {
            return new SQLiteAsyncConnection(DbConstraints.DatabasePath, DbConstraints.Flags);
        });

        private static BiblesData viSoDatabase;

        private static SQLiteAsyncConnection database => BiblesData.LazyInitializer.Value;

        private static bool IsInitialized = false;

        private static object databaseLockObject = new object();

        public BiblesData()
        {
            InitializeAsync().SafeFireAndForget(false);
        }

        public static BiblesData Database
        {
            get
            {
                if (BiblesData.viSoDatabase == null)
                {
                    lock (BiblesData.databaseLockObject)
                    {
                        if (BiblesData.viSoDatabase == null)
                        {
                            BiblesData.viSoDatabase = new BiblesData();
                        }
                    }
                }

                return BiblesData.viSoDatabase;
            }
        }

        public Task<UserPreferenceModel> GetPreferences()
        {
            Task<UserPreferenceModel> result = BiblesData.database.Table<UserPreferenceModel>().FirstOrDefaultAsync();

            return result;
        }

        public Task<int> InsertserPreference(UserPreferenceModel userPreference)
        {
            Task<UserPreferenceModel> existing = BiblesData.database.Table<UserPreferenceModel>().FirstOrDefaultAsync();

            if (existing.Result == null)
            {
                return BiblesData.database.InsertAsync(userPreference);
            }

            existing.Result.DefaultBible = userPreference.DefaultBible;

            existing.Result.Language = userPreference.Language;

            existing.Result.SynchronizzeTabs = userPreference.SynchronizzeTabs;

            existing.Result.Font = userPreference.Font;

            existing.Result.FontSize = userPreference.FontSize;

            BiblesData.database.UpdateAsync(existing.Result);

            return Task<int>.Factory.StartNew(() => existing.Result.UserId);
        }

        public Task<List<BibleModel>> GetBibles()
        {
            return BiblesData.database.Table<BibleModel>()
                .ToListAsync();
        }

        public BibleModel GetBible(string bibleName)
        {
            Task<BibleModel> existing = BiblesData.database.Table<BibleModel>().FirstOrDefaultAsync(b => b.BibleName == bibleName);

            return existing.Result;
        }

        public void InsertBible(BibleModel bible)
        {
            Task<BibleModel> existing = BiblesData.database.Table<BibleModel>().FirstOrDefaultAsync(b => b.BibleName == bible.BibleName);

            if (existing.Result != null)
            {
                return;
            }

            BiblesData.database.InsertAsync(bible);
        }

        public BibleVerseModel GetVerse(string verseKey)
        {
            return BiblesData.database.Table<BibleVerseModel>().FirstOrDefaultAsync(vk => vk.BibleVerseKey == verseKey).Result;
        }

        public Dictionary<int, BibleVerseModel> GetVerses(string bibleKey)
        {
            int bookKey = Formatters.GetChapterFromKey(bibleKey);

            if (bookKey < 0)
            {
                return new Dictionary<int, BibleVerseModel>();
            }

            string searchKey = $"{Formatters.GetBibleFromKey(bibleKey)}||{Formatters.GetBookFromKey(bibleKey)}||{Formatters.GetChapterFromKey(bibleKey)}||";

            // ENG - 1||03O||4||
            // AFR - 2||03O||4||
            // GER - 3||03O||4||
            // XHO - 4||03O||4||

            Task<List<BibleVerseModel>> resultList = BiblesData.database
                .Table<BibleVerseModel>()
                .Where(v => v.BibleVerseKey.StartsWith(searchKey))
                .ToListAsync();
            
            if (resultList.Result == null)
            {
                return new Dictionary<int, BibleVerseModel>();
            }

            return resultList.Result
                .ToDictionary(vk => Formatters.GetVerseFromKey(vk.BibleVerseKey));
        }

        public void InsertBibleVerseBulk(List<BibleVerseModel> verseList)
        {
            Task<int> result =  BiblesData.database.InsertAllAsync(verseList);
        }

        public void InsertBibleVerse(BibleVerseModel verse)
        {
            Task<BibleVerseModel> existing = BiblesData.database.Table<BibleVerseModel>().FirstOrDefaultAsync(v => v.BibleVerseKey == verse.BibleVerseKey);

            if (existing.Result != null)
            {
                existing.Result.VerseText = verse.VerseText;

                BiblesData.database.UpdateAsync(existing.Result);

                return;
            }

            BiblesData.database.InsertAsync(verse);
        }

        private async Task InitializeAsync()
        {
            if (!BiblesData.IsInitialized)
            {
                if (!database.TableMappings.Any(m => m.MappedType.Name == typeof(BibleModel).Name))
                {
                    await database.CreateTablesAsync(CreateFlags.AutoIncPK, typeof(BibleModel)).ConfigureAwait(false);
                }

                if (!database.TableMappings.Any(m => m.MappedType.Name == typeof(BibleVerseModel).Name))
                {
                    await database.CreateTablesAsync(CreateFlags.None, typeof(BibleVerseModel)).ConfigureAwait(false);
                }

                if (!database.TableMappings.Any(u => u.MappedType.Name == typeof(UserPreferenceModel).Name))
                {
                    await database.CreateTablesAsync(CreateFlags.AutoIncPK, typeof(UserPreferenceModel)).ConfigureAwait(false);
                }
                    
                BiblesData.IsInitialized = true;
            }
        }
    }
}
