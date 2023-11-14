using BHSystem.Web.Constants;
using BHSystem.Web.Core;
using BHSystem.Web.Extensions;
using BHSystem.Web.Features.Admin;
using BHSytem.Models.Models;
using Blazored.LocalStorage;
using Blazored.Toast.Services;
using Microsoft.AspNetCore.Components;
using Newtonsoft.Json;

namespace BHSystem.Web.Features.Client
{
    public partial class IndexClient
    {
        [Inject] private ILogger<IndexClient>? _logger { get; init; }
        [Inject] private ILoadingCore? _spinner { get; set; }
        [Inject] private IToastService? _toastService { get; set; }
        [Inject] private IApiService? _apiService { get; set; }
        [Inject] NavigationManager? _navigationManager { get; set; }
        [Inject] IConfiguration? _configuration { get; set; }
        [Inject] ILocalStorageService? _localStorage { get; set; }

        #region Properties Test
        public string binding { get; set; } = "";
        public List<string> ListData = new List<string>() { "Quận 1", "Quận 2", "Quận 3", "Quận 4", "Quận 5" };
        public int Page { get; set; } = 3;
        public int PageSize { get; set; } = 4;
        public int TotalCount { get; set; } = 50;
        #endregion

        public BHouseSearchModel SearchModel { get; set; } = new BHouseSearchModel();
        public List<CliBoardingHouseModel> ListDataBHouses = new List<CliBoardingHouseModel>();
        public PaginationModel Pagination { get; set; } = new PaginationModel();
        public List<CityModel> ListCity { get; set; } = new List<CityModel>();
        public List<DistinctModel>? ListDistinct { get; set; } = new List<DistinctModel>();
        public List<WardModel>? ListWard { get; set; } = new List<WardModel>();

        public Dictionary<string, string> ListPrices = new Dictionary<string, string>();
        public Dictionary<string, string> ListAcreages = new Dictionary<string, string>();
        #region Override Functions

        protected override async Task OnInitializedAsync()
        {
            try
            {
                await showLoading();
                SearchModel.Limit = 5;
                SearchModel.Page = 1;
                ListPrices.Add("1", "Dưới 1 triệu");
                ListPrices.Add("2", "Từ 1 - 3 triệu");
                ListPrices.Add("3", "Từ 3 - 5 triệu");
                ListPrices.Add("4", "Từ 5 - 7 triệu");
                ListPrices.Add("5", "Từ 7 - 10 triệu");
                ListPrices.Add("6", "Từ 10 - 15 triệu");
                ListPrices.Add("7", "Trên 15 triệu");

                ListAcreages.Add("1", "Dưới 20 m²");
                ListAcreages.Add("2", "20 -30 m²");
                ListAcreages.Add("3", "30 -40 m²");
                ListAcreages.Add("4", "40 -50 m²");
                ListAcreages.Add("5", "Trên 50 m²");
            }
            catch(Exception)
            {

            }
        }
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if(firstRender)
            {
                try
                {
                    if(await _localStorage!.ContainKeyAsync("oFilter"))
                    {
                        SearchModel = await _localStorage!.GetItemAsync<BHouseSearchModel>("oFilter");
                        Task task1 = getDistrictByCity(SearchModel.CityId);
                        Task task2 = getWardByDistrict(SearchModel.DistinctId);
                        await Task.WhenAll(task1, task2);
                        await _localStorage!.RemoveItemAsync("oFilter");
                    }    
                    await getDataBHouse();
                    await getCity();
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
        #endregion

        #region "Private Functions"
        private async Task showLoading(bool isShow = true)
        {
            if (isShow) { _spinner!.Show(); await Task.Yield(); }
            else _spinner!.Hide();
        }

        /// <summary>
        /// lấy danh sách các phòng trọ
        /// </summary>
        /// <returns></returns>
        private async Task getDataBHouse()
        {
            string resString = await _apiService!.GetDataFromBody(EndpointConstants.URL_CLI_BHOUSE_GETDATA, SearchModel);
            if (!string.IsNullOrEmpty(resString))
            {
                string urlRoom = _configuration!.GetSection("appSettings:ApiUrl").Value + DefaultConstants.FOLDER_ROOM + "/";
                string urlHouse = _configuration!.GetSection("appSettings:ApiUrl").Value + DefaultConstants.FOLDER_BHOUSE + "/";
                var result = JsonConvert.DeserializeObject<CliResponseModel<CliBoardingHouseModel>>(resString);
                ListDataBHouses = result.ListData.Update(m=>
                {
                    if (string.IsNullOrWhiteSpace(m.ImageUrlBHouse)) m.ImageUrlBHouse = "./images/img-default.png";
                    else m.ImageUrlBHouse = urlHouse + m.ImageUrlBHouse;
                    List<string> lstData = new List<string>();
                    if (m.ListImages != null && m.ListImages.Any())
                    {
                        m.ListImages.ForEach(item =>
                        {
                            item = urlRoom + item;
                            lstData.Add(item);
                        });
                    }
                    while (lstData.Count < 4)
                    {
                        lstData.Add("./images/img-default.png");
                    }; // thêm cho đủ 4 phần tử
                    m.ListImages = lstData;
                }).ToList();
                Pagination = result.Pagination;
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
                await showLoading();
                SearchModel.Limit = 5;
                SearchModel.Page = 1;
                await getDataBHouse();
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

        protected async void OnChangePageIndex(int pageIndex)
        {
            try
            {
                await showLoading();
                SearchModel.Limit = 5;
                SearchModel.Page = pageIndex;
                await getDataBHouse();
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

        protected async void OnChangePrice(string key, bool isPrice = true)
        {
            try
            {
                await showLoading();
                SearchModel.Limit = 5;
                SearchModel.Page = 1;
                if (isPrice) SearchModel.KeyPrice = SearchModel.KeyPrice == key ? "" : key; 
                else SearchModel.KeyAcreage = SearchModel.KeyAcreage == key ? "" : key;
                await getDataBHouse();
            }
            catch (Exception ex)
            {
                _logger!.LogError(ex, "OnChangePrice");
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
