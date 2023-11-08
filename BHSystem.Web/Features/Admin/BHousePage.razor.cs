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
using Org.BouncyCastle.Asn1.Ocsp;

namespace BHSystem.Web.Features.Admin
{
    public partial class BHousePage
    {
        [Inject] private ILogger<UserPage>? _logger { get; init; }
        [Inject] private ILoadingCore? _spinner { get; set; }
        [Inject] private IToastService? _toastService { get; set; }
        [Inject] private IApiService? _apiService { get; set; }
        public List<BHouseModel>? ListBHouses { get; set; }
        public IEnumerable<BHouseModel>? SelectedBHouses { get; set; } = new List<BHouseModel>();
        public BHouseModel BHouseUpdate { get; set; } = new BHouseModel();
        public bool IsCreate { get; set; } = true;
        public bool IsShowDialog { get; set; }
        public EditContext? _EditContext { get; set; }
        public BHConfirm? _rDialogs { get; set; }

        [CascadingParameter]
        private int pUserId { get; set; } // giá trị từ MainLayout

        public List<CityModel> ListCity { get; set; } = new List<CityModel>();
        public List<DistinctModel>? ListDistinct { get; set; } = new List<DistinctModel>();
        public List<WardModel>? ListWard { get; set; } = new List<WardModel>();
        public List<IBrowserFile> ListBrowserFiles { get; set; } = new();   // Danh sách file lưu tạm => Upload file
        public List<string> ListImages = new List<string>();
        #region
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if(firstRender)
            {
                try
                {
                    await showLoading();
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
                //await getDataUser();
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
        protected void OnOpenDialogHandler(EnumType pAction = EnumType.Add, UserModel? pItemDetails = null)
        {
            try
            {

                if (pAction == EnumType.Add)
                {
                    BHouseUpdate = new BHouseModel();
                    ListImages = new List<string>();
                    ListBrowserFiles = new List<IBrowserFile>();
                    IsCreate = true;
                }
                else
                {
                    //UserUpdate.UserId = pItemDetails!.UserId;
                    //UserUpdate.FullName = pItemDetails!.FullName;
                    //UserUpdate.UserName = pItemDetails!.UserName;
                    //UserUpdate.Password = EncryptHelper.Decrypt(pItemDetails!.Password + "");
                    //UserUpdate.Address = pItemDetails!.Address;
                    //UserUpdate.Email = pItemDetails!.Email;
                    IsCreate = false;
                }
                IsShowDialog = true;
                _EditContext = new EditContext(BHouseUpdate);
            }
            catch (Exception ex)
            {
                _logger!.LogError(ex, "BHousePage", "OnOpenDialogHandler");
                _toastService!.ShowError(ex.Message);
            }
        }

        /// <summary>
        /// Thêm/Cập nhật thông tin người dùng
        /// </summary>
        /// <param name="pEnum"></param>
        /// <returns></returns>
        protected async void SaveDataHandler(EnumType pEnum = EnumType.SaveAndClose)
        {
            try
            {
                await showLoading();
                // lưu file -> nhả lên các 
                string resStringFile = await _apiService!.UploadMultiFiles("Images/UploadImages", ListBrowserFiles);
                if (!string.IsNullOrEmpty(resStringFile))
                {
                    BHouseUpdate.ListFile = JsonConvert.DeserializeObject<List<ImagesDetailModel>>(resStringFile);
                    //bool isUpdate = await _apiService!.UpdateAsync(JsonConvert.SerializeObject(BHouseUpdate), sAction, pUserId);
                    //if (isUpdate)
                    //{
                    //    _toastService!.ShowSuccess($"Đã lưu thông tin phòng trọ.");
                    //    //await getDataUser();
                    //    if (pEnum == EnumType.SaveAndCreate)
                    //    {
                    //        BHouseUpdate = new BHouseModel();
                    //        _EditContext = new EditContext(BHouseUpdate);
                    //        ListImages = new List<string>();
                    //        ListBrowserFiles = new List<IBrowserFile>();
                    //        return;
                    //    }
                    //    IsShowDialog = false;
                    //}
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

        
        protected async void OnLoadFileHandler(InputFileChangeEventArgs args)
        {
            try
            {
                //await args.File.RequestImageFileAsync("image/*", 600, 600);
                ListImages = new List<string>();
                if (ListBrowserFiles == null) ListBrowserFiles = new List<IBrowserFile>();
                ListBrowserFiles.AddRange(args.GetMultipleFiles());
                foreach (var item in args.GetMultipleFiles())
                {
                    using Stream imageStream = item.OpenReadStream(long.MaxValue);
                    using MemoryStream ms = new();
                    //copy imageStream to Memory stream
                    await imageStream.CopyToAsync(ms);
                    //convert stream to base64
                    ListImages.Add($"data:image/png;base64,{Convert.ToBase64String(ms.ToArray())}");
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
        #endregion
    }
}
