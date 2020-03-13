using Bibles.Common;
using GeneralExtensions;
using System;
using System.Collections.Generic;
using WPF.Tools.Attributes;
using WPF.Tools.BaseClasses;
using WPF.Tools.ModelViewer;
using WPF.Tools.ToolModels;

namespace Bible.Models.Bookmarks
{
    [ModelNameAttribute("Bookmark")]
    public class BookmarkModel : ModelsBase
    {
        private string bookMarkName;

        private string description;
        
        private string selectedVerse;
        
        private string verseRangeEnd;
        
        private DateTime bookmarkDate;

        [FieldInformationAttribute("Name", Sort = 1)]
        public string BookMarkName 
        {
            get
            {
                return this.bookMarkName;
            }

            set
            {
                this.bookMarkName = value;

                base.OnPropertyChanged(() => this.BookMarkName);
            }
        }

        [FieldInformationAttribute("Description", Sort = 2)]
        [BrowseButtonAttribute("DescriptionBrowse", "", "Edit")]
        public string Description
        {
            get
            {
                return this.description;
            }

            set
            {
                this.description = value;

                base.OnPropertyChanged(() => this.Description);
            }
        }

        [FieldInformationAttribute("Selected Verse", Sort = 3, IsReadOnly = true)]
        public string SelectedVerse
        {
            get
            {
                return this.selectedVerse;
            }

            set
            {
                this.selectedVerse = value;

                base.OnPropertyChanged(() => this.SelectedVerse);
            }
        }

        [FieldInformationAttribute("To Verse", Sort = 4)]
        [ValuesSource("ToVersesRange")]
        [ItemType(ModelItemTypeEnum.ComboBox, IsComboboxEditable = false)]
        public string VerseRangeEnd
        {
            get
            {
                return this.verseRangeEnd;
            }

            set
            {
                this.verseRangeEnd = value;

                base.OnPropertyChanged(this.VerseRangeEnd);
            }
        }
    
        public DateTime BookmarkDate
        {
            get
            {
                return this.bookmarkDate;
            }

            set
            {
                this.bookmarkDate = value;

                base.OnPropertyChanged(() => this.BookmarkDate);
            }
        }
    
        public DataItemModel[] ToVersesRange
        {
            get;

            set;
        }
    
        public void SetVerse(string verseKey)
        {
            this.SelectedVerse = this.InvokeMethod("Bibles.Data.GlobalInvokeData,Bibles.Data", "GetKeyDescription", new object[] { verseKey }).ParseToString();

            int chapterVerseCount = this.InvokeMethod("Bibles.Data.GlobalInvokeData,Bibles.Data", "GetChapterVerseCount", new object[] { verseKey }).ToInt32();

            int selectedVerse = Formatters.GetVerseFromKey(verseKey);

            List<DataItemModel> toRangeList = new List<DataItemModel>();

            while(selectedVerse < chapterVerseCount)
            {
                ++selectedVerse;

                toRangeList.Add(new DataItemModel { DisplayValue = selectedVerse.ToString(), ItemKey = selectedVerse });
            }

            this.ToVersesRange = toRangeList.ToArray();
            //Formatters.GetBibleFromKey
        }
    }
}
