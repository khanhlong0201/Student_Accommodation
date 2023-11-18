using BHSystem.Web.Constants;
using BHSystem.Web.Controls;
using BHSystem.Web.Core;
using BHSytem.Models.Entities;
using BHSytem.Models.Models;
using Blazored.Toast.Services;
using Microsoft.AspNetCore.Components;
using Newtonsoft.Json;

namespace BHSystem.Web.Features.Admin
{
    public partial class MessagePage
    {
        [Inject] private ILogger<UserPage>? _logger { get; init; }
        [Inject] private ILoadingCore? _spinner { get; set; }
        [Inject] private IToastService? _toastService { get; set; }
        [Inject] private IApiService? _apiService { get; set; }

        public List<MessageModel> ListMessages = new List<MessageModel>();
        public List<MessageModel> ListMessagesAll = new List<MessageModel>();
        public IEnumerable<MessageModel>? SelectedMessages { get; set; } = new List<MessageModel>();


        public BHConfirm? _rDialogs { get; set; }
        [CascadingParameter(Name = "pUserId")]
        private int pUserId { get; set; } // giá trị từ MainLayout

        [CascadingParameter(Name = "pListMeassgesHandler")] // giá trị từ MainLayout
        public EventCallback<List<MessageModel>> NotifyMessage { get; set; }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await base.OnAfterRenderAsync(firstRender);
            if (firstRender)
            {
                try
                {
                    await showLoading();
                    var userId = pUserId;
                    await getUnReadMessageByUser(false);
                }
                catch (Exception ex)
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

        #region "Private Functions"
        private async Task showLoading(bool isShow = true)
        {
            if (isShow) { _spinner!.Show(); await Task.Yield(); }
            else _spinner!.Hide();
        }

        /// <summary>
        /// lấy danh sách message theo user
        /// </summary>
        /// <returns></returns>
        private async Task getUnReadMessageByUser(bool isAll = false)
        {
            ListMessages = new List<MessageModel>();
            ListMessagesAll = new List<MessageModel>();
            Dictionary<string, object> pParams = new Dictionary<string, object>()
            {
                {"pUserId", $"{pUserId}"},
                {"pIsAll", $"{isAll}"}
            };
            string resString = await _apiService!.GetData(EndpointConstants.URL_MESSAGE_BY_USER, pParams);
            if (!string.IsNullOrEmpty(resString))
            {
                if(isAll) ListMessagesAll = JsonConvert.DeserializeObject<List<MessageModel>>(resString);
                else
                {
                    ListMessages = JsonConvert.DeserializeObject<List<MessageModel>>(resString);
                }    
            }    
        }

        #endregion

        /// <summary>
        /// load dữ liệu
        /// </summary>
        /// <returns></returns>
        protected async void ReLoadDataHandler(bool isAll = false)
        {
            try
            {
                await showLoading();
                await getUnReadMessageByUser(isAll);
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

        protected async void UpdateStatusMultiHandler()
        {
            try
            {
                if (SelectedMessages == null || !SelectedMessages.Any())
                {
                    _toastService!.ShowWarning("Vui lòng chọn dòng dữ liệu");
                    return;
                }
                var confirm = await _rDialogs!.ConfirmAsync($"Bạn có chắc muốn đánh dấu đã đọc các thông báo được chọn? ");
                if(confirm)
                {
                    await showLoading();
                    bool isUpdate = false;
                    foreach(var item in SelectedMessages)
                    {
                        RequestModel request = new RequestModel()
                        {
                            Json = JsonConvert.SerializeObject(item),
                            UserId = pUserId,
                        };
                        string resString = await _apiService!.AddOrUpdateData(EndpointConstants.URL_MESSAGE_UPDATE_ISREAD, request);
                        if (!string.IsNullOrEmpty(resString)) isUpdate = true;
                    } 
                    if(isUpdate)
                    {
                        _toastService!.ShowSuccess("Đã đánh dấu thông báo đã đọc");
                        SelectedMessages = new List<MessageModel>();
                        await getUnReadMessageByUser(false);
                        await NotifyMessage.InvokeAsync(ListMessages); // gửi về lại MainLayout cập nhật giá trị
                    }    
                }    
            }
            catch (Exception ex)
            {
                _logger!.LogError(ex, "UpdateStatusMultiHandler");
                _toastService!.ShowError(ex.Message);
            }
            finally
            {
                await showLoading(false);
                await InvokeAsync(StateHasChanged);
            }
        }    
    }
}
