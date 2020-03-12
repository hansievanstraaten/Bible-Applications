using Bible.Models.AvailableBooks;
using Bibles.BookIndex;
using Bibles.Common;
using Bibles.Data;
using Bibles.DataResources;
using Bibles.DataResources.Models;
using GeneralExtensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
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

             InitializeData initialData = new InitializeData();

            initialData.InitialDataLoadCompleted += this.InitialDataLoad_Completed;

            initialData.LoadEmbeddedBibles(this.Dispatcher, Application.Current.MainWindow.FontFamily);
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

        private void OnSelectedTabBible_Changed(object sender, BibleBookModel bible)
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

        private void Exit_Cliked(object sender, RoutedEventArgs e)
        {
            this.Dispatcher.InvokeShutdown();
        }

        private void MenuBiblesItem_Clicked(object sender, RoutedEventArgs e)
        {
            try
            {
                MenuItem item = (MenuItem)sender;

                Reader.Reader reader = new Reader.Reader { ShowCloseButton = true };

                reader.BibleBookChanged += this.OnSelectedTabBible_Changed;

                this.uxMainTab.Items.Add(reader);

                reader.SetBible(item.Tag.ToInt32());

                reader.SetChapter(this.selectedItemKey);

                reader.SetVerse(this.selectedItemKey);
            }
            catch (Exception err)
            {
                ErrorLog.ShowError(err);
            }
        }
        
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

            Reader.Reader reader = new Reader.Reader();

            reader.BibleBookChanged += this.OnSelectedTabBible_Changed;
            
            this.uxMainTab.Items.Add(reader);
            
            reader.SetBible(GlobalResources.UserPreferences.DefaultBible);

        }
    }
}
