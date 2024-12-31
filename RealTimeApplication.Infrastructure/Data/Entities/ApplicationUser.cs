using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace RealTimeApplication.Infrastructure.Data.Entities;

public sealed class ApplicationUser : IdentityUser
{
    [MaxLength(25)]
    public string UserIdentifier { get; set; } = default!;

    [MaxLength(25)]
    public string FirstName { get; set; } = default!;

    [MaxLength(25)]
    public string LastName { get; set; } = default!;
    public DateTimeOffset TimeCreated { get; set; }
    public DateTimeOffset TimeUpdated { get; set; }
}

public sealed class AppRole : IdentityRole<long> {};
