using GeneralExtensions;
using System;

namespace Bibles.Common
{
    public static class Formatters
    {
        public readonly static string[] KeySplitValue = new string[] { "||" };

        public static bool IsBiblesKey(string key, out string[] keyItems)
        {
            keyItems = key.Split(Formatters.KeySplitValue, StringSplitOptions.RemoveEmptyEntries);

            return Formatters.IsBiblesKey(keyItems);
        }

        public static bool IsBiblesKey(string[] keyItems)
        {
            if (keyItems.Length == 1)
            {
                return false;
            }

            // Bible Key's start with the Bible index First
            // Book Key's constains an O or N for Old or New Testaments
            return keyItems[0].IsNumberic() && !keyItems[1].IsNumberic();
        }

        public static int GetBibleFromKey(string bibleKey)
        {
            string[] keySplit = bibleKey.Split(Formatters.KeySplitValue, StringSplitOptions.RemoveEmptyEntries);

            return keySplit.Length >= 1 ? keySplit[0].ToInt32() : -1;
        }

        public static string GetBookFromKey(string bibleKey)
        {
            string[] keySplit = Formatters.CreateBibleKeySplit(bibleKey);

            return keySplit.Length >= 2 ? keySplit[1] : string.Empty;
        }

        public static int GetChapterFromKey(string bibleKey)
        {
            string[] keySplit = Formatters.CreateBibleKeySplit(bibleKey);

            return keySplit.Length >= 3 ? keySplit[2].ToInt32() : -1;
        }

        public static int GetVerseFromKey(string bibleKey)
        {
            string[] keySplit = Formatters.CreateBibleKeySplit(bibleKey);
         
            return keySplit.Length >= 4 ? keySplit[3].ToInt32() : -1;
        }

        public static string RemoveBibleId(string bibleKey)
        {
            string[] keySplit = bibleKey.Split(Formatters.KeySplitValue, StringSplitOptions.RemoveEmptyEntries);

            return $"{keySplit[1]}||{keySplit[2]}||{keySplit[3]}||";
        }
    
        public static string[] CreateBibleKeySplit(string key)
        {
            string[] keySplit = null;

            bool isBibleKey = Formatters.IsBiblesKey(key, out keySplit);

            if (isBibleKey)
            {
                return keySplit;
            }

            return new string[] { "Empty", keySplit[0], keySplit[1], keySplit[2] };
        }
    }
}
