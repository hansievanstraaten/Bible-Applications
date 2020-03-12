using Bibles.Common;
using Bibles.DataResources;
using Bibles.DataResources.Models;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using WPF.Tools.Specialized;

namespace Bibles.Reader
{
    internal static class BibleLoader
    {
        internal static HighlightRitchTextBox GetVerseAsTextBox(int bibleId, BibleVerseModel verse, int column)
        {
            HighlightRitchTextBox result = new HighlightRitchTextBox
            {
                Text = verse.VerseText,
                Tag = verse,
                BorderBrush = Brushes.Transparent,
                IsReadOnly = true,
                Margin = new Thickness(2, 0, 0, 15)
            };

            //foreach (string item in GlobalDictionary.GetVerseHighlights(bible, verse.VerseKey))
            //{
            //    string[] itemSplit = item.Split(new string[] { "||" }, StringSplitOptions.None);

            //    result.HighlightText(itemSplit[0].ToInt32(), itemSplit[1].ToInt32(), ColourConverters.GetBrushfromHex(itemSplit[2]));
            //}


            Grid.SetRow(result, (Formatters.GetVerseFromKey(verse.BibleVerseKey) - 1));

            Grid.SetColumn(result, column);

            return result;
        }

        internal static StackPanel GetVerseNumberPanel(int bibleId, BibleVerseModel verse, int column)
        {
            StackPanel result = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Top
            };

            UIElement[] children = BibleLoader.GetVerseNumberElements(bibleId, verse);

            result.Children.Add(children[0]);

            for (int x = 1; x < children.Length; ++x)
            {
                if (children[x] == null)
                {
                    continue;
                }

                result.Children.Add(children[x]);
            }

            Grid.SetRow(result, (Formatters.GetVerseFromKey(verse.BibleVerseKey) - 1));

            Grid.SetColumn(result, column);

            return result;
        }
        
        private static UIElement[] GetVerseNumberElements(int bibleId, BibleVerseModel verse)
        {
            UIElement[] result = new UIElement[4];

            Label labelVerse = new Label { Content = Formatters.GetVerseFromKey(verse.BibleVerseKey), Foreground = Brushes.LightGray, Tag = verse };

            result[0] = labelVerse;

            //result[1] = BibleLoader.GetVerseBookmarkImage(bibleName, verse.VerseKey);

            //result[2] = GlobalDictionary.GetStudyBookmarkImage(verse.VerseKey);

            //result[3] = LinkManager.GetLinkImage(verse.VerseKey);

            return result;
        }

    }
}
