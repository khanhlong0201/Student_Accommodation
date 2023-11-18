using BHSystem.Web.Constants;
using BHSystem.Web.Core;
using BHSystem.Web.Providers;
using BHSytem.Models.Models;
using Blazored.Toast.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.SignalR.Client;
using Newtonsoft.Json;
using NPOI.POIFS.Crypt.Dsig;

namespace BHSystem.Web.Shared
{
    public partial class MainLayout : IDisposable
    {
        [Inject] AuthenticationStateProvider? _authenticationStateProvider { get; set; }
        [Inject] private ILoadingCore? _spinner { get; set; }
        [Inject] private ILogger<MainLayout>? _logger { get; init; }
        [Inject] private IToastService? _toastService { get; set; }
        [Inject] private IApiService? _apiService { get; set; }
        [Inject] IConfiguration? _configuration { get; set; }
        [Inject] NavigationManager? _navigationManager { get; set; }

        public string breadcumb { get; set; } = "";
        public DateTime dt { get; set; } = DateTime.Now;
        public string FullName { get; set; } = "TTI";
        public string UserName { get; set; } = "";
        public int UserId { get; set; } = -1;
        public bool IsSupperAdmin { get; set; } = false;
        public List<MenuModel>? ListMenus { get; set; }

        private HubConnection? hubConnection;
        public List<MessageModel> ListMeassges { get; set; } = new List<MessageModel>();
        public int CountMessages { get; set; } = 0;

        // Event Call back -> đến danh sách message
        EventCallback<List<MessageModel>> ListMeassgesHandler =>
        EventCallback.Factory.Create(this, (Action<List<MessageModel>>)NotifyMessage);

        /// <summary>
        /// callback lại danh sách message khi người dùng từ page theo dõi danh sách thông báo
        /// đánh dấu đã đọc -> call back lại tg này
        /// </summary>
        /// <param name="_breadcrumbs"></param>
        private void NotifyMessage(List<MessageModel> _breadcrumbs)
        {
            ListMeassges = _breadcrumbs;
            CountMessages = ListMeassges?.Count() ?? 0;
        }    


        protected override async Task  OnInitializedAsync()
        {
            //await base.OnInitializedAsync();
            try
            {
                var oUser = await ((Providers.ApiAuthenticationStateProvider)_authenticationStateProvider!).GetAuthenticationStateAsync();
                if (oUser != null)
                {
                    UserName = oUser.User.Claims.FirstOrDefault(m => m.Type == "UserName")?.Value + "";
                    IsSupperAdmin = oUser.User.Claims.FirstOrDefault(m => m.Type == "IsAdmin")?.Value + "" == "Admin";
                    FullName = oUser.User.Claims.FirstOrDefault(m => m.Type == "FullName")?.Value + "";
                    UserId = int.Parse(oUser.User.Claims.FirstOrDefault(m => m.Type == "UserId")?.Value + "");
                    try
                    {
                        await showLoading(true);
                        await getMenu();
                        await getUnReadMessageByUser();
                        string url = _configuration!.GetSection("appSettings:ApiUrl").Value + "Signalhub";
                        hubConnection = new HubConnectionBuilder()
                        .WithUrl(url, options =>
                        {
                            options.Headers.Add("UserName", $"{UserId}"); // kết nối user

                        }).Build();
                        hubConnection.On<MessageModel>("ReceiveMessage", async (incomingMess) =>
                        {
                            _toastService!.ShowInfo($"{incomingMess.Message}");
                            await getUnReadMessageByUser();
                            _ = InvokeAsync(StateHasChanged);
                        });

                        await hubConnection.StartAsync();
                    }
                    catch (Exception) { }
                    finally
                    {
                        await showLoading(false);
                    }

                }
            }
            catch(Exception){}

        }

        #region "Private Functions"
        private async Task showLoading(bool isShow = true)
        {
            if (isShow) { _spinner!.Show(); await Task.Yield(); }
            else _spinner!.Hide();
        }

        private async Task getMenu()
        {
            ListMenus = new List<MenuModel>();
            Dictionary<string, object> pParams = new Dictionary<string, object>()
            {
                {"pUserId", $"{UserId}"}
            };
            string resString = await _apiService!.GetData(EndpointConstants.URL_MENU_GET_BY_USER, pParams);
            if (!string.IsNullOrEmpty(resString)) ListMenus = JsonConvert.DeserializeObject<List<MenuModel>>(resString);
        }

        /// <summary>
        /// lấy danh sách message theo user
        /// </summary>
        /// <returns></returns>
        private async Task getUnReadMessageByUser()
        {
            ListMeassges = new List<MessageModel>();
            Dictionary<string, object> pParams = new Dictionary<string, object>()
            {
                {"pUserId", $"{UserId}"},
                {"pIsAll", $"{false}"}
            };
            string resString = await _apiService!.GetData(EndpointConstants.URL_MESSAGE_BY_USER, pParams);
            if (!string.IsNullOrEmpty(resString)) ListMeassges = JsonConvert.DeserializeObject<List<MessageModel>>(resString);
            CountMessages = ListMeassges?.Count() ?? 0;
        }

        public void Dispose() => hubConnection?.DisposeAsync();
        #endregion

        protected void Navigato(string link) => _navigationManager!.NavigateTo(link);

        protected async void ReadMessageHander(MessageModel item)
        {
            try
            {
                await showLoading();
                RequestModel request = new RequestModel()
                {
                    Json = JsonConvert.SerializeObject(item),
                    UserId = UserId,
                };
                string resString = await _apiService!.AddOrUpdateData(EndpointConstants.URL_MESSAGE_UPDATE_ISREAD, request);
                if (!string.IsNullOrEmpty(resString))
                {
                    await getUnReadMessageByUser();
                    _navigationManager!.NavigateTo(item.Type == "Booking" ? "/admin/approve-booking" : "/admin/approve-room");
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

        protected void NavigatoEncrypt()
        {
            Dictionary<string, string> pParams = new Dictionary<string, string>
            {
                { "UserId", $"{UserId}" },
            };
            string key = EncryptHelper.Encrypt(JsonConvert.SerializeObject(pParams)); // mã hóa key phân quyền
            _navigationManager!.NavigateTo($"/admin/info-user?key={key}");
        }
    }


}
