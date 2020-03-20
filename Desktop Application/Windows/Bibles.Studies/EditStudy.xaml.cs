using Bibles.Common;
using Bibles.Studies.Models;
using System;
using WPF.Tools.BaseClasses;

namespace Bibles.Studies
{
    /// <summary>
    /// Interaction logic for EditStudy.xaml
    /// </summary>
    public partial class EditStudy : UserControlBase
    {
        public EditStudy(bool isNewStudy)
        {
            this.InitializeComponent();

            if (isNewStudy)
            {
                this.SubjectHeader = new StudyHeader();
            }

            this.uxSubjectHeader.Items.Add(this.SubjectHeader); ;
        }

        public StudyHeader SubjectHeader { get; set; }

        public bool SaveStudy()
        {
            return true;
        }

        private void SubjectHeder_Browsed(object sender, string buttonKey)
        {
            try
            {

            }
            catch (Exception err)
            {
                ErrorLog.ShowError(err);
            }
        }
    }
}
