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
using System.Threading.Tasks;
using Telerik.Blazor.Components;
using Telerik.Blazor.Resources;

namespace BHSystem.Web.Features.Admin
{
    public partial class UserPage
    {
        [Inject] NavigationManager? _navigationManager { get; set; }
        [Inject] private ILogger<UserPage>? _logger { get; init; }
        [Inject] private ILoadingCore? _spinner { get; set; }
        [Inject] private IToastService? _toastService { get; set; }
        [Inject] private ICliUserService? _userService { get; set; }
        [Inject] private IApiService? _apiService { get; set; }
        public List<UserModel>? ListUser { get; set; }
        public IEnumerable<UserModel>? SelectedUsers { get; set; } = new List<UserModel>();
        public bool IsInitialDataLoadComplete { get; set; } = true;
        public UserModel UserUpdate { get; set; } = new UserModel();
        public bool IsCreate { get; set; } = true;
        public bool IsShowDialog { get; set; }
        public EditContext? _EditContext { get; set; }
        public BHConfirm? _rDialogs { get; set; }

        [CascadingParameter(Name = "pUserId")]
        private int pUserId { get; set; } // giá trị từ MainLayout

        [CascadingParameter(Name = "pIsSupperAdmin")]
        private bool pIsSupperAdmin { get; set; } // giá trị từ MainLayout

        public List<CityModel> ListCity { get; set; } = new List<CityModel>();
        public List<DistinctModel>? ListDistinct { get; set; } = new List<DistinctModel>();
        public List<WardModel>? ListWard { get; set; } = new List<WardModel>();

        [CascadingParameter(Name = "pListMenus")]
        private List<MenuModel>? ListMenus { get; set; } // giá trị từ MainLayout
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await base.OnAfterRenderAsync(firstRender);
            if (firstRender)
            {
                try
                {
                    var findMenu = ListMenus?.FirstOrDefault(m => m.Link?.ToUpper() == "/admin/user".ToUpper()); // tìm lấy menu không
                    if (findMenu == null)
                    {
                        _toastService!.ShowInfo("Bạn không có quyền vào menu này!!!");
                        await Task.Delay(4500);
                        _navigationManager!.NavigateTo("/admin/logout");
                        return;
                    }
                    await showLoading();
                    await getCity();
                    await getDataUser();
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

        private async Task getCity()
        {
            var resStringCity = await _apiService!.GetData(EndpointConstants.URL_CITY_GETALL);
            ListCity = JsonConvert.DeserializeObject<List<CityModel>>(resStringCity);
        }

        private async Task getDistrictByCity(int iCityId)
        {
            try
            {
                await showLoading();
                ListDistinct = new List<DistinctModel>();
                var request = new Dictionary<string, object>
                    {
                        { "city_id", iCityId }
                    };
                var resStringDistinct = await _apiService!.GetData(EndpointConstants.URL_DISTINCT_GET_BY_CITY, request);
                ListDistinct = JsonConvert.DeserializeObject<List<DistinctModel>>(resStringDistinct);
            }
            catch (Exception ex)
            {
                _logger!.LogError(ex, "OnLoadDistrictByCity");
                _toastService!.ShowError(ex.Message);
            }
            finally
            {
                await showLoading(false);
                await InvokeAsync(StateHasChanged);
            }

        }

        private async Task getWardByDistrict(int iDistinctId)
        {
            try
            {
                await showLoading();
                ListWard = new List<WardModel>();
                var request = new Dictionary<string, object>
                    {
                        { "distinct_id", iDistinctId }
                    };
                var resStringWard = await _apiService!.GetData(EndpointConstants.URL_WARD_GET_BY_DISTINCT, request);
                ListWard = JsonConvert.DeserializeObject<List<WardModel>>(resStringWard);
            }
            catch (Exception ex)
            {
                _logger!.LogError(ex, "onLoadWardByDistrict");
                _toastService!.ShowError(ex.Message);
            }
            finally
            {
                await showLoading(false);
                await InvokeAsync(StateHasChanged);
            }
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
        protected async void OnOpenDialogHandler(EnumType pAction = EnumType.Add, UserModel? pItemDetails = null)
        {
            try
            {

                if (pAction == EnumType.Add)
                {
                    UserUpdate = new UserModel();
                    IsCreate = true;
                    _EditContext = new EditContext(UserUpdate);
                }
                else
                {
                    UserUpdate.UserId = pItemDetails!.UserId;
                    UserUpdate.FullName = pItemDetails!.FullName;
                    UserUpdate.UserName = pItemDetails!.UserName;
                    UserUpdate.Password = EncryptHelper.Decrypt(pItemDetails!.Password+"");
                    UserUpdate.Address = pItemDetails!.Address;
                    UserUpdate.Phone = pItemDetails!.Phone;
                    UserUpdate.Email = pItemDetails!.Email;
                    UserUpdate.Ward_Id = pItemDetails!.Ward_Id;
                    UserUpdate.City_Id = pItemDetails!.City_Id;
                    UserUpdate.Distinct_Id = pItemDetails!.Distinct_Id;
                    UserUpdate.IsAdmin = pItemDetails!.IsAdmin;
                    IsCreate = false;
                    _EditContext = new EditContext(UserUpdate);
                    await showLoading();
                    Task task1 = getDistrictByCity(UserUpdate.City_Id);
                    Task task2 = getWardByDistrict(UserUpdate.Distinct_Id);
                    await Task.WhenAll(task1, task2);
                }
                IsShowDialog = true;
            }
            catch (Exception ex)
            {
                _logger!.LogError(ex, "ReceiptController", "OnOpenDialogHandler");
                _toastService!.ShowError(ex.Message);
            }
            finally
            {
                await showLoading(false);
                await InvokeAsync(StateHasChanged);
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
                if(!pIsSupperAdmin)
                {
                    _toastService!.ShowInfo($"Chỉ có tài khoản Admin mới được phép cập nhật thông tin người dùng.");
                    return;
                }    
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
                //UserUpdate.Ward_Id = 1;
                string sKind = UserUpdate.IsAdmin ? "Admin" : "Client";
                bool isUpdate = await _userService!.UpdateAsync(JsonConvert.SerializeObject(UserUpdate), sAction, pUserId, sKind);
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
                    bool isSuccess = await _userService!.DeleteAsync(JsonConvert.SerializeObject(oDelete), pUserId);
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

        protected async void OnChangeCityHandler(int iCityId)
        {
            UserUpdate.City_Id = iCityId;
            await getDistrictByCity(iCityId);
        }

        protected async void OnChangeDistinctHandler(int iDistinctId)
        {
            UserUpdate.Distinct_Id = iDistinctId;
            await getWardByDistrict(iDistinctId);
        }
        #endregion "Protected Functions"
    }
}
