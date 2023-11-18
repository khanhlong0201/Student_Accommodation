using BHSystem.Web.Constants;
using BHSystem.Web.Controls;
using BHSystem.Web.Core;
using BHSystem.Web.Providers;
using BHSystem.Web.ViewModels;
using BHSytem.Models.Models;
using Blazored.Toast.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.JSInterop;
using Newtonsoft.Json;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using Telerik.Blazor.Components;
using Telerik.Blazor.Components.Editor;
using Telerik.DataSource.Extensions;

namespace BHSystem.Web.Features.Admin
{
    public partial class RoomPage
    {
        [Inject] private ILogger<RoomPage>? _logger { get; init; }
        [Inject] private ILoadingCore? _spinner { get; set; }
        [Inject] private IToastService? _toastService { get; set; }
        [Inject] private IApiService? _apiService { get; set; }
        [Inject] private IConfiguration? _configuration { get; set; }
        [Inject] NavigationManager? _navigationManager { get; set; }
        [Inject] private IJSRuntime? jsRuntime { get; set; }

        public List<RoomModel>? ListRooms { get; set; }
        public IEnumerable<RoomModel>? SelectedRooms { get; set; } = new List<RoomModel>();
        public RoomModel RoomUpdate { get; set; } = new RoomModel();
        public bool IsCreate { get; set; } = true;
        public bool IsShowDialog { get; set; }
        public EditContext? _EditContext { get; set; }
        public BHConfirm? _rDialogs { get; set; }
        public int pBHouseId { get; set; } = -1;
        public bool IsShowUpdateStatus { get; set; }
        public List<string> ListStatus = new List<string>() { "Đã thuê", "Phòng trống" };
        public string Status = "";
        public List<IBrowserFile> ListBrowserFiles { get; set; } = new();   // Danh sách file lưu tạm => Upload file
        public List<ImagesDetailModel> ListImages = new List<ImagesDetailModel>();

        [CascadingParameter(Name = "pUserId")]
        private int pUserId { get; set; } // giá trị từ MainLayout

        [CascadingParameter(Name = "pIsSupperAdmin")]
        private bool pIsSupperAdmin { get; set; } // giá trị từ MainLayout

        [CascadingParameter(Name = "pListMenus")]
        private List<MenuModel>? ListMenus { get; set; } // giá trị từ MainLayout

