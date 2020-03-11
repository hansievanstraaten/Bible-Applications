using SQLite;

namespace Bibles.DataResources.Models
{
    public class BibleModel
    {
        [PrimaryKey, AutoIncrement]
        public int BiblesId { get; set; }

        public string BibleName { get; set; }
    }
}
