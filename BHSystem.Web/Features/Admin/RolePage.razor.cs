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
using Org.BouncyCastle.Asn1.Ocsp;
using Telerik.Blazor.Components;
using Telerik.Blazor.Resources;

namespace BHSystem.Web.Features.Admin
{
    public partial class RolePage
    {
        [Inject] private ILogger<UserPage>? _logger { get; init; }
        [Inject] private ILoadingCore? _spinner { get; set; }
        [Inject] private IToastService? _toastService { get; set; }
        [Inject] private IApiService? _apiService { get; set; }

        public List<RoleModel>? ListRoles { get; set; }
        public IEnumerable<RoleModel>? SelectedRoles { get; set; } = new List<RoleModel>();
        public RoleModel RoleUpdate { get; set; } = new RoleModel();
        public bool IsCreate { get; set; } = true;
        public bool IsShowDialog { get; set; }
        public EditContext? _EditContext { get; set; }
        public BHConfirm? _rDialogs { get; set; }

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
                    Type = sAction
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
                        Json = JsonConvert.SerializeObject(oDelete)
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

        #endregion
    }
}