        #region "Override Functions"
        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            try
            {
                // đọc giá tri câu query
                var uri = _navigationManager?.ToAbsoluteUri(_navigationManager.Uri);
                if (uri != null)
                {
                    var queryStrings = QueryHelpers.ParseQuery(uri.Query);
                    if (queryStrings.Count() > 0)
                    {
                        string key = uri.Query.Substring(5); // để tránh parse lỗi;
                        Dictionary<string, string> pParams = JsonConvert.DeserializeObject<Dictionary<string, string>>(EncryptHelper.Decrypt(key));
                        //Dictionary<string, string> pParams = JsonConvert.DeserializeObject<Dictionary<string, string>>(EncryptHelper.Decrypt(key + ""));
                        if (pParams != null && pParams.Any() && pParams.ContainsKey("BHouseID"))
                        {
                            pBHouseId = Convert.ToInt32(pParams["BHouseID"]);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "OnInitializedAsync");
            }
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await base.OnAfterRenderAsync(firstRender);
            if (firstRender)
            {
                try
                {
                    var findMenu = ListMenus?.FirstOrDefault(m => m.Link?.ToUpper() == "/admin/boarding-house".ToUpper()); // tìm lấy menu không
                    if (findMenu == null)
                    {
                        _toastService!.ShowInfo("Bạn không có quyền vào menu này!!!");
                        await Task.Delay(4500);
                        _navigationManager!.NavigateTo("/admin/logout");
                        return;
                    }
                    if (pBHouseId > 0)
                    {
                        await showLoading();
                        await getDataRoomByBHouse();
                        return;
                    }
                    _toastService!.ShowError("Dữ liệu mã hóa phòng trọ không khớp. Vui lòng kiểm tra lại");
                }
                catch (Exception ex)
                {
                    _logger?.LogError(ex, "OnInitializedAsync");
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
        /// lấy danh sách phòng trọ
        /// </summary>
        /// <returns></returns>
        private async Task getDataRoomByBHouse()
        {
            ListRooms = new List<RoomModel>();
            SelectedRooms = new List<RoomModel>();
            var request = new Dictionary<string, object>
            {
                { "pBHouseId", pBHouseId }
            };
            string resString = await _apiService!.GetData("Rooms/GetDataByBHouse", request);
            if (!string.IsNullOrEmpty(resString)) ListRooms = JsonConvert.DeserializeObject<List<RoomModel>>(resString);
        }

        /// <summary>
        /// lấy danh sách ảnh
        /// </summary>
        /// <param name="imageId"></param>
        /// <returns></returns>
        private async Task getImageDeteailByImageId(int imageId)
        {
            ListImages = new List<ImagesDetailModel>();
            Dictionary<string, object> pParams = new Dictionary<string, object>()
            {
                {"imageId", $"{imageId}"}
            };
            var resString = await _apiService!.GetData(EndpointConstants.URL_IMAGE_DETAIL_GET_BY_IMAGE_ID, pParams);
            if (!string.IsNullOrEmpty(resString))
            {
                string url = _configuration!.GetSection("appSettings:ApiUrl").Value + DefaultConstants.FOLDER_ROOM + "/";
                var lstData = JsonConvert.DeserializeObject<List<ImagesDetailModel>>(resString)!;
                ListImages = lstData.Select(m => new ImagesDetailModel() { ImageUrl = url + m.ImageUrl, Id = m.Id, Image_Id = m.Image_Id, File_Name = m.ImageUrl }).ToList();
            }
        }
        #endregion

        #region "Protected Functions"
        /// <summary>
        /// load dữ liệu
        /// </summary>
        /// <returns></returns>
        protected async void ReLoadDataHandler()
        {
            try
            {
                if(pBHouseId < 0)
                {
                    _toastService!.ShowError("Dữ liệu mã hóa phòng trọ không khớp. Vui lòng kiểm tra lại");
                    return;
                }    
                await showLoading();
                await getDataRoomByBHouse();
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
        protected async void OnOpenDialogHandler(EnumType pAction = EnumType.Add, RoomModel? pItemDetails = null)
        {
            try
            {
                if (pBHouseId < 0)
                {
                    _toastService!.ShowError("Dữ liệu mã hóa phòng trọ không khớp. Vui lòng kiểm tra lại");
                    return;
                } 
                if(pAction == EnumType.UpdateStatus)
                {
                    if (SelectedRooms == null || !SelectedRooms.Any())
                    {
                        _toastService!.ShowWarning("Vui lòng chọn dòng để cập nhật tình trạng.");
                        return;
                    }
                    IsShowUpdateStatus = true;
                    Status = "";
                    return;
                }
                ListBrowserFiles = new List<IBrowserFile>();
                if (pAction == EnumType.Add)
                {
                    RoomUpdate = new RoomModel();
                    ListImages = new List<ImagesDetailModel>();
                    IsCreate = true;
                    _EditContext = new EditContext(RoomUpdate);
                }
                else
                {
                    RoomUpdate.Id = pItemDetails!.Id;
                    RoomUpdate.Name = pItemDetails!.Name;
                    RoomUpdate.Status = pItemDetails!.Status;
                    RoomUpdate.BHouseId = pItemDetails!.BHouseId;
                    RoomUpdate.Address = pItemDetails!.Address;
                    RoomUpdate.Length  = pItemDetails!.Length;
                    RoomUpdate.Width = pItemDetails!.Width;
                    RoomUpdate.Price = pItemDetails!.Price;
                    RoomUpdate.Image_Id = pItemDetails!.Image_Id;
                    RoomUpdate.Description = pItemDetails!.Description;
                    IsCreate = false;
                    _EditContext = new EditContext(RoomUpdate);
                    await showLoading();
                    await getImageDeteailByImageId(RoomUpdate.Image_Id);
                }
                IsShowDialog = true;
            }
            catch (Exception ex)
            {
                _logger!.LogError(ex, "BHousePage", "OnOpenDialogHandler");
                _toastService!.ShowError(ex.Message);
            }
            finally
            {
                await showLoading(false);
                await InvokeAsync(StateHasChanged);
            }
        }

        protected void OnRowDoubleClickHandler(GridRowClickEventArgs args) => OnOpenDialogHandler(EnumType.Update, args.Item as RoomModel);

        protected async void SaveDataHandler(EnumType pEnum = EnumType.SaveAndClose)
        {
            try
            {
                var checkData = _EditContext!.Validate();
                if (!checkData) return;
                await showLoading();
                string sMessage = "Thêm";
                string sAction = nameof(EnumType.Add);
                if (RoomUpdate.Id > 0)
                {
                    sAction = nameof(EnumType.Update);
                    sMessage = "Cập nhật";
                }

                async Task Action()
                {
                    RequestModel request = new RequestModel()
                    {
                        Json = JsonConvert.SerializeObject(RoomUpdate),
                        Type = sAction,
                        UserId = pUserId
                    };
                    string resString = await _apiService!.AddOrUpdateData("Rooms/Update", request);
                    if (!string.IsNullOrEmpty(resString))
                    {
                        _toastService!.ShowSuccess($"Đã {sMessage} thông tin phòng trọ. Vui lòng đợi Admin hệ thống duyệt.");
                        await getDataRoomByBHouse();
                        if (pEnum == EnumType.SaveAndCreate)
                        {
                            RoomUpdate = new RoomModel();
                            ListImages = new List<ImagesDetailModel>();
                            ListBrowserFiles = new List<IBrowserFile>();
                            _EditContext = new EditContext(RoomUpdate);
                            return;
                        }
                        IsShowDialog = false;
                    }
                }

                if (sAction == nameof(EnumType.Add))
                {
                    if (ListBrowserFiles == null || !ListBrowserFiles.Any())
                    {
                        // kiểm tra file
                        _toastService!.ShowWarning("Vui lòng chọn hình ảnh cho phòng");
                        return;
                    }
                    RoomUpdate.BHouseId = pBHouseId;
                    // lưu file -> nhả lên các 
                    string resStringFile = await _apiService!.UploadMultiFiles($"Images/UploadImages?subFolder={DefaultConstants.FOLDER_ROOM}", ListBrowserFiles);
                    if (!string.IsNullOrEmpty(resStringFile))
                    {
                        RoomUpdate.ListFile = JsonConvert.DeserializeObject<List<ImagesDetailModel>>(resStringFile);
                        await Action();
                    }
                }
                else
                {
                    RoomUpdate.ListFile = ListImages;
                    if (RoomUpdate.ListFile == null) RoomUpdate.ListFile = new List<ImagesDetailModel>();
                    // nếu có file đính kèm ->
                    if (ListBrowserFiles != null && ListBrowserFiles.Any())
                    {
                        // lưu file -> nhả lên các 
                        string resStringFile = await _apiService!.UploadMultiFiles($"Images/UploadImages?subFolder={DefaultConstants.FOLDER_ROOM}", ListBrowserFiles);
                        if (!string.IsNullOrEmpty(resStringFile))
                        {
                            if (RoomUpdate.ListFile == null) RoomUpdate.ListFile = new List<ImagesDetailModel>();
                            RoomUpdate.ListFile.AddRange(JsonConvert.DeserializeObject<List<ImagesDetailModel>>(resStringFile));
                            await Action();
                        }
                        return;
                    }
                    await Action();
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

        /// <summary>
        /// load image lên để view
        /// </summary>
        /// <param name="args"></param>
        protected async void OnLoadFileHandler(InputFileChangeEventArgs args)
        {
            try
            {
                //await args.File.RequestImageFileAsync("image/*", 600, 600);
                ListImages = new List<ImagesDetailModel>();
                if (ListBrowserFiles == null) ListBrowserFiles = new List<IBrowserFile>();
                ListBrowserFiles.AddRange(args.GetMultipleFiles());
                foreach (var item in args.GetMultipleFiles())
                {
                    using Stream imageStream = item.OpenReadStream(long.MaxValue);
                    using MemoryStream ms = new();
                    //copy imageStream to Memory stream
                    await imageStream.CopyToAsync(ms);
                    //convert stream to base64
                    ListImages.Add(new ImagesDetailModel() { ImageUrl = $"data:image/png;base64,{Convert.ToBase64String(ms.ToArray())}" });
                    await ms.FlushAsync();
                    await ms.DisposeAsync();
                }
            }
            catch (Exception ex)
            {
                _logger!.LogError(ex, "OnLoadFileHandler");
                _toastService!.ShowError(ex.Message);
            }
            finally
            {
                await InvokeAsync(StateHasChanged);
            }
        }

        /// <summary>
        /// Xóa 1 hoặc nhiều phòng trọ
        /// </summary>
        /// <returns></returns>
        protected async Task ConfirmDeleteHandler()
        {
            if (pBHouseId < 0)
            {
                _toastService!.ShowError("Dữ liệu mã hóa phòng trọ không khớp. Vui lòng kiểm tra lại");
                return;
            }
            if (SelectedRooms == null || !SelectedRooms.Any())
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
                    var oDelete = SelectedRooms.Select(m => new { m.Id });
                    RequestModel request = new RequestModel()
                    {
                        Json = JsonConvert.SerializeObject(oDelete),
                        UserId = pUserId
                    };
                    string resString = await _apiService!.AddOrUpdateData("Rooms/Delete", request);
                    if (!string.IsNullOrEmpty(resString))
                    {
                        _toastService!.ShowSuccess($"Đã xóa thông tin phòng được chọn.");
                        await getDataRoomByBHouse();
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

        protected async void ConfirmUpdateStatus()
        {
            if (string.IsNullOrEmpty(Status))
            {
                _toastService!.ShowError("Vui lòng chọn tình trạng");
                return;
            }
            try
            {
                await showLoading();
                var oDelete = SelectedRooms!.Select(m => new { m.Id });
                RequestModel request = new RequestModel()
                {
                    Json = JsonConvert.SerializeObject(oDelete),
                    UserId = pUserId,
                    Type = Status
                };
                string resString = await _apiService!.AddOrUpdateData(EndpointConstants.URL_ROOM_UPDATE_STATUS, request);
                if (!string.IsNullOrEmpty(resString))
                {
                    _toastService!.ShowSuccess($"Đã nhập nhật tình trạng [{Status}] cho các phòng được chọn");
                    await getDataRoomByBHouse();
                }
            }
            catch (Exception ex)
            {
                _logger!.LogError(ex, "ConfirmUpdateStatus");
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
