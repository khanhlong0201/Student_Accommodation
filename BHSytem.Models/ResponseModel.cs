using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BHSytem.Models
{
    public class RequestModel
    {
        public int UserId { get; set; }
        public string? Json { get; set; }
        public string? Type { get; set; }
    }
    public class ResponseModel
    {
        public int Status { get; set; }
        public string? Message { get; set; }
        public ResponseModel()
        {
            Status = -1;
            Message = string.Empty;
        }
        public ResponseModel(int status, string? message)
        {
            Status = status;
            Message = message;
        }
    }

    public class ResponseModel<T>
    {
        public int StatusCode { get; set; }
        public string? Message { get; set; }
        public T Data { get; set; }

        public ResponseModel()
        {
            StatusCode = 0;
            Message = string.Empty;
            Data = Activator.CreateInstance<T>();
        }
    }
}
