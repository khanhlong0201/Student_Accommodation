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
using Telerik.Blazor.Components;
using Telerik.Blazor.Resources;

namespace BHSystem.Web.Features.Admin
{
    public partial class UserPage
    {
        [Inject] private ILogger<UserPage>? _logger { get; init; }
        [Inject] private ILoadingCore? _spinner { get; set; }
        [Inject] private IToastService? _toastService { get; set; }
        [Inject] private ICliUserService? _userService { get; set; }
        public List<UserModel>? ListUser { get; set; }
        public IEnumerable<UserModel>? SelectedUsers { get; set; } = new List<UserModel>();
        public bool IsInitialDataLoadComplete { get; set; } = true;
        public UserModel UserUpdate { get; set; } = new UserModel();
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
        /// lấy danh sách User
        /// </summary>
        /// <param name="isLoading"></param>
        /// <returns></returns>
        private async Task getDataUser(bool isLoading = false)
        {
            ListUser = new List<UserModel>();
            SelectedUsers = new List<UserModel>();
            ListUser = await _userService!.GetDataAsync();
        }
        #endregion "Private Functions"

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
                await getDataUser();
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
        protected void OnOpenDialogHandler(EnumType pAction = EnumType.Add, UserModel? pItemDetails = null)
        {
            IsShowDialog = true;
            
            
            try
            {

                if (pAction == EnumType.Add)
                {
                    UserUpdate = new UserModel();
                    IsCreate = true;
                }
                else
                {
                    UserUpdate.UserId = pItemDetails!.UserId;
                    UserUpdate.FullName = pItemDetails!.FullName;
                    UserUpdate.UserName = pItemDetails!.UserName;
                    UserUpdate.Password = EncryptHelper.Decrypt(pItemDetails!.Password+"");
                    UserUpdate.Address = pItemDetails!.Address;
                    UserUpdate.Email = pItemDetails!.Email;
                    IsCreate = false;
                }
                IsShowDialog = true;
                _EditContext = new EditContext(UserUpdate);
            }
            catch (Exception ex)
            {
                _logger!.LogError(ex, "ReceiptController", "OnOpenDialogHandler");
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
                if (UserUpdate.UserId > 0)
                {
                    sAction = nameof(EnumType.Update);
                    sMessage = "Cập nhật";
                }
                var checkData = _EditContext!.Validate();
                if (!checkData) return;
                await showLoading();
                UserUpdate.Ward_Id = 1;
                bool isUpdate = await _userService!.UpdateAsync(JsonConvert.SerializeObject(UserUpdate), sAction);
                if (isUpdate)
                {
                    _toastService!.ShowSuccess($"Đã {sMessage} thông tin người dùng.");
                    await getDataUser();
                    if (pEnum == EnumType.SaveAndCreate)
                    {
                        UserUpdate = new UserModel();
                        _EditContext = new EditContext(UserUpdate);
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

        protected void OnRowDoubleClickHandler(GridRowClickEventArgs args) => OnOpenDialogHandler(EnumType.Update, args.Item as UserModel);

        /// <summary>
        /// conrfirm xóa user
        /// </summary>
        /// <returns></returns>
        protected async Task ConfirmDeleteHandler()
        {
            if (SelectedUsers == null || !SelectedUsers.Any())
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
                    var oDelete = SelectedUsers.Select(m => new { m.UserId});
                    bool isSuccess = await _userService!.DeleteAsync(JsonConvert.SerializeObject(oDelete));
                    if (isSuccess)
                    {
                        _toastService!.ShowSuccess($"Đã xóa thông tin người dùng.");
                        await getDataUser();
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
        #endregion "Protected Functions"
    }
}
