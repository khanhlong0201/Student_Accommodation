using BHSystem.Web.Constants;
using BHSystem.Web.Core;
using BHSystem.Web.Providers;
using BHSystem.Web.ViewModels;
using BHSytem.Models.Models;
using Blazored.LocalStorage;
using Blazored.Toast.Services;
using Microsoft.AspNetCore.Components.Authorization;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Net.Http.Headers;
using System.Text;

namespace BHSystem.Web.Services
{
    /// <summary>
    /// đặt thù cho User -> Login, Logout
    /// </summary>
    public interface ICliUserService
    {
        Task<string> LoginAsync(LoginViewModel request);
        Task<bool> UpdateAsync(string pJson, string pAction, int pUserId, string pKind);
        Task<List<UserModel>?> GetDataAsync();
        Task<bool> DeleteAsync(string pJson, int pUserId);
        Task LogoutAsync();
        Task<UserModel?> GetUserById(int pUserId);
    }
    public class CliUserService : ApiService, ICliUserService
    {
        private readonly AuthenticationStateProvider _authenticationStateProvider;
        public CliUserService(IHttpClientFactory factory, ILogger<ApiService> logger, IToastService toastService
            , ILocalStorageService localStorage, AuthenticationStateProvider authenticationStateProvider
            , BHDialogService bhDialogService, IWebHostEnvironment webHostEnvironment)
            : base(factory, logger, toastService, localStorage, bhDialogService, webHostEnvironment)
        {
            _authenticationStateProvider = authenticationStateProvider;
        }

        public async Task<string> LoginAsync(LoginViewModel request)
        {
            try
            {
                var loginRequest = new LoginViewModel();
                loginRequest.UserName = request.UserName;
                loginRequest.Password = EncryptHelper.Encrypt(request.Password + "");
                string jsonBody = JsonConvert.SerializeObject(loginRequest);
                HttpResponseMessage httpResponse = await _httpClient.PostAsync($"api/{EndpointConstants.URL_USER_LOGIN}", new StringContent(jsonBody, UnicodeEncoding.UTF8, "application/json"));
                Debug.Print(jsonBody);
                var content = await httpResponse.Content.ReadAsStringAsync();
                LoginResponseViewModel response = JsonConvert.DeserializeObject<LoginResponseViewModel>(content)!;
                if (!httpResponse.IsSuccessStatusCode) return response.Message + "";
                // save token
                if (await _localStorage.ContainKeyAsync("authToken")) await _localStorage.RemoveItemAsync("authToken");
                await _localStorage.SetItemAsync("authToken", response.Token);
                ((ApiAuthenticationStateProvider)_authenticationStateProvider).MarkUserAsAuthenticated($"{response!.FullName}");
                _httpClient.DefaultRequestHeaders.Add("UserId", $"{response!.UserId}");
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", response!.Token);
                return "";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Login");
                return ex.Message;
            }
        }

        public async Task<bool> UpdateAsync(string pJson, string pAction, int pUserId, string pKind)
        {
            try
            {
                RequestModel request = new RequestModel()
                {
                    Json = pJson,
                    Type = pAction,
                    UserId = pUserId,
                    Kind = pKind
                };
                var resString = await AddOrUpdateData(EndpointConstants.URL_USER_UPDATE, request, isAuth: true);
                if (!string.IsNullOrEmpty(resString)) return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "UpdateAsync");
                _toastService.ShowError(ex.Message);
            }
            return false;
        }

        public async Task<List<UserModel>?> GetDataAsync()
        {
            try
            {
                var resString = await GetData(EndpointConstants.URL_USER_GETALL);
                if (!string.IsNullOrEmpty(resString))
                {
                    var data = JsonConvert.DeserializeObject<List<UserModel>>(resString);
                    return data;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "UpdateAsync");
                _toastService.ShowError(ex.Message);
            }
            return default;
        }

        public async Task<bool> DeleteAsync(string pJson, int pUserId)
        {
            try
            {
                RequestModel request = new RequestModel()
                {
                    Json = pJson,
                    UserId = pUserId
                };
                var resString = await AddOrUpdateData(EndpointConstants.URL_USER_DELETE, request, isAuth: true);
                if (!string.IsNullOrEmpty(resString)) return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "UpdateAsync");
                _toastService.ShowError(ex.Message);
            }
            return false;
        }

        public async Task LogoutAsync()
        {
            await _localStorage.RemoveItemAsync("authToken");
            ((ApiAuthenticationStateProvider)_authenticationStateProvider).MarkUserAsLoggedOut();
        }   
        
        public async Task<UserModel?> GetUserById(int pUserId)
        {
            try
            {
                Dictionary<string, object> pParams = new Dictionary<string, object>()
                {
                    {"pUserId", $"{pUserId}"}
                };
                var resString = await GetData(EndpointConstants.URL_USER_BY_ID, pParams, isAuth: true);
                if (!string.IsNullOrEmpty(resString))
                {
                    var data = JsonConvert.DeserializeObject<UserModel>(resString);
                    return data;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "UpdateAsync");
                _toastService.ShowError(ex.Message);
            }
            return default;
        }    


    }
}
