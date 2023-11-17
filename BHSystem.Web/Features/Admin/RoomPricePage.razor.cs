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
    public partial class RoomPricePage
    {
        [Inject] private ILogger<RoomPricePage>? _logger { get; init; }
        [Inject] private ILoadingCore? _spinner { get; set; }
        [Inject] private IToastService? _toastService { get; set; }
        [Inject] private IApiService? _service { get; set; }
        public List<RoomPriceModel>? ListRoomPrice { get; set; }

        public List<BoardingHouseModel> ListBoardingHouse { get; set; } = new List<BoardingHouseModel>();
        public List<RoomModel> ListRoom { get; set; } = new List<RoomModel>();
        public IEnumerable<RoomPriceModel>? SelectedRoomPrice { get; set; } = new List<RoomPriceModel>();
        public RoomPriceModel RoomPriceUpdate { get; set; } = new RoomPriceModel();
        
        public bool IsShowDialog { get; set; }
        public bool IsUpate { get; set; } = false;
        public EditContext? _EditContext { get; set; }

        public BHConfirm? _rDialogs { get; set; }
        [CascadingParameter(Name = "pUserId")]
        private int pUserId { get; set; } // giá trị từ MainLayout

        [CascadingParameter(Name = "pIsSupperAdmin")]
        private bool pIsSupperAdmin { get; set; } // giá trị từ MainLayout

        #region "Private Functions"

        /// <summary>
        /// get dữ liệu bảng giá
        /// </summary>
        /// <param name="isLoading"></param>
        /// <returns></returns>
        private async Task getData(bool isLoading = false)
        {
            ListRoomPrice = new List<RoomPriceModel>();
            SelectedRoomPrice = new List<RoomPriceModel>();
            var resString = await _service!.GetData(EndpointConstants.URL_ROOMPRICE_GETALL);
            ListRoomPrice = JsonConvert.DeserializeObject<List<RoomPriceModel>>(resString);
        }

        //get dữ liệu phòng
        private async Task getCombo()
        {
            //var resStringRoom = await _service!.GetData(EndpointConstants.URL_ROOM_GETALL);
            //ListRoom = JsonConvert.DeserializeObject<List<RoomModel>>(resStringRoom);

            var resStringBHouse = await _service!.GetData(EndpointConstants.URL_BOARDINGHOUSE_GETALL);
            ListBoardingHouse = JsonConvert.DeserializeObject<List<BoardingHouseModel>>(resStringBHouse);

        }
        private async Task showLoading(bool isShow = true)
        {
            if (isShow) { _spinner!.Show(); await Task.Yield(); }
            else _spinner!.Hide();
        }

        private async Task getRoomByBHouse(int bHouseId)
        {
            try
            {
                await showLoading();
                ListRoom = new List<RoomModel>();
                var request = new Dictionary<string, object>
                    {
                        { "bhouse_id", bHouseId }
                    };
                var resStringRoom = await _service!.GetData(EndpointConstants.URL_ROOM_GET_BY_BHOUSE, request);
                ListRoom = JsonConvert.DeserializeObject<List<RoomModel>>(resStringRoom);
            }
            catch (Exception ex)
            {
                _logger!.LogError(ex, "getRoomByBHouse");
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
        protected async void OnChangeBHouseHandler(int bHouseId)
        {
            RoomPriceUpdate.BoardingHouse_Id = bHouseId;
            await getRoomByBHouse(bHouseId);
        }

        protected async Task OnRowDoubleClickHandler(GridRowClickEventArgs args) 
            => await OnOpenDialogHandler(EnumType.Update,args.Item as RoomPriceModel);
        

        /// <summary>
        /// mở popup
        /// </summary>
        protected async Task OnOpenDialogHandler(EnumType pAction = EnumType.Add, RoomPriceModel? pItemDetails = null)
        {
            try
            {
                if (pAction == EnumType.Add)
                {
                    RoomPriceUpdate = new RoomPriceModel();
                    _EditContext = new EditContext(RoomPriceUpdate);
                    IsUpate = false;
                }
                else
                {
                    RoomPriceUpdate.Id = pItemDetails!.Id;
                    RoomPriceUpdate.Price = pItemDetails!.Price;
                    RoomPriceUpdate.Room_Id = pItemDetails!.Room_Id;
                    _EditContext = new EditContext(RoomPriceUpdate);
                    IsUpate = true;
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
                await getData();
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
            _EditContext = new EditContext(RoomPriceUpdate);
        }

        /// <summary>
        /// Thêm/Cập nhật thông tin gía phòng
        /// </summary>
        /// <param name="pEnum"></param>
        /// <returns></returns>
        protected async void SaveDataHandler(EnumType pEnum = EnumType.SaveAndClose)
        {
            try
            {
                string sMessage = "Thêm";
                string sAction = nameof(EnumType.Add);
                if (RoomPriceUpdate.Id > 0)
                {
                    sAction = nameof(EnumType.Update);
                    sMessage = "Cập nhật";
                }
                var checkData = _EditContext!.Validate();
                if (!checkData) return;
                if (!IsUpate) // nếu là thêm
                {
                    await showLoading();
                    RequestModel request = new RequestModel()
                    {
                        Json = JsonConvert.SerializeObject(RoomPriceUpdate),
                    };
                    var resString = await _service!.AddOrUpdateData(EndpointConstants.URL_ROOMPRICE_CREATE, request);
                    var response = JsonConvert.DeserializeObject<ResponseModel<RoomPriceModel>>(resString);
                    if (response != null && response.StatusCode == 200)
                    {
                        _toastService!.ShowSuccess($"Đã {sMessage} thông tin giá phòng.");
                        await getData();
                        IsShowDialog = false;
                        return;
                    }
                    _toastService?.ShowError(response?.Message);
                }
                else{ // nếu là sửa
                    await showLoading();
                    RequestModel request = new RequestModel()
                    {
                        Json = JsonConvert.SerializeObject(RoomPriceUpdate)
                    };
                    var resString = await _service!.AddOrUpdateData(EndpointConstants.URL_ROOMPRICE_UPDATE, request);
                    var response = JsonConvert.DeserializeObject<ResponseModel<RoomPriceModel>>(resString);
                    if (response != null && response.StatusCode == 200)
                    {
                        _toastService!.ShowSuccess($"Đã {sMessage} thông tin giá phòng.");
                        await getData();
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
            if (SelectedRoomPrice == null || !SelectedRoomPrice.Any())
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
                    var oDelete = SelectedRoomPrice.Select(m => new { m.Id });
                    string json = JsonConvert.SerializeObject(oDelete);
                    RequestModel request = new RequestModel()
                    {
                        Json = json
                    };
                    var resString = await _service.AddOrUpdateData(EndpointConstants.URL_ROOMPRICE_DELETE, request);
                    if (!string.IsNullOrEmpty(resString))
                    {
                        _toastService!.ShowSuccess($"Đã xóa thông tin giá phòng.");
                        await getData();
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
