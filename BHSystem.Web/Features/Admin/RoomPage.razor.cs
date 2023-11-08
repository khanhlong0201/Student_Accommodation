using BHSystem.Web.Controls;
using BHSystem.Web.Core;
using BHSystem.Web.Providers;
using BHSystem.Web.ViewModels;
using BHSytem.Models.Models;
using Blazored.Toast.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.WebUtilities;
using Newtonsoft.Json;
using Telerik.Blazor.Components;
using Telerik.Blazor.Components.Editor;

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

        public List<RoomModel>? ListRooms { get; set; }
        public IEnumerable<RoomModel>? SelectedRooms { get; set; } = new List<RoomModel>();
        public RoomModel RoomUpdate { get; set; } = new RoomModel();
        public bool IsCreate { get; set; } = true;
        public bool IsShowDialog { get; set; }
        public EditContext? _EditContext { get; set; }
        public BHConfirm? _rDialogs { get; set; }
        public int pBHouseId { get; set; } = -1;

        public List<IBrowserFile> ListBrowserFiles { get; set; } = new();   // Danh sách file lưu tạm => Upload file
        public List<ImagesDetailModel> ListImages = new List<ImagesDetailModel>();

        [CascadingParameter]
        private int pUserId { get; set; } // giá trị từ MainLayout

        public List<IEditorTool> Tools { get; set; } =
        new List<IEditorTool>()
        {
            new EditorButtonGroup(new Telerik.Blazor.Components.Editor.Bold(), new Telerik.Blazor.Components.Editor.Italic(), new Telerik.Blazor.Components.Editor.Underline()),
            new EditorButtonGroup(new Telerik.Blazor.Components.Editor.AlignLeft(), new Telerik.Blazor.Components.Editor.AlignCenter(), new Telerik.Blazor.Components.Editor.AlignRight()),
            new UnorderedList(),
            new EditorButtonGroup(new CreateLink(), new Telerik.Blazor.Components.Editor.Unlink(), new InsertImage()),
            new InsertTable(),
            new EditorButtonGroup(new AddRowBefore(), new AddRowAfter(), new MergeCells(), new SplitCell()),
            new Format(),
            new Telerik.Blazor.Components.Editor.FontSize(),
            new Telerik.Blazor.Components.Editor.FontFamily()
        };

        #region "Override Functions"
        protected override Task OnInitializedAsync()
        {
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
            return base.OnInitializedAsync();
        }

        #endregion

        #region "Private Functions"
        private async Task showLoading(bool isShow = true)
        {
            if (isShow) { _spinner!.Show(); await Task.Yield(); }
            else _spinner!.Hide();
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
                //await getDataBHouse();
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

                if (pAction == EnumType.Add)
                {
                    RoomUpdate = new RoomModel();
                    ListImages = new List<ImagesDetailModel>();
                    ListBrowserFiles = new List<IBrowserFile>();
                    IsCreate = true;
                    _EditContext = new EditContext(RoomUpdate);
                }
                else
                {

                    //BHouseUpdate.Id = pItemDetails!.Id;
                    //BHouseUpdate.Name = pItemDetails!.Name;
                    //BHouseUpdate.Qty = pItemDetails!.Qty;
                    //BHouseUpdate.Ward_Id = pItemDetails!.Ward_Id;
                    //BHouseUpdate.City_Id = pItemDetails!.City_Id;
                    //BHouseUpdate.Distinct_Id = pItemDetails!.Distinct_Id;
                    //BHouseUpdate.Adddress = pItemDetails!.Adddress;
                    //BHouseUpdate.Image_Id = pItemDetails!.Image_Id;
                    //IsCreate = false;
                    //_EditContext = new EditContext(BHouseUpdate);
                    //await showLoading();
                    //Task task1 = getDistrictByCity(BHouseUpdate.City_Id);
                    //Task task2 = getWardByDistrict(BHouseUpdate.Distinct_Id);
                    //Task task3 = getImageDeteailByImageId(BHouseUpdate.Image_Id);
                    //await Task.WhenAll(task1, task2, task3);
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
        #endregion
    }
}
