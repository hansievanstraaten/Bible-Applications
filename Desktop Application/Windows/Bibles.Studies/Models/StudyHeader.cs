using WPF.Tools.Attributes;
using WPF.Tools.BaseClasses;

namespace Bibles.Studies.Models
{
    [ModelNameAttribute("Subject")]
    public class StudyHeader : ModelsBase
    {
        private int studyHeaderId;

        private string studyName;
        
        private string author;
        
        private int studyCategory;

        public int StudyHeaderId 
        { 
            get
            {
                return this.studyHeaderId;
            }
            
            set
            {
                this.studyHeaderId = value;

                base.OnPropertyChanged(() => this.StudyHeaderId);
            }
        }

        [FieldInformation("Subject", IsRequired = true, Sort = 1)]
        public string StudyName 
        {             
            get
            {
                return this.studyName;
            }
            
            set
            {
                this.studyName = value;

                base.OnPropertyChanged(() => this.StudyName);
            }
        }

        [FieldInformation("Author", Sort = 2)]
        public string Author 
        { 
            get
            {
                return this.author;
            }
            
            set
            {
                this.author = value;

                base.OnPropertyChanged(() => this.Author);
            }
        }

        [FieldInformation("Category", IsRequired = true, Sort = 3)]
        [BrowseButton("StudyCategoryBrowse", "Search Category", "Search")]
        public int StudyCategory
        { 
            get
            {
                return this.studyCategory;
            }
                        
            set
            {
                this.studyCategory = value;

                base.OnPropertyChanged(() => this.StudyCategory);
            }
        }
    }
}
