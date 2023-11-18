using BHSystem.Web.Constants;
using BHSystem.Web.Controls;
using BHSystem.Web.Core;
using BHSystem.Web.Providers;
using BHSystem.Web.Services;
using BHSystem.Web.ViewModels;
using BHSytem.Models.Models;
using Blazored.Toast.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Newtonsoft.Json;
using Org.BouncyCastle.Asn1.Ocsp;
using Telerik.Blazor.Components;
using Telerik.Blazor.Resources;

namespace BHSystem.Web.Features.Admin
{
    public partial class RolePage
    {
        [Inject] private ILogger<RolePage>? _logger { get; init; }
        [Inject] private ILoadingCore? _spinner { get; set; }
        [Inject] private IToastService? _toastService { get; set; }
        [Inject] private IApiService? _apiService { get; set; }
        [Inject] NavigationManager? _navigationManager { get; set; }

        public List<RoleModel>? ListRoles { get; set; }
        public IEnumerable<RoleModel>? SelectedRoles { get; set; } = new List<RoleModel>();
        public RoleModel RoleUpdate { get; set; } = new RoleModel();
        public bool IsCreate { get; set; } = true;
        public bool IsShowDialog { get; set; }
        public bool IsShowDialogUserRole { get; set; }
        public bool IsShowDialogRoleMenu { get; set; }
        public EditContext? _EditContext { get; set; }
        public BHConfirm? _rDialogs { get; set; }

        [CascadingParameter(Name = "pUserId")]
        private int pUserId { get; set; } // giá trị từ MainLayout

