using Travel.Models;
using Travel.Repositories;
namespace Travel.Modules.Users;

public interface IUserRepository : IRepository<User>
{   
    Task<User?> GetByEmailAsync(string email);
    Task<bool> EmailExistsAsync(string email);
    Task<IEnumerable<User>> GetAllByRoleAsync(string? role);
}