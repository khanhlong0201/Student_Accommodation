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
using Newtonsoft.Json;
using Org.BouncyCastle.Asn1.Ocsp;
using System.Diagnostics;
using Telerik.Blazor.Components;

namespace BHSystem.Web.Features.Admin
{
    public partial class ApproveBookingPage
    {
        [Inject] private ILogger<UserPage>? _logger { get; init; }
        [Inject] private ILoadingCore? _spinner { get; set; }
        [Inject] private IToastService? _toastService { get; set; }
        [Inject] private IApiService? _apiService { get; set; }
        [Inject] private IConfiguration? _configuration { get; set; }
        [Inject] NavigationManager? _navigationManager { get; set; }
        public List<BookingModel>? ListBookingWaitting { get; set; }
        public List<BookingModel>? ListBookingAll { get; set; }
        public IEnumerable<BookingModel>? SelectedBookingWaitting { get; set; } = new List<BookingModel>();
        public BHConfirm? _rDialogs { get; set; }

        [CascadingParameter(Name = "pUserId")]
        private int pUserId { get; set; } // giá trị từ MainLayout

        [CascadingParameter(Name = "pIsSupperAdmin")]
        private bool pIsSupperAdmin { get; set; } // giá trị từ MainLayout

        [CascadingParameter(Name = "pListMenus")]
        private List<MenuModel>? ListMenus { get; set; } // giá trị từ MainLayout
        #region
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await base.OnAfterRenderAsync(firstRender);
            if (firstRender)
            {
                try
                {
                    var findMenu = ListMenus?.FirstOrDefault(m => m.Link?.ToUpper() == "/admin/approve-booking".ToUpper()); // tìm lấy menu không
                    if (findMenu == null)
                    {
                        _toastService!.ShowInfo("Bạn không có quyền vào menu này!!!");
                        await Task.Delay(4500);
                        _navigationManager!.NavigateTo("/admin/logout");
                        return;
                    }
                    await showLoading();
                    await getDataBooking("Chờ xử lý");
                }   
                catch(Exception ex)
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
        /// lấy danh sách booking
        /// </summary>
        /// <returns></returns>
        private async Task getDataBooking(string type = "")
        {
            // Gọi hàm và truyền giá trị cho pParams
            Dictionary<string, object> parameters = new Dictionary<string, object>
            {
                { "type", type },
                {"pUserId", $"{pUserId}"},
                {"pIsAdmin", $"{pIsSupperAdmin}"}
            };
           
            string resString = await _apiService!.GetData(EndpointConstants.URL_BOOKING_GET_BY_STATUS, parameters);
            if (!string.IsNullOrEmpty(resString))
            {
                if (type + "" == "Chờ xử lý")
                {
                    ListBookingWaitting = new List<BookingModel>();
                    ListBookingWaitting = JsonConvert.DeserializeObject<List<BookingModel>>(resString);
                }
                else
                {
                    ListBookingAll = new List<BookingModel>();
                    ListBookingAll = JsonConvert.DeserializeObject<List<BookingModel>>(resString);
                }
            }
        }
        #endregion
        #region "Protected Functions"
        /// <summary>
        /// load dữ liệu
        /// </summary>
        /// <returns></returns>
        protected async void ReLoadDataHandler(string Wait ="")
        {
            try
            {
                await showLoading();
                await getDataBooking(Wait);
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
        /// Từ chối 1 hoặc nhiều booking
        /// </summary>
        /// <returns></returns>
        protected async Task ConfirmHandler(string type= "")
        {
            if (SelectedBookingWaitting == null || !SelectedBookingWaitting.Any())
            {
                _toastService!.ShowWarning("Vui lòng chọn dòng dữ liệu");
                return;
            }
            var confirm = await _rDialogs!.ConfirmAsync($"Bạn có chắc muốn {type} các dòng được chọn? ");
            if (confirm)
            {
                try
                {
                    await showLoading();
                    var oDelete = SelectedBookingWaitting.Select(m => new { m.Id });
                    RequestModel request = new RequestModel()
                    {
                        Json = JsonConvert.SerializeObject(oDelete),
                        UserId = pUserId,
                        Type = type
                    };
                    string resString = await _apiService!.AddOrUpdateData(EndpointConstants.URL_BOOKING_UPDATE_STATUS, request);
                    if (!string.IsNullOrEmpty(resString))
                    {
                        if(type +""=="Từ chối") _toastService!.ShowSuccess($"Đã từ chối danh sách đặt phòng được chọn.");
                        else _toastService!.ShowSuccess($"Đã Xác nhận thông tin. Vui lòng vào phòng trọ cập nhật tình trạng");
                        await getDataBooking("Chờ xử lý");
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

        #endregion
    }
}
