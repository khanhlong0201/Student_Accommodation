using BHSystem.Web.Constants;
using BHSystem.Web.Core;
using BHSystem.Web.Extensions;
using BHSystem.Web.Features.Admin;
using BHSytem.Models.Models;
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
        #region Override Functions

        protected override void OnInitialized()
        {
            SearchModel.Limit = 4;
            SearchModel.Page = 0;
            base.OnInitialized();
        }
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if(firstRender)
            {
                try
                {
                    await showLoading();
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
                SearchModel.Limit = 2;
                SearchModel.Page = 0;
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
                if (pageIndex < 0 || pageIndex > Pagination.TotalPage) return;
                await showLoading();
                SearchModel.Limit = 2;
                SearchModel.Page = pageIndex - 1;
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
        #endregion

    }
}
