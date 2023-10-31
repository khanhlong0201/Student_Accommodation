using BHSystem.Web.Constants;
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
    public partial class BoardingHousePage
    {
        [Inject] private ILogger<BoardingHousePage>? _logger { get; init; }
        [Inject] private ILoadingCore? _spinner { get; set; }
        [Inject] private IToastService? _toastService { get; set; }
        [Inject] private IApiService? _service { get; set; }
        public List<BoardingHouseModel>? ListBoardingHouse { get; set; }
        public List<CityModel> ListCity { get; set; } = new List<CityModel>();
        public List<DistinctModel>? ListDistinct { get; set; } = new List<DistinctModel>();
        public List<WardModel>? ListWard { get; set; } = new List<WardModel>();
        public IEnumerable<BoardingHouseModel>? SelectedBoardingHouse { get; set; } = new List<BoardingHouseModel>();
        public bool IsInitialDataLoadComplete { get; set; } = true;
        public BoardingHouseModel BoardingHouseUpdate { get; set; } = new BoardingHouseModel();
        
        private int selectCity;
        public int SelectCity 
        {
            get { return selectCity; }
            set 
            {
                if (value != selectCity)
                    selectCity = value;
                _ = onLoadDistrictByCity(true);
            }
        }
        private int selectDistinct;
        public int SelectDistinct
        {
            get { return selectDistinct; }
            set
            {
                if (value != selectDistinct)
                    selectDistinct = value;
                _ = onLoadWardByDistrict(true);
            }
        }

        public int SelectWard { get; set; }
        public bool IsShowDialog { get; set; }
        public EditContext? _EditContext { get; set; }

        #region "Private Functions"
        private async Task getDataBoardingHouse(bool isLoading = false)
        {
            ListBoardingHouse = new List<BoardingHouseModel>();
            SelectedBoardingHouse = new List<BoardingHouseModel>();
            var resString = await _service!.GetData(EndpointConstants.URL_BOARDINGHOUSE_GETALL);
            ListBoardingHouse = JsonConvert.DeserializeObject<List<BoardingHouseModel>>(resString);
        }

        private async Task getCombo()
        {
            var resStringCity = await _service!.GetData(EndpointConstants.URL_CITY_GETALL);
            ListCity = JsonConvert.DeserializeObject<List<CityModel>>(resStringCity);

            var resStringDistinct = await _service!.GetData(EndpointConstants.URL_DISTINCT_GETALL);
            ListDistinct = JsonConvert.DeserializeObject<List<DistinctModel>>(resStringDistinct);

            var resStringWard = await _service!.GetData(EndpointConstants.URL_WARD_GETALL);
            ListWard = JsonConvert.DeserializeObject<List<WardModel>>(resStringWard);
        }

        private async Task onLoadDistrictByCity(bool chooseFirstRow = false)
        {
            try
            {
                if (chooseFirstRow)
                {
                    _spinner!.Show();
                    ListDistinct = ListDistinct.Where(d => d.City_Id == SelectCity).ToList();
                }
            }
            catch (Exception ex)
            {
                _logger!.LogError(ex, "OnLoadDistrictByCity");
                _toastService!.ShowError(ex.Message);
            }
            finally
            {
                _spinner!.Hide();
                await InvokeAsync(StateHasChanged);
            }
        }

        private async Task onLoadWardByDistrict(bool chooseFirstRow = false)
        {
            try
            {
                if (chooseFirstRow)
                {
                    _spinner!.Show();
                    ListWard = ListWard.Where(d => d.Distincts_Id == SelectDistinct).ToList();
                }
            }
            catch (Exception ex)
            {
                _logger!.LogError(ex, "onLoadWardByDistrict");
                _toastService!.ShowError(ex.Message);
            }
            finally
            {
                _spinner!.Hide();
                await InvokeAsync(StateHasChanged);
            }
        }
        #endregion "Private Functions"

        #region "Protected Functions"



        /// <summary>
        /// load dữ liệu
        /// </summary>
        /// <returns></returns>
        protected async Task ReLoadDataHandler()
        {
            try
            {
                _spinner!.Show();
                await getDataBoardingHouse();
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
            _EditContext = new EditContext(BoardingHouseUpdate);
        }

        /// <summary>
        /// Thêm/Cập nhật thông tin trọ
        /// </summary>
        /// <param name="pEnum"></param>
        /// <returns></returns>
        protected async void SaveDataHandler(EnumType pEnum = EnumType.SaveAndClose)
        {
            try
            {
                string sMessage = "Thêm";
                string sAction = nameof(EnumType.Add);
                if (BoardingHouseUpdate.Id > 0)
                {
                    sAction = nameof(EnumType.Update);
                    sMessage = "Cập nhật";
                }
                var checkData = _EditContext!.Validate();
                if (!checkData) return;
                _spinner!.Show();

                BoardingHouseUpdate.Adddress = "abc";
                BoardingHouseUpdate.Ward_Id = SelectWard;
                BoardingHouseUpdate.User_Id = 4;
                BoardingHouseUpdate.Image_Id = 1;
                RequestModel request = new RequestModel()
                {
                    Json = JsonConvert.SerializeObject(BoardingHouseUpdate)
                };
                var resString = await _service!.AddOrUpdateData(EndpointConstants.URL_BOARDINGHOUSE_CREATE, request);
                var response = JsonConvert.DeserializeObject<ResponseModel<BoardingHouseModel>>(resString);
                if (response != null && response.StatusCode == 200)
                {
                    _toastService!.ShowSuccess($"Đã {sMessage} thông tin trọ.");
                    await getDataBoardingHouse();
                    IsShowDialog = false;
                    return;
                }
                _toastService?.ShowError(response?.Message);
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
        #region "Form Events"
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            try
            {
                if (firstRender)
                {
                    await ReLoadDataHandler();
                    await getCombo();
                }
            }
            catch (Exception ex)
            {
                _logger!.LogError(ex, "OnAfterRenderAsync");
            }
        }

        #endregion "Form Events"

    }
}
