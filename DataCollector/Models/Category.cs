namespace DataCollector.Models
{
    public class Category: AuditableEntity
    {
        public string Code { get; set; } = null!;
        public string Name { get; set; } = null!;
    }
}
