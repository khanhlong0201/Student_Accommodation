using BHSystem.Web.Services;
using BHSytem.Models.Models;
using Blazored.LocalStorage;
using Blazored.Toast.Services;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Org.BouncyCastle.Asn1.Ocsp;
using System;
using System.Diagnostics;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace BHSystem.Web.Core
{
    public interface IApiService //gọi api
    {
        Task<string> GetData(string link, Dictionary<string, object>? pParams = null, bool isAuth = false);

        Task<string> GetDataFromBody(string link, object? objData = null, bool isAuth = false);

        Task<string> AddOrUpdateData(string link, object? objData = null, bool isAuth = false);

        Task<string> DeleteData(string link, params object[] objRequestModel);
        Task<string> UploadMultiFiles(string link, List<IBrowserFile> lstIBrowserFiles);
    }

    public class ApiService : IApiService
    {
        #region "Properties"

        private readonly IHttpClientFactory _factory;
        public readonly ILogger<ApiService> _logger;
        public readonly HttpClient _httpClient;
        public readonly IToastService _toastService;
        private readonly BHDialogService _bhDialogService;
        public readonly ILocalStorageService _localStorage;
        private readonly IWebHostEnvironment _webHostEnvironment;
        long maxFileSize = 134217728;
        #endregion "Properties"

        public ApiService(IHttpClientFactory factory, ILogger<ApiService> logger, IToastService toastService
            , ILocalStorageService localStorage, BHDialogService bhDialogService, IWebHostEnvironment webHostEnvironment)
        {
            _logger = logger;
            _factory = factory;
            _httpClient = factory.CreateClient("api");
            _toastService = toastService;
            _bhDialogService = bhDialogService;
            _localStorage = localStorage;
            _webHostEnvironment = webHostEnvironment;
        }

        /// <summary>
        /// Trả về chuỗi content sau khi call api -> qua bên controller parse to Object
        /// </summary>
        /// <param name="link">Enpoint API</param>
        /// <param name="objRequestModel">List query string</param>
        /// <returns></returns>
        public async Task<string> GetData(string link, Dictionary<string, object>? pParams = null, bool isAuth = false)
        {
            string json = JsonConvert.SerializeObject(pParams);
            try
            {
                if (isAuth) await setTokenBearer();
                string queryPrams = "";
                if (pParams != null && pParams.Any()) queryPrams = "?" + string.Join("&", pParams.Select(m => $"{m.Key}={m.Value}"));
                var stringContext = new StringContent(json, UnicodeEncoding.UTF8, "application/json"); ;
                string uri = $"/api/{link}";
                string strResponse = await _httpClient.GetAsync(
                    String.Format(uri + $"{queryPrams}")
                    ).Result.Content.ReadAsStringAsync();
                HttpResponseMessage httpResponse = await _httpClient.GetAsync(string.Format(uri + $"{queryPrams}"));
                var content = await httpResponse.Content.ReadAsStringAsync();
                if (httpResponse.IsSuccessStatusCode) return content; // nếu APi trả về OK 200
                if (httpResponse.StatusCode == System.Net.HttpStatusCode.Unauthorized) // hết token
                {
                    _bhDialogService.ShowDialog();
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
        /// Call api using method POST
        /// </summary>
        /// <param name="link"></param>
        /// <param name="objData"></param>
        /// <returns></returns>
        public async Task<string> GetDataFromBody(string link, object? objData = null, bool isAuth = false)
        {
            string json = JsonConvert.SerializeObject(objData);
            try
            {
                if (isAuth) await setTokenBearer();
                var stringContent = new StringContent(json, UnicodeEncoding.UTF8, "application/json");
                string uri = "api/" + link;
                HttpResponseMessage httpResponse = await _httpClient.PostAsync(uri, stringContent);
                Debug.Print(_httpClient.BaseAddress + uri);
                Debug.Print(json);
                var content = await httpResponse.Content.ReadAsStringAsync();
                if (httpResponse.IsSuccessStatusCode) return content; // nếu APi trả về OK 200
                if (httpResponse.StatusCode == System.Net.HttpStatusCode.Unauthorized) // hết token
                {
                    _bhDialogService.ShowDialog();
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
        public async Task<string> AddOrUpdateData(string link, object? objData = null, bool isAuth = false)
        {
            string json = JsonConvert.SerializeObject(objData);
            try
            {
                if (isAuth) await setTokenBearer();

                var stringContent = new StringContent(json, UnicodeEncoding.UTF8, "application/json");
                string uri = "api/" + link;
                HttpResponseMessage httpResponse = await _httpClient.PostAsync(uri, stringContent);
                Debug.Print(_httpClient.BaseAddress + uri);
                Debug.Print(json);
                var content = await httpResponse.Content.ReadAsStringAsync();
                if (httpResponse.IsSuccessStatusCode) return content; // nếu APi trả về OK 200
                if (httpResponse.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    _bhDialogService.ShowDialog();
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

        /// <summary>
        /// set token cho cacs request
        /// </summary>
        private async Task setTokenBearer()
        {
            // lấy ra token -> add vào Header Authorization Bearer Token
            var savedToken = await _localStorage.GetItemAsync<string>("authToken");
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", savedToken);
        }

        /// <summary>
        /// upload multifile
        /// </summary>
        /// <param name="link"></param>
        /// <param name="lstIBrowserFiles"></param>
        /// <returns></returns>
        public async Task<string> UploadMultiFiles(string link, List<IBrowserFile> lstIBrowserFiles)
        {
            List<string> listFilePath = new List<string>();
            try
            {
                Console.WriteLine(System.Net.ServicePointManager.DefaultConnectionLimit);
                _httpClient.DefaultRequestHeaders.Accept.Clear();
                _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                using var content = new MultipartFormDataContent();
                content.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data");
                var rootFolder = Path.Combine(_webHostEnvironment.WebRootPath, "Upload", "TEMP");
                //tạo thư mục
                if (!Directory.Exists(rootFolder)) Directory.CreateDirectory(rootFolder);
                string strFileFullName = string.Empty;
                foreach (var file in lstIBrowserFiles)
                {
                    string fileNameNew = $"{Guid.NewGuid()}---{file.Name}";
                    strFileFullName = Path.Combine(rootFolder, fileNameNew);
                    await using FileStream fs = new(strFileFullName, FileMode.Create);
                    await file.OpenReadStream(long.MaxValue).CopyToAsync(fs); // lưu vào www tạm để đọc file
                    listFilePath.Add(strFileFullName); // khi lưu vào www ok -> lưu vào list tạm để finally -> remove ra
                    await fs.FlushAsync();
                    await fs.DisposeAsync();
                    content.Add(new StreamContent(File.OpenRead(@$"{strFileFullName}")), name: "\"files\"", fileName: fileNameNew);
                }

                string sUrl = $"api/{link}";
                HttpResponseMessage httpResponse = await _httpClient.PostAsync(sUrl, content);
                var resContent = await httpResponse.Content.ReadAsStringAsync();
                if (httpResponse.IsSuccessStatusCode) return resContent; // nếu APi trả về OK 200
                if (httpResponse.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    _bhDialogService.ShowDialog();
                    _toastService.ShowInfo("Hết phiên đăng nhập!");
                    return "";
                }
                var oMessage = JsonConvert.DeserializeObject<ResponseModel>(resContent); // mã lỗi dưới API
                _toastService.ShowError($"{oMessage?.Message}");
                return "";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "UploadMultiFiles");
                _toastService.ShowError(ex.Message);
                return "";
            }
            finally
            {
                // xóa các ảnh được lưu vào folder tạm
                listFilePath.ForEach(item => { if (File.Exists(@$"{item}")) File.Delete(item); });
            }
        }
    }
}