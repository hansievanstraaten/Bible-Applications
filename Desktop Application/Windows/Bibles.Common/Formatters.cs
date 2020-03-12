using GeneralExtensions;
using System;

namespace Bibles.Common
{
    public static class Formatters
    {
        public readonly static string[] KeySplitValue = new string[] { "||" };

        public static int GetBibleFromKey(string bibleKey)
        {
            string[] keySplit = bibleKey.Split(Formatters.KeySplitValue, StringSplitOptions.RemoveEmptyEntries);

            return keySplit.Length >= 1 ? keySplit[0].ToInt32() : -1;
        }

        public static string GetBookFromKey(string bibleKey)
        {
            string[] keySplit = bibleKey.Split(Formatters.KeySplitValue, StringSplitOptions.RemoveEmptyEntries);

            return keySplit.Length >= 2 ? keySplit[1] : string.Empty;
        }

        public static int GetChapterFromKey(string bibleKey)
        {
            string[] keySplit = bibleKey.Split(Formatters.KeySplitValue, StringSplitOptions.RemoveEmptyEntries);

            return keySplit.Length >= 3 ? keySplit[2].ToInt32() : -1;
        }

        public static int GetVerseFromKey(string bibleKey)
        {
            string[] keySplit = bibleKey.Split(Formatters.KeySplitValue, StringSplitOptions.RemoveEmptyEntries);

            return keySplit.Length >= 4 ? keySplit[3].ToInt32() : -1;
        }

        public static string RemoveBibleId(string bibleKey)
        {
            string[] keySplit = bibleKey.Split(Formatters.KeySplitValue, StringSplitOptions.RemoveEmptyEntries);

            return $"{keySplit[1]}||{keySplit[2]}||{keySplit[3]}||";
        }
    }
}
