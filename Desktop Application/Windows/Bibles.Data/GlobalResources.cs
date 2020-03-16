using Bibles.DataResources;
using Bibles.DataResources.Models;
using System.Collections.Generic;
using System.Linq;
using WPF.Tools.ToolModels;

namespace Bibles.Data
{
    public static class GlobalResources
    {
        public static UserPreferenceModel UserPreferences
        {
            get
            {
                return BiblesData.Database.GetPreferences().Result;
            }
        }

        public static DataItemModel[] ListedBibles()
        {
            List<BibleModel> bibles = BiblesData.Database.GetBibles().Result;

            List<DataItemModel> result = new List<DataItemModel>();

            foreach (Bibles.DataResources.Models.BibleModel model in bibles)
            {
                result.Add(new DataItemModel { DisplayValue = model.BibleName, ItemKey = model.BiblesId });
            }

            return result.OrderBy(d => d.DisplayValue).ToArray();
        }
    }
}
