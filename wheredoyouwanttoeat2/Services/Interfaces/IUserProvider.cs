using System.Threading.Tasks;
using wheredoyouwanttoeat2.Models;

namespace wheredoyouwanttoeat2.Services.Interfaces
{
    public interface IUserProvider
    {
        string GetUserId();

        Task<User> GetLoggedInUserAsync();

        Task<User> GetByEmail(string email);
    }
}