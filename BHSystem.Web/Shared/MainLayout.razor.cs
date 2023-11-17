using BHSystem.Web.Constants;
using BHSystem.Web.Core;
using BHSystem.Web.Features.Admin;
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
    public partial class MainLayout
    {
        [Inject] AuthenticationStateProvider? _authenticationStateProvider { get; set; }
        [Inject] private ILoadingCore? _spinner { get; set; }
        [Inject] private ILogger<MainLayout>? _logger { get; init; }
        [Inject] private IToastService? _toastService { get; set; }
        [Inject] private IApiService? _apiService { get; set; }
        public string breadcumb { get; set; } = "";
        public DateTime dt { get; set; } = DateTime.Now;
        public string FullName { get; set; } = "";
        public string UserName { get; set; } = "";
        public int UserId { get; set; } = -1;
        public bool IsSupperAdmin { get; set; } = false;
        public List<MenuModel>? ListMenus { get; set; }
        private bool preventOnAfterRender { get; set; } = false;

        protected override void OnInitialized()
        {
            preventOnAfterRender = false;
            base.OnInitialized();
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender && !preventOnAfterRender)
            {
                try
                {
                    var oUser = await ((Providers.ApiAuthenticationStateProvider)_authenticationStateProvider!).GetAuthenticationStateAsync();
                    if (oUser != null)
                    {
                        UserName = oUser.User.Claims.FirstOrDefault(m => m.Type == "UserName")?.Value + "";
                        IsSupperAdmin = oUser.User.Claims.FirstOrDefault(m => m.Type == "IsAdmin")?.Value + "" == "Admin";
                        FullName = oUser.User.Claims.FirstOrDefault(m => m.Type == "FullName")?.Value + "";
                        UserId = int.Parse(oUser.User.Claims.FirstOrDefault(m => m.Type == "UserId")?.Value + "");
                        await showLoading(true);
                        await getMenu();
                    }
                }
                catch (Exception ex)
                {
                    _logger?.LogError(ex, "OnAfterRenderAsync");
                    _toastService!.ShowError(ex.Message);
                }
                finally
                {
                    await showLoading(false);
                    await InvokeAsync(StateHasChanged);
                }

            }
        }

        #region "Private Functions"
        private async Task showLoading(bool isShow = true)
        {
            if (isShow) { _spinner!.Show(); await Task.Yield(); }
            else _spinner!.Hide();
        }

        private async Task getMenu()
        {
            ListMenus = new List<MenuModel>();
            Dictionary<string, object> pParams = new Dictionary<string, object>()
            {
                {"pUserId", $"{UserId}"}
            };
            string resString = await _apiService!.GetData(EndpointConstants.URL_MENU_GET_BY_USER, pParams);
            if (!string.IsNullOrEmpty(resString)) ListMenus = JsonConvert.DeserializeObject<List<MenuModel>>(resString);
        }
        #endregion
    }


}
