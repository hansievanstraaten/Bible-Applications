using Bibles.BookIndex;
using Bibles.Common;
using Bibles.Data;
using System;
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

            this.Initialize();
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

        private void Initialize()
        {
            this.uxLeftTab.Items.Add(this.uxIndexer);

            this.uxIndexer.BookChange += this.SlectedBook_Changed;

            this.uxIndexer.ChapterChanged += this.SeletedChapter_Changed;

            this.uxIndexer.VerseChanged += this.SelectedVerse_Changed;
        }  
    }
}
