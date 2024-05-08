using DataCollector.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataCollector.Models
{
    public class Store : AuditableEntity
    {
        public string Name { get; set; } = null!;
        public int CategoryId { get; set; }
        public Category Category { get; set; }=null!;
        public string? CollectorNote { get; set; }
        public string Latitude { get; set; } = null!;
        public string Longitude { get; set; }=null!;
        public bool IsThereANearbyStore { get; set; }
        [ForeignKey("CreatedBy")]
        public ApplicationUser CreatedByUser { get; set; }
        public List<StorePhoto> StorePhotos { get; set; } = new();
    }

    public class StorePhoto : AuditableEntity
    {
        public int StoreId { get; set; }
        public Store Store { get; set; }
        public string PhotoURL { get; set; } = null!;
    }
}
