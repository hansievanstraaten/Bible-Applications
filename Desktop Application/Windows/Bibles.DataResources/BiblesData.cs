using SQLite;
using System;
using System.Linq;
using System.Threading.Tasks;
using GeneralExtensions;
using Bibles.DataResources.Models;
using System.Collections.Generic;
using Bibles.Common;
using Bible.Models.Link;

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

        #region USERPREFERENCEMODEL

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

            existing.Result.LastReadVerse = userPreference.LastReadVerse;

            BiblesData.database.UpdateAsync(existing.Result);

            return Task<int>.Factory.StartNew(() => existing.Result.UserId);
        }

        #endregion

        #region BIBLEMODEL

        public Task<List<BibleModel>> GetBibles()
        {
            return BiblesData.database.Table<BibleModel>()
                .ToListAsync();
        }

        public string GetBibleName(int bibleId)
        {
            Task<BibleModel> model = BiblesData.database.Table<BibleModel>().FirstOrDefaultAsync(bi => bi.BiblesId == bibleId);

            return model.Result == null ? string.Empty : model.Result.BibleName;
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

        #endregion

        #region BIBLEVERSEMODEL

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
            Task<int> result = BiblesData.database.InsertAllAsync(verseList);
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

        #endregion

        #region BOOKMARKMODEL

        public List<BookmarkModel> GetBookmarks()
        {
            return BiblesData.database.Table<BookmarkModel>().ToListAsync().Result;
        }

        public BookmarkModel GetBookmark(string verseKey)
        {
            Task<BookmarkModel> existing = BiblesData.database.Table<BookmarkModel>()
                .FirstOrDefaultAsync(bm => bm.VerseKey == verseKey);

            return existing.Result;
        }

        public void InsertBookmarkModel(BookmarkModel bookmark)
        {
            Task<BookmarkModel> existing = BiblesData.database.Table<BookmarkModel>()
                .FirstOrDefaultAsync(bm => bm.VerseKey == bookmark.VerseKey);

            if (existing.Result == null)
            {
                BiblesData.database.InsertAsync(bookmark);
            }
            else
            {
                existing.Result.BookMarkName = bookmark.BookMarkName;

                existing.Result.Description = bookmark.Description;

                existing.Result.VerseRangeEnd = bookmark.VerseRangeEnd;

                BiblesData.database.UpdateAsync(existing.Result);
            }
        }

        public void DeleteBookmark(string bibleVerseKey)
        {
            BiblesData.database.Table<BookmarkModel>().DeleteAsync(b => b.VerseKey == bibleVerseKey);
        }

        #endregion

        #region LINK

        private static List<string> linkedList = new List<string>();

        private static char[] linkSplit = new char[] { '*'};

        public bool HaveLink(string bibleKey)
        {
            Task<LinkModel> result = BiblesData.database
                .Table<LinkModel>()
                .FirstOrDefaultAsync(li => li.LinkKeyId.Contains(bibleKey));

            return result.Result != null;
        }

        public string GetLinkComments(string bibleKey)
        {
            Task<LinkModel> result = BiblesData.database
                .Table<LinkModel>()
                .FirstOrDefaultAsync(li => li.LinkKeyId.Contains(bibleKey));

            return result.Result.Comments;
        }

        public void SaveLinkComments(string bibleKey, string comments)
        {
            Task<LinkModel> result = BiblesData.database
                .Table<LinkModel>()
                .FirstOrDefaultAsync(li => li.LinkKeyId.Contains(bibleKey));

            result.Result.Comments = comments;

            BiblesData.database.UpdateAsync(result.Result);
        }

        public ModelsLink GetLinkTree(string bibleKey)
        {
            try
            {
                return this.GetLinkedVerses(bibleKey);
            }
            finally
            {
                BiblesData.linkedList.Clear();
            }
        }

        private ModelsLink GetLinkedVerses(string bibleKey)
        {
            BiblesData.linkedList.Add(bibleKey);

            string parentKey = $"{bibleKey}*";

            string childKey = $"*{bibleKey}";

            Task<List<LinkModel>> parentToChild = BiblesData.database
                .Table<LinkModel>()
                .Where(pl => pl.LinkKeyId.StartsWith(parentKey))
                .ToListAsync();

            Task<List<LinkModel>> childToParent = BiblesData.database
                .Table<LinkModel>()
                .Where(pl => pl.LinkKeyId.EndsWith(childKey))
                .ToListAsync();

            ModelsLink results = new ModelsLink { BibleVerseKey = bibleKey };

            foreach(LinkModel model in parentToChild.Result)
            {
                string[] verseSplit = model.LinkKeyId.Split(linkSplit);

                if (BiblesData.linkedList.Contains(verseSplit[1]))
                {
                    continue;
                }

                ModelsLink child = new ModelsLink { BibleVerseKey = verseSplit[1], LinkKeyId = model.LinkKeyId };

                child.BibleVerseChildLinks.AddRange(this.GetLinkedVerses(verseSplit[1]).BibleVerseChildLinks);

                results.BibleVerseChildLinks.Add(child);
            }

            foreach (LinkModel model in childToParent.Result)
            {
                string[] verseSplit = model.LinkKeyId.Split(linkSplit);

                if (BiblesData.linkedList.Contains(verseSplit[0]))
                {
                    continue;
                }

                ModelsLink child = new ModelsLink { BibleVerseKey = verseSplit[0], LinkKeyId = model.LinkKeyId };

                child.BibleVerseChildLinks.AddRange(this.GetLinkedVerses(verseSplit[0]).BibleVerseChildLinks);

                results.BibleVerseChildLinks.Add(child);
            }

            return results;
        }

        public void CreateLink (LinkModel link)
        {
            Task<LinkModel> exising = BiblesData.database.Table<LinkModel>().FirstOrDefaultAsync(li => li.LinkKeyId == link.LinkKeyId);

            if (exising.Result != null)
            {
                return;
            }

            BiblesData.database.InsertAsync(link);
        }
        
        public void DeleteLink(string linkKeyId)
        {
            BiblesData.database.Table<LinkModel>().DeleteAsync(b => b.LinkKeyId == linkKeyId);
        }

        #endregion

        private async Task InitializeAsync()
        {
            if (!BiblesData.IsInitialized)
            {
                if (!database.TableMappings.Any(bi => bi.MappedType.Name == typeof(BibleModel).Name))
                {
                    await database.CreateTablesAsync(CreateFlags.AutoIncPK, typeof(BibleModel)).ConfigureAwait(false);
                }

                if (!database.TableMappings.Any(bv => bv.MappedType.Name == typeof(BibleVerseModel).Name))
                {
                    await database.CreateTablesAsync(CreateFlags.None, typeof(BibleVerseModel)).ConfigureAwait(false);
                }

                if (!database.TableMappings.Any(up => up.MappedType.Name == typeof(UserPreferenceModel).Name))
                {
                    await database.CreateTablesAsync(CreateFlags.AutoIncPK, typeof(UserPreferenceModel)).ConfigureAwait(false);
                }

                if (!database.TableMappings.Any(bm => bm.MappedType.Name == typeof(BookmarkModel).Name))
                {
                    await database.CreateTablesAsync(CreateFlags.None, typeof(BookmarkModel)).ConfigureAwait(false);
                }

                if (!database.TableMappings.Any(li => li.MappedType.Name == typeof(LinkModel).Name))
                {
                    await database.CreateTablesAsync(CreateFlags.ImplicitIndex, typeof(LinkModel)).ConfigureAwait(false);
                }
                    
                BiblesData.IsInitialized = true;
            }
        }
    }
}
