using BHSystem.Web.Core;
using BHSystem.Web.Services;
using BHSystem.Web.ViewModels;
using BHSytem.Models.Models;
using Blazored.Toast.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Newtonsoft.Json;

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

        public bool IsShowDialog { get; set; }
        public EditContext? _EditContext { get; set; }

        protected override Task OnInitializedAsync()
        {
            ListUser = new List<UserModel>();
            ListUser.Add(new UserModel() { UserId = 1, FullName = "Nguyễn Tấn Hải" });
            ListUser.Add(new UserModel() { UserId = 2, FullName = "Nguyễn Tấn Hải" });
            ListUser.Add(new UserModel() { UserId = 3, FullName = "Nguyễn Tấn Hải" });
            ListUser.Add(new UserModel() { UserId = 4, FullName = "Nguyễn Tấn Hải" });
            ListUser.Add(new UserModel() { UserId = 5, FullName = "Nguyễn Tấn Hải" });
            ListUser.Add(new UserModel() { UserId = 6, FullName = "Nguyễn Tấn Hải" });
            return base.OnInitializedAsync();
        }

        #region "Private Functions"
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
                _spinner!.Show();
                await getDataUser();
            }
            catch (Exception ex)
            {
                _logger!.LogError(ex, "ReLoadDataHandler");
                _toastService!.ShowError(ex.Message);
            }
            finally
            {
                _spinner!.Hide();
                await InvokeAsync(StateHasChanged);
            }
        }


        /// <summary>
        /// mở popup
        /// </summary>
        protected void OnOpenDialogHandler()
        {
            IsShowDialog = true;
            _EditContext = new EditContext(UserUpdate);
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
                _spinner!.Show();
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
                _spinner!.Hide();
                await InvokeAsync(StateHasChanged);
            }
        }

        #endregion "Protected Functions"
    }
}
