using BHSystem.Web.Constants;
using BHSystem.Web.Core;
using BHSystem.Web.Features.Admin;
using BHSystem.Web.ViewModels;
using BHSytem.Models.Models;
using Blazored.Toast.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
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

        [CascadingParameter]
        private int pUserId { get; set; } // giá trị từ MainLayout
        #region "Private Functions"
        private async Task showLoading(bool isShow = true)
        {
            if (isShow) { _spinner!.Show(); await Task.Yield(); }
            else _spinner!.Hide();
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

        protected async void SaveDataHandler()
        {
            try
            {
                var checkData = _EditContext!.Validate();
                if (!checkData) return;
                await showLoading();
                BookingUpdate.Room_Id = 3;
                BookingUpdate.UserId = 1;
                RequestModel request = new RequestModel()
                {
                    Json = JsonConvert.SerializeObject(BookingUpdate),
                    Type = nameof(EnumType.Add),
                    UserId = pUserId
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
        #endregion
    }
}
