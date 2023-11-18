using BHSystem.Web.Constants;
using BHSystem.Web.Core;
using BHSystem.Web.Features.Admin;
using BHSystem.Web.Providers;
using BHSystem.Web.ViewModels;
using BHSytem.Models.Models;
using Blazored.LocalStorage;
using Blazored.Toast.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.WebUtilities;
using Newtonsoft.Json;

namespace BHSystem.Web.Features.Client
{
    public partial class BHouseDetail
    {
        [Inject] private ILogger<BHouseDetail>? _logger { get; init; }
        [Inject] private ILoadingCore? _spinner { get; set; }
        [Inject] private IToastService? _toastService { get; set; }
        [Inject] private IApiService? _apiService { get; set; }
        [Inject] NavigationManager? _navigationManager { get; set; }
        [Inject] IConfiguration? _configuration { get; set; }
        [Inject] ILocalStorageService? _localStorage { get; set; }

        public int PageIndex = 1;
        public List<CarouselModel> CarouselData { get; set; } = new List<CarouselModel>()
        {
            new CarouselModel() { Url = @"https://lighthouse.chotot.com/_next/image?url=https%3A%2F%2Fcdn.chotot.com%2Fadmincentre%2FICGqIPhBAn559vSI4v7jaBAYFYegeRG7xSfUJ6tkugI%2Fpreset%3Araw%2Fplain%2F6ec3994f81e14d768dfc467847ce430c-2820195948173896828.jpg&w=1080&q=90"},
            new CarouselModel() { Url = @"https://lighthouse.chotot.com/_next/image?url=https%3A%2F%2Fcdn.chotot.com%2Fadmincentre%2FICGqIPhBAn559vSI4v7jaBAYFYegeRG7xSfUJ6tkugI%2Fpreset%3Araw%2Fplain%2F6ec3994f81e14d768dfc467847ce430c-2820195948173896828.jpg&w=1080&q=90"},
            new CarouselModel() { Url = @"https://lighthouse.chotot.com/_next/image?url=https%3A%2F%2Fcdn.chotot.com%2Fadmincentre%2FICGqIPhBAn559vSI4v7jaBAYFYegeRG7xSfUJ6tkugI%2Fpreset%3Araw%2Fplain%2F6ec3994f81e14d768dfc467847ce430c-2820195948173896828.jpg&w=1080&q=90"},
            new CarouselModel() { Url = @"https://lighthouse.chotot.com/_next/image?url=https%3A%2F%2Fcdn.chotot.com%2Fadmincentre%2FICGqIPhBAn559vSI4v7jaBAYFYegeRG7xSfUJ6tkugI%2Fpreset%3Araw%2Fplain%2F6ec3994f81e14d768dfc467847ce430c-2820195948173896828.jpg&w=1080&q=90"},
        };

        public string binding { get; set; } = "";
        public List<string> ListData = new List<string>() { "Quận 1", "Quận 2", "Quận 3", "Quận 4", "Quận 5" };

        public bool IsShowDialog { get; set; }
        public BookingModel BookingUpdate { get; set; } = new BookingModel();
        public EditContext? _EditContext { get; set; }


        [CascadingParameter(Name = "pUserId")]
        private int pUserId { get; set; } // giá trị từ ClientLayout

        [CascadingParameter(Name = "pIsSupperAdmin")]
        private bool pIsSupperAdmin { get; set; } // giá trị từ MainLayout

        #region Properties
        public CliBoardingHouseModel CliBoardingHouse { get; set; } = new CliBoardingHouseModel();
        public List<string> ListImages { get; set; } = new List<string>();
        public int pRoomId { get; set; }
        public List<CityModel> ListCity { get; set; } = new List<CityModel>();
        public List<DistinctModel>? ListDistinct { get; set; } = new List<DistinctModel>();
        public List<WardModel>? ListWard { get; set; } = new List<WardModel>();
        public BHouseSearchModel SearchModel { get; set; } = new BHouseSearchModel();
        #endregion

