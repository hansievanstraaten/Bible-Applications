﻿using Bible.Models.AvailableBooks;
using Bible.Models.Bookmarks;
using Bibles.Common;
using Bibles.Data;
using Bibles.DataResources;
using Bibles.DataResources.Models;
using GeneralExtensions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using ViSo.Dialogs.ModelViewer;
using ViSo.Dialogs.TextEditor;
using WPF.Tools.BaseClasses;
using WPF.Tools.Specialized;

namespace Bibles.Reader
{
    /// <summary>
    /// Interaction logic for Reader.xaml
    /// </summary>
    public partial class Reader : UserControlBase
    {
        public delegate void BibleBookChangedEvent(object sender, ModelsBibleBook bible);

        public delegate void SelectedVerseChangedEvent(object sender, BibleVerseModel verse);

        public event BibleBookChangedEvent BibleBookChanged;

        public event SelectedVerseChangedEvent SelectedVerseChanged;

        private int scrollOnLoadVerse = -1;

        private string selectedKey;

        private Dictionary<int, BibleVerseModel> versesDictionary;

        private Dictionary<int, HighlightRitchTextBox> loadedTextBoxDictionary = new Dictionary<int, HighlightRitchTextBox>();

        private Dictionary<int, StackPanel> loadedVerseStackDictionary = new Dictionary<int, StackPanel>();

        public Reader()
        {
            this.InitializeComponent();

            this.Loaded += this.Reader_Loaded;

            this.Bible = new ModelsBibleBook();

            this.Bible.PropertyChanged += this.Bible_Changed;
            
            this.uxBible.Items.Add(this.Bible);
        }

        public string SelectedVerseKey
        {
            get
            {
                return this.selectedKey;
            }
        }
        
        public ModelsBibleBook Bible { get; set; }

        public void SetBible(int bibleId)
        {
            this.uxBible[0, 0].SetValue(bibleId);            
        }
        
        public void SetChapter(string key)
        {
            this.selectedKey = key;

            this.SetHeader();

            this.LoadVerses();

            this.SetCanPage(Formatters.GetChapterFromKey(key));
        }

        public void SetVerse(string key)
        {
            if (key.IsNullEmptyOrWhiteSpace())
            {
                return;
            }

            this.SetCanPage(Formatters.GetChapterFromKey(key));

            this.selectedKey = key;

            this.SetHeader();

            if (this.versesDictionary == null || this.versesDictionary.Count == 0)
            {
                this.LoadVerses();
            }

            if (Formatters.IsBiblesKey(key))
            {
                this.ScrollToVerse(Formatters.GetVerseFromKey(key));
            }
            else
            {
                this.ScrollToVerse(Formatters.GetVerseFromKey($"{this.Bible.BibleId}||{key}"));
            }
        }

        private void Reader_Loaded(object sender, RoutedEventArgs e)
        {
            if (base.WasFirstLoaded)
            {
                return;
            }

            try
            {
                this.ScrollToVerse(this.scrollOnLoadVerse);

                this.scrollOnLoadVerse = -1;
                
                base.WasFirstLoaded = true;
                
                this.SetCanPage(Formatters.GetChapterFromKey(this.selectedKey));
            }
            catch (Exception err)
            {
                ErrorLog.ShowError(err);
            }
        }

        private void Bible_Changed(object sender, PropertyChangedEventArgs e)
        {
            switch(e.PropertyName)
            {
                case "BibleName":
                    
                    this.BibleBookChanged?.Invoke(this, this.Bible);

                    break;

                case "BibleId":

                    if (base.IsLoaded)
                    {
                        this.LoadVerses();
                    }

                    break;
            }
        }

        private void Bookmark_Cliked(object sender, RoutedEventArgs e)
        {
            try
            {
                int selectedVerse = Formatters.GetVerseFromKey(this.selectedKey);

                if (selectedVerse <= 0)
                {
                    throw new ApplicationException("Please select a Verse.");
                }

                ModelsBookmark bookmark = new ModelsBookmark();
                
                if (Formatters.IsBiblesKey(this.selectedKey))
                {
                    bookmark.SetVerse(this.selectedKey);
                }
                else
                {
                    bookmark.SetVerse($"{this.Bible.BibleId}||{this.selectedKey}");
                }

                ModelView.OnItemBrowse += this.BookmarkModel_Browse;

                if (ModelView.ShowDialog("Bookmark", bookmark).IsFalse())
                {
                    return;
                }

                BookmarkModel dbModel = bookmark.CopyToObject(new BookmarkModel()).To<BookmarkModel>();

                BiblesData.Database.InsertBookmarkModel(dbModel);

                BibleLoader.RefreshVerseNumberPanel
                    (
                        this.loadedVerseStackDictionary[selectedVerse],
                        this.Bible.BibleId,
                        this.versesDictionary[selectedVerse]
                    );
            }
            catch (Exception err)
            {
                ErrorLog.ShowError(err);
            }
            finally
            {
                ModelView.OnItemBrowse -= this.BookmarkModel_Browse;
            }
        }

