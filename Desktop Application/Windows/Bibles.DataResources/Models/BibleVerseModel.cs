using SQLite;

namespace Bibles.DataResources.Models
{
    public class BibleVerseModel
    {
        [PrimaryKey]
        public string BibleVerseKey { get; set; }
        
        public string VerseText { get; set; }
    }
}
