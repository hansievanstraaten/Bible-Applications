using SQLite;

namespace Bibles.DataResources.Models
{
    public class LinkModel
    {
        [PrimaryKey, MaxLength(50)]
        public string LinkKeyId { get; set; }

        public string Comments { get; set; }
    }
}
