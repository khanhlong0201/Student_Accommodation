using BHSystem.Web.Features.Admin;
using BHSystem.Web.Services;
using BHSystem.Web.ViewModels;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;

namespace BHSystem.Web.Shared
{
    public partial class MainLayout
    {
        [Inject] public BHDialogService? _bhDialoginService { get; set; }
        [Inject] AuthenticationStateProvider? _authenticationStateProvider { get; set; }
        public string breadcumb { get; set; } = "";
        public DateTime dt { get; set; } = DateTime.Now;
        public string FullName { get; set; } = "";
        public string UserName { get; set; } = "";

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                var oUser = await ((Providers.ApiAuthenticationStateProvider)_authenticationStateProvider!).GetAuthenticationStateAsync();
                if (oUser != null)
                {
                    UserName = oUser.User.Claims.FirstOrDefault(m => m.Type == "UserName")?.Value + "";
                    FullName = oUser.User.Claims.FirstOrDefault(m => m.Type == "FullName")?.Value + "";
                    StateHasChanged();
                }
            }
        }
    }

    
}
