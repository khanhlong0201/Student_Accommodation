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
        public IEnumerable<BoardingHouseModel>? SelectedBoardingHouse { get; set; } = new List<BoardingHouseModel>();
        public bool IsInitialDataLoadComplete { get; set; } = true;
        public BoardingHouseModel BoardingHouseUpdate { get; set; } = new BoardingHouseModel();

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
                //await InvokeAsync(StateHasChanged);
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


                var request = new
                {
                    Name = BoardingHouseUpdate.Name,
                    User_Id = BoardingHouseUpdate.User_Id,
                    Ward_Id = BoardingHouseUpdate.Ward_Id,
                    Adddress = BoardingHouseUpdate.Adddress,
                    Qty = BoardingHouseUpdate.Qty
                    //Image_Id =  BoardingHouseUpdate.Image_Id
                };
                var resString = await _service!.AddOrUpdateData(EndpointConstants.URL_BOARDINGHOUSE_UPDATE, request);
                var response = JsonConvert.DeserializeObject<ResponseModel<BoardingHouseModel>>(resString);
                if (response != null && response.StatusCode == 0)
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
                //await InvokeAsync(StateHasChanged);
            }
        }

        #endregion "Protected Functions"
    }
}
