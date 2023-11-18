using BHSystem.Web.Constants;
using BHSystem.Web.Core;
using BHSystem.Web.Features.Admin;
using BHSystem.Web.Providers;
using BHSystem.Web.Services;
using BHSystem.Web.ViewModels;
using BHSytem.Models.Entities;
using BHSytem.Models.Models;
using Blazored.Toast.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Newtonsoft.Json;
using System.Runtime.CompilerServices;

namespace BHSystem.Web.Shared
{
    public partial class ClientLayout
    {
        [Inject] AuthenticationStateProvider? _authenticationStateProvider { get; set; }
        [Inject] private ILoadingCore? _spinner { get; set; }
        [Inject] private ILogger<MainLayout>? _logger { get; init; }
        [Inject] private IToastService? _toastService { get; set; }
        [Inject] private IApiService? _apiService { get; set; }
        [Inject] NavigationManager? _navigationManager { get; set; }
        public string breadcumb { get; set; } = "";
        public DateTime dt { get; set; } = DateTime.Now;
        public string FullName { get; set; } = "";
        public string UserName { get; set; } = "";
        public int UserId { get; set; } = -1;
        public List<MenuModel>? ListMenus { get; set; }

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            try
            {
                var oUser = await ((Providers.ApiAuthenticationStateProvider)_authenticationStateProvider!).GetAuthenticationStateAsync();
                if (oUser != null)
                {
                    UserName = oUser.User.Claims.FirstOrDefault(m => m.Type == "UserName")?.Value + "";
                    FullName = oUser.User.Claims.FirstOrDefault(m => m.Type == "FullName")?.Value + "";
                    UserId = int.Parse(oUser.User.Claims.FirstOrDefault(m => m.Type == "UserId")?.Value + "");
                    //await showLoading(true);
                }
            }
            catch(Exception)
            {

            }
        }

        //protected override async Task OnAfterRenderAsync(bool firstRender)
        //{
        //    if (firstRender)
        //    {
        //        try
        //        {
        //            var oUser = await ((Providers.ApiAuthenticationStateProvider)_authenticationStateProvider!).GetAuthenticationStateAsync();
        //            if (oUser != null)
        //            {
        //                UserName = oUser.User.Claims.FirstOrDefault(m => m.Type == "UserName")?.Value + "";
        //                FullName = oUser.User.Claims.FirstOrDefault(m => m.Type == "FullName")?.Value + "";
        //                UserId = int.Parse(oUser.User.Claims.FirstOrDefault(m => m.Type == "UserId")?.Value + "");
        //                await showLoading(true);
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            _logger?.LogError(ex, "OnAfterRenderAsync");
        //            //_toastService!.ShowError(ex.Message);
        //        }
        //        finally
        //        {
        //            await showLoading(false);
        //            await InvokeAsync(StateHasChanged);
        //        }

        //    }
        //}

        #region "Private Functions"
        private async Task showLoading(bool isShow = true)
        {
            if (isShow) { _spinner!.Show(); await Task.Yield(); }
            else _spinner!.Hide();
        }


        #endregion

        protected void NavigatoEncrypt()
        {
            Dictionary<string, string> pParams = new Dictionary<string, string>
            {
                { "UserId", $"{UserId}" },
            };
            string key = EncryptHelper.Encrypt(JsonConvert.SerializeObject(pParams)); // mã hóa key phân quyền
            _navigationManager!.NavigateTo($"/admin/info-user?key={key}");
        }
    }


}
