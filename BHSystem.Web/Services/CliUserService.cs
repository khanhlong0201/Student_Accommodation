using BHSystem.Web.Constants;
using BHSystem.Web.Core;
using BHSystem.Web.Providers;
using BHSytem.Models.Models;
using Blazored.Toast.Services;
using Newtonsoft.Json;
using System.Net.Http.Headers;

namespace BHSystem.Web.Services
{
    /// <summary>
    /// đặt thù cho User -> Login, Logout
    /// </summary>
    public interface ICliUserService
    {
        Task<UserModel?> LoginAsync(UserModel request);
        Task<bool> UpdateAsync(string pJson, string pAction);
    }
    public class CliUserService : ApiService, ICliUserService
    {
        public CliUserService(IHttpClientFactory factory, ILogger<ApiService> logger, IToastService toastService) : base(factory, logger, toastService){ }

        public async Task<UserModel?> LoginAsync(UserModel request)
        {
            var loginRequest = new UserModel();
            loginRequest.UserName = request.UserName;
            loginRequest.Password = EncryptHelper.Encrypt(request.Password + "");
            var resString = await GetDataFromBody(EndpointConstants.URL_USER_LOGIN, loginRequest);
            if (string.IsNullOrEmpty(resString)) return default;
            var oUser = JsonConvert.DeserializeObject<UserModel>(resString);
            return oUser;
        }

        public async Task<bool> UpdateAsync(string pJson, string pAction)
        {
            try
            {
                RequestModel request = new RequestModel()
                {
                    Json = pJson,
                    Type = pAction
                };
                //var savedToken = await _localStorage.GetItemAsync<string>("authToken");
                //_httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", savedToken);
                var resString = await AddOrUpdateData(EndpointConstants.URL_USER_UPDATE, request);
                if (!string.IsNullOrEmpty(resString)) return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "UpdateAsync");
                _toastService.ShowError(ex.Message);
            }
            return false;
        }


    }
}
