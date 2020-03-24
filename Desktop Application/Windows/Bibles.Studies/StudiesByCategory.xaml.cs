using Bibles.Common;
using Bibles.DataResources;
using Bibles.DataResources.Aggregates;
using System;
using System.Windows;
using ViSo.Dialogs.Controls;
using WPF.Tools.BaseClasses;

namespace Bibles.Studies
{
    /// <summary>
    /// Interaction logic for StudiesByCategory.xaml
    /// </summary>
    public partial class StudiesByCategory : UserControlBase
    {
        private StudyHeaderModel selectedStudyHeader;

        private StudyHeaderModel[] categoryStudyHeaders;

        public StudiesByCategory()
        {
            this.InitializeComponent();

            this.DataContext = this;
        }

        public StudyHeaderModel SelectedStudyHeader
        {
            get
            {
                return this.selectedStudyHeader;
            }

            set
            {
                this.selectedStudyHeader = value;

                base.OnPropertyChanged(() => this.SelectedStudyHeader);
            }
        }

        public StudyHeaderModel[] CategoryStudyHeaders
        {
            get
            {
                return this.categoryStudyHeaders;
            }

            set
            {
                this.categoryStudyHeaders = value;

                base.OnPropertyChanged(() => this.CategoryStudyHeaders);
            }
        }

        private void SelectedCategory_Changed(object sender, System.Windows.RoutedPropertyChangedEventArgs<object> e)
        {
            try
            {
                StudyCategoryModel category = this.uxStudyCategories.SelectedCategory;

                if (category == null)
                {
                    this.CategoryStudyHeaders = new StudyHeaderModel[] { };

                    return;
                }

                this.CategoryStudyHeaders = BiblesData.Database.GetStudyHeaderByCategory(category.StudyCategoryId).ToArray();
            }
            catch (Exception err)
            {
                ErrorLog.ShowError(err);
            }
        }

        private void EditStudy_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (this.SelectedStudyHeader == null)
            {
                MessageBox.Show("Please select a Study.");

                return;
            }

            try
            {
                EditStudy edit = new EditStudy(this.SelectedStudyHeader);

                ControlDialog.Show(this.SelectedStudyHeader.StudyName, edit, "SaveStudy", autoSize:false);
            }
            catch (Exception err)
            {
                ErrorLog.ShowError(err);
            }
        }
    }
}
