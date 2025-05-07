using PizzaShop.ViewModels;

namespace PizzaShop.Service.Interfaces;

public interface IProfileService
{
    public Task<bool> UpdateProfileByEmail(ProfileViewModel model, string userId);
    public Task<ProfileViewModel> GetUserProfile(string? email);

    public Task<string> ChangePassword(ChangePasswordViewModel model, string email);
}
