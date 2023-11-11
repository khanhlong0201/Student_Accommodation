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

        #region Override Functions

        protected override void OnInitialized()
        {
            SearchModel.Limit = 10;
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

        /// <summary>
        /// lấy danh sách các phòng trọ
        /// </summary>
        /// <returns></returns>
        private async Task getDataBHouse()
        {
            string resString = await _apiService!.GetDataFromBody(EndpointConstants.URL_CLI_BHOUSE_GETDATA, SearchModel);
            if (!string.IsNullOrEmpty(resString))
            {
                
                var result = JsonConvert.DeserializeObject<CliResponseModel<CliBoardingHouseModel>>(resString);
                ListDataBHouses = result.ListData;
                Pagination = result.Pagination;
            }    
        }
        #endregion

        #region "Protected Functions"
        #endregion

    }
}
