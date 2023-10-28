using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
namespace BHSystem.Web.Module
{

    public static class MessageExtensions
    {
        /// <summary>
        /// hiện thông báo hỏi yes/no
        /// </summary>
        /// <param name="jsRuntime"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public static ValueTask<bool> Confirm(this IJSRuntime jsRuntime, string message)
        {
            return jsRuntime.InvokeAsync<bool>("confirm", message);
        }

        /// <summary>
        /// hiện thông báo
        /// </summary>
        /// <param name="jsRuntime"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public static ValueTask Message(this IJSRuntime jsRuntime, string message)
        {
            return jsRuntime.InvokeVoidAsync("alert", message);
        }

        /// <summary>
        /// hiện imput box
        /// </summary>
        /// <param name="jsRuntime"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public static ValueTask<string> Input(this IJSRuntime jsRuntime, string message)
        {
            return jsRuntime.InvokeAsync<string>("prompt", message);
        }

    }
}