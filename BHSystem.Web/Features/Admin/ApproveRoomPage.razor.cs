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
    public partial class ApproveRoomPage
    {
        [Inject] private ILogger<UserPage>? _logger { get; init; }
        [Inject] private ILoadingCore? _spinner { get; set; }
        [Inject] private IToastService? _toastService { get; set; }
        [Inject] private IApiService? _apiService { get; set; }
        [Inject] private IConfiguration? _configuration { get; set; }
        [Inject] NavigationManager? _navigationManager { get; set; }
        public List<RoomModel>? ListRoomWaitting { get; set; }
        public List<RoomModel>? ListRoomAll { get; set; }
        public IEnumerable<RoomModel>? SelectedRoomWaitting { get; set; } = new List<RoomModel>();
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
                    var findMenu = ListMenus?.FirstOrDefault(m => m.Link?.ToUpper() == "/admin/approve-room".ToUpper()); // tìm lấy menu không
                    if (findMenu == null)
                    {
                        _toastService!.ShowInfo("Bạn không có quyền vào menu này!!!");
                        await Task.Delay(4500);
                        _navigationManager!.NavigateTo("/admin/logout");
                        return;
                    }
                    await showLoading();
                    await getDataRoom("Chờ xử lý");
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
        /// lấy danh sách phòng cần phê duyệt
        /// </summary>
        /// <returns></returns>
        private async Task getDataRoom(string type = "")
        {
            // Gọi hàm và truyền giá trị cho pParams
            Dictionary<string, object> parameters = new Dictionary<string, object>
            {
                { "type", type }
            };
           
            string resString = await _apiService!.GetData(EndpointConstants.URL_ROOM_GET_BY_STATUS, parameters);
            if (!string.IsNullOrEmpty(resString))
            {
                if (type + "" == "Chờ xử lý")
                {
                    ListRoomWaitting = new List<RoomModel>();
                    ListRoomWaitting = JsonConvert.DeserializeObject<List<RoomModel>>(resString);
                }
                else
                {
                    ListRoomAll = new List<RoomModel>();
                    ListRoomAll = JsonConvert.DeserializeObject<List<RoomModel>>(resString);
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
                await getDataRoom(Wait);
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
            if (SelectedRoomWaitting == null || !SelectedRoomWaitting.Any())
            {
                _toastService!.ShowWarning("Vui lòng chọn dòng để từ chối");
                return;
            }
            var confirm = await _rDialogs!.ConfirmAsync($"Bạn có chắc muốn {type} các dòng được chọn? ");
            if (confirm)
            {
                try
                {
                    await showLoading();
                    var oDelete = SelectedRoomWaitting.Select(m => new { m.Id });
                    RequestModel request = new RequestModel()
                    {
                        Json = JsonConvert.SerializeObject(oDelete),
                        UserId = pUserId,
                        Type = type
                    };
                    string resString = await _apiService!.AddOrUpdateData(EndpointConstants.URL_ROOM_UPDATE_STATUS, request);
                    if (!string.IsNullOrEmpty(resString))
                    {
                        if(type +""=="Từ chối") _toastService!.ShowSuccess($"Đã từ chối danh sách phòng được chọn.");
                        else _toastService!.ShowSuccess($"Đã phê duyệt danh sách phòng được chọn.");
                        await getDataRoom("Chờ xử lý");
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
