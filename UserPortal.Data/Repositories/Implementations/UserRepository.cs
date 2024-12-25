using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using UserPortal.Data.Context;
using UserPortal.Data.Entities;
using UserPortal.Data.Repositories.Interfaces;

namespace UserPortal.Data.Repositories.Implementations;

public class UserRepository : GenericRepository<User>, IUserRepository
{
    private readonly ILogger<UserRepository> _logger;

    public UserRepository(
        ApplicationDbContext context,
        ILogger<UserRepository> logger) : base(context)
    {
        _logger = logger;
    }

    public async Task<bool> IsEmailInUseAsync(string email)
    {
        return await _dbSet.AnyAsync(u => 
            u.Email.ToLower() == email.ToLower() && 
            u.IsActive);
    }

    public async Task<User?> GetByUsernameOrEmailAsync(string usernameOrEmail)
    {
        return await _dbSet
            .Include(u => u.Role)
            .FirstOrDefaultAsync(u => 
                (u.Username.ToLower() == usernameOrEmail.ToLower() || 
                 u.Email.ToLower() == usernameOrEmail.ToLower()) && 
                u.IsActive);
    }

    public async Task AddRefreshTokenAsync(RefreshToken refreshToken)
    {
        var user = await _dbSet.FindAsync(refreshToken.UserId);
        if (user != null)
        {
            user.RefreshTokens.Add(refreshToken);
            _context.Set<RefreshToken>().Add(refreshToken);
        }
    }

    public async Task UpdateLastLoginAsync(int userId)
    {
        var user = await _dbSet.FindAsync(userId);
        if (user != null)
        {
            user.LastLogin = DateTime.UtcNow;
            _dbSet.Update(user);
        }
    }

    public IQueryable<User> GetQueryable()
    {
        return _dbSet.AsQueryable();
    }

    public override async Task<User?> GetByIdAsync(int id)
    {
        return await _dbSet
            .Include(u => u.Role)
            .FirstOrDefaultAsync(u => u.Id == id && u.IsActive);
    }
}