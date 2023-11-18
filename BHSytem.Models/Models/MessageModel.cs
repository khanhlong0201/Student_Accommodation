using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BHSytem.Models.Models
{
    public class MessageModel : Auditable
    {
        public int Id { get; set; }
        public string? Type { get; set; }
        public string? Message { get; set; } // thông báo về cái gì
        public string? JText { get; set; } // json lưu thông tin cần parse
        public int UserId { get; set; } // gửi cho ai
        public bool IsReaded { get; set; } // đã đọc => không hiện lên nữa
    }
}
