using Bibles.DataResources;
using Bibles.DataResources.Models;
using System.Collections.Generic;
using WPF.Tools.Attributes;
using WPF.Tools.BaseClasses;
using WPF.Tools.ModelViewer;
using WPF.Tools.ToolModels;
using System.Linq;
using GeneralExtensions;

namespace Bible.Models.AvailableBooks
{
    [ModelNameAttribute("Bibles")]
    public class ModelsBibleBook : ModelsBase
    {
        private int bibleId;

        private string bibleName;

        [FieldInformationAttribute("Bible")]
        [ItemTypeAttribute(ModelItemTypeEnum.ComboBox, IsComboboxEditable = false)]
        [ValuesSourceAttribute("ListedBibles")]
        public int BibleId
        {
            get
            {
                return this.bibleId;
            }

            set
            {
                this.bibleId = value;

                DataItemModel nameModel = this.ListedBibles.FirstOrDefault(id => id.ItemKey.ToInt32() == value);

                this.BibleName = nameModel == null ? string.Empty : nameModel.DisplayValue;

                base.OnPropertyChanged("BibleId");

                base.OnPropertyChanged("BibleName");
            }
        }
        
        public string BibleName 
        {
            get
            {
                return this.bibleName;
            }

            set
            {
                this.bibleName = value;
            }
        }

        public DataItemModel[] ListedBibles
        {
            get
            {
                List<Bibles.DataResources.Models.BibleModel> bibles = BiblesData.Database.GetBibles().Result;

                List<DataItemModel> result = new List<DataItemModel>();

                foreach(Bibles.DataResources.Models.BibleModel model in bibles)
                {
                    result.Add(new DataItemModel { DisplayValue = model.BibleName, ItemKey = model.BiblesId });
                }

                return result.OrderBy(d => d.DisplayValue).ToArray();
            }
        }
    }
}
