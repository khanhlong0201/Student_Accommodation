using BHSystem.Web.Constants;
using BHSystem.Web.Core;
using BHSystem.Web.Services;
using BHSystem.Web.ViewModels;
using BHSytem.Models.Models;
using Blazored.Toast.Services;
using Microsoft.AspNetCore.Components;
using Newtonsoft.Json;
using BHSystem.Web.Constants;
using BHSystem.Web.Controls;
using BHSystem.Web.Core;
using BHSystem.Web.Services;
using BHSystem.Web.ViewModels;
using BHSytem.Models.Models;
using Blazored.Toast.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Newtonsoft.Json;
using Telerik.Blazor.Components;
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
                if(!string.IsNullOrEmpty(resString))
                {
                    _toastService?.ShowSuccess("Đăng kí thành công");
                    // Chờ một khoảng thời gian trước khi chuyển hướng
                    await Task.Delay(1000); // Thời gian delay là 2000 milliseconds (2 giây)
                    _navigationManager!.NavigateTo("/client/login");
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