        [CascadingParameter(Name = "pIsSupperAdmin")]
        private bool pIsSupperAdmin { get; set; } // giá trị từ MainLayout
        [CascadingParameter(Name = "pListMenus")]
        private List<MenuModel>? ListMenus { get; set; } // giá trị từ MainLayout

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
                    await getDataRole();
                }
                catch (Exception ex)
                {
                    _logger!.LogError(ex, "OnAfterRenderAsync");
                    _toastService!.ShowError(ex.Message);
                }
                finally
                {
                    await InvokeAsync(StateHasChanged);
                    await showLoading(false);
                }
            }
        }

        #region "Private Functions"
        private async Task showLoading(bool isShow = true)
        {
            if (isShow) { _spinner!.Show(); await Task.Yield(); }
            else _spinner!.Hide();
        }

        /// <summary>
        /// lấy danh sách quyền
        /// </summary>
        /// <param name="isLoading"></param>
        /// <returns></returns>
        private async Task getDataRole()
        {
            ListRoles = new List<RoleModel>();
            SelectedRoles = new List<RoleModel>();
            string resString = await _apiService!.GetData(EndpointConstants.URL_ROLE_GETALL);
            if (!string.IsNullOrEmpty(resString)) ListRoles = JsonConvert.DeserializeObject<List<RoleModel>>(resString);
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
                await getDataRole();
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

        /// <summary>
        /// mở popup
        /// </summary>
        protected void OnOpenDialogHandler(EnumType pAction = EnumType.Add, RoleModel? pItemDetails = null)
        {
            IsShowDialog = true;
            try
            {

                if (pAction == EnumType.Add)
                {
                    RoleUpdate = new RoleModel();
                    IsCreate = true;
                }
                else
                {
                    RoleUpdate.Id = pItemDetails!.Id;
                    RoleUpdate.Name = pItemDetails!.Name;
                    IsCreate = false;
                }
                IsShowDialog = true;
                _EditContext = new EditContext(RoleUpdate);
            }
            catch (Exception ex)
            {
                _logger!.LogError(ex, "OnOpenDialogHandler");
                _toastService!.ShowError(ex.Message);
            }
        }

        /// <summary>
        /// Thêm/Cập nhật thông tin người dùng
        /// </summary>
        /// <param name="pEnum"></param>
        /// <returns></returns>
        protected async void SaveDataHandler(EnumType pEnum = EnumType.SaveAndClose)
        {
            try
            {
                string sMessage = "Thêm";
                string sAction = nameof(EnumType.Add);
                if (RoleUpdate.Id > 0)
                {
                    sAction = nameof(EnumType.Update);
                    sMessage = "Cập nhật";
                }
                var checkData = _EditContext!.Validate();
                if (!checkData) return;
                await showLoading();
                RequestModel request = new RequestModel()
                {
                    Json = JsonConvert.SerializeObject(RoleUpdate),
                    Type = sAction,
                    UserId = pUserId
                };
                string resString = await _apiService!.AddOrUpdateData(EndpointConstants.URL_ROLE_UPDATE, request);
                if (!string.IsNullOrEmpty(resString))
                {
                    _toastService!.ShowSuccess($"Đã {sMessage} thông tin quyền.");
                    await getDataRole();
                    if (pEnum == EnumType.SaveAndCreate)
                    {
                        RoleUpdate = new RoleModel();
                        _EditContext = new EditContext(RoleUpdate);
                        return;
                    }
                    IsShowDialog = false;
                }
            }
            catch (Exception ex)
            {
                _logger!.LogError(ex, "SaveDataHandler");
                _toastService!.ShowError(ex.Message);
            }
            finally
            {
                await showLoading(false);
                await InvokeAsync(StateHasChanged);
            }
        }

        /// <summary>
        /// conrfirm xóa user
        /// </summary>
        /// <returns></returns>
        protected async Task ConfirmDeleteHandler()
        {
            if (SelectedRoles == null || !SelectedRoles.Any())
            {
                _toastService!.ShowWarning("Vui lòng chọn dòng để xóa");
                return;
            }
            var confirm = await _rDialogs!.ConfirmAsync(" Bạn có chắc muốn xóa các dòng được chọn? ");
            if (confirm)
            {
                try
                {
                    await showLoading();
                    var oDelete = SelectedRoles.Select(m => new { m.Id });
                    RequestModel request = new RequestModel()
                    {
                        Json = JsonConvert.SerializeObject(oDelete),
                        UserId = pUserId
                    };
                    string resString = await _apiService!.AddOrUpdateData(EndpointConstants.URL_ROLE_DELETE, request);
                    await getDataRole();
                    if (!string.IsNullOrEmpty(resString))
                    {
                        _toastService!.ShowSuccess($"Đã xóa thông tin quyền.");
                        await getDataRole();
                    }
                }
                catch (Exception ex)
                {
                    _logger!.LogError(ex, "ConfirmDeleteHandler");
                    _toastService!.ShowError(ex.Message);
                }
                finally
                {
                    await showLoading(false);
                    await InvokeAsync(StateHasChanged);
                }
            }
        }

        protected void OnRowDoubleClickHandler(GridRowClickEventArgs args) => OnOpenDialogHandler(EnumType.Update, args.Item as RoleModel);

        protected void AuthRoleHandler(EnumType pEnum = EnumType.RoleUser)
        {
            try
            {
                // local function
                bool checkUser(out string key)
                {
                    if (SelectedRoles == null || !SelectedRoles.Any())
                    {
                        _toastService!.ShowWarning("Vui lòng chọn nhóm quyền.");
                        key = "";
                        return false;
                    }
                    if (SelectedRoles.Count() > 1)
                    {
                        _toastService!.ShowWarning("Chỉ được phép chọn một nhóm quyền.");
                        key = "";
                        return false;
                    }
                    var data = SelectedRoles.First();
                    Dictionary<string, string> pParams = new Dictionary<string, string>
                    {
                        { "RoleId", $"{data.Id}" },
                        { "RoleName", $"{data.Name}" }
                    };
                    key = EncryptHelper.Encrypt(JsonConvert.SerializeObject(pParams)); // mã hóa key phân quyền
                    return true;
                }
                string skeyData = "";
                switch (pEnum)
                {
                    case EnumType.RoleUser:
                        if(checkUser(out skeyData)) _navigationManager!.NavigateTo($"/admin/role/role-user?key={skeyData}");
                        break;
                    case EnumType.RoleMenu:
                        if (checkUser(out skeyData)) _navigationManager!.NavigateTo($"/admin/role/role-menu?key={skeyData}");
                        break;
                    default:
                        _toastService!.ShowError("Không xác định Event. Vui lòng liên hệ IT để được hổ trợ");
                        break;
                }
            }
            catch (Exception ex)
            {
                _logger!.LogError(ex, "AuthRoleHandler");
                _toastService!.ShowError(ex.Message);
            }
        }
        #endregion
    }
}
