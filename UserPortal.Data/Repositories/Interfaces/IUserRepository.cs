using System.Threading.Tasks;
using UserPortal.Data.Entities;
using UserPortal.Shared.Models;

namespace UserPortal.Data.Repositories.Interfaces;

public interface IUserRepository : IGenericRepository<User>
{
    Task<bool> IsEmailInUseAsync(string email);
    Task<User?> GetByUsernameOrEmailAsync(string usernameOrEmail);
    Task AddRefreshTokenAsync(RefreshToken refreshToken);
    Task UpdateLastLoginAsync(int userId);
    IQueryable<User> GetQueryable();
}