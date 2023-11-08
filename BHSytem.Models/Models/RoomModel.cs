using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using BHSytem.Models.Entities;
namespace BHSytem.Models.Models
{
    public class RoomModel : Auditable
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Status { get; set; }
        public string? Address { get; set; }
        public decimal Length { get; set; }
        public decimal Width { get; set; }
        public decimal Price { get; set; }
        public int Image_Id { get; set; }
        public int BHouseId { get; set; } // mã phòng
        public string? BHouseName { get; set; } // tên phòng
        public string? Phone { get; set; } // SĐT liên hệ
        public string? UserName { get; set; } // tên chủ phòng
        public string? Description { get; set; } // mô tả
        public List<ImagesDetailModel>? ListFile { get; set; }
    }
}
