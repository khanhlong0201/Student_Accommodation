using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BHSytem.Models.Models
{
    public class RequestModel
    {
        public int UserId { get; set; }
        public string? Json { get; set; }
        public string? Type { get; set; }
        public string? Json_Detail { get; set; }
        public string? Kind { get; set; }
    }
    public class ResponseModel
    {
        public int StatusCode { get; set; }
        public string? Message { get; set; }
        public ResponseModel()
        {
            StatusCode = -1;
            Message = string.Empty;
        }
        public ResponseModel(int status, string? message)
        {
            StatusCode = status;
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
            Data = Activator.CreateInstance<T>(); //longtran Tạo một thể hiện
        }
    }

    public class CliResponseModel<T> where T : class
    {
        public PaginationModel Pagination { get; set; }
        public List<T> ListData { get; set; }

        public CliResponseModel()
        {
            Pagination = new PaginationModel();
            ListData = new List<T>();
        }
    }
}
