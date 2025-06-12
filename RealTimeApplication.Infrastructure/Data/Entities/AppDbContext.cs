using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace RealTimeApplication.Infrastructure.Data.Entities;

public class AppDbContext : IdentityDbContext<ApplicationUser>
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
    : base(options)
    {

    }
    public DbSet<ApplicationUser> AppUsers { get; set; } = default!;
    public DbSet<FriendRequests> FriendRequests { get; set; } = default!;
    public DbSet<Messages> Messages { get; set; } = default!;
    public DbSet<FriendInvite> FriendInvites { get; set; } = default!;
}