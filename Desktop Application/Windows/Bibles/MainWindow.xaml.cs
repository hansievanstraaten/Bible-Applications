using Bibles.BookIndex;
using Bibles.Common;
using Bibles.Data;
using Bibles.DataResources;
using GeneralExtensions;
using System;
using System.Windows;
using WPF.Tools.BaseClasses;
using WPF.Tools.TabControl;

namespace Bibles
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : WindowBase
    {
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
        
        private void InitializeTabs()
        {
            this.uxLeftTab.Items.Add(this.uxIndexer);

            this.uxIndexer.ChapterChanged += this.SeletedChapter_Changed;

            this.uxIndexer.VerseChanged += this.SelectedVerse_Changed;

            Reader.Reader reader = new Reader.Reader(); 

            this.uxMainTab.Items.Add(reader);

            reader.SetBible(GlobalResources.UserPreferences.DefaultBible);
        }

    }
}
