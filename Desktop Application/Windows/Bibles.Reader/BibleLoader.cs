using Bible.Models.Bookmarks;
using Bibles.Common;
using Bibles.DataResources;
using Bibles.DataResources.Models;
using GeneralExtensions;
using IconSet;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using ViSo.Dialogs.Controls;
using WPF.Tools.BaseClasses;
using WPF.Tools.Specialized;

namespace Bibles.Reader
{
    internal static class BibleLoader
    {
        public delegate void LinkViewerClosedEvent(object sender, string verseKey);

        public static event LinkViewerClosedEvent LinkViewerClosed;

        private static readonly char[] veseSplitValues = new char[] { '*' };

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

            List<HighlightVerseModel> verseColours = BiblesData.Database.GetVerseColours(verse.BibleVerseKey);

            foreach(HighlightVerseModel colour in verseColours)
            {
                string[] itemSplit = colour.BibleVerseKeyId.Split(BibleLoader.veseSplitValues);

                result.HighlightText(itemSplit[1].ToInt32(), itemSplit[2].ToInt32(), ColourConverters.GetBrushfromHex(colour.HexColour));
            }
            
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

        private static void LinkImage_Selected(object sender, MouseButtonEventArgs e)
        {
            try
            {
                Image item = (Image)sender;

                string verseKey = item.Tag.ParseToString();

                Type linkViewerType = Type.GetType("Bibles.Link.LinkViewer,Bibles.Link");

                UserControlBase linkViewer = Activator.CreateInstance(linkViewerType, new object[] { verseKey }) as UserControlBase;
                
                if (ControlDialog.ShowDialog("Link Viewer",linkViewer, "SaveComments", autoSize: false).IsFalse())
                {
                    return;
                }

                string[] deletedLinks = linkViewer.GetPropertyValue("GetDeletedLinks").To<string[]>();

                foreach (string key in deletedLinks)
                {
                    BibleLoader.LinkViewerClosed?.Invoke(linkViewer, key);
                }
            }
            catch (Exception err)
            {
                ErrorLog.ShowError(err);
            }
        }
        
        private static UIElement[] GetVerseNumberElements(int bibleId, BibleVerseModel verse)
        {
            UIElement[] result = new UIElement[4];

            Label labelVerse = new Label { Content = Formatters.GetVerseFromKey(verse.BibleVerseKey), Foreground = Brushes.LightGray, Tag = verse };

            result[0] = labelVerse;

            result[1] = BibleLoader.GetVerseBookmarkImage(bibleId, verse.BibleVerseKey);

            result[2] = BibleLoader.GetStudyBookmarkImage(verse.BibleVerseKey);

            result[3] = BibleLoader.GetLinkImage(verse.BibleVerseKey);

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

        private static Image GetStudyBookmarkImage(string verseKey)
        {
            return null;

            //Dictionary<string, string> bookmarks = new Dictionary<string, string>(); //GlobalDictionary.GetBookmarkedStudies(verseKey);

            //List<Guid> removedBookmarksList = new List<Guid>();

            //foreach (KeyValuePair<string, string> keyItem in GlobalDictionary.studyBookmarks)
            //{
            //    if (keyItem.Key.EndsWith(verseKey))
            //    {
            //        if (!File.Exists(keyItem.Value))
            //        {
            //            string[] keySplit = keyItem.Key.Split(new string[] { "||" }, StringSplitOptions.RemoveEmptyEntries);

            //            removedBookmarksList.Add(Guid.Parse(keySplit[0]));

            //            continue;
            //        }

            //        bookmarks.Add(keyItem.Key, keyItem.Value);
            //    }
            //}

            //foreach (Guid key in removedBookmarksList)
            //{
            //    GlobalDictionary.RemoveStudyBookmark(key, verseKey);
            //}

            //if (bookmarks.Count == 0)
            //{
            //    return null;
            //}

            //Image img = new Image
            //{
            //    Source = IconSets.ResourceImageSource("BookmarkSmallRed", 16),
            //    Opacity = 0.5
            //};

            //if (bookmarks.Count > 0)
            //{
            //    ContextMenu bookmarkMenu = new ContextMenu();

            //    StringBuilder imageTooltip = new StringBuilder();

            //    imageTooltip.AppendLine("(Click to Edit)");

            //    foreach (KeyValuePair<string, string> item in bookmarks)
            //    {
            //        //string[] keySplit = item.Key.Split(new string[] { "||" }, StringSplitOptions.RemoveEmptyEntries);
            //        string subjectName = Path.GetFileNameWithoutExtension(item.Value);

            //        MenuItem menuItem = new MenuItem { Header = subjectName, Tag = item.Value };

            //        imageTooltip.AppendLine(subjectName);

            //        menuItem.Click += GlobalDictionary.StudyBookmarkMenuItem_Cliked;

            //        bookmarkMenu.Items.Add(menuItem);
            //    }

            //    img.ToolTip = imageTooltip.ToString();

            //    img.PreviewMouseUp += GlobalDictionary.StudyBookmarkContextMenu_Selected;

            //    img.ContextMenu = bookmarkMenu;
            //}

            //return img;
        }

        public static Image GetLinkImage(string verseKey)
        {
            if (!BiblesData.Database.HaveLink(verseKey))
            {
                return null;
            }

            Image linkImage = new Image
            {
                Source = IconSets.ResourceImageSource("Link", 16),
                Opacity = 0.5,
                Tag = verseKey
            };

            linkImage.PreviewMouseLeftButtonUp += BibleLoader.LinkImage_Selected;

            return linkImage;
        }

    }
}