        #region Override Functions

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
                        string key = uri.Query.Substring(5); // để tránh parse lỗi;
                        pRoomId = Convert.ToInt32(key);
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
                    await getCity();
                    await getDataDetail();
                }
                catch (Exception ex)
                {
                    _logger!.LogError(ex, "OnAfterRenderAsync");
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

        private async Task getDataDetail()
        {
            try
            {
                Dictionary<string, object> pKeys = new Dictionary<string, object>()
                {
                    {"pRoomId", $"{pRoomId}" }
                };
                string resString = await _apiService!.GetData(EndpointConstants.URL_CLI_BHOUSE_GETDATA_DETAIL, pKeys);
                if (!string.IsNullOrEmpty(resString))
                {
                    string urlRoom = _configuration!.GetSection("appSettings:ApiUrl").Value + DefaultConstants.FOLDER_ROOM + "/";
                    CliBoardingHouse = JsonConvert.DeserializeObject<CliBoardingHouseModel>(resString);
                    ListImages = new List<string>();
                    if (CliBoardingHouse?.ListImages != null && CliBoardingHouse.ListImages.Any())
                    {
                        CliBoardingHouse.ListImages.ForEach(item =>
                        {
                            item = urlRoom + item;
                            ListImages.Add(item);
                        });
                    }

                    if(!ListImages.Any())
                    {

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

        #region "Protected Functions"
        protected void OpenPopupBookingHandler()
        {
            try
            {
                BookingUpdate = new BookingModel();
                _EditContext = new EditContext(BookingUpdate);
                IsShowDialog = true;
            }
            catch(Exception ex)
            {
                _logger!.LogError(ex, "OnOpenDialogHandler");
                _toastService!.ShowError(ex.Message);
            }
        }

        /// <summary>
        /// lưu dữ liệu booking
        /// </summary>
        protected async void SaveDataHandler()
        {
            try
            {
                var checkData = _EditContext!.Validate();
                if (!checkData) return;
                await showLoading();
                BookingUpdate.Room_Id = pRoomId;
                BookingUpdate.UserId = 1;
                BookingUpdate.Status = "Chờ xử lý";
                RequestModel request = new RequestModel()
                {
                    Json = JsonConvert.SerializeObject(BookingUpdate),
                    Type = nameof(EnumType.Add),
                    UserId = pUserId > 0 ? pUserId : 1
                };
                string resString = await _apiService!.AddOrUpdateData(EndpointConstants.URL_BOOKING_UPDATE, request);
                if (!string.IsNullOrEmpty(resString))
                {
                    _toastService!.ShowSuccess($"Đã lưu thông tin. Chủ phòng sẽ liên hệ lại sau!!!");
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

        protected async void OnChangeCityHandler(int iCityId)
        {
            SearchModel.CityId = iCityId;
            SearchModel.DistinctId = 0;
            await getDistrictByCity(iCityId);
        }

        protected async void OnChangeDistinctHandler(int iDistinctId)
        {
            SearchModel.DistinctId = iDistinctId;
            SearchModel.WardId = 0;
            await getWardByDistrict(iDistinctId);
        }

        protected async void ReLoadDataHandler()
        {
            try
            {
                SearchModel.Limit = 5;
                SearchModel.Page = 0;
                if (await _localStorage!.ContainKeyAsync("oFilter")) await _localStorage!.RemoveItemAsync("oFilter");
                await _localStorage!.SetItemAsync("oFilter", SearchModel);
                _navigationManager!.NavigateTo("/trang-chu");
                //await getDataBHouse();
            }
            catch (Exception ex)
            {
                _logger!.LogError(ex, "ReLoadDataHandler");
                _toastService!.ShowError(ex.Message);
            }
            finally
            {
                await InvokeAsync(StateHasChanged);
            }
        }
        #endregion
    }
}
