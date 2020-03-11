using Bibles.BookIndex;
using Bibles.Common;
using Bibles.Data;
using Bibles.DataResources;
using GeneralExtensions;
using System;
using System.Windows;
using WPF.Tools.BaseClasses;

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

            initialData.LoadEmbeddedBibles(this.Dispatcher);

            this.InitializeTabs();
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

        private void SlectedBook_Changed(object sender, string key)
        {
            try
            {

            }
            catch (Exception err)
            {
                ErrorLog.ShowError(err);
            }
        }

        private void SeletedChapter_Changed(object sender, string key)
        {
            try
            {

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

            }
            catch (Exception err)
            {
                ErrorLog.ShowError(err);
            }
        }
        
        private void InitializeTabs()
        {
            this.uxLeftTab.Items.Add(this.uxIndexer);

            this.uxIndexer.BookChange += this.SlectedBook_Changed;

            this.uxIndexer.ChapterChanged += this.SeletedChapter_Changed;

            this.uxIndexer.VerseChanged += this.SelectedVerse_Changed;
        }  
    }
}
