using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using BHSytem.Models.Entities;
namespace BHSytem.Models.Models
{
    public class RoomModel : Auditable
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Vui lòng điền Tên phòng")]
        public string? Name { get; set; }
        public string? Status { get; set; }
        public string? Address { get; set; }
        [Range(1, int.MaxValue, ErrorMessage = "Vui lòng điền Chiều dài")]
        public decimal Length { get; set; }
        [Range(1, int.MaxValue, ErrorMessage = "Vui lòng điền Chiều rộng")]
        public decimal Width { get; set; }
        [Range(1, int.MaxValue, ErrorMessage = "Vui lòng điền Đơn giá")]
        public decimal Price { get; set; }
        public int Image_Id { get; set; }
        public int BHouseId { get; set; } // mã phòng
        public string? BHouseName { get; set; } // tên phòng
        public string? Phone { get; set; } // SĐT liên hệ
        public string? UserName { get; set; } // tên chủ phòng

        [Required(ErrorMessage = "Vui lòng điền Thông tin mô tả")]
        public string? Description { get; set; } // mô tả
        public List<ImagesDetailModel>? ListFile { get; set; }
        public DateTime? Date_Create { get; set; }
        public DateTime? Date_Update { get; set; }
        public string? File_Path { get; set; }
    }
}
