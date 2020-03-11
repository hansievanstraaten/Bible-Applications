using SQLite;
using System;
using System.Linq;
using System.Threading.Tasks;
using GeneralExtensions;
using Bibles.DataResources.Models;
using System.Collections.Generic;

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

        public Task<List<BibleModel>> GetBibles()
        {
            return BiblesData.database.Table<BibleModel>()
                .ToListAsync();
        }

        public Task<int> InsertBible(BibleModel bible)
        {
            Task<BibleModel> existing = BiblesData.database.Table<BibleModel>().FirstOrDefaultAsync(b => b.BiblesId == bible.BiblesId);

            if (existing.Result != null)
            {
                return Task<int>.Factory.StartNew(() => existing.Result.BiblesId);
            }

            return BiblesData.database.InsertAsync(bible);
        }

        public void InsertBibleVerseBulk(List<BibleVerseModel> verseList)
        {
            BiblesData.database.InsertAllAsync(verseList);
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
                    await database.CreateTablesAsync(CreateFlags.None, typeof(BibleModel)).ConfigureAwait(false);

                    BiblesData.IsInitialized = true;
                }

                if (!database.TableMappings.Any(m => m.MappedType.Name == typeof(BibleVerseModel).Name))
                {
                    await database.CreateTablesAsync(CreateFlags.None, typeof(BibleVerseModel)).ConfigureAwait(false);

                    BiblesData.IsInitialized = true;
                }
            }
        }

    }
}
