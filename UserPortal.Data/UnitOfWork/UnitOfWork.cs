using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using UserPortal.Data.Context;
using UserPortal.Data.Repositories.Implementations;
using UserPortal.Data.Repositories.Interfaces;

namespace UserPortal.Data.UnitOfWork;

public class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _context;
    private readonly ILoggerFactory _loggerFactory;
    private IUserRepository? _userRepository;
    private IRoleRepository? _roleRepository;
    private bool _disposed;

    public UnitOfWork(
        ApplicationDbContext context,
        ILoggerFactory loggerFactory)
    {
        _context = context;
        _loggerFactory = loggerFactory;
    }

    public IUserRepository Users => 
        _userRepository ??= new UserRepository(
            _context, 
            _loggerFactory.CreateLogger<UserRepository>());

    public IRoleRepository Roles =>
        _roleRepository ??= new RoleRepository(_context);

    public async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed && disposing)
        {
            _context.Dispose();
        }
        _disposed = true;
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
}