        private void BookmarkModel_Browse(object sender, object model, string buttonKey)
        {
            try
            {
                ModelsBookmark bookmark = (ModelsBookmark)model;

                if (TextEditing.ShowDialog("Bookmark Description", bookmark.Description).IsFalse())
                {
                    return;
                }

                bookmark.Description = TextEditing.Text;                
            }
            catch (Exception err)
            {
                ErrorLog.ShowError(err);
            }
        }

        private void LinkVerse_Cliked(object sender, RoutedEventArgs e)
        {
            try
            {

            }
            catch (Exception err)
            {
                ErrorLog.ShowError(err);
            }
        }

        private void BackColour_Clicked(object sender, RoutedEventArgs e)
        {
            try
            {

            }
            catch (Exception err)
            {
                ErrorLog.ShowError(err);
            }
        }

        private void ClearBackColour_Clicked(object sender, RoutedEventArgs e)
        {
            try
            {

            }
            catch (Exception err)
            {
                ErrorLog.ShowError(err);
            }
        }

        private void Left_Cliked(object sender, RoutedEventArgs e)
        {
            try
            {
                int chapter = Formatters.GetChapterFromKey(this.selectedKey);

                --chapter;

                this.PageToChapter(chapter);
            }
            catch (Exception err)
            {
                ErrorLog.ShowError(err);
            }
        }

        private void Right_Cliked(object sender, RoutedEventArgs e)
        {
            try
            {
                int chapter = Formatters.GetChapterFromKey(this.selectedKey);

                ++chapter;

                this.PageToChapter(chapter);
            }
            catch (Exception err)
            {
                ErrorLog.ShowError(err);
            }
        }

        private void Verse_GotFocus(object sender, RoutedEventArgs e)
        {
            try
            {
                HighlightRitchTextBox box = (HighlightRitchTextBox)sender;

                this.selectedKey = Formatters.RemoveBibleId(((BibleVerseModel)box.Tag).BibleVerseKey);

                this.SetHeader();

                this.SelectedVerseChanged?.Invoke(this, this.versesDictionary[Formatters.GetVerseFromKey(this.selectedKey)]);
            }
            catch (Exception err)
            {
                ErrorLog.ShowError(err);
            }
        }

        private void SetHeader()
        {
            this.uxBible[0].Header = GlobalStaticData.Intance.GetKeyDescription(this.selectedKey);
        }

        private void LoadVerses()
        {
            this.versesDictionary = null;

            this.versesDictionary = Formatters.IsBiblesKey(this.selectedKey) ?
                BiblesData.Database.GetVerses(this.selectedKey)
                :
                BiblesData.Database.GetVerses($"{this.Bible.BibleId}||{this.selectedKey}");

            this.ResetversSetup();

            for (int verse = 1; verse <= this.versesDictionary.Count; ++verse)
            {
                BibleVerseModel item = this.versesDictionary[verse];

                StackPanel panel = BibleLoader.GetVerseNumberPanel(this.Bible.BibleId, item, 0);

                this.uxVerseGrid.Children.Add(panel);

                HighlightRitchTextBox textBox = BibleLoader.GetVerseAsTextBox(this.Bible.BibleId, item, 1);

                textBox.GotFocus += this.Verse_GotFocus;

                this.uxVerseGrid.Children.Add(textBox);

                this.loadedTextBoxDictionary.Add(verse, textBox);

                this.loadedVerseStackDictionary.Add(verse, panel);
            }
        }

        private void ResetversSetup()
        {
            this.loadedTextBoxDictionary.Clear();

            this.loadedVerseStackDictionary.Clear();

            this.uxVerseGrid.Children.Clear();

            this.uxVerseGrid.RowDefinitions.Clear();

            this.uxBookmark.IsEnabled = this.versesDictionary.Count > 0;

            this.uxLink.IsEnabled = this.versesDictionary.Count > 0;

            for (int x = 0; x < this.versesDictionary.Count; ++x)
            {
                this.uxVerseGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(26, GridUnitType.Auto) });
            }
        }

        private void ScrollToVerse(int verseNumber)
        {
            if (verseNumber <= 0 || !this.loadedTextBoxDictionary.ContainsKey(verseNumber))
            {
                return;
            }

            if (!this.IsLoaded)
            {
                this.scrollOnLoadVerse = verseNumber;
            }

            HighlightRitchTextBox verseBox = this.loadedTextBoxDictionary[verseNumber];

            Point versePoint = verseBox.TranslatePoint(new Point(), this.uxVerseGrid);

            this.uxVerseGridScroll.ScrollToVerticalOffset(versePoint.Y);

            verseBox.Focus();
        }    
    
        private void SetCanPage(int chapter)
        {
            if (!base.WasFirstLoaded)
            {
                return;
            }

            this.uxLeftButton.IsEnabled = chapter > 1;

            this.uxRightButton.IsEnabled = chapter < GlobalStaticData.Intance.GetChaptersCount(this.selectedKey);
        }

        private void PageToChapter(int chapter)
        {
            this.SetCanPage(chapter);

            string next = Formatters.ChangeChapter(this.selectedKey, chapter);

            next = Formatters.ChangeVerse(next, 1);

            this.selectedKey = next;

            this.SetChapter(next);                     

            this.uxVerseGridScroll.ScrollToTop();
        }
    }
}
