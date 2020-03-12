using Bibles.DataResources;
using Bibles.DataResources.Models;
using System.Collections.Generic;
using WPF.Tools.Attributes;
using WPF.Tools.BaseClasses;
using WPF.Tools.ModelViewer;
using WPF.Tools.ToolModels;
using System.Linq;

namespace Bible.Models.AvailableBooks
{
    [ModelNameAttribute("Bibles")]
    public class AvailableBibles : ModelsBase
    {
        private int bibleId;

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

                base.OnPropertyChanged("BibleName");
            }
        }
        
        public DataItemModel[] ListedBibles
        {
            get
            {
                List<BibleModel> bibles = BiblesData.Database.GetBibles().Result;

                List<DataItemModel> result = new List<DataItemModel>();

                foreach(BibleModel model in bibles)
                {
                    result.Add(new DataItemModel { DisplayValue = model.BibleName, ItemKey = model.BiblesId });
                }

                return result.OrderBy(d => d.DisplayValue).ToArray();
            }
        }
    }
}
