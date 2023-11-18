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
using Microsoft.AspNetCore.WebUtilities;
using Newtonsoft.Json;

namespace BHSystem.Web.Features.Admin
{
    public partial class InfoUserPage
    {
        [Inject] NavigationManager? _navigationManager { get; set; }
        [Inject] private ILogger<InfoUserPage>? _logger { get; init; }
        [Inject] private ILoadingCore? _spinner { get; set; }
        [Inject] private IToastService? _toastService { get; set; }
        [Inject] private ICliUserService? _userService { get; set; }
        [Inject] private IApiService? _apiService { get; set; }

        public UserModel UserUpdate { get; set; } = new UserModel();
        public EditContext? _EditContext { get; set; }

        public BHConfirm? _rDialogs { get; set; }

        private int pUserId { get; set; }

        public List<CityModel> ListCity { get; set; } = new List<CityModel>();
        public List<DistinctModel>? ListDistinct { get; set; } = new List<DistinctModel>();
        public List<WardModel>? ListWard { get; set; } = new List<WardModel>();

        protected override void OnInitialized()
        {
            base.OnInitialized();
            _EditContext = new EditContext(UserUpdate);
            try
            {
                // đọc giá tri câu query
                var uri = _navigationManager?.ToAbsoluteUri(_navigationManager.Uri);
                if (uri != null)
                {
                    var queryStrings = QueryHelpers.ParseQuery(uri.Query);
                    if (queryStrings.Count() > 0)
                    {
                        string key = uri.Query.Substring(5); // để tránh parse lỗi;
                        Dictionary<string, string> pParams = JsonConvert.DeserializeObject<Dictionary<string, string>>(EncryptHelper.Decrypt(key));
                        if (pParams != null && pParams.Any() && pParams.ContainsKey("UserId")) pUserId = Convert.ToInt32(pParams["UserId"]);
                    }
                }    
            }
            catch
            {

            }
            _EditContext = new EditContext(UserUpdate);
        }
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await base.OnAfterRenderAsync(firstRender);
            if (firstRender)
            {
                try
                {
                    await showLoading();
                    await getCity();
                    await getDataUser();
                    Task task1 = getDistrictByCity(UserUpdate.City_Id);
                    Task task2 = getWardByDistrict(UserUpdate.Distinct_Id);
                    await Task.WhenAll(task1, task2);
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

        private async Task getDataUser()
        {
            try
            {
                var data = await _userService!.GetUserById(pUserId);
                if(data != null)
                {
                    UserUpdate.Password = EncryptHelper.Decrypt(data.Password+"");
                    UserUpdate.UserId = data!.UserId;
                    UserUpdate.FullName = data!.FullName;
                    UserUpdate.UserName = data!.UserName;
                    UserUpdate.Password = EncryptHelper.Decrypt(data!.Password + "");
                    UserUpdate.Address = data!.Address;
                    UserUpdate.Phone = data!.Phone;
                    UserUpdate.Email = data!.Email;
                    UserUpdate.Ward_Id = data!.Ward_Id;
                    UserUpdate.City_Id = data!.City_Id;
                    UserUpdate.Distinct_Id = data!.Distinct_Id;
                    UserUpdate.IsAdmin = data!.IsAdmin;

                }    
            }
            catch (Exception ex)
            {
                _logger!.LogError(ex, "OnLoadDistrictByCity");
                _toastService!.ShowError(ex.Message);
            }
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
        #endregion

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

        protected async void SaveDataHandler()
        {
            try
            {
                if(pUserId <= 0)
                {
                    _toastService!.ShowInfo($"Không tìm thấy thông tin người dùng.");
                    return;
                }
                _EditContext = new EditContext(UserUpdate);
                var confirm = await _rDialogs!.ConfirmAsync(" Bạn có chắc muốn cập nhật thông tin? ");
                if (confirm)
                {
                    var checkData = _EditContext!.Validate();
                    if (!checkData) return;
                    await showLoading();
                    string sKind = UserUpdate.IsAdmin ? "Admin" : "Client";
                    bool isUpdate = await _userService!.UpdateAsync(JsonConvert.SerializeObject(UserUpdate), nameof(EnumType.Update), pUserId, sKind);
                    if (isUpdate)
                    {
                        _toastService!.ShowSuccess($"Đã cập nhật thông tin người dùng.");
                        await getDataUser();
                        _EditContext = new EditContext(UserUpdate);
                    }
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
    }
}
