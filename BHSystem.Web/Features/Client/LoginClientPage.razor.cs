using BHSystem.Web.Services;
using BHSystem.Web.ViewModels;
using BHSytem.Models.Models;
using Microsoft.AspNetCore.Components;

namespace BHSystem.Web.Features.Client
{
    public partial class LoginClientPage
    {
        [Inject] NavigationManager? _navigationManager { get; set; }
        [Inject] private ICliUserService? _userService { get; set; }
        public LoginViewModel LoginRequest { get; set; } = new LoginViewModel();
        public bool IsLoading { get; set; }
        public string ErrorMessage = "";

        protected async Task LoginHandler()
        {
            try
            {
                ErrorMessage = "";
                IsLoading = true;
                await Task.Yield();
                var response = await _userService!.LoginAsync(LoginRequest);
                if (!string.IsNullOrWhiteSpace(response)) { ErrorMessage = response; return; }
                _navigationManager!.NavigateTo("/");
            }
            catch (Exception ex) { ErrorMessage = ex.Message; }
            finally
            {
                await Task.Delay(200);
                IsLoading = false;
            }
        }
    }
}
