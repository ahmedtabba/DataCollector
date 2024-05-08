using DataCollector.Identity;
using DataCollector.Models.Identity;
using DataCollector.Models.Shared;

namespace DataCollector.Services.Interfaces
{
    public interface IIdentityService
    {
        Task<AuthenticationResult> RegisterAsync(RegisterViewModel model);
        Task<AuthenticationResult> CreateUserAsync(AddUserViewModel model);
        Task<AuthenticationResult> LoginAsync(string email, string password);
        Task<AuthenticationResult> ResetPasswordAsync(string email, string password, string userId);
        Task<RequestResult> DeleteAsync(string userId);
        Task<RequestResult> UpdateUserAsync(EditUserVM model, string userId);
        ApplicationUser? GetUserBtId(string userId);
    }
}
