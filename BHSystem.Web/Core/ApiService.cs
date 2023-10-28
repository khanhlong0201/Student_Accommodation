using System.Diagnostics;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Components.Forms;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using System.Web;
using BHSytem.Models.Models;
using Microsoft.JSInterop;

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
        IHttpClientFactory factory;
        HttpClient request;
        IConfiguration m_config;
        ILogger<ApiService> logger;
        long maxFileSize = 134217728;
        private readonly string API_KEY_NAME = "ApiKey";
        private readonly string API_KEY_VALUE = "123"; // lấy từ settings
        #endregion "Properties"

        public ApiService(IHttpClientFactory factory, IConfiguration config, ILogger<ApiService> logger)
        {
            this.factory = factory;
            this.request = factory.CreateClient("api");
            this.request.DefaultRequestHeaders.Add(API_KEY_NAME, API_KEY_VALUE);
            //this.logger = logger;
            m_config = config;
            this.logger = logger;
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
                string uri = $"/api/vi/{link}";
                string strResponse = await request.GetAsync(
                    String.Format(uri + $"{queryPrams}")
                    ).Result.Content.ReadAsStringAsync();
                Debug.Print(request.BaseAddress + String.Format(uri + $"{queryPrams}"));

                return strResponse;
            }
            catch (Exception objEx)
            {
                this.logger.LogError(objEx, json);
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
                string uri = "/api/vi/" + link;
                string strResponse = await request.PostAsync(uri, stringContent).Result.Content.ReadAsStringAsync();
                Debug.Print(request.BaseAddress + uri);
                Debug.Print(json);
                return strResponse;
            }
            catch (Exception objEx)
            {
                this.logger.LogError(objEx, json);
                return JsonConvert.SerializeObject(new ResponseModel<object> { StatusCode = -1, Message = objEx.Message });
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
                string uri = "/api/vi/" + link;
                string strResponse = await request.PostAsync(uri, stringContent).Result.Content.ReadAsStringAsync();
                Debug.Print(request.BaseAddress + uri);
                Debug.Print(json);
                return strResponse;
            }
            catch (Exception objEx)
            {
                this.logger.LogError(objEx, json);
                return JsonConvert.SerializeObject(new ResponseModel<object> { StatusCode = -1, Message = objEx.Message });
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
                string uri = "/api/vi/" + link;
                string strResponse = await request.DeleteAsync(
                    String.Format(uri + $"{queryPrams}")
                    ).Result.Content.ReadAsStringAsync();
                Debug.Print(request.BaseAddress + String.Format(uri + $"{queryPrams}"));

                return strResponse;
            }
            catch (Exception objEx)
            {
                this.logger.LogError(objEx, json);
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
                request.DefaultRequestHeaders.Accept.Clear();
                request.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
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
                string URL = $"/api/vi/{link}?strSubFolder={strSubFolder}&strSubFolderProdLine={strSubFolderProdLine}";
                var objResponse = await request.PostAsync(URL, content);
                if (objResponse.IsSuccessStatusCode)
                {
                    // cho phép properties name trùng, phân biệt hoa thường
                    var options = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true,
                    };
                    strResponse = await objResponse.Content.ReadAsStringAsync();
                }


                Debug.Print(request.BaseAddress + URL);
                Debug.Print(json);
                return strResponse;
            }
            catch (Exception objEx)
            {
                this.logger.LogError(objEx, json);
                return JsonConvert.SerializeObject(new ResponseModel<object> { StatusCode = -1, Message = objEx.Message });
            }
        }
    }
}