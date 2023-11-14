using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using BHSytem.Models.Models;

namespace BHSytem.Models.Entities
{
    public class UserMessages : Auditable
    {
        [Key]
        [Column(Order = 1)]
        public int Message_Id { get; set; } // message gì

        [Key]
        [Column(Order = 2)]
        public int UserId { get; set; } // gửi cho ai

        public bool IsReaded { get; set; } // đã đọc => không hiện lên nữa

        [ForeignKey("Message_Id")]
        public Messages Messages { get; set; } // Đây là dùng để tham chiếu đến bảng Messages.

        [ForeignKey("UserId")]
        public Users Users { get; set; } // Đây là dùng để tham chiếu đến bảng Users.
    }
}
