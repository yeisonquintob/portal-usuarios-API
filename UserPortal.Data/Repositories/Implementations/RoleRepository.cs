using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using UserPortal.Data.Context;
using UserPortal.Data.Entities;
using UserPortal.Data.Repositories.Interfaces;

namespace UserPortal.Data.Repositories.Implementations;

/// <summary>
/// Implementación del repositorio de roles
/// </summary>
public class RoleRepository : GenericRepository<Role>, IRoleRepository
{
    public RoleRepository(ApplicationDbContext context) : base(context)
    {
    }

    /// <inheritdoc/>
    public async Task<Role?> GetByNameAsync(string name)
    {
        return await _dbSet
            .FirstOrDefaultAsync(r => 
                r.Name.ToLower() == name.ToLower() && 
                r.IsActive);
    }

    /// <inheritdoc/>
    public async Task<bool> IsDefaultRoleAsync(int roleId)
    {
        var role = await _dbSet.FindAsync(roleId);
        if (role == null) return false;
        
        return role.Name == "Admin" || role.Name == "User";
    }

    /// <inheritdoc/>
    public async Task<Role?> GetDefaultRoleAsync()
    {
        return await _dbSet
            .FirstOrDefaultAsync(r => 
                r.Name == "User" && 
                r.IsActive);
    }

    /// <inheritdoc/>
    public async Task<bool> HasUsersAsync(int roleId)
    {
        return await _context.Users
            .AnyAsync(u => 
                u.RoleId == roleId && 
                u.IsActive);
    }
}
