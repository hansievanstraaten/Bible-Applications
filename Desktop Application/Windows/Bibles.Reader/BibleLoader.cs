using Bible.Models.Bookmarks;
using Bibles.Common;
using Bibles.DataResources;
using Bibles.DataResources.Models;
using GeneralExtensions;
using IconSet;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
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

            BibleLoader.RefreshVerseNumberPanel(result, bibleId, verse);
            
            Grid.SetRow(result, (Formatters.GetVerseFromKey(verse.BibleVerseKey) - 1));

            Grid.SetColumn(result, column);

            return result;
        }

        public static void RefreshVerseNumberPanel(StackPanel versePanel, int bibleId, BibleVerseModel verse)
        {
            versePanel.Children.Clear();

            UIElement[] children = BibleLoader.GetVerseNumberElements(bibleId, verse);

            versePanel.Children.Add(children[0]);

            for (int x = 1; x < children.Length; ++x)
            {
                if (children[x] == null)
                {
                    continue;
                }

                versePanel.Children.Add(children[x]);
            }
        }
        
        private static void Bookmark_Selected(object sender, MouseButtonEventArgs e)
        {
        }

        private static UIElement[] GetVerseNumberElements(int bibleId, BibleVerseModel verse)
        {
            UIElement[] result = new UIElement[4];

            Label labelVerse = new Label { Content = Formatters.GetVerseFromKey(verse.BibleVerseKey), Foreground = Brushes.LightGray, Tag = verse };

            result[0] = labelVerse;

            result[1] = BibleLoader.GetVerseBookmarkImage(bibleId, verse.BibleVerseKey);

            //result[2] = GlobalDictionary.GetStudyBookmarkImage(verse.VerseKey);

            //result[3] = LinkManager.GetLinkImage(verse.VerseKey);

            return result;
        }

        private static Image GetVerseBookmarkImage(int bibleId, string verseKey)
        {
            string bibleKey = Formatters.IsBiblesKey(verseKey) ?
                verseKey : $"{bibleId}||{verseKey}";

            BookmarkModel model = BiblesData.Database.GetBookmark(bibleKey);

            if (model == null)
            {
                return null;
            }

            ModelsBookmark bookmark = model.CopyToObject(new ModelsBookmark()).To<ModelsBookmark>();

            string imgToolTip = bookmark.BookMarkName.IsNullEmptyOrWhiteSpace() && bookmark.Description.IsNullEmptyOrWhiteSpace() ?
            bookmark.SelectedVerse : bookmark.BookMarkName.IsNullEmptyOrWhiteSpace() ?
            $"{bookmark.SelectedVerse}{Environment.NewLine}{bookmark.Description}" :
            $"{bookmark.SelectedVerse}{Environment.NewLine}{bookmark.BookMarkName}{Environment.NewLine}{Environment.NewLine}{bookmark.Description}";

            Image img = new Image
            {
                Source = IconSets.ResourceImageSource("BookmarkSmall", 16),
                ToolTip = imgToolTip,
                Opacity = 0.5,
                Tag = bibleKey
            };

            img.PreviewMouseLeftButtonUp += BibleLoader.Bookmark_Selected;

            return img;
        }

        
    }
}
