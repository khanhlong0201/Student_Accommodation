﻿using BHSystem.Web.Constants;
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
    public partial class ApprovePage
    {
        [Inject] private ILogger<UserPage>? _logger { get; init; }
        [Inject] private ILoadingCore? _spinner { get; set; }
        [Inject] private IToastService? _toastService { get; set; }
        [Inject] private IApiService? _apiService { get; set; }
        [Inject] private IConfiguration? _configuration { get; set; }
        [Inject] NavigationManager? _navigationManager { get; set; }
        public List<BookingModel>? ListBookingWaitting { get; set; }
        public List<BookingModel>? ListBookingAll { get; set; }
        public IEnumerable<BookingModel>? SelectedBookingWaitting { get; set; } = new List<BookingModel>();
        public BHouseModel BookingUpdate { get; set; } = new BHouseModel();
        public BHConfirm? _rDialogs { get; set; }

        [CascadingParameter]
        private int pUserId { get; set; } // giá trị từ MainLayout
        #region
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if(firstRender)
            {
                try
                {
                    await showLoading();
                    await getDataBooking();
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

        /// <summary>
        /// lấy danh sách booking
        /// </summary>
        /// <returns></returns>
        private async Task getDataBooking(string type = "")
        {
            // Gọi hàm và truyền giá trị cho pParams
            Dictionary<string, object> parameters = new Dictionary<string, object>
            {
                { "status", type }
            };
            
            ListBookingWaitting = new List<BookingModel>();
            string resString = await _apiService!.GetData("BHouses/GetAll", parameters);
            if (!string.IsNullOrEmpty(resString))
            {
                if(type+""=="Chờ xử lý") ListBookingWaitting = JsonConvert.DeserializeObject<List<BookingModel>>(resString);
                else ListBookingAll = JsonConvert.DeserializeObject<List<BookingModel>>(resString);
            }
        }
        #endregion
        #region "Protected Functions"
        /// <summary>
        /// load dữ liệu
        /// </summary>
        /// <returns></returns>
        protected async void ReLoadDataHandler(string Wait ="")
        {
            try
            {
                await showLoading();
                await getDataBooking(Wait);
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
        /// Từ chối 1 hoặc nhiều booking
        /// </summary>
        /// <returns></returns>
        protected async Task ConfirmRefuseHandler()
        {
            if (SelectedBookingWaitting == null || !SelectedBookingWaitting.Any())
            {
                _toastService!.ShowWarning("Vui lòng chọn dòng để từ chối");
                return;
            }
            var confirm = await _rDialogs!.ConfirmAsync("Bạn có chắc muốn từ chối các dòng được chọn? ");
            if (confirm)
            {
                try
                {
                    await showLoading();
                    var oDelete = SelectedBookingWaitting.Select(m => new { m.Id });
                    RequestModel request = new RequestModel()
                    {
                        Json = JsonConvert.SerializeObject(oDelete),
                        UserId = pUserId
                    };
                    string resString = await _apiService!.AddOrUpdateData("BHouses/Delete", request);
                    if (!string.IsNullOrEmpty(resString))
                    {
                        _toastService!.ShowSuccess($"Đã từ chối thông tin các phòng được chọn.");
                        await getDataBooking();
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

        #endregion
    }
}
