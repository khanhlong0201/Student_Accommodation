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
    public partial class BHousePage
    {
        [Inject] private ILogger<UserPage>? _logger { get; init; }
        [Inject] private ILoadingCore? _spinner { get; set; }
        [Inject] private IToastService? _toastService { get; set; }
        [Inject] private IApiService? _apiService { get; set; }
        [Inject] private IConfiguration? _configuration { get; set; }
        [Inject] NavigationManager? _navigationManager { get; set; }
        public List<BHouseModel>? ListBHouses { get; set; }
        public IEnumerable<BHouseModel>? SelectedBHouses { get; set; } = new List<BHouseModel>();
        public BHouseModel BHouseUpdate { get; set; } = new BHouseModel();
        public bool IsCreate { get; set; } = true;
        public bool IsShowDialog { get; set; }
        public EditContext? _EditContext { get; set; }
        public BHConfirm? _rDialogs { get; set; }

        [CascadingParameter(Name = "pUserId")]
        private int pUserId { get; set; } // giá trị từ MainLayout

        [CascadingParameter(Name = "pIsSupperAdmin")]
        private bool pIsSupperAdmin { get; set; } // giá trị từ MainLayout

        public List<CityModel> ListCity { get; set; } = new List<CityModel>();
        public List<DistinctModel>? ListDistinct { get; set; } = new List<DistinctModel>();
        public List<WardModel>? ListWard { get; set; } = new List<WardModel>();
        public List<IBrowserFile> ListBrowserFiles { get; set; } = new();   // Danh sách file lưu tạm => Upload file
        public List<ImagesDetailModel> ListImages = new List<ImagesDetailModel>();
        #region
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await base.OnAfterRenderAsync(firstRender);
            if (firstRender)
            {
                try
                {
                    await showLoading();
                    await getDataBHouse();
                    await getCity();
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

        /// <summary>
        /// lấy danh sách phòng trọ
        /// </summary>
        /// <returns></returns>
        private async Task getDataBHouse()
        {
            ListBHouses = new List<BHouseModel>();
            SelectedBHouses = new List<BHouseModel>();
            Dictionary<string, object> pParams = new Dictionary<string, object>()
            {
                {"pUserId", $"{pUserId}"},
                {"pIsAdmin", $"{pIsSupperAdmin}"}
            };
            string resString = await _apiService!.GetData("BHouses/GetAll", pParams);
            if (!string.IsNullOrEmpty(resString)) ListBHouses = JsonConvert.DeserializeObject<List<BHouseModel>>(resString);
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
                string url = _configuration!.GetSection("appSettings:ApiUrl").Value + DefaultConstants.FOLDER_BHOUSE + "/";
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
                await showLoading();
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

        /// <summary>
        /// mở popup
        /// </summary>
        protected async void OnOpenDialogHandler(EnumType pAction = EnumType.Add, BHouseModel? pItemDetails = null)
        {
            try
            {
                ListBrowserFiles = new List<IBrowserFile>();
                if (pAction == EnumType.Add)
                {
                    BHouseUpdate = new BHouseModel();
                    ListImages = new List<ImagesDetailModel>();
                    IsCreate = true;
                    _EditContext = new EditContext(BHouseUpdate);
                }
                else
                {

                    BHouseUpdate.Id = pItemDetails!.Id;
                    BHouseUpdate.Name = pItemDetails!.Name;
                    BHouseUpdate.Qty = pItemDetails!.Qty;
                    BHouseUpdate.Ward_Id = pItemDetails!.Ward_Id;
                    BHouseUpdate.City_Id = pItemDetails!.City_Id;
                    BHouseUpdate.Distinct_Id = pItemDetails!.Distinct_Id;
                    BHouseUpdate.Adddress = pItemDetails!.Adddress;
                    BHouseUpdate.Image_Id = pItemDetails!.Image_Id;
                    IsCreate = false;
                    _EditContext = new EditContext(BHouseUpdate);
                    await showLoading();
                    Task task1 = getDistrictByCity(BHouseUpdate.City_Id);
                    Task task2 = getWardByDistrict(BHouseUpdate.Distinct_Id);
                    Task task3 = getImageDeteailByImageId(BHouseUpdate.Image_Id);
                    await Task.WhenAll(task1, task2, task3);
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

        protected void OnRowDoubleClickHandler(GridRowClickEventArgs args) => OnOpenDialogHandler(EnumType.Update, args.Item as BHouseModel);

        /// <summary>
        /// Thêm/Cập nhật thông tin phòng trọ
        /// </summary>
        /// <param name="pEnum"></param>
        /// <returns></returns>
        protected async void SaveDataHandler(EnumType pEnum = EnumType.SaveAndClose)
        {
            try
            {
                var checkData = _EditContext!.Validate();
                if (!checkData) return;
                await showLoading();
                string sMessage = "Thêm";
                string sAction = nameof(EnumType.Add);
                if (BHouseUpdate.Id > 0)
                {
                    sAction = nameof(EnumType.Update);
                    sMessage = "Cập nhật";
                } 
                
                async Task Action()
                {
                    RequestModel request = new RequestModel()
                    {
                        Json = JsonConvert.SerializeObject(BHouseUpdate),
                        Type = sAction,
                        UserId = pUserId
                    };
                    string resString = await _apiService!.AddOrUpdateData("BHouses/Update", request);
                    if (!string.IsNullOrEmpty(resString))
                    {
                        _toastService!.ShowSuccess($"Đã {sMessage} thông tin phòng trọ.");
                        await getDataBHouse();
                        if (pEnum == EnumType.SaveAndCreate)
                        {
                            BHouseUpdate = new BHouseModel();
                            ListImages = new List<ImagesDetailModel>();
                            ListBrowserFiles = new List<IBrowserFile>();
                            _EditContext = new EditContext(BHouseUpdate);
                            return;
                        }
                        IsShowDialog = false;
                    }
                }

                if(sAction == nameof(EnumType.Add))
                {
                    if (ListBrowserFiles == null || !ListBrowserFiles.Any())
                    {
                        // kiểm tra file
                        _toastService!.ShowWarning("Vui lòng chọn hình ảnh đại diện cho phòng trọ");
                        return;
                    }
                    // lưu file -> nhả lên các 
                    string resStringFile = await _apiService!.UploadMultiFiles($"Images/UploadImages?subFolder={DefaultConstants.FOLDER_BHOUSE}", ListBrowserFiles);
                    if (!string.IsNullOrEmpty(resStringFile))
                    {
                        BHouseUpdate.ListFile = JsonConvert.DeserializeObject<List<ImagesDetailModel>>(resStringFile);
                        await Action();
                    }    
                }  
                else
                {
                    BHouseUpdate.ListFile = ListImages;
                    if (BHouseUpdate.ListFile == null) BHouseUpdate.ListFile = new List<ImagesDetailModel>();
                    // nếu có file đính kèm ->
                    if (ListBrowserFiles != null && ListBrowserFiles.Any())
                    {
                        // lưu file -> nhả lên các 
                        string resStringFile = await _apiService!.UploadMultiFiles("Images/UploadImages", ListBrowserFiles);
                        if (!string.IsNullOrEmpty(resStringFile))
                        {
                            BHouseUpdate.ListFile.AddRange(JsonConvert.DeserializeObject<List<ImagesDetailModel>>(resStringFile));
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

        protected async void OnChangeCityHandler(int iCityId)
        {
            BHouseUpdate.City_Id = iCityId;
            await getDistrictByCity(iCityId);
        }

        protected async void OnChangeDistinctHandler(int iDistinctId)
        {
            BHouseUpdate.Distinct_Id = iDistinctId;
            await getWardByDistrict(iDistinctId);
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
                    ListImages.Add(new ImagesDetailModel() { ImageUrl= $"data:image/png;base64,{Convert.ToBase64String(ms.ToArray())}" });
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
            if (SelectedBHouses == null || !SelectedBHouses.Any())
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
                    var oDelete = SelectedBHouses.Select(m => new { m.Id });
                    RequestModel request = new RequestModel()
                    {
                        Json = JsonConvert.SerializeObject(oDelete),
                        UserId = pUserId
                    };
                    string resString = await _apiService!.AddOrUpdateData("BHouses/Delete", request);
                    if (!string.IsNullOrEmpty(resString))
                    {
                        _toastService!.ShowSuccess($"Đã xóa thông tin phòng được chọn.");
                        await getDataBHouse();
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

        /// <summary>
        /// mở trang tạo mới phòng theo trọ
        /// </summary>
        protected void CreateRoomHandler()
        {
            try
            {
                // local function
                if (SelectedBHouses == null || !SelectedBHouses.Any())
                {
                    _toastService!.ShowWarning("Vui lòng chọn trọ cần thêm phòng.");
                    return;
                }
                if (SelectedBHouses.Count() > 1)
                {
                    _toastService!.ShowWarning("Chỉ được phép chọn một phòng trọ.");
                    return;
                }
                var data = SelectedBHouses.First();
                Dictionary<string, string> pParams = new Dictionary<string, string>
                {
                    { "BHouseID", $"{data.Id}" },
                };
                string key = EncryptHelper.Encrypt(JsonConvert.SerializeObject(pParams)); // mã hóa key
                _navigationManager!.NavigateTo($"/admin/boarding-house/room?key={key}");
            }
            catch (Exception ex)
            {
                _logger!.LogError(ex, "CreateRoomHandler");
                _toastService!.ShowError(ex.Message);
            }
        }
        #endregion
    }
}
