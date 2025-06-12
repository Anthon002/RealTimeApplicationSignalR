using System.ComponentModel.DataAnnotations;
using RealTimeApplication.Infrastructure.Models;

namespace RealTimeApplication.Infrastructure.Data.Entities;

public sealed class Messages : BaseEntity
{
    [MaxLength(100)]
    public string ChatToken { get; set; } = default!;

    [MaxLength(50)]
    public string SenderId { get; set; } = default!;

    [MaxLength(50)]
    public string ReceiverId { get; set; } = default!;

    [MaxLength(500)]
    public string Content { get; set; } = default!;
}
