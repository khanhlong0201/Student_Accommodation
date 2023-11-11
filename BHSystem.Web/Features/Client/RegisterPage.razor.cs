using BHSystem.Web.Constants;
using BHSystem.Web.Core;
using BHSystem.Web.Services;
using BHSystem.Web.ViewModels;
using BHSytem.Models.Models;
using Blazored.Toast.Services;
using Microsoft.AspNetCore.Components;
using Newtonsoft.Json;

namespace BHSystem.Web.Features.Client
{
    public partial class RegisterPage
    {
        [Inject] NavigationManager? _navigationManager { get; set; }
        [Inject] private IToastService? _toastService { get; set; }
        [Inject] private IApiService? _service { get; set; }
        [Inject] private ILoadingCore? _spinner { get; set; }
        public RegisterViewModel RegisterRequest { get; set; } = new RegisterViewModel();
        public bool IsLoading { get; set; }
        public string ErrorMessage = "";


        private async Task showLoading(bool isShow = true)
        {
            if (isShow) { _spinner!.Show(); await Task.Yield(); }
            else _spinner!.Hide();
        }

        protected async Task LoginHandler()
        {
            try
            {
                await showLoading();
                ErrorMessage = "";
                IsLoading = true;
                RequestModel request = new RequestModel()
                {
                    Json = JsonConvert.SerializeObject(RegisterRequest),
                };
                var resString = await _service!.AddOrUpdateData(EndpointConstants.URL_USER_REGISTER, request);
                var response = JsonConvert.DeserializeObject<ResponseModel<UserModel>>(resString);
                if (response != null && response.StatusCode == 200)
                {
                    _toastService?.ShowSuccess(response?.Message);
                    _navigationManager!.NavigateTo("/admin/user");
                }
            }
            catch (Exception ex) {
                ErrorMessage = ex.Message; 
                await showLoading(false);
            }
            finally
            {
                await showLoading(false);
                IsLoading = false;
            }
        }
    }
}
