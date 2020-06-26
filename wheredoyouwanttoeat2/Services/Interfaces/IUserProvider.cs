using System.Threading.Tasks;
using WhereDoYouWantToEat2.Models;

namespace WhereDoYouWantToEat2.Services.Interfaces
{
    public interface IUserProvider
    {
        string GetUserId();

        Task<User> GetLoggedInUserAsync();

        Task<User> GetByEmail(string email);
    }
}