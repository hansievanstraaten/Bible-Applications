using SQLite;

namespace Bibles.DataResources.Models
{
    public class UserPreferenceModel
    {
        [PrimaryKey, AutoIncrement]
        public int UserId { get; set; }

        public int DefaultBible { get; set; }

        public string Language { get; set; }

        public bool SynchronizzeTabs { get; set; }

        public string Font { get; set; }

        public int FontSize { get; set; }
    }
}
