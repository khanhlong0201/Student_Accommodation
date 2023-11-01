using BHSystem.Web.Constants;
using BHSystem.Web.Core;
using BHSystem.Web.Providers;
using BHSytem.Models.Models;
using Blazored.Toast.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.WebUtilities;
using Newtonsoft.Json;

namespace BHSystem.Web.Features.Admin
{
    public partial class RoleUserPage
    {
        [Inject] private ILogger<RoleUserPage>? _logger { get; init; }
        [Inject] private ILoadingCore? _spinner { get; set; }
        [Inject] private IToastService? _toastService { get; set; }
        [Inject] private IApiService? _apiService { get; set; }
        [Inject] NavigationManager? _navigationManager { get; set; }
        public List<UserModel>? ListUserEmpties { get; set; } // ds user chưa được vào nhóm
        public IEnumerable<UserModel>? SelectedUserEmpties { get; set; } = new List<UserModel>(); // ds chọn

        public List<UserModel>? ListUserRoles { get; set; } // ds user được vào nhóm
        public IEnumerable<UserModel>? SelectedUserRoles { get; set; } = new List<UserModel>(); // ds chọn

        public string pRoleName { get; set; } = "";
        public int pRoleId { get; set; } = -1;

        #region "Override Functions"
        protected override Task OnInitializedAsync()
        {
            try
            {
                // đọc giá tri câu query
                var uri = _navigationManager?.ToAbsoluteUri(_navigationManager.Uri);
                if (uri != null)
                {
                    var queryStrings = QueryHelpers.ParseQuery(uri.Query);
                    if (queryStrings.Count() > 0 && queryStrings.TryGetValue("key", out var key))
                    {
                        Dictionary<string, string> pParams = JsonConvert.DeserializeObject<Dictionary<string, string>>(EncryptHelper.Decrypt(key + ""));
                        if (pParams != null && pParams.Any())
                        {
                            if (pParams.ContainsKey("RoleId")) pRoleId = Convert.ToInt32(pParams["RoleId"]);
                            if (pParams.ContainsKey("RoleName")) pRoleName = pParams["RoleName"];
                        }    
                    }    
                }    
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "OnInitializedAsync");
            }
            return base.OnInitializedAsync();
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if(firstRender)
            {
                try
                {
                    await showLoading();
                    if (pRoleId > 0)await getListUser();
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

        #endregion

        #region "Private Functions"
        private async Task showLoading(bool isShow = true)
        {
            if (isShow) { _spinner!.Show(); await Task.Yield(); }
            else _spinner!.Hide();
        }

        /// <summary>
        /// lấy 2 danh sách User
        /// 1 cái tồn tại trong role Id -> cái không tồn tại
        /// </summary>
        /// <returns></returns>
        private async Task getListUser()
        {
            Dictionary<string, object> pParams = new Dictionary<string, object>()
            {
                {"pRoleId", $"{pRoleId}"}
            };
            string resString = await _apiService!.GetData("Users/GetUserByRole", pParams);
            if (!string.IsNullOrEmpty(resString))
            {
                Dictionary<string, string> response = JsonConvert.DeserializeObject<Dictionary<string, string>>(resString);
                ListUserEmpties = JsonConvert.DeserializeObject<List<UserModel>>(response["oUserNotExists"]);
                ListUserRoles = JsonConvert.DeserializeObject<List<UserModel>>(response["oUserExists"]);
            }    
        }
        #endregion
    }
}
