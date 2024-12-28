
namespace RealTimeApplication.Infrastructure.Data.Entities;

public class BaseEntity
{
    public long Id { get; set; }
    public DateTimeOffset TimeCreated { get; set; }
    public DateTimeOffset TimeUpdated { get; set; }
}
