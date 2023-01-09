using SafeShare.Models;

namespace SafeShare.DAL;

public interface IUserRepository
{
    Task<User?> GetUserByEmail(string email);
    Task InsertUser(User user);
    Task<bool> CheckIsUserPresentByEmail(string email);
}