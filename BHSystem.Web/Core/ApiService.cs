using BHSytem.Models.Models;
using Blazored.Toast.Services;
using Microsoft.AspNetCore.Components.Forms;
using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace BHSystem.Web.Core
{
    public interface IApiService //gọi api
    {
        Task<string> GetData(string link, Dictionary<string, object>? pParams = null);

        Task<string> GetDataFromBody(string link, object? objData = null);

        Task<string> AddOrUpdateData(string link, object? objData = null);

        Task<string> DeleteData(string link, params object[] objRequestModel);
    }

    public class ApiService : IApiService
    {
        #region "Properties"

        private readonly IHttpClientFactory _factory;
        public readonly ILogger<ApiService> _logger;
        public readonly HttpClient _httpClient;
        public readonly IToastService _toastService;
        long maxFileSize = 134217728;
        #endregion "Properties"

        public ApiService(IHttpClientFactory factory, ILogger<ApiService> logger, IToastService toastService)
        {
            this._logger = logger;
            this._factory = factory;
            this._httpClient = factory.CreateClient("api");
            _toastService = toastService;
            //this.logger = logger;
        }

        /// <summary>
        /// Trả về chuỗi content sau khi call api -> qua bên controller parse to Object
        /// </summary>
        /// <param name="link">Enpoint API</param>
        /// <param name="objRequestModel">List query string</param>
        /// <returns></returns>
        public async Task<string> GetData(string link, Dictionary<string, object>? pParams = null)
        {
            string json = JsonConvert.SerializeObject(pParams);
            try
            {
                string queryPrams = "";
                if (pParams != null && pParams.Any()) queryPrams = "?" + string.Join("&", pParams.Select(m => $"{m.Key}={m.Value}"));
                var stringContext = new StringContent(json, UnicodeEncoding.UTF8, "application/json"); ;
                string uri = $"/api/{link}";
                string strResponse = await _httpClient.GetAsync(
                    String.Format(uri + $"{queryPrams}")
                    ).Result.Content.ReadAsStringAsync();
                Debug.Print(_httpClient.BaseAddress + String.Format(uri + $"{queryPrams}"));

                return strResponse;
            }
            catch (Exception objEx)
            {
                _logger.LogError(objEx, json);
                return JsonConvert.SerializeObject(new ResponseModel<object> { StatusCode = -1, Message = objEx.Message });
            }
        }

        /// <summary>
        /// Call api using method POST
        /// </summary>
        /// <param name="link"></param>
        /// <param name="objData"></param>
        /// <returns></returns>
        public async Task<string> GetDataFromBody(string link, object? objData = null)
        {
            string json = JsonConvert.SerializeObject(objData);
            try
            {
                var stringContent = new StringContent(json, UnicodeEncoding.UTF8, "application/json");
                string uri = "api/" + link;
                HttpResponseMessage httpResponse = await _httpClient.PostAsync(uri, stringContent);
                Debug.Print(_httpClient.BaseAddress + uri);
                Debug.Print(json);
                var content = await httpResponse.Content.ReadAsStringAsync();
                if (httpResponse.IsSuccessStatusCode) return content; // nếu APi trả về OK 200
                if (httpResponse.StatusCode == System.Net.HttpStatusCode.Unauthorized) // hết token
                {
                    _toastService.ShowInfo("Hết phiên đăng nhập!");
                    return "";
                }
                var oMessage = JsonConvert.DeserializeObject<ResponseModel>(content); // mã lỗi dưới API
                _toastService.ShowError($"{oMessage?.Message}");
                return "";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, json);
                _toastService.ShowError(ex.Message);
                return "";
            }
        }

        /// <summary>
        /// Xử lý Call API Method Post
        /// </summary>
        /// <param name="link"></param>
        /// <param name="objData"></param>
        /// <returns></returns>
        public async Task<string> AddOrUpdateData(string link, object? objData = null)
        {
            string json = JsonConvert.SerializeObject(objData);
            try
            {
                var stringContent = new StringContent(json, UnicodeEncoding.UTF8, "application/json");
                string uri = "api/" + link;
                HttpResponseMessage httpResponse = await _httpClient.PostAsync(uri, stringContent);
                Debug.Print(_httpClient.BaseAddress + uri);
                Debug.Print(json);
                var content = await httpResponse.Content.ReadAsStringAsync();
                if (httpResponse.IsSuccessStatusCode) return content; // nếu APi trả về OK 200
                if (httpResponse.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    _toastService.ShowInfo("Hết phiên đăng nhập!");
                    return "";
                }
                var oMessage = JsonConvert.DeserializeObject<ResponseModel>(content); // mã lỗi dưới API
                _toastService.ShowError($"{oMessage?.Message}");
                return "";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, json);
                _toastService.ShowError(ex.Message);
                return "";
            }
        }

        /// <summary>
        /// Xử lý call API method Delete
        /// </summary>
        /// <param name="link"></param>
        /// <param name="objRequestModel"></param>
        /// <returns></returns>
        public async Task<string> DeleteData(string link, params object[] objRequestModel)
        {
            string json = JsonConvert.SerializeObject(objRequestModel);
            try
            {
                string queryPrams = "";
                if (objRequestModel != null)
                {
                    for (int i = 0; i < objRequestModel.Count() - 1; i += 2)
                    {
                        queryPrams += $"{objRequestModel[i]}={objRequestModel[i + 1]}&";
                    }
                    queryPrams = "?" + queryPrams.TrimEnd('&');
                }
                var stringContent = new StringContent(json, UnicodeEncoding.UTF8, "application/json");
                string uri = "/api/" + link;
                string strResponse = await _httpClient.DeleteAsync(
                    String.Format(uri + $"{queryPrams}")
                    ).Result.Content.ReadAsStringAsync();
                Debug.Print(_httpClient.BaseAddress + String.Format(uri + $"{queryPrams}"));

                return strResponse;
            }
            catch (Exception objEx)
            {
                _logger.LogError(objEx, json);
                return JsonConvert.SerializeObject(new ResponseModel<object> { StatusCode = -1, Message = objEx.Message });
            }
        }

        /// <summary>
        /// Xử lý call api Upload file
        /// </summary>
        /// <param name="link"></param>
        /// <param name="lstIBrowserFiles"></param>
        /// <param name="strSubFolder"></param>
        /// <param name="strSubFolderProdLine"></param>
        /// <returns></returns>
        public async Task<string> UploadMultiFiles(string link, List<IBrowserFile> lstIBrowserFiles,
                string strSubFolder, string strSubFolderProdLine)
        {
            string json = JsonConvert.SerializeObject(lstIBrowserFiles);
            string strResponse = "";
            try
            {
                _httpClient.DefaultRequestHeaders.Accept.Clear();
                _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                using var content = new MultipartFormDataContent();
                content.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data");
                foreach (var file in lstIBrowserFiles)
                {
                    var fileContent = new StreamContent(file.OpenReadStream(maxFileSize));
                    fileContent.Headers.ContentType = new MediaTypeHeaderValue(file.ContentType);
                    content.Add(
                        content: fileContent,
                        name: "\"files\"",
                        fileName: file.Name);
                }
                string URL = $"/api/{link}?strSubFolder={strSubFolder}&strSubFolderProdLine={strSubFolderProdLine}";
                var objResponse = await _httpClient.PostAsync(URL, content);
                if (objResponse.IsSuccessStatusCode)
                {
                    // cho phép properties name trùng, phân biệt hoa thường
                    var options = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true,
                    };
                    strResponse = await objResponse.Content.ReadAsStringAsync();
                }

                Debug.Print(_httpClient.BaseAddress + URL);
                Debug.Print(json);
                return strResponse;
            }
            catch (Exception objEx)
            {
                _logger.LogError(objEx, json);
                return JsonConvert.SerializeObject(new ResponseModel<object> { StatusCode = -1, Message = objEx.Message });
            }
        }
    }
}