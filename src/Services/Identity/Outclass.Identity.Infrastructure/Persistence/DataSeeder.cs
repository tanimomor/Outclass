using Microsoft.EntityFrameworkCore;
using Outclass.Identity.Application.Services;
using Outclass.Identity.Domain.Entities;
using Outclass.Identity.Infrastructure.Persistence;

namespace Outclass.Identity.Infrastructure;

public static class DataSeeder
{
    public static async Task SeedAsync(IdentityDbContext db, IPasswordHasher passwordHasher)
    {
        // System Tenant ID (e.g., hardcoded for consistent admin access)
        var systemTenantId = Guid.Parse("11111111-1111-1111-1111-111111111111");

        // Seed Roles
        var adminRole = await db.Roles.IgnoreQueryFilters()
            .FirstOrDefaultAsync(r => r.Name == "Admin" && r.TenantId == systemTenantId);

        if (adminRole == null)
        {
            adminRole = Role.Create(systemTenantId, "Admin", "System Administrator", true);
            db.Roles.Add(adminRole);
        }

        var memberRole = await db.Roles.IgnoreQueryFilters()
            .FirstOrDefaultAsync(r => r.Name == "Member" && r.TenantId == systemTenantId);

        if (memberRole == null)
        {
            memberRole = Role.Create(systemTenantId, "Member", "Standard User", true);
            db.Roles.Add(memberRole);
        }

        await db.SaveChangesAsync();

        // Seed Admin User
        var adminEmail = "admin@outclass.com";
        var adminUser = await db.Users.IgnoreQueryFilters()
            .FirstOrDefaultAsync(u => u.Email == adminEmail && u.TenantId == systemTenantId);

        if (adminUser == null)
        {
            var passwordHash = passwordHasher.Hash("Admin123!");
            adminUser = User.Create(systemTenantId, adminEmail, passwordHash, "System", "Admin");
            adminUser.AssignRole(adminRole!);
            adminUser.Activate();
            
            db.Users.Add(adminUser);
            await db.SaveChangesAsync();
        }
    }
}
