namespace DataCollector.Models
{
    public class AuditableEntity: Entity
    {
        public string CreatedBy { get; set; } = null!;
        public DateTime CreationDate { get; set; }
        public string? LastModifiedBy { get; set; }
        public DateTime? LastModifiedDate { get; set; }
    }

    //public class AuditableEntityDto : Entity
    //{
    //    public string CreatedBy { get; set; } = null!;
    //    public string CreationDate { get; set; }
    //    public string? LastModifiedBy { get; set; }
    //    public string? LastModifiedDate { get; set; }
    //}
}
