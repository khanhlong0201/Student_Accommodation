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
using Telerik.Blazor.Components;

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

        List<ImagesDetailModel> ListImageDetail = new List<ImagesDetailModel>();
        

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
        public bool IsUpate { get; set; } = false;
        public EditContext? _EditContext { get; set; }

        public BHConfirm? _rDialogs { get; set; }
        [CascadingParameter(Name = "pUserId")]
        private int pUserId { get; set; } // giá trị từ MainLayout

        [CascadingParameter(Name = "pIsSupperAdmin")]
        private bool pIsSupperAdmin { get; set; } // giá trị từ MainLayout

        #region "Private Functions"
        private async Task getImageDeteailByImageId(int imageId)
        {
            ListImageDetail = new List<ImagesDetailModel>();
            Dictionary<string, object> pParams = new Dictionary<string, object>()
            {
                {"imageId", $"{imageId}"}
            };
            var resString = await _service!.GetData(EndpointConstants.URL_IMAGE_DETAIL_GET_BY_IMAGE_ID, pParams);
            ListImageDetail = JsonConvert.DeserializeObject<List<ImagesDetailModel>>(resString);
        }

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

        }

        private async Task onLoadDistrictByCity(bool chooseFirstRow = false)
        {
            try
            {
                if (chooseFirstRow)
                {
                    await showLoading();
                    var request = new Dictionary<string, object>
                    {
                        { "city_id", SelectCity }
                    };
                    var resStringDistinct = await _service!.GetData(EndpointConstants.URL_DISTINCT_GET_BY_CITY, request);
                    ListDistinct = JsonConvert.DeserializeObject<List<DistinctModel>>(resStringDistinct);
                }
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

        private async Task onLoadWardByDistrict(bool chooseFirstRow = false)
        {
            try
            {
                if (chooseFirstRow)
                {
                    await showLoading();
                    var request = new Dictionary<string, object>
                    {
                        { "distinct_id", selectDistinct }
                    };
                    var resStringWard = await _service!.GetData(EndpointConstants.URL_WARD_GET_BY_DISTINCT, request);
                    ListWard = JsonConvert.DeserializeObject<List<WardModel>>(resStringWard);
                }
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

        private async Task showLoading(bool isShow = true)
        {
            if (isShow) { _spinner!.Show(); await Task.Yield(); }
            else _spinner!.Hide();
        }

        #endregion "Private Functions"

        #region "Protected Functions"
        /// <summary>
        ///longtran 20230301 EventCallback Chọn dòng trên lưới đối với thong so giam sat tinh trang
        /// </summary>
        /// <param name="item"></param>
        public void ListImageDetailChanged(List<ImagesDetailModel> list)
        {
            try
            {
                if (list != null) ListImageDetail = list;
            }
            catch (Exception)
            {
                throw;
            }
        }



        //protected async Task OnRowDoubleClickHandler(GridRowClickEventArgs args) => await OnOpenDialogHandler(EnumType.Update,args.Item as BoardingHouseModel);
        protected async Task OnRowDoubleClickHandler(GridRowClickEventArgs args)
        {
            var boardingHouseModel = args.Item as BoardingHouseModel;
            await OnOpenDialogHandler(EnumType.Update, boardingHouseModel);
        }

        /// <summary>
        /// mở popup
        /// </summary>
        protected async Task OnOpenDialogHandler(EnumType pAction = EnumType.Add,BoardingHouseModel ? pItemDetails = null)
        {
            try
            {
                if (pAction == EnumType.Add)
                {
                    BoardingHouseUpdate = new BoardingHouseModel();
                    _EditContext = new EditContext(BoardingHouseUpdate);
                    IsUpate = false;
                    selectCity = -1;
                    selectDistinct = -1;
                    SelectWard = -1;
                    ListImageDetail = new List<ImagesDetailModel>();
                }
                else
                {
                    SelectCity = pItemDetails.City_Id;
                    SelectDistinct = pItemDetails.Distinct_Id;
                    SelectWard = pItemDetails.Ward_Id;
                    SelectCity = pItemDetails.City_Id;
                    BoardingHouseUpdate.Id = pItemDetails!.Id;
                    BoardingHouseUpdate.Name = pItemDetails!.Name;
                    BoardingHouseUpdate.Qty = pItemDetails!.Qty;
                    BoardingHouseUpdate.Ward_Id = pItemDetails!.Ward_Id;
                    BoardingHouseUpdate.Adddress = pItemDetails!.Adddress;
                    BoardingHouseUpdate.Image_Id = pItemDetails!.Image_Id;
                    _EditContext = new EditContext(BoardingHouseUpdate);
                    IsUpate = true;
                    await  getImageDeteailByImageId(pItemDetails.Image_Id);
                    

                }
                
                IsShowDialog = true;
  
            }
            catch (Exception ex)
            {
                _logger!.LogError(ex, "ReceiptController", "OnOpenDialogHandler");
                _toastService!.ShowError(ex.Message);
            }
        }


        /// <summary>
        /// load dữ liệu
        /// </summary>
        /// <returns></returns>
        protected async Task ReLoadDataHandler()
        {
            try
            {
                await showLoading();
                await getDataBoardingHouse();
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
        protected void OnOpenDialogHandler(EnumType update)
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
                if (!IsUpate) // nếu là thêm
                {
                    await showLoading();
                    BoardingHouseUpdate.Ward_Id = SelectWard;
                    BoardingHouseUpdate.User_Id = pUserId;
                    RequestModel request = new RequestModel()
                    {
                        Json = JsonConvert.SerializeObject(BoardingHouseUpdate),
                        Json_Detail = JsonConvert.SerializeObject(ListImageDetail)
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
                else{ // nếu là sửa
                    await showLoading();
                    BoardingHouseUpdate.Ward_Id = SelectWard;
                    RequestModel request = new RequestModel()
                    {
                        Json = JsonConvert.SerializeObject(BoardingHouseUpdate),
                        Json_Detail = JsonConvert.SerializeObject(ListImageDetail)
                    };
                    var resString = await _service!.AddOrUpdateData(EndpointConstants.URL_BOARDINGHOUSE_UPDATE, request);
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
            }
            catch (Exception ex)
            {
                _logger!.LogError(ex, "SaveDataHandler");
                _toastService!.ShowError(ex.Message);
                IsShowDialog = false;
                await showLoading(false);
                IsUpate = false;
            }
            finally
            {
                await showLoading(false);
                await InvokeAsync(StateHasChanged);
            }
        }
        /// <summary>
        /// conrfirm xóa 
        /// </summary>
        /// <returns></returns>
        protected async Task ConfirmDeleteHandler()
        {
            if (SelectedBoardingHouse == null || !SelectedBoardingHouse.Any())
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
                    var oDelete = SelectedBoardingHouse.Select(m => new { m.Id });
                    string json = JsonConvert.SerializeObject(oDelete);
                    RequestModel request = new RequestModel()
                    {
                        Json = json
                    };
                    var resString = await _service.AddOrUpdateData(EndpointConstants.URL_BOARDINGHOUSE_DELETE, request);
                    if (!string.IsNullOrEmpty(resString))
                    {
                        _toastService!.ShowSuccess($"Đã xóa thông tin trọ.");
                        await getDataBoardingHouse();
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
