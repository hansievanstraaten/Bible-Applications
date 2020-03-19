using Bible.Models.AvailableBooks;
using Bible.Models.Bookmarks;
using Bibles.BookIndex;
using Bibles.Bookmarks;
using Bibles.Common;
using Bibles.Data;
using Bibles.DataResources;
using Bibles.DataResources.Models;
using Bibles.Reader;
using Bibles.Search;
using GeneralExtensions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using ViSo.Dialogs.Controls;
using WPF.Tools.BaseClasses;
using WPF.Tools.TabControl;

namespace Bibles
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : WindowBase
    {
        private string selectedItemKey = string.Empty;
        
        private Indexer uxIndexer = new Indexer();

        public MainWindow()
        {
            this.InitializeComponent();

            this.uxMessageLable.Content = "Loading…";

            //this.Loaded += this.MainWindow_Loaded;

            this.Closing += this.MainWindow_Closing;

            InitializeData initialData = new InitializeData();

            initialData.InitialDataLoadCompleted += this.InitialDataLoad_Completed;

            initialData.LoadEmbeddedBibles(this.Dispatcher, Application.Current.MainWindow.FontFamily);
        }

        #region MAIN WINDOW EVENTS

        private void MainWindow_Closing(object sender, CancelEventArgs e)
        {
            try
            {
                UserPreferenceModel preference = GlobalResources.UserPreferences;

                string biblesKey = ((Reader.Reader)this.uxMainTab.Items[0]).SelectedVerseKey;

                if (!Formatters.IsBiblesKey(biblesKey))
                {
                    int biblesId = ((Reader.Reader)this.uxMainTab.Items[0]).Bible.BibleId;

                    biblesKey = Formatters.ChangeBible(biblesKey, biblesId);
                }

                preference.LastReadVerse = biblesKey;

                BiblesData.Database.InsertserPreference(preference);
            }
            catch (Exception err)
            {
                // DO NOTHING
            }
        }

        private void InitialDataLoad_Completed(object sender, string message, bool completed, Exception error)
        {
            try
            {
                if (error != null)
                {
                    throw error;
                }

                if (completed)
                {
                    this.uxMessageLable.Content = string.Empty;

                    this.InitializeTabs();

                    this.LoadDynamicMenus();

                    this.selectedItemKey = GlobalResources.UserPreferences.LastReadVerse;

                    int bibleId = !this.selectedItemKey.IsNullEmptyOrWhiteSpace() && Formatters.IsBiblesKey(this.selectedItemKey) ?
                        Formatters.GetBibleFromKey(this.selectedItemKey)
                        :
                        GlobalResources.UserPreferences.DefaultBible;
                    
                    ((Reader.Reader)this.uxMainTab.Items[0]).SetBible(bibleId);

                    ((Reader.Reader)this.uxMainTab.Items[0]).SetVerse(this.selectedItemKey);
                }
                else
                {
                    this.uxMessageLable.Content = message;
                }
            }
            catch (Exception err)
            {
                MessageBox.Show(err.InnerExceptionMessage());
            }
        }
        
        private void SeletedChapter_Changed(object sender, string key)
        {
            try
            {
                UserControlBase item = this.uxMainTab.Items[this.uxMainTab.SelectedIndex];

                item.InvokeMethod(item, "SetChapter", new object[] { key }, false);

                this.selectedItemKey = key;
            }
            catch (Exception err)
            {
                ErrorLog.ShowError(err);
            }
        }
        
        private void SelectedVerse_Changed(object sender, string key)
        {
            try
            {
                UserControlBase item = this.uxMainTab.Items[this.uxMainTab.SelectedIndex];

                item.InvokeMethod(item, "SetVerse", new object[] { key }, false);

                this.selectedItemKey = key;
            }
            catch (Exception err)
            {
                ErrorLog.ShowError(err);
            }
        }

        private void OnSelectedTabBible_Changed(object sender, ModelsBibleBook bible)
        {
            try
            {
                this.uxMainTab.SetHeaderName(this.uxMainTab.SelectedIndex, bible.BibleName);
            }
            catch (Exception err)
            {
                ErrorLog.ShowError(err);
            }
        }

        private void OnReaderSelectedVerse_Changed(object sender, BibleVerseModel verse)
        {
            try
            {
                this.selectedItemKey = verse.BibleVerseKey;

                if (GlobalResources.UserPreferences.SynchronizzeTabs)
                {

                }
            }
            catch (Exception err)
            {
                ErrorLog.ShowError(err);
            }
        }

        private void LeftTabPin_Changed(object sender, bool isPined)
        {
            TabControlVertical item = (TabControlVertical)sender;

            this.uxMaingrid.ColumnDefinitions[0].Width = new GridLength(item.ActualWidth, GridUnitType.Auto);

            if (isPined)
            {
                this.uxMaingrid.ColumnDefinitions[1].Width = new GridLength(3);
            }
            else
            {
                this.uxMaingrid.ColumnDefinitions[1].Width = new GridLength(0);
            }
        }

        #endregion

        #region MENU ITEM EVENTS

        private void Exit_Cliked(object sender, RoutedEventArgs e)
        {
            this.Dispatcher.InvokeShutdown();
        }

        private void MenuBiblesItem_Clicked(object sender, RoutedEventArgs e)
        {
            try
            {
                MenuItem item = (MenuItem)sender;

                this.LoadReader(item.Tag.ToInt32(), this.selectedItemKey);
            }
            catch (Exception err)
            {
                ErrorLog.ShowError(err);
            }
        }
   
        private void Bookmarks_Cliked(object sender, RoutedEventArgs e)
        {
            try
            {
                BookmarksList bookmarks = new BookmarksList();

                bookmarks.BookmarkReaderRequest += this.BookmarkReader_Request;

                ControlDialog.Show("Bookmarks", bookmarks, string.Empty, owner:this, isTopMost:true);
            }
            catch (Exception err)
            {
                ErrorLog.ShowError(err);
            }
        }

        private void Search_Cliked(object sender, RoutedEventArgs e)
        {
            try
            {
                SearchView search = new SearchView();

                ControlDialog.Show("Search", search, string.Empty, owner:this, showCancelButton:false);
            }
            catch (Exception err)
            {
                ErrorLog.ShowError(err);
            }
        }
        
        private void Highlights_Cliked(object sender, RoutedEventArgs e)
        {
            try
            {
                BibleHighlights highlight = new BibleHighlights();

                ControlDialog.Show("Highlights", highlight, string.Empty, showCancelButton:false, autoSize:false);
            }
            catch (Exception err)
            {
                ErrorLog.ShowError(err); ;
            }
        }

        private void BookmarkReader_Request(object sender, ModelsBookmark bookmark)
        {
            try
            {
                this.LoadReader(Formatters.GetBibleFromKey(bookmark.VerseKey), bookmark.VerseKey);
            }
            catch (Exception err)
            {
                ErrorLog.ShowError(err);
            }
        }

        private void ParallelReader_Cliked(object sender, RoutedEventArgs e)
        {
            try
            {
                int bibleId = Formatters.GetBibleFromKey(this.selectedItemKey);

                if (bibleId <= 0)
                {
                    bibleId = GlobalResources.UserPreferences.DefaultBible;
                }

                ParallelReader reader = new ParallelReader { ShowCloseButton = true };

                reader.SetBible(bibleId);

                reader.SetChapter(this.selectedItemKey);

                reader.SetVerse(this.selectedItemKey);

                this.uxMainTab.Items.Add(reader);
            }
            catch (Exception err)
            {
                ErrorLog.ShowError(err);
            }
        }

        #endregion

        private void LoadDynamicMenus()
        {
            Task<List<BibleModel>> biblesTask = BiblesData.Database.GetBibles();

            foreach (BibleModel bible in biblesTask.Result.OrderBy(n => n.BibleName))
            {
                MenuItem item = new MenuItem { Header = bible.BibleName, Tag = bible.BiblesId };

                item.Click += this.MenuBiblesItem_Clicked;

                this.uxMenuBiles.Items.Add(item);
            }
        }

        private void InitializeTabs()
        {
            this.uxLeftTab.Items.Add(this.uxIndexer);

            this.uxIndexer.ChapterChanged += this.SeletedChapter_Changed;

            this.uxIndexer.VerseChanged += this.SelectedVerse_Changed;

            Reader.Reader reader = this.CreateReader(false);

            this.uxMainTab.Items.Add(reader);
        }

        private void LoadReader(int bibleId, string verseKey)
        {
            Reader.Reader reader = this.CreateReader(true);

            this.uxMainTab.Items.Add(reader);

            reader.SetBible(bibleId);

            if (!verseKey.IsNullEmptyOrWhiteSpace())
            {
                string bibleKey = Formatters.ChangeBible(verseKey, bibleId);

                reader.SetChapter(bibleKey);

                reader.SetVerse(bibleKey);
            }
        }

        private Reader.Reader CreateReader(bool showCloseButton)
        {
            Reader.Reader reader = new Reader.Reader { ShowCloseButton = showCloseButton };

            reader.BibleChanged += this.OnSelectedTabBible_Changed;

            reader.SelectedVerseChanged += this.OnReaderSelectedVerse_Changed;

            return reader;
        }
    }
}
