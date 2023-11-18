using BHSystem.Web.Constants;
using BHSystem.Web.Controls;
using BHSystem.Web.Core;
using BHSystem.Web.Providers;
using BHSystem.Web.ViewModels;
using BHSytem.Models.Models;
using Blazored.Toast.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.WebUtilities;
using Newtonsoft.Json;

namespace BHSystem.Web.Features.Admin
{
    public partial class RoleMenuPage
    {
        [Inject] private ILogger<RoleMenuPage>? _logger { get; init; }
        [Inject] private ILoadingCore? _spinner { get; set; }
        [Inject] private IToastService? _toastService { get; set; }
        [Inject] private IApiService? _apiService { get; set; }
        [Inject] NavigationManager? _navigationManager { get; set; }

        public List<MenuModel>? ListMenuEmpties { get; set; } // ds user chưa được vào nhóm
        public IEnumerable<MenuModel>? SelectedMenuEmpties { get; set; } = new List<MenuModel>(); // ds chọn

        public List<MenuModel>? ListMenuRoles { get; set; } // ds user được vào nhóm
        public IEnumerable<MenuModel>? SelectedMenuRoles { get; set; } = new List<MenuModel>(); // ds chọn

        public string pRoleName { get; set; } = "";
        public int pRoleId { get; set; } = -1;
        public BHConfirm? _rDialogs { get; set; }
        [CascadingParameter(Name = "pUserId")]
        private int pUserId { get; set; } // giá trị từ MainLayout

        [CascadingParameter(Name = "pIsSupperAdmin")]
        private bool pIsSupperAdmin { get; set; } // giá trị từ MainLayout

        [CascadingParameter(Name = "pListMenus")]
        private List<MenuModel>? ListMenus { get; set; } // giá trị từ MainLayout
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
                    if (queryStrings.Count() > 0)
                    {
                        string key = uri.Query.Substring(5); // để tránh parse lỗi; sub "?key="
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
            await base.OnAfterRenderAsync(firstRender);
            if (firstRender)
            {
                try
                {
                    var findMenu = ListMenus?.FirstOrDefault(m => m.Link?.ToUpper() == "/admin/role".ToUpper()); // tìm lấy menu không
                    if (findMenu == null)
                    {
                        _toastService!.ShowInfo("Bạn không có quyền vào menu này!!!");
                        await Task.Delay(4500);
                        _navigationManager!.NavigateTo("/admin/logout");
                        return;
                    }
                    await showLoading();
                    if (pRoleId > 0) await getListMenu();
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
        private async Task getListMenu()
        {
            ListMenuEmpties = new List<MenuModel>();
            ListMenuRoles = new List<MenuModel>();
            SelectedMenuEmpties = new List<MenuModel>();
            SelectedMenuRoles = new List<MenuModel>();
            Dictionary<string, object> pParams = new Dictionary<string, object>()
            {
                {"pRoleId", $"{pRoleId}"}
            };
            string resString = await _apiService!.GetData(EndpointConstants.URL_MENU_GET_MENU_ROLE, pParams, isAuth: true);
            if (!string.IsNullOrEmpty(resString))
            {
                Dictionary<string, string> response = JsonConvert.DeserializeObject<Dictionary<string, string>>(resString);
                ListMenuEmpties = JsonConvert.DeserializeObject<List<MenuModel>>(response["oMenuNotExists"]);
                ListMenuRoles = JsonConvert.DeserializeObject<List<MenuModel>>(response["oMenuExists"]);
            }
        }
        #endregion

        #region "Protected Functions"
        /// <summary>
        /// load dữ liệu
        /// </summary>
        /// <returns></returns>
        protected async void ReLoadDataHandler()
        {
            try
            {
                await showLoading();
                await getListMenu();
            }
            catch (Exception ex)
            {
                _logger!.LogError(ex, "ReLoadDataHandler");
                _toastService!.ShowError(ex.Message);
            }
            finally
            {
                await showLoading(false);
                await InvokeAsync(StateHasChanged);
            }
        }

        protected async void AddOrDeleteMenuRoleHandler(EnumType pType = EnumType.Add)
        {
            try
            {
                if(pRoleId <= 0)
                {
                    _toastService!.ShowWarning("Không xác định được nhóm quyền. Liên hệ IT để được hổ trợ");
                    return;
                }    
                string sAction = nameof(EnumType.Add);
                string sMessage = "Thêm";
                IEnumerable<MenuModel>? lstMenus = SelectedMenuEmpties;
                if (pType == EnumType.Delete)
                {
                    sAction = nameof(EnumType.Delete);
                    sMessage = "Xóa";
                    lstMenus = SelectedMenuRoles;
                }
                if (lstMenus == null || !lstMenus.Any())
                {
                    _toastService!.ShowWarning($"Vui lòng chọn người dùng để {sMessage}");
                    return;
                }
                var confirm = await _rDialogs!.ConfirmAsync($"Bạn có chắc muốn {sMessage} người dùng quyền [{pRoleName}]? ");
                if (confirm)
                {
                    await showLoading();
                    var oDelete = lstMenus.Select(m => new { Menu_Id = m.MenuId, Role_Id = pRoleId });
                    RequestModel request = new RequestModel()
                    {
                        Json = JsonConvert.SerializeObject(oDelete),
                        Type = sAction,
                        UserId = pUserId
                    };
                    string resString = await _apiService!.AddOrUpdateData(EndpointConstants.URL_ROLE_MENU_UPDATE, request, isAuth: true);
                    if (!string.IsNullOrEmpty(resString))
                    {
                        _toastService!.ShowSuccess($"Đã {sMessage} thông tin phân quyền.");
                        await getListMenu();
                    }
                }
            }
            catch (Exception ex)
            {
                _logger!.LogError(ex, "AddOrDeleteUserRoleHandler");
                _toastService!.ShowError(ex.Message);
            }
            finally
            {
                await showLoading(false);
                await InvokeAsync(StateHasChanged);
            }
        }
        #endregion
    }
}
