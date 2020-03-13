using Bibles.DataResources;
using Bibles.DataResources.Models;

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

            //set
            //{
            //    BiblesData.Database.InsertserPreference(value);
            //}
        }
    }
}
