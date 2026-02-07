using App.Core;

namespace App.Infrastructure;

public sealed class PermissionService : IPermissionService
{
    private static readonly IReadOnlyDictionary<Role, HashSet<string>> RolePermissions =
        new Dictionary<Role, HashSet<string>>
        {
            [Role.Admin] = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                "*"
            },
            [Role.Office] = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                "kofre.read",
                "kofre.password.write",
                "meter.read",
                "audit.read",
                "import.execute",
                "export.execute"
            },
            [Role.Field] = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                "kofre.read",
                "meter.read"
            }
        };

    private readonly AppDbContext _dbContext;

    public PermissionService(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<bool> HasPermissionAsync(Guid? userId, string permissionKey, CancellationToken cancellationToken)
    {
        if (userId is null)
        {
            return false;
        }

        var user = await _dbContext.Users.FindAsync([userId], cancellationToken);
        if (user is null || !user.IsActive)
        {
            return false;
        }

        var permissions = RolePermissions[user.Role];
        return permissions.Contains("*") || permissions.Contains(permissionKey);
    }
}
