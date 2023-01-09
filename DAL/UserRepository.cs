using Microsoft.EntityFrameworkCore;

using SafeShare.Models;

namespace SafeShare.DAL;

public class UserRepository : IUserRepository
{
    private readonly UsersContext _usersContext;

    public UserRepository(UsersContext usersContext)
    {
        _usersContext = usersContext;
    }

    public Task<User?> GetUserByEmail(string email)
    {
        return _usersContext.Users.FirstOrDefaultAsync(user => user.Email == email);
    }

    public async Task InsertUser(User user)
    {
        await _usersContext.Users.AddAsync(user);
        await _usersContext.SaveChangesAsync();
    }

    public Task<bool> CheckIsUserPresentByEmail(string email)
    {
        return _usersContext.Users.AnyAsync(user => user.Email == email);
    }
}

