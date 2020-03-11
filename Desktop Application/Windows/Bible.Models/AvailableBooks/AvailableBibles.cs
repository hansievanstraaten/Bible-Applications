using WPF.Tools.Attributes;
using WPF.Tools.BaseClasses;
using WPF.Tools.ModelViewer;
using WPF.Tools.ToolModels;

namespace Bible.Models.AvailableBooks
{
    [ModelNameAttribute("Bibles")]
    public class AvailableBibles : ModelsBase
    {
        private string bibleName;

        [FieldInformationAttribute("Bible")]
        [ItemTypeAttribute(ModelItemTypeEnum.ComboBox, IsComboboxEditable = false)]
        [ValuesSourceAttribute("ListedBibles")]
        public string BibleName
        {
            get
            {
                return this.bibleName;
            }

            set
            {
                this.bibleName = value;

                base.OnPropertyChanged("BibleName");
            }
        }

        public DataItemModel[] ListedBibles
        {
            get
            {
                //List<DataItemModel> result = new List<DataItemModel>();

                //result.Add(new DataItemModel { DisplayValue = " ", ItemKey = "<Select>" });

                //result.AddRange(GlobalDictionary.GetAvailableBibles().ToArray());

                //return result.ToArray();
                // return GlobalDictionary.GetAvailableBibles().ToArray();

                return null;
            }
        }
    }
}
